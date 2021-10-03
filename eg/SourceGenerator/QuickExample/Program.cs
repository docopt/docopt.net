using System;
using DocoptNet;

Arguments arguments;

try
{
    arguments = Arguments.Apply(args, version: "1.0.0rc2");
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
    Tcp     = {arguments.CmdTcp    },
    Host    = {arguments.ArgHost   },
    Port    = {arguments.ArgPort   },
    Timeout = {arguments.OptTimeout},
    Serial  = {arguments.CmdSerial },
    Baud    = {arguments.OptBaud   },
    H       = {arguments.OptH      },
    Help    = {arguments.OptHelp   },
    Version = {arguments.OptVersion},
}}");

return 0;

[DocoptArguments]
partial class Arguments
{
    public const string Help = @"Usage:
  QuickExample tcp <host> <port> [--timeout=<seconds>]
  QuickExample serial <port> [--baud=9600] [--timeout=<seconds>]
  QuickExample -h | --help | --version
";
}
