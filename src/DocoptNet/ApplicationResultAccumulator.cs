namespace DocoptNet
{
    using System.Collections;
    using System.Collections.Generic;

    interface IApplicationResultAccumulator<T>
    {
        T New();
        T Command(T state, string name, in Box<bool> value);
        T Command(T state, string name, in Box<int> value);
        T Argument(T state, string name);
        T Argument(T state, string name, in Box<string> value);
        T Argument(T state, string name, in Box<ArrayList> value);
        T Option(T state, string name);
        T Option(T state, string name, in Box<bool> value);
        T Option(T state, string name, in Box<string> value);
        T Option(T state, string name, in Box<int> value);
        T Option(T state, string name, in Box<ArrayList> value);
        T Error(DocoptBaseException exception);
    }

    static class StockApplicationResultAccumulators
    {
        public static readonly IApplicationResultAccumulator<IDictionary<string, object>> ObjectDictionary = new DictionaryAccumulator();
        public static readonly IApplicationResultAccumulator<IDictionary<string, ValueObject>> ValueObjectDictionary = new ValueObjectDictionaryAccumulator();

        abstract class DictionaryAccumulator<T> : IApplicationResultAccumulator<IDictionary<string, T>>
        {
            public IDictionary<string, T> New() => new Dictionary<string, T>();
            public IDictionary<string, T> Command(IDictionary<string, T> state, string name, in Box<bool> value) => Adding(state, name, value.Object);
            public IDictionary<string, T> Command(IDictionary<string, T> state, string name, in Box<int> value) => Adding(state, name, value.Object);
            public IDictionary<string, T> Argument(IDictionary<string, T> state, string name) => Adding(state, name, null);
            public IDictionary<string, T> Argument(IDictionary<string, T> state, string name, in Box<string> value) => Adding(state, name, value.Object);
            public IDictionary<string, T> Argument(IDictionary<string, T> state, string name, in Box<ArrayList> value) => Adding(state, name, value.Object);
            public IDictionary<string, T> Option(IDictionary<string, T> state, string name) => Adding(state, name, null);
            public IDictionary<string, T> Option(IDictionary<string, T> state, string name, in Box<bool> value) => Adding(state, name, value.Object);
            public IDictionary<string, T> Option(IDictionary<string, T> state, string name, in Box<string> value) => Adding(state, name, value.Object);
            public IDictionary<string, T> Option(IDictionary<string, T> state, string name, in Box<int> value) => Adding(state, name, value.Object);
            public IDictionary<string, T> Option(IDictionary<string, T> state, string name, in Box<ArrayList> value) => Adding(state, name, value.Object);
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
