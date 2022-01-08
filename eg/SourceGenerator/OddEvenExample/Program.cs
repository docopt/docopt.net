using System;
using DocoptNet;

return Arguments.Parse(args).Run(args =>
{
    foreach (var (name, value) in args)
        Console.WriteLine($"{name} = {value}");

    Console.WriteLine($@"{{
    Help = {args.OptHelp},
    Odd  = [{string.Join(", ", args.ArgOdd)}],
    Even = [{string.Join(", ", args.ArgEven)}],
}}");

    return 0;
});

[DocoptArguments]
partial class Arguments
{
    public const string Help = @"Usage: OddEvenExample [-h | --help] (ODD EVEN)...

Example, try:
  OddEvenExample 1 2 3 4

Options:
  -h, --help
";
}
