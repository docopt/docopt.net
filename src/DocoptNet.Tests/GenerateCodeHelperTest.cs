using NUnit.Framework;

namespace DocoptNet.Tests
{
    [TestFixture]
    public class GenerateCodeHelperTest
    {
        [Test]
        public void GenerateCode()
        {
            var input = "string-with-dashes";
            var expected = "StringWithDashes";
            var actual = GenerateCodeHelper.GenerateCode(input);
            Assert.AreEqual(expected, actual);
        }
    }
}