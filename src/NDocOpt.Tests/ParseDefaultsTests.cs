using NUnit.Framework;

namespace DocoptNet.Tests
{
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
    }
}