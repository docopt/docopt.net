namespace DocoptNet.Tests
{
    using Internals;
    using NUnit.Framework;

    [TestFixture]
    public class ArgumentTests
    {
        [Test]
        public void ToString_string_value_returns_string_in_expected_format()
        {
            var argument = new Argument("arg", "foobar");
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
            var argument = new Argument("arg", new[] { "foo", "bar", "baz" });
            Assert.AreEqual("Argument(arg, [foo, bar, baz])", argument.ToString());
        }
    }
}
