using System;
using System.Collections.Generic;
using System.Linq;
using DocoptNet.Generated;
using CountedExample;

Program program;

try
{
    program = new Program(args, exit: true);
}
catch (DocoptInputErrorException e)
{
    Console.Error.WriteLine(e);
    return 0xbd;
}

foreach (var (name, value) in program.Args)
    Console.WriteLine("{0} = {1}", name, value);

Console.WriteLine($@"{{
    Help       = {program.OptHelp },
    V          = {program.OptV    },
    Go         = {program.CmdGo   },
    File       = [{string.Join(", ", from object file in program.ArgFile select file)}],
    Path       = {program.OptPath },
}}");

return 0;

namespace CountedExample
{
    partial class Program
    {
        readonly IDictionary<string, ValueObject> _args;

        public Program(ICollection<string> argv, bool help = true,
                       object version = null, bool optionsFirst = false, bool exit = false)
        {
            _args = Apply(argv, help, version, optionsFirst, exit);
        }

        public IDictionary<string, ValueObject> Args => _args;
    }
}
