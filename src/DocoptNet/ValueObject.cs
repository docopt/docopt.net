using System;
using System.Collections;
using System.Linq;

namespace DocoptNet
{
    public class ValueObject // TODO : IEquatable<ValueObject>
    {
        public object Value { get; private set; }

        internal ValueObject(object obj)
        {
            if (obj is ArrayList)
            {
                Value = new ArrayList(obj as ArrayList);
                return;
            }
            if (obj is ICollection)
            {
                Value = new ArrayList(obj as ICollection);
                return;
            }
            Value = obj;
        }

        internal ValueObject()
        {
            Value = null;
        }

        public bool IsNullOrEmpty
        {
            get { return Value == null || Value.ToString() == ""; }
        }

        public bool IsFalse
        {
            get { return (Value as bool?) == false; }
        }

        public bool IsTrue
        {
            get { return (Value as bool?) == true; }
        }

        public bool IsList
        {
            get { return Value is ArrayList; }
        }

        internal bool IsOfTypeInt
        {
            get { return Value is int?; }
        }

        public bool IsInt
        {
            get
            {
                int value;
                return Value != null && (Value is int || Int32.TryParse(Value.ToString(), out value));
            }
        }

        public int AsInt
        {
            get { return IsList ? 0 : Convert.ToInt32(Value); }
        }

        public bool IsString
        {
            get { return Value is string; }
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
            if (IsList)
            {
                var l = (from object v in AsList select v.ToString()).ToList();
                return string.Format("[{0}]", String.Join(", ", l));
            }
            return (Value ?? "").ToString();
        }

        internal void Add(ValueObject increment)
        {
            if (increment == null) throw new ArgumentNullException(nameof(increment));

            if (increment.Value == null) throw new InvalidOperationException("increment.Value is null");

            if (Value == null) throw new InvalidOperationException("Value is null");

            if (increment.IsOfTypeInt)
            {
                if (IsList)
                    ((ArrayList)Value).Add(increment.AsInt);
                else
                    Value = increment.AsInt + AsInt;
            }
            else
            {
                var l = new ArrayList();
                if (IsList)
                {
                    l.AddRange(AsList);
                }
                else
                {
                    l.Add(Value);
                }
                if (increment.IsList)
                {
                    l.AddRange(increment.AsList);
                }
                else
                {
                    l.Add(increment);
                }
                Value = l;
            }
        }

        public ArrayList AsList
        {
            get { return IsList ? (Value as ArrayList) : (new ArrayList(new[] {Value})); }
        }
    }
}
