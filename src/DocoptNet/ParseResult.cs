// FIXME remove the following disabling of warning RS0016
#pragma warning disable RS0016 // Add public types and members to the declared API

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
        private readonly string _version;
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
        private readonly string _version;
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
        public void Match(Action<object> args, Action<object> @else);
        public TResult Match<TResult>(Func<object, TResult> args, Func<object, TResult> @else);
    }

    public partial interface IParseResult<out T, out TElse> : IParseResult
    {
        public void Match(Action<T> args, Action<TElse> @else);
        public TResult Match<TResult>(Func<T, TResult> args, Func<TElse, TResult> @else);
    }

    public partial interface IParseResult<out T> : IParseResult<T, IParseElseResult>
    {
    }

    partial interface IParseElseResult
    {
        void Match(Action<HelpResult> help, Action<VersionResult> version, Action<InputErrorResult> error);
        T Match<T>(Func<HelpResult, T> help, Func<VersionResult, T> version, Func<InputErrorResult, T> error);
    }

    public partial interface IParseElseResult<out T>
    {
        void Match(Action<T> @else, Action<InputErrorResult> error);
        TResult Match<TResult>(Func<T, TResult> @else, Func<InputErrorResult, TResult> error);
    }

    public partial class ArgumentsResult<T, TElse> : IParseResult<T, TElse>
    {
        public ArgumentsResult(T args) => Arguments = args;

        public T Arguments { get; }

        public void Match(Action<T> args, Action<TElse> @else) => args(Arguments);
        public TResult Match<TResult>(Func<T, TResult> args, Func<TElse, TResult> @else) => args(Arguments);

        public override string ToString() => Arguments.ToString();

        void IParseResult.Match(Action<object> args, Action<object> @else) => args(Arguments);
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

        public void Match(Action<T> args, Action<TElse> @else) => @else(Else);
        public TResult Match<TResult>(Func<T, TResult> args, Func<TElse, TResult> @else) => @else(Else);

        void IParseResult.Match(Action<object> args, Action<object> @else) => @else(Else);
        TResult IParseResult.Match<TResult>(Func<object, TResult> args, Func<object, TResult> @else) => @else(Else);
    }

    public sealed partial class ParseElseResult<T> : IParseResult<T>
    {
        public IParseElseResult Else { get; }

        public ParseElseResult(IParseElseResult @else) => Else = @else;

        public void Match(Action<T> args, Action<IParseElseResult> @else) => @else(Else);
        public TResult Match<TResult>(Func<T, TResult> args, Func<IParseElseResult, TResult> @else) => @else(Else);

        void IParseResult.Match(Action<object> args, Action<object> @else) => @else(Else);
        TResult IParseResult.Match<TResult>(Func<object, TResult> args, Func<object, TResult> @else) => @else(Else);
    }

    sealed partial class VersionResult : IParseElseResult, IParseElseResult<VersionResult>
    {
        public VersionResult(string version) => Version = version;

        public string Version { get; }

        public override string ToString() => Version;

        public void Match(Action<HelpResult> help, Action<VersionResult> version,
                          Action<InputErrorResult> error) =>
            version(this);

        public T Match<T>(Func<HelpResult, T> help, Func<VersionResult, T> version,
                          Func<InputErrorResult, T> error) =>
            version(this);

        public void Match(Action<VersionResult> @else,
                          Action<InputErrorResult> error) =>
            @else(this);

        public TResult Match<TResult>(Func<VersionResult, TResult> @else,
                                      Func<InputErrorResult, TResult> error) =>
            @else(this);
    }

    sealed partial class HelpResult : IParseElseResult, IParseElseResult<HelpResult>
    {
        public HelpResult(string help) => Help = help;

        public string Help { get; }

        public override string ToString() => Help;

        public void Match(Action<HelpResult> help, Action<VersionResult> version, Action<InputErrorResult> error) => help(this);
        public T Match<T>(Func<HelpResult, T> help, Func<VersionResult, T> version, Func<InputErrorResult, T> error) => help(this);

        public void Match(Action<HelpResult> @else, Action<InputErrorResult> error) => @else(this);
        public TResult Match<TResult>(Func<HelpResult, TResult> @else, Func<InputErrorResult, TResult> error) => @else(this);
    }

    sealed partial class InputErrorResult : IParseElseResult, IParseElseResult<VersionResult>, IParseElseResult<HelpResult>
    {
        public InputErrorResult(string error, string usage) => (Error, Usage) = (error, usage);

        public string Error { get; }
        public string Usage { get; }

        public override string ToString() => Error + Environment.NewLine + Usage;

        public void Match(Action<HelpResult> help, Action<VersionResult> version, Action<InputErrorResult> error) => error(this);
        public T Match<T>(Func<HelpResult, T> help, Func<VersionResult, T> version, Func<InputErrorResult, T> error) => error(this);

        public void Match(Action<VersionResult> @else, Action<InputErrorResult> error) => error(this);
        public TResult Match<TResult>(Func<VersionResult, TResult> @else, Func<InputErrorResult, TResult> error) => error(this);

        public void Match(Action<HelpResult> @else, Action<InputErrorResult> error) => error(this);
        public TResult Match<TResult>(Func<HelpResult, TResult> @else, Func<InputErrorResult, TResult> error) => error(this);
    }
}

namespace DocoptNet
{
    using System.Collections.Generic;

    partial class Docopt
    {
        public ParserHelp<IDictionary<string, ValueObject>> Parse(string doc, Docopt.ParseFlags flags) =>
            new((argv, flags, version) => Parse(doc, argv, flags, version));

        public IParseResult<IDictionary<string, ValueObject>, IParseElseResult>
            Parse(string doc, IEnumerable<string> argv, Docopt.ParseFlags flags, string version)
        {
            var optionsFirst = (flags & Docopt.ParseFlags.OptionsFirst) != Docopt.ParseFlags.None;
            var parsedResult = Parse(doc, Tokens.From(argv), optionsFirst);

            var help = (flags & Docopt.ParseFlags.DisableHelp) == Docopt.ParseFlags.None;
            if (help && parsedResult.IsHelpOptionSpecified)
                return new ParseElseResult<IDictionary<string, ValueObject>, IParseElseResult>(new HelpResult(doc));

            if (version is { } someVersion && parsedResult.IsVersionOptionSpecified)
                return new ParseElseResult<IDictionary<string, ValueObject>, IParseElseResult>(new VersionResult(someVersion));

            return parsedResult.TryApply(out var applicationResult)
                ? new ArgumentsResult<IDictionary<string, ValueObject>, IParseElseResult>(applicationResult.ToValueObjectDictionary())
                : new ParseElseResult<IDictionary<string, ValueObject>, IParseElseResult>(new InputErrorResult(string.Empty, parsedResult.ExitUsage));
        }

        public IParseResult<IDictionary<string, ValueObject>, IParseElseResult<HelpResult>>
            Parse(string doc, ICollection<string> argv, Docopt.ParseFlags flags)
        {
            var optionsFirst = (flags & Docopt.ParseFlags.OptionsFirst) != Docopt.ParseFlags.None;
            var parsedResult = Parse(doc, Tokens.From(argv), optionsFirst);

            var help = (flags & Docopt.ParseFlags.DisableHelp) == Docopt.ParseFlags.None;
            if (help && parsedResult.IsHelpOptionSpecified)
                return new ParseElseResult<IDictionary<string, ValueObject>, IParseElseResult<HelpResult>>(new HelpResult(doc));

            return parsedResult.TryApply(out var applicationResult)
                 ? new ArgumentsResult<IDictionary<string, ValueObject>, IParseElseResult<HelpResult>>(applicationResult.ToValueObjectDictionary())
                 : new ParseElseResult<IDictionary<string, ValueObject>, IParseElseResult<HelpResult>>(new InputErrorResult(string.Empty, parsedResult.ExitUsage));
        }
    }
}
