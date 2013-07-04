using NUnit.Framework;

namespace NDocOpt.Tests
{
    [TestFixture]
    public class PatternFlatTests
    {
        [Test]
        public void Should_flatten_all_types()
        {
            Assert.AreEqual(
                new Pattern[] {new Argument("N"), new Option("-a"), new Argument("M")},
                new Required(new OneOrMore(new Argument("N")),
                             new Option("-a"), new Argument("M")).Flat());
        }

        [Test]
        public void Should_flatten_specific_type()
        {
            Assert.AreEqual(
                new Pattern[] {new OptionsShortcut()},
                new Required(new Optional(new OptionsShortcut()),
                             new Optional(new Option("-a"))).Flat<OptionsShortcut>());
        }
    }
}