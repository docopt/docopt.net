
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
	    private readonly IDictionary<string, object> _args;
		public AnotherArgs(ICollection<string> argv, bool help = true,
                                                      object version = null, bool optionsFirst = false, bool exit = false)
		{
			_args = new Docopt().Apply(USAGE, argv, help, version, optionsFirst, exit);
		}

        public IDictionary<string, object> Args
        {
            get { return _args; }
        }

		public bool CmdCommand1 { get { return _args["command1"] as bool? == true; } }
		public string ArgArg { get { return null == _args["ARG"] ? null : _args["ARG"].ToString(); } }
		public bool OptO { get { return _args["-o"] as bool? == true; } }
		public string OptLong { get { return null == _args["--long"] ? null : _args["--long"].ToString(); } }
		public bool OptSwitch { get { return _args["--switch"] as bool? == true; } }
		public bool OptV { get { return _args["-v"] as bool? == true; } }
		public bool CmdCommand2 { get { return _args["command2"] as bool? == true; } }
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
 --switch     Long switch.

Explanation:
 This is a test usage file.
";
	    private readonly IDictionary<string, object> _args;
		public MainArgs(ICollection<string> argv, bool help = true,
                                                      object version = null, bool optionsFirst = false, bool exit = false)
		{
			_args = new Docopt().Apply(USAGE, argv, help, version, optionsFirst, exit);
		}

        public IDictionary<string, object> Args
        {
            get { return _args; }
        }

		public bool CmdCommand { get { return _args["command"] as bool? == true; } }
		public string ArgArg { get { return null == _args["ARG"] ? null : _args["ARG"].ToString(); } }
		public string ArgMyarg  { get { return null == _args["<myarg>"] ? null : _args["<myarg>"].ToString(); } }
		public string ArgOptionalarg { get { return null == _args["OPTIONALARG"] ? null : _args["OPTIONALARG"].ToString(); } }
		public bool OptO { get { return _args["-o"] as bool? == true; } }
		public string OptS { get { return null == _args["-s"] ? null : _args["-s"].ToString(); } }
		public string OptLong { get { return null == _args["--long"] ? null : _args["--long"].ToString(); } }
		public bool OptSwitch { get { return _args["--switch"] as bool? == true; } }
		public bool CmdFiles { get { return _args["files"] as bool? == true; } }
		public ArrayList ArgFile { get { return (ArrayList)_args["FILE"]; } }
	}

}
