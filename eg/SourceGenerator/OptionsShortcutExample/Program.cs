using System;
using DocoptNet.Generated;
using OptionsShortcutExample;

ProgramArguments arguments;

try
{
    arguments = ProgramArguments.Apply(args, version: "1.0.0rc2", exit: true);
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
    Help    = {arguments.OptHelp   },
    Version = {arguments.OptVersion},
    Number  = {arguments.OptNumber },
    Timeout = {arguments.OptTimeout},
    Apply   = {arguments.OptApply  },
    Q       = {arguments.OptQ      },
    Port    = {arguments.ArgPort   },
}}");

return 0;
