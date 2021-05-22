namespace DocoptNet.Tests
{
    using System.Collections.Generic;
    using NUnit.Framework;

    [TestFixture]
    public class CountMultipleFlagsTests
    {
        [Test]
        public void Simple_flag()
        {
            var expected = new Dictionary<string, object>
                {
                    {"-v", true}
                };
            var actual = new Docopt().Apply("Usage: prog [-v]", "-v");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Flag_0()
        {
            var expected = new Dictionary<string, object>
                {
                    {"-v", 0}
                };
            var actual = new Docopt().Apply("Usage: prog [-vv]", "");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Flag_1()
        {
            var expected = new Dictionary<string, object>
                {
                    {"-v", 1}
                };
            var actual = new Docopt().Apply("Usage: prog [-vv]", "-v");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Flag_2()
        {
            var expected = new Dictionary<string, object>
                {
                    {"-v", 2}
                };
            var actual = new Docopt().Apply("Usage: prog [-vv]", "-vv");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Flag_too_many()
        {
            Assert.Throws<DocoptInputErrorException>(() => new Docopt().Apply("Usage: prog [-vv]", "-vvv"));
        }

        [Test]
        public void Flag_3()
        {
            var expected = new Dictionary<string, object>
                {
                    {"-v", 3}
                };
            var actual = new Docopt().Apply("Usage: prog [-v | -vv | -vvv]", "-vvv");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Flag_one_or_more()
        {
            var expected = new Dictionary<string, object>
                {
                    {"-v", 6}
                };
            var actual = new Docopt().Apply("Usage: prog -v...", "-vvvvvv");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Flag_long_2()
        {
            var expected = new Dictionary<string, object>
                {
                    {"--ver", 2}
                };
            var actual = new Docopt().Apply("Usage: prog [--ver --ver]", "--ver --ver");
            Assert.AreEqual(expected, actual);
        }

    }
}
