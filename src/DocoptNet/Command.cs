namespace DocoptNet
{
    class Command : LeafPattern
    {
        public Command(string name, bool value = false) : base(name, value ? Value.True : Value.False)
        {
        }

        public override Node ToNode() { return new CommandNode(this.Name); }

        public override string GenerateCode()
        {
            var s = Name.ToLowerInvariant();
            s = "Cmd" + GenerateCodeHelper.ConvertToPascalCase(s);
            return $"public bool {s} {{ get {{ ValueObject v = _args[\"{Name}\"]; return v.IsTrue || v.IsOfTypeInt && v.AsInt > 0; }} }}";
        }

    }
}
