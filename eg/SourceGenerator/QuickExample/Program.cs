using System;
using System.Collections.Generic;
using DocoptNet.Generated;
using QuickExample;

Program program;

try
{
    program = new Program(args, version: "0.1.1rc", exit: true);
}
catch (DocoptInputErrorException e)
{
    Console.Error.WriteLine(e);
    return 0xbd;
}

foreach (var (name, value) in program.Args)
    Console.WriteLine("{0} = {1}", name, value);

Console.WriteLine($@"{{
    Tcp     = {program.CmdTcp    },
    Host    = {program.ArgHost   },
    Port    = {program.ArgPort   },
    Timeout = {program.OptTimeout},
    Serial  = {program.CmdSerial },
    Baud    = {program.OptBaud   },
    H       = {program.OptH      },
    Help    = {program.OptHelp   },
    Version = {program.OptVersion},
}}");

return 0;

namespace QuickExample
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
