#nullable enable

namespace DocoptNet.Tests
{
    using System.Collections.Generic;
    using NUnit.Framework;

    [TestFixture]
    public class ApplicationResultAccumulator
    {
        public class ValueObjectDictionary
        {
            static readonly IApplicationResultAccumulator<IDictionary<string, ValueObject>> Accumulator = ApplicationResultAccumulators.ValueObjectDictionary;

            [Test]
            public void New_returns_empty_dictionary()
            {
                var dict = Accumulator.New();
                Assert.That(dict, Is.Empty);
            }

            [Test]
            public void Command_adds_entry_with_value_converted_to_object()
            {
                var dict = Accumulator.New();
                dict = Accumulator.Command(dict, "command", true);
                var value = dict["command"];
                Assert.That(value, Is.InstanceOf<ValueObject>());
                Assert.That(value.Value, Is.EqualTo(true));
            }

            [Test]
            public void Argument_adds_entry_with_value_converted_to_object()
            {
                var dict = Accumulator.New();
                dict = Accumulator.Argument(dict, "<argument>", "value");
                var value = dict["<argument>"];
                Assert.That(value, Is.InstanceOf<ValueObject>());
                Assert.That(value.Value, Is.EqualTo("value"));
            }

            [Test]
            public void Option_adds_entry_with_value_converted_to_object()
            {
                var dict = Accumulator.New();
                dict = Accumulator.Argument(dict, "--option", "value");
                var value = dict["--option"];
                Assert.That(value, Is.InstanceOf<ValueObject>());
                Assert.That(value.Value, Is.EqualTo("value"));
            }

            [Test]
            public void Error_returns_null()
            {
                Assert.That(Accumulator.Error(new DocoptInputErrorException()), Is.Null);
            }
        }

        [TestFixture]
        public class ObjectDictionary
        {
            static readonly IApplicationResultAccumulator<IDictionary<string, object>> Accumulator = ApplicationResultAccumulators.ObjectDictionary;

            [Test]
            public void New_returns_empty_dictionary()
            {
                var dict = Accumulator.New();
                Assert.That(dict, Is.Empty);
            }

            [Test]
            public void Command_adds_entry_with_value_converted_to_object()
            {
                var dict = Accumulator.New();
                dict = Accumulator.Command(dict, "command", true);
                var value = dict["command"];
                Assert.That(value, Is.InstanceOf<bool>());
                Assert.That(value, Is.EqualTo(true));
            }

            [Test]
            public void Argument_adds_entry_with_value_converted_to_object()
            {
                var dict = Accumulator.New();
                dict = Accumulator.Argument(dict, "<argument>", "value");
                var value = dict["<argument>"];
                Assert.That(value, Is.InstanceOf<string>());
                Assert.That(value, Is.EqualTo("value"));
            }

            [Test]
            public void Option_adds_entry_with_value_converted_to_object()
            {
                var dict = Accumulator.New();
                dict = Accumulator.Argument(dict, "--option", "value");
                var value = dict["--option"];
                Assert.That(value, Is.InstanceOf<string>());
                Assert.That(value, Is.EqualTo("value"));
            }

            [Test]
            public void Error_returns_null()
            {
                Assert.That(Accumulator.Error(new DocoptInputErrorException()), Is.Null);
            }
        }
    }
}
