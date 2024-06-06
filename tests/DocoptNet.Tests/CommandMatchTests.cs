namespace DocoptNet.Tests
{
    using NUnit.Framework;
    using static DocoptNet.Tests.ArgumentFactory;
    using static DocoptNet.Tests.PatternFactory;
    using Assert = NUnit.Framework.Legacy.ClassicAssert;

    [TestFixture]
    public class CommandMatchTests
    {
        [Test]
        public void Should_match_command()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                Leaves(),
                                Leaves(new Command("c", true))),
                new Command("c").Match(Argument("c"))
                );
        }

        [Test]
        public void Should_not_match_command()
        {
            Assert.AreEqual(
                new MatchResult(false,
                                Leaves(new Option("-x")),
                                Leaves()),
                new Command("c").Match(new Option("-x"))
                );
        }

        [Test]
        public void Should_match_command_after_options()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                Leaves(new Option("-x"), new Option("-a") ),
                                Leaves(new Command("c", true) )),
                new Command("c").Match(new Option("-x"), new Option("-a"), Argument("c"))
                );
        }

        [Test]
        public void Should_match_either_command()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                Leaves(),
                                Leaves(new Command("rm", true) )),
                new Either(new Command("add"), new Command("rm")).Match(Argument("rm"))
                );
        }

    }
}
