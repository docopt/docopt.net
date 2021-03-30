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
            var p = new StringPartition(input, separator);
            Assert.AreEqual(left, p.LeftString);
            Assert.AreEqual(separator, p.Separator);
            Assert.AreEqual(right, p.RightString);
        }

        [Test]
        public void Should_partition_string_no_sep_match()
        {
            var p = new StringPartition("left", "=");
            Assert.AreEqual("left", p.LeftString);
            Assert.AreEqual("", p.Separator);
            Assert.AreEqual("", p.RightString);
        }
    }
}
