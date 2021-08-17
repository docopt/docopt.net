using System;
using System.Collections.Generic;
using DocoptNet.Generated;
using OptionsShortcutExample;

Program program;

try
{
    program = new Program(args, version: "1.0.0rc2", exit: true);
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

foreach (var (name, value) in program.Args)
    Console.WriteLine("{0} = {1}", name, value);

Console.WriteLine($@"{{
    Help    = {program.OptHelp   },
    Version = {program.OptVersion},
    Number  = {program.OptNumber },
    Timeout = {program.OptTimeout},
    Apply   = {program.OptApply  },
    Q       = {program.OptQ      },
    Port    = {program.ArgPort   },
}}");

return 0;

namespace OptionsShortcutExample
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
