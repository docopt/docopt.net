using System;
using System.IO;
using System.Runtime.CompilerServices;
using DocoptNet;

static partial class ParseResultExtensions
{
    public static int Run<T>(this IParseResult<T> result, Func<T, int> runner) =>
        result.Run(null, null, 0, runner);

    public static int Run<T>(this IParseResult<T> result,
                             TextWriter stdout, TextWriter stderr, int errorExitCode,
                             Func<T, int> runner)
    {
        stdout ??= Console.Out;
        stderr ??= Console.Error;

        switch (result)
        {
            case ArgumentsResult<T> r:
                return runner(r.Arguments);
            case ParseElseResult<T> { Else: HelpResult r }:
                stdout.WriteLine(r.Help);
                return 0;
            case ParseElseResult<T> { Else: VersionResult r }:
                stdout.WriteLine(r.Version);
                return 0;
            case ParseElseResult<T> { Else: InputErrorResult r }:
                stderr.WriteLine(r.Usage);
                return errorExitCode != 0 ? errorExitCode : 1;
            default:
                throw new SwitchExpressionException(result);
        }
    }
}
