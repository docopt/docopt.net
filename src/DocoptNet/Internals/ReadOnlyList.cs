// Licensed under terms of MIT license (see LICENSE-MIT)
// Copyright 2021 Atif Aziz, Dinh Doan Van Bien

namespace DocoptNet.Internals
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    static class ReadOnlyList
    {
        public static ReadOnlyList<T> ToReadOnlyList<T>(this IEnumerable<T> sequence) => AsReadOnly(sequence.ToList());
        public static ReadOnlyList<T> AsReadOnly<T>(this IList<T> list) => new(list);
    }

    /// <summary>
    /// A read-only wrapper for <seealso cref="IList{T}"/>.
    /// </summary>
    /// <remarks>
    /// It is the responsibility of the initializer to guarantee that the wrapped and possibly
    /// read-write <seealso cref="IList{T}"/> is not changed.
    /// </remarks>

    readonly partial struct ReadOnlyList<T> : IReadOnlyList<T>
    {
        readonly IList<T> _list;

        public ReadOnlyList(IList<T> list) => _list = list;
        IList<T> List => _list ?? Array.Empty<T>();
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
