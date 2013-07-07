using NUnit.Framework;

namespace DocoptNet.Tests
{
    [TestFixture]
    public class OptionMatchTests
    {
        [Test]
        public void Test_option_match_opt_matched()
        {
            var expected = new MatchResult(true, new Pattern[0], new[] {new Option("-a", value: new ValueObject(true))});
            var actual = new Option("-a").Match(new[] {new Option("-a", value: new ValueObject(true))});
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_option_match_opt_no_match()
        {
            var expected = new MatchResult(false, new[] {new Option("-x")}, new Pattern[0]);
            var actual = new Option("-a").Match(new[] {new Option("-x")});
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_option_match_opt_no_match_with_arg()
        {
            var expected = new MatchResult(false, new[] {new Argument("N"),}, new Pattern[0]);
            var actual = new Option("-a").Match(new[] {new Argument("N")});
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_option_match_one_opt_out_of_two()
        {
            var expected = new MatchResult(true,
                                           new Pattern[] {new Option("-x"), new Argument("N")},
                                           new[] {new Option("-a")}
                );
            var actual = new Option("-a").Match(new Pattern[] {new Option("-x"), new Option("-a"), new Argument("N")});
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_option_match_same_opt_one_with_value()
        {
            var expected = new MatchResult(true,
                                           new Pattern[] {new Option("-a")},
                                           new Pattern[] {new Option("-a", value: new ValueObject(true))}
                );
            var actual =
                new Option("-a").Match(new Pattern[] { new Option("-a", value: new ValueObject(true)), new Option("-a") });
            Assert.AreEqual(expected, actual);
        }
    }
}