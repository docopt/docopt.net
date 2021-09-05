#nullable enable annotations

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DocoptNet.Generated;
using Leaves = DocoptNet.Generated.ReadOnlyList<DocoptNet.Generated.LeafPattern>;
using static DocoptNet.Generated.GeneratedSourceModule;

namespace Git
{
    partial class GitAddArguments : IEnumerable<KeyValuePair<string, object?>>
    {
        public const string Help = @"usage: git add [options] [--] [<filepattern>...]

    -h, --help
    -n, --dry-run        dry run
    -v, --verbose        be verbose

    -i, --interactive    interactive picking
    -p, --patch          select hunks interactively
    -e, --edit           edit current diff and apply
    -f, --force          allow adding otherwise ignored files
    -u, --update         update tracked files
    -N, --intent-to-add  record only the fact that the path will be added later
    -A, --all            add all, noticing removal of tracked files
    --refresh            don't add, only refresh the index
    --ignore-errors      just skip files which cannot be added because of errors
    --ignore-missing     check if - even missing - files are ignored in dry run
";

        public const string Usage = "usage: git add [options] [--] [<filepattern>...]";

        public static GitAddArguments Apply(IEnumerable<string> args, bool help = true, object? version = null, bool optionsFirst = false)
        {
            var options = new List<Option>
            {
            };
            var left = ParseArgv(Help, args, options, optionsFirst, help, version);
            var required = new RequiredMatcher(1, left, new Leaves());
            Match(ref required);
            var collected = GetSuccessfulCollection(required, Usage);
            var result = new GitAddArguments();

            foreach (var leaf in collected)
            {
                var value = leaf.Value is { IsStringList: true } ? ((StringList)leaf.Value).Reverse() : leaf.Value;
                switch (leaf.Name)
                {
                    case "add": result.CmdAdd = (bool)value; break;
                    case "--": result.Cmd = (bool)value; break;
                    case "<filepattern>": result.ArgFilepattern = (StringList)value; break;
                }
            }

            return result;

            static void Match(ref RequiredMatcher required)
            {
                // Required(Required(Command(add, False), Optional(OptionsShortcut()), Optional(Command(--, False)), Optional(OneOrMore(Argument(<filepattern>, [])))))
                var a = new RequiredMatcher(1, required.Left, required.Collected);
                while (a.Next())
                {
                    // Required(Command(add, False), Optional(OptionsShortcut()), Optional(Command(--, False)), Optional(OneOrMore(Argument(<filepattern>, []))))
                    var b = new RequiredMatcher(4, a.Left, a.Collected);
                    while (b.Next())
                    {
                        switch (b.Index)
                        {
                            case 0:
                            {
                                // Command(add, False)
                                b.Match(PatternMatcher.MatchCommand, "add", ValueKind.Boolean);
                            }
                            break;
                            case 1:
                            {
                                // Optional(OptionsShortcut())
                                var c = new OptionalMatcher(1, b.Left, b.Collected);
                                while (c.Next())
                                {
                                    // OptionsShortcut()
                                    var d = new OptionalMatcher(0, c.Left, c.Collected);
                                    while (d.Next())
                                    {
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
                                // Optional(Command(--, False))
                                var c = new OptionalMatcher(1, b.Left, b.Collected);
                                while (c.Next())
                                {
                                    // Command(--, False)
                                    c.Match(PatternMatcher.MatchCommand, "--", ValueKind.Boolean);
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
                                // Optional(OneOrMore(Argument(<filepattern>, [])))
                                var c = new OptionalMatcher(1, b.Left, b.Collected);
                                while (c.Next())
                                {
                                    // OneOrMore(Argument(<filepattern>, []))
                                    var d = new OneOrMoreMatcher(1, c.Left, c.Collected);
                                    while (d.Next())
                                    {
                                        // Argument(<filepattern>, [])
                                        d.Match(PatternMatcher.MatchArgument, "<filepattern>", ValueKind.StringList);
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
            yield return KeyValuePair.Create("add", (object?)CmdAdd);
            yield return KeyValuePair.Create("--", (object?)Cmd);
            yield return KeyValuePair.Create("<filepattern>", (object?)ArgFilepattern);
        }

        IEnumerator<KeyValuePair<string, object?>> IEnumerable<KeyValuePair<string, object?>>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary><c>Command(add, False)</c></summary>
        public bool CmdAdd { get; private set; }

        /// <summary><c>Command(--, False)</c></summary>
        public bool Cmd { get; private set; }

        /// <summary><c>Argument(&lt;filepattern&gt;, [])</c></summary>
        public StringList ArgFilepattern { get; private set; } = StringList.Empty;
    }
}
