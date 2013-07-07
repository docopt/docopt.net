using System.Collections.Generic;

namespace DocoptNet
{
    internal class Optional : BranchPattern
    {
        public Optional(params Pattern[] patterns) : base(patterns)
        {
            
        }

        public override MatchResult Match(IList<Pattern> left, IEnumerable<Pattern> collected = null)
        {
            var c = collected ?? new List<Pattern>();
            var l = left;
            foreach (var pattern in Children)
            {
                var res = pattern.Match(l, c);
                l = res.Left;
                c = res.Collected;
            }
            return new MatchResult(true, l, c);
        }
    }
}