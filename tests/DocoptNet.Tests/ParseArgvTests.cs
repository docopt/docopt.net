namespace DocoptNet.Tests
{
    using System;
    using static DocoptNet.Tests.ArgumentFactory;
    using NUnit.Framework;
    using Assert = NUnit.Framework.Legacy.ClassicAssert;

    [TestFixture]
    public class ParseArgvTests
    {
        readonly Option[] _options = { new("-h"), new("-v", "--verbose"), new("-f", "--file", 1) };

        static Tokens Tokens(string s) =>
            DocoptNet.Tokens.From(s.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));

        [Test]
        public void Test_parse_argv_empty()
        {
            Assert.IsEmpty(Docopt.ParseArgv(Tokens(""), _options));
        }

        [Test]
        public void Test_parse_argv_one_opt()
        {
            Assert.AreEqual(new[] {new Option("-h", 0, ArgValue.True)},
                            Docopt.ParseArgv(Tokens("-h"), _options));
        }

        [Test]
        public void Test_parse_argv_short_and_long()
        {
            Assert.AreEqual(new[]
                            {
                                new Option("-h", 0, ArgValue.True),
                                new Option("-v", "--verbose", 0, ArgValue.True)
                            },
                            Docopt.ParseArgv(Tokens("-h --verbose"), _options));
        }

        [Test]
        public void Test_parse_argv_opt_with_arg()
        {
            Assert.AreEqual(new[]
                            {
                                new Option("-h", 0, ArgValue.True),
                                new Option("-f", "--file", 1, "f.txt")
                            },
                            Docopt.ParseArgv(Tokens("-h --file f.txt"), _options));
        }

        [Test]
        public void Test_parse_argv_with_arg()
        {
            Assert.AreEqual(new Pattern[]
                            {
                                new Option("-h", 0, ArgValue.True),
                                new Option("-f", "--file", 1, "f.txt"),
                                Argument("arg")
                            },
                            Docopt.ParseArgv(Tokens("-h --file f.txt arg"), _options));
        }

        [Test]
        public void Test_parse_argv_two_args()
        {
            Assert.AreEqual(new Pattern[]
                            {
                                new Option("-h", 0, ArgValue.True),
                                new Option("-f", "--file", 1, "f.txt"),
                                Argument("arg"),
                                Argument("arg2")
                            },
                            Docopt.ParseArgv(Tokens("-h --file f.txt arg arg2"), _options));
        }

        [Test]
        public void Test_parse_argv_with_double_dash()
        {
            Assert.AreEqual(new Pattern[]
                            {
                                new Option("-h", 0, ArgValue.True),
                                Argument("arg"),
                                Argument("--"),
                                Argument("-v")
                            },
                            Docopt.ParseArgv(Tokens("-h arg -- -v"), _options));
        }
    }
}
