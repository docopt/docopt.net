namespace DocoptNet.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Leaf/terminal node of a pattern tree.
    /// </summary>
    abstract partial class LeafPattern: Pattern
    {
        private readonly string _name;

        protected LeafPattern(string name, ArgValue value = default)
        {
            _name = name;
            Value = value;
        }

        protected LeafPattern() { }

        public override string Name
        {
            get { return _name; }
        }

        public ArgValue Value { get; set; }

        public override ICollection<Pattern> Flat(params Type[] types)
        {
            if (types == null) throw new ArgumentNullException(nameof(types));
            if (types.Length == 0 || types.Contains(GetType()))
            {
                return new Pattern[] { this };
            }
            return Array.Empty<Pattern>();
        }

        public override string ToString()
        {
            var value = Value.TryAsStringList(out var list) ? list.Reverse() : Value;
            return $"{GetType().Name}({Name}, {ValueObject.Format(value)})";
        }
    }
}
