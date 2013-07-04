using NUnit.Framework;

namespace NDocOpt.Tests
{
    [TestFixture]
    public class CommandMatchTests
    {
        [Test]
        public void Should_match_command()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                new Pattern[0],
                                new Pattern[] {new Command("c", new ValueObject(true))}),
                new Command("c").Match(new Pattern[] {new Argument(null, new ValueObject("c"))})
                );
        }

        [Test]
        public void Should_not_match_command()
        {
            Assert.AreEqual(
                new MatchResult(false,
                                new Pattern[] {new Option("-x")},
                                new Pattern[0]),
                new Command("c").Match(new Pattern[] {new Option("-x")})
                );
        }

        [Test]
        public void Should_match_command_after_options()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                new Pattern[] { new Option("-x"), new Option("-a") },
                                new Pattern[] { new Command("c", new ValueObject(true)) }),
                new Command("c").Match(new Pattern[] { new Option("-x"), new Option("-a"), new Argument(null, new ValueObject("c"))   })
                );
        }

        [Test]
        public void Should_match_either_command()
        {
            Assert.AreEqual(
                new MatchResult(true,
                                new Pattern[0],
                                new Pattern[] { new Command("rm", new ValueObject(true)) }),
                new Either(new Command("add"), new Command("rm")).Match(new Pattern[] { new Argument(null, new ValueObject("rm")) })
                );
        }

    }
}