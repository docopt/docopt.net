using System;
using DocoptNet.Generated;
using OddEvenExample;

ProgramArguments arguments;

try
{
    arguments = ProgramArguments.Apply(args, exit: true);
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
