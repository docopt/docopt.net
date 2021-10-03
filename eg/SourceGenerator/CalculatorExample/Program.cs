using System;
using DocoptNet;

Arguments arguments;

try
{
    arguments = Arguments.Apply(args);
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

[DocoptArguments]
partial class Arguments
{
    public const string Help = @"Not a serious example.

Usage:
  calculator_example.py <value> ( ( + | - | * | / ) <value> )...
  calculator_example.py <function> <value> [( , <value> )]...
  calculator_example.py (-h | --help)

Examples:
  calculator_example.py 1 + 2 + 3 + 4 + 5
  calculator_example.py 1 + 2 '*' 3 / 4 - 5    # note quotes around '*'
  calculator_example.py sum 10 , 20 , 30 , 40

Options:
  -h, --help
";
}
