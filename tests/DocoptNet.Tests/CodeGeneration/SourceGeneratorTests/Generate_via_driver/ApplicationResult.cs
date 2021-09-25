#nullable enable

namespace DocoptNet.Generated
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    sealed class ApplicationResult
    {
        readonly ReadOnlyList<LeafPattern> _collected;

        internal ApplicationResult(ReadOnlyList<LeafPattern> collected) => _collected = collected;

        public TResult Accumulate<TState, TResult>(TState initialState,
                                                   IApplicationResultAccumulator<TState, TResult> accumulator) =>
            _collected.Aggregate(initialState, (state, p) => (p, p.Value.Object) switch
                                 {
                                     (Command , bool v       ) => accumulator.Command(state, p.Name, v),
                                     (Command , int v        ) => accumulator.Command(state, p.Name, v),
                                     (Argument, null         ) => accumulator.Argument(state, p.Name),
                                     (Argument, string v     ) => accumulator.Argument(state, p.Name, v),
                                     (Argument, StringList v ) => accumulator.Argument(state, p.Name, v.Reverse()),
                                     (Option  , bool v       ) => accumulator.Option(state, p.Name, v),
                                     (Option  , int v        ) => accumulator.Option(state, p.Name, v),
                                     (Option  , string v     ) => accumulator.Option(state, p.Name, v),
                                     (Option  , null         ) => accumulator.Option(state, p.Name),
                                     (Option  , StringList v ) => accumulator.Option(state, p.Name, v.Reverse()),
                                     var other => throw new NotSupportedException($"Unsupported pattern: {other}"),
                                 },
                                 accumulator.GetResult);
    }

    static class ApplicationResultExtensions
    {
        internal static IDictionary<string, ValueObject> ToValueObjectDictionary(this ApplicationResult result)
        {
            if (result is null) throw new ArgumentNullException(nameof(result));

            return result.Accumulate(new Dictionary<string, ValueObject>(),
                                     ApplicationResultAccumulators.ValueObjectDictionary);
        }

        internal static IDictionary<string, Value> ToValueDictionary(this ApplicationResult result)
        {
            if (result is null) throw new ArgumentNullException(nameof(result));

            return result.Accumulate(new Dictionary<string, Value>(),
                                     ApplicationResultAccumulators.ValueDictionary);
        }
    }
}
