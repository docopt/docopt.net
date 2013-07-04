using NUnit.Framework;

namespace NDocOpt.Tests
{
    [TestFixture]
    public class StringPartitionTests
    {
        [Test]
        public void Should_partition_string()
        {
            var p = new StringPartition("left=right", "=");
            Assert.AreEqual("left", p.LeftString);
            Assert.AreEqual("=", p.Separator);
            Assert.AreEqual("right", p.RightString);
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