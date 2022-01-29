namespace DocoptNet
{
    using System;
    using System.Collections.Generic;

    public partial interface IParser<out T, out TElse>
    {
        IParseResult<T, TElse> Parse(IEnumerable<string> argv, Docopt.ParseFlags flags);
    }

    public sealed class Parser<T> : IParser<T, IParseElseResult>
    {
        readonly string _version;
        readonly ParseHandler<T> _handler;

        public Parser(string version, ParseHandler<T> handler)
        {
            _version = version;
            _handler = handler;
        }

        public IParseResult<T, IParseElseResult> Parse(IEnumerable<string> argv, Docopt.ParseFlags flags) =>
            _handler(argv, flags & ~Docopt.ParseFlags.DisableHelp, _version)
                .Match(args => (IParseResult<T, IParseElseResult>)new ArgumentsResult<T, IParseElseResult>(args),
                       @else => @else.Match(help => new ParseElseResult<T, IParseElseResult>(help),
                                            version => new ParseElseResult<T, IParseElseResult>(version),
                                            error => new ParseElseResult<T, IParseElseResult>(error)));

        public ParserVersion<T> DisableHelp() => new(_version, _handler);
    }

    public sealed class ParserVersion<T> : IParser<T, IParseElseResult<VersionResult>>
    {
        readonly string _version;
        readonly ParseHandler<T> _handler;

        public ParserVersion(string version, ParseHandler<T> handler)
        {
            _version = version;
            _handler = handler;
        }

        public IParseResult<T, IParseElseResult<VersionResult>> Parse(IEnumerable<string> argv, Docopt.ParseFlags flags) =>
            _handler(argv, flags | Docopt.ParseFlags.DisableHelp, _version)
                .Match(args => (IParseResult<T, IParseElseResult<VersionResult>>)new ArgumentsResult<T, IParseElseResult<VersionResult>>(args),
                       @else => @else.Match(_ => throw new NotSupportedException(),
                                            version => new ParseElseResult<T, IParseElseResult<VersionResult>>(version),
                                            error => new ParseElseResult<T, IParseElseResult<VersionResult>>(error)));

        public Parser<T> WithHelp() => new(_version, _handler);
    }

    public delegate IParseResult<T, IParseElseResult> ParseHandler<out T>(IEnumerable<string> args, Docopt.ParseFlags options, string version);

    public sealed class ParserHelp<T> : IParser<T, IParseElseResult<HelpResult>>
    {
        readonly ParseHandler<T> _handler;

        public ParserHelp(ParseHandler<T> handler) => _handler = handler;

        public IParseResult<T, IParseElseResult<HelpResult>> Parse(IEnumerable<string> argv, Docopt.ParseFlags flags) =>
            _handler(argv, flags & ~Docopt.ParseFlags.DisableHelp, null)
                .Match(args => (IParseResult<T, IParseElseResult<HelpResult>>)new ArgumentsResult<T, IParseElseResult<HelpResult>>(args),
                       @else => @else.Match(help => new ParseElseResult<T, IParseElseResult<HelpResult>>(help),
                                            _ => throw new NotSupportedException(),
                                            error => new ParseElseResult<T, IParseElseResult<HelpResult>>(error)));

        public Parser<T> WithVersion(string value) => new(value, _handler);
        public ParserError<T> DisableHelp() => new(_handler);
    }

    public sealed class ParserError<T> : IParser<T, InputErrorResult>
    {
        readonly ParseHandler<T> _handler;

        public ParserError(ParseHandler<T> handler) => _handler = handler;

        public IParseResult<T, InputErrorResult> Parse(IEnumerable<string> argv, Docopt.ParseFlags flags) =>
            _handler(argv, flags | Docopt.ParseFlags.DisableHelp, null)
                .Match(args => (IParseResult<T, InputErrorResult>)new ArgumentsResult<T, InputErrorResult>(args),
                       @else => @else.Match(_ => throw new NotSupportedException(),
                                            _ => throw new NotSupportedException(),
                                            error => new ParseErrorResult<T>(error)));

        public ParserVersion<T> WithVersion(string value) => new(value, _handler);
        public ParserHelp<T> EnableHelp() => new(_handler);
    }

    public partial interface IParseResult
    {
        public TResult Match<TResult>(Func<object, TResult> args, Func<object, TResult> @else);
    }

    public partial interface IParseResult<out T, out TElse> : IParseResult
    {
        public TResult Match<TResult>(Func<T, TResult> args, Func<TElse, TResult> @else);
    }

    public partial interface IParseResult<out T> : IParseResult<T, IParseElseResult>
    {
    }

    partial interface IParseElseResult
    {
        IParseElseResult Else { get; }
        T Match<T>(Func<HelpResult, T> help, Func<VersionResult, T> version, Func<InputErrorResult, T> error);
    }

    public partial interface IParseElseResult<out T>
    {
        TResult Match<TResult>(Func<T, TResult> @else, Func<InputErrorResult, TResult> error);
    }

    public interface IArgumentsResult<out T>
    {
        public T Arguments { get; }
    }

    public partial class ArgumentsResult<T, TElse> : IArgumentsResult<T>, IParseResult<T, TElse>
    {
        public ArgumentsResult(T args) => Arguments = args;

        public T Arguments { get; }

        public override string ToString() => Arguments.ToString();

        TResult IParseResult<T, TElse>.Match<TResult>(Func<T, TResult> args,
                                                      Func<TElse, TResult> @else) =>
            args(Arguments);

        TResult IParseResult.Match<TResult>(Func<object, TResult> args,
                                            Func<object, TResult> @else) =>
            args(Arguments);
    }

    public sealed partial class ArgumentsResult<T> : ArgumentsResult<T, IParseElseResult>, IParseResult<T>
    {
        public ArgumentsResult(T args) : base(args) { }
    }

    public sealed partial class ParseElseResult<T, TElse> : IParseResult<T, TElse>, IParseElseResult<TElse>
    {
        public ParseElseResult(TElse @else) => Else = @else;

        public TElse Else { get; }

        public TResult Match<TResult>(Func<T, TResult> args,
                                      Func<TElse, TResult> @else) =>
            @else(Else);

        TResult IParseResult.Match<TResult>(Func<object, TResult> args,
                                            Func<object, TResult> @else) =>
            @else(Else);

        TResult IParseElseResult<TElse>.Match<TResult>(Func<TElse, TResult> @else,
                                                       Func<InputErrorResult, TResult> error) =>
            @else(Else);
    }

    public sealed partial class ParseErrorResult<T> : IParseResult<T, InputErrorResult>, IInputErrorResult
    {
        readonly InputErrorResult _error;

        public ParseErrorResult(InputErrorResult error) => _error = error;

        public string Error => _error.Error;
        public string Usage => _error.Usage;

        public TResult Match<TResult>(Func<T, TResult> args,
                                      Func<InputErrorResult, TResult> @else) =>
            @else(_error);

        TResult IParseResult.Match<TResult>(Func<object, TResult> args,
                                            Func<object, TResult> @else) =>
            @else(_error);
    }

    public sealed partial class ParseElseResult<T> : IParseResult<T>, IParseElseResult
    {
        public IParseElseResult Else { get; }

        public ParseElseResult(IParseElseResult @else) => Else = @else;

        TResult IParseResult<T, IParseElseResult>.Match<TResult>(Func<T, TResult> args,
                                                                 Func<IParseElseResult, TResult> @else) =>
            @else(Else);

        TResult IParseResult.Match<TResult>(Func<object, TResult> args,
                                            Func<object, TResult> @else) =>
            @else(Else);

        public TResult Match<TResult>(Func<HelpResult, TResult> help,
                                      Func<VersionResult, TResult> version,
                                      Func<InputErrorResult, TResult> error) =>
            Else.Match(help, version, error);
    }

    sealed partial class VersionResult : IParseElseResult, IParseElseResult<VersionResult>
    {
        public VersionResult(string version) => Version = version;

        public string Version { get; }

        public override string ToString() => Version;

        IParseElseResult IParseElseResult.Else => this;

        T IParseElseResult.Match<T>(Func<HelpResult, T> help,
                                    Func<VersionResult, T> version,
                                    Func<InputErrorResult, T> error) =>
            version(this);

        TResult IParseElseResult<VersionResult>.Match<TResult>(Func<VersionResult, TResult> @else,
                                                               Func<InputErrorResult, TResult> error) =>
            @else(this);
    }

    sealed partial class HelpResult : IParseElseResult, IParseElseResult<HelpResult>
    {
        public HelpResult(string help) => Help = help;

        public string Help { get; }

        public override string ToString() => Help;

        IParseElseResult IParseElseResult.Else => this;

        T IParseElseResult.Match<T>(Func<HelpResult, T> help,
                                    Func<VersionResult, T> version,
                                    Func<InputErrorResult, T> error) => help(this);

        TResult IParseElseResult<HelpResult>.Match<TResult>(Func<HelpResult, TResult> @else,
                                                            Func<InputErrorResult, TResult> error) =>
            @else(this);
    }

    public partial interface IInputErrorResult
    {
        public string Error { get; }
        public string Usage { get; }
    }

    sealed partial class InputErrorResult : IParseElseResult, IParseElseResult<VersionResult>, IParseElseResult<HelpResult>, IInputErrorResult
    {
        public InputErrorResult(string error, string usage) => (Error, Usage) = (error, usage);

        public string Error { get; }
        public string Usage { get; }

        public override string ToString() => Error + Environment.NewLine + Usage;

        IParseElseResult IParseElseResult.Else => this;

        T IParseElseResult.Match<T>(Func<HelpResult, T> help,
                                    Func<VersionResult, T> version,
                                    Func<InputErrorResult, T> error) =>
            error(this);

        TResult IParseElseResult<VersionResult>.Match<TResult>(Func<VersionResult, TResult> @else,
                                                               Func<InputErrorResult, TResult> error) =>
            error(this);

        TResult IParseElseResult<HelpResult>.Match<TResult>(Func<HelpResult, TResult> @else,
                                                            Func<InputErrorResult, TResult> error) =>
            error(this);
    }
}

namespace DocoptNet
{
    using System.Collections.Generic;

    partial class Docopt
    {
        public static ParserHelp<IDictionary<string, ValueObject>> Parse(string doc, ParseFlags flags) =>
            new((argv, flags, version) => Parse(doc, argv, flags, version));

        public static IParseResult<IDictionary<string, ValueObject>, IParseElseResult>
            Parse(string doc, IEnumerable<string> argv, ParseFlags flags, string version)
        {
            var optionsFirst = (flags & ParseFlags.OptionsFirst) != ParseFlags.None;
            var parsedResult = Parse(doc, Tokens.From(argv), optionsFirst);

            var help = (flags & ParseFlags.DisableHelp) == ParseFlags.None;
            if (help && parsedResult.IsHelpOptionSpecified)
                return new ParseElseResult<IDictionary<string, ValueObject>, IParseElseResult>(new HelpResult(doc));

            if (version is { } someVersion && parsedResult.IsVersionOptionSpecified)
                return new ParseElseResult<IDictionary<string, ValueObject>, IParseElseResult>(new VersionResult(someVersion));

            return parsedResult.TryApply(out var applicationResult)
                ? new ArgumentsResult<IDictionary<string, ValueObject>, IParseElseResult>(applicationResult.ToValueObjectDictionary())
                : new ParseElseResult<IDictionary<string, ValueObject>, IParseElseResult>(new InputErrorResult(string.Empty, parsedResult.ExitUsage));
        }

        public static IParseResult<IDictionary<string, ValueObject>, IParseElseResult<HelpResult>>
            Parse(string doc, ICollection<string> argv, ParseFlags flags)
        {
            var optionsFirst = (flags & ParseFlags.OptionsFirst) != ParseFlags.None;
            var parsedResult = Parse(doc, Tokens.From(argv), optionsFirst);

            var help = (flags & ParseFlags.DisableHelp) == ParseFlags.None;
            if (help && parsedResult.IsHelpOptionSpecified)
                return new ParseElseResult<IDictionary<string, ValueObject>, IParseElseResult<HelpResult>>(new HelpResult(doc));

            return parsedResult.TryApply(out var applicationResult)
                 ? new ArgumentsResult<IDictionary<string, ValueObject>, IParseElseResult<HelpResult>>(applicationResult.ToValueObjectDictionary())
                 : new ParseElseResult<IDictionary<string, ValueObject>, IParseElseResult<HelpResult>>(new InputErrorResult(string.Empty, parsedResult.ExitUsage));
        }
    }
}

namespace DocoptNet
{
    using System;
    using System.Collections.Generic;

    public partial class ArgumentsResult2<T> :
        IArgumentsResult<T>,
        IParser2<T>.IResult, IParserWithHelpSupport<T>.IResult, IParserWithVersionSupport<T>.IResult, IBasicParser<T>.IResult
    {
        public ArgumentsResult2(T args) => Arguments = args;

        public T Arguments { get; }

        public override string ToString() => Arguments.ToString();

        TResult IParser2<T>.IResult.Match<TResult>(Func<T, TResult> args,
                                                   Func<IHelpResult, TResult> help,
                                                   Func<IVersionResult, TResult> version,
                                                   Func<IInputErrorResult, TResult> error) =>
            args(Arguments);

        TResult IParser2<T>.IResult.Match<TResult>(Func<T, IParser2<T>.IResult, TResult> args,
                                                   Func<IHelpResult, IParser2<T>.IResult, TResult> help,
                                                   Func<IVersionResult, IParser2<T>.IResult, TResult> version,
                                                   Func<IInputErrorResult, IParser2<T>.IResult, TResult> error) =>
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

    public partial interface IHelpResult
    {
        string Help { get; }
    }

    public sealed partial class ParseHelpResult<T> :
        IHelpResult,
        IParser2<T>.IResult, IParserWithHelpSupport<T>.IResult
    {
        public ParseHelpResult(string help) => Help = help;

        public string Help { get; }

        TResult IParser2<T>.IResult.Match<TResult>(Func<T, TResult> args,
                                                Func<IHelpResult, TResult> help,
                                                Func<IVersionResult, TResult> version,
                                                Func<IInputErrorResult, TResult> error) =>
            help(this);

        TResult IParser2<T>.IResult.Match<TResult>(Func<T, IParser2<T>.IResult, TResult> args,
                                                   Func<IHelpResult, IParser2<T>.IResult, TResult> help,
                                                   Func<IVersionResult, IParser2<T>.IResult, TResult> version,
                                                   Func<IInputErrorResult, IParser2<T>.IResult, TResult> error) =>
            help(this, this);

        TResult IParserWithHelpSupport<T>.IResult.Match<TResult>(Func<T, TResult> args,
                                                                 Func<IHelpResult, TResult> help,
                                                                 Func<IInputErrorResult, TResult> error) =>
            help(this);
    }

    public partial interface IVersionResult
    {
        string Version { get; }
    }

    public sealed partial class ParseVersionResult<T> :
        IVersionResult,
        IParser2<T>.IResult,
        IParserWithVersionSupport<T>.IResult
    {
        public ParseVersionResult(string version) => Version = version;

        public string Version { get; }

        TResult IParser2<T>.IResult.Match<TResult>(Func<T, TResult> args,
                                                Func<IHelpResult, TResult> help,
                                                Func<IVersionResult, TResult> version,
                                                Func<IInputErrorResult, TResult> error) =>
            version(this);

        TResult IParser2<T>.IResult.Match<TResult>(Func<T, IParser2<T>.IResult, TResult> args,
                                                   Func<IHelpResult, IParser2<T>.IResult, TResult> help,
                                                   Func<IVersionResult, IParser2<T>.IResult, TResult> version,
                                                   Func<IInputErrorResult, IParser2<T>.IResult, TResult> error) =>
            version(this, this);

        TResult IParserWithVersionSupport<T>.IResult.Match<TResult>(Func<T, TResult> args,
                                                Func<IVersionResult, TResult> version,
                                                Func<IInputErrorResult, TResult> error) =>
            version(this);
    }

    public sealed partial class ParseInputErrorResult<T> :
        IInputErrorResult,
        IParser2<T>.IResult,
        IParserWithHelpSupport<T>.IResult,
        IParserWithVersionSupport<T>.IResult,
        IBasicParser<T>.IResult
    {
        public ParseInputErrorResult(string error, string usage) => (Error, Usage) = (error, usage);

        public string Error { get; }
        public string Usage { get; }

        TResult IParser2<T>.IResult.Match<TResult>(Func<T, TResult> args,
                                                Func<IHelpResult, TResult> help,
                                                Func<IVersionResult, TResult> version,
                                                Func<IInputErrorResult, TResult> error) =>
            error(this);

        TResult IParser2<T>.IResult.Match<TResult>(Func<T, IParser2<T>.IResult, TResult> args,
                                                   Func<IHelpResult, IParser2<T>.IResult, TResult> help,
                                                   Func<IVersionResult, IParser2<T>.IResult, TResult> version,
                                                   Func<IInputErrorResult, IParser2<T>.IResult, TResult> error) =>
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

    /// <summary>
    /// args + help + version + error
    /// </summary>
    public partial interface IParser2<out T>
    {
        IResult Parse(IEnumerable<string> argv, Docopt.ParseFlags flags);
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
        IResult Parse(IEnumerable<string> argv, Docopt.ParseFlags flags);
        IParser2<T> WithVersion(string value);
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
        IResult Parse(IEnumerable<string> argv, Docopt.ParseFlags flags);
        IParser2<T> EnableHelp();
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
        IResult Parse(IEnumerable<string> argv, Docopt.ParseFlags flags);
        IParserWithHelpSupport<T> EnableHelp();
        IParserWithVersionSupport<T> WithVersion(string value);

        public partial interface IResult
        {
            TResult Match<TResult>(Func<T, TResult> args,
                                   Func<IInputErrorResult, TResult> error);
        }
    }

    public delegate IParser2<T>.IResult ParseHandler2<out T>(string doc, IEnumerable<string> args, Docopt.ParseFlags flags, string version);

    sealed class Parser2<T> :
        IParser2<T>,
        IParserWithHelpSupport<T>,
        IParserWithVersionSupport<T>,
        IBasicParser<T>
    {
        readonly string _doc;
        readonly string _version;
        readonly ParseHandler2<T> _handler;

        public Parser2(string doc, string version, ParseHandler2<T> handler)
        {
            _doc = doc;
            _version = version;
            _handler = handler;
        }

        string Version => _version ?? throw new InvalidOperationException();

        IParser2<T>.IResult IParser2<T>.Parse(IEnumerable<string> argv, Docopt.ParseFlags flags) =>
            _handler(_doc, argv, flags & ~Docopt.ParseFlags.DisableHelp, Version);

        IParserWithHelpSupport<T>.IResult IParserWithHelpSupport<T>.Parse(IEnumerable<string> argv, Docopt.ParseFlags flags) =>
            _handler(_doc, argv, flags & ~Docopt.ParseFlags.DisableHelp, null)
                .Match((_, r) => (IParserWithHelpSupport<T>.IResult)r,
                       (_, r) => (IParserWithHelpSupport<T>.IResult)r,
                       (_, _) => throw new NotSupportedException(),
                       (_, r) => (IParserWithHelpSupport<T>.IResult)r);

        IParser2<T> IParserWithHelpSupport<T>.WithVersion(string value) =>
            new Parser2<T>(_doc, value, _handler);

        IParserWithVersionSupport<T> IBasicParser<T>.WithVersion(string value) =>
            new Parser2<T>(_doc, value, _handler);

        IParserWithHelpSupport<T> IParser2<T>.DisableVersion() =>
            new Parser2<T>(_doc, null, _handler);

        IBasicParser<T> IParserWithVersionSupport<T>.DisableVersion() =>
            new Parser2<T>(_doc, null, _handler);

        IParserWithVersionSupport<T> IParser2<T>.DisableHelp() =>
            new Parser2<T>(_doc, _version, _handler);

        IBasicParser<T> IParserWithHelpSupport<T>.DisableHelp() =>
            new Parser2<T>(_doc, null, _handler);

        IParserWithHelpSupport<T> IBasicParser<T>.EnableHelp() =>
            new Parser2<T>(_doc, null, _handler);

        IParser2<T> IParserWithVersionSupport<T>.EnableHelp() =>
            new Parser2<T>(_doc, _version, _handler);

        IParserWithVersionSupport<T>.IResult IParserWithVersionSupport<T>.Parse(IEnumerable<string> argv, Docopt.ParseFlags flags) =>
            _handler(_doc, argv, flags | Docopt.ParseFlags.DisableHelp, Version)
                .Match((_, r) => (IParserWithVersionSupport<T>.IResult)r,
                       (_, _) => throw new NotSupportedException(),
                       (_, r) => (IParserWithVersionSupport<T>.IResult)r,
                       (_, r) => (IParserWithVersionSupport<T>.IResult)r);

        IBasicParser<T>.IResult IBasicParser<T>.Parse(IEnumerable<string> argv, Docopt.ParseFlags flags) =>
            _handler(_doc, argv, flags | Docopt.ParseFlags.DisableHelp, null)
                .Match((_, r) => (IBasicParser<T>.IResult)r,
                       (_, _) => throw new NotSupportedException(),
                       (_, _) => throw new NotSupportedException(),
                       (_, r) => (IBasicParser<T>.IResult)r);
    }

    partial class Docopt
    {
        public static IParserWithHelpSupport<IDictionary<string, ValueObject>> Parser2(string doc) =>
            new Parser2<IDictionary<string, ValueObject>>(doc, null, Parse2);

        public static IParser2<IDictionary<string, ValueObject>>.IResult
            Parse2(string doc, IEnumerable<string> argv, ParseFlags flags, string version)
        {
            var optionsFirst = (flags & ParseFlags.OptionsFirst) != ParseFlags.None;
            var parsedResult = Parse(doc, Tokens.From(argv), optionsFirst);

            var help = (flags & ParseFlags.DisableHelp) == ParseFlags.None;
            if (help && parsedResult.IsHelpOptionSpecified)
                return new ParseHelpResult<IDictionary<string, ValueObject>>(doc);

            if (version is { } someVersion && parsedResult.IsVersionOptionSpecified)
                return new ParseVersionResult<IDictionary<string, ValueObject>>(someVersion);

            return parsedResult.TryApply(out var applicationResult)
                ? new ArgumentsResult2<IDictionary<string, ValueObject>>(applicationResult.ToValueObjectDictionary())
                : new ParseInputErrorResult<IDictionary<string, ValueObject>>("Input error.", parsedResult.ExitUsage);
        }
    }
}
