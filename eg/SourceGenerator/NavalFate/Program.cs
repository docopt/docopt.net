using System;
using DocoptNet;
using NavalFate;

return ProgramArguments.Parse(args, version: "Naval Fate 2.0").Run(args =>
{
    foreach (var (name, value) in args)
        Console.WriteLine($"{name} = {value}");

    Console.WriteLine($@"{{
    Ship     = {args.CmdShip    },
    New      = {args.CmdNew     },
    Name     = [{string.Join(", ", args.ArgName)}],
    Move     = {args.CmdMove    },
    X        = {args.ArgX       },
    Y        = {args.ArgY       },
    Speed    = {args.OptSpeed   },
    Shoot    = {args.CmdShoot   },
    Mine     = {args.CmdMine    },
    Set      = {args.CmdSet     },
    Remove   = {args.CmdRemove  },
    Moored   = {args.OptMoored  },
    Drifting = {args.OptDrifting},
    Help     = {args.OptHelp    },
    Version  = {args.OptVersion },
}}");

    return 0;
});
