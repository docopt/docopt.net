namespace DocoptNet.Tests
{
    using NUnit.Framework;
    using Assert = NUnit.Framework.Legacy.ClassicAssert;

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
            var branch = (BranchPattern)pattern.Children[0];
            Assert.AreEqual(branch.Children[1], pattern.Children[1]);
            Assert.AreNotSame(branch.Children[1], pattern.Children[1]);
            pattern.FixIdentities();
            Assert.AreSame(branch.Children[1], pattern.Children[1]);
        }
    }
}
