namespace DocoptNet.Tests
{
    using System;
    using NUnit.Framework;
    using Assert = NUnit.Framework.Legacy.ClassicAssert;

    /// <summary>
    ///     Set of tests to validate assumptions about the BCL or other APIs.
    /// </summary>
    [TestFixture]
    public class Assumptions
    {
        [Test]
        public void String_Split_with_no_args_should_split_on_white_space()
        {
            const string testString = "first second\tthird\nfourth \n\t\ffifth";
            var s1 = testString.Split();
            Assert.AreEqual(8, s1.Length);
            var s2 = testString.Split(Array.Empty<char>(), StringSplitOptions.RemoveEmptyEntries);
            Assert.AreEqual(5, s2.Length);
        }
    }
}
