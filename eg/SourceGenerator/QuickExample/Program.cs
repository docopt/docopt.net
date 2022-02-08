using System;
using DocoptNet;

return Arguments.CreateParser()
                .WithVersion("1.0.0rc2")
                .Run(args, Main);

static int Main(Arguments args)
{
    foreach (var (name, value) in args)
        Console.WriteLine($"{name} = {value}");

    Console.WriteLine($@"{{
    Tcp     = {args.CmdTcp    },
    Host    = {args.ArgHost   },
    Port    = {args.ArgPort   },
    Timeout = {args.OptTimeout},
    Serial  = {args.CmdSerial },
    Baud    = {args.OptBaud   },
    H       = {args.OptH      },
    Help    = {args.OptHelp   },
    Version = {args.OptVersion},
}}");

    return 0;
}

[DocoptArguments]
partial class Arguments
{
    public const string Help = @"Usage:
  QuickExample tcp <host> <port> [--timeout=<seconds>]
  QuickExample serial <port> [--baud=9600] [--timeout=<seconds>]
  QuickExample -h | --help | --version
";
}
