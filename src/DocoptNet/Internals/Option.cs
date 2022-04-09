// Licensed under terms of MIT license (see LICENSE-MIT)
// Copyright 2012 Vladimir Keleshev, 2013 Dinh Doan Van Bien, 2021 Atif Aziz

namespace DocoptNet.Internals
{
    using System;
    using System.Text.RegularExpressions;

    partial class Option : LeafPattern
    {
        public string ShortName { get; private set; }
        public string LongName { get; private set; }
        public int ArgCount { get; private set; }

        public Option(string shortName = null, string longName = null, int argCount = 0, ArgValue? value = null)
        {
            ShortName = shortName;
            LongName = longName;
            ArgCount = argCount;
            var v = value ?? ArgValue.False;
            Value = v.IsFalse && argCount > 0 ? ArgValue.None : v;
        }

        public Option(string shortName, string longName, int argCount, string value) :
            this(shortName, longName, argCount, (ArgValue)value) { }

        public override string Name
        {
            get { return LongName ?? ShortName; }
        }

        public override string ToString()
        {
            return $"Option({ShortName},{LongName},{ArgCount},{Value})";
        }

        const string DESC_SEPARATOR = "  ";

        static readonly char[] OptionDelimiters = { ' ', '\t', ',', '=' };

        public static Option Parse(string optionDescription)
        {
            if (optionDescription == null) throw new ArgumentNullException(nameof(optionDescription));

            string shortName = null;
            string longName = null;
            var argCount = 0;
            var value = ArgValue.False;
            var (options, _, description) = optionDescription.Trim().Partition(DESC_SEPARATOR);
            foreach (var s in options.Split(OptionDelimiters, StringSplitOptions.RemoveEmptyEntries))
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
                value = m.Success ? m.Groups[1].Value : ArgValue.None;
            }
            return new Option(shortName, longName, argCount, value);
        }
    }
}
