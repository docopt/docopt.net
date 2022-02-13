#nullable enable

namespace DocoptNet
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// args + help + version + error
    /// </summary>
    partial interface IParser<out T>
    {
        IResult Parse(IEnumerable<string> argv);
        ArgsParseOptions Options { get; }
        IParser<T> WithOptions(ArgsParseOptions value);
        IVersionFeaturingParser<T> DisableHelp();
        IHelpFeaturingParser<T> DisableVersion();

        interface IResult
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
    partial interface IHelpFeaturingParser<out T>
    {
        IResult Parse(IEnumerable<string> argv);
        ArgsParseOptions Options { get; }
        IHelpFeaturingParser<T> WithOptions(ArgsParseOptions value);
        IParser<T> WithVersion(string value);
        IBaselineParser<T> DisableHelp();

        interface IResult
        {
            TResult Match<TResult>(Func<T, TResult> args,
                                   Func<IHelpResult, TResult> help,
                                   Func<IInputErrorResult, TResult> error);
        }
    }

    /// <summary>
    /// args + version + error
    /// </summary>
    partial interface IVersionFeaturingParser<out T>
    {
        IResult Parse(IEnumerable<string> argv);
        IParser<T> EnableHelp();
        IBaselineParser<T> DisableVersion();

        interface IResult
        {
            TResult Match<TResult>(Func<T, TResult> args,
                                   Func<IVersionResult, TResult> version,
                                   Func<IInputErrorResult, TResult> error);
        }
    }

    /// <summary>
    /// args + error
    /// </summary>
    partial interface IBaselineParser<out T>
    {
        IResult Parse(IEnumerable<string> argv);
        ArgsParseOptions Options { get; }
        IBaselineParser<T> WithOptions(ArgsParseOptions value);
        IHelpFeaturingParser<T> EnableHelp();
        IVersionFeaturingParser<T> WithVersion(string value);

        interface IResult
        {
            TResult Match<TResult>(Func<T, TResult> args,
                                   Func<IInputErrorResult, TResult> error);
        }
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
}
