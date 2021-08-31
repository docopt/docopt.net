#nullable enable annotations

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DocoptNet.Generated;
using Leaves = DocoptNet.Generated.ReadOnlyList<DocoptNet.Generated.LeafPattern>;
using static DocoptNet.Generated.GeneratedSourceModule;

namespace CountedExample
{
    partial class ProgramArguments : IEnumerable<KeyValuePair<string, object?>>
    {
        public const string Help = @"Usage: CountedExample --help
       CountedExample -v...
       CountedExample go [go]
       CountedExample (--path=<path>)...
       CountedExample <file> <file>

Try: CountedExample -vvvvvvvvvv
     CountedExample go go
     CountedExample --path ./here --path ./there
     CountedExample this.txt that.txt
";

        public const string Usage = @"Usage: CountedExample --help
       CountedExample -v...
       CountedExample go [go]
       CountedExample (--path=<path>)...
       CountedExample <file> <file>";

        public static ProgramArguments Apply(IEnumerable<string> args, bool help = true, object? version = null, bool optionsFirst = false, bool exit = false)
        {
            var options = new List<Option>
            {
                new Option(null, "--help", 0, false),
                new Option("-v", null, 0, 0),
                new Option(null, "--path", 1, StringList.Empty),
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
                    case "-v": result.OptV = (int)value; break;
                    case "go": result.CmdGo = (int)value; break;
                    case "--path": result.OptPath = (StringList)value; break;
                    case "<file>": result.ArgFile = (StringList)value; break;
                }
            }

            return result;

            static void Match(ref RequiredMatcher required)
            {
                // Required(Either(Required(Option(,--help,0,False)), Required(OneOrMore(Option(-v,,0,0))), Required(Command(go, 0), Optional(Command(go, 0))), Required(OneOrMore(Required(Option(,--path,1,[])))), Required(Argument(<file>, []), Argument(<file>, []))))
                var a = new RequiredMatcher(1, required.Left, required.Collected);
                while (a.Next())
                {
                    // Either(Required(Option(,--help,0,False)), Required(OneOrMore(Option(-v,,0,0))), Required(Command(go, 0), Optional(Command(go, 0))), Required(OneOrMore(Required(Option(,--path,1,[])))), Required(Argument(<file>, []), Argument(<file>, [])))
                    var b = new EitherMatcher(5, a.Left, a.Collected);
                    while (b.Next())
                    {
                        switch (b.Index)
                        {
                            case 0:
                            {
                                // Required(Option(,--help,0,False))
                                var c = new RequiredMatcher(1, b.Left, b.Collected);
                                while (c.Next())
                                {
                                    // Option(,--help,0,False)
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
                                // Required(OneOrMore(Option(-v,,0,0)))
                                var c = new RequiredMatcher(1, b.Left, b.Collected);
                                while (c.Next())
                                {
                                    // OneOrMore(Option(-v,,0,0))
                                    var d = new OneOrMoreMatcher(1, c.Left, c.Collected);
                                    while (d.Next())
                                    {
                                        // Option(-v,,0,0)
                                        d.Match(PatternMatcher.MatchOption, "-v", ValueKind.Integer);
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
                            case 2:
                            {
                                // Required(Command(go, 0), Optional(Command(go, 0)))
                                var c = new RequiredMatcher(2, b.Left, b.Collected);
                                while (c.Next())
                                {
                                    switch (c.Index)
                                    {
                                        case 0:
                                        {
                                            // Command(go, 0)
                                            c.Match(PatternMatcher.MatchCommand, "go", ValueKind.Integer);
                                        }
                                        break;
                                        case 1:
                                        {
                                            // Optional(Command(go, 0))
                                            var d = new OptionalMatcher(1, c.Left, c.Collected);
                                            while (d.Next())
                                            {
                                                // Command(go, 0)
                                                d.Match(PatternMatcher.MatchCommand, "go", ValueKind.Integer);
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
                            case 3:
                            {
                                // Required(OneOrMore(Required(Option(,--path,1,[]))))
                                var c = new RequiredMatcher(1, b.Left, b.Collected);
                                while (c.Next())
                                {
                                    // OneOrMore(Required(Option(,--path,1,[])))
                                    var d = new OneOrMoreMatcher(1, c.Left, c.Collected);
                                    while (d.Next())
                                    {
                                        // Required(Option(,--path,1,[]))
                                        var e = new RequiredMatcher(1, d.Left, d.Collected);
                                        while (e.Next())
                                        {
                                            // Option(,--path,1,[])
                                            e.Match(PatternMatcher.MatchOption, "--path", ValueKind.StringList);
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
                                    if (!c.LastMatched)
                                    {
                                        break;
                                    }
                                }
                                b.Fold(c.Result);
                            }
                            break;
                            case 4:
                            {
                                // Required(Argument(<file>, []), Argument(<file>, []))
                                var c = new RequiredMatcher(2, b.Left, b.Collected);
                                while (c.Next())
                                {
                                    switch (c.Index)
                                    {
                                        case 0:
                                        {
                                            // Argument(<file>, [])
                                            c.Match(PatternMatcher.MatchArgument, "<file>", ValueKind.StringList);
                                        }
                                        break;
                                        case 1:
                                        {
                                            // Argument(<file>, [])
                                            c.Match(PatternMatcher.MatchArgument, "<file>", ValueKind.StringList);
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
            yield return KeyValuePair.Create("--help", (object?)OptHelp);
            yield return KeyValuePair.Create("-v", (object?)OptV);
            yield return KeyValuePair.Create("go", (object?)CmdGo);
            yield return KeyValuePair.Create("--path", (object?)OptPath);
            yield return KeyValuePair.Create("<file>", (object?)ArgFile);
        }

        IEnumerator<KeyValuePair<string, object?>> IEnumerable<KeyValuePair<string, object?>>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary><c>Option(,--help,0,False)</c></summary>
        public bool OptHelp { get; private set; }

        /// <summary><c>Option(-v,,0,0)</c></summary>
        public int OptV { get; private set; }

        /// <summary><c>Command(go, 0)</c></summary>
        public int CmdGo { get; private set; }

        /// <summary><c>Option(,--path,1,[])</c></summary>
        public StringList OptPath { get; private set; } = StringList.Empty;

        /// <summary><c>Argument(&lt;file&gt;, [])</c></summary>
        public StringList ArgFile { get; private set; } = StringList.Empty;
    }
}
