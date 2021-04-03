using NUnit.Framework;

namespace DocoptNet.Tests
{
    [TestFixture]
    public class OptionalMatchTests
    {
        [Test]
        public void Should_match_option()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                new LeafPattern[0],
                                new LeafPattern[] {new Option("-a")}),
                new Optional(new Option("-a")).Match(new LeafPattern[] {new Option("-a")})
                );
        }

        [Test]
        public void Should_match_empty()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                new LeafPattern[0],
                                new LeafPattern[0]),
                new Optional(new Option("-a")).Match(new LeafPattern[0])
                );
        }

        [Test]
        public void Should_not_collect_other_opt()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                new LeafPattern[] {new Option("-x")},
                                new LeafPattern[0]
                    ),
                new Optional(new Option("-a")).Match(new LeafPattern[] {new Option("-x")})
                );
        }

        [Test]
        public void Should_match_first_option()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                new LeafPattern[0],
                                new LeafPattern[] {new Option("-a")}),
                new Optional(new Option("-a"), new Option("-b")).Match(new LeafPattern[] {new Option("-a")})
                );
        }

        [Test]
        public void Should_not_collect_other_option_2()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                new LeafPattern[] {new Option("-x")},
                                new LeafPattern[0]),
                new Optional(new Option("-a"), new Option("-b")).Match(new LeafPattern[] {new Option("-x")})
                );
        }

        [Test]
        public void Should_match_optional_arg()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                new LeafPattern[0],
                                new LeafPattern[] { new Argument("N", new ValueObject(9)) }),
                new Optional(new Argument("N")).Match(new LeafPattern[] { new Argument(null, new ValueObject(9))  })
                );
        }

        [Test]
        public void Should_collect_all_except_other()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                new LeafPattern[] { new Option("-x") },
                                new LeafPattern[] { new Option("-a"), new Option("-b") }),
                new Optional(new Option("-a"), new Option("-b")).Match(
                    new LeafPattern[] { new Option("-b"), new Option("-x"), new Option("-a") })
                );
        }
    }
}
