#nullable disable

namespace DocoptNet.Generated
{
    using System;
    using System.Collections;
    using System.Linq;

    partial class ValueObject // TODO : IEquatable<ValueObject>
    {
        public object Value { get; }

        internal ValueObject(object obj) =>
            Value = obj is ICollection collection ? new ArrayList(collection) : obj;

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
            return Format(IsList ? AsList : Value);
        }

        internal static string Format(object value)
        {
            return value switch
            {
                null => string.Empty,
                string s => s,
                IEnumerable items => $"[{string.Join(", ", items.Cast<object>())}]",
                _ => value.ToString()
            };
        }
    }
}
