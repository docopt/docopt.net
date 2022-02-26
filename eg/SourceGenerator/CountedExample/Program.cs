using System;
using DocoptNet;

return Arguments.CreateParser().Run(args, Main);

static int Main(Arguments args)
{
    foreach (var (name, value) in args)
        Console.WriteLine($"{name} = {value}");

    Console.WriteLine($@"{{
    Help       = {args.OptHelp},
    V          = {args.OptV},
    Go         = {args.CmdGo},
    File       = [{string.Join(", ", args.ArgFile)}],
    Path       = {args.OptPath},
}}");

    return 0;
}

[DocoptArguments]
partial class Arguments
{
    public const string Help = @"Usage: CountedExample --help
       CountedExample -v...
       CountedExample go [go]
       CountedExample (--path=<path>)...
       CountedExample <file> <file>

Try: CountedExample -vvvvvvvvvv
     CountedExample go go
     CountedExample --path ./here --path ./there
     CountedExample this.txt that.txt
";
}
