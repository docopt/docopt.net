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
    Help       = {arguments.OptHelp },
    V          = {arguments.OptV    },
    Go         = {arguments.CmdGo   },
    File       = [{string.Join(", ", arguments.ArgFile)}],
    Path       = {arguments.OptPath },
}}");

return 0;

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
