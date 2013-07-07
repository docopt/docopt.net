using NUnit.Framework;

namespace DocoptNet.Tests
{
    [TestFixture]
    public class BasicPatternMatchingTests
    {
        // ( -a N [ -x Z ] )
        private readonly Pattern _pattern = new Required(new Option("-a"), new Argument("N"),
                                                         new Optional(new Option("-x"), new Argument("Z")));

        [Test]
        public void Should_match_required()
        {
            // -a N
            Assert.AreEqual(
                new MatchResult(true,
                                new Pattern[0],
                                new Pattern[] {new Option("-a"), new Argument("N", "9")}),
                _pattern.Match(new Pattern[] {new Option("-a"), new Argument(null, "9")})
                );
        }

        [Test]
        public void Should_match_required_and_optional()
        {
            // -a -x N Z
            Assert.AreEqual(
                new MatchResult(true,
                                new Pattern[0],
                                new Pattern[]
                                    {
                                        new Option("-a"), new Argument("N", "9"),
                                        new Option("-x"), new Argument("Z", "5")
                                    }),
                _pattern.Match(new Pattern[]
                    {new Option("-a"), new Option("-x"), new Argument(null, "9"), new Argument(null, "5")})
                );
        }

        [Test]
        public void Should_not_match_spurious_arg()
        {
            // -x N Z
            Assert.AreEqual(
                new MatchResult(false,
                                new Pattern[]
                                    {
                                        new Option("-x"), new Argument(null, "9"), new Argument(null, "5")
                                    },
                                new Pattern[0]
                    ),
                _pattern.Match(new Pattern[] {new Option("-x"), new Argument(null, "9"), new Argument(null, "5")})
                );
        }
    }
}