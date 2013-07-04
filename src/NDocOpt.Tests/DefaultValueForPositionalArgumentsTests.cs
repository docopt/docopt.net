using System.Collections.Generic;
using NUnit.Framework;

namespace NDocOpt.Tests
{
    [TestFixture]
    public class DefaultValueForPositionalArgumentsTests
    {
        // disabled right now
        [Test]
        public void Expect_none()
        {
            var expected = new Dictionary<string, ValueObject>
                {
                    {"<p>", null}
                };
            var actual = new DocOpt().Apply("usage: prog [<p>]\n\n<p>  [default: x]", "");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Expect_empty_collection()
        {
            var expected = new Dictionary<string, ValueObject>
                {
                    {"<p>", new ValueObject(new string[0])}
                };
            var actual = new DocOpt().Apply("usage: prog [<p>]...\n\n<p>  [default: x y]", "");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Expect_one_arg()
        {
            var expected = new Dictionary<string, ValueObject>
                {
                    {"<p>", new ValueObject(new[] {"this"})}
                };
            var actual = new DocOpt().Apply("usage: prog [<p>]...\n\n<p>  [default: x y]", "this");
            Assert.AreEqual(expected, actual);
        }
    }
}