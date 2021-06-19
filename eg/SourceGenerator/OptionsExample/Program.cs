using System;
using System.Collections.Generic;
using System.Linq;
using DocoptNet.Generated;
using OptionsExample;

Program program;

try
{
    program = new Program(args, version: "1.0.0rc2", exit: true);
}
catch (DocoptInputErrorException e)
{
    Console.Error.WriteLine(e);
    return 0xbd;
}

foreach (var (name, value) in program.Args)
    Console.WriteLine("{0} = {1}", name, value);

Console.WriteLine($@"{{
    Help       = {program.OptHelp      },
    Verbose    = {program.OptVerbose   },
    Quiet      = {program.OptQuiet     },
    Repeat     = {program.OptRepeat    },
    File       = {program.OptFile      },
    Exclude    = {program.OptExclude   },
    Select     = {program.OptSelect    },
    Ignore     = {program.OptIgnore    },
    ShowSource = {program.OptShowSource},
    Statistics = {program.OptStatistics},
    Count      = {program.OptCount     },
    Benchmark  = {program.OptBenchmark },
    Path       = [{string.Join(", ", from object path in program.ArgPath select path)}],
    Doctest    = {program.OptDoctest   },
    Testsuite  = {program.OptTestsuite },
    Version    = {program.OptVersion   },
}}");

return 0;

namespace OptionsExample
{
    partial class Program
    {
        readonly IDictionary<string, ValueObject> _args;

        public Program(ICollection<string> argv, bool help = true,
                       object version = null, bool optionsFirst = false, bool exit = false)
        {
            _args = Apply(argv, help, version, optionsFirst, exit);
        }

        public IDictionary<string, ValueObject> Args => _args;
    }
}
