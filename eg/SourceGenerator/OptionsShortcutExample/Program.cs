using System;
using OptionsShortcutExample;

return ProgramArguments.CreateParser()
                       .WithVersion("1.0.0rc2")
                       .Run(args, Main);

static int Main(ProgramArguments args)
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
}
