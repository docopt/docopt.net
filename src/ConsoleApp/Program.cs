using DocoptNet.ConsoleApp;

return ProgramArguments.CreateParser()
                       .WithVersion(ThisAssembly.Info.Version)
                       .Parse(args)
                       .Match(Main,
                              result => Print(Console.Out, FormatHelp(result.Help)),
                              result => Print(Console.Out, result.Version),
                              result => Print(Console.Error, FormatHelp(result.Usage), exitCode: 1));

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
