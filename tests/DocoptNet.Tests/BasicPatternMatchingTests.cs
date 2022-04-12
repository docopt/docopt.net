namespace DocoptNet.Tests
{
    using NUnit.Framework;
    using static DocoptNet.Tests.ArgumentFactory;
    using static DocoptNet.Tests.PatternFactory;

    [TestFixture]
    public class BasicPatternMatchingTests
    {
        // ( -a N [ -x Z ] )
        readonly Pattern _pattern = new Required(new Option("-a"), new Argument("N"),
                                                 new Optional(new Option("-x"), new Argument("Z")));

        [Test]
        public void Should_match_required()
        {
            // -a N
            Assert.AreEqual(
                new MatchResult(true,
                                Leaves(),
                                Leaves(new Option("-a"), Argument("N", "9"))),
                _pattern.Match(new Option("-a"), Argument("9"))
                );
        }

        [Test]
        public void Should_match_required_and_optional()
        {
            // -a -x N Z
            Assert.AreEqual(
                new MatchResult(true,
                                Leaves(),
                                Leaves(new Option("-a"), Argument("N", "9"),
                                       new Option("-x"), Argument("Z", "5"))),
                _pattern.Match(new Option("-a"), new Option("-x"), Argument("9"), Argument("5"))
                );
        }

        [Test]
        public void Should_not_match_spurious_arg()
        {
            // -x N Z
            Assert.AreEqual(
                new MatchResult(false,
                                Leaves(new Option("-x"), Argument("9"), Argument("5")),
                                Leaves()
                    ),
                _pattern.Match(new Option("-x"), Argument("9"), Argument("5"))
                );
        }
    }
}
