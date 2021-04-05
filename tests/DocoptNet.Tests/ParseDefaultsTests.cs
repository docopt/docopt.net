namespace DocoptNet.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class ParseDefaultsTests
    {
        [Test]
        public void Test_parse_defaults()
        {
            const string DOC = @"usage: prog
                Options:
                -h, --help  Print help message.
                -o FILE     Output file.
                --verbose   Verbose mode.
            ";

            var expected = new Option[]
                {new Option("-h", "--help"), new Option("-o", null, 1), new Option(null, "--verbose")};
            Assert.AreEqual(expected, Docopt.ParseDefaults(DOC));
        }

        [Test]
        public void Test_issue_126_defaults_not_parsed_correctly_when_tabs()
        {
            const string section = "Options:\n\t--foo=<arg>  [default: bar]";
            var expected = new Option[] { new Option(null, "--foo", 1, "bar")};
            Assert.AreEqual(expected, Docopt.ParseDefaults(section));
        }
    }
}
