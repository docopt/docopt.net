using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace DocoptNet.Tests
{
    [TestFixture]
    public class ValueObjectTests
    {
        [Test]
        public void IsNullOrEmpty_null_object_evaluates_true()
        {
            var systemUnderTest = new ValueObject(null);
            Assert.IsTrue(systemUnderTest.IsNullOrEmpty, "IsNullOrEmpty should evaluate to true when ValueObject wraps null.");
        }

        [Test]
        public void IsNullOrEmpty_empty_string_evaluates_true()
        {
            var systemUnderTest = new ValueObject(string.Empty);
            Assert.IsTrue(systemUnderTest.IsNullOrEmpty, "IsNullOrEmpty should evaluate to true when ValueObject wraps the empty string.");
        }

        [Test]
        public void IsNullOrEmpty_nonempty_string_evaluates_false()
        {
            var systemUnderTest = new ValueObject("a");
            Assert.IsFalse(systemUnderTest.IsNullOrEmpty, "IsNullOrEmpty should evaluate to false when ValueObject wraps a non-empty string.");
        }

        [Test]
        public void IsNullOrEmpty_number_evaluates_false()
        {
            var systemUnderTest = new ValueObject(1);
            Assert.IsFalse(systemUnderTest.IsNullOrEmpty, "IsNullOrEmpty should evaluate to false when ValueObject wraps an integer.");
        }

        [Test]
        public void IsNullOrEmpty_bool_evaluates_false()
        {
            var systemUnderTest = new ValueObject(true);
            Assert.IsFalse(systemUnderTest.IsNullOrEmpty, "IsNullOrEmpty should evaluate to false when ValueObject wraps a bool.");
            systemUnderTest = new ValueObject(false);
            Assert.IsFalse(systemUnderTest.IsNullOrEmpty, "IsNullOrEmpty should evaluate to false when ValueObject wraps a bool.");
        }

        [Test]
        public void IsNullOrEmpty_list_evaluates_false()
        {
            var systemUnderTest = new ValueObject(new ArrayList());
            Assert.IsFalse(systemUnderTest.IsNullOrEmpty, "IsNullOrEmpty should evaluate to false when ValueObject wraps a list.");
        }

        [Test]
        public void IsFalse_false_evaluates_true()
        {
            var systemUnderTest = new ValueObject(false);
            Assert.IsTrue(systemUnderTest.IsFalse, "IsFalse should evaluate to true when ValueObject wraps false.");
        }

        [Test]
        public void IsFalse_true_evaluates_false()
        {
            var systemUnderTest = new ValueObject(new ArrayList());
            Assert.IsFalse(systemUnderTest.IsFalse, "IsFalse should evaluate to false when ValueObject wraps true.");
        }

        [Test]
        public void IsFalse_null_evaluates_false()
        {
            var systemUnderTest = new ValueObject(null);
            Assert.IsFalse(systemUnderTest.IsFalse, "IsFalse should evaluate to false when ValueObject wraps null.");
        }

        [Test]
        public void IsFalse_string_evaluates_false()
        {
            var systemUnderTest = new ValueObject("false");
            Assert.IsFalse(systemUnderTest.IsFalse, "IsFalse should evaluate to false when ValueObject wraps a string.");
        }

        [Test]
        public void IsFalse_number_evaluates_false()
        {
            var systemUnderTest = new ValueObject(0);
            Assert.IsFalse(systemUnderTest.IsFalse, "IsFalse should evaluate to false when ValueObject wraps an integer.");
        }

        [Test]
        public void IsTrue_true_evaluates_true()
        {
            var systemUnderTest = new ValueObject(true);
            Assert.IsTrue(systemUnderTest.IsTrue, "IsTrue should evaluate to true when ValueObject wraps true.");
        }

        [Test]
        public void IsTrue_false_evaluates_false()
        {
            var systemUnderTest = new ValueObject(new ArrayList());
            Assert.IsFalse(systemUnderTest.IsTrue, "IsTrue should evaluate to false when ValueObject wraps false.");
        }

        [Test]
        public void IsTrue_null_evaluates_false()
        {
            var systemUnderTest = new ValueObject(null);
            Assert.IsFalse(systemUnderTest.IsTrue, "IsTrue should evaluate to false when ValueObject wraps null.");
        }

        [Test]
        public void IsTrue_string_evaluates_false()
        {
            var systemUnderTest = new ValueObject("true");
            Assert.IsFalse(systemUnderTest.IsTrue, "IsTrue should evaluate to false when ValueObject wraps a string.");
        }

        [Test]
        public void IsTrue_number_evaluates_false()
        {
            var systemUnderTest = new ValueObject(1);
            Assert.IsFalse(systemUnderTest.IsTrue, "IsTrue should evaluate to false when ValueObject wraps an integer.");
        }

        [Test]
        public void IsList_arraylist_evaluates_true()
        {
            var systemUnderTest = new ValueObject(new ArrayList());
            Assert.IsTrue(systemUnderTest.IsList, "IsList should evaluate to true when ValueObject wraps an ArrayList.");
        }

        [Test]
        public void IsList_genericlist_evaluates_true()
        {
            var systemUnderTest = new ValueObject(new List<object>());
            Assert.IsTrue(systemUnderTest.IsList, "IsList should evaluate to true when ValueObject wraps an implementation of ICollection.");
        }

        [Test]
        public void IsList_string_evaluates_false()
        {
            var systemUnderTest = new ValueObject("1,2,3,4,5");
            Assert.IsFalse(systemUnderTest.IsList, "IsList should evaluate to false when ValueObject wraps a string.");
        }

        [Test]
        public void IsOfTypeInt_int_evaluates_true()
        {
            var systemUnderTest = new ValueObject(1);
            Assert.IsTrue(systemUnderTest.IsOfTypeInt, "IsOfTypeInt should evaluate to true when ValueObject wraps an integer.");
        }

        [Test]
        public void IsOfTypeInt_integer_string_evaluates_false()
        {
            var systemUnderTest = new ValueObject("2");
            Assert.IsFalse(systemUnderTest.IsOfTypeInt, "IsOfTypeInt should evaluate to false when ValueObject wraps a string that can be converted to an integer.");
        }

        [Test]
        public void IsOfTypeInt_noninteger_string_evaluates_false()
        {
            var systemUnderTest = new ValueObject("3.14");
            Assert.IsFalse(systemUnderTest.IsOfTypeInt, "IsOfTypeInt should evaluate to false when ValueObject wraps a string that cannot be converted to an integer.");
            systemUnderTest = new ValueObject("abcde");
            Assert.IsFalse(systemUnderTest.IsOfTypeInt, "IsOfTypeInt should evaluate to false when ValueObject wraps a string that cannot be converted to an integer.");
        }

        [Test]
        public void IsOfTypeInt_list_evaluates_false()
        {
            var systemUnderTest = new ValueObject(new ArrayList());
            Assert.IsFalse(systemUnderTest.IsOfTypeInt, "IsOfTypeInt should evaluate to false when ValueObject wraps a list.");
        }

        [Test]
        public void IsOfTypeInt_null_evaluates_false()
        {
            var systemUnderTest = new ValueObject(null);
            Assert.IsFalse(systemUnderTest.IsOfTypeInt, "IsOfTypeInt should evaluate to false when ValueObject wraps null.");
        }

        [Test]
        public void IsInt_int_evaluates_true()
        {
            var systemUnderTest = new ValueObject(1);
            Assert.IsTrue(systemUnderTest.IsInt, "IsInt should evaluate to true when ValueObject wraps an integer.");
        }

        [Test]
        public void IsInt_integer_string_evaluates_true()
        {
            var systemUnderTest = new ValueObject("2");
            Assert.IsTrue(systemUnderTest.IsInt, "IsInt should evaluate to true when ValueObject wraps a string that can be converted to an integer.");
        }

        [Test]
        public void IsInt_noninteger_string_evaluates_false()
        {
            var systemUnderTest = new ValueObject("3.14");
            Assert.IsFalse(systemUnderTest.IsInt, "IsInt should evaluate to false when ValueObject wraps a string that cannot be converted to an integer.");
            systemUnderTest = new ValueObject("abcde");
            Assert.IsFalse(systemUnderTest.IsInt, "IsInt should evaluate to false when ValueObject wraps a string that cannot be converted to an integer.");
        }

        [Test]
        public void IsInt_list_evaluates_false()
        {
            var systemUnderTest = new ValueObject(new ArrayList());
            Assert.IsFalse(systemUnderTest.IsInt, "IsInt should evaluate to false when ValueObject wraps a list.");
        }

        [Test]
        public void IsInt_null_evaluates_false()
        {
            var systemUnderTest = new ValueObject(null);
            Assert.IsFalse(systemUnderTest.IsInt, "IsInt should evaluate to false when ValueObject wraps null.");
        }

        [Test]
        public void AsInt_int_evaluates_to_expected_value()
        {
            const int expected = 10;
            var systemUnderTest = new ValueObject(expected);
            Assert.AreEqual(expected, systemUnderTest.AsInt, "AsInt should return the value of the wrapped integer.");
        }

        [Test]
        public void AsInt_integer_string_evaluates_to_expected_value()
        {
            const int expected = 20;
            var systemUnderTest = new ValueObject(expected.ToString());
            Assert.AreEqual(expected, systemUnderTest.AsInt, "AsInt should return the value of the wrapped string.");
        }

        [Test]
        public void IsString_empty_string_evalutes_true()
        {
            var systemUnderTest = new ValueObject(string.Empty);
            Assert.IsTrue(systemUnderTest.IsString, "IsString should evaluate to true when ValueObject wraps an empty string.");
        }

        [Test]
        public void IsString_nonempty_string_evalutes_true()
        {
            var systemUnderTest = new ValueObject("b");
            Assert.IsTrue(systemUnderTest.IsString, "IsString should evaluate to true when ValueObject wraps a non-empty string.");
        }

        [Test]
        public void IsString_integer_string_evalutes_true()
        {
            var systemUnderTest = new ValueObject("5");
            Assert.IsTrue(systemUnderTest.IsString, "IsString should evaluate to true when ValueObject wraps a string that can be converted to an integer.");
        }

        [Test]
        public void IsString_list_evalutes_false()
        {
            var systemUnderTest = new ValueObject(new ArrayList());
            Assert.IsFalse(systemUnderTest.IsString, "IsString should evaluate to false when ValueObject wraps a list.");
        }

        [Test]
        public void IsString_null_evalutes_false()
        {
            var systemUnderTest = new ValueObject(null);
            Assert.IsFalse(systemUnderTest.IsString, "IsString should evaluate to false when ValueObject wraps null.");
        }

        [Test]
        public void AsList_list_evalutes_to_expected_collection()
        {
            var expected = new ArrayList() { 1, "xyz", null };
            var systemUnderTest = new ValueObject(expected);
            CollectionAssert.AreEqual(expected, systemUnderTest.AsList, "AsList should evalute to a collection with the same elements in the same order as the wrapped list.");
        }

        [Test]
        public void AsList_array_evalutes_to_expected_collection()
        {
            var expected = new int[] { 1, 1, 2, 3, 5, 8 };
            var systemUnderTest = new ValueObject(expected);
            CollectionAssert.AreEqual(expected, systemUnderTest.AsList, "AsList should evalute to a collection with the same elements in the same order as the wrapped array.");
        }

        [Test]
        public void AsList_string_evalutes_to_single_element_collection()
        {
            const string expected = "c";
            var systemUnderTest = new ValueObject(expected);
            CollectionAssert.AreEqual(new [] { expected }, systemUnderTest.AsList, "AsList should evalute to a collection containing only the wrapped object.");
        }
    }
}
