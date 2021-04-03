using NUnit.Framework;

namespace DocoptNet.Tests
{
    [TestFixture]
    public class EitherMatchTests
    {
        [Test]
        public void Should_match_option()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                new LeafPattern[0],
                                new LeafPattern[] { new Option("-a") }),
                new Either(new Option("-a"), new Option("-b")).Match(new LeafPattern[] { new Option("-a") })
                );
        }

        [Test]
        public void Should_match_first_option()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                new LeafPattern[] { new Option("-b") },
                                new LeafPattern[] { new Option("-a") }),
                new Either(new Option("-a"), new Option("-b")).Match(new LeafPattern[] { new Option("-a"), new Option("-b"),  })
                );
        }

        [Test]
        public void Should_not_match_other_option()
        {
            Assert.AreEqual(
                new MatchResult(false,
                                new LeafPattern[] { new Option("-x") },
                                new LeafPattern[0]),
                new Either(new Option("-a"), new Option("-b")).Match(new LeafPattern[] { new Option("-x") })
                );
        }

        [Test]
        public void Should_match_one_option_out_of_three()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                new LeafPattern[] { new Option("-x") },
                                new LeafPattern[] { new Option("-b") }),
                new Either(new Option("-a"), new Option("-b"), new Option("-c")).Match(new LeafPattern[] { new Option("-x"), new Option("-b") })
                );
        }

        [Test]
        public void Should_match_required_args()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                new LeafPattern[0],
                                new LeafPattern[] { new Argument("N", 1), new Argument("M", 2) }),
                new Either(new Argument("M"), new Required(new Argument("N"), new Argument("M"))).Match(new LeafPattern[] { new Argument(null, 1), new Argument(null, 2) })
                );
        }

    }
}
