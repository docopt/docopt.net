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
                                new Pattern[0],
                                new Pattern[] {new Option("-a")}),
                new Optional(new Option("-a")).Match(new Pattern[] {new Option("-a")})
                );
        }

        [Test]
        public void Should_match_empty()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                new Pattern[0],
                                new Pattern[0]),
                new Optional(new Option("-a")).Match(new Pattern[0])
                );
        }

        [Test]
        public void Should_not_collect_other_opt()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                new Pattern[] {new Option("-x")},
                                new Pattern[0]
                    ),
                new Optional(new Option("-a")).Match(new Pattern[] {new Option("-x")})
                );
        }

        [Test]
        public void Should_match_first_option()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                new Pattern[0],
                                new Pattern[] {new Option("-a")}),
                new Optional(new Option("-a"), new Option("-b")).Match(new Pattern[] {new Option("-a")})
                );
        }

        [Test]
        public void Should_not_collect_other_option_2()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                new Pattern[] {new Option("-x")},
                                new Pattern[0]),
                new Optional(new Option("-a"), new Option("-b")).Match(new Pattern[] {new Option("-x")})
                );
        }

        [Test]
        public void Should_match_optional_arg()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                new Pattern[0],
                                new Pattern[] { new Argument("N", new ValueObject(9)) }),
                new Optional(new Argument("N")).Match(new Pattern[] { new Argument(null, new ValueObject(9))  })
                );
        }

        [Test]
        public void Should_collect_all_except_other()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                new Pattern[] { new Option("-x") },
                                new Pattern[] { new Option("-a"), new Option("-b") }),
                new Optional(new Option("-a"), new Option("-b")).Match(
                    new Pattern[] { new Option("-b"), new Option("-x"), new Option("-a") })
                );
        }
    }
}