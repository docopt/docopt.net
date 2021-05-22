namespace DocoptNet.Tests
{
    using NUnit.Framework;
    using static DocoptNet.Tests.PatternFactory;

    [TestFixture]
    public class OptionalMatchTests
    {
        [Test]
        public void Should_match_option()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                Leaves(),
                                Leaves(new Option("-a"))),
                new Optional(new Option("-a")).Match(new Option("-a"))
                );
        }

        [Test]
        public void Should_match_empty()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                Leaves(),
                                Leaves()),
                new Optional(new Option("-a")).Match()
                );
        }

        [Test]
        public void Should_not_collect_other_opt()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                Leaves(new Option("-x")),
                                Leaves()
                    ),
                new Optional(new Option("-a")).Match(new Option("-x"))
                );
        }

        [Test]
        public void Should_match_first_option()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                Leaves(),
                                Leaves(new Option("-a"))),
                new Optional(new Option("-a"), new Option("-b")).Match(new Option("-a"))
                );
        }

        [Test]
        public void Should_not_collect_other_option_2()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                Leaves(new Option("-x")),
                                Leaves()),
                new Optional(new Option("-a"), new Option("-b")).Match(new Option("-x"))
                );
        }

        [Test]
        public void Should_match_optional_arg()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                Leaves(),
                                Leaves(new Argument("N", 9) )),
                new Optional(new Argument("N")).Match(new Argument(null, 9))
                );
        }

        [Test]
        public void Should_collect_all_except_other()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                Leaves(new Option("-x") ),
                                Leaves(new Option("-a"), new Option("-b") )),
                new Optional(new Option("-a"), new Option("-b")).Match(new Option("-b"), new Option("-x"), new Option("-a"))
                );
        }
    }
}
