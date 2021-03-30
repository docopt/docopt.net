using System.Collections;
using NUnit.Framework;

namespace DocoptNet.Tests
{
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
                new Required(new Argument("N", new ArrayList()), new Argument("N", new ArrayList())),
                new Required(new Argument("N"), new Argument("N")).FixRepeatingArguments()
                );
        }

        [Test]
        public void Should_fix_either_args()
        {
            Assert.AreEqual(
                new Either(new Argument("N", new ArrayList()),
                           new OneOrMore(new Argument("N", new ArrayList()))),
                new Either(new Argument("N"), new OneOrMore(new Argument("N"))).Fix()
                );
        }
    }
}
