using System;
using System.IO;
using System.Runtime.CompilerServices;
using DocoptNet;

static partial class ParseResultExtensions
{
    public static int Run<T>(this IParser<T>.IResult result, Func<T, int> runner) =>
        result.Run(null, null, 0, runner);

    public static int Run<T>(this IParser<T>.IResult result,
                             TextWriter stdout, TextWriter stderr, int errorExitCode,
                             Func<T, int> runner)
    {
        stdout ??= Console.Out;
        stderr ??= Console.Error;

        switch (result)
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
            default:
                throw new SwitchExpressionException(result);
        }
    }
}
