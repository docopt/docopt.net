namespace DocoptNet.Tests
{
    using Internals;
    using NUnit.Framework;
    using static DocoptNet.Tests.PatternFactory;

    [TestFixture]
    public class ArgumentMatchTests
    {
        [Test]
        public void Should_match_arg()
        {
            Assert.AreEqual(
                new MatchResult(true, Leaves(), Leaves(new Argument("N", 9))),
                new Argument("N").Match(new Argument(null, 9))
                );
        }

        [Test]
        public void Should_not_match_arg()
        {
            Assert.AreEqual(
                new MatchResult(false, Leaves(new Option("-x")), Leaves()),
                new Argument("N").Match(new Option("-x"))
                );
        }

        [Test]
        public void Should_match_arg_after_opts()
        {
            Assert.AreEqual(
                new MatchResult(true, Leaves(new Option("-x"), new Option("-a")),
                                Leaves(new Argument("N", 5))),
                new Argument("N").Match(new Option("-x"),
                                        new Option("-a"),
                                        new Argument(null, 5))
                );
        }

        [Test]
        public void Should_match_first_arg_only()
        {
            Assert.AreEqual(
                new MatchResult(true,
                    Leaves(new Argument(null, 0)),
                    Leaves(new Argument("N", 9))),
                new Argument("N").Match(new Argument(null, 9), new Argument(null, 0))
                );
        }
    }
}
