#nullable enable annotations

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DocoptNet.Generated;
using Leaves = DocoptNet.Generated.ReadOnlyList<DocoptNet.Generated.LeafPattern>;
using static DocoptNet.Generated.GeneratedSourceModule;

namespace Git
{
    partial class GitCommitArguments : IEnumerable<KeyValuePair<string, object?>>
    {
        public const string Help = @"usage: git commit [options] [--] [<filepattern>...]

    -h, --help
    -q, --quiet           suppress summary after successful commit
    -v, --verbose         show diff in commit message template

Commit message options
    -F, --file <file>     read message from file
    --author <author>     override author for commit
    --date <date>         override date for commit
    -m, --message <message>
                          commit message
    -c, --reedit-message <commit>
                          reuse and edit message from specified commit
    -C, --reuse-message <commit>
                          reuse message from specified commit
    --fixup <commit>      use autosquash formatted message to fixup specified commit
    --squash <commit>     use autosquash formatted message to squash specified commit
    --reset-author        the commit is authored by me now
                          (used with -C-c/--amend)
    -s, --signoff         add Signed-off-by:
    -t, --template <file>
                          use specified template file
    -e, --edit            force edit of commit
    --cleanup <default>   how to strip spaces and #comments from message
    --status              include status in commit message template

Commit contents options
    -a, --all             commit all changed files
    -i, --include         add specified files to index for commit
    --interactive         interactively add files
    -o, --only            commit only specified files
    -n, --no-verify       bypass pre-commit hook
    --dry-run             show what would be committed
    --short               show status concisely
    --branch              show branch information
    --porcelain           machine-readable output
    -z, --null            terminate entries with NUL
    --amend               amend previous commit
    --no-post-rewrite     bypass post-rewrite hook
    -u, --untracked-files=<mode>
                          show untracked files, optional modes: all, normal, no.
                          [default: all]
";

        public const string Usage = "usage: git commit [options] [--] [<filepattern>...]";

        public static GitCommitArguments Apply(IEnumerable<string> args, bool help = true, object? version = null, bool optionsFirst = false)
        {
            var options = new List<Option>
            {
            };
            var left = ParseArgv(Help, args, options, optionsFirst, help, version);
            var required = new RequiredMatcher(1, left, new Leaves());
            Match(ref required);
            var collected = GetSuccessfulCollection(required, Usage);
            var result = new GitCommitArguments();

            foreach (var leaf in collected)
            {
                var value = leaf.Value is { IsStringList: true } ? ((StringList)leaf.Value).Reverse() : leaf.Value;
                switch (leaf.Name)
                {
                    case "commit": result.CmdCommit = (bool)value; break;
                    case "--": result.Cmd = (bool)value; break;
                    case "<filepattern>": result.ArgFilepattern = (StringList)value; break;
                }
            }

            return result;

            static void Match(ref RequiredMatcher required)
            {
                // Required(Required(Command(commit, False), Optional(OptionsShortcut()), Optional(Command(--, False)), Optional(OneOrMore(Argument(<filepattern>, [])))))
                var a = new RequiredMatcher(1, required.Left, required.Collected);
                while (a.Next())
                {
                    // Required(Command(commit, False), Optional(OptionsShortcut()), Optional(Command(--, False)), Optional(OneOrMore(Argument(<filepattern>, []))))
                    var b = new RequiredMatcher(4, a.Left, a.Collected);
                    while (b.Next())
                    {
                        switch (b.Index)
                        {
                            case 0:
                            {
                                // Command(commit, False)
                                b.Match(PatternMatcher.MatchCommand, "commit", ValueKind.Boolean);
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
            yield return KeyValuePair.Create("commit", (object?)CmdCommit);
            yield return KeyValuePair.Create("--", (object?)Cmd);
            yield return KeyValuePair.Create("<filepattern>", (object?)ArgFilepattern);
        }

        IEnumerator<KeyValuePair<string, object?>> IEnumerable<KeyValuePair<string, object?>>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary><c>Command(commit, False)</c></summary>
        public bool CmdCommit { get; private set; }

        /// <summary><c>Command(--, False)</c></summary>
        public bool Cmd { get; private set; }

        /// <summary><c>Argument(&lt;filepattern&gt;, [])</c></summary>
        public StringList ArgFilepattern { get; private set; } = StringList.Empty;
    }
}
