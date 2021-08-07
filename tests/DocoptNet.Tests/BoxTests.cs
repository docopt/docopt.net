namespace DocoptNet.Tests
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class BoxTests
    {
        public class General
        {
            [TestCase(42.0)]
            [TestCase("foobar")]
            public void Value_type_mismatch_throws(object value)
            {
                Assert.Throws<InvalidCastException>(() => Box<int>.General(value));
            }

            [Test]
            public void Reference_type_mismatch_throws()
            {
                Assert.Throws<InvalidCastException>(() => Box<string>.General(42));
            }
        }

        public class ValueProperty
        {
            [Test]
            public void Specific_value_type_returns_initial()
            {
                var specific = Box.Specific(42);
                int value = specific.Value;
                Assert.That(value, Is.EqualTo(42));
            }

            [Test]
            public void General_value_type_returns_initial()
            {
                var general = Box<int>.General(42);
                int value = general.Value;
                Assert.That(value, Is.EqualTo(42));
            }

            [Test]
            public void Default_value_type_throws()
            {
                var @default = default(Box<int>);
                Assert.Throws<NullReferenceException>(() => _ = @default.Value);
            }

            [Test]
            public void Specific_nullable_value_type_returns_initial()
            {
                var specific = Box.Specific((int?)42);
                int? value = specific.Value;
                Assert.That(value, Is.EqualTo(42));
            }

            [Test]
            public void General_nullable_value_type_returns_initial()
            {
                var general = Box<int?>.General(42);
                int? value = general.Value;
                Assert.That(value, Is.EqualTo(42));
            }

            [Test]
            public void Default_nullable_value_type_returns_null()
            {
                var @default = default(Box<int?>);
                int? value = @default.Value;
                Assert.That(value, Is.Null);
            }

            [Test]
            public void Specific_reference_type_returns_same_reference_to_initial()
            {
                var str = "foobar";
                var specific = Box<string>.Specific(str);
                string value1 = specific.Value;
                string value2 = specific.Value;
                Assert.That(value1, Is.SameAs(str));
                Assert.That(value2, Is.SameAs(str));
            }

            [Test]
            public void General_reference_type_returns_same_reference_to_initial()
            {
                var str = "foobar";
                var general = Box<string>.General(str);
                string value1 = general.Value;
                string value2 = general.Value;
                Assert.That(value1, Is.SameAs(str));
                Assert.That(value2, Is.SameAs(str));
            }

            [Test]
            public void Default_reference_type_returns_null()
            {
                var @default = default(Box<string>);
                string value = @default.Value;
                Assert.That(value, Is.Null);
            }
        }

        public class ObjectProperty
        {
            [Test]
            public void Specific_value_type_returns_unique_references_to_initial()
            {
                var specific = Box.Specific(42);
                var object1 = specific.Object;
                var object2 = specific.Object;
                Assert.That(object1, Is.EqualTo(42));
                Assert.That(object2, Is.EqualTo(object1));
                Assert.That(object2, Is.Not.SameAs(object1));
            }

            [Test]
            public void General_value_type_returns_same_references_to_initial()
            {
                var general = Box<int>.General(42);
                var object1 = general.Object;
                var object2 = general.Object;
                Assert.That(object1, Is.EqualTo(42));
                Assert.That(object2, Is.EqualTo(object1));
                Assert.That(object2, Is.SameAs(object1));
            }

            [Test]
            public void Default_value_type_returns_null()
            {
                var @default = default(Box<int>);
                Assert.That(@default.Object, Is.Null);
            }

            [Test]
            public void Specific_nullable_value_type_returns_unique_references_to_initial()
            {
                var specific = Box.Specific((int?)42);
                var object1 = specific.Object;
                var object2 = specific.Object;
                Assert.That(object1, Is.EqualTo(42));
                Assert.That(object2, Is.EqualTo(object1));
                Assert.That(object2, Is.Not.SameAs(object1));
            }

            [Test]
            public void General_nullable_value_type_returns_same_references_to_initial()
            {
                var general = Box<int?>.General(42);
                var object1 = general.Object;
                var object2 = general.Object;
                Assert.That(object1, Is.EqualTo(42));
                Assert.That(object2, Is.EqualTo(object1));
                Assert.That(object2, Is.SameAs(object1));
            }

            [Test]
            public void Default_nullable_value_type_returns_null()
            {
                var @default = default(Box<int?>);
                Assert.That(@default.Object, Is.Null);
            }

            [Test]
            public void Specific_null_nullable_value_type_returns_unique_references_to_initial()
            {
                var specific = Box.Specific((int?)null);
                Assert.That(specific.Object, Is.Null);
            }

            [Test]
            public void General_null_nullable_value_type_returns_same_references_to_initial()
            {
                var general = Box<int?>.General(null);
                Assert.That(general.Object, Is.Null);
            }

            [Test]
            public void Default_null_nullable_value_type_returns_null()
            {
                var @default = default(Box<int?>);
                Assert.That(@default.Object, Is.Null);
            }

            [Test]
            public void Specific_reference_type_returns_same_reference_to_initial()
            {
                var str = "foobar";
                var specific = Box<string>.Specific(str);
                var object1 = specific.Object;
                var object2 = specific.Object;
                Assert.That(object1, Is.SameAs(str));
                Assert.That(object2, Is.SameAs(str));
            }

            [Test]
            public void General_reference_type_returns_same_reference_to_initial()
            {
                var str = "foobar";
                var general = Box<string>.General(str);
                var object1 = general.Object;
                var object2 = general.Object;
                Assert.That(object1, Is.SameAs(str));
                Assert.That(object2, Is.SameAs(str));
            }

            [Test]
            public void Default_reference_type_returns_null()
            {
                var @default = default(Box<string>);
                Assert.That(@default.Object, Is.Null);
            }
        }

        public new class ToString
        {
            [Test]
            public void Specific_value_type_returns_string_representation()
            {
                var id = Guid.NewGuid();
                var specific = Box.Specific(id);
                Assert.That(specific.ToString(), Is.EqualTo(id.ToString()));
            }

            [Test]
            public void General_value_type_returns_string_representation()
            {
                var id = Guid.NewGuid();
                var general = Box<Guid>.General(id);
                Assert.That(general.ToString(), Is.EqualTo(id.ToString()));
            }

            [Test]
            public void Default_value_type_returns_empty_string()
            {
                var @default = default(Box<Guid>);
                Assert.That(@default.ToString(), Is.Empty);
            }
        }

        public class Generalize
        {
            [Test]
            public void General_value_type_returns_same_reference()
            {
                var general1 = Box<int>.General(42);
                var general2 = general1.Generalize();
                Assert.That(general2.Object, Is.SameAs(general1.Object));
            }

            [Test]
            public void Specific_value_type_returns_different_reference()
            {
                var specific = Box.Specific(42);
                var general = specific.Generalize();
                Assert.That(general.Object, Is.Not.SameAs(specific.Object));
            }

            [Test]
            public void Default_value_type_returns_different_reference()
            {
                var @default = default(Box<int>);
                var general = @default.Generalize();
                Assert.That(general.Object, Is.SameAs(@default.Object));
            }

            [Test]
            public void General_reference_type_returns_same_reference()
            {
                var general1 = Box<string>.General("foobar");
                var general2 = general1.Generalize();
                Assert.That(general2.Object, Is.SameAs(general1.Object));
            }

            [Test]
            public void Specific_reference_type_returns_same_reference()
            {
                var general1 = Box.Specific("foobar");
                var general2 = general1.Generalize();
                Assert.That(general2.Object, Is.SameAs(general1.Object));
            }

            [Test]
            public void Default_reference_type_returns_same_reference()
            {
                var general1 = default(Box<string>);
                var general2 = general1.Generalize();
                Assert.That(general2.Object, Is.SameAs(general1.Object));
            }
        }
    }
}
