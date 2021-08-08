namespace DocoptNet
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    partial class ValueObject // TODO : IEquatable<ValueObject>
    {
        public object Value { get; private set; }

        internal ValueObject(object obj) =>
            Value = obj is ICollection collection ? new ArrayList(collection) : obj;

        internal ValueObject() => Value = null;

        public bool IsNullOrEmpty => Value == null || Value.ToString().Length == 0;
        public bool IsFalse => Value as bool? == false;
        public bool IsTrue => Value as bool? == true;
        public bool IsList => Value is ArrayList;
        internal bool IsOfTypeInt => Value is int;
        public bool IsInt => Value != null && (Value is int || int.TryParse(Value.ToString(), out _));
        public bool IsString => Value is string;
        public int AsInt => IsList ? 0 : Convert.ToInt32(Value);
        public ArrayList AsList => IsList ? Value as ArrayList : new ArrayList { Value };

        public override bool Equals(object obj)
        {
            return obj is ValueObject { Value: var v } other
                && (Value == null && v == null
                    || Value != null && v != null
                    // TODO avoid string allocations during equality check
                    && (IsList || other.IsList ? Value.ToString().Equals(v.ToString())
                                               : Value.Equals(v)));
        }

        public override int GetHashCode()
        {
            // TODO avoid string allocations when getting hash code
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return Format(this, Value, c => c.IsList ? c.AsList.Cast<object>() : null);
        }

        internal static string Format<TContainer, TValue, TItem>(TContainer container, TValue value,
                                                                 Func<TContainer, IEnumerable<TItem>> asList)
        {
            return value is null ? string.Empty
                 : asList(container) is { } items ? $"[{string.Join(", ", items)}]"
                 : value.ToString();
        }
    }
}
