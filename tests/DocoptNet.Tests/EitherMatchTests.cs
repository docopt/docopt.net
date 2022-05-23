namespace DocoptNet.Tests
{
    using NUnit.Framework;
    using static DocoptNet.Tests.ArgumentFactory;
    using static DocoptNet.Tests.PatternFactory;

    [TestFixture]
    public class EitherMatchTests
    {
        [Test]
        public void Should_match_option()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                Leaves(),
                                Leaves(new Option("-a") )),
                new Either(new Option("-a"), new Option("-b")).Match(new Option("-a"))
                );
        }

        [Test]
        public void Should_match_first_option()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                Leaves(new Option("-b") ),
                                Leaves(new Option("-a") )),
                new Either(new Option("-a"), new Option("-b")).Match(new Option("-a"), new Option("-b"))
                );
        }

        [Test]
        public void Should_not_match_other_option()
        {
            Assert.AreEqual(
                new MatchResult(false,
                                Leaves(new Option("-x") ),
                                Leaves()),
                new Either(new Option("-a"), new Option("-b")).Match(new Option("-x"))
                );
        }

        [Test]
        public void Should_match_one_option_out_of_three()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                Leaves(new Option("-x") ),
                                Leaves(new Option("-b") )),
                new Either(new Option("-a"), new Option("-b"), new Option("-c")).Match(new Option("-x"), new Option("-b"))
                );
        }

        [Test]
        public void Should_match_required_args()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                Leaves(),
                                Leaves(Argument("N", 1), Argument("M", 2) )),
                new Either(new Argument("M"), new Required(new Argument("N"), new Argument("M"))).Match(Argument(1), Argument(2))
                );
        }

    }
}
