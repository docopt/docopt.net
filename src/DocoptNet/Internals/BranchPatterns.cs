// Licensed under terms of MIT license (see LICENSE-MIT)
// Copyright 2012 Vladimir Keleshev, 2013 Dinh Doan Van Bien, 2021 Atif Aziz

namespace DocoptNet.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    ///     Branch/inner node of a pattern tree.
    /// </summary>
    abstract partial class BranchPattern : Pattern
    {
        protected BranchPattern(params Pattern[] children)
        {
            if (children == null) throw new ArgumentNullException(nameof(children));
            Children = children;
        }

        public override string Name
        {
            get { return ToString(); }
        }

        public IList<Pattern> Children { get; set; }

        public IEnumerable<Pattern> Flat<T>() where T: Pattern
        {
            return Flat(typeof (T));
        }

        public override ICollection<Pattern> Flat(params Type[] types)
        {
            if (types == null) throw new ArgumentNullException(nameof(types));
            if (types.Contains(GetType()))
            {
                return [this];
            }
            return Children.SelectMany(child => child.Flat(types)).ToList();
        }

        public override string ToString()
        {
            return $"{GetType().Name}({string.Join(", ", Children.Select(c => c == null ? "None" : c.ToString()))})";
        }

        public Pattern Fix()
        {
            FixIdentities();
            FixRepeatingArguments();
            return this;
        }

        /// <summary>
        ///     Make pattern-tree tips point to same object if they are equal.
        /// </summary>
        public void FixIdentities(ICollection<Pattern>? uniq = null)
        {
            var listUniq = uniq ?? Flat().Distinct().ToList();
            for (var i = 0; i < Children.Count; i++)
            {
                var child = Children[i];
                if (child is LeafPattern)
                {
                    Debug.Assert(listUniq.Contains(child));
                    Children[i] = listUniq.First(p => p.Equals(child));
                }
                else
                {
                    ((BranchPattern)child).FixIdentities(listUniq);
                }
            }
        }

        public Pattern FixRepeatingArguments()
        {
            var transform = Transform(this);
            var either = transform.Children.OfType<BranchPattern>().Select(c => c.Children);
            foreach (var aCase in either)
            {
                var cx = aCase.ToList();
                var l = aCase.Where(e => cx.Count(c2 => c2.Equals(e)) > 1).ToList();

                foreach (var e in l.OfType<LeafPattern>())
                {
                    if (e is Argument or Option { ArgCount: > 0 })
                    {
                        if (e.Value.IsNone)
                        {
                            e.Value = StringList.Empty;
                        }
                        else if (!e.Value.IsStringList)
                        {
                            e.Value = StringList.BottomTop(e.Value.ToString().Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
                        }
                    }
                    if (e is Command or Option { ArgCount: 0 })
                    {
                        e.Value = 0;
                    }
                }
            }
            return this;
        }
    }

    class Required : BranchPattern
    {
        public Required(params Pattern[] patterns) : base(patterns) { }
    }

    class Optional : BranchPattern
    {
        public Optional(params Pattern[] patterns) : base(patterns) { }
    }

    // Marker/placeholder for [options] shortcut.
    class OptionsShortcut : Optional
    {
        // TODO consider single pattern
        public OptionsShortcut(params Pattern[] patterns) : base(patterns) { }
    }

    partial class Either : BranchPattern
    {
        public Either(params Pattern[] patterns) : base(patterns) { }
    }

    class OneOrMore : BranchPattern
    {
        // TODO consider single pattern
        public OneOrMore(params Pattern[] patterns) : base(patterns) { }
    }
}
