namespace DocoptNet
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using Internals;

    partial class Docopt
    {
        public event EventHandler<PrintExitEventArgs> PrintExit;

        public IDictionary<string, ValueObject> Apply(string doc)
        {
            return Apply(doc, Array.Empty<string>());
        }

        public IDictionary<string, ValueObject> Apply(string doc, ICollection<string> argv, bool help = true,
            object version = null, bool optionsFirst = false, bool exit = false)
        {
            return Apply(doc, argv.AsEnumerable(), help, version, optionsFirst, exit)?.ToValueObjectDictionary();
        }

        ApplicationResult Apply(string doc, IEnumerable<string> argv,
                                bool help = true, object version = null,
                                bool optionsFirst = false, bool exit = false) =>
            Apply(doc, Tokens.From(argv), help, version, optionsFirst, exit);

        ApplicationResult Apply(string doc, Tokens tokens,
                                bool help, object version, bool optionsFirst, bool exit)
        {
            try
            {
                SetDefaultPrintExitHandlerIfNecessary(exit);

                var parsedResult = Parse(doc, tokens, optionsFirst);

                if (help && parsedResult.IsHelpOptionSpecified)
                    OnPrintExit(doc);

                if (version is not null && parsedResult.IsVersionOptionSpecified)
                    OnPrintExit(version.ToString());

                return parsedResult.Apply();
            }
            catch (DocoptBaseException e)
            {
                if (!exit)
                    throw;

                OnPrintExit(e.Message, e.ErrorCode);

                return null;
            }
        }

        static ParsedResult Parse(string doc, Tokens tokens, bool optionsFirst)
        {
            var usageSections = ParseSection("usage:", doc);
            if (usageSections.Length == 0)
                throw new DocoptLanguageErrorException("\"usage:\" (case-insensitive) not found.");
            if (usageSections.Length > 1)
                throw new DocoptLanguageErrorException("More that one \"usage:\" (case-insensitive).");
            var exitUsage = usageSections[0];
            var options = ParseDefaults(doc);
            var pattern = ParsePattern(FormalUsage(exitUsage), options);
            var arguments = ParseArgv(tokens, options, optionsFirst).AsReadOnly();
            var patternOptions = pattern.Flat<Option>().Distinct().ToList();
            // [default] syntax for argument is disabled
            foreach (OptionsShortcut optionsShortcut in pattern.Flat(typeof (OptionsShortcut)))
            {
                var docOptions = ParseDefaults(doc);
                optionsShortcut.Children = docOptions.Distinct().Except(patternOptions).ToList();
            }

            return new ParsedResult(pattern, arguments, exitUsage);
        }

        sealed class ParsedResult
        {
            readonly Required _pattern;
            readonly ReadOnlyList<LeafPattern> _arguments;
            readonly string _exitUsage;

            public ParsedResult(Required pattern, ReadOnlyList<LeafPattern> arguments, string exitUsage)
            {
                _pattern = pattern;
                _arguments = arguments;
                _exitUsage = exitUsage;
            }

            public bool IsHelpOptionSpecified =>
                _arguments.Any(o => o is { Name: "-h" or "--help", Value: { IsTrue: true } });

            public bool IsVersionOptionSpecified =>
                _arguments.Any(o => o is { Name: "--version", Value: { IsTrue: true } });

            public ApplicationResult Apply() =>
                _pattern.Fix().Match(_arguments) is (true, { Count: 0 }, var collected)
                    ? new ApplicationResult(_pattern.Flat().OfType<LeafPattern>().Concat(collected).ToReadOnlyList())
                    : throw new DocoptInputErrorException(_exitUsage);
        }

        private void SetDefaultPrintExitHandlerIfNecessary(bool exit)
        {
            if (exit && PrintExit == null)
                // Default behaviour is to print usage
                // and exit with error code 1
                PrintExit += (sender, args) =>
                {
                    Console.WriteLine(args.Message);
                    Environment.Exit(args.ErrorCode);
                };
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static class Internal
        {
            public static IDictionary<string, Value> Apply(Docopt docopt, string doc, IEnumerable<string> argv,
                                                           bool help = true, object version = null,
                                                           bool optionsFirst = false, bool exit = false) =>
                docopt.Apply(doc, Tokens.From(argv), help, version, optionsFirst, exit)?.ToValueDictionary();

            public static IEnumerable<T> GetNodes<T>(string doc,
                                                     Func<string, Value, T> commandSelector,
                                                     Func<string, Value, T> argumentSelector,
                                                     Func<string, string, string, int, Value, T> optionSelector)
            {
                var nodes =
                    from p in GetFlatPatterns(doc)
                    select p switch
                    {
                        Command command   => (true, Value: commandSelector(command.Name, command.Value)),
                        Argument argument => (true, Value: argumentSelector(argument.Name, argument.Value)),
                        Option option     => (true, Value: optionSelector(option.Name, option.LongName, option.ShortName, option.ArgCount, option.Value)),
                        _ => default,
                    }
                    into p
                    where p is (true, _)
                    select p.Value;

                return nodes.ToArray();
            }

            // TODO consider consolidating duplication with portions of Apply above
            public static (Pattern Pattern, ICollection<Option> Options, string ExitUsage)
                ParsePattern(string doc)
            {
                var usageSections = ParseSection("usage:", doc);
                if (usageSections.Length == 0)
                    throw new DocoptLanguageErrorException("\"usage:\" (case-insensitive) not found.");
                if (usageSections.Length > 1)
                    throw new DocoptLanguageErrorException("More that one \"usage:\" (case-insensitive).");
                var exitUsage = usageSections[0];
                var options = ParseDefaults(doc);
                var pattern = Docopt.ParsePattern(FormalUsage(exitUsage), options);
                var patternOptions = pattern.Flat<Option>().Distinct().ToList();
                // [default] syntax for argument is disabled
                foreach (OptionsShortcut optionsShortcut in pattern.Flat(typeof (OptionsShortcut)))
                {
                    var docOptions = ParseDefaults(doc);
                    optionsShortcut.Children = docOptions.Distinct().Except(patternOptions).ToList();
                }
                return (pattern.Fix(), options, exitUsage);
            }

            public static IEnumerable<Pattern> GetFlatPatterns(string doc)
            {
                var usageSections = ParseSection("usage:", doc);
                if (usageSections.Length == 0)
                    throw new DocoptLanguageErrorException("\"usage:\" (case-insensitive) not found.");
                if (usageSections.Length > 1)
                    throw new DocoptLanguageErrorException("More that one \"usage:\" (case-insensitive).");
                var exitUsage = usageSections[0];
                var options = ParseDefaults(doc);
                var pattern = Docopt.ParsePattern(FormalUsage(exitUsage), options);
                var patternOptions = pattern.Flat<Option>().Distinct().ToList();
                // [default] syntax for argument is disabled
                foreach (OptionsShortcut optionsShortcut in pattern.Flat(typeof (OptionsShortcut)))
                {
                    var docOptions = ParseDefaults(doc);
                    optionsShortcut.Children = docOptions.Distinct().Except(patternOptions).ToList();
                }
                return pattern.Fix().Flat();
            }
        }

        protected void OnPrintExit(string doc, int errorCode = 0)
        {
            if (PrintExit == null)
            {
                throw new DocoptExitException(doc);
            }
            else
            {
                PrintExit(this, new PrintExitEventArgs(doc, errorCode));
            }
        }

        /// <summary>
        ///     Parse command-line argument vector.
        /// </summary>
        internal static IList<LeafPattern> ParseArgv(Tokens tokens, ICollection<Option> options,
            bool optionsFirst = false)
        {
            //    If options_first:
            //        argv ::= [ long | shorts ]* [ argument ]* [ '--' [ argument ]* ] ;
            //    else:
            //        argv ::= [ long | shorts | argument ]* [ '--' [ argument ]* ] ;

            var parsed = new List<LeafPattern>();
            while (tokens.Current() is { } token)
            {
                if (token == "--")
                {
                    parsed.AddRange(tokens.Select(v => new Argument(null, v)));
                    return parsed;
                }

                if (token.StartsWith("--"))
                {
                    parsed.AddRange(ParseLong(tokens, options));
                }
                else if (token.StartsWith("-") && tokens.Current() != "-")
                {
                    parsed.AddRange(ParseShorts(tokens, options));
                }
                else if (optionsFirst)
                {
                    parsed.AddRange(tokens.Select(v => new Argument(null, v)));
                    return parsed;
                }
                else
                {
                    parsed.Add(new Argument(null, tokens.Move()));
                }
            }
            return parsed;
        }

        internal static string FormalUsage(string exitUsage)
        {
            var (_, _, section) = exitUsage.Partition(":"); // drop "usage:"
            var pu = section.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
            var join = new StringBuilder();
            join.Append("( ");
            for (var i = 1; i < pu.Length; i++)
            {
                var s = pu[i];
                if (i > 1) join.Append(" ");
                join.Append((s == pu[0]) ? ") | (" : s);
            }
            join.Append(" )");
            return join.ToString();
        }

        internal static Required ParsePattern(string source, ICollection<Option> options)
        {
            var tokens = Tokens.FromPattern(source);
            var result = ParseExpr(tokens, options);
            if (tokens.Current() != null)
                throw tokens.CreateException("unexpected ending: " + string.Join(" ", tokens.ToArray()));
            return new Required(result.ToArray());
        }

        private static IEnumerable<Pattern> ParseExpr(Tokens tokens, ICollection<Option> options)
        {
            // expr ::= seq ( '|' seq )* ;
            var seq = ParseSeq(tokens, options);
            if (tokens.Current() != "|")
                return seq;
            var result = new List<Pattern>();
            if (seq.Count > 1)
            {
                result.Add(new Required(seq.ToArray()));
            }
            else
            {
                result.AddRange(seq);
            }
            while (tokens.Current() == "|")
            {
                tokens.Move();
                seq = ParseSeq(tokens, options);
                if (seq.Count > 1)
                {
                    result.Add(new Required(seq.ToArray()));
                }
                else
                {
                    result.AddRange(seq);
                }
            }
            result = result.Distinct().ToList();
            if (result.Count > 1)
                return new[] {new Either(result.ToArray())};
            return result;
        }

        private static ICollection<Pattern> ParseSeq(Tokens tokens, ICollection<Option> options)
        {
            // seq ::= ( atom [ '...' ] )* ;
            var result = new List<Pattern>();
            while (!new[] {null, "]", ")", "|"}.Contains(tokens.Current()))
            {
                var atom = ParseAtom(tokens, options);
                if (tokens.Current() == "...")
                {
                    atom = new[] { new OneOrMore(atom.ToArray()) };
                    tokens.Move();
                }
                result.AddRange(atom);
            }
            return result;
        }

        private static IEnumerable<Pattern> ParseAtom(Tokens tokens, ICollection<Option> options)
        {
            // atom ::= '(' expr ')' | '[' expr ']' | 'options'
            //  | long | shorts | argument | command ;

            var token = tokens.Current() ?? throw new NullReferenceException();
            var result = new List<Pattern>();
            switch (token)
            {
                case "[":
                case "(":
                {
                    tokens.Move();
                    string matching;
                    if (token == "(")
                    {
                        matching = ")";
                        result.Add(new Required(ParseExpr(tokens, options).ToArray()));
                    }
                    else
                    {
                        matching = "]";
                        result.Add(new Optional(ParseExpr(tokens, options).ToArray()));
                    }
                    if (tokens.Current() != matching)
                        throw tokens.CreateException("unmatched '" + token + "'");
                    _ = tokens.Move();
                }
                    break;
                case "options":
                    tokens.Move();
                    result.Add(new OptionsShortcut());
                    break;
                default:
                    if (token.StartsWith("--") && token != "--")
                    {
                        return ParseLong(tokens, options);
                    }
                    if (token.StartsWith("-") && token != "-" && token != "--")
                    {
                        return ParseShorts(tokens, options);
                    }
                    if ((token.StartsWith("<") && token.EndsWith(">")) || token.All(c => char.IsUpper(c)))
                    {
                        result.Add(new Argument(tokens.Move()));
                    }
                    else
                    {
                        result.Add(new Command(tokens.Move()));
                    }
                    break;
            }
            return result;
        }

        private static IEnumerable<Option> ParseShorts(Tokens tokens, ICollection<Option> options)
        {
            // shorts ::= '-' ( chars )* [ [ ' ' ] chars ] ;

            var token = tokens.Move() ?? throw new NullReferenceException();
            Debug.Assert(token.StartsWith("-") && !token.StartsWith("--"));
            var left = token.TrimStart(new[] {'-'});
            var parsed = new List<Option>();
            while (left != "")
            {
                var shortName = "-" + left[0];
                left = left.Substring(1);
                var similar = options.Where(o => o.ShortName == shortName).ToList();
                Option option;
                if (similar.Count > 1)
                {
                    throw tokens.CreateException($"{shortName} is specified ambiguously {similar.Count} times");
                }
                if (similar.Count < 1)
                {
                    option = new Option(shortName, null, 0);
                    options.Add(option);
                    if (tokens.ThrowsInputError)
                    {
                        option = new Option(shortName, null, 0, Value.True);
                    }
                }
                else
                {
                    // why is copying necessary here?
                    option = new Option(shortName, similar[0].LongName, similar[0].ArgCount, similar[0].Value);
                    Value? value = null;
                    if (option.ArgCount != 0)
                    {
                        if (left == "")
                        {
                            if (tokens.Current() is null or "--")
                            {
                                throw tokens.CreateException(shortName + " requires argument");
                            }
                            value = tokens.Move() ?? Value.None;
                        }
                        else
                        {
                            value = left;
                            left = "";
                        }
                    }
                    if (tokens.ThrowsInputError)
                        option.Value = value ?? Value.True;
                }
                parsed.Add(option);
            }
            return parsed;
        }

        private static IEnumerable<Option> ParseLong(Tokens tokens, ICollection<Option> options)
        {
            // long ::= '--' chars [ ( ' ' | '=' ) chars ] ;
            var (longName, eq, value) = tokens.Move().Partition("=") switch
            {
                (var ln, "", _) => (ln, false, null),
                var (ln, _, vs) => (ln, true, vs)
            };
            Debug.Assert(longName.StartsWith("--"));
            var similar = options.Where(o => o.LongName == longName).ToList();
            if (tokens.ThrowsInputError && similar.Count == 0)
            {
                // If not exact match
                similar =
                    options.Where(o => !string.IsNullOrEmpty(o.LongName) && o.LongName.StartsWith(longName)).ToList();
            }
            if (similar.Count > 1)
            {
                // Might be simply specified ambiguously 2+ times?
                throw tokens.CreateException($"{longName} is not a unique prefix: {string.Join(", ", similar.Select(o => o.LongName))}?");
            }
            Option option;
            if (similar.Count < 1)
            {
                var argCount = eq ? 1 : 0;
                option = new Option(null, longName, argCount);
                options.Add(option);
                if (tokens.ThrowsInputError)
                    option = new Option(null, longName, argCount, value is { } v ? v : Value.True);
            }
            else
            {
                option = new Option(similar[0].ShortName, similar[0].LongName, similar[0].ArgCount, similar[0].Value);
                if (option.ArgCount == 0)
                {
                    if (value != null)
                        throw tokens.CreateException(option.LongName + " must not have an argument");
                }
                else
                {
                    if (value == null)
                    {
                        if (tokens.Current() == null || tokens.Current() == "--")
                            throw tokens.CreateException(option.LongName + " requires an argument");
                        value = tokens.Move();
                    }
                }
                if (tokens.ThrowsInputError)
                    option.Value = value is { } v ? v : Value.True;
            }
            return new[] {option};
        }

        internal static ICollection<Option> ParseDefaults(string doc)
        {
            var defaults = new List<Option>();
            foreach (var s in ParseSection("options:", doc))
            {
                // FIXME corner case "bla: options: --foo"

                var (_, _, optionsText) = s.Partition(":"); // get rid of "options:"
                var a = Regex.Split("\n" + optionsText, @"\r?\n[ \t]*(-\S+?)");
                var split = new List<string>();
                for (var i = 1; i < a.Length - 1; i += 2)
                {
                    var s1 = a[i];
                    var s2 = a[i + 1];
                    split.Add(s1 + s2);
                }
                var options = split.Where(x => x.StartsWith("-")).Select(x => Option.Parse(x));
                defaults.AddRange(options);
            }
            return defaults;
        }

        internal static string[] ParseSection(string name, string source)
        {
            var pattern = new Regex(@"^([^\r\n]*" + Regex.Escape(name) + @"[^\r\n]*\r?\n?(?:[ \t].*?(?:\r?\n|$))*)",
                                    RegexOptions.IgnoreCase | RegexOptions.Multiline);
            return (from Match match in pattern.Matches(source) select match.Value.Trim()).ToArray();
        }
    }

    partial class PrintExitEventArgs : EventArgs
    {
        public PrintExitEventArgs(string msg, int errorCode)
        {
            Message = msg;
            ErrorCode = errorCode;
        }

        public string Message { get; set; }
        public int ErrorCode { get; set; }
    }
}
