using System;
using System.Collections.Generic;
using System.Linq;

namespace DocoptNet
{
    /// <summary>
    /// Leaf/terminal node of a pattern tree.
    /// </summary>
    internal class LeafPattern: Pattern
    {
        private readonly string _name;

        protected LeafPattern(string name, ValueObject value=null)
        {
            _name = name;
            Value = value;
        }

        protected LeafPattern()
        {
        }

        public override string Name
        {
            get { return _name; }
        }

        public override ICollection<Pattern> Flat(params Type[] types)
        {
            if (types == null) throw new ArgumentNullException("types");
            if (types.Length == 0 || types.Contains(this.GetType()))
            {
                return new Pattern[] { this };
            }
            return new Pattern[] {};
        }

        public virtual SingleMatchResult SingleMatch(IList<Pattern> patterns)
        {
            return new SingleMatchResult();
        }

        public override MatchResult Match(IList<Pattern> left, IEnumerable<Pattern> collected = null)
        {
            var coll = collected ?? new List<Pattern>();
            var sresult = SingleMatch(left);
            var match = sresult.Match;
            if (match == null)
            {
                return new MatchResult(false, left, coll);
            }
            var left_ = new List<Pattern>();
            left_.AddRange(left.Take(sresult.Position));
            left_.AddRange(left.Skip(sresult.Position + 1));
            var sameName = coll.Where(a => a.Name == Name).ToList();
            if (Value != null && (Value.IsList || Value.IsOfTypeInt))
            {
                var increment = new ValueObject(1);
                if (!Value.IsOfTypeInt)
                {
                    increment = match.Value.IsString ? new ValueObject(new [] {match.Value})  : match.Value;
                }
                if (sameName.Count == 0) 
                {
                    match.Value = increment;
                    var res = new List<Pattern>(coll) {match};
                    return new MatchResult(true, left_, res);
                }
                sameName[0].Value.Add(increment);
                return new MatchResult(true, left_, coll);
            }
            var resColl = new List<Pattern>();
            resColl.AddRange(coll);
            resColl.Add(match);
            return new MatchResult(true, left_, resColl);
        }

        public override string ToString()
        {
            return string.Format("{0}({1}, {2})", GetType().Name, Name, Value);
        }
    }

    internal class SingleMatchResult
    {
        public SingleMatchResult(int index, Pattern match)
        {
            Position = index;
            Match = match;
        }

        public SingleMatchResult()
        {
        }

        public int Position { get; set; }
        public Pattern Match { get; set; }
    }
}