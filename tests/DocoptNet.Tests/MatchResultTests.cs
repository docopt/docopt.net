namespace DocoptNet.Tests
{
    using NUnit.Framework;
    using static DocoptNet.Tests.PatternFactory;

    [TestFixture]
    public class MatchResultTests
    {
        [TestCase(true)]
        [TestCase(false)]
        public void Initialization(bool matched)
        {
            var left = Leaves(new Option("-a"));
            var collected = Leaves(new Option("-b"));
            var match = new MatchResult(matched, left, collected);

            Assert.That(match.Matched, Is.EqualTo(matched));
            Assert.That(match.Left, Is.EqualTo(left));
            Assert.That(match.Collected, Is.EqualTo(collected));
        }

        [Test]
        public void Default_Is_Mismatch_Having_Empty_Leaves()
        {
            var match = default(MatchResult); // == new MatchResult()
            Assert.That(match.Matched, Is.False);
            Assert.That(match.Left, Is.Empty);
            Assert.That(match.Collected, Is.Empty);
        }

        [Test]
        public void Deconstruction()
        {
            var match = new MatchResult(true, Leaves(new Option("-a")), Leaves(new Option("-b")));
            var (matched, left, collected) = match;

            Assert.That(matched, Is.EqualTo(match.Matched));
            Assert.That(left, Is.EqualTo(match.Left));
            Assert.That(collected, Is.EqualTo(match.Collected));
        }
    }
}
