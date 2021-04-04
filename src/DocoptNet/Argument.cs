namespace DocoptNet
{
    using System.Collections;
    using System.Collections.Generic;

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

        public override (int Index, LeafPattern Match) SingleMatch(IList<LeafPattern> left)
        {
            for (var i = 0; i < left.Count; i++)
            {
                if (left[i] is Argument arg)
                    return (i, new Argument(Name, arg.Value));
            }
            return default;
        }

        public override Node ToNode()
        {
            return new ArgumentNode(this.Name, (this.Value != null && this.Value.IsList) ? ValueType.List : ValueType.String);
        }

        public override string GenerateCode()
        {
            var s = Name.Replace("<", "").Replace(">", " ").ToLowerInvariant();
            s = "Arg" + GenerateCodeHelper.ConvertDashesToCamelCase(s);

            if (Value != null && Value.IsList)
            {
                return string.Format("public ArrayList {0} {{ get {{ return _args[\"{1}\"].AsList; }} }}", s, Name);
            }
            return string.Format("public string {0} {{ get {{ return null == _args[\"{1}\"] ? null : _args[\"{1}\"].ToString(); }} }}", s, Name);
        }
    }
}
