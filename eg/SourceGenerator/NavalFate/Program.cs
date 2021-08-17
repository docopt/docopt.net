using System;
using System.Collections.Generic;
using System.Linq;
using DocoptNet.Generated;
using NavalFate;

Program program;

try
{
    program = new Program(args, version: "Naval Fate 2.0", exit: true);
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

foreach (var (name, value) in program.Args)
    Console.WriteLine("{0} = {1}", name, value);

Console.WriteLine($@"{{
    Ship     = {program.CmdShip    },
    New      = {program.CmdNew     },
    Name     = [{string.Join(", ", from object name in program.ArgName select name)}],
    Move     = {program.CmdMove    },
    X        = {program.ArgX       },
    Y        = {program.ArgY       },
    Speed    = {program.OptSpeed   },
    Shoot    = {program.CmdShoot   },
    Mine     = {program.CmdMine    },
    Set      = {program.CmdSet     },
    Remove   = {program.CmdRemove  },
    Moored   = {program.OptMoored  },
    Drifting = {program.OptDrifting},
    Help     = {program.OptHelp    },
    Version  = {program.OptVersion },
}}");

return 0;

namespace NavalFate
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
