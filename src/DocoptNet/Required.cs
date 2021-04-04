using System.Collections.Generic;

namespace DocoptNet
{
    internal class Required : BranchPattern
    {
        public Required(params Pattern[] patterns)
            : base(patterns)
        {
        }

        public override MatchResult Match(IList<LeafPattern> left,
                                          IEnumerable<LeafPattern> collected = null)
        {
            var coll = collected ?? new List<LeafPattern>();
            var l = left;
            var c = coll;
            foreach (var pattern in Children)
            {
                bool matched;
                (matched, l, c) = pattern.Match(l, c);
                if (!matched)
                    return new MatchResult(false, left, coll);
            }
            return new MatchResult(true, l, c);
        }
    }
}
