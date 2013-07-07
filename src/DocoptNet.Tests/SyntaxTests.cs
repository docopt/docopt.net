using System.Collections.Generic;
using NUnit.Framework;

namespace DocoptNet.Tests
{
    [TestFixture]
    public class SyntaxTests
    {
        [Test]
        public void Missing_closing_square_bracket()
        {
            Assert.Throws<DocoptLanguageErrorException>(
                () => new Docopt().Apply("Usage: prog [a [b]"));
            
        }

        [Test]
        public void Missing_opening_paren()
        {
            Assert.Throws<DocoptLanguageErrorException>(
                () => new Docopt().Apply("Usage: prog [a [b] ] c )"));

        }

        [Test]
        public void Detect_double_dash()
        {
            var expected = new Dictionary<string, ValueObject>
                {
                    {"-o", new ValueObject(false)},
                    {"<arg>", new ValueObject("-o")},
                    {"--", new ValueObject(true)}
                };
            var actual = new Docopt().Apply("usage: prog [-o] [--] <arg>\nOptions: -o", "-- -o");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void No_double_dash()
        {
            var expected = new Dictionary<string, ValueObject>
                {
                    {"-o", new ValueObject(true)},
                    {"<arg>", new ValueObject("1")},
                    {"--", new ValueObject(false)}
                };
            var actual = new Docopt().Apply("usage: prog [-o] [--] <arg>\nOptions: -o", "-o 1");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Double_dash_not_allowed() // FIXME?
        {
            Assert.Throws<DocoptExitException>(
                () => new Docopt().Apply("usage: prog [-o] <arg>\noptions:-o", "-- -o"));

        }

        [Test]
        public void No_usage()
        {
            Assert.Throws<DocoptLanguageErrorException>(
                () => new Docopt().Apply("no usage with colon here"));

        }

        [Test]
        public void Duplicate_usage()
        {
            Assert.Throws<DocoptLanguageErrorException>(
                () => new Docopt().Apply("usage: here \n\n and again usage: here"));

        }

        [Test]
        public void Test_issue_71_double_dash_is_not_a_valid_option_argument()
        {
            Assert.Throws<DocoptExitException>(
                () => new Docopt().Apply("usage: prog [--log=LEVEL] [--] <args>...", "--log -- 1 2"));
        }

    }
}