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
            public void Command_adds_entry_with_value_converted_to_object()
            {
                IDictionary<string, ValueObject> dict = new Dictionary<string, ValueObject>();
                dict = Accumulator.Command(dict, "command", true);
                var value = dict["command"];
                Assert.That(value, Is.InstanceOf<ValueObject>());
                Assert.That(value.Value, Is.EqualTo(true));
            }

            [Test]
            public void Argument_adds_entry_with_value_converted_to_object()
            {
                IDictionary<string, ValueObject> dict = new Dictionary<string, ValueObject>();
                dict = Accumulator.Argument(dict, "<argument>", "value");
                var value = dict["<argument>"];
                Assert.That(value, Is.InstanceOf<ValueObject>());
                Assert.That(value.Value, Is.EqualTo("value"));
            }

            [Test]
            public void Option_adds_entry_with_value_converted_to_object()
            {
                IDictionary<string, ValueObject> dict = new Dictionary<string, ValueObject>();
                dict = Accumulator.Option(dict, "--option", "value");
                var value = dict["--option"];
                Assert.That(value, Is.InstanceOf<ValueObject>());
                Assert.That(value.Value, Is.EqualTo("value"));
            }

            [Test]
            public void GetResult_returns_same()
            {
                IDictionary<string, ValueObject> dict = new Dictionary<string, ValueObject>();
                Assert.That(Accumulator.GetResult(dict), Is.SameAs(dict));
            }
        }

        [TestFixture]
        public class ValueDictionary
        {
            static readonly IApplicationResultAccumulator<IDictionary<string, ArgValue>> Accumulator = ApplicationResultAccumulators.ValueDictionary;

            [Test]
            public void Command_adds_entry_with_value()
            {
                IDictionary<string, ArgValue> dict = new Dictionary<string, ArgValue>();
                dict = Accumulator.Command(dict, "command", true);
                var value = dict["command"];
                Assert.That(value, Is.InstanceOf<ArgValue>());
                Assert.That((bool)value, Is.EqualTo(true));
            }

            [Test]
            public void Argument_adds_entry_with_value()
            {
                IDictionary<string, ArgValue> dict = new Dictionary<string, ArgValue>();
                dict = Accumulator.Argument(dict, "<argument>", "value");
                var value = dict["<argument>"];
                Assert.That(value, Is.InstanceOf<ArgValue>());
                Assert.That((string)value, Is.EqualTo("value"));
            }

            [Test]
            public void Option_adds_entry_with_value()
            {
                IDictionary<string, ArgValue> dict = new Dictionary<string, ArgValue>();
                dict = Accumulator.Option(dict, "--option", "value");
                var value = dict["--option"];
                Assert.That(value, Is.InstanceOf<ArgValue>());
                Assert.That((string)value, Is.EqualTo("value"));
            }

            [Test]
            public void GetResult_returns_same()
            {
                IDictionary<string, ArgValue> dict = new Dictionary<string, ArgValue>();
                Assert.That(Accumulator.GetResult(dict), Is.SameAs(dict));
            }
        }
    }
}
