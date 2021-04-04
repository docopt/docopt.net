namespace DocoptNet
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///     Branch/inner node of a pattern tree.
    /// </summary>
    internal class BranchPattern : Pattern
    {

        public BranchPattern(params Pattern[] children)
        {
            if (children == null) throw new ArgumentNullException(nameof(children));
            Children = children;
        }

        public override bool HasChildren { get { return true; } }

        public IEnumerable<Pattern> Flat<T>() where T: Pattern
        {
            return Flat(typeof (T));
        }

        public override ICollection<Pattern> Flat(params Type[] types)
        {
            if (types == null) throw new ArgumentNullException(nameof(types));
            if (types.Contains(this.GetType()))
            {
                return new Pattern[] { this };
            }
            return Children.SelectMany(child => child.Flat(types)).ToList();
        }

        public override string ToString()
        {
            return string.Format("{0}({1})", GetType().Name, string.Join(", ", Children.Select(c => c == null ? "None" : c.ToString())));
        }
    }
}
