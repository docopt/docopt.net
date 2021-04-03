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
                bool matched;
                (matched, l, c) = pattern.Match(l, c);
                if (!matched)
                    return new MatchResult(false, left, coll);
            }
            return new MatchResult(true, l, c);
        }
    }
}
