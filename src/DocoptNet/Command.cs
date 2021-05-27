namespace DocoptNet
{
    class Command : Argument
    {
        public Command(string name, object value = null) : base(name, value ?? Boxed.False)
        {
        }

        public override Node ToNode() { return new CommandNode(this.Name); }

        public override string GenerateCode()
        {
            var s = Name.ToLowerInvariant();
            s = "Cmd" + GenerateCodeHelper.ConvertToPascalCase(s);
            return $"public bool {s} {{ get {{ return _args[\"{Name}\"] as bool? == true; }} }}";
        }

    }
}
