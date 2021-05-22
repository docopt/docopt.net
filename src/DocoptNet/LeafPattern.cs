namespace DocoptNet
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Leaf/terminal node of a pattern tree.
    /// </summary>
    internal abstract class LeafPattern: Pattern
    {
        private readonly string _name;

        protected LeafPattern(string name, object value = null)
        {
            _name = name;
            Value = value;
        }

        protected LeafPattern()
        {
        }

        public override string Name
        {
            get { return _name; }
        }

        public object Value { get;  set; }

        internal void Add(object increment)
        {
            if (increment == null) throw new ArgumentNullException(nameof(increment));
            if (Value == null) throw new InvalidOperationException();

            if (increment is int n)
            {
                if (Value is IList list)
                    list.Add(n);
                else
                    Value = n + (Value is ICollection ? 0 : Convert.ToInt32(Value));
            }
            else
            {
                var newList = new ArrayList();
                if (Value is ICollection collection)
                    newList.AddRange(collection);
                else
                    newList.Add(Value);
                if (increment is ArrayList list)
                    newList.AddRange(list);
                else
                    newList.Add(increment);
                Value = newList;
            }
        }

        public override ICollection<Pattern> Flat(params Type[] types)
        {
            if (types == null) throw new ArgumentNullException(nameof(types));
            if (types.Length == 0 || types.Contains(this.GetType()))
            {
                return new Pattern[] { this };
            }
            return new Pattern[] {};
        }

        public override string ToString()
        {
            return $"{GetType().Name}({Name}, {Value})";
        }
    }
}
