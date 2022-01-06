#nullable enable

namespace DocoptNet.Internals
{
    using System.Collections.Generic;
    using System.Linq;
    using Leaves = ReadOnlyList<LeafPattern>;

    static partial class GeneratedSourceModule
    {
        public static Leaves ParseArgv(string doc, IEnumerable<string> args, List<Option> options,
                                       bool optionsFirst, bool help, object? version)
        {
            var tokens = Tokens.From(args);
            var arguments = Docopt.ParseArgv(tokens, options, optionsFirst).AsReadOnly();
            if (help && arguments.Any(o => o is { Name: "-h" or "--help", Value: { IsTrue: true } }))
                throw new DocoptExitException(doc);
            if (version is not null && arguments.Any(o => o is { Name: "--version", Value: { IsTrue: true } }))
                throw new DocoptExitException(version.ToString());
            return arguments;
        }

        public static Leaves GetSuccessfulCollection(RequiredMatcher a, string usage) =>
            !a.Result || a.Left.Count > 0
            ? throw new DocoptInputErrorException(usage)
            : a.Collected;
    }
}
