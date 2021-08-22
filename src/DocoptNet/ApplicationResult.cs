#nullable enable

namespace DocoptNet
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    abstract class ApplicationResult
    {
        public abstract T Map<T>(Func<SuccessResult, T> successResult,
                                 Func<ErrorResult, T> errorSelector);

        public sealed class SuccessResult : ApplicationResult
        {
            readonly ReadOnlyList<LeafPattern> _collected;

            internal SuccessResult(ReadOnlyList<LeafPattern> collected) => _collected = collected;

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

            public override T Map<T>(Func<SuccessResult, T> successResult,
                                     Func<ErrorResult, T> errorSelector) =>
                successResult(this);
        }

        public sealed class ErrorResult : ApplicationResult
        {
            internal ErrorResult(string usage) => Usage = usage;

            public string Usage { get; }

            public override T Map<T>(Func<SuccessResult, T> successResult,
                                     Func<ErrorResult, T> errorSelector) =>
                errorSelector(this);
        }
    }

    static class ApplicationResultExtensions
    {
        internal static T Map<T>(this ApplicationResult result, Func<ApplicationResult.SuccessResult, T> selector)
        {
            if (result is null) throw new ArgumentNullException(nameof(result));

            return result.Map(selector, r => throw new DocoptInputErrorException(r.Usage));
        }

        internal static IDictionary<string, ValueObject> ToValueObjectDictionary(this ApplicationResult result)
        {
            if (result is null) throw new ArgumentNullException(nameof(result));

            return result.Map(r => r.Accumulate(new Dictionary<string, ValueObject>(),
                                                ApplicationResultAccumulators.ValueObjectDictionary));
        }

        internal static IDictionary<string, Value> ToValueDictionary(this ApplicationResult result)
        {
            if (result is null) throw new ArgumentNullException(nameof(result));

            return result.Map(sr => sr.Accumulate(new Dictionary<string, Value>(),
                                                  ApplicationResultAccumulators.ValueDictionary));
        }
    }
}
