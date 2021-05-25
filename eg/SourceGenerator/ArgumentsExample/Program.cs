using System;
using System.Collections.Generic;
using System.Linq;
using DocoptNet.Generated;
using ArgumentsExample;

Program program;

try
{
    program = new Program(args, exit: true);
}
catch (DocoptInputErrorException e)
{
    Console.Error.WriteLine(e);
    return 0xbd;
}

foreach (var (name, value) in program.Args)
    Console.WriteLine("{0} = {1}", name, value);

Console.WriteLine($@"{{
    Help       = {program.OptHelp    },
    V          = {program.OptV       },
    Q          = {program.OptQ       },
    R          = {program.OptR       },
    Left       = {program.OptLeft    },
    Right      = {program.OptRight   },
    Correction = {program.ArgCorrection},
    File       = [{string.Join(", ", from object file in program.ArgFile select file)}],
}}");

return 0;

namespace ArgumentsExample
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