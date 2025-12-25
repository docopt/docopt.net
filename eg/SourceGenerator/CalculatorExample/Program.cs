using System;
using DocoptNet;

return Arguments.CreateParser().Run(args, Main);

static int Main(Arguments args)
{
    foreach (var (name, value) in args)
        Console.WriteLine($"{name} = {value}");

    Console.WriteLine($$"""
        {
            Help     = {{args.OptHelp}},
            +        = {{args.CmdPlus}},
            -        = {{args.CmdMinus}},
            *        = {{args.CmdStar}},
            /        = {{args.CmdSlash}},
            Function = {{args.ArgFunction}},
            Comma    = {{args.CmdComma}},
            Value    = [{{string.Join(", ", args.ArgValue)}}],
        }
        """);

    return 0;
}

[DocoptArguments]
partial class Arguments
{
    public const string Help = """
        Not a serious example.

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

        """;
}
