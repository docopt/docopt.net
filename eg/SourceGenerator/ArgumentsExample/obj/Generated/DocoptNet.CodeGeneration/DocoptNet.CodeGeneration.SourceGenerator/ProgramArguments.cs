#nullable enable annotations

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DocoptNet.Generated;
using Leaves = DocoptNet.Generated.ReadOnlyList<DocoptNet.Generated.LeafPattern>;
using static DocoptNet.Generated.GeneratedSourceModule;

namespace ArgumentsExample
{
    partial class ProgramArguments : IEnumerable<KeyValuePair<string, object?>>
    {
        public const string Help = @"Usage: ArgumentsExample [-vqrh] [FILE] ...
       ArgumentsExample (--left | --right) CORRECTION FILE

Process FILE and optionally apply correction to either left-hand side or
right-hand side.

Arguments:
  FILE        optional input file
  CORRECTION  correction angle, needs FILE, --left or --right to be present

Options:
  -h --help
  -v       verbose mode
  -q       quiet mode
  -r       make report
  --left   use left-hand side
  --right  use right-hand side
";

        public const string Usage = @"Usage: ArgumentsExample [-vqrh] [FILE] ...
       ArgumentsExample (--left | --right) CORRECTION FILE";

        public static ProgramArguments Apply(IEnumerable<string> args, bool help = true, object? version = null, bool optionsFirst = false, bool exit = false)
        {
            var options = new List<Option>
            {
                new Option("-h", "--help", 0, false),
                new Option("-v", null, 0, false),
                new Option("-q", null, 0, false),
                new Option("-r", null, 0, false),
                new Option(null, "--left", 0, false),
                new Option(null, "--right", 0, false),
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
                    case "-v": result.OptV = (bool)value; break;
                    case "-q": result.OptQ = (bool)value; break;
                    case "-r": result.OptR = (bool)value; break;
                    case "--help": result.OptHelp = (bool)value; break;
                    case "FILE": result.ArgFile = (StringList)value; break;
                    case "--left": result.OptLeft = (bool)value; break;
                    case "--right": result.OptRight = (bool)value; break;
                    case "CORRECTION": result.ArgCorrection = (string?)value; break;
                }
            }

            return result;

            static void Match(ref RequiredMatcher required)
            {
                // Required(Either(Required(Optional(Option(-v,,0,False), Option(-q,,0,False), Option(-r,,0,False), Option(-h,--help,0,False)), OneOrMore(Optional(Argument(FILE, [])))), Required(Required(Either(Option(,--left,0,False), Option(,--right,0,False))), Argument(CORRECTION, ), Argument(FILE, []))))
                var a = new RequiredMatcher(1, required.Left, required.Collected);
                while (a.Next())
                {
                    // Either(Required(Optional(Option(-v,,0,False), Option(-q,,0,False), Option(-r,,0,False), Option(-h,--help,0,False)), OneOrMore(Optional(Argument(FILE, [])))), Required(Required(Either(Option(,--left,0,False), Option(,--right,0,False))), Argument(CORRECTION, ), Argument(FILE, [])))
                    var b = new EitherMatcher(2, a.Left, a.Collected);
                    while (b.Next())
                    {
                        switch (b.Index)
                        {
                            case 0:
                            {
                                // Required(Optional(Option(-v,,0,False), Option(-q,,0,False), Option(-r,,0,False), Option(-h,--help,0,False)), OneOrMore(Optional(Argument(FILE, []))))
                                var c = new RequiredMatcher(2, b.Left, b.Collected);
                                while (c.Next())
                                {
                                    switch (c.Index)
                                    {
                                        case 0:
                                        {
                                            // Optional(Option(-v,,0,False), Option(-q,,0,False), Option(-r,,0,False), Option(-h,--help,0,False))
                                            var d = new OptionalMatcher(4, c.Left, c.Collected);
                                            while (d.Next())
                                            {
                                                switch (d.Index)
                                                {
                                                    case 0:
                                                    {
                                                        // Option(-v,,0,False)
                                                        d.Match(PatternMatcher.MatchOption, "-v", ValueKind.Boolean);
                                                    }
                                                    break;
                                                    case 1:
                                                    {
                                                        // Option(-q,,0,False)
                                                        d.Match(PatternMatcher.MatchOption, "-q", ValueKind.Boolean);
                                                    }
                                                    break;
                                                    case 2:
                                                    {
                                                        // Option(-r,,0,False)
                                                        d.Match(PatternMatcher.MatchOption, "-r", ValueKind.Boolean);
                                                    }
                                                    break;
                                                    case 3:
                                                    {
                                                        // Option(-h,--help,0,False)
                                                        d.Match(PatternMatcher.MatchOption, "--help", ValueKind.Boolean);
                                                    }
                                                    break;
                                                }
                                                if (!d.LastMatched)
                                                {
                                                    break;
                                                }
                                            }
                                            c.Fold(d.Result);
                                        }
                                        break;
                                        case 1:
                                        {
                                            // OneOrMore(Optional(Argument(FILE, [])))
                                            var d = new OneOrMoreMatcher(1, c.Left, c.Collected);
                                            while (d.Next())
                                            {
                                                // Optional(Argument(FILE, []))
                                                var e = new OptionalMatcher(1, d.Left, d.Collected);
                                                while (e.Next())
                                                {
                                                    // Argument(FILE, [])
                                                    e.Match(PatternMatcher.MatchArgument, "FILE", ValueKind.StringList);
                                                    if (!e.LastMatched)
                                                    {
                                                        break;
                                                    }
                                                }
                                                d.Fold(e.Result);
                                                if (!d.LastMatched)
                                                {
                                                    break;
                                                }
                                            }
                                            c.Fold(d.Result);
                                        }
                                        break;
                                    }
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
                                // Required(Required(Either(Option(,--left,0,False), Option(,--right,0,False))), Argument(CORRECTION, ), Argument(FILE, []))
                                var c = new RequiredMatcher(3, b.Left, b.Collected);
                                while (c.Next())
                                {
                                    switch (c.Index)
                                    {
                                        case 0:
                                        {
                                            // Required(Either(Option(,--left,0,False), Option(,--right,0,False)))
                                            var d = new RequiredMatcher(1, c.Left, c.Collected);
                                            while (d.Next())
                                            {
                                                // Either(Option(,--left,0,False), Option(,--right,0,False))
                                                var e = new EitherMatcher(2, d.Left, d.Collected);
                                                while (e.Next())
                                                {
                                                    switch (e.Index)
                                                    {
                                                        case 0:
                                                        {
                                                            // Option(,--left,0,False)
                                                            e.Match(PatternMatcher.MatchOption, "--left", ValueKind.Boolean);
                                                        }
                                                        break;
                                                        case 1:
                                                        {
                                                            // Option(,--right,0,False)
                                                            e.Match(PatternMatcher.MatchOption, "--right", ValueKind.Boolean);
                                                        }
                                                        break;
                                                    }
                                                    if (!e.LastMatched)
                                                    {
                                                        break;
                                                    }
                                                }
                                                d.Fold(e.Result);
                                                if (!d.LastMatched)
                                                {
                                                    break;
                                                }
                                            }
                                            c.Fold(d.Result);
                                        }
                                        break;
                                        case 1:
                                        {
                                            // Argument(CORRECTION, )
                                            c.Match(PatternMatcher.MatchArgument, "CORRECTION", ValueKind.None);
                                        }
                                        break;
                                        case 2:
                                        {
                                            // Argument(FILE, [])
                                            c.Match(PatternMatcher.MatchArgument, "FILE", ValueKind.StringList);
                                        }
                                        break;
                                    }
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
            yield return KeyValuePair.Create("-v", (object?)OptV);
            yield return KeyValuePair.Create("-q", (object?)OptQ);
            yield return KeyValuePair.Create("-r", (object?)OptR);
            yield return KeyValuePair.Create("--help", (object?)OptHelp);
            yield return KeyValuePair.Create("FILE", (object?)ArgFile);
            yield return KeyValuePair.Create("--left", (object?)OptLeft);
            yield return KeyValuePair.Create("--right", (object?)OptRight);
            yield return KeyValuePair.Create("CORRECTION", (object?)ArgCorrection);
        }

        IEnumerator<KeyValuePair<string, object?>> IEnumerable<KeyValuePair<string, object?>>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary><c>Option(-v,,0,False)</c></summary>
        public bool OptV { get; private set; }

        /// <summary><c>Option(-q,,0,False)</c></summary>
        public bool OptQ { get; private set; }

        /// <summary><c>Option(-r,,0,False)</c></summary>
        public bool OptR { get; private set; }

        /// <summary><c>Option(-h,--help,0,False)</c></summary>
        public bool OptHelp { get; private set; }

        /// <summary><c>Argument(FILE, [])</c></summary>
        public StringList ArgFile { get; private set; } = StringList.Empty;

        /// <summary><c>Option(,--left,0,False)</c></summary>
        public bool OptLeft { get; private set; }

        /// <summary><c>Option(,--right,0,False)</c></summary>
        public bool OptRight { get; private set; }

        /// <summary><c>Argument(CORRECTION, )</c></summary>
        public string? ArgCorrection { get; private set; }
    }
}