#nullable enable annotations

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DocoptNet.Generated;
using Leaves = DocoptNet.Generated.ReadOnlyList<DocoptNet.Generated.LeafPattern>;
using static DocoptNet.Generated.GeneratedSourceModule;

namespace OddEvenExample
{
    partial class ProgramArguments : IEnumerable<KeyValuePair<string, object?>>
    {
        public const string Help = @"Usage: OddEvenExample [-h | --help] (ODD EVEN)...

Example, try:
  OddEvenExample 1 2 3 4

Options:
  -h, --help
";

        public const string Usage = "Usage: OddEvenExample [-h | --help] (ODD EVEN)...";

        public static ProgramArguments Apply(IEnumerable<string> args, bool help = true, object? version = null, bool optionsFirst = false)
        {
            var options = new List<Option>
            {
                new Option("-h", "--help", 0, false),
            };
            var left = ParseArgv(Help, args, options, optionsFirst, help, version);
            var required = new RequiredMatcher(1, left, new Leaves());
            Match(ref required);
            var collected = GetSuccessfulCollection(required, Usage);
            var result = new ProgramArguments();

            foreach (var leaf in collected)
            {
                var value = leaf.Value is { IsStringList: true } ? ((StringList)leaf.Value).Reverse() : leaf.Value;
                switch (leaf.Name)
                {
                    case "--help": result.OptHelp = (bool)value; break;
                    case "ODD": result.ArgOdd = (StringList)value; break;
                    case "EVEN": result.ArgEven = (StringList)value; break;
                }
            }

            return result;

            static void Match(ref RequiredMatcher required)
            {
                // Required(Required(Optional(Option(-h,--help,0,False)), OneOrMore(Required(Argument(ODD, []), Argument(EVEN, [])))))
                var a = new RequiredMatcher(1, required.Left, required.Collected);
                while (a.Next())
                {
                    // Required(Optional(Option(-h,--help,0,False)), OneOrMore(Required(Argument(ODD, []), Argument(EVEN, []))))
                    var b = new RequiredMatcher(2, a.Left, a.Collected);
                    while (b.Next())
                    {
                        switch (b.Index)
                        {
                            case 0:
                            {
                                // Optional(Option(-h,--help,0,False))
                                var c = new OptionalMatcher(1, b.Left, b.Collected);
                                while (c.Next())
                                {
                                    // Option(-h,--help,0,False)
                                    c.Match(PatternMatcher.MatchOption, "--help", ValueKind.Boolean);
                                    if (!c.LastMatched)
                                    {
                                        break;
                                    }
                                }
                                b.Fold(c.Result);
                            }
                            break;
                            case 1:
                            {
                                // OneOrMore(Required(Argument(ODD, []), Argument(EVEN, [])))
                                var c = new OneOrMoreMatcher(1, b.Left, b.Collected);
                                while (c.Next())
                                {
                                    // Required(Argument(ODD, []), Argument(EVEN, []))
                                    var d = new RequiredMatcher(2, c.Left, c.Collected);
                                    while (d.Next())
                                    {
                                        switch (d.Index)
                                        {
                                            case 0:
                                            {
                                                // Argument(ODD, [])
                                                d.Match(PatternMatcher.MatchArgument, "ODD", ValueKind.StringList);
                                            }
                                            break;
                                            case 1:
                                            {
                                                // Argument(EVEN, [])
                                                d.Match(PatternMatcher.MatchArgument, "EVEN", ValueKind.StringList);
                                            }
                                            break;
                                        }
                                        if (!d.LastMatched)
                                        {
                                            break;
                                        }
                                    }
                                    c.Fold(d.Result);
                                    if (!c.LastMatched)
                                    {
                                        break;
                                    }
                                }
                                b.Fold(c.Result);
                            }
                            break;
                        }
                        if (!b.LastMatched)
                        {
                            break;
                        }
                    }
                    a.Fold(b.Result);
                    if (!a.LastMatched)
                    {
                        break;
                    }
                }
                required.Fold(a.Result);
            }
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
