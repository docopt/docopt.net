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
    Help = {arguments.OptHelp},
    Odd  = [{string.Join(", ", arguments.ArgOdd)}],
    Even = [{string.Join(", ", arguments.ArgEven)}],
}}");

return 0;

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
