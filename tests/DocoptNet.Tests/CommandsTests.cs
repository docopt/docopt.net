namespace DocoptNet.Tests
{
    using System.Collections.Generic;
    using NUnit.Framework;

    [TestFixture]
    public class CommandsTests
    {
        [Test]
        public void Required()
        {
            var expected = new Dictionary<string, ArgValue>
                {
                    {"add", true}
                };
            var actual = new Docopt().Apply("Usage: prog add", "add");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Optional_no_args()
        {
            var expected = new Dictionary<string, ArgValue>
                {
                    {"add", false}
                };
            var actual = new Docopt().Apply("Usage: prog [add]", "");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Optional_one_arg()
        {
            var expected = new Dictionary<string, ArgValue>
                {
                    {"add", true}
                };
            var actual = new Docopt().Apply("Usage: prog [add]", "add");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Optional_either_first_specified()
        {
            var expected = new Dictionary<string, ArgValue>
                {
                    {"add", true},
                    {"rm", false}
                };
            var actual = new Docopt().Apply("Usage: prog (add|rm)", "add");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Optional_either_second_specified()
        {
            var expected = new Dictionary<string, ArgValue>
                {
                    {"add", false},
                    {"rm", true}
                };
            var actual = new Docopt().Apply("Usage: prog (add|rm)", "rm");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Required_both_specified()
        {
            var expected = new Dictionary<string, ArgValue>
                {
                    {"a", true},
                    {"b", true}
                };
            var actual = new Docopt().Apply("Usage: prog a b", "a b");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Required_wrong_order()
        {
            Assert.Throws<DocoptInputErrorException>(() => new Docopt().Apply("Usage: prog a b", "b a"));
        }
    }
}
