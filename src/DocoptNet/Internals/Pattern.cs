// Licensed under terms of MIT license (see LICENSE-MIT)
// Copyright 2012 Vladimir Keleshev, 2013 Dinh Doan Van Bien, 2021 Atif Aziz

namespace DocoptNet.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    abstract partial class Pattern
    {
        public abstract string Name { get; }

        // override object.Equals
        public override bool Equals(object? obj)
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
                if (children.OfType<BranchPattern>().FirstOrDefault() is { } branch)
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
            return Flat([]);
        }
    }
}
