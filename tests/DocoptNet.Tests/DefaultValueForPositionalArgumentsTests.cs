namespace DocoptNet.Tests
{
    using System.Collections.Generic;
    using NUnit.Framework;

    [TestFixture]
    public class DefaultValueForPositionalArgumentsTests
    {
        [Test]
        public void Expect_default_value()
        {
            var expected = new Dictionary<string, ArgValue>
                {
                    {"--data", StringList.TopBottom("x")}
                };
            var actual = new Docopt().Apply("Usage: prog [--data=<data>...]\n\nOptions:\n\t-d --data=<arg>    Input data [default: x]", "");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Expect_default_collection()
        {
            var expected = new Dictionary<string, ArgValue>
                {
                    {"--data", StringList.TopBottom("x", "y")}
                };
            var actual = new Docopt().Apply("Usage: prog [--data=<data>...]\n\n         Options:\n\t-d --data=<arg>    Input data [default: x y]", "");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Expect_one_arg()
        {
            var expected = new Dictionary<string, ArgValue>
                {
                    {"--data", StringList.TopBottom("this")}
                };
            var actual = new Docopt().Apply("Usage: prog [--data=<data>...]\n\n         Options:\n\t-d --data=<arg>    Input data [default: x y]", "--data=this");
            Assert.AreEqual(expected, actual);
        }
    }
}
