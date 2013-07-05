using System.Collections.Generic;
using NUnit.Framework;

namespace NDocOpt.Tests
{
    [TestFixture]
    public partial class LanguageAgnosticTests
    {
      #region Language agnostic tests generated code
    

      [Test]
      public void Test_1()
      {
          var doc = @"Usage: prog

";
          var actual = DocOpt(doc, @"");
          var expected = @"{}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_2()
      {
          var doc = @"Usage: prog

";
          var actual = DocOpt(doc, @"--xxx");
          var expected = @"""user-error""";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_3()
      {
          var doc = @"Usage: prog [options]

Options: -a  All.

";
          var actual = DocOpt(doc, @"");
          var expected = @"{""-a"": false}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_4()
      {
          var doc = @"Usage: prog [options]

Options: -a  All.

";
          var actual = DocOpt(doc, @"-a");
          var expected = @"{""-a"": true}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_5()
      {
          var doc = @"Usage: prog [options]

Options: -a  All.

";
          var actual = DocOpt(doc, @"-x");
          var expected = @"""user-error""";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_6()
      {
          var doc = @"Usage: prog [options]

Options: --all  All.

";
          var actual = DocOpt(doc, @"");
          var expected = @"{""--all"": false}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_7()
      {
          var doc = @"Usage: prog [options]

Options: --all  All.

";
          var actual = DocOpt(doc, @"--all");
          var expected = @"{""--all"": true}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_8()
      {
          var doc = @"Usage: prog [options]

Options: --all  All.

";
          var actual = DocOpt(doc, @"--xxx");
          var expected = @"""user-error""";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_9()
      {
          var doc = @"Usage: prog [options]

Options: -v, --verbose  Verbose.

";
          var actual = DocOpt(doc, @"--verbose");
          var expected = @"{""--verbose"": true}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_10()
      {
          var doc = @"Usage: prog [options]

Options: -v, --verbose  Verbose.

";
          var actual = DocOpt(doc, @"--ver");
          var expected = @"{""--verbose"": true}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_11()
      {
          var doc = @"Usage: prog [options]

Options: -v, --verbose  Verbose.

";
          var actual = DocOpt(doc, @"-v");
          var expected = @"{""--verbose"": true}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_12()
      {
          var doc = @"Usage: prog [options]

Options: -p PATH

";
          var actual = DocOpt(doc, @"-p home/");
          var expected = @"{""-p"": ""home/""}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_13()
      {
          var doc = @"Usage: prog [options]

Options: -p PATH

";
          var actual = DocOpt(doc, @"-phome/");
          var expected = @"{""-p"": ""home/""}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_14()
      {
          var doc = @"Usage: prog [options]

Options: -p PATH

";
          var actual = DocOpt(doc, @"-p");
          var expected = @"""user-error""";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_15()
      {
          var doc = @"Usage: prog [options]

Options: --path <path>

";
          var actual = DocOpt(doc, @"--path home/");
          var expected = @"{""--path"": ""home/""}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_16()
      {
          var doc = @"Usage: prog [options]

Options: --path <path>

";
          var actual = DocOpt(doc, @"--path=home/");
          var expected = @"{""--path"": ""home/""}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_17()
      {
          var doc = @"Usage: prog [options]

Options: --path <path>

";
          var actual = DocOpt(doc, @"--pa home/");
          var expected = @"{""--path"": ""home/""}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_18()
      {
          var doc = @"Usage: prog [options]

Options: --path <path>

";
          var actual = DocOpt(doc, @"--pa=home/");
          var expected = @"{""--path"": ""home/""}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_19()
      {
          var doc = @"Usage: prog [options]

Options: --path <path>

";
          var actual = DocOpt(doc, @"--path");
          var expected = @"""user-error""";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_20()
      {
          var doc = @"Usage: prog [options]

Options: -p PATH, --path=<path>  Path to files.

";
          var actual = DocOpt(doc, @"-proot");
          var expected = @"{""--path"": ""root""}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_21()
      {
          var doc = @"Usage: prog [options]

Options:    -p --path PATH  Path to files.

";
          var actual = DocOpt(doc, @"-p root");
          var expected = @"{""--path"": ""root""}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_22()
      {
          var doc = @"Usage: prog [options]

Options:    -p --path PATH  Path to files.

";
          var actual = DocOpt(doc, @"--path root");
          var expected = @"{""--path"": ""root""}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_23()
      {
          var doc = @"Usage: prog [options]

Options:
 -p PATH  Path to files [default: ./]

";
          var actual = DocOpt(doc, @"");
          var expected = @"{""-p"": ""./""}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_24()
      {
          var doc = @"Usage: prog [options]

Options:
 -p PATH  Path to files [default: ./]

";
          var actual = DocOpt(doc, @"-phome");
          var expected = @"{""-p"": ""home""}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_25()
      {
          var doc = @"UsAgE: prog [options]

OpTiOnS: --path=<files>  Path to files
                [dEfAuLt: /root]

";
          var actual = DocOpt(doc, @"");
          var expected = @"{""--path"": ""/root""}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_26()
      {
          var doc = @"UsAgE: prog [options]

OpTiOnS: --path=<files>  Path to files
                [dEfAuLt: /root]

";
          var actual = DocOpt(doc, @"--path=home");
          var expected = @"{""--path"": ""home""}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_27()
      {
          var doc = @"usage: prog [options]

options:
    -a        Add
    -r        Remote
    -m <msg>  Message

";
          var actual = DocOpt(doc, @"-a -r -m Hello");
          var expected = @"{""-a"": true,
 ""-r"": true,
 ""-m"": ""Hello""}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_28()
      {
          var doc = @"usage: prog [options]

options:
    -a        Add
    -r        Remote
    -m <msg>  Message

";
          var actual = DocOpt(doc, @"-armyourass");
          var expected = @"{""-a"": true,
 ""-r"": true,
 ""-m"": ""yourass""}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_29()
      {
          var doc = @"usage: prog [options]

options:
    -a        Add
    -r        Remote
    -m <msg>  Message

";
          var actual = DocOpt(doc, @"-a -r");
          var expected = @"{""-a"": true,
 ""-r"": true,
 ""-m"": null}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_30()
      {
          var doc = @"Usage: prog [options]

Options: --version
         --verbose

";
          var actual = DocOpt(doc, @"--version");
          var expected = @"{""--version"": true,
 ""--verbose"": false}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_31()
      {
          var doc = @"Usage: prog [options]

Options: --version
         --verbose

";
          var actual = DocOpt(doc, @"--verbose");
          var expected = @"{""--version"": false,
 ""--verbose"": true}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_32()
      {
          var doc = @"Usage: prog [options]

Options: --version
         --verbose

";
          var actual = DocOpt(doc, @"--ver");
          var expected = @"""user-error""";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_33()
      {
          var doc = @"Usage: prog [options]

Options: --version
         --verbose

";
          var actual = DocOpt(doc, @"--verb");
          var expected = @"{""--version"": false,
 ""--verbose"": true}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_34()
      {
          var doc = @"usage: prog [-a -r -m <msg>]

options:
 -a        Add
 -r        Remote
 -m <msg>  Message

";
          var actual = DocOpt(doc, @"-armyourass");
          var expected = @"{""-a"": true,
 ""-r"": true,
 ""-m"": ""yourass""}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_35()
      {
          var doc = @"usage: prog [-armmsg]

options: -a        Add
         -r        Remote
         -m <msg>  Message

";
          var actual = DocOpt(doc, @"-a -r -m Hello");
          var expected = @"{""-a"": true,
 ""-r"": true,
 ""-m"": ""Hello""}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_36()
      {
          var doc = @"usage: prog -a -b

options:
 -a
 -b

";
          var actual = DocOpt(doc, @"-a -b");
          var expected = @"{""-a"": true, ""-b"": true}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_37()
      {
          var doc = @"usage: prog -a -b

options:
 -a
 -b

";
          var actual = DocOpt(doc, @"-b -a");
          var expected = @"{""-a"": true, ""-b"": true}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_38()
      {
          var doc = @"usage: prog -a -b

options:
 -a
 -b

";
          var actual = DocOpt(doc, @"-a");
          var expected = @"""user-error""";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_39()
      {
          var doc = @"usage: prog -a -b

options:
 -a
 -b

";
          var actual = DocOpt(doc, @"");
          var expected = @"""user-error""";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_40()
      {
          var doc = @"usage: prog (-a -b)

options: -a
         -b

";
          var actual = DocOpt(doc, @"-a -b");
          var expected = @"{""-a"": true, ""-b"": true}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_41()
      {
          var doc = @"usage: prog (-a -b)

options: -a
         -b

";
          var actual = DocOpt(doc, @"-b -a");
          var expected = @"{""-a"": true, ""-b"": true}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_42()
      {
          var doc = @"usage: prog (-a -b)

options: -a
         -b

";
          var actual = DocOpt(doc, @"-a");
          var expected = @"""user-error""";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_43()
      {
          var doc = @"usage: prog (-a -b)

options: -a
         -b

";
          var actual = DocOpt(doc, @"");
          var expected = @"""user-error""";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_44()
      {
          var doc = @"usage: prog [-a] -b

options: -a
 -b

";
          var actual = DocOpt(doc, @"-a -b");
          var expected = @"{""-a"": true, ""-b"": true}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_45()
      {
          var doc = @"usage: prog [-a] -b

options: -a
 -b

";
          var actual = DocOpt(doc, @"-b -a");
          var expected = @"{""-a"": true, ""-b"": true}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_46()
      {
          var doc = @"usage: prog [-a] -b

options: -a
 -b

";
          var actual = DocOpt(doc, @"-a");
          var expected = @"""user-error""";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_47()
      {
          var doc = @"usage: prog [-a] -b

options: -a
 -b

";
          var actual = DocOpt(doc, @"-b");
          var expected = @"{""-a"": false, ""-b"": true}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_48()
      {
          var doc = @"usage: prog [-a] -b

options: -a
 -b

";
          var actual = DocOpt(doc, @"");
          var expected = @"""user-error""";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_49()
      {
          var doc = @"usage: prog [(-a -b)]

options: -a
         -b

";
          var actual = DocOpt(doc, @"-a -b");
          var expected = @"{""-a"": true, ""-b"": true}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_50()
      {
          var doc = @"usage: prog [(-a -b)]

options: -a
         -b

";
          var actual = DocOpt(doc, @"-b -a");
          var expected = @"{""-a"": true, ""-b"": true}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_51()
      {
          var doc = @"usage: prog [(-a -b)]

options: -a
         -b

";
          var actual = DocOpt(doc, @"-a");
          var expected = @"""user-error""";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_52()
      {
          var doc = @"usage: prog [(-a -b)]

options: -a
         -b

";
          var actual = DocOpt(doc, @"-b");
          var expected = @"""user-error""";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_53()
      {
          var doc = @"usage: prog [(-a -b)]

options: -a
         -b

";
          var actual = DocOpt(doc, @"");
          var expected = @"{""-a"": false, ""-b"": false}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_54()
      {
          var doc = @"usage: prog (-a|-b)

options: -a
         -b

";
          var actual = DocOpt(doc, @"-a -b");
          var expected = @"""user-error""";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_55()
      {
          var doc = @"usage: prog (-a|-b)

options: -a
         -b

";
          var actual = DocOpt(doc, @"");
          var expected = @"""user-error""";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_56()
      {
          var doc = @"usage: prog (-a|-b)

options: -a
         -b

";
          var actual = DocOpt(doc, @"-a");
          var expected = @"{""-a"": true, ""-b"": false}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_57()
      {
          var doc = @"usage: prog (-a|-b)

options: -a
         -b

";
          var actual = DocOpt(doc, @"-b");
          var expected = @"{""-a"": false, ""-b"": true}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_58()
      {
          var doc = @"usage: prog [ -a | -b ]

options: -a
         -b

";
          var actual = DocOpt(doc, @"-a -b");
          var expected = @"""user-error""";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_59()
      {
          var doc = @"usage: prog [ -a | -b ]

options: -a
         -b

";
          var actual = DocOpt(doc, @"");
          var expected = @"{""-a"": false, ""-b"": false}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_60()
      {
          var doc = @"usage: prog [ -a | -b ]

options: -a
         -b

";
          var actual = DocOpt(doc, @"-a");
          var expected = @"{""-a"": true, ""-b"": false}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_61()
      {
          var doc = @"usage: prog [ -a | -b ]

options: -a
         -b

";
          var actual = DocOpt(doc, @"-b");
          var expected = @"{""-a"": false, ""-b"": true}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_62()
      {
          var doc = @"usage: prog <arg>";
          var actual = DocOpt(doc, @"10");
          var expected = @"{""<arg>"": ""10""}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_63()
      {
          var doc = @"usage: prog <arg>";
          var actual = DocOpt(doc, @"10 20");
          var expected = @"""user-error""";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_64()
      {
          var doc = @"usage: prog <arg>";
          var actual = DocOpt(doc, @"");
          var expected = @"""user-error""";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_65()
      {
          var doc = @"usage: prog [<arg>]";
          var actual = DocOpt(doc, @"10");
          var expected = @"{""<arg>"": ""10""}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_66()
      {
          var doc = @"usage: prog [<arg>]";
          var actual = DocOpt(doc, @"10 20");
          var expected = @"""user-error""";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_67()
      {
          var doc = @"usage: prog [<arg>]";
          var actual = DocOpt(doc, @"");
          var expected = @"{""<arg>"": null}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_68()
      {
          var doc = @"usage: prog <kind> <name> <type>";
          var actual = DocOpt(doc, @"10 20 40");
          var expected = @"{""<kind>"": ""10"", ""<name>"": ""20"", ""<type>"": ""40""}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_69()
      {
          var doc = @"usage: prog <kind> <name> <type>";
          var actual = DocOpt(doc, @"10 20");
          var expected = @"""user-error""";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_70()
      {
          var doc = @"usage: prog <kind> <name> <type>";
          var actual = DocOpt(doc, @"");
          var expected = @"""user-error""";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_71()
      {
          var doc = @"usage: prog <kind> [<name> <type>]";
          var actual = DocOpt(doc, @"10 20 40");
          var expected = @"{""<kind>"": ""10"", ""<name>"": ""20"", ""<type>"": ""40""}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_72()
      {
          var doc = @"usage: prog <kind> [<name> <type>]";
          var actual = DocOpt(doc, @"10 20");
          var expected = @"{""<kind>"": ""10"", ""<name>"": ""20"", ""<type>"": null}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_73()
      {
          var doc = @"usage: prog <kind> [<name> <type>]";
          var actual = DocOpt(doc, @"");
          var expected = @"""user-error""";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_74()
      {
          var doc = @"usage: prog [<kind> | <name> <type>]";
          var actual = DocOpt(doc, @"10 20 40");
          var expected = @"""user-error""";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_75()
      {
          var doc = @"usage: prog [<kind> | <name> <type>]";
          var actual = DocOpt(doc, @"20 40");
          var expected = @"{""<kind>"": null, ""<name>"": ""20"", ""<type>"": ""40""}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_76()
      {
          var doc = @"usage: prog [<kind> | <name> <type>]";
          var actual = DocOpt(doc, @"");
          var expected = @"{""<kind>"": null, ""<name>"": null, ""<type>"": null}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_77()
      {
          var doc = @"usage: prog (<kind> --all | <name>)

options:
 --all

";
          var actual = DocOpt(doc, @"10 --all");
          var expected = @"{""<kind>"": ""10"", ""--all"": true, ""<name>"": null}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_78()
      {
          var doc = @"usage: prog (<kind> --all | <name>)

options:
 --all

";
          var actual = DocOpt(doc, @"10");
          var expected = @"{""<kind>"": null, ""--all"": false, ""<name>"": ""10""}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_79()
      {
          var doc = @"usage: prog (<kind> --all | <name>)

options:
 --all

";
          var actual = DocOpt(doc, @"");
          var expected = @"""user-error""";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_80()
      {
          var doc = @"usage: prog [<name> <name>]";
          var actual = DocOpt(doc, @"10 20");
          var expected = @"{""<name>"": [""10"", ""20""]}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_81()
      {
          var doc = @"usage: prog [<name> <name>]";
          var actual = DocOpt(doc, @"10");
          var expected = @"{""<name>"": [""10""]}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_82()
      {
          var doc = @"usage: prog [<name> <name>]";
          var actual = DocOpt(doc, @"");
          var expected = @"{""<name>"": []}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_83()
      {
          var doc = @"usage: prog [(<name> <name>)]";
          var actual = DocOpt(doc, @"10 20");
          var expected = @"{""<name>"": [""10"", ""20""]}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_84()
      {
          var doc = @"usage: prog [(<name> <name>)]";
          var actual = DocOpt(doc, @"10");
          var expected = @"""user-error""";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_85()
      {
          var doc = @"usage: prog [(<name> <name>)]";
          var actual = DocOpt(doc, @"");
          var expected = @"{""<name>"": []}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_86()
      {
          var doc = @"usage: prog NAME...";
          var actual = DocOpt(doc, @"10 20");
          var expected = @"{""NAME"": [""10"", ""20""]}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_87()
      {
          var doc = @"usage: prog NAME...";
          var actual = DocOpt(doc, @"10");
          var expected = @"{""NAME"": [""10""]}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_88()
      {
          var doc = @"usage: prog NAME...";
          var actual = DocOpt(doc, @"");
          var expected = @"""user-error""";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_89()
      {
          var doc = @"usage: prog [NAME]...";
          var actual = DocOpt(doc, @"10 20");
          var expected = @"{""NAME"": [""10"", ""20""]}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_90()
      {
          var doc = @"usage: prog [NAME]...";
          var actual = DocOpt(doc, @"10");
          var expected = @"{""NAME"": [""10""]}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_91()
      {
          var doc = @"usage: prog [NAME]...";
          var actual = DocOpt(doc, @"");
          var expected = @"{""NAME"": []}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_92()
      {
          var doc = @"usage: prog [NAME...]";
          var actual = DocOpt(doc, @"10 20");
          var expected = @"{""NAME"": [""10"", ""20""]}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_93()
      {
          var doc = @"usage: prog [NAME...]";
          var actual = DocOpt(doc, @"10");
          var expected = @"{""NAME"": [""10""]}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_94()
      {
          var doc = @"usage: prog [NAME...]";
          var actual = DocOpt(doc, @"");
          var expected = @"{""NAME"": []}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_95()
      {
          var doc = @"usage: prog [NAME [NAME ...]]";
          var actual = DocOpt(doc, @"10 20");
          var expected = @"{""NAME"": [""10"", ""20""]}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_96()
      {
          var doc = @"usage: prog [NAME [NAME ...]]";
          var actual = DocOpt(doc, @"10");
          var expected = @"{""NAME"": [""10""]}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_97()
      {
          var doc = @"usage: prog [NAME [NAME ...]]";
          var actual = DocOpt(doc, @"");
          var expected = @"{""NAME"": []}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_98()
      {
          var doc = @"usage: prog (NAME | --foo NAME)

options: --foo

";
          var actual = DocOpt(doc, @"10");
          var expected = @"{""NAME"": ""10"", ""--foo"": false}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_99()
      {
          var doc = @"usage: prog (NAME | --foo NAME)

options: --foo

";
          var actual = DocOpt(doc, @"--foo 10");
          var expected = @"{""NAME"": ""10"", ""--foo"": true}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_100()
      {
          var doc = @"usage: prog (NAME | --foo NAME)

options: --foo

";
          var actual = DocOpt(doc, @"--foo=10");
          var expected = @"""user-error""";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_101()
      {
          var doc = @"usage: prog (NAME | --foo) [--bar | NAME]

options: --foo
options: --bar

";
          var actual = DocOpt(doc, @"10");
          var expected = @"{""NAME"": [""10""], ""--foo"": false, ""--bar"": false}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_102()
      {
          var doc = @"usage: prog (NAME | --foo) [--bar | NAME]

options: --foo
options: --bar

";
          var actual = DocOpt(doc, @"10 20");
          var expected = @"{""NAME"": [""10"", ""20""], ""--foo"": false, ""--bar"": false}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_103()
      {
          var doc = @"usage: prog (NAME | --foo) [--bar | NAME]

options: --foo
options: --bar

";
          var actual = DocOpt(doc, @"--foo --bar");
          var expected = @"{""NAME"": [], ""--foo"": true, ""--bar"": true}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_104()
      {
          var doc = @"Naval Fate.

Usage:
  prog ship new <name>...
  prog ship [<name>] move <x> <y> [--speed=<kn>]
  prog ship shoot <x> <y>
  prog mine (set|remove) <x> <y> [--moored|--drifting]
  prog -h | --help
  prog --version

Options:
  -h --help     Show this screen.
  --version     Show version.
  --speed=<kn>  Speed in knots [default: 10].
  --moored      Mored (anchored) mine.
  --drifting    Drifting mine.

";
          var actual = DocOpt(doc, @"ship Guardian move 150 300 --speed=20");
          var expected = @"{""--drifting"": false,
 ""--help"": false,
 ""--moored"": false,
 ""--speed"": ""20"",
 ""--version"": false,
 ""<name>"": [""Guardian""],
 ""<x>"": ""150"",
 ""<y>"": ""300"",
 ""mine"": false,
 ""move"": true,
 ""new"": false,
 ""remove"": false,
 ""set"": false,
 ""ship"": true,
 ""shoot"": false}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_105()
      {
          var doc = @"usage: prog --hello";
          var actual = DocOpt(doc, @"--hello");
          var expected = @"{""--hello"": true}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_106()
      {
          var doc = @"usage: prog [--hello=<world>]";
          var actual = DocOpt(doc, @"");
          var expected = @"{""--hello"": null}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_107()
      {
          var doc = @"usage: prog [--hello=<world>]";
          var actual = DocOpt(doc, @"--hello wrld");
          var expected = @"{""--hello"": ""wrld""}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_108()
      {
          var doc = @"usage: prog [-o]";
          var actual = DocOpt(doc, @"");
          var expected = @"{""-o"": false}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_109()
      {
          var doc = @"usage: prog [-o]";
          var actual = DocOpt(doc, @"-o");
          var expected = @"{""-o"": true}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_110()
      {
          var doc = @"usage: prog [-opr]";
          var actual = DocOpt(doc, @"-op");
          var expected = @"{""-o"": true, ""-p"": true, ""-r"": false}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_111()
      {
          var doc = @"usage: prog --aabb | --aa";
          var actual = DocOpt(doc, @"--aa");
          var expected = @"{""--aabb"": false, ""--aa"": true}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_112()
      {
          var doc = @"usage: prog --aabb | --aa";
          var actual = DocOpt(doc, @"--a");
          var expected = @"""user-error""";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_113()
      {
          var doc = @"Usage: prog -v";
          var actual = DocOpt(doc, @"-v");
          var expected = @"{""-v"": true}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_114()
      {
          var doc = @"Usage: prog [-v -v]";
          var actual = DocOpt(doc, @"");
          var expected = @"{""-v"": 0}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_115()
      {
          var doc = @"Usage: prog [-v -v]";
          var actual = DocOpt(doc, @"-v");
          var expected = @"{""-v"": 1}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_116()
      {
          var doc = @"Usage: prog [-v -v]";
          var actual = DocOpt(doc, @"-vv");
          var expected = @"{""-v"": 2}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_117()
      {
          var doc = @"Usage: prog -v ...";
          var actual = DocOpt(doc, @"");
          var expected = @"""user-error""";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_118()
      {
          var doc = @"Usage: prog -v ...";
          var actual = DocOpt(doc, @"-v");
          var expected = @"{""-v"": 1}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_119()
      {
          var doc = @"Usage: prog -v ...";
          var actual = DocOpt(doc, @"-vv");
          var expected = @"{""-v"": 2}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_120()
      {
          var doc = @"Usage: prog -v ...";
          var actual = DocOpt(doc, @"-vvvvvv");
          var expected = @"{""-v"": 6}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_121()
      {
          var doc = @"Usage: prog [-v | -vv | -vvv]

This one is probably most readable user-friednly variant.

";
          var actual = DocOpt(doc, @"");
          var expected = @"{""-v"": 0}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_122()
      {
          var doc = @"Usage: prog [-v | -vv | -vvv]

This one is probably most readable user-friednly variant.

";
          var actual = DocOpt(doc, @"-v");
          var expected = @"{""-v"": 1}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_123()
      {
          var doc = @"Usage: prog [-v | -vv | -vvv]

This one is probably most readable user-friednly variant.

";
          var actual = DocOpt(doc, @"-vv");
          var expected = @"{""-v"": 2}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_124()
      {
          var doc = @"Usage: prog [-v | -vv | -vvv]

This one is probably most readable user-friednly variant.

";
          var actual = DocOpt(doc, @"-vvvv");
          var expected = @"""user-error""";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_125()
      {
          var doc = @"usage: prog [--ver --ver]";
          var actual = DocOpt(doc, @"--ver --ver");
          var expected = @"{""--ver"": 2}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_126()
      {
          var doc = @"usage: prog [go]";
          var actual = DocOpt(doc, @"go");
          var expected = @"{""go"": true}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_127()
      {
          var doc = @"usage: prog [go go]";
          var actual = DocOpt(doc, @"");
          var expected = @"{""go"": 0}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_128()
      {
          var doc = @"usage: prog [go go]";
          var actual = DocOpt(doc, @"go");
          var expected = @"{""go"": 1}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_129()
      {
          var doc = @"usage: prog [go go]";
          var actual = DocOpt(doc, @"go go");
          var expected = @"{""go"": 2}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_130()
      {
          var doc = @"usage: prog [go go]";
          var actual = DocOpt(doc, @"go go go");
          var expected = @"""user-error""";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_131()
      {
          var doc = @"usage: prog go...";
          var actual = DocOpt(doc, @"go go go go go");
          var expected = @"{""go"": 5}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_132()
      {
          var doc = @"usage: prog [options] [-a]

options: -a
         -b
";
          var actual = DocOpt(doc, @"-a");
          var expected = @"{""-a"": true, ""-b"": false}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_133()
      {
          var doc = @"usage: prog [options] [-a]

options: -a
         -b
";
          var actual = DocOpt(doc, @"-aa");
          var expected = @"""user-error""";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_134()
      {
          var doc = @"Usage: prog [options] A
Options:
    -q  Be quiet
    -v  Be verbose.

";
          var actual = DocOpt(doc, @"arg");
          var expected = @"{""A"": ""arg"", ""-v"": false, ""-q"": false}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_135()
      {
          var doc = @"Usage: prog [options] A
Options:
    -q  Be quiet
    -v  Be verbose.

";
          var actual = DocOpt(doc, @"-v arg");
          var expected = @"{""A"": ""arg"", ""-v"": true, ""-q"": false}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_136()
      {
          var doc = @"Usage: prog [options] A
Options:
    -q  Be quiet
    -v  Be verbose.

";
          var actual = DocOpt(doc, @"-q arg");
          var expected = @"{""A"": ""arg"", ""-v"": false, ""-q"": true}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_137()
      {
          var doc = @"usage: prog [-]";
          var actual = DocOpt(doc, @"-");
          var expected = @"{""-"": true}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_138()
      {
          var doc = @"usage: prog [-]";
          var actual = DocOpt(doc, @"");
          var expected = @"{""-"": false}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_139()
      {
          var doc = @"usage: prog [NAME [NAME ...]]";
          var actual = DocOpt(doc, @"a b");
          var expected = @"{""NAME"": [""a"", ""b""]}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_140()
      {
          var doc = @"usage: prog [NAME [NAME ...]]";
          var actual = DocOpt(doc, @"");
          var expected = @"{""NAME"": []}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_141()
      {
          var doc = @"usage: prog [options]
options:
 -a        Add
 -m <msg>  Message

";
          var actual = DocOpt(doc, @"-a");
          var expected = @"{""-m"": null, ""-a"": true}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_142()
      {
          var doc = @"usage: prog --hello";
          var actual = DocOpt(doc, @"--hello");
          var expected = @"{""--hello"": true}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_143()
      {
          var doc = @"usage: prog [--hello=<world>]";
          var actual = DocOpt(doc, @"");
          var expected = @"{""--hello"": null}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_144()
      {
          var doc = @"usage: prog [--hello=<world>]";
          var actual = DocOpt(doc, @"--hello wrld");
          var expected = @"{""--hello"": ""wrld""}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_145()
      {
          var doc = @"usage: prog [-o]";
          var actual = DocOpt(doc, @"");
          var expected = @"{""-o"": false}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_146()
      {
          var doc = @"usage: prog [-o]";
          var actual = DocOpt(doc, @"-o");
          var expected = @"{""-o"": true}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_147()
      {
          var doc = @"usage: prog [-opr]";
          var actual = DocOpt(doc, @"-op");
          var expected = @"{""-o"": true, ""-p"": true, ""-r"": false}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_148()
      {
          var doc = @"usage: git [-v | --verbose]";
          var actual = DocOpt(doc, @"-v");
          var expected = @"{""-v"": true, ""--verbose"": false}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_149()
      {
          var doc = @"usage: git remote [-v | --verbose]";
          var actual = DocOpt(doc, @"remote -v");
          var expected = @"{""remote"": true, ""-v"": true, ""--verbose"": false}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_150()
      {
          var doc = @"usage: prog";
          var actual = DocOpt(doc, @"");
          var expected = @"{}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_151()
      {
          var doc = @"usage: prog
           prog <a> <b>
";
          var actual = DocOpt(doc, @"1 2");
          var expected = @"{""<a>"": ""1"", ""<b>"": ""2""}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_152()
      {
          var doc = @"usage: prog
           prog <a> <b>
";
          var actual = DocOpt(doc, @"");
          var expected = @"{""<a>"": null, ""<b>"": null}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_153()
      {
          var doc = @"usage: prog <a> <b>
           prog
";
          var actual = DocOpt(doc, @"");
          var expected = @"{""<a>"": null, ""<b>"": null}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_154()
      {
          var doc = @"usage: prog [--file=<f>]";
          var actual = DocOpt(doc, @"");
          var expected = @"{""--file"": null}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_155()
      {
          var doc = @"usage: prog [--file=<f>]

options: --file <a>

";
          var actual = DocOpt(doc, @"");
          var expected = @"{""--file"": null}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_156()
      {
          var doc = @"Usage: prog [-a <host:port>]

Options: -a, --address <host:port>  TCP address [default: localhost:6283].

";
          var actual = DocOpt(doc, @"");
          var expected = @"{""--address"": ""localhost:6283""}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_157()
      {
          var doc = @"usage: prog --long=<arg> ...";
          var actual = DocOpt(doc, @"--long one");
          var expected = @"{""--long"": [""one""]}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_158()
      {
          var doc = @"usage: prog --long=<arg> ...";
          var actual = DocOpt(doc, @"--long one --long two");
          var expected = @"{""--long"": [""one"", ""two""]}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_159()
      {
          var doc = @"usage: prog (go <direction> --speed=<km/h>)...";
          var actual = DocOpt(doc, @" go left --speed=5  go right --speed=9");
          var expected = @"{""go"": 2, ""<direction>"": [""left"", ""right""], ""--speed"": [""5"", ""9""]}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_160()
      {
          var doc = @"usage: prog [options] -a

options: -a

";
          var actual = DocOpt(doc, @"-a");
          var expected = @"{""-a"": true}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_161()
      {
          var doc = @"usage: prog [-o <o>]...

options: -o <o>  [default: x]

";
          var actual = DocOpt(doc, @"-o this -o that");
          var expected = @"{""-o"": [""this"", ""that""]}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_162()
      {
          var doc = @"usage: prog [-o <o>]...

options: -o <o>  [default: x]

";
          var actual = DocOpt(doc, @"");
          var expected = @"{""-o"": [""x""]}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_163()
      {
          var doc = @"usage: prog [-o <o>]...

options: -o <o>  [default: x y]

";
          var actual = DocOpt(doc, @"-o this");
          var expected = @"{""-o"": [""this""]}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_164()
      {
          var doc = @"usage: prog [-o <o>]...

options: -o <o>  [default: x y]

";
          var actual = DocOpt(doc, @"");
          var expected = @"{""-o"": [""x"", ""y""]}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_165()
      {
          var doc = @"usage: prog -pPATH

options: -p PATH

";
          var actual = DocOpt(doc, @"-pHOME");
          var expected = @"{""-p"": ""HOME""}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_166()
      {
          var doc = @"Usage: foo (--xx=x|--yy=y)...";
          var actual = DocOpt(doc, @"--xx=1 --yy=2");
          var expected = @"{""--xx"": [""1""], ""--yy"": [""2""]}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_167()
      {
          var doc = @"usage: prog [<input file>]";
          var actual = DocOpt(doc, @"f.txt");
          var expected = @"{""<input file>"": ""f.txt""}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_168()
      {
          var doc = @"usage: prog [--input=<file name>]...";
          var actual = DocOpt(doc, @"--input a.txt --input=b.txt");
          var expected = @"{""--input"": [""a.txt"", ""b.txt""]}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_169()
      {
          var doc = @"usage: prog good [options]
           prog fail [options]

options: --loglevel=N

";
          var actual = DocOpt(doc, @"fail --loglevel 5");
          var expected = @"{""--loglevel"": ""5"", ""fail"": true, ""good"": false}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_170()
      {
          var doc = @"usage:prog --foo";
          var actual = DocOpt(doc, @"--foo");
          var expected = @"{""--foo"": true}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_171()
      {
          var doc = @"PROGRAM USAGE: prog --foo";
          var actual = DocOpt(doc, @"--foo");
          var expected = @"{""--foo"": true}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_172()
      {
          var doc = @"Usage: prog --foo
           prog --bar
NOT PART OF SECTION";
          var actual = DocOpt(doc, @"--foo");
          var expected = @"{""--foo"": true, ""--bar"": false}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_173()
      {
          var doc = @"Usage:
 prog --foo
 prog --bar

NOT PART OF SECTION";
          var actual = DocOpt(doc, @"--foo");
          var expected = @"{""--foo"": true, ""--bar"": false}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_174()
      {
          var doc = @"Usage:
 prog --foo
 prog --bar
NOT PART OF SECTION";
          var actual = DocOpt(doc, @"--foo");
          var expected = @"{""--foo"": true, ""--bar"": false}";
          CheckResult(expected, actual);
      }


      [Test]
      public void Test_175()
      {
          var doc = @"Usage: prog [options]

global options: --foo
local options: --baz
               --bar
other options:
 --egg
 --spam
-not-an-option-

";
          var actual = DocOpt(doc, @"--baz --egg");
          var expected = @"{""--foo"": false, ""--baz"": true, ""--bar"": false, ""--egg"": true, ""--spam"": false}";
          CheckResult(expected, actual);
      }


      #endregion    
    }
}
