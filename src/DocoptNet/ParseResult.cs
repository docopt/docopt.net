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
            _handler(argv, flags & Docopt.ParseFlags.DisableHelp, null)
                .Match(args => (IParseResult<T, IParseElseResult>)new ArgumentsResult<T, IParseElseResult>(args),
                       @else => @else.Match(help => new ParseElseResult<T, IParseElseResult>(help),
                                            _ => throw new NotSupportedException(),
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
            _handler(argv, flags & Docopt.ParseFlags.DisableHelp, null)
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
            _handler(argv, flags & Docopt.ParseFlags.DisableHelp, null)
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
            _handler(argv, flags & Docopt.ParseFlags.DisableHelp, null)
                .Match(args => (IParseResult<T, InputErrorResult>)new ArgumentsResult<T, InputErrorResult>(args),
                       @else => @else.Match(_ => throw new NotSupportedException(),
                                            _ => throw new NotSupportedException(),
                                            error => new ParseElseResult<T, InputErrorResult>(error)));

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
        T Match<T>(Func<HelpResult, T> help, Func<VersionResult, T> version, Func<InputErrorResult, T> error);
    }

    public partial interface IParseElseResult<out T>
    {
        TResult Match<TResult>(Func<T, TResult> @else, Func<InputErrorResult, TResult> error);
    }

    public partial class ArgumentsResult<T, TElse> : IParseResult<T, TElse>
    {
        public ArgumentsResult(T args) => Arguments = args;

        public T Arguments { get; }

        public TResult Match<TResult>(Func<T, TResult> args, Func<TElse, TResult> @else) => args(Arguments);

        public override string ToString() => Arguments.ToString();

        TResult IParseResult.Match<TResult>(Func<object, TResult> args, Func<object, TResult> @else) => args(Arguments);
    }

    public sealed partial class ArgumentsResult<T> : ArgumentsResult<T, IParseElseResult>, IParseResult<T>
    {
        public ArgumentsResult(T args) : base(args) { }
    }

    public sealed partial class ParseElseResult<T, TElse> : IParseResult<T, TElse>
    {
        public ParseElseResult(TElse @else) => Else = @else;

        public TElse Else { get; }

        public TResult Match<TResult>(Func<T, TResult> args, Func<TElse, TResult> @else) => @else(Else);

        TResult IParseResult.Match<TResult>(Func<object, TResult> args, Func<object, TResult> @else) => @else(Else);
    }

    public sealed partial class ParseElseResult<T> : IParseResult<T>
    {
        public IParseElseResult Else { get; }

        public ParseElseResult(IParseElseResult @else) => Else = @else;

        public TResult Match<TResult>(Func<T, TResult> args, Func<IParseElseResult, TResult> @else) => @else(Else);

        TResult IParseResult.Match<TResult>(Func<object, TResult> args, Func<object, TResult> @else) => @else(Else);
    }

    sealed partial class VersionResult : IParseElseResult, IParseElseResult<VersionResult>
    {
        public VersionResult(string version) => Version = version;

        public string Version { get; }

        public override string ToString() => Version;

        public T Match<T>(Func<HelpResult, T> help, Func<VersionResult, T> version,
                          Func<InputErrorResult, T> error) =>
            version(this);

        public TResult Match<TResult>(Func<VersionResult, TResult> @else,
                                      Func<InputErrorResult, TResult> error) =>
            @else(this);
    }

    sealed partial class HelpResult : IParseElseResult, IParseElseResult<HelpResult>
    {
        public HelpResult(string help) => Help = help;

        public string Help { get; }

        public override string ToString() => Help;

        public T Match<T>(Func<HelpResult, T> help, Func<VersionResult, T> version, Func<InputErrorResult, T> error) => help(this);

        public TResult Match<TResult>(Func<HelpResult, TResult> @else, Func<InputErrorResult, TResult> error) => @else(this);
    }

    sealed partial class InputErrorResult : IParseElseResult, IParseElseResult<VersionResult>, IParseElseResult<HelpResult>
    {
        public InputErrorResult(string error, string usage) => (Error, Usage) = (error, usage);

        public string Error { get; }
        public string Usage { get; }

        public override string ToString() => Error + Environment.NewLine + Usage;

        public T Match<T>(Func<HelpResult, T> help, Func<VersionResult, T> version, Func<InputErrorResult, T> error) => error(this);

        public TResult Match<TResult>(Func<VersionResult, TResult> @else, Func<InputErrorResult, TResult> error) => error(this);

        public TResult Match<TResult>(Func<HelpResult, TResult> @else, Func<InputErrorResult, TResult> error) => error(this);
    }
}

namespace DocoptNet
{
    using System.Collections.Generic;

    partial class Docopt
    {
        public ParserHelp<IDictionary<string, ValueObject>> Parse(string doc, ParseFlags flags) =>
            new((argv, flags, version) => Parse(doc, argv, flags, version));

        public IParseResult<IDictionary<string, ValueObject>, IParseElseResult>
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

        public IParseResult<IDictionary<string, ValueObject>, IParseElseResult<HelpResult>>
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
