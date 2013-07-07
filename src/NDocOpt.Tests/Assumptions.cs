using System;
using NUnit.Framework;

namespace DocoptNet.Tests
{
    /// <summary>
    ///     Set of tests to validate assumptions about the BCL or other APIs.
    /// </summary>
    [TestFixture]
    public class Assumptions
    {
        [Test]
        public void String_Split_with_no_args_should_split_on_white_space()
        {
            const string TEST_STRING = "first second\tthird\nfourth \n\t\ffifth";
            var s1 = TEST_STRING.Split();
            Assert.AreEqual(8, s1.Length);
            var s2 = TEST_STRING.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
            Assert.AreEqual(5, s2.Length);
        }
    }
}