using System;
using DocoptNet.Generated;
using NavalFate;

Program.Arguments arguments;

try
{
    arguments = Program.Arguments.Apply(args, version: "Naval Fate 2.0", exit: true);
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
    Ship     = {arguments.CmdShip    },
    New      = {arguments.CmdNew     },
    Name     = [{string.Join(", ", arguments.ArgName)}],
    Move     = {arguments.CmdMove    },
    X        = {arguments.ArgX       },
    Y        = {arguments.ArgY       },
    Speed    = {arguments.OptSpeed   },
    Shoot    = {arguments.CmdShoot   },
    Mine     = {arguments.CmdMine    },
    Set      = {arguments.CmdSet     },
    Remove   = {arguments.CmdRemove  },
    Moored   = {arguments.OptMoored  },
    Drifting = {arguments.OptDrifting},
    Help     = {arguments.OptHelp    },
    Version  = {arguments.OptVersion },
}}");

return 0;
