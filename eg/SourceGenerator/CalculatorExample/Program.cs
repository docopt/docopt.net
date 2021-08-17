using System;
using System.Collections.Generic;
using System.Linq;
using DocoptNet.Generated;
using CalculatorExample;

Program program;

try
{
    program = new Program(args, exit: true);
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
    Help     = {program.OptHelp    },
    +        = {program.CmdPlus    },
    -        = {program.CmdMinus   },
    *        = {program.CmdStar    },
    /        = {program.CmdSlash   },
    Function = {program.ArgFunction},
    Comma    = {program.CmdComma   },
    Value    = [{string.Join(", ", from object v in program.ArgValue select v)}],
}}");

return 0;

namespace CalculatorExample
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
