using NUnit.Framework;

namespace NDocOpt.Tests
{
    [TestFixture]
    public class RequiredMatchTests
    {
        [Test]
        public void Should_match_option()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                new Pattern[0],
                                new Pattern[] {new Option("-a")}),
                new Required(new Option("-a")).Match(new Pattern[] {new Option("-a")})
                );
        }

        [Test]
        public void Should_not_match_empty()
        {
            Assert.AreEqual(
                new MatchResult(false,
                                new Pattern[0],
                                new Pattern[0]),
                new Required(new Option("-a")).Match(new Pattern[0])
                );
        }

        [Test]
        public void Should_not_match_other_option()
        {
            Assert.AreEqual(
                new MatchResult(false,
                                new Pattern[] {new Option("-x")},
                                new Pattern[0]
                    ),
                new Required(new Option("-a")).Match(new Pattern[] {new Option("-x")})
                );
        }

        [Test]
        public void Should_not_match_2_required()
        {
            Assert.AreEqual(
                new MatchResult(false,
                                new Pattern[] { new Option("-a") },
                                new Pattern[0]
                    ),
                new Required(new Option("-a"), new Option("-b")).Match(new Pattern[] { new Option("-a") })
                );
        }
    }
}