namespace DocoptNet
{
    using System.Collections.Generic;

    class Optional : BranchPattern
    {
        public Optional(params Pattern[] patterns) : base(patterns)
        {

        }

        public override MatchResult Match(IList<LeafPattern> left,
                                          IEnumerable<LeafPattern> collected = null)
        {
            var c = collected ?? new List<LeafPattern>();
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
