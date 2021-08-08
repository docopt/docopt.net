namespace DocoptNet
{
    using System.Collections.Generic;

    interface IApplicationResultAccumulator<T>
    {
        T New();
        T Command(T state, string name, in Value value);
        T Argument(T state, string name, in Value value);
        T Option(T state, string name, in Value value);
        T Error(DocoptBaseException exception);
    }

    static class ApplicationResultAccumulators
    {
        public static readonly IApplicationResultAccumulator<IDictionary<string, object>> ObjectDictionary = new DictionaryAccumulator();
        public static readonly IApplicationResultAccumulator<IDictionary<string, ValueObject>> ValueObjectDictionary = new ValueObjectDictionaryAccumulator();

        abstract class DictionaryAccumulator<T> : IApplicationResultAccumulator<IDictionary<string, T>>
        {
            public IDictionary<string, T> New() => new Dictionary<string, T>();
            public IDictionary<string, T> Command(IDictionary<string, T> state, string name, in Value value) => Adding(state, name, value.Box());
            public IDictionary<string, T> Argument(IDictionary<string, T> state, string name, in Value value) => Adding(state, name, value.Box());
            public IDictionary<string, T> Option(IDictionary<string, T> state, string name, in Value value) => Adding(state, name, value.Box());
            public IDictionary<string, T> Error(DocoptBaseException exception) => null;

            IDictionary<string, T> Adding(IDictionary<string, T> dict, string name, object value)
            {
                dict[name] = Convert(value);
                return dict;
            }

            protected abstract T Convert(object value);
        }

        sealed class ValueObjectDictionaryAccumulator : DictionaryAccumulator<ValueObject>
        {
            protected override ValueObject Convert(object value) => new(value);
        }

        sealed class DictionaryAccumulator : DictionaryAccumulator<object>
        {
            protected override object Convert(object value) => value;
        }
    }
}
