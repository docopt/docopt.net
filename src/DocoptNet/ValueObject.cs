namespace DocoptNet
{
    using System;
    using System.Collections;
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
            return IsList ? $"[{string.Join(", ", AsList.Cast<object>())}]"
                 : (Value ?? string.Empty).ToString();
        }
    }
}
