using NUnit.Framework;

namespace DocoptNet.Tests
{
    [TestFixture]
    public class RequiredMatchTests
    {
        [Test]
        public void Should_match_option()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                new LeafPattern[0],
                                new LeafPattern[] {new Option("-a")}),
                new Required(new Option("-a")).Match(new LeafPattern[] {new Option("-a")})
                );
        }

        [Test]
        public void Should_not_match_empty()
        {
            Assert.AreEqual(
                new MatchResult(false,
                                new LeafPattern[0],
                                new LeafPattern[0]),
                new Required(new Option("-a")).Match(new LeafPattern[0])
                );
        }

        [Test]
        public void Should_not_match_other_option()
        {
            Assert.AreEqual(
                new MatchResult(false,
                                new LeafPattern[] {new Option("-x")},
                                new LeafPattern[0]
                    ),
                new Required(new Option("-a")).Match(new LeafPattern[] {new Option("-x")})
                );
        }

        [Test]
        public void Should_not_match_2_required()
        {
            Assert.AreEqual(
                new MatchResult(false,
                                new LeafPattern[] { new Option("-a") },
                                new LeafPattern[0]
                    ),
                new Required(new Option("-a"), new Option("-b")).Match(new LeafPattern[] { new Option("-a") })
                );
        }
    }
}
