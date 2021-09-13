using System;
using DocoptNet.Generated;
using CalculatorExample;

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
    Help     = {arguments.OptHelp    },
    +        = {arguments.CmdPlus    },
    -        = {arguments.CmdMinus   },
    *        = {arguments.CmdStar    },
    /        = {arguments.CmdSlash   },
    Function = {arguments.ArgFunction},
    Comma    = {arguments.CmdComma   },
    Value    = [{string.Join(", ", arguments.ArgValue)}],
}}");

return 0;
