namespace DocoptNet
{
    using System.Collections;

    class Argument: LeafPattern
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

        public override Node ToNode()
        {
            return new ArgumentNode(this.Name, (this.Value != null && this.Value.IsList) ? ValueType.List : ValueType.String);
        }

        public override string GenerateCode()
        {
            var s = Name.Replace("<", "").Replace(">", "").ToLowerInvariant();
            s = "Arg" + GenerateCodeHelper.ConvertToPascalCase(s);

            if (Value != null && Value.IsList)
            {
                return $"public ArrayList {s} {{ get {{ return _args[\"{Name}\"].AsList; }} }}";
            }
            return string.Format("public string {0} {{ get {{ return null == _args[\"{1}\"] ? null : _args[\"{1}\"].ToString(); }} }}", s, Name);
        }
    }
}
