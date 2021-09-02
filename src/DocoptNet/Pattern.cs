namespace DocoptNet
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    abstract class Pattern
    {
        public abstract string Name { get; }

        public virtual string GenerateCode()
        {
            return "// No code for " + Name;
        }

        public virtual Node ToNode()
        {
            return null;
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            //
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return ToString() == obj.ToString();
        }

// override object.GetHashCode
        public override int GetHashCode()
        {
            return ToString()!.GetHashCode();
        }

        public IList<Pattern> Children { get; set; }

        public Pattern Fix()
        {
            FixIdentities();
            FixRepeatingArguments();
            return this;
        }

        /// <summary>
        ///     Make pattern-tree tips point to same object if they are equal.
        /// </summary>
        public void FixIdentities(ICollection<Pattern> uniq = null)
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
                    child.FixIdentities(listUniq);
                }
            }
        }

        public Pattern FixRepeatingArguments()
        {
            var transform = Transform(this);
            var either = transform.Children.Select(c => c.Children);
            foreach (var aCase in either)
            {
                var cx = aCase.ToList();
                var l = aCase.Where(e => cx.Count(c2 => c2.Equals(e)) > 1).ToList();

                foreach (var e in l.OfType<LeafPattern>())
                {
                    if (e is Argument || e is Option { ArgCount: > 0 })
                    {
                        if (e.Value.IsNone)
                        {
                            e.Value = StringList.Empty;
                        }
                        else if (!e.Value.IsStringList)
                        {
                            e.Value = StringList.BottomTop(e.Value.ToString().Split(new char[0], StringSplitOptions.RemoveEmptyEntries));
                        }
                    }
                    if (e is Command || e is Option { ArgCount: 0 })
                    {
                        e.Value = 0;
                    }
                }
            }
            return this;
        }

        /// <summary>
        ///     Expand pattern into an (almost) equivalent one, but with single Either.
        ///     Example: ((-a | -b) (-c | -d)) => (-a -c | -a -d | -b -c | -b -d)
        ///     Quirks: [-a] => (-a), (-a...) => (-a -a)
        /// </summary>
        public static Either Transform(Pattern pattern)
        {
            var result = new List<List<Pattern>>();
            var groups = new List<List<Pattern>> { new() { pattern } };
            while (groups.Count > 0)
            {
                var children = groups[0];
                groups.RemoveAt(0);
                if (children.FirstOrDefault(c => c is BranchPattern) is { } branch)
                {
                    children.Remove(branch);
                    switch (branch)
                    {
                        case Either either:
                        {
                            foreach (var c in either.Children)
                            {
                                var l = new List<Pattern> { c };
                                l.AddRange(children);
                                groups.Add(l);
                            }
                            break;
                        }
                        case OneOrMore oneOrMore:
                        {
                            var l = new List<Pattern>();
                            l.AddRange(oneOrMore.Children);
                            l.AddRange(oneOrMore.Children); // add twice
                            l.AddRange(children);
                            groups.Add(l);
                            break;
                        }
                        default:
                        {
                            var l = new List<Pattern>();
                            l.AddRange(branch.Children);
                            l.AddRange(children);
                            groups.Add(l);
                            break;
                        }
                    }
                }
                else
                {
                    result.Add(children);
                }
            }
            return new Either(result.Select(r => new Required(r.ToArray()) as Pattern).ToArray());
        }

        public abstract ICollection<Pattern> Flat(params Type[] types);

        /// <summary>
        ///     Flattens the current patterns to the leaves only
        /// </summary>
        public IEnumerable<Pattern> Flat()
        {
            return Flat(new Type[0]);
        }
    }
}
