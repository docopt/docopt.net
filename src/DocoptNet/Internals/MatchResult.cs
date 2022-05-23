// Licensed under terms of MIT license (see LICENSE-MIT)
// Copyright 2012 Vladimir Keleshev, 2013 Dinh Doan Van Bien, 2021 Atif Aziz

namespace DocoptNet.Internals
{
    readonly partial struct MatchResult
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

        public override bool Equals(object? obj)
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

        public static bool operator true(MatchResult result) => result.Matched;
        public static bool operator false(MatchResult result) => !result;
        public static bool operator !(MatchResult result) => result ? false : true;
    }
}
