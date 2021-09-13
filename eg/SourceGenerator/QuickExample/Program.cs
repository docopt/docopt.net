using System;
using DocoptNet.Generated;
using QuickExample;

ProgramArguments arguments;

try
{
    arguments = ProgramArguments.Apply(args, version: "1.0.0rc2");
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
