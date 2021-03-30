using NUnit.Framework;

namespace DocoptNet.Tests
{
    [TestFixture]
    public class OptionParseTests
    {
        [Test]
        public void Short_and_long()
        {
            Assert.AreEqual(new Option("-h", null), Option.Parse("-h"));
            Assert.AreEqual(new Option(null, "--help"), Option.Parse("--help"));
            Assert.AreEqual(new Option("-h", "--help"), Option.Parse("-h --help"));
            Assert.AreEqual(new Option("-h", "--help"), Option.Parse("-h, --help"));
        }

        [Test]
        public void With_args()
        {
            Assert.AreEqual(new Option("-h", null, 1), Option.Parse("-h TOPIC"));
            Assert.AreEqual(new Option(null, "--help", 1), Option.Parse("--help TOPIC"));
            Assert.AreEqual(new Option("-h", "--help", 1), Option.Parse("-h TOPIC --help TOPIC"));
            Assert.AreEqual(new Option("-h", "--help", 1), Option.Parse("-h TOPIC, --help TOPIC"));
            Assert.AreEqual(new Option("-h", "--help", 1), Option.Parse("-h TOPIC, --help=TOPIC"));

        }

        [Test]
        public void With_description()
        {
            Assert.AreEqual(new Option("-h", null), Option.Parse("-h  Description..."));
            Assert.AreEqual(new Option("-h", "--help"), Option.Parse("-h --help  Description..."));
            Assert.AreEqual(new Option("-h", null, 1), Option.Parse("-h TOPIC  Description..."));
        }

        [Test]
        public void Leading_spaces()
        {

            Assert.AreEqual(new Option("-h", null), Option.Parse("    -h"));
        }

        [Test]
        public void With_defaults()
        {

            Assert.AreEqual(new Option("-h", null, 1, "2"), Option.Parse("-h TOPIC  Description... [default: 2]"));
            Assert.AreEqual(new Option("-h", null, 1, "2"), Option.Parse("-h TOPIC  Description... [dEfAuLt: 2]"));
        }

        [Test]
        public void With_args_and_defaults()
        {

            Assert.AreEqual(new Option("-h", null, 1, "topic-1"),
                            Option.Parse("-h TOPIC  Description... [default: topic-1]"));
            Assert.AreEqual(new Option(null, "--help", 1, "3.14"), Option.Parse("--help=TOPIC  ... [default: 3.14]"));
            Assert.AreEqual(new Option("-h", "--help", 1, "./"), Option.Parse("-h, --help=DIR  ... [default: ./]"));
        }
    }
}
