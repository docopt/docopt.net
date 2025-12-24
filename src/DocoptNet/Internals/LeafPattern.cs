// Licensed under terms of MIT license (see LICENSE-MIT)
// Copyright 2012 Vladimir Keleshev, 2013 Dinh Doan Van Bien, 2021 Atif Aziz

namespace DocoptNet.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Leaf/terminal node of a pattern tree.
    /// </summary>
    abstract partial class LeafPattern : Pattern
    {
#pragma warning disable IDE0290 // Use primary constructor (protected)
        protected LeafPattern(string name, ArgValue value = default)
#pragma warning restore IDE0290 // Use primary constructor
        {
            Name = name;
            Value = value;
        }

        public override string Name { get; }

        public ArgValue Value { get; set; }

        public override ICollection<Pattern> Flat(params Type[] types)
        {
            if (types == null) throw new ArgumentNullException(nameof(types));
            if (types.Length == 0 || types.Contains(GetType()))
            {
                return [this];
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
