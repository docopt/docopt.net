#nullable enable

namespace DocoptNet
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// A list of strings modeled after "cons" lists.
    /// </summary>

    [DebuggerDisplay("{" + nameof(DebugDisplay) + "(),nq}")]
    sealed class StringList : IEnumerable<string>, ICollection
    {
        public static readonly StringList Empty = new(default!, null, 0);

        readonly string _top;
        readonly StringList? _next;

        public static StringList BottomTop(params string[] items) =>
            items.Aggregate(Empty, (stack, item) => stack.Push(item));

        public static StringList TopBottom(params string[] items) =>
            items.Reverse().Aggregate(Empty, (stack, item) => stack.Push(item));

        StringList(string value, StringList next) : this(value, next, next.Count + 1) { }

        StringList(string value, StringList? next, int count) =>
            (_top, _next, Count) = (value, next, count);

        public bool IsEmpty => Count == 0;

        public int Count { get; }

        string DebugDisplay() => Count > 1 ? $"Count = {Count}, Top = {Peek()}" : "(empty)";

        public string Peek() => !IsEmpty ? _top : throw new InvalidOperationException();
        public StringList Pop() => _next ?? Empty;
        public StringList Push(string value) => new(value, this);
        public StringList Reverse() => this.Aggregate(Empty, (stack, item) => stack.Push(item));

        public List<string> ToList()
        {
            var list = new List<string>(Count);
            list.AddRange(this);
            return list;
        }

        public string[] ToArray()
        {
            var array = new string[Count];
            var i = 0;
            foreach (var item in this)
                array[i++] = item;
            return array;
        }

        public IEnumerator<string> GetEnumerator()
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
