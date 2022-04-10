// Licensed under terms of MIT license (see LICENSE-MIT)
// Copyright 2012 Vladimir Keleshev, 2013 Dinh Doan Van Bien, 2021 Atif Aziz

namespace DocoptNet.Internals
{
    using System;
    using System.Text.RegularExpressions;

    partial class Option : LeafPattern
    {
        public string? ShortName { get; }
        public string? LongName { get; }
        public int ArgCount { get; }

        public Option(char shortName, int argCount = 0, ArgValue? value = null) :
            this(ShortNameString(shortName), null, argCount, value) { }

        public Option(string longName, int argCount = 0, ArgValue? value = null) :
            this(null, longName, argCount, value) { }

        public Option(char shortName, string longName, int argCount = 0, ArgValue? value = null) :
            this(ShortNameString(shortName), longName, argCount, value) { }

        /* FIXME */public Option(string? shortName, string? longName, int argCount = 0, ArgValue? value = null) :
            base(longName ?? shortName!, value switch { null or { IsFalse: true } when argCount > 0 => ArgValue.None, var v => v ?? ArgValue.False })
        {
            ShortName = shortName;
            LongName = longName;
            ArgCount = argCount;
        }

        protected Option(string name, int argCount, string str) :
            this(name, argCount, (ArgValue)str) { }

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
            return new Option(shortName, longName, argCount, value);
        }

        static readonly string?[] ShortNameMap =
        {
            "-0", "-1", "-2", "-3", "-4", "-5", "-6", "-7", "-8", "-9",
            null, // :
            null, // ;
            null, // <
            null, // =
            null, // >
            "-?", // ?
            null, // @
            "-A", "-B", "-C", "-D", "-E", "-F", "-G", "-H", "-I", "-J", "-K", "-L", "-M",
            "-N", "-O", "-P", "-Q", "-R", "-S", "-T", "-U", "-V", "-W", "-X", "-Y", "-Z",
            null, // [
            null, // \
            null, // ]
            null, // ^
            null, // _
            null, // `
            "-a", "-b", "-c", "-d", "-e", "-f", "-g", "-h", "-i", "-j", "-k", "-l", "-m",
            "-n", "-o", "-p", "-q", "-r", "-s", "-t", "-u", "-v", "-w", "-x", "-y", "-z",
        };

        public static string ShortNameString(char ch) =>
            (ch is >= '0' and <= 'z' ? ShortNameMap[ch - '0'] : null) ?? "-" + ch;
    }
}
