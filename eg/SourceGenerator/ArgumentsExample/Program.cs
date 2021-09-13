using System;
using DocoptNet.Generated;
using ArgumentsExample;

ProgramArguments arguments;

try
{
    arguments = ProgramArguments.Apply(args);
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
    Help       = {arguments.OptHelp    },
    V          = {arguments.OptV       },
    Q          = {arguments.OptQ       },
    R          = {arguments.OptR       },
    Left       = {arguments.OptLeft    },
    Right      = {arguments.OptRight   },
    Correction = {arguments.ArgCorrection},
    File       = [{string.Join(", ", arguments.ArgFile)}],
}}");

return 0;
