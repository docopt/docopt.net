#nullable enable

namespace DocoptNet
{
    using System;
    using System.Collections.Generic;

    public partial interface IArgumentsResult<out T>
    {
        public T Arguments { get; }
    }

    public partial interface IInputErrorResult
    {
        public string Error { get; }
        public string Usage { get; }
    }

    public partial interface IHelpResult
    {
        string Help { get; }
    }

    public partial interface IVersionResult
    {
        string Version { get; }
    }

    /// <summary>
    /// args + help + version + error
    /// </summary>
    public partial interface IParser<out T>
    {
        IResult Parse(IEnumerable<string> argv);
        ArgsParseOptions Options { get; }
        IParser<T> WithOptions(ArgsParseOptions value);
        IVersionFeaturingParser<T> DisableHelp();
        IHelpFeaturingParser<T> DisableVersion();

        public partial interface IResult
        {
            TResult Match<TResult>(Func<T, TResult> args,
                                   Func<IHelpResult, TResult> help,
                                   Func<IVersionResult, TResult> version,
                                   Func<IInputErrorResult, TResult> error);
            TResult Match<TResult>(Func<T, IResult, TResult> args,
                                   Func<IHelpResult, IResult, TResult> help,
                                   Func<IVersionResult, IResult, TResult> version,
                                   Func<IInputErrorResult, IResult, TResult> error);
        }
    }

    /// <summary>
    /// args + help + error
    /// </summary>
    public partial interface IHelpFeaturingParser<out T>
    {
        IResult Parse(IEnumerable<string> argv);
        ArgsParseOptions Options { get; }
        IHelpFeaturingParser<T> WithOptions(ArgsParseOptions value);
        IParser<T> WithVersion(string value);
        IBaselineParser<T> DisableHelp();

        public partial interface IResult
        {
            TResult Match<TResult>(Func<T, TResult> args,
                                   Func<IHelpResult, TResult> help,
                                   Func<IInputErrorResult, TResult> error);
        }
    }

    /// <summary>
    /// args + version + error
    /// </summary>
    public partial interface IVersionFeaturingParser<out T>
    {
        IResult Parse(IEnumerable<string> argv);
        IParser<T> EnableHelp();
        IBaselineParser<T> DisableVersion();

        public partial interface IResult
        {
            TResult Match<TResult>(Func<T, TResult> args,
                                   Func<IVersionResult, TResult> version,
                                   Func<IInputErrorResult, TResult> error);
        }
    }

    /// <summary>
    /// args + error
    /// </summary>
    public partial interface IBaselineParser<out T>
    {
        IResult Parse(IEnumerable<string> argv);
        ArgsParseOptions Options { get; }
        IBaselineParser<T> WithOptions(ArgsParseOptions value);
        IHelpFeaturingParser<T> EnableHelp();
        IVersionFeaturingParser<T> WithVersion(string value);

        public partial interface IResult
        {
            TResult Match<TResult>(Func<T, TResult> args,
                                   Func<IInputErrorResult, TResult> error);
        }
    }
}
namespace DocoptNet
{
    using System;
    using System.Collections.Generic;

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

    delegate IParser<T>.IResult ParseHandler<out T>(string doc, IEnumerable<string> argv, ParseFlags flags, string? version);

    sealed class Parser<T> :
        IParser<T>,
        IHelpFeaturingParser<T>,
        IVersionFeaturingParser<T>,
        IBaselineParser<T>
    {
        readonly string _doc;
        readonly string? _version;
        readonly ParseHandler<T> _handler;
        readonly ArgsParseOptions _options;

        public Parser(string doc, ParseHandler<T> handler) :
            this(doc, ArgsParseOptions.Default, null, handler) { }

        public Parser(string doc, ArgsParseOptions options, string? version, ParseHandler<T> handler)
        {
            _doc = doc;
            _options = options;
            _version = version;
            _handler = handler;
        }

        string Version => _version ?? throw new InvalidOperationException();

        ParseFlags ParseFlags => _options.OptionsFirst ? ParseFlags.OptionsFirst : ParseFlags.None;

        IParser<T>.IResult IParser<T>.Parse(IEnumerable<string> argv) =>
            _handler(_doc, argv, ParseFlags, Version);

        IHelpFeaturingParser<T>.IResult IHelpFeaturingParser<T>.Parse(IEnumerable<string> argv) =>
            _handler(_doc, argv, ParseFlags, null)
                .Match((_, r) => (IHelpFeaturingParser<T>.IResult)r,
                       (_, r) => (IHelpFeaturingParser<T>.IResult)r,
                       (_, _) => throw new NotSupportedException(),
                       (_, r) => (IHelpFeaturingParser<T>.IResult)r);

        IParser<T> IHelpFeaturingParser<T>.WithVersion(string value) =>
            new Parser<T>(_doc, _options, value, _handler);

        IVersionFeaturingParser<T> IBaselineParser<T>.WithVersion(string value) =>
            new Parser<T>(_doc, _options, value, _handler);

        IHelpFeaturingParser<T> IParser<T>.DisableVersion() =>
            new Parser<T>(_doc, _options, null, _handler);

        IBaselineParser<T> IVersionFeaturingParser<T>.DisableVersion() =>
            new Parser<T>(_doc, _options, null, _handler);

        IVersionFeaturingParser<T> IParser<T>.DisableHelp() =>
            new Parser<T>(_doc, _options, _version, _handler);

        IBaselineParser<T> IHelpFeaturingParser<T>.DisableHelp() =>
            new Parser<T>(_doc, _options, null, _handler);

        IHelpFeaturingParser<T> IBaselineParser<T>.EnableHelp() =>
            new Parser<T>(_doc, _options, null, _handler);

        IParser<T> IVersionFeaturingParser<T>.EnableHelp() =>
            new Parser<T>(_doc, _options, _version, _handler);

        IVersionFeaturingParser<T>.IResult IVersionFeaturingParser<T>.Parse(IEnumerable<string> argv) =>
            _handler(_doc, argv, ParseFlags | ParseFlags.DisableHelp, Version)
                .Match((_, r) => (IVersionFeaturingParser<T>.IResult)r,
                       (_, _) => throw new NotSupportedException(),
                       (_, r) => (IVersionFeaturingParser<T>.IResult)r,
                       (_, r) => (IVersionFeaturingParser<T>.IResult)r);

        IBaselineParser<T>.IResult IBaselineParser<T>.Parse(IEnumerable<string> argv) =>
            _handler(_doc, argv, ParseFlags | ParseFlags.DisableHelp, null)
                .Match((_, r) => (IBaselineParser<T>.IResult)r,
                       (_, _) => throw new NotSupportedException(),
                       (_, _) => throw new NotSupportedException(),
                       (_, r) => (IBaselineParser<T>.IResult)r);

        ArgsParseOptions IParser<T>.Options => _options;

        IParser<T> IParser<T>.WithOptions(ArgsParseOptions value) =>
            new Parser<T>(_doc, value, _version, _handler);

        ArgsParseOptions IHelpFeaturingParser<T>.Options => _options;

        IHelpFeaturingParser<T> IHelpFeaturingParser<T>.WithOptions(ArgsParseOptions value) =>
            new Parser<T>(_doc, value, _version, _handler);

        ArgsParseOptions IBaselineParser<T>.Options => _options;

        IBaselineParser<T> IBaselineParser<T>.WithOptions(ArgsParseOptions value) =>
            new Parser<T>(_doc, value, _version, _handler);
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
