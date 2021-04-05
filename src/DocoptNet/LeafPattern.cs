namespace DocoptNet
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Leaf/terminal node of a pattern tree.
    /// </summary>
    internal class LeafPattern: Pattern
    {
        private readonly string _name;

        protected LeafPattern(string name, ValueObject value=null)
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

        public ValueObject Value { get; set; }

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
            return string.Format("{0}({1}, {2})", GetType().Name, Name, Value);
        }
    }
}
