using System;
using DocoptNet;
using OptionsShortcutExample;

return ProgramArguments.Parse(args, version: "1.0.0rc2").Run(args =>
{
    foreach (var (name, value) in args)
        Console.WriteLine($"{name} = {value}");

Console.WriteLine($@"{{
    Help    = {args.OptHelp   },
    Version = {args.OptVersion},
    Number  = {args.OptNumber },
    Timeout = {args.OptTimeout},
    Apply   = {args.OptApply  },
    Q       = {args.OptQ      },
    Port    = {args.ArgPort   },
}}");

    return 0;
});
