using NUnit.Framework;

namespace DocoptNet.Tests
{
    [TestFixture]
    public class OptionNameTests
    {
        [Test]
        public void Short_only()
        {
            Assert.AreEqual("-h", new Option("-h", null).Name);
        }

        [Test]
        public void Short_and_long()
        {
            Assert.AreEqual("--help", new Option("-h", "--help").Name);
        }

        [Test]
        public void Long_only()
        {
            Assert.AreEqual("--help", new Option(null, "--help").Name);
        }
    }
}