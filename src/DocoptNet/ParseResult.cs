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
        IParserWithVersionSupport<T> DisableHelp();
        IParserWithHelpSupport<T> DisableVersion();

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
    public partial interface IParserWithHelpSupport<out T>
    {
        IResult Parse(IEnumerable<string> argv);
        ArgsParseOptions Options { get; }
        IParserWithHelpSupport<T> WithOptions(ArgsParseOptions value);
        IParser<T> WithVersion(string value);
        IBasicParser<T> DisableHelp();

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
    public partial interface IParserWithVersionSupport<out T>
    {
        IResult Parse(IEnumerable<string> argv);
        IParser<T> EnableHelp();
        IBasicParser<T> DisableVersion();

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
    public partial interface IBasicParser<out T>
    {
        IResult Parse(IEnumerable<string> argv);
        ArgsParseOptions Options { get; }
        IBasicParser<T> WithOptions(ArgsParseOptions value);
        IParserWithHelpSupport<T> EnableHelp();
        IParserWithVersionSupport<T> WithVersion(string value);

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
        IParser<T>.IResult, IParserWithHelpSupport<T>.IResult,
        IParserWithVersionSupport<T>.IResult,
        IBasicParser<T>.IResult
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

        TResult IParserWithHelpSupport<T>.IResult.Match<TResult>(Func<T, TResult> args,
                                                                 Func<IHelpResult, TResult> help,
                                                                 Func<IInputErrorResult, TResult> error) =>
            args(Arguments);

        TResult IParserWithVersionSupport<T>.IResult.Match<TResult>(Func<T, TResult> args,
                                                                    Func<IVersionResult, TResult> version,
                                                                    Func<IInputErrorResult, TResult> error) =>
            args(Arguments);

        TResult IBasicParser<T>.IResult.Match<TResult>(Func<T, TResult> args,
                                                       Func<IInputErrorResult, TResult> error) =>
            args(Arguments);
    }

    sealed class ParseHelpResult<T> :
        IHelpResult,
        IParser<T>.IResult,
        IParserWithHelpSupport<T>.IResult
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

        TResult IParserWithHelpSupport<T>.IResult.Match<TResult>(Func<T, TResult> args,
                                                                 Func<IHelpResult, TResult> help,
                                                                 Func<IInputErrorResult, TResult> error) =>
            help(this);
    }

    sealed class ParseVersionResult<T> :
        IVersionResult,
        IParser<T>.IResult,
        IParserWithVersionSupport<T>.IResult
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

        TResult IParserWithVersionSupport<T>.IResult.Match<TResult>(Func<T, TResult> args,
                                                                    Func<IVersionResult, TResult> version,
                                                                    Func<IInputErrorResult, TResult> error) =>
            version(this);
    }

    sealed class ParseInputErrorResult<T> :
        IInputErrorResult,
        IParser<T>.IResult,
        IParserWithHelpSupport<T>.IResult,
        IParserWithVersionSupport<T>.IResult,
        IBasicParser<T>.IResult
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

        TResult IParserWithHelpSupport<T>.IResult.Match<TResult>(Func<T, TResult> args,
                                                                 Func<IHelpResult, TResult> help,
                                                                 Func<IInputErrorResult, TResult> error) =>
            error(this);

        TResult IParserWithVersionSupport<T>.IResult.Match<TResult>(Func<T, TResult> args,
                                                                    Func<IVersionResult, TResult> version,
                                                                    Func<IInputErrorResult, TResult> error) =>
            error(this);

        TResult IBasicParser<T>.IResult.Match<TResult>(Func<T, TResult> args,
                                                       Func<IInputErrorResult, TResult> error) =>
            error(this);
    }

    delegate IParser<T>.IResult ParseHandler<out T>(string doc, IEnumerable<string> argv, ParseFlags flags, string? version);

    sealed class Parser<T> :
        IParser<T>,
        IParserWithHelpSupport<T>,
        IParserWithVersionSupport<T>,
        IBasicParser<T>
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

        IParserWithHelpSupport<T>.IResult IParserWithHelpSupport<T>.Parse(IEnumerable<string> argv) =>
            _handler(_doc, argv, ParseFlags, null)
                .Match((_, r) => (IParserWithHelpSupport<T>.IResult)r,
                       (_, r) => (IParserWithHelpSupport<T>.IResult)r,
                       (_, _) => throw new NotSupportedException(),
                       (_, r) => (IParserWithHelpSupport<T>.IResult)r);

        IParser<T> IParserWithHelpSupport<T>.WithVersion(string value) =>
            new Parser<T>(_doc, _options, value, _handler);

        IParserWithVersionSupport<T> IBasicParser<T>.WithVersion(string value) =>
            new Parser<T>(_doc, _options, value, _handler);

        IParserWithHelpSupport<T> IParser<T>.DisableVersion() =>
            new Parser<T>(_doc, _options, null, _handler);

        IBasicParser<T> IParserWithVersionSupport<T>.DisableVersion() =>
            new Parser<T>(_doc, _options, null, _handler);

        IParserWithVersionSupport<T> IParser<T>.DisableHelp() =>
            new Parser<T>(_doc, _options, _version, _handler);

        IBasicParser<T> IParserWithHelpSupport<T>.DisableHelp() =>
            new Parser<T>(_doc, _options, null, _handler);

        IParserWithHelpSupport<T> IBasicParser<T>.EnableHelp() =>
            new Parser<T>(_doc, _options, null, _handler);

        IParser<T> IParserWithVersionSupport<T>.EnableHelp() =>
            new Parser<T>(_doc, _options, _version, _handler);

        IParserWithVersionSupport<T>.IResult IParserWithVersionSupport<T>.Parse(IEnumerable<string> argv) =>
            _handler(_doc, argv, ParseFlags | ParseFlags.DisableHelp, Version)
                .Match((_, r) => (IParserWithVersionSupport<T>.IResult)r,
                       (_, _) => throw new NotSupportedException(),
                       (_, r) => (IParserWithVersionSupport<T>.IResult)r,
                       (_, r) => (IParserWithVersionSupport<T>.IResult)r);

        IBasicParser<T>.IResult IBasicParser<T>.Parse(IEnumerable<string> argv) =>
            _handler(_doc, argv, ParseFlags | ParseFlags.DisableHelp, null)
                .Match((_, r) => (IBasicParser<T>.IResult)r,
                       (_, _) => throw new NotSupportedException(),
                       (_, _) => throw new NotSupportedException(),
                       (_, r) => (IBasicParser<T>.IResult)r);

        ArgsParseOptions IParser<T>.Options => _options;

        IParser<T> IParser<T>.WithOptions(ArgsParseOptions value) =>
            new Parser<T>(_doc, value, _version, _handler);

        ArgsParseOptions IParserWithHelpSupport<T>.Options => _options;

        IParserWithHelpSupport<T> IParserWithHelpSupport<T>.WithOptions(ArgsParseOptions value) =>
            new Parser<T>(_doc, value, _version, _handler);

        ArgsParseOptions IBasicParser<T>.Options => _options;

        IBasicParser<T> IBasicParser<T>.WithOptions(ArgsParseOptions value) =>
            new Parser<T>(_doc, value, _version, _handler);
    }

    partial class Docopt
    {
        public static IParserWithHelpSupport<IDictionary<string, ValueObject>> CreateParser(string doc) =>
            new Parser<IDictionary<string, ValueObject>>(doc, static (doc, argv, flags, version) =>
            {
                var optionsFirst = (flags & ParseFlags.OptionsFirst) != ParseFlags.None;
                var parsedResult = Parse(doc, Tokens.From(argv), optionsFirst);

                var help = (flags & ParseFlags.DisableHelp) == ParseFlags.None;
                if (help && parsedResult.IsHelpOptionSpecified)
                    return new ParseHelpResult<IDictionary<string, ValueObject>>(doc);

                if (version is { } someVersion && parsedResult.IsVersionOptionSpecified)
                    return new ParseVersionResult<IDictionary<string, ValueObject>>(someVersion);

                return parsedResult.TryApply(out var applicationResult)
                     ? new ArgumentsResult<IDictionary<string, ValueObject>>(applicationResult.ToValueObjectDictionary())
                     : new ParseInputErrorResult<IDictionary<string, ValueObject>>("Input error.", parsedResult.ExitUsage);
            });
    }
}
