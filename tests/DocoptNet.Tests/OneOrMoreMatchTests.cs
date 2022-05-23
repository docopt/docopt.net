namespace DocoptNet.Tests
{
    using NUnit.Framework;
    using static DocoptNet.Tests.ArgumentFactory;
    using static DocoptNet.Tests.PatternFactory;

    [TestFixture]
    public class OneOrMoreMatchTests
    {
        [Test]
        public void Should_match_arg()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                Leaves(),
                                Leaves(Argument("N", 9))),
                new OneOrMore(new Argument("N")).Match(Argument(9))
                );
        }

        [Test]
        public void Should_not_match_empty()
        {
            Assert.AreEqual(
                new MatchResult(false,
                                Leaves(),
                                Leaves()),
                new OneOrMore(new Argument("N")).Match()
                );
        }

        [Test]
        public void Should_not_match_option()
        {
            Assert.AreEqual(
                new MatchResult(false,
                                Leaves(new Option("-x")),
                                Leaves()
                    ),
                new OneOrMore(new Argument("N")).Match(new Option("-x"))
                );
        }

        [Test]
        public void Should_match_all_args()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                Leaves(),
                                Leaves(Argument("N", 9), Argument("N", 8))),
                new OneOrMore(new Argument("N")).Match(Argument(9), Argument(8))
                );
        }

        [Test]
        public void Should_not_match_unknown_opt()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                Leaves(new Option("-x")),
                                Leaves(Argument("N", 9), Argument("N", 8))),
                new OneOrMore(new Argument("N")).Match(Argument(9), new Option("-x"), Argument(8))
                );
        }

        [Test]
        public void Should_not_match_unknown_arg()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                Leaves(Argument(8)),
                                Leaves(new Option("-a"), new Option("-a"))
                    ),
                new OneOrMore(new Option("-a")).Match(new Option("-a"), Argument(8), new Option("-a"))
                );
        }

        [Test]
        public void Should_not_match_unknown_opt_and_arg()
        {
            Assert.AreEqual(
                new MatchResult(false,
                                Leaves(Argument(98), new Option("-x")),
                                Leaves()),
                new OneOrMore(new Option("-a")).Match(Argument(98), new Option("-x"))
                );
        }

        [Test]
        public void Should_match_all_opt_and_args()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                Leaves(new Option("-x")),
                                Leaves(new Option("-a"), Argument("N", 1),
                                       new Option("-a"), Argument("N", 2))
                    ),
                new OneOrMore(new Required(new Option("-a"), new Argument("N"))).Match(new Option("-a"), Argument(1), new Option("-x"), new Option("-a"), Argument(2))
                );
        }

        [Test]
        public void Should_match_optional_arg()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                Leaves(),
                                Leaves(Argument("N", 9))
                    ),
                new OneOrMore(new Optional(new Argument("N"))).Match(Argument(9))
                );
        }
    }
}
