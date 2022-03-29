using DocoptNet;

return ProgramArguments.CreateParser()
                       .WithVersion(ThisAssembly.Info.Version)
                       .Parse(args)
                       .Match(Main,
                              result => Print(Console.Out, FormatHelp(result.Help)),
                              result => Print(Console.Out, result.Version),
                              result => Print(Console.Error, FormatHelp(result.Usage), 1));

static int Main(ProgramArguments args)
{
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
}

static string FormatHelp(string text) =>
    text.Replace("$", Path.GetFileName(Environment.ProcessPath));

static int Print(TextWriter writer, string message, int exitCode = 0)
{
    writer.WriteLine(message);
    return exitCode;
}

[DocoptArguments]
partial class ProgramArguments
{
    const string Help = @"Naval Fate.

Usage:
    $ ship new <name>...
    $ ship <name> move <x> <y> [--speed=<kn>]
    $ ship shoot <x> <y>
    $ mine (set|remove) <x> <y> [--moored | --drifting]
    $ (-h | --help)
    $ --version

Options:
    -h --help     Show this screen.
    --version     Show version.
    --speed=<kn>  Speed in knots [default: 10].
    --moored      Moored (anchored) mine.
    --drifting    Drifting mine.
";
}
