namespace DocoptNet
{
    using System.Collections.Generic;
    using System.Linq;

    internal class Either : BranchPattern
    {
        public Either(params Pattern[] patterns) : base(patterns)
        {
        }

        public override MatchResult Match(IList<LeafPattern> left,
                                          IEnumerable<LeafPattern> collected = null)
        {
            var coll = collected ?? new List<LeafPattern>();
            var outcomes =
                Children.Select(pattern => pattern.Match(left, coll))
                        .Where(outcome => outcome.Matched)
                        .ToList();
            if (outcomes.Count != 0)
            {
                var minCount = outcomes.Min(x => x.Left.Count);
                return outcomes.First(x => x.Left.Count == minCount);
            }
            return new MatchResult(false, left, coll);
        }
    }
}
