#nullable enable annotations

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DocoptNet.Generated;
using Leaves = DocoptNet.Generated.ReadOnlyList<DocoptNet.Generated.LeafPattern>;
using static DocoptNet.Generated.GeneratedSourceModule;

namespace Git
{
    partial class GitArguments : IEnumerable<KeyValuePair<string, object?>>
    {
        public const string Help = @"usage: git [--version] [--exec-path=<path>] [--html-path]
           [-p|--paginate|--no-pager] [--no-replace-objects]
           [--bare] [--git-dir=<path>] [--work-tree=<path>]
           [-c <name>=<value>] [--help]
           <command> [<args>...]

options:
   -c <name=value>
   -h, --help
   -p, --paginate

The most commonly used git commands are:
   add        Add file contents to the index
   branch     List, create, or delete branches
   checkout   Checkout a branch or paths to the working tree
   clone      Clone a repository into a new directory
   commit     Record changes to the repository
   push       Update remote refs along with associated objects
   remote     Manage set of tracked repositories

See 'git help <command>' for more information on a specific command.
";

        public const string Usage = @"usage: git [--version] [--exec-path=<path>] [--html-path]
           [-p|--paginate|--no-pager] [--no-replace-objects]
           [--bare] [--git-dir=<path>] [--work-tree=<path>]
           [-c <name>=<value>] [--help]
           <command> [<args>...]";

        public static GitArguments Apply(IEnumerable<string> args, bool help = true, object? version = null, bool optionsFirst = false, bool exit = false)
        {
            var options = new List<Option>
            {
                new Option("-c", null, 1, null),
                new Option("-h", "--help", 0, false),
                new Option("-p", "--paginate", 0, false),
                new Option(null, "--version", 0, false),
                new Option(null, "--exec-path", 1, null),
                new Option(null, "--html-path", 0, false),
                new Option(null, "--no-pager", 0, false),
                new Option(null, "--no-replace-objects", 0, false),
                new Option(null, "--bare", 0, false),
                new Option(null, "--git-dir", 1, null),
                new Option(null, "--work-tree", 1, null),
            };
            var left = ParseArgv(Help, args, options, optionsFirst, help, version);
            var required = new RequiredMatcher(1, left, new Leaves());
            Match(ref required);
            var collected = GetSuccessfulCollection(required, Usage);
            var result = new GitArguments();

            foreach (var leaf in collected)
            {
                var value = leaf.Value is { IsStringList: true } ? ((StringList)leaf.Value).Reverse() : leaf.Value;
                switch (leaf.Name)
                {
                    case "--version": result.OptVersion = (bool)value; break;
                    case "--exec-path": result.OptExecPath = (string?)value; break;
                    case "--html-path": result.OptHtmlPath = (bool)value; break;
                    case "--paginate": result.OptPaginate = (bool)value; break;
                    case "--no-pager": result.OptNoPager = (bool)value; break;
                    case "--no-replace-objects": result.OptNoReplaceObjects = (bool)value; break;
                    case "--bare": result.OptBare = (bool)value; break;
                    case "--git-dir": result.OptGitDir = (string?)value; break;
                    case "--work-tree": result.OptWorkTree = (string?)value; break;
                    case "-c": result.OptC = (string?)value; break;
                    case "--help": result.OptHelp = (bool)value; break;
                    case "<command>": result.ArgCommand = (string?)value; break;
                    case "<args>": result.ArgArgs = (StringList)value; break;
                }
            }

            return result;

            static void Match(ref RequiredMatcher required)
            {
                // Required(Required(Optional(Option(,--version,0,False)), Optional(Option(,--exec-path,1,)), Optional(Option(,--html-path,0,False)), Optional(Either(Option(-p,--paginate,0,False), Option(,--no-pager,0,False))), Optional(Option(,--no-replace-objects,0,False)), Optional(Option(,--bare,0,False)), Optional(Option(,--git-dir,1,)), Optional(Option(,--work-tree,1,)), Optional(Option(-c,,1,)), Optional(Option(-h,--help,0,False)), Argument(<command>, ), Optional(OneOrMore(Argument(<args>, [])))))
                var a = new RequiredMatcher(1, required.Left, required.Collected);
                while (a.Next())
                {
                    // Required(Optional(Option(,--version,0,False)), Optional(Option(,--exec-path,1,)), Optional(Option(,--html-path,0,False)), Optional(Either(Option(-p,--paginate,0,False), Option(,--no-pager,0,False))), Optional(Option(,--no-replace-objects,0,False)), Optional(Option(,--bare,0,False)), Optional(Option(,--git-dir,1,)), Optional(Option(,--work-tree,1,)), Optional(Option(-c,,1,)), Optional(Option(-h,--help,0,False)), Argument(<command>, ), Optional(OneOrMore(Argument(<args>, []))))
                    var b = new RequiredMatcher(12, a.Left, a.Collected);
                    while (b.Next())
                    {
                        switch (b.Index)
                        {
                            case 0:
                            {
                                // Optional(Option(,--version,0,False))
                                var c = new OptionalMatcher(1, b.Left, b.Collected);
                                while (c.Next())
                                {
                                    // Option(,--version,0,False)
                                    c.Match(PatternMatcher.MatchOption, "--version", ValueKind.Boolean);
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
                                // Optional(Option(,--exec-path,1,))
                                var c = new OptionalMatcher(1, b.Left, b.Collected);
                                while (c.Next())
                                {
                                    // Option(,--exec-path,1,)
                                    c.Match(PatternMatcher.MatchOption, "--exec-path", ValueKind.None);
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
                                // Optional(Option(,--html-path,0,False))
                                var c = new OptionalMatcher(1, b.Left, b.Collected);
                                while (c.Next())
                                {
                                    // Option(,--html-path,0,False)
                                    c.Match(PatternMatcher.MatchOption, "--html-path", ValueKind.Boolean);
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
                                // Optional(Either(Option(-p,--paginate,0,False), Option(,--no-pager,0,False)))
                                var c = new OptionalMatcher(1, b.Left, b.Collected);
                                while (c.Next())
                                {
                                    // Either(Option(-p,--paginate,0,False), Option(,--no-pager,0,False))
                                    var d = new EitherMatcher(2, c.Left, c.Collected);
                                    while (d.Next())
                                    {
                                        switch (d.Index)
                                        {
                                            case 0:
                                            {
                                                // Option(-p,--paginate,0,False)
                                                d.Match(PatternMatcher.MatchOption, "--paginate", ValueKind.Boolean);
                                            }
                                            break;
                                            case 1:
                                            {
                                                // Option(,--no-pager,0,False)
                                                d.Match(PatternMatcher.MatchOption, "--no-pager", ValueKind.Boolean);
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
                            case 4:
                            {
                                // Optional(Option(,--no-replace-objects,0,False))
                                var c = new OptionalMatcher(1, b.Left, b.Collected);
                                while (c.Next())
                                {
                                    // Option(,--no-replace-objects,0,False)
                                    c.Match(PatternMatcher.MatchOption, "--no-replace-objects", ValueKind.Boolean);
                                    if (!c.LastMatched)
                                    {
                                        break;
                                    }
                                }
                                b.Fold(c.Result);
                            }
                            break;
                            case 5:
                            {
                                // Optional(Option(,--bare,0,False))
                                var c = new OptionalMatcher(1, b.Left, b.Collected);
                                while (c.Next())
                                {
                                    // Option(,--bare,0,False)
                                    c.Match(PatternMatcher.MatchOption, "--bare", ValueKind.Boolean);
                                    if (!c.LastMatched)
                                    {
                                        break;
                                    }
                                }
                                b.Fold(c.Result);
                            }
                            break;
                            case 6:
                            {
                                // Optional(Option(,--git-dir,1,))
                                var c = new OptionalMatcher(1, b.Left, b.Collected);
                                while (c.Next())
                                {
                                    // Option(,--git-dir,1,)
                                    c.Match(PatternMatcher.MatchOption, "--git-dir", ValueKind.None);
                                    if (!c.LastMatched)
                                    {
                                        break;
                                    }
                                }
                                b.Fold(c.Result);
                            }
                            break;
                            case 7:
                            {
                                // Optional(Option(,--work-tree,1,))
                                var c = new OptionalMatcher(1, b.Left, b.Collected);
                                while (c.Next())
                                {
                                    // Option(,--work-tree,1,)
                                    c.Match(PatternMatcher.MatchOption, "--work-tree", ValueKind.None);
                                    if (!c.LastMatched)
                                    {
                                        break;
                                    }
                                }
                                b.Fold(c.Result);
                            }
                            break;
                            case 8:
                            {
                                // Optional(Option(-c,,1,))
                                var c = new OptionalMatcher(1, b.Left, b.Collected);
                                while (c.Next())
                                {
                                    // Option(-c,,1,)
                                    c.Match(PatternMatcher.MatchOption, "-c", ValueKind.None);
                                    if (!c.LastMatched)
                                    {
                                        break;
                                    }
                                }
                                b.Fold(c.Result);
                            }
                            break;
                            case 9:
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
                            case 10:
                            {
                                // Argument(<command>, )
                                b.Match(PatternMatcher.MatchArgument, "<command>", ValueKind.None);
                            }
                            break;
                            case 11:
                            {
                                // Optional(OneOrMore(Argument(<args>, [])))
                                var c = new OptionalMatcher(1, b.Left, b.Collected);
                                while (c.Next())
                                {
                                    // OneOrMore(Argument(<args>, []))
                                    var d = new OneOrMoreMatcher(1, c.Left, c.Collected);
                                    while (d.Next())
                                    {
                                        // Argument(<args>, [])
                                        d.Match(PatternMatcher.MatchArgument, "<args>", ValueKind.StringList);
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
            yield return KeyValuePair.Create("--version", (object?)OptVersion);
            yield return KeyValuePair.Create("--exec-path", (object?)OptExecPath);
            yield return KeyValuePair.Create("--html-path", (object?)OptHtmlPath);
            yield return KeyValuePair.Create("--paginate", (object?)OptPaginate);
            yield return KeyValuePair.Create("--no-pager", (object?)OptNoPager);
            yield return KeyValuePair.Create("--no-replace-objects", (object?)OptNoReplaceObjects);
            yield return KeyValuePair.Create("--bare", (object?)OptBare);
            yield return KeyValuePair.Create("--git-dir", (object?)OptGitDir);
            yield return KeyValuePair.Create("--work-tree", (object?)OptWorkTree);
            yield return KeyValuePair.Create("-c", (object?)OptC);
            yield return KeyValuePair.Create("--help", (object?)OptHelp);
            yield return KeyValuePair.Create("<command>", (object?)ArgCommand);
            yield return KeyValuePair.Create("<args>", (object?)ArgArgs);
        }

        IEnumerator<KeyValuePair<string, object?>> IEnumerable<KeyValuePair<string, object?>>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary><c>Option(,--version,0,False)</c></summary>
        public bool OptVersion { get; private set; }

        /// <summary><c>Option(,--exec-path,1,)</c></summary>
        public string? OptExecPath { get; private set; }

        /// <summary><c>Option(,--html-path,0,False)</c></summary>
        public bool OptHtmlPath { get; private set; }

        /// <summary><c>Option(-p,--paginate,0,False)</c></summary>
        public bool OptPaginate { get; private set; }

        /// <summary><c>Option(,--no-pager,0,False)</c></summary>
        public bool OptNoPager { get; private set; }

        /// <summary><c>Option(,--no-replace-objects,0,False)</c></summary>
        public bool OptNoReplaceObjects { get; private set; }

        /// <summary><c>Option(,--bare,0,False)</c></summary>
        public bool OptBare { get; private set; }

        /// <summary><c>Option(,--git-dir,1,)</c></summary>
        public string? OptGitDir { get; private set; }

        /// <summary><c>Option(,--work-tree,1,)</c></summary>
        public string? OptWorkTree { get; private set; }

        /// <summary><c>Option(-c,,1,)</c></summary>
        public string? OptC { get; private set; }

        /// <summary><c>Option(-h,--help,0,False)</c></summary>
        public bool OptHelp { get; private set; }

        /// <summary><c>Argument(&lt;command&gt;, )</c></summary>
        public string? ArgCommand { get; private set; }

        /// <summary><c>Argument(&lt;args&gt;, [])</c></summary>
        public StringList ArgArgs { get; private set; } = StringList.Empty;
    }
}
