using System.Collections;
using System.Collections.Generic;

namespace NDocOpt
{
    public class Argument: LeafPattern
    {
        public Argument(string name, ValueObject value = null) : base(name, value)
        {
        }

        public Argument(string name, string value)
            : base(name, new ValueObject(value))
        {
        }

        public Argument(string name, ICollection coll)
            : base(name, new ValueObject(coll))
        {
        }

        public Argument(string name, int value)
            : base(name, new ValueObject(value))
        {
        }

        public override SingleMatchResult SingleMatch(IList<Pattern> left)
        {
            for (var i = 0; i < left.Count; i++)
            {
                if (left[i] is Argument)
                    return new SingleMatchResult(i, new Argument(Name, left[i].Value));
            }
            return new SingleMatchResult();
        }
    }
}