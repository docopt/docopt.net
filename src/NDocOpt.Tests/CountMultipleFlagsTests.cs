using System.Collections.Generic;
using NUnit.Framework;

namespace NDocOpt.Tests
{
    [TestFixture]
    public class CountMultipleFlagsTests
    {
        [Test]
        public void Simple_flag()
        {
            var expected = new Dictionary<string, ValueObject>
                {
                    {"-v", new ValueObject(true)}
                };
            var actual = new DocOpt().Apply("Usage: prog [-v]", "-v");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Flag_0()
        {
            var expected = new Dictionary<string, ValueObject>
                {
                    {"-v", new ValueObject(0)}
                };
            var actual = new DocOpt().Apply("Usage: prog [-vv]", "");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Flag_1()
        {
            var expected = new Dictionary<string, ValueObject>
                {
                    {"-v", new ValueObject(1)}
                };
            var actual = new DocOpt().Apply("Usage: prog [-vv]", "-v");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Flag_2()
        {
            var expected = new Dictionary<string, ValueObject>
                {
                    {"-v", new ValueObject(2)}
                };
            var actual = new DocOpt().Apply("Usage: prog [-vv]", "-vv");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Flag_too_many()
        {
            Assert.Throws<DocOptExitException>(() => new DocOpt().Apply("Usage: prog [-vv]", "-vvv"));
        }

        [Test]
        public void Flag_3()
        {
            var expected = new Dictionary<string, ValueObject>
                {
                    {"-v", new ValueObject(3)}
                };
            var actual = new DocOpt().Apply("Usage: prog [-v | -vv | -vvv]", "-vvv");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Flag_one_or_more()
        {
            var expected = new Dictionary<string, ValueObject>
                {
                    {"-v", new ValueObject(6)}
                };
            var actual = new DocOpt().Apply("Usage: prog -v...", "-vvvvvv");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Flag_long_2()
        {
            var expected = new Dictionary<string, ValueObject>
                {
                    {"--ver", new ValueObject(2)}
                };
            var actual = new DocOpt().Apply("Usage: prog [--ver --ver]", "--ver --ver");
            Assert.AreEqual(expected, actual);
        }

    }
}