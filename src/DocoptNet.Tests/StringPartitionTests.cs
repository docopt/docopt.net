using NUnit.Framework;

namespace DocoptNet.Tests
{
    [TestFixture]
    public class StringPartitionTests
    {
        [TestCase("left=right", "left", "=", "right")]
        [TestCase("=right", "", "=", "right")]
        public void Should_partition_string(string input, string left, string separator, string right)
        {
            var (actualLeft, actualSeparator, actualRight) = input.Partition(separator);
            Assert.AreEqual(left, actualLeft);
            Assert.AreEqual(separator, actualSeparator);
            Assert.AreEqual(right, actualRight);
        }

        [Test]
        public void Should_partition_string_no_sep_match()
        {
            var (actualLeft, actualSeparator, actualRight) = "left".Partition("=");
            Assert.AreEqual("left", actualLeft);
            Assert.AreEqual("", actualSeparator);
            Assert.AreEqual("", actualRight);
        }
    }
}
