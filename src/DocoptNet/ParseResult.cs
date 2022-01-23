namespace DocoptNet
{
    using System;

#if false
#if DOCOPTNET_PUBLIC
    public // ...
#endif
        /* ... */ enum ParseResultKind
    {
        InputError,
        Arguments,
        Help,
        Version,
    }
#endif

// FIXME remove the following disabling of warning RS0016
#pragma warning disable RS0016 // Add public types and members to the declared API

    partial interface IParseResult
    {
        public void Match(Action<object> args, Action<IParseElseResult> @else);
        public T Match<T>(Func<object, T> args, Func<IParseElseResult, T> @else);
    }

    partial interface IParseResult<out T> : IParseResult
    {
        public void Match(Action<T> args, Action<IParseElseResult> @else);
        public TResult Match<TResult>(Func<T, TResult> args, Func<IParseElseResult, TResult> @else);
    }

    partial interface IParseElseResult
    {
        void Match(Action<HelpResult> help, Action<VersionResult> version, Action<InputErrorResult> error);
        T Match<T>(Func<HelpResult, T> help, Func<VersionResult, T> version, Func<InputErrorResult, T> error);
    }

    sealed partial class ArgumentsResult<T> : IParseResult<T>
    {
        public ArgumentsResult(T args) => Arguments = args;

        public T Arguments { get; }

        public void Match(Action<T> args, Action<IParseElseResult> @else) => args(Arguments);
        public TResult Match<TResult>(Func<T, TResult> args, Func<IParseElseResult, TResult> @else) => args(Arguments);

        public override string ToString() => Arguments.ToString();

        void IParseResult.Match(Action<object> args, Action<IParseElseResult> @else) => args(Arguments);
        TResult IParseResult.Match<TResult>(Func<object, TResult> args, Func<IParseElseResult, TResult> @else) => args(Arguments);
    }

    partial interface IParseElseResult2
    {
        IParseElseResult Else { get; }
    }

    sealed partial class ParseElseResult<T> : IParseResult<T>, IParseElseResult2
    {
        public ParseElseResult(IParseElseResult @else) => Else = @else;

        public IParseElseResult Else { get; }

        public void Match(Action<T> args, Action<IParseElseResult> @else) => @else(Else);
        public TResult Match<TResult>(Func<T, TResult> args, Func<IParseElseResult, TResult> @else) => @else(Else);

        void IParseResult.Match(Action<object> args, Action<IParseElseResult> @else) => @else(Else);
        TResult IParseResult.Match<TResult>(Func<object, TResult> args, Func<IParseElseResult, TResult> @else) => @else(Else);
    }

    sealed partial class VersionResult : IParseElseResult
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
    }

    sealed partial class HelpResult : IParseElseResult
    {
        public HelpResult(string help) => Help = help;

        public string Help { get; }

        public override string ToString() => Help;

        public void Match(Action<HelpResult> help, Action<VersionResult> version, Action<InputErrorResult> error) => help(this);
        public T Match<T>(Func<HelpResult, T> help, Func<VersionResult, T> version, Func<InputErrorResult, T> error) => help(this);
    }

    sealed partial class InputErrorResult : IParseElseResult
    {
        public InputErrorResult(string error, string usage) => (Error, Usage) = (error, usage);

        public string Error { get; }
        public string Usage { get; }

        public override string ToString() => Error + Environment.NewLine + Usage;

        public void Match(Action<HelpResult> help, Action<VersionResult> version, Action<InputErrorResult> error) => error(this);
        public T Match<T>(Func<HelpResult, T> help, Func<VersionResult, T> version, Func<InputErrorResult, T> error) => error(this);
    }
}
