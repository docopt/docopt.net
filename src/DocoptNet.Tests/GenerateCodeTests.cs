using System;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace DocoptNet.Tests
{
    [TestFixture]
    public class GenerateCodeTests
    {
        [Test]
        public void Should_generate_code()
        {
            // TODO: not a very useful test, more of a sanity check

            const string USAGE = @"Test host app for T4 Docopt.NET

Usage:
  prog command ARG <myarg> [OPTIONALARG] [-o -s=<arg> --long=ARG --switch-dash]
  prog command2 ARG <myarg>
  prog files FILE...

Options:
 -o                Short switch.
 -s=<arg>          Short option with arg. [default: 128]
 --long=ARG        Long option with arg.
 --switch-dash     Long switch.

Explanation:
 This is a test usage file.
";
            const string expected = @"
public bool CmdCommand { get { return _args[""command""].IsTrue; } }
public string ArgArg { get { return null == _args[""ARG""] ? null : _args[""ARG""].ToString(); } }
public string ArgMyarg  { get { return null == _args[""<myarg>""] ? null : _args[""<myarg>""].ToString(); } }
public string ArgOptionalarg { get { return null == _args[""OPTIONALARG""] ? null : _args[""OPTIONALARG""].ToString(); } }
public bool OptO { get { return _args[""-o""].IsTrue; } }
public string OptS { get { return null == _args[""-s""] ? ""128"" : _args[""-s""].ToString(); } }
public string OptLong { get { return null == _args[""--long""] ? null : _args[""--long""].ToString(); } }
public bool OptSwitchDash { get { return _args[""--switch-dash""].IsTrue; } }
public bool CmdCommand2 { get { return _args[""command2""].IsTrue; } }
public bool CmdFiles { get { return _args[""files""].IsTrue; } }
public ArrayList ArgFile { get { return _args[""FILE""].AsList; } }
";
            var s = new Docopt().GenerateCode(USAGE);
            Assert.AreEqual(NormalizeNewLines(expected), NormalizeNewLines(s));
        }

        private static string NormalizeNewLines(string s)
        {
            return string.Join(Environment.NewLine, s.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
        }

        [Test]
        public void Short_and_long_option_lead_to_only_one_property()
        {
            const string usage = @"Usage:
  naval_fate -h | --help

Options:
  -h --help     Show this screen.";

            var generatedCode = new Docopt().GenerateCode(usage);

            Assert.AreEqual(1, Regex.Matches(generatedCode, "OptHelp").Count);
        }
    }
}