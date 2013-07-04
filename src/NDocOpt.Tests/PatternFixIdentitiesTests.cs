using NUnit.Framework;

namespace NDocOpt.Tests
{
    [TestFixture]
    public class PatternFixIdentitiesTests
    {
        [Test]
        public void Should_fix_all_args()
        {
            var pattern = new Required(new Argument("N"), new Argument("N"));
            Assert.AreEqual(pattern.Children[0], pattern.Children[1]);
            Assert.AreNotSame(pattern.Children[0], pattern.Children[1]);
            pattern.FixIdentities();
            Assert.AreSame(pattern.Children[0], pattern.Children[1]);
        }

        [Test]
        public void Should_fix_some_args()
        {
            var pattern = new Required(new Optional(new Argument("X"), new Argument("N")), new Argument("N"));
            Assert.AreEqual(pattern.Children[0].Children[1], pattern.Children[1]);
            Assert.AreNotSame(pattern.Children[0].Children[1], pattern.Children[1]);
            pattern.FixIdentities();
            Assert.AreSame(pattern.Children[0].Children[1], pattern.Children[1]);
        }
    }
}