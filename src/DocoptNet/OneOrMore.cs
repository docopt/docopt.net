using System.Collections.Generic;
using System.Diagnostics;

namespace DocoptNet
{
    internal class OneOrMore : BranchPattern
    {
        public OneOrMore(params Pattern[] patterns)
            : base(patterns)
        {
        }

        public override MatchResult Match(IList<Pattern> left, IEnumerable<Pattern> collected = null)
        {
            Debug.Assert(Children.Count == 1);
            var coll = collected ?? new List<Pattern>();
            var l = left;
            var c = coll;
            IList<Pattern> l_ = null;
            var matched = true;
            var times = 0;
            while (matched)
            {
                // could it be that something didn't match but changed l or c?
                var res = Children[0].Match(l, c);
                matched = res.Matched;
                l = res.Left;
                c = res.Collected;
                times += matched ? 1 : 0;
                if (l_ != null && l_.Equals(l))
                    break;
                l_ = l;
            }
            if (times >= 1)
            {
                return new MatchResult(true, l, c);
            }
            return new MatchResult(false, left, coll);
        }
    }
}