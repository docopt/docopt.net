using System;
using OptionsExample;

return ProgramArguments.CreateParser()
                       .WithVersion("1.0.0rc2")
                       .Run(args, Main);

static int Main(ProgramArguments args)
{
    foreach (var (name, value) in args)
        Console.WriteLine($"{name} = {value}");

    Console.WriteLine($@"{{
    Help       = {args.OptHelp      },
    Verbose    = {args.OptVerbose   },
    Quiet      = {args.OptQuiet     },
    Repeat     = {args.OptRepeat    },
    File       = {args.OptFile      },
    Exclude    = {args.OptExclude   },
    Select     = {args.OptSelect    },
    Ignore     = {args.OptIgnore    },
    ShowSource = {args.OptShowSource},
    Statistics = {args.OptStatistics},
    Count      = {args.OptCount     },
    Benchmark  = {args.OptBenchmark },
    Path       = [{string.Join(", ", args.ArgPath)}],
    Doctest    = {args.OptDoctest   },
    Testsuite  = {args.OptTestsuite },
    Version    = {args.OptVersion   },
}}");

    return 0;
}
