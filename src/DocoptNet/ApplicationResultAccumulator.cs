// Licensed under terms of MIT license (see LICENSE-MIT)
// Copyright 2021 Atif Aziz, Dinh Doan Van Bien

namespace DocoptNet
{
    using System.Collections.Generic;

    interface IApplicationResultAccumulator<T, out TResult>
    {
        T Command(T state, string name, bool value);
        T Command(T state, string name, int value);
        T Argument(T state, string name);
        T Argument(T state, string name, string value);
        T Argument(T state, string name, StringList value);
        T Option(T state, string name);
        T Option(T state, string name, bool value);
        T Option(T state, string name, string value);
        T Option(T state, string name, int value);
        T Option(T state, string name, StringList value);
        TResult GetResult(T state);
    }

    interface IApplicationResultAccumulator<T> : IApplicationResultAccumulator<T, T>;

    static class ApplicationResultAccumulators
    {
        public static readonly IApplicationResultAccumulator<IDictionary<string, ArgValue>> ValueDictionary = new ValueDictionaryAccumulator();
        public static readonly IApplicationResultAccumulator<IDictionary<string, ValueObject>> ValueObjectDictionary = new ValueObjectDictionaryAccumulator();

        sealed class ValueDictionaryAccumulator : IApplicationResultAccumulator<IDictionary<string, ArgValue>>
        {
            public IDictionary<string, ArgValue> Command(IDictionary<string, ArgValue> state, string name, bool value) => Adding(state, name, value);
            public IDictionary<string, ArgValue> Command(IDictionary<string, ArgValue> state, string name, int value) => Adding(state, name, value);
            public IDictionary<string, ArgValue> Argument(IDictionary<string, ArgValue> state, string name) => Adding(state, name, ArgValue.None);
            public IDictionary<string, ArgValue> Argument(IDictionary<string, ArgValue> state, string name, string value) => Adding(state, name, value);
            public IDictionary<string, ArgValue> Argument(IDictionary<string, ArgValue> state, string name, StringList value) => Adding(state, name, value);
            public IDictionary<string, ArgValue> Option(IDictionary<string, ArgValue> state, string name) => Adding(state, name, ArgValue.None);
            public IDictionary<string, ArgValue> Option(IDictionary<string, ArgValue> state, string name, bool value) => Adding(state, name, value);
            public IDictionary<string, ArgValue> Option(IDictionary<string, ArgValue> state, string name, string value) => Adding(state, name, value);
            public IDictionary<string, ArgValue> Option(IDictionary<string, ArgValue> state, string name, int value) => Adding(state, name, value);
            public IDictionary<string, ArgValue> Option(IDictionary<string, ArgValue> state, string name, StringList value) => Adding(state, name, value);

            public IDictionary<string, ArgValue> GetResult(IDictionary<string, ArgValue> state) => state;

            static IDictionary<string, ArgValue> Adding(IDictionary<string, ArgValue> dict, string name, ArgValue value)
            {
                dict[name] = value;
                return dict;
            }
        }

        sealed class ValueObjectDictionaryAccumulator : IApplicationResultAccumulator<IDictionary<string, ValueObject>>
        {
            public IDictionary<string, ValueObject> Command(IDictionary<string, ValueObject> state, string name, bool value) => Adding(state, name, value);
            public IDictionary<string, ValueObject> Command(IDictionary<string, ValueObject> state, string name, int value) => Adding(state, name, value);
            public IDictionary<string, ValueObject> Argument(IDictionary<string, ValueObject> state, string name) => Adding(state, name, null);
            public IDictionary<string, ValueObject> Argument(IDictionary<string, ValueObject> state, string name, string value) => Adding(state, name, value);
            public IDictionary<string, ValueObject> Argument(IDictionary<string, ValueObject> state, string name, StringList value) => Adding(state, name, value);
            public IDictionary<string, ValueObject> Option(IDictionary<string, ValueObject> state, string name) => Adding(state, name, null);
            public IDictionary<string, ValueObject> Option(IDictionary<string, ValueObject> state, string name, bool value) => Adding(state, name, value);
            public IDictionary<string, ValueObject> Option(IDictionary<string, ValueObject> state, string name, string value) => Adding(state, name, value);
            public IDictionary<string, ValueObject> Option(IDictionary<string, ValueObject> state, string name, int value) => Adding(state, name, value);
            public IDictionary<string, ValueObject> Option(IDictionary<string, ValueObject> state, string name, StringList value) => Adding(state, name, value);

            public IDictionary<string, ValueObject> GetResult(IDictionary<string, ValueObject> state) => state;

            static IDictionary<string, ValueObject> Adding(IDictionary<string, ValueObject> dict, string name, object? value)
            {
                dict[name] = new ValueObject(value);
                return dict;
            }
        }
    }
}
