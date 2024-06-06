namespace DocoptNet.Tests
{
    using NUnit.Framework;
    using static DocoptNet.Tests.PatternFactory;
    using Assert = NUnit.Framework.Legacy.ClassicAssert;

    [TestFixture]
    public class OptionMatchTests
    {
        [Test]
        public void Test_option_match_opt_matched()
        {
            var expected = new MatchResult(true, Leaves(), Leaves(new Option("-a", value: ArgValue.True)));
            var actual = new Option("-a").Match(new Option("-a", value: ArgValue.True));
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_option_match_opt_no_match()
        {
            var expected = new MatchResult(false, Leaves(new Option("-x")), Leaves());
            var actual = new Option("-a").Match(new Option("-x"));
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_option_match_opt_no_match_with_arg()
        {
            var expected = new MatchResult(false, Leaves(new Argument("N")), Leaves());
            var actual = new Option("-a").Match(new Argument("N"));
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_option_match_one_opt_out_of_two()
        {
            var expected = new MatchResult(true,
                                           Leaves(new Option("-x"), new Argument("N")),
                                           Leaves(new Option("-a"))
                );
            var actual = new Option("-a").Match(new Option("-x"), new Option("-a"), new Argument("N"));
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_option_match_same_opt_one_with_value()
        {
            var expected = new MatchResult(true,
                                           Leaves(new Option("-a")),
                                           Leaves(new Option("-a", value: ArgValue.True))
                );
            var actual =
                new Option("-a").Match(new Option("-a", value: ArgValue.True), new Option("-a"));
            Assert.AreEqual(expected, actual);
        }
    }
}
