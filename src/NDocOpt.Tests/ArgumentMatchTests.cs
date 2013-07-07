using NUnit.Framework;

namespace DocoptNet.Tests
{
    [TestFixture]
    public class ArgumentMatchTests
    {
        [Test]
        public void Should_match_arg()
        {
            Assert.AreEqual(
                new MatchResult(true, new Pattern[0], new Pattern[] {new Argument("N", new ValueObject(9))}),
                new Argument("N").Match(new Pattern[] {new Argument(null, new ValueObject(9))})
                );
        }

        [Test]
        public void Should_not_match_arg()
        {
            Assert.AreEqual(
                new MatchResult(false, new Pattern[] {new Option("-x")}, new Pattern[0]),
                new Argument("N").Match(new Pattern[] {new Option("-x")})
                );
        }

        [Test]
        public void Should_match_arg_after_opts()
        {
            Assert.AreEqual(
                new MatchResult(true, new Pattern[] {new Option("-x"), new Option("-a")},
                                new Pattern[] {new Argument("N", new ValueObject(5))}),
                new Argument("N").Match(new Pattern[]
                    {
                        new Option("-x"),
                        new Option("-a"),
                        new Argument(null, new ValueObject(5))
                    })
                );
        }

        [Test]
        public void Should_match_first_arg_only()
        {
            Assert.AreEqual(
                new MatchResult(true,
                    new Pattern[] {new Argument(null, new ValueObject(0))},
                    new Pattern[] {new Argument("N", new ValueObject(9))}),
                new Argument("N").Match(new Pattern[]
                    {new Argument(null, new ValueObject(9)), new Argument(null, new ValueObject(0))})
                );
        }
    }
}