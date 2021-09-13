using System;
using DocoptNet.Generated;
using OptionsExample;

ProgramArguments arguments;

try
{
    arguments = ProgramArguments.Apply(args, version: "1.0.0rc2");
}
catch (DocoptExitException e)
{
    Console.WriteLine(e.Message);
    return e.ErrorCode;
}
catch (DocoptInputErrorException e)
{
    Console.Error.WriteLine(e);
    return 0xbd;
}

foreach (var (name, value) in arguments)
    Console.WriteLine($"{name} = {value}");

Console.WriteLine($@"{{
    Help       = {arguments.OptHelp      },
    Verbose    = {arguments.OptVerbose   },
    Quiet      = {arguments.OptQuiet     },
    Repeat     = {arguments.OptRepeat    },
    File       = {arguments.OptFile      },
    Exclude    = {arguments.OptExclude   },
    Select     = {arguments.OptSelect    },
    Ignore     = {arguments.OptIgnore    },
    ShowSource = {arguments.OptShowSource},
    Statistics = {arguments.OptStatistics},
    Count      = {arguments.OptCount     },
    Benchmark  = {arguments.OptBenchmark },
    Path       = [{string.Join(", ", arguments.ArgPath)}],
    Doctest    = {arguments.OptDoctest   },
    Testsuite  = {arguments.OptTestsuite },
    Version    = {arguments.OptVersion   },
}}");

return 0;
