using NUnit.Framework;

namespace DocoptNet.Tests
{
    [TestFixture]
    public class CommandMatchTests
    {
        [Test]
        public void Should_match_command()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                new LeafPattern[0],
                                new LeafPattern[] {new Command("c", new ValueObject(true))}),
                new Command("c").Match(new LeafPattern[] {new Argument(null, new ValueObject("c"))})
                );
        }

        [Test]
        public void Should_not_match_command()
        {
            Assert.AreEqual(
                new MatchResult(false,
                                new LeafPattern[] {new Option("-x")},
                                new LeafPattern[0]),
                new Command("c").Match(new LeafPattern[] {new Option("-x")})
                );
        }

        [Test]
        public void Should_match_command_after_options()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                new LeafPattern[] { new Option("-x"), new Option("-a") },
                                new LeafPattern[] { new Command("c", new ValueObject(true)) }),
                new Command("c").Match(new LeafPattern[] { new Option("-x"), new Option("-a"), new Argument(null, new ValueObject("c"))   })
                );
        }

        [Test]
        public void Should_match_either_command()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                new LeafPattern[0],
                                new LeafPattern[] { new Command("rm", new ValueObject(true)) }),
                new Either(new Command("add"), new Command("rm")).Match(new LeafPattern[] { new Argument(null, new ValueObject("rm")) })
                );
        }

    }
}
