#nullable enable

namespace DocoptNet.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Leaves = ReadOnlyList<LeafPattern>;

    static partial class GeneratedSourceModule
    {
        public static IParseResult<T> Parse<T>(string doc, string usage, IEnumerable<string> args, List<Option> options,
                                               Docopt.ParseFlags flags, string? version,
                                               Func<Leaves, IParseResult<T>> resultSelector)
        {
            var tokens = Tokens.From(args);
            Leaves arguments;
            try
            {
                var optionsFirst = (flags & Docopt.ParseFlags.OptionsFirst) != Docopt.ParseFlags.None;
                arguments = Docopt.ParseArgv(tokens, options, optionsFirst).AsReadOnly();
            }
            catch (DocoptInputErrorException e)
            {
                return new ParseElseResult<T>(new InputErrorResult(e.Message, usage));
            }
            var help = (flags & Docopt.ParseFlags.DisableHelp) == Docopt.ParseFlags.None;
            if (help && arguments.Any(o => o is { Name: "-h" or "--help", Value.IsTrue: true }))
                return new ParseElseResult<T>(new HelpResult(doc));
            if (version is {} someVersion && arguments.Any(o => o is { Name: "--version", Value.IsTrue: true }))
                return new ParseElseResult<T>(new VersionResult(someVersion));
            return resultSelector(arguments);
        }

        public static Leaves ParseArgv(string doc, IEnumerable<string> args, List<Option> options,
                                       bool optionsFirst, bool help, object? version)
        {
            var tokens = Tokens.From(args);
            var arguments = Docopt.ParseArgv(tokens, options, optionsFirst).AsReadOnly();
            if (help && arguments.Any(o => o is { Name: "-h" or "--help", Value.IsTrue: true }))
                throw new DocoptExitException(doc);
            if (version is not null && arguments.Any(o => o is { Name: "--version", Value.IsTrue: true }))
                throw new DocoptExitException(version.ToString());
            return arguments;
        }
    }
}
