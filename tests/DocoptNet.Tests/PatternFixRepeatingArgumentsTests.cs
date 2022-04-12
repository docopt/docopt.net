namespace DocoptNet.Tests
{
    using System;
    using NUnit.Framework;
    using static DocoptNet.Tests.ArgumentFactory;

    [TestFixture]
    public class PatternFixRepeatingArgumentsTests
    {
        /* FIXME
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
        */
        [Test]
        public void Should_fix_required_args()
        {
            Assert.AreEqual(
                new Required(Argument("N", StringList.Empty), Argument("N", StringList.Empty)),
                new Required(new Argument("N"), new Argument("N")).FixRepeatingArguments()
                );
        }

        [Test]
        public void Should_fix_either_args()
        {
            Assert.AreEqual(
                new Either(Argument("N", StringList.Empty),
                           new OneOrMore(Argument("N", StringList.Empty))),
                new Either(new Argument("N"), new OneOrMore(new Argument("N"))).Fix()
                );
        }
    }
}
