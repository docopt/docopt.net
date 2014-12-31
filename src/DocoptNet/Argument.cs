using System.Collections;
using System.Collections.Generic;

namespace DocoptNet
{
    internal class Argument: LeafPattern
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

        public override Node ToNode()
        {
            return new ArgumentNode(this.Name, (this.Value != null && this.Value.IsList) ? ValueType.List : ValueType.String);
        }

        public override string GenerateCode()
        {
            var s = Name.Replace("<", "").Replace(">", " ").ToLowerInvariant();
            s = "Arg" + char.ToUpperInvariant(s[0]) + s.Substring(1);

            if (Value != null && Value.IsList)
            {
                return string.Format("public ArrayList {0} {{ get {{ return _args[\"{1}\"].AsList; }} }}", s, Name);
            }
            return string.Format("public string {0} {{ get {{ return _args[\"{1}\"].ToString(); }} }}", s, Name);
        }
    }
}