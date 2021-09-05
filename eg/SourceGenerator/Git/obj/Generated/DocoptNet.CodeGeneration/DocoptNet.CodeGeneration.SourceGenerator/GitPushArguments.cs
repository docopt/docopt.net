#nullable enable annotations

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DocoptNet.Generated;
using Leaves = DocoptNet.Generated.ReadOnlyList<DocoptNet.Generated.LeafPattern>;
using static DocoptNet.Generated.GeneratedSourceModule;

namespace Git
{
    partial class GitPushArguments : IEnumerable<KeyValuePair<string, object?>>
    {
        public const string Help = @"usage: git push [options] [<repository> [<refspec>...]]

    -h, --help
    -v, --verbose         be more verbose
    -q, --quiet           be more quiet
    --repo <repository>   repository
    --all                 push all refs
    --mirror              mirror all refs
    --delete              delete refs
    --tags                push tags (can't be used with --all or --mirror)
    -n, --dry-run         dry run
    --porcelain           machine-readable output
    -f, --force           force updates
    --thin                use thin pack
    --receive-pack <receive-pack>
                          receive pack program
    --exec <receive-pack>
                          receive pack program
    -u, --set-upstream    set upstream for git pull/status
    --progress            force progress reporting
";

        public const string Usage = "usage: git push [options] [<repository> [<refspec>...]]";

        public static GitPushArguments Apply(IEnumerable<string> args, bool help = true, object? version = null, bool optionsFirst = false)
        {
            var options = new List<Option>
            {
            };
            var left = ParseArgv(Help, args, options, optionsFirst, help, version);
            var required = new RequiredMatcher(1, left, new Leaves());
            Match(ref required);
            var collected = GetSuccessfulCollection(required, Usage);
            var result = new GitPushArguments();

            foreach (var leaf in collected)
            {
                var value = leaf.Value is { IsStringList: true } ? ((StringList)leaf.Value).Reverse() : leaf.Value;
                switch (leaf.Name)
                {
                    case "push": result.CmdPush = (bool)value; break;
                    case "<repository>": result.ArgRepository = (string?)value; break;
                    case "<refspec>": result.ArgRefspec = (StringList)value; break;
                }
            }

            return result;

            static void Match(ref RequiredMatcher required)
            {
                // Required(Required(Command(push, False), Optional(OptionsShortcut()), Optional(Argument(<repository>, ), Optional(OneOrMore(Argument(<refspec>, []))))))
                var a = new RequiredMatcher(1, required.Left, required.Collected);
                while (a.Next())
                {
                    // Required(Command(push, False), Optional(OptionsShortcut()), Optional(Argument(<repository>, ), Optional(OneOrMore(Argument(<refspec>, [])))))
                    var b = new RequiredMatcher(3, a.Left, a.Collected);
                    while (b.Next())
                    {
                        switch (b.Index)
                        {
                            case 0:
                            {
                                // Command(push, False)
                                b.Match(PatternMatcher.MatchCommand, "push", ValueKind.Boolean);
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
                                // Optional(Argument(<repository>, ), Optional(OneOrMore(Argument(<refspec>, []))))
                                var c = new OptionalMatcher(2, b.Left, b.Collected);
                                while (c.Next())
                                {
                                    switch (c.Index)
                                    {
                                        case 0:
                                        {
                                            // Argument(<repository>, )
                                            c.Match(PatternMatcher.MatchArgument, "<repository>", ValueKind.None);
                                        }
                                        break;
                                        case 1:
                                        {
                                            // Optional(OneOrMore(Argument(<refspec>, [])))
                                            var d = new OptionalMatcher(1, c.Left, c.Collected);
                                            while (d.Next())
                                            {
                                                // OneOrMore(Argument(<refspec>, []))
                                                var e = new OneOrMoreMatcher(1, d.Left, d.Collected);
                                                while (e.Next())
                                                {
                                                    // Argument(<refspec>, [])
                                                    e.Match(PatternMatcher.MatchArgument, "<refspec>", ValueKind.StringList);
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
            yield return KeyValuePair.Create("push", (object?)CmdPush);
            yield return KeyValuePair.Create("<repository>", (object?)ArgRepository);
            yield return KeyValuePair.Create("<refspec>", (object?)ArgRefspec);
        }

        IEnumerator<KeyValuePair<string, object?>> IEnumerable<KeyValuePair<string, object?>>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary><c>Command(push, False)</c></summary>
        public bool CmdPush { get; private set; }

        /// <summary><c>Argument(&lt;repository&gt;, )</c></summary>
        public string? ArgRepository { get; private set; }

        /// <summary><c>Argument(&lt;refspec&gt;, [])</c></summary>
        public StringList ArgRefspec { get; private set; } = StringList.Empty;
    }
}
