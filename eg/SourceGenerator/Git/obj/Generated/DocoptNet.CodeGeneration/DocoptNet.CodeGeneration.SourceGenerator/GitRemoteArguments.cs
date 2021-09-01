#nullable enable annotations

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DocoptNet.Generated;
using Leaves = DocoptNet.Generated.ReadOnlyList<DocoptNet.Generated.LeafPattern>;
using static DocoptNet.Generated.GeneratedSourceModule;

namespace Git
{
    partial class GitRemoteArguments : IEnumerable<KeyValuePair<string, object?>>
    {
        public const string Help = @"usage: git remote [-v | --verbose]
       git remote add [-t <branch>] [-m <master>] [-f] [--mirror] <name> <url>
       git remote rename <old> <new>
       git remote rm <name>
       git remote set-head <name> (-a | -d | <branch>)
       git remote [-v | --verbose] show [-n] <name>
       git remote prune [-n | --dry-run] <name>
       git remote [-v | --verbose] update [-p | --prune] [(<group> | <remote>)...]
       git remote set-branches <name> [--add] <branch>...
       git remote set-url <name> <newurl> [<oldurl>]
       git remote set-url --add <name> <newurl>
       git remote set-url --delete <name> <url>

    -v, --verbose         be verbose; must be placed before a subcommand
";

        public const string Usage = @"usage: git remote [-v | --verbose]
       git remote add [-t <branch>] [-m <master>] [-f] [--mirror] <name> <url>
       git remote rename <old> <new>
       git remote rm <name>
       git remote set-head <name> (-a | -d | <branch>)
       git remote [-v | --verbose] show [-n] <name>
       git remote prune [-n | --dry-run] <name>
       git remote [-v | --verbose] update [-p | --prune] [(<group> | <remote>)...]
       git remote set-branches <name> [--add] <branch>...
       git remote set-url <name> <newurl> [<oldurl>]
       git remote set-url --add <name> <newurl>
       git remote set-url --delete <name> <url>";

        public static GitRemoteArguments Apply(IEnumerable<string> args, bool help = true, object? version = null, bool optionsFirst = false, bool exit = false)
        {
            var options = new List<Option>
            {
                new Option("-v", null, 0, false),
                new Option(null, "--verbose", 0, false),
                new Option("-t", null, 0, false),
                new Option("-m", null, 0, false),
                new Option("-f", null, 0, false),
                new Option(null, "--mirror", 0, false),
                new Option("-a", null, 0, false),
                new Option("-d", null, 0, false),
                new Option("-n", null, 0, false),
                new Option(null, "--dry-run", 0, false),
                new Option("-p", null, 0, false),
                new Option(null, "--prune", 0, false),
                new Option(null, "--add", 0, false),
                new Option(null, "--delete", 0, false),
            };
            var left = ParseArgv(Help, args, options, optionsFirst, help, version);
            var required = new RequiredMatcher(1, left, new Leaves());
            Match(ref required);
            var collected = GetSuccessfulCollection(required, Usage);
            var result = new GitRemoteArguments();

            foreach (var leaf in collected)
            {
                var value = leaf.Value is { IsStringList: true } ? ((StringList)leaf.Value).Reverse() : leaf.Value;
                switch (leaf.Name)
                {
                    case "remote": result.CmdRemote = (bool)value; break;
                    case "-v": result.OptV = (bool)value; break;
                    case "--verbose": result.OptVerbose = (bool)value; break;
                    case "add": result.CmdAdd = (bool)value; break;
                    case "-t": result.OptT = (bool)value; break;
                    case "<branch>": result.ArgBranch = (StringList)value; break;
                    case "-m": result.OptM = (bool)value; break;
                    case "<master>": result.ArgMaster = (string?)value; break;
                    case "-f": result.OptF = (bool)value; break;
                    case "--mirror": result.OptMirror = (bool)value; break;
                    case "<name>": result.ArgName = (string?)value; break;
                    case "<url>": result.ArgUrl = (string?)value; break;
                    case "rename": result.CmdRename = (bool)value; break;
                    case "<old>": result.ArgOld = (string?)value; break;
                    case "<new>": result.ArgNew = (string?)value; break;
                    case "rm": result.CmdRm = (bool)value; break;
                    case "set-head": result.CmdSetHead = (bool)value; break;
                    case "-a": result.OptA = (bool)value; break;
                    case "-d": result.OptD = (bool)value; break;
                    case "show": result.CmdShow = (bool)value; break;
                    case "-n": result.OptN = (bool)value; break;
                    case "prune": result.CmdPrune = (bool)value; break;
                    case "--dry-run": result.OptDryRun = (bool)value; break;
                    case "update": result.CmdUpdate = (bool)value; break;
                    case "-p": result.OptP = (bool)value; break;
                    case "--prune": result.OptPrune = (bool)value; break;
                    case "<group>": result.ArgGroup = (StringList)value; break;
                    case "<remote>": result.ArgRemote = (StringList)value; break;
                    case "set-branches": result.CmdSetBranches = (bool)value; break;
                    case "--add": result.OptAdd = (bool)value; break;
                    case "set-url": result.CmdSetUrl = (bool)value; break;
                    case "<newurl>": result.ArgNewurl = (string?)value; break;
                    case "<oldurl>": result.ArgOldurl = (string?)value; break;
                    case "--delete": result.OptDelete = (bool)value; break;
                }
            }

            return result;

            static void Match(ref RequiredMatcher required)
            {
                // Required(Either(Required(Command(remote, False), Optional(Either(Option(-v,,0,False), Option(,--verbose,0,False)))), Required(Command(remote, False), Command(add, False), Optional(Option(-t,,0,False), Argument(<branch>, [])), Optional(Option(-m,,0,False), Argument(<master>, )), Optional(Option(-f,,0,False)), Optional(Option(,--mirror,0,False)), Argument(<name>, ), Argument(<url>, )), Required(Command(remote, False), Command(rename, False), Argument(<old>, ), Argument(<new>, )), Required(Command(remote, False), Command(rm, False), Argument(<name>, )), Required(Command(remote, False), Command(set-head, False), Argument(<name>, ), Required(Either(Option(-a,,0,False), Option(-d,,0,False), Argument(<branch>, [])))), Required(Command(remote, False), Optional(Either(Option(-v,,0,False), Option(,--verbose,0,False))), Command(show, False), Optional(Option(-n,,0,False)), Argument(<name>, )), Required(Command(remote, False), Command(prune, False), Optional(Either(Option(-n,,0,False), Option(,--dry-run,0,False))), Argument(<name>, )), Required(Command(remote, False), Optional(Either(Option(-v,,0,False), Option(,--verbose,0,False))), Command(update, False), Optional(Either(Option(-p,,0,False), Option(,--prune,0,False))), Optional(OneOrMore(Required(Either(Argument(<group>, []), Argument(<remote>, [])))))), Required(Command(remote, False), Command(set-branches, False), Argument(<name>, ), Optional(Option(,--add,0,False)), OneOrMore(Argument(<branch>, []))), Required(Command(remote, False), Command(set-url, False), Argument(<name>, ), Argument(<newurl>, ), Optional(Argument(<oldurl>, ))), Required(Command(remote, False), Command(set-url, False), Option(,--add,0,False), Argument(<name>, ), Argument(<newurl>, )), Required(Command(remote, False), Command(set-url, False), Option(,--delete,0,False), Argument(<name>, ), Argument(<url>, ))))
                var a = new RequiredMatcher(1, required.Left, required.Collected);
                while (a.Next())
                {
                    // Either(Required(Command(remote, False), Optional(Either(Option(-v,,0,False), Option(,--verbose,0,False)))), Required(Command(remote, False), Command(add, False), Optional(Option(-t,,0,False), Argument(<branch>, [])), Optional(Option(-m,,0,False), Argument(<master>, )), Optional(Option(-f,,0,False)), Optional(Option(,--mirror,0,False)), Argument(<name>, ), Argument(<url>, )), Required(Command(remote, False), Command(rename, False), Argument(<old>, ), Argument(<new>, )), Required(Command(remote, False), Command(rm, False), Argument(<name>, )), Required(Command(remote, False), Command(set-head, False), Argument(<name>, ), Required(Either(Option(-a,,0,False), Option(-d,,0,False), Argument(<branch>, [])))), Required(Command(remote, False), Optional(Either(Option(-v,,0,False), Option(,--verbose,0,False))), Command(show, False), Optional(Option(-n,,0,False)), Argument(<name>, )), Required(Command(remote, False), Command(prune, False), Optional(Either(Option(-n,,0,False), Option(,--dry-run,0,False))), Argument(<name>, )), Required(Command(remote, False), Optional(Either(Option(-v,,0,False), Option(,--verbose,0,False))), Command(update, False), Optional(Either(Option(-p,,0,False), Option(,--prune,0,False))), Optional(OneOrMore(Required(Either(Argument(<group>, []), Argument(<remote>, [])))))), Required(Command(remote, False), Command(set-branches, False), Argument(<name>, ), Optional(Option(,--add,0,False)), OneOrMore(Argument(<branch>, []))), Required(Command(remote, False), Command(set-url, False), Argument(<name>, ), Argument(<newurl>, ), Optional(Argument(<oldurl>, ))), Required(Command(remote, False), Command(set-url, False), Option(,--add,0,False), Argument(<name>, ), Argument(<newurl>, )), Required(Command(remote, False), Command(set-url, False), Option(,--delete,0,False), Argument(<name>, ), Argument(<url>, )))
                    var b = new EitherMatcher(12, a.Left, a.Collected);
                    while (b.Next())
                    {
                        switch (b.Index)
                        {
                            case 0:
                            {
                                // Required(Command(remote, False), Optional(Either(Option(-v,,0,False), Option(,--verbose,0,False))))
                                var c = new RequiredMatcher(2, b.Left, b.Collected);
                                while (c.Next())
                                {
                                    switch (c.Index)
                                    {
                                        case 0:
                                        {
                                            // Command(remote, False)
                                            c.Match(PatternMatcher.MatchCommand, "remote", ValueKind.Boolean);
                                        }
                                        break;
                                        case 1:
                                        {
                                            // Optional(Either(Option(-v,,0,False), Option(,--verbose,0,False)))
                                            var d = new OptionalMatcher(1, c.Left, c.Collected);
                                            while (d.Next())
                                            {
                                                // Either(Option(-v,,0,False), Option(,--verbose,0,False))
                                                var e = new EitherMatcher(2, d.Left, d.Collected);
                                                while (e.Next())
                                                {
                                                    switch (e.Index)
                                                    {
                                                        case 0:
                                                        {
                                                            // Option(-v,,0,False)
                                                            e.Match(PatternMatcher.MatchOption, "-v", ValueKind.Boolean);
                                                        }
                                                        break;
                                                        case 1:
                                                        {
                                                            // Option(,--verbose,0,False)
                                                            e.Match(PatternMatcher.MatchOption, "--verbose", ValueKind.Boolean);
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
                                // Required(Command(remote, False), Command(add, False), Optional(Option(-t,,0,False), Argument(<branch>, [])), Optional(Option(-m,,0,False), Argument(<master>, )), Optional(Option(-f,,0,False)), Optional(Option(,--mirror,0,False)), Argument(<name>, ), Argument(<url>, ))
                                var c = new RequiredMatcher(8, b.Left, b.Collected);
                                while (c.Next())
                                {
                                    switch (c.Index)
                                    {
                                        case 0:
                                        {
                                            // Command(remote, False)
                                            c.Match(PatternMatcher.MatchCommand, "remote", ValueKind.Boolean);
                                        }
                                        break;
                                        case 1:
                                        {
                                            // Command(add, False)
                                            c.Match(PatternMatcher.MatchCommand, "add", ValueKind.Boolean);
                                        }
                                        break;
                                        case 2:
                                        {
                                            // Optional(Option(-t,,0,False), Argument(<branch>, []))
                                            var d = new OptionalMatcher(2, c.Left, c.Collected);
                                            while (d.Next())
                                            {
                                                switch (d.Index)
                                                {
                                                    case 0:
                                                    {
                                                        // Option(-t,,0,False)
                                                        d.Match(PatternMatcher.MatchOption, "-t", ValueKind.Boolean);
                                                    }
                                                    break;
                                                    case 1:
                                                    {
                                                        // Argument(<branch>, [])
                                                        d.Match(PatternMatcher.MatchArgument, "<branch>", ValueKind.StringList);
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
                                        case 3:
                                        {
                                            // Optional(Option(-m,,0,False), Argument(<master>, ))
                                            var d = new OptionalMatcher(2, c.Left, c.Collected);
                                            while (d.Next())
                                            {
                                                switch (d.Index)
                                                {
                                                    case 0:
                                                    {
                                                        // Option(-m,,0,False)
                                                        d.Match(PatternMatcher.MatchOption, "-m", ValueKind.Boolean);
                                                    }
                                                    break;
                                                    case 1:
                                                    {
                                                        // Argument(<master>, )
                                                        d.Match(PatternMatcher.MatchArgument, "<master>", ValueKind.None);
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
                                        case 4:
                                        {
                                            // Optional(Option(-f,,0,False))
                                            var d = new OptionalMatcher(1, c.Left, c.Collected);
                                            while (d.Next())
                                            {
                                                // Option(-f,,0,False)
                                                d.Match(PatternMatcher.MatchOption, "-f", ValueKind.Boolean);
                                                if (!d.LastMatched)
                                                {
                                                    break;
                                                }
                                            }
                                            c.Fold(d.Result);
                                        }
                                        break;
                                        case 5:
                                        {
                                            // Optional(Option(,--mirror,0,False))
                                            var d = new OptionalMatcher(1, c.Left, c.Collected);
                                            while (d.Next())
                                            {
                                                // Option(,--mirror,0,False)
                                                d.Match(PatternMatcher.MatchOption, "--mirror", ValueKind.Boolean);
                                                if (!d.LastMatched)
                                                {
                                                    break;
                                                }
                                            }
                                            c.Fold(d.Result);
                                        }
                                        break;
                                        case 6:
                                        {
                                            // Argument(<name>, )
                                            c.Match(PatternMatcher.MatchArgument, "<name>", ValueKind.None);
                                        }
                                        break;
                                        case 7:
                                        {
                                            // Argument(<url>, )
                                            c.Match(PatternMatcher.MatchArgument, "<url>", ValueKind.None);
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
                            case 2:
                            {
                                // Required(Command(remote, False), Command(rename, False), Argument(<old>, ), Argument(<new>, ))
                                var c = new RequiredMatcher(4, b.Left, b.Collected);
                                while (c.Next())
                                {
                                    switch (c.Index)
                                    {
                                        case 0:
                                        {
                                            // Command(remote, False)
                                            c.Match(PatternMatcher.MatchCommand, "remote", ValueKind.Boolean);
                                        }
                                        break;
                                        case 1:
                                        {
                                            // Command(rename, False)
                                            c.Match(PatternMatcher.MatchCommand, "rename", ValueKind.Boolean);
                                        }
                                        break;
                                        case 2:
                                        {
                                            // Argument(<old>, )
                                            c.Match(PatternMatcher.MatchArgument, "<old>", ValueKind.None);
                                        }
                                        break;
                                        case 3:
                                        {
                                            // Argument(<new>, )
                                            c.Match(PatternMatcher.MatchArgument, "<new>", ValueKind.None);
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
                                // Required(Command(remote, False), Command(rm, False), Argument(<name>, ))
                                var c = new RequiredMatcher(3, b.Left, b.Collected);
                                while (c.Next())
                                {
                                    switch (c.Index)
                                    {
                                        case 0:
                                        {
                                            // Command(remote, False)
                                            c.Match(PatternMatcher.MatchCommand, "remote", ValueKind.Boolean);
                                        }
                                        break;
                                        case 1:
                                        {
                                            // Command(rm, False)
                                            c.Match(PatternMatcher.MatchCommand, "rm", ValueKind.Boolean);
                                        }
                                        break;
                                        case 2:
                                        {
                                            // Argument(<name>, )
                                            c.Match(PatternMatcher.MatchArgument, "<name>", ValueKind.None);
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
                            case 4:
                            {
                                // Required(Command(remote, False), Command(set-head, False), Argument(<name>, ), Required(Either(Option(-a,,0,False), Option(-d,,0,False), Argument(<branch>, []))))
                                var c = new RequiredMatcher(4, b.Left, b.Collected);
                                while (c.Next())
                                {
                                    switch (c.Index)
                                    {
                                        case 0:
                                        {
                                            // Command(remote, False)
                                            c.Match(PatternMatcher.MatchCommand, "remote", ValueKind.Boolean);
                                        }
                                        break;
                                        case 1:
                                        {
                                            // Command(set-head, False)
                                            c.Match(PatternMatcher.MatchCommand, "set-head", ValueKind.Boolean);
                                        }
                                        break;
                                        case 2:
                                        {
                                            // Argument(<name>, )
                                            c.Match(PatternMatcher.MatchArgument, "<name>", ValueKind.None);
                                        }
                                        break;
                                        case 3:
                                        {
                                            // Required(Either(Option(-a,,0,False), Option(-d,,0,False), Argument(<branch>, [])))
                                            var d = new RequiredMatcher(1, c.Left, c.Collected);
                                            while (d.Next())
                                            {
                                                // Either(Option(-a,,0,False), Option(-d,,0,False), Argument(<branch>, []))
                                                var e = new EitherMatcher(3, d.Left, d.Collected);
                                                while (e.Next())
                                                {
                                                    switch (e.Index)
                                                    {
                                                        case 0:
                                                        {
                                                            // Option(-a,,0,False)
                                                            e.Match(PatternMatcher.MatchOption, "-a", ValueKind.Boolean);
                                                        }
                                                        break;
                                                        case 1:
                                                        {
                                                            // Option(-d,,0,False)
                                                            e.Match(PatternMatcher.MatchOption, "-d", ValueKind.Boolean);
                                                        }
                                                        break;
                                                        case 2:
                                                        {
                                                            // Argument(<branch>, [])
                                                            e.Match(PatternMatcher.MatchArgument, "<branch>", ValueKind.StringList);
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
                                    }
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
                                // Required(Command(remote, False), Optional(Either(Option(-v,,0,False), Option(,--verbose,0,False))), Command(show, False), Optional(Option(-n,,0,False)), Argument(<name>, ))
                                var c = new RequiredMatcher(5, b.Left, b.Collected);
                                while (c.Next())
                                {
                                    switch (c.Index)
                                    {
                                        case 0:
                                        {
                                            // Command(remote, False)
                                            c.Match(PatternMatcher.MatchCommand, "remote", ValueKind.Boolean);
                                        }
                                        break;
                                        case 1:
                                        {
                                            // Optional(Either(Option(-v,,0,False), Option(,--verbose,0,False)))
                                            var d = new OptionalMatcher(1, c.Left, c.Collected);
                                            while (d.Next())
                                            {
                                                // Either(Option(-v,,0,False), Option(,--verbose,0,False))
                                                var e = new EitherMatcher(2, d.Left, d.Collected);
                                                while (e.Next())
                                                {
                                                    switch (e.Index)
                                                    {
                                                        case 0:
                                                        {
                                                            // Option(-v,,0,False)
                                                            e.Match(PatternMatcher.MatchOption, "-v", ValueKind.Boolean);
                                                        }
                                                        break;
                                                        case 1:
                                                        {
                                                            // Option(,--verbose,0,False)
                                                            e.Match(PatternMatcher.MatchOption, "--verbose", ValueKind.Boolean);
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
                                        case 2:
                                        {
                                            // Command(show, False)
                                            c.Match(PatternMatcher.MatchCommand, "show", ValueKind.Boolean);
                                        }
                                        break;
                                        case 3:
                                        {
                                            // Optional(Option(-n,,0,False))
                                            var d = new OptionalMatcher(1, c.Left, c.Collected);
                                            while (d.Next())
                                            {
                                                // Option(-n,,0,False)
                                                d.Match(PatternMatcher.MatchOption, "-n", ValueKind.Boolean);
                                                if (!d.LastMatched)
                                                {
                                                    break;
                                                }
                                            }
                                            c.Fold(d.Result);
                                        }
                                        break;
                                        case 4:
                                        {
                                            // Argument(<name>, )
                                            c.Match(PatternMatcher.MatchArgument, "<name>", ValueKind.None);
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
                            case 6:
                            {
                                // Required(Command(remote, False), Command(prune, False), Optional(Either(Option(-n,,0,False), Option(,--dry-run,0,False))), Argument(<name>, ))
                                var c = new RequiredMatcher(4, b.Left, b.Collected);
                                while (c.Next())
                                {
                                    switch (c.Index)
                                    {
                                        case 0:
                                        {
                                            // Command(remote, False)
                                            c.Match(PatternMatcher.MatchCommand, "remote", ValueKind.Boolean);
                                        }
                                        break;
                                        case 1:
                                        {
                                            // Command(prune, False)
                                            c.Match(PatternMatcher.MatchCommand, "prune", ValueKind.Boolean);
                                        }
                                        break;
                                        case 2:
                                        {
                                            // Optional(Either(Option(-n,,0,False), Option(,--dry-run,0,False)))
                                            var d = new OptionalMatcher(1, c.Left, c.Collected);
                                            while (d.Next())
                                            {
                                                // Either(Option(-n,,0,False), Option(,--dry-run,0,False))
                                                var e = new EitherMatcher(2, d.Left, d.Collected);
                                                while (e.Next())
                                                {
                                                    switch (e.Index)
                                                    {
                                                        case 0:
                                                        {
                                                            // Option(-n,,0,False)
                                                            e.Match(PatternMatcher.MatchOption, "-n", ValueKind.Boolean);
                                                        }
                                                        break;
                                                        case 1:
                                                        {
                                                            // Option(,--dry-run,0,False)
                                                            e.Match(PatternMatcher.MatchOption, "--dry-run", ValueKind.Boolean);
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
                                        case 3:
                                        {
                                            // Argument(<name>, )
                                            c.Match(PatternMatcher.MatchArgument, "<name>", ValueKind.None);
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
                            case 7:
                            {
                                // Required(Command(remote, False), Optional(Either(Option(-v,,0,False), Option(,--verbose,0,False))), Command(update, False), Optional(Either(Option(-p,,0,False), Option(,--prune,0,False))), Optional(OneOrMore(Required(Either(Argument(<group>, []), Argument(<remote>, []))))))
                                var c = new RequiredMatcher(5, b.Left, b.Collected);
                                while (c.Next())
                                {
                                    switch (c.Index)
                                    {
                                        case 0:
                                        {
                                            // Command(remote, False)
                                            c.Match(PatternMatcher.MatchCommand, "remote", ValueKind.Boolean);
                                        }
                                        break;
                                        case 1:
                                        {
                                            // Optional(Either(Option(-v,,0,False), Option(,--verbose,0,False)))
                                            var d = new OptionalMatcher(1, c.Left, c.Collected);
                                            while (d.Next())
                                            {
                                                // Either(Option(-v,,0,False), Option(,--verbose,0,False))
                                                var e = new EitherMatcher(2, d.Left, d.Collected);
                                                while (e.Next())
                                                {
                                                    switch (e.Index)
                                                    {
                                                        case 0:
                                                        {
                                                            // Option(-v,,0,False)
                                                            e.Match(PatternMatcher.MatchOption, "-v", ValueKind.Boolean);
                                                        }
                                                        break;
                                                        case 1:
                                                        {
                                                            // Option(,--verbose,0,False)
                                                            e.Match(PatternMatcher.MatchOption, "--verbose", ValueKind.Boolean);
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
                                        case 2:
                                        {
                                            // Command(update, False)
                                            c.Match(PatternMatcher.MatchCommand, "update", ValueKind.Boolean);
                                        }
                                        break;
                                        case 3:
                                        {
                                            // Optional(Either(Option(-p,,0,False), Option(,--prune,0,False)))
                                            var d = new OptionalMatcher(1, c.Left, c.Collected);
                                            while (d.Next())
                                            {
                                                // Either(Option(-p,,0,False), Option(,--prune,0,False))
                                                var e = new EitherMatcher(2, d.Left, d.Collected);
                                                while (e.Next())
                                                {
                                                    switch (e.Index)
                                                    {
                                                        case 0:
                                                        {
                                                            // Option(-p,,0,False)
                                                            e.Match(PatternMatcher.MatchOption, "-p", ValueKind.Boolean);
                                                        }
                                                        break;
                                                        case 1:
                                                        {
                                                            // Option(,--prune,0,False)
                                                            e.Match(PatternMatcher.MatchOption, "--prune", ValueKind.Boolean);
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
                                        case 4:
                                        {
                                            // Optional(OneOrMore(Required(Either(Argument(<group>, []), Argument(<remote>, [])))))
                                            var d = new OptionalMatcher(1, c.Left, c.Collected);
                                            while (d.Next())
                                            {
                                                // OneOrMore(Required(Either(Argument(<group>, []), Argument(<remote>, []))))
                                                var e = new OneOrMoreMatcher(1, d.Left, d.Collected);
                                                while (e.Next())
                                                {
                                                    // Required(Either(Argument(<group>, []), Argument(<remote>, [])))
                                                    var f = new RequiredMatcher(1, e.Left, e.Collected);
                                                    while (f.Next())
                                                    {
                                                        // Either(Argument(<group>, []), Argument(<remote>, []))
                                                        var g = new EitherMatcher(2, f.Left, f.Collected);
                                                        while (g.Next())
                                                        {
                                                            switch (g.Index)
                                                            {
                                                                case 0:
                                                                {
                                                                    // Argument(<group>, [])
                                                                    g.Match(PatternMatcher.MatchArgument, "<group>", ValueKind.StringList);
                                                                }
                                                                break;
                                                                case 1:
                                                                {
                                                                    // Argument(<remote>, [])
                                                                    g.Match(PatternMatcher.MatchArgument, "<remote>", ValueKind.StringList);
                                                                }
                                                                break;
                                                            }
                                                            if (!g.LastMatched)
                                                            {
                                                                break;
                                                            }
                                                        }
                                                        f.Fold(g.Result);
                                                        if (!f.LastMatched)
                                                        {
                                                            break;
                                                        }
                                                    }
                                                    e.Fold(f.Result);
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
                            case 8:
                            {
                                // Required(Command(remote, False), Command(set-branches, False), Argument(<name>, ), Optional(Option(,--add,0,False)), OneOrMore(Argument(<branch>, [])))
                                var c = new RequiredMatcher(5, b.Left, b.Collected);
                                while (c.Next())
                                {
                                    switch (c.Index)
                                    {
                                        case 0:
                                        {
                                            // Command(remote, False)
                                            c.Match(PatternMatcher.MatchCommand, "remote", ValueKind.Boolean);
                                        }
                                        break;
                                        case 1:
                                        {
                                            // Command(set-branches, False)
                                            c.Match(PatternMatcher.MatchCommand, "set-branches", ValueKind.Boolean);
                                        }
                                        break;
                                        case 2:
                                        {
                                            // Argument(<name>, )
                                            c.Match(PatternMatcher.MatchArgument, "<name>", ValueKind.None);
                                        }
                                        break;
                                        case 3:
                                        {
                                            // Optional(Option(,--add,0,False))
                                            var d = new OptionalMatcher(1, c.Left, c.Collected);
                                            while (d.Next())
                                            {
                                                // Option(,--add,0,False)
                                                d.Match(PatternMatcher.MatchOption, "--add", ValueKind.Boolean);
                                                if (!d.LastMatched)
                                                {
                                                    break;
                                                }
                                            }
                                            c.Fold(d.Result);
                                        }
                                        break;
                                        case 4:
                                        {
                                            // OneOrMore(Argument(<branch>, []))
                                            var d = new OneOrMoreMatcher(1, c.Left, c.Collected);
                                            while (d.Next())
                                            {
                                                // Argument(<branch>, [])
                                                d.Match(PatternMatcher.MatchArgument, "<branch>", ValueKind.StringList);
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
                            case 9:
                            {
                                // Required(Command(remote, False), Command(set-url, False), Argument(<name>, ), Argument(<newurl>, ), Optional(Argument(<oldurl>, )))
                                var c = new RequiredMatcher(5, b.Left, b.Collected);
                                while (c.Next())
                                {
                                    switch (c.Index)
                                    {
                                        case 0:
                                        {
                                            // Command(remote, False)
                                            c.Match(PatternMatcher.MatchCommand, "remote", ValueKind.Boolean);
                                        }
                                        break;
                                        case 1:
                                        {
                                            // Command(set-url, False)
                                            c.Match(PatternMatcher.MatchCommand, "set-url", ValueKind.Boolean);
                                        }
                                        break;
                                        case 2:
                                        {
                                            // Argument(<name>, )
                                            c.Match(PatternMatcher.MatchArgument, "<name>", ValueKind.None);
                                        }
                                        break;
                                        case 3:
                                        {
                                            // Argument(<newurl>, )
                                            c.Match(PatternMatcher.MatchArgument, "<newurl>", ValueKind.None);
                                        }
                                        break;
                                        case 4:
                                        {
                                            // Optional(Argument(<oldurl>, ))
                                            var d = new OptionalMatcher(1, c.Left, c.Collected);
                                            while (d.Next())
                                            {
                                                // Argument(<oldurl>, )
                                                d.Match(PatternMatcher.MatchArgument, "<oldurl>", ValueKind.None);
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
                            case 10:
                            {
                                // Required(Command(remote, False), Command(set-url, False), Option(,--add,0,False), Argument(<name>, ), Argument(<newurl>, ))
                                var c = new RequiredMatcher(5, b.Left, b.Collected);
                                while (c.Next())
                                {
                                    switch (c.Index)
                                    {
                                        case 0:
                                        {
                                            // Command(remote, False)
                                            c.Match(PatternMatcher.MatchCommand, "remote", ValueKind.Boolean);
                                        }
                                        break;
                                        case 1:
                                        {
                                            // Command(set-url, False)
                                            c.Match(PatternMatcher.MatchCommand, "set-url", ValueKind.Boolean);
                                        }
                                        break;
                                        case 2:
                                        {
                                            // Option(,--add,0,False)
                                            c.Match(PatternMatcher.MatchOption, "--add", ValueKind.Boolean);
                                        }
                                        break;
                                        case 3:
                                        {
                                            // Argument(<name>, )
                                            c.Match(PatternMatcher.MatchArgument, "<name>", ValueKind.None);
                                        }
                                        break;
                                        case 4:
                                        {
                                            // Argument(<newurl>, )
                                            c.Match(PatternMatcher.MatchArgument, "<newurl>", ValueKind.None);
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
                            case 11:
                            {
                                // Required(Command(remote, False), Command(set-url, False), Option(,--delete,0,False), Argument(<name>, ), Argument(<url>, ))
                                var c = new RequiredMatcher(5, b.Left, b.Collected);
                                while (c.Next())
                                {
                                    switch (c.Index)
                                    {
                                        case 0:
                                        {
                                            // Command(remote, False)
                                            c.Match(PatternMatcher.MatchCommand, "remote", ValueKind.Boolean);
                                        }
                                        break;
                                        case 1:
                                        {
                                            // Command(set-url, False)
                                            c.Match(PatternMatcher.MatchCommand, "set-url", ValueKind.Boolean);
                                        }
                                        break;
                                        case 2:
                                        {
                                            // Option(,--delete,0,False)
                                            c.Match(PatternMatcher.MatchOption, "--delete", ValueKind.Boolean);
                                        }
                                        break;
                                        case 3:
                                        {
                                            // Argument(<name>, )
                                            c.Match(PatternMatcher.MatchArgument, "<name>", ValueKind.None);
                                        }
                                        break;
                                        case 4:
                                        {
                                            // Argument(<url>, )
                                            c.Match(PatternMatcher.MatchArgument, "<url>", ValueKind.None);
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
            yield return KeyValuePair.Create("remote", (object?)CmdRemote);
            yield return KeyValuePair.Create("-v", (object?)OptV);
            yield return KeyValuePair.Create("--verbose", (object?)OptVerbose);
            yield return KeyValuePair.Create("add", (object?)CmdAdd);
            yield return KeyValuePair.Create("-t", (object?)OptT);
            yield return KeyValuePair.Create("<branch>", (object?)ArgBranch);
            yield return KeyValuePair.Create("-m", (object?)OptM);
            yield return KeyValuePair.Create("<master>", (object?)ArgMaster);
            yield return KeyValuePair.Create("-f", (object?)OptF);
            yield return KeyValuePair.Create("--mirror", (object?)OptMirror);
            yield return KeyValuePair.Create("<name>", (object?)ArgName);
            yield return KeyValuePair.Create("<url>", (object?)ArgUrl);
            yield return KeyValuePair.Create("rename", (object?)CmdRename);
            yield return KeyValuePair.Create("<old>", (object?)ArgOld);
            yield return KeyValuePair.Create("<new>", (object?)ArgNew);
            yield return KeyValuePair.Create("rm", (object?)CmdRm);
            yield return KeyValuePair.Create("set-head", (object?)CmdSetHead);
            yield return KeyValuePair.Create("-a", (object?)OptA);
            yield return KeyValuePair.Create("-d", (object?)OptD);
            yield return KeyValuePair.Create("show", (object?)CmdShow);
            yield return KeyValuePair.Create("-n", (object?)OptN);
            yield return KeyValuePair.Create("prune", (object?)CmdPrune);
            yield return KeyValuePair.Create("--dry-run", (object?)OptDryRun);
            yield return KeyValuePair.Create("update", (object?)CmdUpdate);
            yield return KeyValuePair.Create("-p", (object?)OptP);
            yield return KeyValuePair.Create("--prune", (object?)OptPrune);
            yield return KeyValuePair.Create("<group>", (object?)ArgGroup);
            yield return KeyValuePair.Create("<remote>", (object?)ArgRemote);
            yield return KeyValuePair.Create("set-branches", (object?)CmdSetBranches);
            yield return KeyValuePair.Create("--add", (object?)OptAdd);
            yield return KeyValuePair.Create("set-url", (object?)CmdSetUrl);
            yield return KeyValuePair.Create("<newurl>", (object?)ArgNewurl);
            yield return KeyValuePair.Create("<oldurl>", (object?)ArgOldurl);
            yield return KeyValuePair.Create("--delete", (object?)OptDelete);
        }

        IEnumerator<KeyValuePair<string, object?>> IEnumerable<KeyValuePair<string, object?>>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary><c>Command(remote, False)</c></summary>
        public bool CmdRemote { get; private set; }

        /// <summary><c>Option(-v,,0,False)</c></summary>
        public bool OptV { get; private set; }

        /// <summary><c>Option(,--verbose,0,False)</c></summary>
        public bool OptVerbose { get; private set; }

        /// <summary><c>Command(add, False)</c></summary>
        public bool CmdAdd { get; private set; }

        /// <summary><c>Option(-t,,0,False)</c></summary>
        public bool OptT { get; private set; }

        /// <summary><c>Argument(&lt;branch&gt;, [])</c></summary>
        public StringList ArgBranch { get; private set; } = StringList.Empty;

        /// <summary><c>Option(-m,,0,False)</c></summary>
        public bool OptM { get; private set; }

        /// <summary><c>Argument(&lt;master&gt;, )</c></summary>
        public string? ArgMaster { get; private set; }

        /// <summary><c>Option(-f,,0,False)</c></summary>
        public bool OptF { get; private set; }

        /// <summary><c>Option(,--mirror,0,False)</c></summary>
        public bool OptMirror { get; private set; }

        /// <summary><c>Argument(&lt;name&gt;, )</c></summary>
        public string? ArgName { get; private set; }

        /// <summary><c>Argument(&lt;url&gt;, )</c></summary>
        public string? ArgUrl { get; private set; }

        /// <summary><c>Command(rename, False)</c></summary>
        public bool CmdRename { get; private set; }

        /// <summary><c>Argument(&lt;old&gt;, )</c></summary>
        public string? ArgOld { get; private set; }

        /// <summary><c>Argument(&lt;new&gt;, )</c></summary>
        public string? ArgNew { get; private set; }

        /// <summary><c>Command(rm, False)</c></summary>
        public bool CmdRm { get; private set; }

        /// <summary><c>Command(set-head, False)</c></summary>
        public bool CmdSetHead { get; private set; }

        /// <summary><c>Option(-a,,0,False)</c></summary>
        public bool OptA { get; private set; }

        /// <summary><c>Option(-d,,0,False)</c></summary>
        public bool OptD { get; private set; }

        /// <summary><c>Command(show, False)</c></summary>
        public bool CmdShow { get; private set; }

        /// <summary><c>Option(-n,,0,False)</c></summary>
        public bool OptN { get; private set; }

        /// <summary><c>Command(prune, False)</c></summary>
        public bool CmdPrune { get; private set; }

        /// <summary><c>Option(,--dry-run,0,False)</c></summary>
        public bool OptDryRun { get; private set; }

        /// <summary><c>Command(update, False)</c></summary>
        public bool CmdUpdate { get; private set; }

        /// <summary><c>Option(-p,,0,False)</c></summary>
        public bool OptP { get; private set; }

        /// <summary><c>Option(,--prune,0,False)</c></summary>
        public bool OptPrune { get; private set; }

        /// <summary><c>Argument(&lt;group&gt;, [])</c></summary>
        public StringList ArgGroup { get; private set; } = StringList.Empty;

        /// <summary><c>Argument(&lt;remote&gt;, [])</c></summary>
        public StringList ArgRemote { get; private set; } = StringList.Empty;

        /// <summary><c>Command(set-branches, False)</c></summary>
        public bool CmdSetBranches { get; private set; }

        /// <summary><c>Option(,--add,0,False)</c></summary>
        public bool OptAdd { get; private set; }

        /// <summary><c>Command(set-url, False)</c></summary>
        public bool CmdSetUrl { get; private set; }

        /// <summary><c>Argument(&lt;newurl&gt;, )</c></summary>
        public string? ArgNewurl { get; private set; }

        /// <summary><c>Argument(&lt;oldurl&gt;, )</c></summary>
        public string? ArgOldurl { get; private set; }

        /// <summary><c>Option(,--delete,0,False)</c></summary>
        public bool OptDelete { get; private set; }
    }
}
