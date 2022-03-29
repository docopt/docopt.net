// Licensed under terms of MIT license (see LICENSE-MIT)
// Copyright 2021 Atif Aziz, Dinh Doan Van Bien

#nullable enable

namespace DocoptNet.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Leaves = ReadOnlyList<LeafPattern>;

    static partial class GeneratedSourceModule
    {
        public static IParser<T>.IResult Parse<T>(string doc, string usage, IEnumerable<string> args, List<Option> options,
                                                  ParseFlags flags, string? version,
                                                  Func<Leaves, IParser<T>.IResult> resultSelector)
        {
            var tokens = Tokens.From(args);
            Leaves arguments;
            try
            {
                var optionsFirst = (flags & ParseFlags.OptionsFirst) != ParseFlags.None;
                arguments = Docopt.ParseArgv(tokens, options, optionsFirst).AsReadOnly();
            }
            catch (DocoptInputErrorException e)
            {
                return new ParseInputErrorResult<T>(e.Message, usage);
            }
            var help = (flags & ParseFlags.DisableHelp) == ParseFlags.None;
            if (help && arguments.Any(o => o is { Name: "-h" or "--help", Value.IsTrue: true }))
                return new ParseHelpResult<T>(doc);
            if (version is { } someVersion && arguments.Any(o => o is { Name: "--version", Value.IsTrue: true }))
                return new ParseVersionResult<T>(someVersion);
            return resultSelector(arguments);
        }

        public static IBaselineParser<T> CreateParser<T>(string doc, Func<IEnumerable<string>, ParseFlags, string?, IParser<T>.IResult> parseFunction) =>
            new Parser<T>(doc, ArgsParseOptions.Default, null, (_, args, flags, version) => parseFunction(args, flags, version));

        public static IParser<T>.IResult CreateArgumentsResult<T>(T args) =>
            new ArgumentsResult<T>(args);

        public static IParser<T>.IResult CreateInputErrorResult<T>(string error, string usage) =>
            new ParseInputErrorResult<T>(error, usage);
    }
}
