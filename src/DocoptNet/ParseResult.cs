#nullable enable

namespace DocoptNet
{
    using System;
    using System.Collections.Generic;

    partial interface IArgumentsResult<out T>
    {
        public T Arguments { get; }
    }

    partial interface IInputErrorResult
    {
        public string Error { get; }
        public string Usage { get; }
    }

    partial interface IHelpResult
    {
        string Help { get; }
    }

    partial interface IVersionResult
    {
        string Version { get; }
    }

    sealed class ArgumentsResult<T> :
        IArgumentsResult<T>,
        IParser<T>.IResult, IHelpFeaturingParser<T>.IResult,
        IVersionFeaturingParser<T>.IResult,
        IBaselineParser<T>.IResult
    {
        public ArgumentsResult(T args) => Arguments = args;

        public T Arguments { get; }

        public override string ToString() => Arguments?.ToString() ?? string.Empty;

        TResult IParser<T>.IResult.Match<TResult>(Func<T, TResult> args,
                                                  Func<IHelpResult, TResult> help,
                                                  Func<IVersionResult, TResult> version,
                                                  Func<IInputErrorResult, TResult> error) =>
            args(Arguments);

        TResult IParser<T>.IResult.Match<TResult>(Func<T, IParser<T>.IResult, TResult> args,
                                                  Func<IHelpResult, IParser<T>.IResult, TResult> help,
                                                  Func<IVersionResult, IParser<T>.IResult, TResult> version,
                                                  Func<IInputErrorResult, IParser<T>.IResult, TResult> error) =>
            args(Arguments, this);

        TResult IHelpFeaturingParser<T>.IResult.Match<TResult>(Func<T, TResult> args,
                                                               Func<IHelpResult, TResult> help,
                                                               Func<IInputErrorResult, TResult> error) =>
            args(Arguments);

        TResult IVersionFeaturingParser<T>.IResult.Match<TResult>(Func<T, TResult> args,
                                                                  Func<IVersionResult, TResult> version,
                                                                  Func<IInputErrorResult, TResult> error) =>
            args(Arguments);

        TResult IBaselineParser<T>.IResult.Match<TResult>(Func<T, TResult> args,
                                                          Func<IInputErrorResult, TResult> error) =>
            args(Arguments);
    }

    sealed class ParseHelpResult<T> :
        IHelpResult,
        IParser<T>.IResult,
        IHelpFeaturingParser<T>.IResult
    {
        public ParseHelpResult(string help) => Help = help;

        public string Help { get; }

        TResult IParser<T>.IResult.Match<TResult>(Func<T, TResult> args,
                                                  Func<IHelpResult, TResult> help,
                                                  Func<IVersionResult, TResult> version,
                                                  Func<IInputErrorResult, TResult> error) =>
            help(this);

        TResult IParser<T>.IResult.Match<TResult>(Func<T, IParser<T>.IResult, TResult> args,
                                                  Func<IHelpResult, IParser<T>.IResult, TResult> help,
                                                  Func<IVersionResult, IParser<T>.IResult, TResult> version,
                                                  Func<IInputErrorResult, IParser<T>.IResult, TResult> error) =>
            help(this, this);

        TResult IHelpFeaturingParser<T>.IResult.Match<TResult>(Func<T, TResult> args,
                                                               Func<IHelpResult, TResult> help,
                                                               Func<IInputErrorResult, TResult> error) =>
            help(this);
    }

    sealed class ParseVersionResult<T> :
        IVersionResult,
        IParser<T>.IResult,
        IVersionFeaturingParser<T>.IResult
    {
        public ParseVersionResult(string version) => Version = version;

        public string Version { get; }

        TResult IParser<T>.IResult.Match<TResult>(Func<T, TResult> args,
                                                  Func<IHelpResult, TResult> help,
                                                  Func<IVersionResult, TResult> version,
                                                  Func<IInputErrorResult, TResult> error) =>
            version(this);

        TResult IParser<T>.IResult.Match<TResult>(Func<T, IParser<T>.IResult, TResult> args,
                                                  Func<IHelpResult, IParser<T>.IResult, TResult> help,
                                                  Func<IVersionResult, IParser<T>.IResult, TResult> version,
                                                  Func<IInputErrorResult, IParser<T>.IResult, TResult> error) =>
            version(this, this);

        TResult IVersionFeaturingParser<T>.IResult.Match<TResult>(Func<T, TResult> args,
                                                                  Func<IVersionResult, TResult> version,
                                                                  Func<IInputErrorResult, TResult> error) =>
            version(this);
    }

    sealed class ParseInputErrorResult<T> :
        IInputErrorResult,
        IParser<T>.IResult,
        IHelpFeaturingParser<T>.IResult,
        IVersionFeaturingParser<T>.IResult,
        IBaselineParser<T>.IResult
    {
        public ParseInputErrorResult(string error, string usage) => (Error, Usage) = (error, usage);

        public string Error { get; }
        public string Usage { get; }

        TResult IParser<T>.IResult.Match<TResult>(Func<T, TResult> args,
                                                  Func<IHelpResult, TResult> help,
                                                  Func<IVersionResult, TResult> version,
                                                  Func<IInputErrorResult, TResult> error) =>
            error(this);

        TResult IParser<T>.IResult.Match<TResult>(Func<T, IParser<T>.IResult, TResult> args,
                                                  Func<IHelpResult, IParser<T>.IResult, TResult> help,
                                                  Func<IVersionResult, IParser<T>.IResult, TResult> version,
                                                  Func<IInputErrorResult, IParser<T>.IResult, TResult> error) =>
            error(this, this);

        TResult IHelpFeaturingParser<T>.IResult.Match<TResult>(Func<T, TResult> args,
                                                               Func<IHelpResult, TResult> help,
                                                               Func<IInputErrorResult, TResult> error) =>
            error(this);

        TResult IVersionFeaturingParser<T>.IResult.Match<TResult>(Func<T, TResult> args,
                                                                  Func<IVersionResult, TResult> version,
                                                                  Func<IInputErrorResult, TResult> error) =>
            error(this);

        TResult IBaselineParser<T>.IResult.Match<TResult>(Func<T, TResult> args,
                                                          Func<IInputErrorResult, TResult> error) =>
            error(this);
    }

    partial class Docopt
    {
        public static IHelpFeaturingParser<IDictionary<string, Value>> CreateParser(string doc) =>
            new Parser<IDictionary<string, Value>>(doc, static (doc, argv, flags, version) =>
            {
                var optionsFirst = (flags & ParseFlags.OptionsFirst) != ParseFlags.None;
                var parsedResult = Parse(doc, Tokens.From(argv), optionsFirst);

                var help = (flags & ParseFlags.DisableHelp) == ParseFlags.None;
                if (help && parsedResult.IsHelpOptionSpecified)
                    return new ParseHelpResult<IDictionary<string, Value>>(doc);

                if (version is { } someVersion && parsedResult.IsVersionOptionSpecified)
                    return new ParseVersionResult<IDictionary<string, Value>>(someVersion);

                return parsedResult.TryApply(out var applicationResult)
                     ? new ArgumentsResult<IDictionary<string, Value>>(applicationResult.ToValueDictionary())
                     : new ParseInputErrorResult<IDictionary<string, Value>>("Input error.", parsedResult.ExitUsage);
            });
    }
}
