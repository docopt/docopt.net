using NUnit.Framework;

namespace NDocOpt.Tests
{
    [TestFixture]
    public class ParseArgvTests
    {
        private readonly Option[] _options = new[]
            {new Option("-h"), new Option("-v", "--verbose"), new Option("-f", "--file", 1)};

        private Tokens TS(string s)
        {
            return new Tokens(s, typeof (DocOptExitException));
        }

        [Test]
        public void Test_parse_argv_empty()
        {
            Assert.IsEmpty(DocOpt.ParseArgv(TS(""), _options));
        }

        [Test]
        public void Test_parse_argv_one_opt()
        {
            Assert.AreEqual(new[] {new Option("-h", null, 0, new ValueObject(true))},
                            DocOpt.ParseArgv(TS("-h"), _options));
        }

        [Test]
        public void Test_parse_argv_short_and_long()
        {
            Assert.AreEqual(new[]
                {
                    new Option("-h", null, 0, new ValueObject(true)),
                    new Option("-v", "--verbose", 0, new ValueObject(true))
                },
                            DocOpt.ParseArgv(TS("-h --verbose"), _options));
        }

        [Test]
        public void Test_parse_argv_opt_with_arg()
        {
            Assert.AreEqual(new[]
                {
                    new Option("-h", null, 0, new ValueObject(true)),
                    new Option("-f", "--file", 1, "f.txt")
                },
                            DocOpt.ParseArgv(TS("-h --file f.txt"), _options));
        }

        [Test]
        public void Test_parse_argv_with_arg()
        {
            Assert.AreEqual(
                new Pattern[]
                    {
                        new Option("-h", null, 0, new ValueObject(true)),
                        new Option("-f", "--file", 1, "f.txt"),
                        new Argument(null, "arg")
                    },
                DocOpt.ParseArgv(TS("-h --file f.txt arg"), _options));
        }

        [Test]
        public void Test_parse_argv_two_args()
        {
            Assert.AreEqual(
                new Pattern[]
                    {
                        new Option("-h", null, 0, new ValueObject(true)),
                        new Option("-f", "--file", 1, "f.txt"),
                        new Argument(null, "arg"),
                        new Argument(null, "arg2")
                    },
                DocOpt.ParseArgv(TS("-h --file f.txt arg arg2"), _options));
        }

        [Test]
        public void Test_parse_argv_with_double_dash()
        {
            Assert.AreEqual(
                new Pattern[]
                    {
                        new Option("-h", null, 0, new ValueObject(true)),
                        new Argument(null, "arg"),
                        new Argument(null, "--"),
                        new Argument(null, "-v")
                    },
                DocOpt.ParseArgv(TS("-h arg -- -v"), _options));
        }
    }
}