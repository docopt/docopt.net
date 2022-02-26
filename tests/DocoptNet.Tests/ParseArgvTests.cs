namespace DocoptNet.Tests
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class ParseArgvTests
    {
        private readonly Option[] _options = new[]
            {new Option("-h"), new Option("-v", "--verbose"), new Option("-f", "--file", 1)};

        private Tokens TS(string s)
        {
            return Tokens.From(s.Split((char[])null, StringSplitOptions.RemoveEmptyEntries));
        }

        [Test]
        public void Test_parse_argv_empty()
        {
            Assert.IsEmpty(Docopt.ParseArgv(TS(""), _options));
        }

        [Test]
        public void Test_parse_argv_one_opt()
        {
            Assert.AreEqual(new[] {new Option("-h", null, 0, ArgValue.True)},
                            Docopt.ParseArgv(TS("-h"), _options));
        }

        [Test]
        public void Test_parse_argv_short_and_long()
        {
            Assert.AreEqual(new[]
                {
                    new Option("-h", null, 0, ArgValue.True),
                    new Option("-v", "--verbose", 0, ArgValue.True)
                },
                            Docopt.ParseArgv(TS("-h --verbose"), _options));
        }

        [Test]
        public void Test_parse_argv_opt_with_arg()
        {
            Assert.AreEqual(new[]
                {
                    new Option("-h", null, 0, ArgValue.True),
                    new Option("-f", "--file", 1, "f.txt")
                },
                            Docopt.ParseArgv(TS("-h --file f.txt"), _options));
        }

        [Test]
        public void Test_parse_argv_with_arg()
        {
            Assert.AreEqual(
                new Pattern[]
                    {
                        new Option("-h", null, 0, ArgValue.True),
                        new Option("-f", "--file", 1, "f.txt"),
                        new Argument(null, "arg")
                    },
                Docopt.ParseArgv(TS("-h --file f.txt arg"), _options));
        }

        [Test]
        public void Test_parse_argv_two_args()
        {
            Assert.AreEqual(
                new Pattern[]
                    {
                        new Option("-h", null, 0, ArgValue.True),
                        new Option("-f", "--file", 1, "f.txt"),
                        new Argument(null, "arg"),
                        new Argument(null, "arg2")
                    },
                Docopt.ParseArgv(TS("-h --file f.txt arg arg2"), _options));
        }

        [Test]
        public void Test_parse_argv_with_double_dash()
        {
            Assert.AreEqual(
                new Pattern[]
                    {
                        new Option("-h", null, 0, ArgValue.True),
                        new Argument(null, "arg"),
                        new Argument(null, "--"),
                        new Argument(null, "-v")
                    },
                Docopt.ParseArgv(TS("-h arg -- -v"), _options));
        }
    }
}
