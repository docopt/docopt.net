using NUnit.Framework;

namespace DocoptNet.Tests
{
    [TestFixture]
    public class GenerateCodeHelperTest
    {
        [Test]
        public void ConvertDashesToCamelCase_single_dashes()
        {
            var actual = GenerateCodeHelper.ConvertDashesToCamelCase("string-with-dashes");
            Assert.AreEqual("StringWithDashes", actual);
        }

        [Test]
        public void ConvertDashesToCamelCase_consecutive_dashes()
        {
            var input = "string--with----dashes";
            var expected = "StringWithDashes";
            var actual = GenerateCodeHelper.ConvertDashesToCamelCase(input);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ConvertDashesToCamelCase_existing_uppercase_letters()
        {
            var input = "string-With-Dashes";
            var expected = "StringWithDashes";
            var actual = GenerateCodeHelper.ConvertDashesToCamelCase(input);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ConvertDashesToCamelCase_all_uppercase_letters()
        {
            var input = "STRING-WITH-DASHES";
            var expected = "STRINGWITHDASHES";
            var actual = GenerateCodeHelper.ConvertDashesToCamelCase(input);
            Assert.AreEqual(expected, actual);
        }
    }
}
