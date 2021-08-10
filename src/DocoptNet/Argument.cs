namespace DocoptNet
{
    class Argument: LeafPattern
    {
        public Argument(string name) : this(name, Value.Null)
        {
        }

        public Argument(string name, string value)
            : this(name, Value.Init(value))
        {
        }

        public Argument(string name, string[] values)
            : this(name, Value.Init(Stack.BottomTop(values)))
        {
        }

        public Argument(string name, int value)
            : this(name, Value.Init(value))
        {
        }

        public Argument(string name, Value value) : base(name, value)
        {
        }

        public override Node ToNode()
        {
            return new ArgumentNode(this.Name, Value.IsStringList ? ValueType.List : ValueType.String);
        }

        public override string GenerateCode()
        {
            var s = Name.Replace("<", "").Replace(">", "").ToLowerInvariant();
            s = "Arg" + GenerateCodeHelper.ConvertToPascalCase(s);

            if (Value.IsStringList)
            {
                return $"public ArrayList {s} {{ get {{ return _args[\"{Name}\"].AsList; }} }}";
            }
            return string.Format("public string {0} {{ get {{ return null == _args[\"{1}\"] ? null : _args[\"{1}\"].ToString(); }} }}", s, Name);
        }
    }
}
