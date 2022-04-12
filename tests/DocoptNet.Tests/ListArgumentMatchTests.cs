namespace DocoptNet.Tests
{
    using NUnit.Framework;
    using static DocoptNet.Tests.ArgumentFactory;
    using static DocoptNet.Tests.PatternFactory;

    [TestFixture]
    public class ListArgumentMatchTests
    {
        [Test]
        public void Should_match_required_two_args_into_a_list()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                Leaves(),
                                Leaves(Argument("N", StringList.BottomTop("1", "2")) )),
                new Required(new Argument("N"), new Argument("N")).Fix().Match(Argument("1"), Argument("2"))
                );
        }

        [Test]
        public void Should_match_oneormore_arg_into_a_list()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                Leaves(),
                                Leaves(Argument("N", StringList.BottomTop("1", "2", "3")))),
                new OneOrMore(new Argument("N")).Fix().Match(Argument("1"), Argument("2"), Argument("3"))
                );
        }

        [Test]
        public void Should_match_required_and_oneormore_arg_into_a_list()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                Leaves(),
                                Leaves(Argument("N", StringList.BottomTop("1", "2", "3")))),
                    new Required(new Argument("N"), new OneOrMore(new Argument("N"))).Fix().Match(Argument("1"), Argument("2"), Argument("3"))
                );
        }

        [Test]
        public void Should_match_doubly_required_arg_into_a_list()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                Leaves(),
                                Leaves(Argument("N", StringList.BottomTop("1", "2")) )),
                    new Required(new Argument("N"), new Required(new Argument("N"))).Fix().Match(Argument("1"), Argument("2"))
                );
        }
    }
}
