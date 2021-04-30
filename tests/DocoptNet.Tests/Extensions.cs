namespace DocoptNet.Tests
{
    static class Extensions
    {
        public static MatchResult Match(this Pattern pattern, params LeafPattern[] left)
        {
            return PatternMatcher.Match(pattern, left.AsReadOnly());
        }
    }
}
