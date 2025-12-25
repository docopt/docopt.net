namespace DocoptNet.Tests
{
    using NUnit.Framework;
    using Assert = NUnit.Framework.Legacy.ClassicAssert;

    [TestFixture]
    public class ParseDefaultsTests
    {
        [Test]
        public void Test_parse_defaults()
        {
            const string doc = """
                usage: prog
                Options:
                  -h, --help  Print help message.
                  -o FILE     Output file.
                  --verbose   Verbose mode.

                """;

            Option[] expected = [new("-h", "--help"), new("-o", 1), new("--verbose")];
            Assert.AreEqual(expected, Docopt.ParseDefaults(doc));
        }

        [Test]
        public void Test_issue_126_defaults_not_parsed_correctly_when_tabs()
        {
            const string section = $"""
                Options:
                {"\t"}--foo=<arg>  [default: bar]

                """;
            Option[] expected = [new("--foo", 1, "bar")];
            Assert.AreEqual(expected, Docopt.ParseDefaults(section));
        }
    }
}
