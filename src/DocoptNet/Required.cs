using System.Collections.Generic;

namespace DocoptNet
{
    internal class Required : BranchPattern
    {
        public Required(params Pattern[] patterns)
            : base(patterns)
        {
        }

        public override MatchResult Match(IList<Pattern> left,
                                          IEnumerable<Pattern> collected = null)
        {
            var coll = collected ?? new List<Pattern>();
            var l = left;
            var c = coll;
            foreach (var pattern in Children)
            {
                var res = pattern.Match(l, c);
                l = res.Left;
                c = res.Collected;
                if (!res.Matched)
                    return new MatchResult(false, left, coll);
            }
            return new MatchResult(true, l, c);
        }
    }
}