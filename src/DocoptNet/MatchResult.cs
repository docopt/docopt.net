using System;
using System.Collections.Generic;
using System.Linq;

namespace DocoptNet
{
    internal class MatchResult
    {
        public bool Matched;
        public IList<Pattern> Left;
        public IEnumerable<Pattern> Collected;

        public MatchResult() { }

        public MatchResult(bool matched, IList<Pattern> left, IEnumerable<Pattern> collected)
        {
            Matched = matched;
            Left = left;
            Collected = collected;
        }

        public bool LeftIsEmpty { get { return Left.Count == 0; } }

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
    }
}