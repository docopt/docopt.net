namespace DocoptNet
{
    using System.Collections.Generic;
    using System.Linq;

    internal class MatchResult
    {
        public bool Matched;
        public IList<LeafPattern> Left;
        public IEnumerable<LeafPattern> Collected;

        public MatchResult() { }

        public MatchResult(bool matched, IList<LeafPattern> left, IEnumerable<LeafPattern> collected)
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
            return string.Format("matched={0} left=[{1}], collected=[{2}]",
                Matched,
                Left == null ? "" : string.Join(", ", Left.Select(p => p.ToString())),
                Collected == null ? "" : string.Join(", ", Collected.Select(p => p.ToString()))
            );
        }

        public void Deconstruct(out bool matched, out IList<LeafPattern> left, out IEnumerable<LeafPattern> collected)
        {
            (matched, left, collected) = (Matched, Left, Collected);
        }
    }
}
