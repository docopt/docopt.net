using System.Collections.Generic;

namespace DocoptNet
{
    internal class Command : Argument
    {
        public Command(string name, ValueObject value = null) : base(name, value ?? new ValueObject(false))
        {
        }

        public override (int Index, Pattern Match) SingleMatch(IList<Pattern> left)
        {
            for (var i = 0; i < left.Count; i++)
            {
                var pattern = left[i];
                if (pattern is Argument)
                {
                    if (pattern.Value.ToString() == Name)
                        return (i, new Command(Name, new ValueObject(true)));
                    break;
                }
            }
            return default;
        }

        public override Node ToNode() { return new CommandNode(this.Name); }

        public override string GenerateCode()
        {
            var s = Name.ToLowerInvariant();
            s = "Cmd" + GenerateCodeHelper.ConvertDashesToCamelCase(s);
            return string.Format("public bool {0} {{ get {{ return _args[\"{1}\"].IsTrue; }} }}", s, Name);
        }

    }
}
