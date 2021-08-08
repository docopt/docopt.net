namespace DocoptNet.Tests
{
    using System.Collections;
    using System.Collections.Generic;
    using NUnit.Framework;

    [TestFixture]
    public class PatternFixRepeatingArgumentsTests
    {
        [Test]
        public void Should_fix_option()
        {
            Assert.AreEqual(
                new Option("-a"),
                new Option("-a").FixRepeatingArguments()
                );
        }

        [Test]
        public void Should_fix_arg()
        {
            Assert.AreEqual(
                new Argument("A"),
                new Argument("A").FixRepeatingArguments()
                );
        }

        [Test]
        public void Should_fix_required_args()
        {
            Assert.AreEqual(
                new Required(new Argument("N", new List<string>()), new Argument("N", new List<string>())),
                new Required(new Argument("N"), new Argument("N")).FixRepeatingArguments()
                );
        }

        [Test]
        public void Should_fix_either_args()
        {
            Assert.AreEqual(
                new Either(new Argument("N", new List<string>()),
                           new OneOrMore(new Argument("N", new List<string>()))),
                new Either(new Argument("N"), new OneOrMore(new Argument("N"))).Fix()
                );
        }
    }
}
