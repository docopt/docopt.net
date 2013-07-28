

using System.Collections;
using System.Collections.Generic;
using DocoptNet;

namespace T4DocoptNetHostApp
{
    // Generated class for Another.usage.txt
	public class AnotherArgs
	{
		public const string USAGE = @"Test host app for T4 Docopt.NET

Usage:
  other command1 ARG [-o --long=ARG --switch -v]
  other command2

Options:
 -o   A string with some double ""quotes"".
";
	    private readonly IDictionary<string, ValueObject> _args;
		public AnotherArgs(ICollection<string> argv, bool help = true,
                                                      object version = null, bool optionsFirst = false, bool exit = false)
		{
			_args = new Docopt().Apply(USAGE, argv, help, version, optionsFirst, exit);
		}

        public IDictionary<string, ValueObject> Args
        {
            get { return _args; }
        }

		public bool CmdCommand1 { get { return _args["command1"].IsTrue; } }
		public string ArgArg { get { return _args["ARG"].ToString(); } }
		public bool OptO { get { return _args["-o"].IsTrue; } }
		public string OptLong { get { return _args["--long"].ToString(); } }
		public bool OptSwitch { get { return _args["--switch"].IsTrue; } }
		public bool OptV { get { return _args["-v"].IsTrue; } }
		public bool CmdCommand2 { get { return _args["command2"].IsTrue; } }
	
	}

    // Generated class for Main.usage.txt
	public class MainArgs
	{
		public const string USAGE = @"Test host app for T4 Docopt.NET

Usage:
  prog command ARG <myarg> [OPTIONALARG] [-o -s=<arg> --long=ARG --switch]
  prog files FILE...

Options:
 -o           Short switch.
 -s=<arg>     Short option with arg.
 --long=ARG   Long option with arg.
 --swith      Long switch.

Explanation:
 This is a test usage file.
";
	    private readonly IDictionary<string, ValueObject> _args;
		public MainArgs(ICollection<string> argv, bool help = true,
                                                      object version = null, bool optionsFirst = false, bool exit = false)
		{
			_args = new Docopt().Apply(USAGE, argv, help, version, optionsFirst, exit);
		}

        public IDictionary<string, ValueObject> Args
        {
            get { return _args; }
        }

		public bool CmdCommand { get { return _args["command"].IsTrue; } }
		public string ArgArg { get { return _args["ARG"].ToString(); } }
		public string ArgMyarg  { get { return _args["<myarg>"].ToString(); } }
		public string ArgOptionalarg { get { return _args["OPTIONALARG"].ToString(); } }
		public bool OptO { get { return _args["-o"].IsTrue; } }
		public string OptS { get { return _args["-s"].ToString(); } }
		public string OptLong { get { return _args["--long"].ToString(); } }
		public bool OptSwitch { get { return _args["--switch"].IsTrue; } }
		public bool CmdFiles { get { return _args["files"].IsTrue; } }
		public ArrayList ArgFile { get { return _args["FILE"].AsList; } }
	
	}

	
}


