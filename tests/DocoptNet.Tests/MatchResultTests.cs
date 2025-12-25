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

        [Test]
        public void Definitely_True_When_Matched()
        {
            var match = new MatchResult(true, Leaves(), Leaves());

            if (match)
                Assert.Pass();
            else
                Assert.Fail();
        }

        [Test]
        public void Not_Definitely_True_When_Not_Matched()
        {
            var match = new MatchResult(false, Leaves(), Leaves());

            if (match)
                Assert.Fail();
            else
                Assert.Pass();
        }

        [TestCase(false)]
        [TestCase(true)]
        public void Definitely_False(bool matched)
        {
            var match = new MatchResult(matched, Leaves(), Leaves());
            // Direct invocation of some operators is forbidden:
            //     MatchResult.op_False(match);
            // The compiler emits the following error:
            //     error CS0571: 'MatchResult.operator false(MatchResult)': cannot explicitly call operator or accessor
            // Therefore resort to run-time reflection...
            var method = typeof(MatchResult).GetMethod("op_False");
            Assert.That(method, Is.Not.Null);
            var result = (bool?)method!.Invoke(null, [match]);

            if (result == !matched)
                Assert.Pass();
            else
                Assert.Fail();
        }
    }
}
