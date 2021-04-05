namespace DocoptNet.Tests
{
    using NUnit.Framework;
    using static DocoptNet.Tests.PatternFactory;

    [TestFixture]
    public class RequiredMatchTests
    {
        [Test]
        public void Should_match_option()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                Leaves(),
                                Leaves(new Option("-a"))),
                new Required(new Option("-a")).Match(new Option("-a"))
                );
        }

        [Test]
        public void Should_not_match_empty()
        {
            Assert.AreEqual(
                new MatchResult(false,
                                Leaves(),
                                Leaves()),
                new Required(new Option("-a")).Match()
                );
        }

        [Test]
        public void Should_not_match_other_option()
        {
            Assert.AreEqual(
                new MatchResult(false,
                                Leaves(new Option("-x")),
                                Leaves()
                    ),
                new Required(new Option("-a")).Match(new Option("-x"))
                );
        }

        [Test]
        public void Should_not_match_2_required()
        {
            Assert.AreEqual(
                new MatchResult(false,
                                Leaves(new Option("-a")),
                                Leaves()
                    ),
                new Required(new Option("-a"), new Option("-b")).Match(new Option("-a"))
                );
        }
    }
}
