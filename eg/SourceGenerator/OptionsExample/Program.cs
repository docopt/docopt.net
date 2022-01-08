using System;
using DocoptNet;
using OptionsExample;

return ProgramArguments.Parse(args, version: "1.0.0rc2").Run(args =>
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
});
