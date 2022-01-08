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

    partial interface IParseResult
    {
    }

    partial interface IParseResult<T> : IParseResult
    {
    }

    partial interface IParseElseResult : IParseResult
    {
        ParseElseResult Else { get; }
    }

    sealed partial class ParseElseResult<T> : IParseResult<T>
    {
        public ParseElseResult(ParseElseResult @else) => Else = @else;
        public ParseElseResult Else { get; }
    }

    partial class ParseElseResult { }

    sealed partial class VersionResult : ParseElseResult
    {
        public VersionResult(string version) => Version = version;
        public string Version { get; }
        public override string ToString() => Version;
    }

    sealed partial class HelpResult : ParseElseResult
    {
        public HelpResult(string help) => Help = help;
        public string Help { get; }
        public override string ToString() => Help;
    }

    sealed partial class InputErrorResult : ParseElseResult
    {
        public InputErrorResult(string error, string usage) => (Error, Usage) = (error, usage);
        public string Error { get; }
        public string Usage { get; }
        public override string ToString() => Error + Environment.NewLine + Usage;
    }

    abstract partial class ArgumentsResult
    {
    }

    sealed partial class ArgumentsResult<T> : ArgumentsResult, IParseResult<T>
    {
        public ArgumentsResult(T args) => Arguments = args;
        public T Arguments { get; }
        public override string ToString() => Arguments.ToString();
    }
}
