namespace DocoptNet
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    class Option : LeafPattern
    {
        public string ShortName { get; private set; }
        public string LongName { get; private set; }
        public int ArgCount { get; private set; }

        public Option(string shortName = null, string longName = null, int argCount = 0, ValueObject value = null)
            : base()
        {
            ShortName = shortName;
            LongName = longName;
            ArgCount = argCount;
            var v = value ?? new ValueObject(false);
            Value = (v.IsFalse && argCount > 0) ? null : v;
        }

        public Option(string shortName, string longName, int argCount, string value)
            : this(shortName, longName, argCount, new ValueObject(value))
        {
        }

        public override string Name
        {
            get { return LongName ?? ShortName; }
        }

        public override Node ToNode()
        {
            return new OptionNode(this.Name.TrimStart('-'), this.ArgCount == 0 ? ValueType.Bool : ValueType.String);
        }

        public override string GenerateCode()
        {
            var s = Name.ToLowerInvariant();
            s = "Opt" + GenerateCodeHelper.ConvertDashesToCamelCase(s);

            if (ArgCount == 0)
            {
                return string.Format("public bool {0} {{ get {{ return _args[\"{1}\"].IsTrue; }} }}", s, Name);
            }
            var defaultValue = Value == null ? "null" : string.Format("\"{0}\"", Value);
            return string.Format("public string {0} {{ get {{ return null == _args[\"{1}\"] ? {2} : _args[\"{1}\"].ToString(); }} }}", s, Name, defaultValue);
        }

        public override (int Index, LeafPattern Match) SingleMatch(IList<LeafPattern> left)
        {
            for (var i = 0; i < left.Count; i++)
            {
                if (left[i].Name == Name)
                    return (i, left[i]);
            }
            return default;
        }

        public override string ToString()
        {
            return string.Format("Option({0},{1},{2},{3})", ShortName, LongName, ArgCount, Value);
        }

        private const string DESC_SEPARATOR = "  ";

        public static Option Parse(string optionDescription)
        {
            if (optionDescription == null) throw new ArgumentNullException(nameof(optionDescription));

            string shortName = null;
            string longName = null;
            var argCount = 0;
            var value = new ValueObject(false);
            var (options, _, description) = optionDescription.Trim().Partition(DESC_SEPARATOR);
            foreach (var s in options.Split(" \t,=".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                if (s.StartsWith("--"))
                    longName = s;
                else if (s.StartsWith("-"))
                {
                    shortName = s;
                }
                else
                {
                    argCount = 1;
                }
            }
            if (argCount > 0)
            {
                var r = new Regex(@"\[default: (.*)\]", RegexOptions.IgnoreCase);
                var m = r.Match(description);
                value = m.Success ? new ValueObject(m.Groups[1].Value) : null;
            }
            return new Option(shortName, longName, argCount, value);
        }
    }
}
