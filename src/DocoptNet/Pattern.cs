using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DocoptNet
{
    internal abstract class Pattern
    {
        public ValueObject Value { get; set; }

        public virtual string Name
        {
            get { return ToString(); }
        }

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
            return ToString().GetHashCode();
        }

        public virtual bool HasChildren
        {
            get { return false; }
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
        /// <param name="uniq"></param>
        public void FixIdentities(ICollection<Pattern> uniq = null)
        {
            var listUniq = uniq ?? Flat().Distinct().ToList();
            for (int i = 0; i < Children.Count; i++)
            {
                var child = Children[i];
                if (!child.HasChildren)
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

                foreach (var e in l)
                {
                    if (e is Argument || (e is Option && (e as Option).ArgCount > 0))
                    {
                        if (e.Value == null)
                        {
                            e.Value = new ValueObject(new ArrayList());
                        }
                        else if (!e.Value.IsList)
                        {
                            e.Value =
                                new ValueObject(e.Value.ToString()
                                                 .Split(new char[0], StringSplitOptions.RemoveEmptyEntries));
                        }
                    }
                    if (e is Command || (e is Option && (e as Option).ArgCount == 0))
                    {
                        e.Value = new ValueObject(0);
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
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static Either Transform(Pattern pattern)
        {
            var result = new List<IList<Pattern>>();
            var groups = new List<IList<Pattern>> {new List<Pattern> {pattern}};
            while (groups.Count > 0)
            {
                var children = groups[0];
                groups.RemoveAt(0);
                var parents = new[]
                    {
                        typeof (Required), typeof (Optional), typeof (OptionsShortcut), typeof (Either), typeof (OneOrMore)
                    };
                if (parents.Any(t => children.Any(c => c.GetType() == t)))
                {
                    var child = children.First(c => parents.Contains(c.GetType()));
                    children.Remove(child);
                    if (child is Either)
                    {
                        foreach (var c in (child as Either).Children)
                        {
                            var l = new List<Pattern> {c};
                            l.AddRange(children);
                            groups.Add(l);
                        }
                    }
                    else if (child is OneOrMore)
                    {
                        var l = new List<Pattern>();
                        l.AddRange((child as OneOrMore).Children);
                        l.AddRange((child as OneOrMore).Children); // add twice
                        l.AddRange(children);
                        groups.Add(l);
                    }
                    else
                    {
                        var l = new List<Pattern>();
                        if (child.HasChildren)
                            l.AddRange(child.Children);
                        l.AddRange(children);
                        groups.Add(l);
                    }
                }
                else
                {
                    result.Add(children);
                }
            }
            return new Either(result.Select(r => new Required(r.ToArray()) as Pattern).ToArray());
        }

        public virtual MatchResult Match(IList<Pattern> left, IEnumerable<Pattern> collected = null)
        {
            return new MatchResult();
        }

        public abstract ICollection<Pattern> Flat(params Type[] types);

        /// <summary>
        ///     Flattens the current patterns to the leaves only
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Pattern> Flat()
        {
            return Flat(new Type[0]);
        }
    }
}