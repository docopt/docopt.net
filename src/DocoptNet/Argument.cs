namespace DocoptNet
{
    using System.Collections;

    class Argument: LeafPattern
    {
        public Argument(string name, object value = null) : base(name, value)
        {
        }

        public Argument(string name, string value)
            : base(name, value)
        {
        }

        public Argument(string name, ICollection coll)
            : base(name, coll)
        {
        }

        public Argument(string name, int value)
            : base(name, value)
        {
        }

        public override Node ToNode()
        {
            return new ArgumentNode(this.Name, this.Value is ICollection ? ValueType.List : ValueType.String);
        }

        public override string GenerateCode()
        {
            var s = Name.Replace("<", "").Replace(">", "").ToLowerInvariant();
            s = "Arg" + GenerateCodeHelper.ConvertToPascalCase(s);

            if (Value is ICollection)
            {
                return $"public ArrayList {s} {{ get {{ return _args[\"{Name}\"].AsList; }} }}";
            }
            return string.Format("public string {0} {{ get {{ return null == _args[\"{1}\"] ? null : _args[\"{1}\"].ToString(); }} }}", s, Name);
        }
    }
}
