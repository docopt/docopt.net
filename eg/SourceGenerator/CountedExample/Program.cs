using System;
using DocoptNet.Generated;
using CountedExample;

Program.Arguments arguments;

try
{
    arguments = Program.Arguments.Apply(args, exit: true);
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
