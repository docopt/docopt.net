#nullable enable

namespace DocoptNet.Tests
{
    using System;
    using System.Globalization;
    using NUnit.Framework;

    [TestFixture]
    public class ValueTests
    {
        [Test]
        public void Default_value_is_none()
        {
            var value = default(Value);
            Assert.That(value.Kind, Is.EqualTo(ValueKind.None));
        }

        [TestFixture]
        public class None
        {
            static Value Value => Value.None;

            [Test]
            public void Kind_returns_None()
            {
                Assert.That(Value.Kind, Is.EqualTo(ValueKind.None));
            }

            [Test]
            public void IsNone_returns_true()
            {
                Assert.That(Value.IsNone, Is.True);
            }

            [Test]
            public void IsBoolean_returns_false()
            {
                Assert.That(Value.IsBoolean, Is.False);
            }

            [Test]
            public void IsTrue_returns_false()
            {
                Assert.That(Value.IsTrue, Is.False);
            }

            [Test]
            public void IsFalse_returns_false()
            {
                Assert.That(Value.IsFalse, Is.False);
            }

            [Test]
            public void IsInteger_returns_false()
            {
                Assert.That(Value.IsInteger, Is.False);
            }

            [Test]
            public void IsString_returns_false()
            {
                Assert.That(Value.IsString, Is.False);
            }

            [Test]
            public void IsStringList_returns_false()
            {
                Assert.That(Value.IsStringList, Is.False);
            }

            [Test]
            public void ToString_returns_empty()
            {
                Assert.That(Value.ToString(), Is.Empty);
            }

            [Test]
            public void Box_returns_null()
            {
                Assert.That(Value.Box, Is.Null);
            }

            [Test]
            public void TryAsBoolean_returns_false()
            {
                Assert.That(Value.TryAsBoolean(out _), Is.False);
            }

            [Test]
            public void TryAsInteger_returns_false()
            {
                Assert.That(Value.TryAsInteger(out _), Is.False);
            }

            [Test]
            public void TryAsString_returns_false()
            {
                Assert.That(Value.TryAsString(out _), Is.False);
            }

            [Test]
            public void TryAsStringList_returns_false()
            {
                Assert.That(Value.TryAsStringList(out _), Is.False);
            }

            [Test]
            public void Boolean_cast_throws()
            {
                Assert.Throws<InvalidCastException>(() => _ = (bool)Value);
            }

            [Test]
            public void Integer_cast_throws()
            {
                Assert.Throws<InvalidCastException>(() => _ = (int)Value);
            }

            [Test]
            public void String_cast_throws()
            {
                Assert.Throws<InvalidCastException>(() => _ = (string)Value);
            }

            [Test]
            public void StringList_cast_throws()
            {
                Assert.Throws<InvalidCastException>(() => _ = (DocoptNet.StringList)Value);
            }
        }

        [TestFixture]
        public class True
        {
            static Value Value => true;

            [Test]
            public void Kind_returns_Boolean()
            {
                Assert.That(Value.Kind, Is.EqualTo(ValueKind.Boolean));
            }

            [Test]
            public void IsNone_returns_true()
            {
                Assert.That(Value.IsNone, Is.False);
            }

            [Test]
            public void IsBoolean_returns_true()
            {
                Assert.That(Value.IsBoolean, Is.True);
            }

            [Test]
            public void IsTrue_returns_true()
            {
                Assert.That(Value.IsTrue, Is.True);
            }

            [Test]
            public void IsFalse_returns_false()
            {
                Assert.That(Value.IsFalse, Is.False);
            }

            [Test]
            public void IsInteger_returns_false()
            {
                Assert.That(Value.IsInteger, Is.False);
            }

            [Test]
            public void IsString_returns_false()
            {
                Assert.That(Value.IsString, Is.False);
            }

            [Test]
            public void IsStringList_returns_false()
            {
                Assert.That(Value.IsStringList, Is.False);
            }

            [Test]
            public void ToString_returns_string_representation()
            {
                Assert.That(Value.ToString(), Is.EqualTo("True"));
            }

            [Test]
            public void Box_returns_true()
            {
                Assert.That(Value.Box, Is.True);
            }

            [Test]
            public void TryAsBoolean_returns_true_with_output_true()
            {
                var result = Value.TryAsBoolean(out var value);
                Assert.That(result, Is.True);
                Assert.That(value, Is.True);
            }

            [Test]
            public void TryAsInteger_returns_false()
            {
                Assert.That(Value.TryAsInteger(out _), Is.False);
            }

            [Test]
            public void TryAsString_returns_false()
            {
                Assert.That(Value.TryAsString(out _), Is.False);
            }

            [Test]
            public void TryAsStringList_returns_false()
            {
                Assert.That(Value.TryAsStringList(out _), Is.False);
            }

            [Test]
            public void Boolean_cast_returns_true()
            {
                Assert.That((bool)Value, Is.True);
            }

            [Test]
            public void Integer_cast_throws()
            {
                Assert.Throws<InvalidCastException>(() => _ = (int)Value);
            }

            [Test]
            public void String_cast_throws()
            {
                Assert.Throws<InvalidCastException>(() => _ = (string)Value);
            }

            [Test]
            public void StringList_cast_throws()
            {
                Assert.Throws<InvalidCastException>(() => _ = (DocoptNet.StringList)Value);
            }
        }

        [TestFixture]
        public class False
        {
            static Value Value => false;

            [Test]
            public void Kind_returns_Boolean()
            {
                Assert.That(Value.Kind, Is.EqualTo(ValueKind.Boolean));
            }

            [Test]
            public void IsNone_returns_true()
            {
                Assert.That(Value.IsNone, Is.False);
            }

            [Test]
            public void IsBoolean_returns_true()
            {
                Assert.That(Value.IsBoolean, Is.True);
            }

            [Test]
            public void IsTrue_returns_false()
            {
                Assert.That(Value.IsTrue, Is.False);
            }

            [Test]
            public void IsFalse_returns_true()
            {
                Assert.That(Value.IsFalse, Is.True);
            }

            [Test]
            public void IsInteger_returns_false()
            {
                Assert.That(Value.IsInteger, Is.False);
            }

            [Test]
            public void IsString_returns_false()
            {
                Assert.That(Value.IsString, Is.False);
            }

            [Test]
            public void IsStringList_returns_false()
            {
                Assert.That(Value.IsStringList, Is.False);
            }

            [Test]
            public void ToString_returns_string_representation()
            {
                Assert.That(Value.ToString(), Is.EqualTo("False"));
            }

            [Test]
            public void Box_returns_false()
            {
                Assert.That(Value.Box, Is.False);
            }

            [Test]
            public void TryAsBoolean_returns_true_with_output_false()
            {
                var result = Value.TryAsBoolean(out var value);
                Assert.That(result, Is.True);
                Assert.That(value, Is.False);
            }

            [Test]
            public void TryAsInteger_returns_false()
            {
                Assert.That(Value.TryAsInteger(out _), Is.False);
            }

            [Test]
            public void TryAsString_returns_false()
            {
                Assert.That(Value.TryAsString(out _), Is.False);
            }

            [Test]
            public void TryAsStringList_returns_false()
            {
                Assert.That(Value.TryAsStringList(out _), Is.False);
            }

            [Test]
            public void Boolean_cast_returns_false()
            {
                Assert.That((bool)Value, Is.False);
            }

            [Test]
            public void Integer_cast_throws()
            {
                Assert.Throws<InvalidCastException>(() => _ = (int)Value);
            }

            [Test]
            public void String_cast_throws()
            {
                Assert.Throws<InvalidCastException>(() => _ = (string)Value);
            }

            [Test]
            public void StringList_cast_throws()
            {
                Assert.Throws<InvalidCastException>(() => _ = (DocoptNet.StringList)Value);
            }
        }

        [TestFixture]
        public class Integer
        {
            const int UnderlyingValue = 42;
            static Value Value => UnderlyingValue;

            [Test]
            public void Kind_returns_Integer()
            {
                Assert.That(Value.Kind, Is.EqualTo(ValueKind.Integer));
            }

            [Test]
            public void IsNone_returns_true()
            {
                Assert.That(Value.IsNone, Is.False);
            }

            [Test]
            public void IsBoolean_returns_false()
            {
                Assert.That(Value.IsBoolean, Is.False);
            }

            [Test]
            public void IsTrue_returns_false()
            {
                Assert.That(Value.IsTrue, Is.False);
            }

            [Test]
            public void IsFalse_returns_false()
            {
                Assert.That(Value.IsFalse, Is.False);
            }

            [Test]
            public void IsInteger_returns_true()
            {
                Assert.That(Value.IsInteger, Is.True);
            }

            [Test]
            public void IsString_returns_false()
            {
                Assert.That(Value.IsString, Is.False);
            }

            [Test]
            public void IsStringList_returns_false()
            {
                Assert.That(Value.IsStringList, Is.False);
            }

            [Test]
            public void ToString_returns_string_representation()
            {
                Assert.That(Value.ToString(), Is.EqualTo(UnderlyingValue.ToString(CultureInfo.InvariantCulture)));
            }

            [Test]
            public void Box_returns_underlying_value()
            {
                Assert.That(Value.Box, Is.EqualTo(UnderlyingValue));
            }

            [Test]
            public void TryAsBoolean_returns_true_with_output_false()
            {
                Assert.That(Value.TryAsBoolean(out _), Is.False);
            }

            [Test]
            public void TryAsInteger_returns_true_with_output_as_underlying_value()
            {
                var result = Value.TryAsInteger(out var value);
                Assert.That(result, Is.True);
                Assert.That(value, Is.EqualTo(UnderlyingValue));
            }

            [Test]
            public void TryAsString_returns_false()
            {
                Assert.That(Value.TryAsString(out _), Is.False);
            }

            [Test]
            public void TryAsStringList_returns_false()
            {
                Assert.That(Value.TryAsStringList(out _), Is.False);
            }

            [Test]
            public void Boolean_cast_throws()
            {
                Assert.Throws<InvalidCastException>(() => _ = (bool)Value);
            }

            [Test]
            public void Integer_cast_returns_underlying_value()
            {
                Assert.That((int)Value, Is.EqualTo(UnderlyingValue));
            }

            [Test]
            public void String_cast_throws()
            {
                Assert.Throws<InvalidCastException>(() => _ = (string)Value);
            }

            [Test]
            public void StringList_cast_throws()
            {
                Assert.Throws<InvalidCastException>(() => _ = (DocoptNet.StringList)Value);
            }
        }

        [TestFixture]
        public class String
        {
            const string UnderlyingValue = "foobar";
            static Value Value => UnderlyingValue;

            [Test]
            public void Kind_returns_String()
            {
                Assert.That(Value.Kind, Is.EqualTo(ValueKind.String));
            }

            [Test]
            public void IsNone_returns_true()
            {
                Assert.That(Value.IsNone, Is.False);
            }

            [Test]
            public void IsBoolean_returns_false()
            {
                Assert.That(Value.IsBoolean, Is.False);
            }

            [Test]
            public void IsTrue_returns_false()
            {
                Assert.That(Value.IsTrue, Is.False);
            }

            [Test]
            public void IsFalse_returns_false()
            {
                Assert.That(Value.IsFalse, Is.False);
            }

            [Test]
            public void IsInteger_returns_false()
            {
                Assert.That(Value.IsInteger, Is.False);
            }

            [Test]
            public void IsString_returns_true()
            {
                Assert.That(Value.IsString, Is.True);
            }

            [Test]
            public void IsStringList_returns_false()
            {
                Assert.That(Value.IsStringList, Is.False);
            }

            [Test]
            public void ToString_returns_string_representation()
            {
                Assert.That(Value.ToString(), Is.EqualTo(UnderlyingValue));
            }

            [Test]
            public void Box_returns_underlying_value()
            {
                Assert.That(Value.Box, Is.EqualTo(UnderlyingValue));
            }

            [Test]
            public void TryAsBoolean_returns_true_with_output_false()
            {
                Assert.That(Value.TryAsBoolean(out _), Is.False);
            }

            [Test]
            public void TryAsInteger_returns_false()
            {
                Assert.That(Value.TryAsInteger(out _), Is.False);
            }

            [Test]
            public void TryAsString_returns_true_with_output_as_underlying_value()
            {
                var result = Value.TryAsString(out var value);
                Assert.That(result, Is.True);
                Assert.That(value, Is.EqualTo(UnderlyingValue));
            }

            [Test]
            public void TryAsStringList_returns_false()
            {
                Assert.That(Value.TryAsStringList(out _), Is.False);
            }

            [Test]
            public void Boolean_cast_throws()
            {
                Assert.Throws<InvalidCastException>(() => _ = (bool)Value);
            }

            [Test]
            public void Integer_cast_throws()
            {
                Assert.Throws<InvalidCastException>(() => _ = (int)Value);
            }

            [Test]
            public void String_cast_throws()
            {
                Assert.That((string)Value, Is.EqualTo(UnderlyingValue));
            }

            [Test]
            public void StringList_cast_throws()
            {
                Assert.Throws<InvalidCastException>(() => _ = (DocoptNet.StringList)Value);
            }
        }

        public class StringList
        {
            static readonly DocoptNet.StringList UnderlyingValue = DocoptNet.StringList.TopBottom("foo", "bar", "baz");

            static Value Value => UnderlyingValue;

            [Test]
            public void Kind_returns_String()
            {
                Assert.That(Value.Kind, Is.EqualTo(ValueKind.StringList));
            }

            [Test]
            public void IsNone_returns_true()
            {
                Assert.That(Value.IsNone, Is.False);
            }

            [Test]
            public void IsBoolean_returns_false()
            {
                Assert.That(Value.IsBoolean, Is.False);
            }

            [Test]
            public void IsTrue_returns_false()
            {
                Assert.That(Value.IsTrue, Is.False);
            }

            [Test]
            public void IsFalse_returns_false()
            {
                Assert.That(Value.IsFalse, Is.False);
            }

            [Test]
            public void IsInteger_returns_false()
            {
                Assert.That(Value.IsInteger, Is.False);
            }

            [Test]
            public void IsString_returns_false()
            {
                Assert.That(Value.IsString, Is.False);
            }

            [Test]
            public void IsStringList_returns_true()
            {
                Assert.That(Value.IsStringList, Is.True);
            }

            [Test]
            public void ToString_returns_string_representation()
            {
                Assert.That(Value.ToString(), Is.EqualTo("[foo, bar, baz]"));
            }

            [Test]
            public void Box_returns_underlying_value()
            {
                Assert.That(Value.Box, Is.EqualTo(UnderlyingValue));
            }

            [Test]
            public void TryAsBoolean_returns_true_with_output_false()
            {
                Assert.That(Value.TryAsBoolean(out _), Is.False);
            }

            [Test]
            public void TryAsInteger_returns_false()
            {
                Assert.That(Value.TryAsInteger(out _), Is.False);
            }

            [Test]
            public void TryAsString_returns_false()
            {
                Assert.That(Value.TryAsString(out _), Is.False);
            }

            [Test]
            public void TryAsStringList_returns_true_with_output_as_underlying_value()
            {
                var result = Value.TryAsStringList(out var value);
                Assert.That(result, Is.True);
                Assert.That(value, Is.SameAs(UnderlyingValue));
            }

            [Test]
            public void Boolean_cast_throws()
            {
                Assert.Throws<InvalidCastException>(() => _ = (bool)Value);
            }

            [Test]
            public void Integer_cast_throws()
            {
                Assert.Throws<InvalidCastException>(() => _ = (int)Value);
            }

            [Test]
            public void String_cast_throws()
            {
                Assert.Throws<InvalidCastException>(() => _ = (string)Value);
            }

            [Test]
            public void StringList_cast_returns_underlying_value()
            {
                Assert.That((DocoptNet.StringList)Value, Is.SameAs(UnderlyingValue));
            }
        }
    }
}
