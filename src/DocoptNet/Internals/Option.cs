// Licensed under terms of MIT license (see LICENSE-MIT)
// Copyright 2012 Vladimir Keleshev, 2013 Dinh Doan Van Bien, 2021 Atif Aziz

namespace DocoptNet.Internals
{
    using System;
    using System.Diagnostics;
    using System.Text.RegularExpressions;

    sealed partial class Option : LeafPattern
    {
        public string? ShortName { get; }
        public string? LongName { get; }
        public int ArgCount { get; }

        public Option(string name, int argCount = 0, ArgValue? value = null) :
            this((name.Length == 2 ? name : null, // short names are always 2 characters long, e.g. "-h".
                  name.Length > 2 ? name : null), // long names are always at least 3 characters long, e.g. "--help".
                 argCount, value) { }

        public Option(string shortName, string longName, int argCount = 0, ArgValue? value = null) :
            this((shortName, longName), argCount, value) { }

        Option((string? Short, string? Long) name, int argCount, ArgValue? value) :
            base(name.Long ?? name.Short!, value switch { null or { IsFalse: true } when argCount > 0 => ArgValue.None, var v => v ?? ArgValue.False })
        {
            Debug.Assert(name is not (null, null));
            (ShortName, LongName) = name;
            ArgCount = argCount;
        }

        public TResult MapName<TResult>(Func<Option, string, TResult> longSelector,
                                        Func<Option, string, TResult> shortSelector,
                                        Func<Option, string, string, TResult> longShortSelector) =>
            MapName((Long: longSelector, Short: shortSelector, LongShort: longShortSelector),
                    static (self, ln, f) => f.Long(self, ln),
                    static (self, sn, f) => f.Short(self, sn),
                    static (self, ln, sn, f) => f.LongShort(self, ln, sn));

        public TResult MapName<T, TResult>(T arg,
                                           Func<Option, string, T, TResult> longSelector,
                                           Func<Option, string, T, TResult> shortSelector,
                                           Func<Option, string, string, T, TResult> longShortSelector) =>
            (LongName, ShortName) switch
            {
                ({ } longName, null) => longSelector(this, longName, arg),
                (null, { } shortName) => shortSelector(this, shortName, arg),
                var (longName, shortName) => longShortSelector(this, longName!, shortName!, arg),
            };

        public override string ToString()
        {
            return $"Option({ShortName},{LongName},{ArgCount},{Value})";
        }

        const string DescSeparator = "  ";

        static readonly char[] OptionDelimiters = { ' ', '\t', ',', '=' };

        public static Option Parse(string optionDescription)
        {
            if (optionDescription == null) throw new ArgumentNullException(nameof(optionDescription));

            string? shortName = null;
            string? longName = null;
            var argCount = 0;
            var value = ArgValue.False;
            var (options, _, description) = optionDescription.Trim().Partition(DescSeparator);
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
            return new Option((shortName, longName), argCount, value);
        }
    }
}
