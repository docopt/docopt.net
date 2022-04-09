namespace DocoptNet.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;

    [TestFixture]
    public class DocoptTests
    {
        [Test]
        public void Test_tokens_from_pattern()
        {
            var tokens = Tokens.FromPattern("[ -h ]");
            Assert.AreEqual(3, tokens.Count());
            Assert.AreEqual("[", tokens.Current());
            Assert.AreEqual("[", tokens.Move());
            Assert.AreEqual("-h", tokens.Move());
            Assert.AreEqual("]", tokens.Move());
        }

        [Test]
        public void Test_set()
        {
            Assert.AreEqual(new Argument("N"), new Argument("N"));
            var l = new List<Pattern> {new Argument("N"), new Argument("N")};
            var s = new HashSet<Pattern>(l);
            Assert.AreEqual(new Pattern[] {new Argument("N"),}, s.ToList());
        }

        [Test]
        public void Test_issue_40_help()
        {
            var message = "";
            var d = new Docopt();
            d.PrintExit += (s, e) => message = e.Message;
            d.Apply("usage: prog --help-commands | --help", "--help");
            StringAssert.StartsWith("usage", message);
        }

        [Test]
        public void Test_issue_106_exit()
        {
            Assert.Throws<DocoptExitException>(
                () => new Docopt().Apply("usage: prog --help-commands | --help", "--help", exit: false));
        }

        [Test]
        public void Test_issue_40_same_prefix()
        {
            var expected = new Dictionary<string, ArgValue>
                {
                    {"--aabb", false},
                    {"--aa", true}
                };
            var actual = new Docopt().Apply("usage: prog --aabb | --aa", "--aa");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Match_arg_only()
        {
            var expected = new Dictionary<string, ArgValue>
                {
                    {"-v", false},
                    {"A", "arg"}
                };
            var actual = new Docopt().Apply(@"Usage: prog [-v] A

             Options: -v  Be verbose.", "arg");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Match_opt_and_arg()
        {
            var expected = new Dictionary<string, ArgValue>
                {
                    {"-v", true},
                    {"A", "arg"}
                };
            var actual = new Docopt().Apply(@"Usage: prog [-v] A

             Options: -v  Be verbose.", "-v arg");
            Assert.AreEqual(expected, actual);
        }

        const string Doc = @"Usage: prog [-vqr] [FILE]
              prog INPUT OUTPUT
              prog --help

    Options:
      -v  print status messages
      -q  report only file names
      -r  show all occurrences of the same error
      --help
    ";

        [Test]
        public void Match_one_opt_with_arg()
        {
            var expected = new Dictionary<string, ArgValue>
                {
                    {"-v", true},
                    {"-q", false},
                    {"-r", false},
                    {"--help", false},
                    {"FILE", "file.py"},
                    {"INPUT", default},
                    {"OUTPUT", default}
                };
            var actual = new Docopt().Apply(Doc, "-v file.py");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Match_one_opt_only()
        {
            var expected = new Dictionary<string, ArgValue>
                {
                    {"-v", true},
                    {"-q", false},
                    {"-r", false},
                    {"--help", false},
                    {"FILE", default},
                    {"INPUT", default},
                    {"OUTPUT", default}
                };
            var actual = new Docopt().Apply(Doc, "-v");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void No_match()
        {
            Assert.Throws<DocoptInputErrorException>(() => new Docopt().Apply(Doc, "-v input.py output.py"));
        }

        [Test]
        public void Non_existent_long()
        {
            Assert.Throws<DocoptInputErrorException>(() => new Docopt().Apply(Doc, "--fake"));
        }

        [Test]
        public void Should_exit_error_code_1()
        {
            var message = "";
            var errorCode = 0;
            var d = new Docopt();
            d.PrintExit += (s, e) =>
                {
                    message = e.Message;
                    errorCode = e.ErrorCode;
                };
            d.Apply(Doc, "--fake", exit:true);
            StringAssert.StartsWith("Usage", message);
            Assert.AreEqual(1, errorCode, "Should exit with error code 1 when exit=true and invalid args provided");
        }

        [Test]
        public void Display_help()
        {
            var message = "";
            var errorCode = 0;
            var d = new Docopt();
            d.PrintExit += (s, e) =>
            {
                message = e.Message;
                errorCode = e.ErrorCode;
            };
            d.Apply(Doc, "--help");
            StringAssert.StartsWith("Usage", message);
            Assert.AreEqual(0, errorCode);
        }

        [Test]
        public void Test_issue_59_assign_empty_string_to_long()
        {
            var expected = new Dictionary<string, ArgValue>
                {
                    {"--long", ""}
                };
            var actual = new Docopt().Apply("usage: prog --long=<a>", "--long=");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_issue_59_assign_empty_string_to_short()
        {
            var expected = new Dictionary<string, ArgValue>
                {
                    {"-l", ""}
                };
            var actual = new Docopt().Apply("usage: prog -l <a>\noptions: -l <a>", Args.Argv("-l", ""));
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_issue_68_options_shortcut_does_not_include_options_in_usage_pattern()
        {
            var args = new Docopt().Apply("usage: prog [-ab] [options]\noptions: -x\n -y", "-ax");
            Assert.True(args["-a"].IsTrue);
            Assert.True(args["-b"].IsFalse);
            Assert.True(args["-x"].IsTrue);
            Assert.True(args["-y"].IsFalse);
        }

        [Test]
        public void Test_issue_32_should_parse()
        {
            const string doc = @"
Usage: Conversion (load | brokers | loadnonxl | <pn>... [--clean]) [--whatif]

-h --help    show this
--verbose    print more text";
            Assert.DoesNotThrow(() => new Docopt().Apply(doc, "dfg67 dfg4 dg2 --clean"));
        }
    }
}
