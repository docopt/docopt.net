using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using DocoptNet;

static class ParseResultExtensions
{
    extension<T>(IHelpFeaturingParser<T> parser)
    {
        public int Run(IEnumerable<string> args, Func<T, int> runner) =>
            parser.Run(args, null, null, 0, runner);

        public int Run(IEnumerable<string> args,
                       TextWriter? stdout, TextWriter? stderr, int errorExitCode,
                       Func<T, int> runner) =>
                       Run(parser.Parse(args), stdout, stderr, errorExitCode, runner);
    }

    extension<T>(IParser<T> parser)
    {
        public int Run(IEnumerable<string> args, Func<T, int> runner) =>
            parser.Run(args, null, null, 0, runner);

        public int Run(IEnumerable<string> args,
                       TextWriter? stdout, TextWriter? stderr, int errorExitCode,
                       Func<T, int> runner) =>
            Run(parser.Parse(args), stdout, stderr, errorExitCode, runner);
    }

    extension<T>(IBaselineParser<T> parser)
    {
        public int Run(IEnumerable<string> args, Func<T, int> runner) =>
            parser.Run(args, null, 0, runner);

        public int Run(IEnumerable<string> args,
                       TextWriter? stderr, int errorExitCode,
                       Func<T, int> runner) =>
            Run(parser.Parse(args), null, stderr, errorExitCode, runner);
    }

    static int Run<T>(object result,
                      TextWriter? stdout, TextWriter? stderr, int errorExitCode,
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
            case IInputErrorResult r:
                stderr.WriteLine(r.Usage);
                return errorExitCode != 0 ? errorExitCode : 1;
            case var unmatched:
                throw new SwitchExpressionException(unmatched);
        }
    }
}
