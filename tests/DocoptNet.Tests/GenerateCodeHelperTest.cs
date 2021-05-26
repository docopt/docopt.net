namespace DocoptNet.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class GenerateCodeHelperTest
    {
        [TestCase('-')]
        [TestCase(' ')]
        public void ConvertToPascalCase_single_dashes(char sep)
        {
            var actual = GenerateCodeHelper.ConvertToPascalCase("string" + sep + "with" + sep + "dashes");
            Assert.AreEqual("StringWithDashes", actual);
        }

        [TestCase('-')]
        [TestCase(' ')]
        public void ConvertToPascalCase_consecutive_dashes(char sep)
        {
            var input = "string" + sep + sep + "with" + sep + sep + sep + sep + "dashes";
            var expected = "StringWithDashes";
            var actual = GenerateCodeHelper.ConvertToPascalCase(input);
            Assert.AreEqual(expected, actual);
        }

        [TestCase('-')]
        [TestCase(' ')]
        public void ConvertToPascalCase_existing_uppercase_letters(char sep)
        {
            var input = "string" + sep + "With" + sep + "Dashes";
            var expected = "StringWithDashes";
            var actual = GenerateCodeHelper.ConvertToPascalCase(input);
            Assert.AreEqual(expected, actual);
        }

        [TestCase('-')]
        [TestCase(' ')]
        public void ConvertToPascalCase_all_uppercase_letters(char sep)
        {
            var input = "STRING" + sep + "WITH" + sep + "DASHES";
            var expected = "STRINGWITHDASHES";
            var actual = GenerateCodeHelper.ConvertToPascalCase(input);
            Assert.AreEqual(expected, actual);
        }

        [TestCase("foo-bar", "FooBar")]
        [TestCase("foo bar", "FooBar")]
        [TestCase("foo+bar", "FooPlusBar")]
        [TestCase("foo/bar", "FooSlashBar")]
        [TestCase("-", "Minus")]
        public void ConvertDashesToCamelCase(string input, string expected)
        {
            var actual = GenerateCodeHelper.ConvertToPascalCase(input);
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
