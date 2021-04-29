namespace DocoptNet
{
    using System.Collections.Generic;
    using System.Linq;

    class MatchResult
    {
        public readonly bool Matched;
        public readonly IReadOnlyList<LeafPattern> Left;
        public readonly IReadOnlyList<LeafPattern> Collected;

        public MatchResult() { }

        public MatchResult(bool matched, IReadOnlyList<LeafPattern> left, IReadOnlyList<LeafPattern> collected)
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
            var left = string.Join(", ", Left ?? Array<LeafPattern>.Empty);
            var collected = string.Join(", ", Collected ?? Array<LeafPattern>.Empty);
            return $"matched={Matched} left=[{left}], collected=[{collected}]";
        }

        public void Deconstruct(out bool matched, out IReadOnlyList<LeafPattern> left, out IReadOnlyList<LeafPattern> collected)
        {
            (matched, left, collected) = (Matched, Left, Collected);
        }
    }
}
