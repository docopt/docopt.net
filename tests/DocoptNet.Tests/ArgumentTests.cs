namespace DocoptNet.Tests
{
    using NUnit.Framework;
    using static DocoptNet.Tests.ArgumentFactory;

    [TestFixture]
    public class ArgumentTests
    {
        [Test]
        public void ToString_string_value_returns_string_in_expected_format()
        {
            var argument = Argument("arg", "foobar");
            Assert.AreEqual("Argument(arg, foobar)", argument.ToString());
        }

        [Test]
        public void ToString_null_value_returns_string_in_expected_format()
        {
            var argument = new Argument("arg");
            Assert.AreEqual("Argument(arg, )", argument.ToString());
        }

        [Test]
        public void ToString_array_value_returns_string_in_expected_format()
        {
            var argument = Argument("arg", StringList.BottomTop("foo", "bar", "baz"));
            Assert.AreEqual("Argument(arg, [foo, bar, baz])", argument.ToString());
        }
    }
}
