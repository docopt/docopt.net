
namespace DocoptNet.Generated
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Leaves = ReadOnlyList<LeafPattern>;

    abstract record Pattern;

    abstract record BranchPattern(ImmutableArray<Pattern> Children) : Pattern;
    sealed record Required(ImmutableArray<Pattern> Children) : BranchPattern(Children);
    sealed record Optional(ImmutableArray<Pattern> Children) : BranchPattern(Children);
    sealed record Either(ImmutableArray<Pattern> Children) : BranchPattern(Children);
    sealed record OneOrMore(Pattern Pattern) : BranchPattern(ImmutableArray.Create(Pattern));
    abstract record LeafPattern(string Name, ValueObject Value) : Pattern;

    sealed record Command(string Name) : LeafPattern(Name, new ValueObject(true));
    sealed record Argument(string Name, ValueObject Value) : LeafPattern(Name, Value);
    sealed record Option(string ShortName, string LongName, int ArgCount, ValueObject Value) : LeafPattern(LongName ?? ShortName, Value);

    sealed partial class ValueObject { }

    partial class ValueObject
    {
        public object Value { get; private set; }

        internal ValueObject(object obj) =>
            Value = obj is ICollection collection ? new ArrayList(collection) : obj;

        internal ValueObject() => Value = null;

        public bool IsNullOrEmpty => Value?.ToString()?.Length is null or 0;
        public bool IsFalse => Value as bool? == false;
        public bool IsTrue => Value as bool? == true;
        public bool IsList => Value is ArrayList;
        internal bool IsOfTypeInt => Value is int;
        public bool IsInt => Value != null && (Value is int || int.TryParse(Value.ToString(), out _));
        public int AsInt => IsList ? 0 : Convert.ToInt32(Value);
        public bool IsString => Value is string;
        public ArrayList AsList => IsList ? Value as ArrayList : new ArrayList { Value };

        internal void Add(ValueObject increment)
        {
            if (increment == null) throw new ArgumentNullException(nameof(increment));
            if (increment.Value == null) throw new ArgumentException(nameof(increment));
            if (Value == null) throw new InvalidOperationException();

            if (increment.IsOfTypeInt)
            {
                if (IsList)
                    ((ArrayList)Value).Add(increment.AsInt);
                else
                    Value = increment.AsInt + AsInt;
            }
            else
            {
                var list = new ArrayList();
                if (IsList)
                    list.AddRange(AsList);
                else
                    list.Add(Value);
                if (increment.IsList)
                    list.AddRange(increment.AsList);
                else
                    list.Add(increment);
                Value = list;
            }
        }
    }

    static class Module
    {
        public static (bool Matched, Leaves Left, Leaves Collected)
            Leaf(Leaves left, Leaves collected,
                 string name, object value, bool isList, bool isInt,
                 int index, LeafPattern match)
        {
            if (match == null)
            {
                return (false, left, collected);
            }
            var left_ = left.RemoveAt(index);
            var sameName = collected.Where(a => a.Name == name).ToList();
            if (value != null && (isList || isInt))
            {
                var increment = new ValueObject(1);
                if (!isInt)
                    increment = match.Value.IsString ? new ValueObject(new [] {match.Value})  : match.Value;
                if (sameName.Count == 0)
                    return (true, left_, collected.Append(match with { Value = increment }));
                sameName[0].Value.Add(increment);
                return (true, left_, collected);
            }
            return (true, left_, collected.Append(match));
        }

        public static (int, LeafPattern) Command(Leaves left, string command)
        {
            for (var i = 0; i < left.Count; i++)
            {
                if (left[i] is Argument { Value: { } value })
                {
                    if (value.ToString() == command)
                        return (i, new Command(command));
                    break;
                }
            }
            return default;
        }

        public static (int, LeafPattern) Argument(Leaves left, string name)
        {
            for (var i = 0; i < left.Count; i++)
            {
                if (left[i] is Argument { Value: var value })
                    return (i, new Argument(name, value));
            }
            return default;
        }

        public static (int, LeafPattern) Option(Leaves left, string name)
        {
            for (var i = 0; i < left.Count; i++)
            {
                if (left[i].Name == name)
                    return (i, left[i]);
            }
            return default;
        }
    }

    static class ReadOnlyList
    {
        public static ReadOnlyList<T> AsReadOnly<T>(this IList<T> list) => new(list);
    }

    readonly struct ReadOnlyList<T> : IReadOnlyList<T>
    {
        static readonly T[] EmptyArray = new T[0];

        readonly IList<T> _list;

        public ReadOnlyList(IList<T> list) => _list = list;
        IList<T> List => _list ?? EmptyArray;
        public int Count => List.Count;
        public T this[int index] => List[index];
        public IEnumerator<T> GetEnumerator() => List.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public ReadOnlyList<T> Append(T item)
        {
            var array = new T[Count + 1];
            CopyTo(array, 0, Count);
            array[Count] = item;
            return array.AsReadOnly();
        }

        public ReadOnlyList<T> RemoveAt(int index)
        {
            var array = new T[Count - 1];
            CopyTo(array, 0, index);
            CopyTo(array, index, index + 1, Count - index - 1);
            return array.AsReadOnly();
        }

        void CopyTo(T[] array, int index, int count) =>
            CopyTo(array, 0, index, count);

        void CopyTo(T[] array, int targetIndex, int sourceIndex, int count)
        {
            var list = List;
            while (count > 0)
            {
                array[targetIndex++] = list[sourceIndex++];
                count--;
            }
        }
    }
}
