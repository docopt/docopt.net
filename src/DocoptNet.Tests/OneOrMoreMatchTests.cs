using NUnit.Framework;

namespace DocoptNet.Tests
{
    [TestFixture]
    public class OneOrMoreMatchTests
    {
        [Test]
        public void Should_match_arg()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                new LeafPattern[0],
                                new LeafPattern[] {new Argument("N", new ValueObject(9))}),
                new OneOrMore(new Argument("N")).Match(new LeafPattern[] {new Argument(null, new ValueObject(9))})
                );
        }

        [Test]
        public void Should_not_match_empty()
        {
            Assert.AreEqual(
                new MatchResult(false,
                                new LeafPattern[0],
                                new LeafPattern[0]),
                new OneOrMore(new Argument("N")).Match(new LeafPattern[0])
                );
        }

        [Test]
        public void Should_not_match_option()
        {
            Assert.AreEqual(
                new MatchResult(false,
                                new LeafPattern[] {new Option("-x")},
                                new LeafPattern[0]
                    ),
                new OneOrMore(new Argument("N")).Match(new LeafPattern[] {new Option("-x")})
                );
        }

        [Test]
        public void Should_match_all_args()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                new LeafPattern[0],
                                new LeafPattern[]
                                    {new Argument("N", new ValueObject(9)), new Argument("N", new ValueObject(8))}),
                new OneOrMore(new Argument("N")).Match(new LeafPattern[]
                                                           {new Argument(null, new ValueObject(9)), new Argument(null, new ValueObject(8))})
                );
        }

        [Test]
        public void Should_not_match_unknown_opt()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                new LeafPattern[] {new Option("-x")},
                                new LeafPattern[]
                                    {new Argument("N", new ValueObject(9)), new Argument("N", new ValueObject(8))}),
                new OneOrMore(new Argument("N")).Match(new LeafPattern[]
                                                           {new Argument(null, new ValueObject(9)), new Option("-x"), new Argument(null, new ValueObject(8))})
                );
        }

        [Test]
        public void Should_not_match_unknown_arg()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                new LeafPattern[] {new Argument(null, new ValueObject(8))},
                                new LeafPattern[] {new Option("-a"), new Option("-a")}
                    ),
                new OneOrMore(new Option("-a")).Match(new LeafPattern[]
                                                          {new Option("-a"), new Argument(null, new ValueObject(8)), new Option("-a")})
                );
        }

        [Test]
        public void Should_not_match_unknown_opt_and_arg()
        {
            Assert.AreEqual(
                new MatchResult(false,
                                new LeafPattern[] {new Argument(null, new ValueObject(98)), new Option("-x")},
                                new LeafPattern[0]),
                new OneOrMore(new Option("-a")).Match(new LeafPattern[]
                                                          {new Argument(null, new ValueObject(98)), new Option("-x")})
                );
        }

        [Test]
        public void Should_match_all_opt_and_args()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                new LeafPattern[] {new Option("-x")},
                                new LeafPattern[]
                                    {
                                        new Option("-a"), new Argument("N", new ValueObject(1)),
                                        new Option("-a"), new Argument("N", new ValueObject(2))
                                    }
                    ),
                new OneOrMore(new Required(new Option("-a"), new Argument("N"))).Match(new LeafPattern[]
                {
                    new Option("-a"), new Argument(null, new ValueObject(1)),
                    new Option("-x"),
                    new Option("-a"), new Argument(null, new ValueObject(2))
                })
                );
        }

        [Test]
        public void Should_match_optional_arg()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                new LeafPattern[0],
                                new LeafPattern[] {new Argument("N", new ValueObject(9))}
                    ),
                new OneOrMore(new Optional(new Argument("N"))).Match(new LeafPattern[]
                    {new Argument(null, new ValueObject(9))})
                );
        }
    }
}
