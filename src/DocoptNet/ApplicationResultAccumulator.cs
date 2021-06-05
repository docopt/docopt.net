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

        sealed class DictionaryAccumulator : IApplicationResultAccumulator<IDictionary<string, object>>
        {
            public IDictionary<string, object> New() => new Dictionary<string, object>();
            public IDictionary<string, object> Command(IDictionary<string, object> state, string name, in Box<bool> value) => Adding(state, name, value.Object);
            public IDictionary<string, object> Command(IDictionary<string, object> state, string name, in Box<int> value) => Adding(state, name, value.Object);
            public IDictionary<string, object> Argument(IDictionary<string, object> state, string name) => Adding(state, name, null);
            public IDictionary<string, object> Argument(IDictionary<string, object> state, string name, in Box<string> value) => Adding(state, name, value.Object);
            public IDictionary<string, object> Argument(IDictionary<string, object> state, string name, in Box<ArrayList> value) => Adding(state, name, value.Object);
            public IDictionary<string, object> Option(IDictionary<string, object> state, string name) => Adding(state, name, null);
            public IDictionary<string, object> Option(IDictionary<string, object> state, string name, in Box<bool> value) => Adding(state, name, value.Object);
            public IDictionary<string, object> Option(IDictionary<string, object> state, string name, in Box<string> value) => Adding(state, name, value.Object);
            public IDictionary<string, object> Option(IDictionary<string, object> state, string name, in Box<int> value) => Adding(state, name, value.Object);
            public IDictionary<string, object> Option(IDictionary<string, object> state, string name, in Box<ArrayList> value) => Adding(state, name, value.Object);
            public IDictionary<string, object> Error(DocoptBaseException exception) => null;

            static IDictionary<string, object> Adding(IDictionary<string, object> dict, string name, object value)
            {
                dict[name] = value;
                return dict;
            }
        }

        sealed class ValueObjectDictionaryAccumulator : IApplicationResultAccumulator<IDictionary<string, ValueObject>>
        {
            public IDictionary<string, ValueObject> New() => new Dictionary<string, ValueObject>();
            public IDictionary<string, ValueObject> Command(IDictionary<string, ValueObject> state, string name, in Box<bool> value) => Adding(state, name, value.Object);
            public IDictionary<string, ValueObject> Command(IDictionary<string, ValueObject> state, string name, in Box<int> value) => Adding(state, name, value.Object);
            public IDictionary<string, ValueObject> Argument(IDictionary<string, ValueObject> state, string name) => Adding(state, name, null);
            public IDictionary<string, ValueObject> Argument(IDictionary<string, ValueObject> state, string name, in Box<string> value) => Adding(state, name, value.Object);
            public IDictionary<string, ValueObject> Argument(IDictionary<string, ValueObject> state, string name, in Box<ArrayList> value) => Adding(state, name, value.Object);
            public IDictionary<string, ValueObject> Option(IDictionary<string, ValueObject> state, string name) => Adding(state, name, null);
            public IDictionary<string, ValueObject> Option(IDictionary<string, ValueObject> state, string name, in Box<bool> value) => Adding(state, name, value.Object);
            public IDictionary<string, ValueObject> Option(IDictionary<string, ValueObject> state, string name, in Box<string> value) => Adding(state, name, value.Object);
            public IDictionary<string, ValueObject> Option(IDictionary<string, ValueObject> state, string name, in Box<int> value) => Adding(state, name, value.Object);
            public IDictionary<string, ValueObject> Option(IDictionary<string, ValueObject> state, string name, in Box<ArrayList> value) => Adding(state, name, value.Object);
            public IDictionary<string, ValueObject> Error(DocoptBaseException exception) => null;

            static IDictionary<string, ValueObject> Adding(IDictionary<string, ValueObject> dict, string name, object value)
            {
                dict[name] = new ValueObject(value);
                return dict;
            }
        }
    }
}
