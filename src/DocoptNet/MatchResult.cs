namespace DocoptNet
{
    class MatchResult
    {
        public readonly bool Matched;
        public readonly ReadOnlyList<LeafPattern> Left;
        public readonly ReadOnlyList<LeafPattern> Collected;

        public MatchResult(bool matched, ReadOnlyList<LeafPattern> left, ReadOnlyList<LeafPattern> collected)
        {
            Matched = matched;
            Left = left;
            Collected = collected;
        }

        public override bool Equals(object obj)
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

            return ToString().Equals(obj.ToString());
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            var left = string.Join(", ", Left);
            var collected = string.Join(", ", Collected);
            return $"matched={Matched} left=[{left}], collected=[{collected}]";
        }

        public void Deconstruct(out bool matched, out ReadOnlyList<LeafPattern> left, out ReadOnlyList<LeafPattern> collected)
        {
            (matched, left, collected) = (Matched, Left, Collected);
        }
    }
}
