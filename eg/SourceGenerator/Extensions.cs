using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using DocoptNet;

static partial class ParseResultExtensions
{
    public static int Run<T>(this IHelpFeaturingParser<T> parser, IEnumerable<string> args, Func<T, int> runner) =>
        parser.Run(args, null, null, 0, runner);

    public static int Run<T>(this IHelpFeaturingParser<T> parser, IEnumerable<string> args,
                             TextWriter stdout, TextWriter stderr, int errorExitCode,
                             Func<T, int> runner)
    {
        stdout ??= Console.Out;
        stderr ??= Console.Error;

        switch (parser.Parse(args))
        {
            case IArgumentsResult<T> r:
                return runner(r.Arguments);
            case IHelpResult r:
                stdout.WriteLine(r.Help);
                return 0;
            case IInputErrorResult r:
                stderr.WriteLine(r.Usage);
                return errorExitCode != 0 ? errorExitCode : 1;
            case var result:
                throw new SwitchExpressionException(result);
        }
    }

    public static int Run<T>(this IParser<T> parser, IEnumerable<string> args, Func<T, int> runner) =>
        parser.Run(args, null, null, 0, runner);

    public static int Run<T>(this IParser<T> parser, IEnumerable<string> args,
                             TextWriter stdout, TextWriter stderr, int errorExitCode,
                             Func<T, int> runner)
    {
        stdout ??= Console.Out;
        stderr ??= Console.Error;

        switch (parser.Parse(args))
        {
            case IArgumentsResult<T> r:
                return runner(r.Arguments);
            case IHelpResult r:
                stdout.WriteLine(r.Help);
                return 0;
            case IVersionResult r:
                stdout.WriteLine(r.Version);
                return 0;
            case IInputErrorResult r:
                stderr.WriteLine(r.Usage);
                return errorExitCode != 0 ? errorExitCode : 1;
            case var result:
                throw new SwitchExpressionException(result);
        }
    }

    public static int Run<T>(this IBaselineParser<T> parser, IEnumerable<string> args, Func<T, int> runner) =>
        parser.Run(args, null, 0, runner);

    public static int Run<T>(this IBaselineParser<T> parser, IEnumerable<string> args,
                             TextWriter stderr, int errorExitCode,
                             Func<T, int> runner)
    {
        stderr ??= Console.Error;

        switch (parser.Parse(args))
        {
            case IArgumentsResult<T> r:
                return runner(r.Arguments);
            case IInputErrorResult r:
                stderr.WriteLine(r.Usage);
                return errorExitCode != 0 ? errorExitCode : 1;
            case var result:
                throw new SwitchExpressionException(result);
        }
    }
}
