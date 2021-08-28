#nullable enable annotations

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DocoptNet.Generated;
using Leaves = DocoptNet.Generated.ReadOnlyList<DocoptNet.Generated.LeafPattern>;

namespace OddEvenExample
{
    partial class ProgramArguments : IEnumerable<KeyValuePair<string, object?>>
    {
        public const string HelpText = @"Usage: OddEvenExample [-h | --help] (ODD EVEN)...

Example, try:
  OddEvenExample 1 2 3 4

Options:
  -h, --help
";

        public const string Usage = @"Usage: OddEvenExample [-h | --help] (ODD EVEN)...";

        public static ProgramArguments Apply(IEnumerable<string> args, bool help = true, object? version = null, bool optionsFirst = false, bool exit = false)
        {
            var tokens = new Tokens(args, typeof(DocoptInputErrorException));
            var options = new List<Option>
            {
                new Option("-h", "--help", 0, false),
            };
            var arguments = Docopt.ParseArgv(tokens, options, optionsFirst).AsReadOnly();
            if (help && arguments.Any(o => o is { Name: "-h" or "--help", Value: { IsTrue: true } }))
            {
                throw new DocoptExitException(HelpText);
            }
            if (version is not null && arguments.Any(o => o is { Name: "--version", Value: { IsTrue: true } }))
            {
                throw new DocoptExitException(version.ToString());
            }
            var left = arguments;
            var collected = new Leaves();
            var a = new RequiredMatcher(1, left, collected);
            do
            {
                // Required(Required(Optional(Option(-h,--help,0,False)), OneOrMore(Required(Argument(ODD, []), Argument(EVEN, [])))))
                var b = new RequiredMatcher(1, a.Left, a.Collected);
                while (b.Next())
                {
                    // Required(Optional(Option(-h,--help,0,False)), OneOrMore(Required(Argument(ODD, []), Argument(EVEN, []))))
                    var c = new RequiredMatcher(2, b.Left, b.Collected);
                    while (c.Next())
                    {
                        switch (c.Index)
                        {
                            case 0:
                            {
                                // Optional(Option(-h,--help,0,False))
                                var d = new OptionalMatcher(1, c.Left, c.Collected);
                                while (d.Next())
                                {
                                    // Option(-h,--help,0,False)
                                    d.Match(PatternMatcher.MatchOption, "--help", ValueKind.Boolean);
                                    if (!d.LastMatched)
                                        break;
                                }
                                c.Fold(d.Result);
                                break;
                            }
                            case 1:
                            {
                                // OneOrMore(Required(Argument(ODD, []), Argument(EVEN, [])))
                                var d = new OneOrMoreMatcher(1, c.Left, c.Collected);
                                while (d.Next())
                                {
                                    // Required(Argument(ODD, []), Argument(EVEN, []))
                                    var e = new RequiredMatcher(2, d.Left, d.Collected);
                                    while (e.Next())
                                    {
                                        switch (e.Index)
                                        {
                                            case 0:
                                            {
                                                // Argument(ODD, [])
                                                e.Match(PatternMatcher.MatchArgument, "ODD", ValueKind.StringList);
                                                break;
                                            }
                                            case 1:
                                            {
                                                // Argument(EVEN, [])
                                                e.Match(PatternMatcher.MatchArgument, "EVEN", ValueKind.StringList);
                                                break;
                                            }
                                        }
                                        if (!e.LastMatched)
                                            break;
                                    }
                                    d.Fold(e.Result);
                                    if (!d.LastMatched)
                                        break;
                                }
                                c.Fold(d.Result);
                                break;
                            }
                        }
                        if (!c.LastMatched)
                            break;
                    }
                    b.Fold(c.Result);
                    if (!b.LastMatched)
                        break;
                }
                a.Fold(b.Result);
            }
            while (false);

            if (!a.Result || a.Left.Count > 0)
            {
                throw new DocoptInputErrorException(Usage);
            }

            collected = a.Collected;
            var result = new ProgramArguments();

            foreach (var p in collected)
            {
                var value = p.Value is { IsStringList: true } ? ((StringList)p.Value).Reverse() : p.Value;
                switch (p.Name)
                {
                    case @"--help": result.OptHelp = (bool)value; break;
                    case @"ODD": result.ArgOdd = (StringList)value; break;
                    case @"EVEN": result.ArgEven = (StringList)value; break;
                }
            }

            return result;
        }

        IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
        {
            yield return KeyValuePair.Create("--help", (object?)OptHelp);
            yield return KeyValuePair.Create("ODD", (object?)ArgOdd);
            yield return KeyValuePair.Create("EVEN", (object?)ArgEven);
        }

        IEnumerator<KeyValuePair<string, object?>> IEnumerable<KeyValuePair<string, object?>>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary><c>Option(-h,--help,0,False)</c></summary>
        public bool OptHelp { get; private set; }

        /// <summary><c>Argument(ODD, [])</c></summary>
        public StringList ArgOdd { get; private set; } = StringList.Empty;

        /// <summary><c>Argument(EVEN, [])</c></summary>
        public StringList ArgEven { get; private set; } = StringList.Empty;
    }
}
