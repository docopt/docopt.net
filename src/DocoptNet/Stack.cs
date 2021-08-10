#nullable enable

namespace DocoptNet
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    static class Stack
    {
        public static Stack<T> BottomTop<T>(params T[] items) =>
            items.Aggregate(Stack<T>.Empty, (stack, item) => stack.Push(item));

        public static Stack<T> TopBottom<T>(params T[] items) =>
            items.Reverse().Aggregate(Stack<T>.Empty, (stack, item) => stack.Push(item));
    }

    [DebuggerDisplay("{" + nameof(DebugDisplay) + "(),nq}")]
    sealed class Stack<T> : IEnumerable<T>, ICollection
    {
        public static readonly Stack<T> Empty = new(default!, null, 0);

        readonly T _top;
        readonly Stack<T>? _next;

        Stack(T value, Stack<T> next) : this(value, next, next.Count + 1) { }

        Stack(T value, Stack<T>? next, int count) =>
            (_top, _next, Count) = (value, next, count);

        public bool IsEmpty => Count == 0;

        public int Count { get; }

        string DebugDisplay() => Count > 1 ? $"Count = {Count}, Top = {Peek()}" : "(empty)";

        public T Peek() => !IsEmpty ? _top : throw new InvalidOperationException();
        public Stack<T> Pop() => _next ?? Empty;
        public Stack<T> Push(T value) => new(value, this);
        public Stack<T> Reverse() => this.Aggregate(Empty, (stack, item) => stack.Push(item));

        public List<T> ToList()
        {
            var list = new List<T>(Count);
            list.AddRange(this);
            return list;
        }

        public T[] ToArray()
        {
            var array = new T[Count];
            var i = 0;
            foreach (var item in this)
                array[i++] = item;
            return array;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (var s = this; !s.IsEmpty; s = s.Pop())
                yield return s._top;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        void ICollection.CopyTo(Array array, int index)
        {
            if (array is null) throw new ArgumentNullException(nameof(array));
            if (array.Rank != 1) throw new ArgumentException(null, nameof(array));
            if (index + Count > array.Length) throw new ArgumentOutOfRangeException(nameof(index), index, null);

            foreach (var item in this)
                array.SetValue(item, index++);
        }

        bool ICollection.IsSynchronized => false;
        object ICollection.SyncRoot => this;
    }
}
