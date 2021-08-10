namespace DocoptNet
{
    using System.Globalization;

    class Argument: LeafPattern
    {
        public Argument(string name) : base(name, Value.Null)
        {
        }

        public Argument(string name, string value)
            : base(name, value)
        {
        }

        public Argument(string name, string[] values)
            : base(name, StringList.BottomTop(values))
        {
        }

        /// <remarks>
        /// This is only used by tests as a convenience. The instantiated
        /// <see cref="Value"/> is a string representation of the integer.
        /// </remarks>

        public Argument(string name, int value)
            : this(name, value.ToString(CultureInfo.InvariantCulture))
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
