// Licensed under terms of MIT license (see LICENSE-MIT)
// Copyright 2021 Atif Aziz, Dinh Doan Van Bien

namespace DocoptNet
{
    using System;

    partial interface IArgumentsResult<out T>
    {
        T Arguments { get; }
    }

    partial interface IInputErrorResult
    {
        string Error { get; }
        string Usage { get; }
    }

    partial interface IHelpResult
    {
        string Help { get; }
    }

    partial interface IVersionResult
    {
        string Version { get; }
    }

    sealed class ArgumentsResult<T>(T args) :
        IArgumentsResult<T>,
        IParser<T>.IResult, IHelpFeaturingParser<T>.IResult,
        IVersionFeaturingParser<T>.IResult,
        IBaselineParser<T>.IResult
    {
        public T Arguments { get; } = args;

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

    sealed class ParseHelpResult<T>(string help) :
        IHelpResult,
        IParser<T>.IResult,
        IHelpFeaturingParser<T>.IResult
    {
        public string Help { get; } = help;

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

    sealed class ParseVersionResult<T>(string version) :
        IVersionResult,
        IParser<T>.IResult,
        IVersionFeaturingParser<T>.IResult
    {
        public string Version { get; } = version;

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

    sealed class ParseInputErrorResult<T>(string error, string usage) :
        IInputErrorResult,
        IParser<T>.IResult,
        IHelpFeaturingParser<T>.IResult,
        IVersionFeaturingParser<T>.IResult,
        IBaselineParser<T>.IResult
    {
        public string Error { get; } = error;
        public string Usage { get; } = usage;

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
}
