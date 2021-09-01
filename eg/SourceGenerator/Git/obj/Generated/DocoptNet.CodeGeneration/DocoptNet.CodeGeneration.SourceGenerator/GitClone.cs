#nullable enable annotations

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DocoptNet.Generated;
using Leaves = DocoptNet.Generated.ReadOnlyList<DocoptNet.Generated.LeafPattern>;
using static DocoptNet.Generated.GeneratedSourceModule;

namespace Git
{
    partial class GitCloneArguments : IEnumerable<KeyValuePair<string, object?>>
    {
        public const string Help = @"usage: git clone [options] [--] <repo> [<dir>]
    -v, --verbose         be more verbose
    -q, --quiet           be more quiet
    --progress            force progress reporting
    -n, --no-checkout     don't create a checkout
    --bare                create a bare repository
    --mirror              create a mirror repository (implies bare)
    -l, --local           to clone from a local repository
    --no-hardlinks        don't use local hardlinks, always copy
    -s, --shared          setup as shared repository
    --recursive           initialize submodules in the clone
    --recurse-submodules  initialize submodules in the clone
    --template <template-directory>
                          directory from which templates will be used
    --reference <repo>    reference repository
    -o, --origin <branch>
                          use <branch> instead of 'origin' to track upstream
    -b, --branch <branch>
                          checkout <branch> instead of the remote's HEAD
    -u, --upload-pack <path>
                          path to git-upload-pack on the remote
    --depth <depth>       create a shallow clone of that depth
";

        public const string Usage = @"usage: git clone [options] [--] <repo> [<dir>]
    -v, --verbose         be more verbose
    -q, --quiet           be more quiet
    --progress            force progress reporting
    -n, --no-checkout     don't create a checkout
    --bare                create a bare repository
    --mirror              create a mirror repository (implies bare)
    -l, --local           to clone from a local repository
    --no-hardlinks        don't use local hardlinks, always copy
    -s, --shared          setup as shared repository
    --recursive           initialize submodules in the clone
    --recurse-submodules  initialize submodules in the clone
    --template <template-directory>
                          directory from which templates will be used
    --reference <repo>    reference repository
    -o, --origin <branch>
                          use <branch> instead of 'origin' to track upstream
    -b, --branch <branch>
                          checkout <branch> instead of the remote's HEAD
    -u, --upload-pack <path>
                          path to git-upload-pack on the remote
    --depth <depth>       create a shallow clone of that depth";

        public static GitCloneArguments Apply(IEnumerable<string> args, bool help = true, object? version = null, bool optionsFirst = false, bool exit = false)
        {
            var options = new List<Option>
            {
                new Option("-v", null, 0, false),
                new Option("-,", null, 0, 0),
                new Option(null, "--verbose", 0, false),
                new Option("-q", null, 0, false),
                new Option(null, "--quiet", 0, false),
                new Option(null, "--progress", 0, false),
                new Option("-n", null, 0, false),
                new Option(null, "--no-checkout", 0, false),
                new Option(null, "--bare", 0, false),
                new Option(null, "--mirror", 0, false),
                new Option("-l", null, 0, false),
                new Option(null, "--local", 0, false),
                new Option(null, "--no-hardlinks", 0, false),
                new Option("-s", null, 0, false),
                new Option(null, "--shared", 0, false),
                new Option(null, "--recursive", 0, false),
                new Option(null, "--recurse-submodules", 0, false),
                new Option(null, "--template", 0, false),
                new Option(null, "--reference", 0, false),
                new Option("-o", null, 0, false),
                new Option(null, "--origin", 0, false),
                new Option("-b", null, 0, false),
                new Option(null, "--branch", 0, false),
                new Option("-u", null, 0, false),
                new Option(null, "--upload-pack", 0, false),
                new Option(null, "--depth", 0, false),
            };
            var left = ParseArgv(Help, args, options, optionsFirst, help, version);
            var required = new RequiredMatcher(1, left, new Leaves());
            Match(ref required);
            var collected = GetSuccessfulCollection(required, Usage);
            var result = new GitCloneArguments();

            foreach (var leaf in collected)
            {
                var value = leaf.Value is { IsStringList: true } ? ((StringList)leaf.Value).Reverse() : leaf.Value;
                switch (leaf.Name)
                {
                    case "clone": result.CmdClone = (int)value; break;
                    case "--": result.Cmd = (bool)value; break;
                    case "<repo>": result.ArgRepo = (StringList)value; break;
                    case "<dir>": result.ArgDir = (string?)value; break;
                    case "-v": result.OptV = (bool)value; break;
                    case "-,": result.OptComma = (int)value; break;
                    case "--verbose": result.OptVerbose = (bool)value; break;
                    case "be": result.CmdBe = (int)value; break;
                    case "more": result.CmdMore = (int)value; break;
                    case "verbose": result.CmdVerbose = (bool)value; break;
                    case "-q": result.OptQ = (bool)value; break;
                    case "--quiet": result.OptQuiet = (bool)value; break;
                    case "quiet": result.CmdQuiet = (bool)value; break;
                    case "--progress": result.OptProgress = (bool)value; break;
                    case "force": result.CmdForce = (bool)value; break;
                    case "progress": result.CmdProgress = (bool)value; break;
                    case "reporting": result.CmdReporting = (bool)value; break;
                    case "-n": result.OptN = (bool)value; break;
                    case "--no-checkout": result.OptNoCheckout = (bool)value; break;
                    case "don't": result.CmdDont = (int)value; break;
                    case "create": result.CmdCreate = (int)value; break;
                    case "a": result.CmdA = (int)value; break;
                    case "checkout": result.CmdCheckout = (int)value; break;
                    case "--bare": result.OptBare = (bool)value; break;
                    case "bare": result.CmdBare = (int)value; break;
                    case "repository": result.CmdRepository = (int)value; break;
                    case "--mirror": result.OptMirror = (bool)value; break;
                    case "mirror": result.CmdMirror = (bool)value; break;
                    case "implies": result.CmdImplies = (bool)value; break;
                    case "-l": result.OptL = (bool)value; break;
                    case "--local": result.OptLocal = (bool)value; break;
                    case "to": result.CmdTo = (int)value; break;
                    case "from": result.CmdFrom = (int)value; break;
                    case "local": result.CmdLocal = (int)value; break;
                    case "--no-hardlinks": result.OptNoHardlinks = (bool)value; break;
                    case "use": result.CmdUse = (int)value; break;
                    case "hardlinks,": result.CmdHardlinksComma = (bool)value; break;
                    case "always": result.CmdAlways = (bool)value; break;
                    case "copy": result.CmdCopy = (bool)value; break;
                    case "-s": result.OptS = (bool)value; break;
                    case "--shared": result.OptShared = (bool)value; break;
                    case "setup": result.CmdSetup = (bool)value; break;
                    case "as": result.CmdAs = (bool)value; break;
                    case "shared": result.CmdShared = (bool)value; break;
                    case "--recursive": result.OptRecursive = (bool)value; break;
                    case "initialize": result.CmdInitialize = (int)value; break;
                    case "submodules": result.CmdSubmodules = (int)value; break;
                    case "in": result.CmdIn = (int)value; break;
                    case "the": result.CmdThe = (int)value; break;
                    case "--recurse-submodules": result.OptRecurseSubmodules = (bool)value; break;
                    case "--template": result.OptTemplate = (bool)value; break;
                    case "<template-directory>": result.ArgTemplateDirectory = (string?)value; break;
                    case "directory": result.CmdDirectory = (bool)value; break;
                    case "which": result.CmdWhich = (bool)value; break;
                    case "templates": result.CmdTemplates = (bool)value; break;
                    case "will": result.CmdWill = (bool)value; break;
                    case "used": result.CmdUsed = (bool)value; break;
                    case "--reference": result.OptReference = (bool)value; break;
                    case "reference": result.CmdReference = (bool)value; break;
                    case "-o": result.OptO = (bool)value; break;
                    case "--origin": result.OptOrigin = (bool)value; break;
                    case "<branch>": result.ArgBranch = (StringList)value; break;
                    case "instead": result.CmdInstead = (int)value; break;
                    case "of": result.CmdOf = (int)value; break;
                    case "'origin'": result.CmdOrigin = (bool)value; break;
                    case "track": result.CmdTrack = (bool)value; break;
                    case "upstream": result.CmdUpstream = (bool)value; break;
                    case "-b": result.OptB = (bool)value; break;
                    case "--branch": result.OptBranch = (bool)value; break;
                    case "remote's": result.CmdRemotes = (bool)value; break;
                    case "HEAD": result.ArgHead = (string?)value; break;
                    case "-u": result.OptU = (bool)value; break;
                    case "--upload-pack": result.OptUploadPack = (bool)value; break;
                    case "<path>": result.ArgPath = (string?)value; break;
                    case "path": result.CmdPath = (bool)value; break;
                    case "git-upload-pack": result.CmdGitUploadPack = (bool)value; break;
                    case "on": result.CmdOn = (bool)value; break;
                    case "remote": result.CmdRemote = (bool)value; break;
                    case "--depth": result.OptDepth = (bool)value; break;
                    case "<depth>": result.ArgDepth = (string?)value; break;
                    case "shallow": result.CmdShallow = (bool)value; break;
                    case "that": result.CmdThat = (bool)value; break;
                    case "depth": result.CmdDepth = (bool)value; break;
                }
            }

            return result;

            static void Match(ref RequiredMatcher required)
            {
                // Required(Required(Command(clone, 0), Optional(OptionsShortcut()), Optional(Command(--, False)), Argument(<repo>, []), Optional(Argument(<dir>, )), Option(-v,,0,False), Option(-,,,0,0), Option(,--verbose,0,False), Command(be, 0), Command(more, 0), Command(verbose, False), Option(-q,,0,False), Option(-,,,0,0), Option(,--quiet,0,False), Command(be, 0), Command(more, 0), Command(quiet, False), Option(,--progress,0,False), Command(force, False), Command(progress, False), Command(reporting, False), Option(-n,,0,False), Option(-,,,0,0), Option(,--no-checkout,0,False), Command(don't, 0), Command(create, 0), Command(a, 0), Command(checkout, 0), Option(,--bare,0,False), Command(create, 0), Command(a, 0), Command(bare, 0), Command(repository, 0), Option(,--mirror,0,False), Command(create, 0), Command(a, 0), Command(mirror, False), Command(repository, 0), Required(Command(implies, False), Command(bare, 0)), Option(-l,,0,False), Option(-,,,0,0), Option(,--local,0,False), Command(to, 0), Command(clone, 0), Command(from, 0), Command(a, 0), Command(local, 0), Command(repository, 0), Option(,--no-hardlinks,0,False), Command(don't, 0), Command(use, 0), Command(local, 0), Command(hardlinks,, False), Command(always, False), Command(copy, False), Option(-s,,0,False), Option(-,,,0,0), Option(,--shared,0,False), Command(setup, False), Command(as, False), Command(shared, False), Command(repository, 0), Option(,--recursive,0,False), Command(initialize, 0), Command(submodules, 0), Command(in, 0), Command(the, 0), Command(clone, 0), Option(,--recurse-submodules,0,False), Command(initialize, 0), Command(submodules, 0), Command(in, 0), Command(the, 0), Command(clone, 0), Option(,--template,0,False), Argument(<template-directory>, ), Command(directory, False), Command(from, 0), Command(which, False), Command(templates, False), Command(will, False), Command(be, 0), Command(used, False), Option(,--reference,0,False), Argument(<repo>, []), Command(reference, False), Command(repository, 0), Option(-o,,0,False), Option(-,,,0,0), Option(,--origin,0,False), Argument(<branch>, []), Command(use, 0), Argument(<branch>, []), Command(instead, 0), Command(of, 0), Command('origin', False), Command(to, 0), Command(track, False), Command(upstream, False), Option(-b,,0,False), Option(-,,,0,0), Option(,--branch,0,False), Argument(<branch>, []), Command(checkout, 0), Argument(<branch>, []), Command(instead, 0), Command(of, 0), Command(the, 0), Command(remote's, False), Argument(HEAD, ), Option(-u,,0,False), Option(-,,,0,0), Option(,--upload-pack,0,False), Argument(<path>, ), Command(path, False), Command(to, 0), Command(git-upload-pack, False), Command(on, False), Command(the, 0), Command(remote, False), Option(,--depth,0,False), Argument(<depth>, ), Command(create, 0), Command(a, 0), Command(shallow, False), Command(clone, 0), Command(of, 0), Command(that, False), Command(depth, False)))
                var a = new RequiredMatcher(1, required.Left, required.Collected);
                while (a.Next())
                {
                    // Required(Command(clone, 0), Optional(OptionsShortcut()), Optional(Command(--, False)), Argument(<repo>, []), Optional(Argument(<dir>, )), Option(-v,,0,False), Option(-,,,0,0), Option(,--verbose,0,False), Command(be, 0), Command(more, 0), Command(verbose, False), Option(-q,,0,False), Option(-,,,0,0), Option(,--quiet,0,False), Command(be, 0), Command(more, 0), Command(quiet, False), Option(,--progress,0,False), Command(force, False), Command(progress, False), Command(reporting, False), Option(-n,,0,False), Option(-,,,0,0), Option(,--no-checkout,0,False), Command(don't, 0), Command(create, 0), Command(a, 0), Command(checkout, 0), Option(,--bare,0,False), Command(create, 0), Command(a, 0), Command(bare, 0), Command(repository, 0), Option(,--mirror,0,False), Command(create, 0), Command(a, 0), Command(mirror, False), Command(repository, 0), Required(Command(implies, False), Command(bare, 0)), Option(-l,,0,False), Option(-,,,0,0), Option(,--local,0,False), Command(to, 0), Command(clone, 0), Command(from, 0), Command(a, 0), Command(local, 0), Command(repository, 0), Option(,--no-hardlinks,0,False), Command(don't, 0), Command(use, 0), Command(local, 0), Command(hardlinks,, False), Command(always, False), Command(copy, False), Option(-s,,0,False), Option(-,,,0,0), Option(,--shared,0,False), Command(setup, False), Command(as, False), Command(shared, False), Command(repository, 0), Option(,--recursive,0,False), Command(initialize, 0), Command(submodules, 0), Command(in, 0), Command(the, 0), Command(clone, 0), Option(,--recurse-submodules,0,False), Command(initialize, 0), Command(submodules, 0), Command(in, 0), Command(the, 0), Command(clone, 0), Option(,--template,0,False), Argument(<template-directory>, ), Command(directory, False), Command(from, 0), Command(which, False), Command(templates, False), Command(will, False), Command(be, 0), Command(used, False), Option(,--reference,0,False), Argument(<repo>, []), Command(reference, False), Command(repository, 0), Option(-o,,0,False), Option(-,,,0,0), Option(,--origin,0,False), Argument(<branch>, []), Command(use, 0), Argument(<branch>, []), Command(instead, 0), Command(of, 0), Command('origin', False), Command(to, 0), Command(track, False), Command(upstream, False), Option(-b,,0,False), Option(-,,,0,0), Option(,--branch,0,False), Argument(<branch>, []), Command(checkout, 0), Argument(<branch>, []), Command(instead, 0), Command(of, 0), Command(the, 0), Command(remote's, False), Argument(HEAD, ), Option(-u,,0,False), Option(-,,,0,0), Option(,--upload-pack,0,False), Argument(<path>, ), Command(path, False), Command(to, 0), Command(git-upload-pack, False), Command(on, False), Command(the, 0), Command(remote, False), Option(,--depth,0,False), Argument(<depth>, ), Command(create, 0), Command(a, 0), Command(shallow, False), Command(clone, 0), Command(of, 0), Command(that, False), Command(depth, False))
                    var b = new RequiredMatcher(129, a.Left, a.Collected);
                    while (b.Next())
                    {
                        switch (b.Index)
                        {
                            case 0:
                            {
                                // Command(clone, 0)
                                b.Match(PatternMatcher.MatchCommand, "clone", ValueKind.Integer);
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
                                // Argument(<repo>, [])
                                b.Match(PatternMatcher.MatchArgument, "<repo>", ValueKind.StringList);
                            }
                            break;
                            case 4:
                            {
                                // Optional(Argument(<dir>, ))
                                var c = new OptionalMatcher(1, b.Left, b.Collected);
                                while (c.Next())
                                {
                                    // Argument(<dir>, )
                                    c.Match(PatternMatcher.MatchArgument, "<dir>", ValueKind.None);
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
                                // Option(-v,,0,False)
                                b.Match(PatternMatcher.MatchOption, "-v", ValueKind.Boolean);
                            }
                            break;
                            case 6:
                            {
                                // Option(-,,,0,0)
                                b.Match(PatternMatcher.MatchOption, "-,", ValueKind.Integer);
                            }
                            break;
                            case 7:
                            {
                                // Option(,--verbose,0,False)
                                b.Match(PatternMatcher.MatchOption, "--verbose", ValueKind.Boolean);
                            }
                            break;
                            case 8:
                            {
                                // Command(be, 0)
                                b.Match(PatternMatcher.MatchCommand, "be", ValueKind.Integer);
                            }
                            break;
                            case 9:
                            {
                                // Command(more, 0)
                                b.Match(PatternMatcher.MatchCommand, "more", ValueKind.Integer);
                            }
                            break;
                            case 10:
                            {
                                // Command(verbose, False)
                                b.Match(PatternMatcher.MatchCommand, "verbose", ValueKind.Boolean);
                            }
                            break;
                            case 11:
                            {
                                // Option(-q,,0,False)
                                b.Match(PatternMatcher.MatchOption, "-q", ValueKind.Boolean);
                            }
                            break;
                            case 12:
                            {
                                // Option(-,,,0,0)
                                b.Match(PatternMatcher.MatchOption, "-,", ValueKind.Integer);
                            }
                            break;
                            case 13:
                            {
                                // Option(,--quiet,0,False)
                                b.Match(PatternMatcher.MatchOption, "--quiet", ValueKind.Boolean);
                            }
                            break;
                            case 14:
                            {
                                // Command(be, 0)
                                b.Match(PatternMatcher.MatchCommand, "be", ValueKind.Integer);
                            }
                            break;
                            case 15:
                            {
                                // Command(more, 0)
                                b.Match(PatternMatcher.MatchCommand, "more", ValueKind.Integer);
                            }
                            break;
                            case 16:
                            {
                                // Command(quiet, False)
                                b.Match(PatternMatcher.MatchCommand, "quiet", ValueKind.Boolean);
                            }
                            break;
                            case 17:
                            {
                                // Option(,--progress,0,False)
                                b.Match(PatternMatcher.MatchOption, "--progress", ValueKind.Boolean);
                            }
                            break;
                            case 18:
                            {
                                // Command(force, False)
                                b.Match(PatternMatcher.MatchCommand, "force", ValueKind.Boolean);
                            }
                            break;
                            case 19:
                            {
                                // Command(progress, False)
                                b.Match(PatternMatcher.MatchCommand, "progress", ValueKind.Boolean);
                            }
                            break;
                            case 20:
                            {
                                // Command(reporting, False)
                                b.Match(PatternMatcher.MatchCommand, "reporting", ValueKind.Boolean);
                            }
                            break;
                            case 21:
                            {
                                // Option(-n,,0,False)
                                b.Match(PatternMatcher.MatchOption, "-n", ValueKind.Boolean);
                            }
                            break;
                            case 22:
                            {
                                // Option(-,,,0,0)
                                b.Match(PatternMatcher.MatchOption, "-,", ValueKind.Integer);
                            }
                            break;
                            case 23:
                            {
                                // Option(,--no-checkout,0,False)
                                b.Match(PatternMatcher.MatchOption, "--no-checkout", ValueKind.Boolean);
                            }
                            break;
                            case 24:
                            {
                                // Command(don't, 0)
                                b.Match(PatternMatcher.MatchCommand, "don't", ValueKind.Integer);
                            }
                            break;
                            case 25:
                            {
                                // Command(create, 0)
                                b.Match(PatternMatcher.MatchCommand, "create", ValueKind.Integer);
                            }
                            break;
                            case 26:
                            {
                                // Command(a, 0)
                                b.Match(PatternMatcher.MatchCommand, "a", ValueKind.Integer);
                            }
                            break;
                            case 27:
                            {
                                // Command(checkout, 0)
                                b.Match(PatternMatcher.MatchCommand, "checkout", ValueKind.Integer);
                            }
                            break;
                            case 28:
                            {
                                // Option(,--bare,0,False)
                                b.Match(PatternMatcher.MatchOption, "--bare", ValueKind.Boolean);
                            }
                            break;
                            case 29:
                            {
                                // Command(create, 0)
                                b.Match(PatternMatcher.MatchCommand, "create", ValueKind.Integer);
                            }
                            break;
                            case 30:
                            {
                                // Command(a, 0)
                                b.Match(PatternMatcher.MatchCommand, "a", ValueKind.Integer);
                            }
                            break;
                            case 31:
                            {
                                // Command(bare, 0)
                                b.Match(PatternMatcher.MatchCommand, "bare", ValueKind.Integer);
                            }
                            break;
                            case 32:
                            {
                                // Command(repository, 0)
                                b.Match(PatternMatcher.MatchCommand, "repository", ValueKind.Integer);
                            }
                            break;
                            case 33:
                            {
                                // Option(,--mirror,0,False)
                                b.Match(PatternMatcher.MatchOption, "--mirror", ValueKind.Boolean);
                            }
                            break;
                            case 34:
                            {
                                // Command(create, 0)
                                b.Match(PatternMatcher.MatchCommand, "create", ValueKind.Integer);
                            }
                            break;
                            case 35:
                            {
                                // Command(a, 0)
                                b.Match(PatternMatcher.MatchCommand, "a", ValueKind.Integer);
                            }
                            break;
                            case 36:
                            {
                                // Command(mirror, False)
                                b.Match(PatternMatcher.MatchCommand, "mirror", ValueKind.Boolean);
                            }
                            break;
                            case 37:
                            {
                                // Command(repository, 0)
                                b.Match(PatternMatcher.MatchCommand, "repository", ValueKind.Integer);
                            }
                            break;
                            case 38:
                            {
                                // Required(Command(implies, False), Command(bare, 0))
                                var c = new RequiredMatcher(2, b.Left, b.Collected);
                                while (c.Next())
                                {
                                    switch (c.Index)
                                    {
                                        case 0:
                                        {
                                            // Command(implies, False)
                                            c.Match(PatternMatcher.MatchCommand, "implies", ValueKind.Boolean);
                                        }
                                        break;
                                        case 1:
                                        {
                                            // Command(bare, 0)
                                            c.Match(PatternMatcher.MatchCommand, "bare", ValueKind.Integer);
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
                            case 39:
                            {
                                // Option(-l,,0,False)
                                b.Match(PatternMatcher.MatchOption, "-l", ValueKind.Boolean);
                            }
                            break;
                            case 40:
                            {
                                // Option(-,,,0,0)
                                b.Match(PatternMatcher.MatchOption, "-,", ValueKind.Integer);
                            }
                            break;
                            case 41:
                            {
                                // Option(,--local,0,False)
                                b.Match(PatternMatcher.MatchOption, "--local", ValueKind.Boolean);
                            }
                            break;
                            case 42:
                            {
                                // Command(to, 0)
                                b.Match(PatternMatcher.MatchCommand, "to", ValueKind.Integer);
                            }
                            break;
                            case 43:
                            {
                                // Command(clone, 0)
                                b.Match(PatternMatcher.MatchCommand, "clone", ValueKind.Integer);
                            }
                            break;
                            case 44:
                            {
                                // Command(from, 0)
                                b.Match(PatternMatcher.MatchCommand, "from", ValueKind.Integer);
                            }
                            break;
                            case 45:
                            {
                                // Command(a, 0)
                                b.Match(PatternMatcher.MatchCommand, "a", ValueKind.Integer);
                            }
                            break;
                            case 46:
                            {
                                // Command(local, 0)
                                b.Match(PatternMatcher.MatchCommand, "local", ValueKind.Integer);
                            }
                            break;
                            case 47:
                            {
                                // Command(repository, 0)
                                b.Match(PatternMatcher.MatchCommand, "repository", ValueKind.Integer);
                            }
                            break;
                            case 48:
                            {
                                // Option(,--no-hardlinks,0,False)
                                b.Match(PatternMatcher.MatchOption, "--no-hardlinks", ValueKind.Boolean);
                            }
                            break;
                            case 49:
                            {
                                // Command(don't, 0)
                                b.Match(PatternMatcher.MatchCommand, "don't", ValueKind.Integer);
                            }
                            break;
                            case 50:
                            {
                                // Command(use, 0)
                                b.Match(PatternMatcher.MatchCommand, "use", ValueKind.Integer);
                            }
                            break;
                            case 51:
                            {
                                // Command(local, 0)
                                b.Match(PatternMatcher.MatchCommand, "local", ValueKind.Integer);
                            }
                            break;
                            case 52:
                            {
                                // Command(hardlinks,, False)
                                b.Match(PatternMatcher.MatchCommand, "hardlinks,", ValueKind.Boolean);
                            }
                            break;
                            case 53:
                            {
                                // Command(always, False)
                                b.Match(PatternMatcher.MatchCommand, "always", ValueKind.Boolean);
                            }
                            break;
                            case 54:
                            {
                                // Command(copy, False)
                                b.Match(PatternMatcher.MatchCommand, "copy", ValueKind.Boolean);
                            }
                            break;
                            case 55:
                            {
                                // Option(-s,,0,False)
                                b.Match(PatternMatcher.MatchOption, "-s", ValueKind.Boolean);
                            }
                            break;
                            case 56:
                            {
                                // Option(-,,,0,0)
                                b.Match(PatternMatcher.MatchOption, "-,", ValueKind.Integer);
                            }
                            break;
                            case 57:
                            {
                                // Option(,--shared,0,False)
                                b.Match(PatternMatcher.MatchOption, "--shared", ValueKind.Boolean);
                            }
                            break;
                            case 58:
                            {
                                // Command(setup, False)
                                b.Match(PatternMatcher.MatchCommand, "setup", ValueKind.Boolean);
                            }
                            break;
                            case 59:
                            {
                                // Command(as, False)
                                b.Match(PatternMatcher.MatchCommand, "as", ValueKind.Boolean);
                            }
                            break;
                            case 60:
                            {
                                // Command(shared, False)
                                b.Match(PatternMatcher.MatchCommand, "shared", ValueKind.Boolean);
                            }
                            break;
                            case 61:
                            {
                                // Command(repository, 0)
                                b.Match(PatternMatcher.MatchCommand, "repository", ValueKind.Integer);
                            }
                            break;
                            case 62:
                            {
                                // Option(,--recursive,0,False)
                                b.Match(PatternMatcher.MatchOption, "--recursive", ValueKind.Boolean);
                            }
                            break;
                            case 63:
                            {
                                // Command(initialize, 0)
                                b.Match(PatternMatcher.MatchCommand, "initialize", ValueKind.Integer);
                            }
                            break;
                            case 64:
                            {
                                // Command(submodules, 0)
                                b.Match(PatternMatcher.MatchCommand, "submodules", ValueKind.Integer);
                            }
                            break;
                            case 65:
                            {
                                // Command(in, 0)
                                b.Match(PatternMatcher.MatchCommand, "in", ValueKind.Integer);
                            }
                            break;
                            case 66:
                            {
                                // Command(the, 0)
                                b.Match(PatternMatcher.MatchCommand, "the", ValueKind.Integer);
                            }
                            break;
                            case 67:
                            {
                                // Command(clone, 0)
                                b.Match(PatternMatcher.MatchCommand, "clone", ValueKind.Integer);
                            }
                            break;
                            case 68:
                            {
                                // Option(,--recurse-submodules,0,False)
                                b.Match(PatternMatcher.MatchOption, "--recurse-submodules", ValueKind.Boolean);
                            }
                            break;
                            case 69:
                            {
                                // Command(initialize, 0)
                                b.Match(PatternMatcher.MatchCommand, "initialize", ValueKind.Integer);
                            }
                            break;
                            case 70:
                            {
                                // Command(submodules, 0)
                                b.Match(PatternMatcher.MatchCommand, "submodules", ValueKind.Integer);
                            }
                            break;
                            case 71:
                            {
                                // Command(in, 0)
                                b.Match(PatternMatcher.MatchCommand, "in", ValueKind.Integer);
                            }
                            break;
                            case 72:
                            {
                                // Command(the, 0)
                                b.Match(PatternMatcher.MatchCommand, "the", ValueKind.Integer);
                            }
                            break;
                            case 73:
                            {
                                // Command(clone, 0)
                                b.Match(PatternMatcher.MatchCommand, "clone", ValueKind.Integer);
                            }
                            break;
                            case 74:
                            {
                                // Option(,--template,0,False)
                                b.Match(PatternMatcher.MatchOption, "--template", ValueKind.Boolean);
                            }
                            break;
                            case 75:
                            {
                                // Argument(<template-directory>, )
                                b.Match(PatternMatcher.MatchArgument, "<template-directory>", ValueKind.None);
                            }
                            break;
                            case 76:
                            {
                                // Command(directory, False)
                                b.Match(PatternMatcher.MatchCommand, "directory", ValueKind.Boolean);
                            }
                            break;
                            case 77:
                            {
                                // Command(from, 0)
                                b.Match(PatternMatcher.MatchCommand, "from", ValueKind.Integer);
                            }
                            break;
                            case 78:
                            {
                                // Command(which, False)
                                b.Match(PatternMatcher.MatchCommand, "which", ValueKind.Boolean);
                            }
                            break;
                            case 79:
                            {
                                // Command(templates, False)
                                b.Match(PatternMatcher.MatchCommand, "templates", ValueKind.Boolean);
                            }
                            break;
                            case 80:
                            {
                                // Command(will, False)
                                b.Match(PatternMatcher.MatchCommand, "will", ValueKind.Boolean);
                            }
                            break;
                            case 81:
                            {
                                // Command(be, 0)
                                b.Match(PatternMatcher.MatchCommand, "be", ValueKind.Integer);
                            }
                            break;
                            case 82:
                            {
                                // Command(used, False)
                                b.Match(PatternMatcher.MatchCommand, "used", ValueKind.Boolean);
                            }
                            break;
                            case 83:
                            {
                                // Option(,--reference,0,False)
                                b.Match(PatternMatcher.MatchOption, "--reference", ValueKind.Boolean);
                            }
                            break;
                            case 84:
                            {
                                // Argument(<repo>, [])
                                b.Match(PatternMatcher.MatchArgument, "<repo>", ValueKind.StringList);
                            }
                            break;
                            case 85:
                            {
                                // Command(reference, False)
                                b.Match(PatternMatcher.MatchCommand, "reference", ValueKind.Boolean);
                            }
                            break;
                            case 86:
                            {
                                // Command(repository, 0)
                                b.Match(PatternMatcher.MatchCommand, "repository", ValueKind.Integer);
                            }
                            break;
                            case 87:
                            {
                                // Option(-o,,0,False)
                                b.Match(PatternMatcher.MatchOption, "-o", ValueKind.Boolean);
                            }
                            break;
                            case 88:
                            {
                                // Option(-,,,0,0)
                                b.Match(PatternMatcher.MatchOption, "-,", ValueKind.Integer);
                            }
                            break;
                            case 89:
                            {
                                // Option(,--origin,0,False)
                                b.Match(PatternMatcher.MatchOption, "--origin", ValueKind.Boolean);
                            }
                            break;
                            case 90:
                            {
                                // Argument(<branch>, [])
                                b.Match(PatternMatcher.MatchArgument, "<branch>", ValueKind.StringList);
                            }
                            break;
                            case 91:
                            {
                                // Command(use, 0)
                                b.Match(PatternMatcher.MatchCommand, "use", ValueKind.Integer);
                            }
                            break;
                            case 92:
                            {
                                // Argument(<branch>, [])
                                b.Match(PatternMatcher.MatchArgument, "<branch>", ValueKind.StringList);
                            }
                            break;
                            case 93:
                            {
                                // Command(instead, 0)
                                b.Match(PatternMatcher.MatchCommand, "instead", ValueKind.Integer);
                            }
                            break;
                            case 94:
                            {
                                // Command(of, 0)
                                b.Match(PatternMatcher.MatchCommand, "of", ValueKind.Integer);
                            }
                            break;
                            case 95:
                            {
                                // Command('origin', False)
                                b.Match(PatternMatcher.MatchCommand, "'origin'", ValueKind.Boolean);
                            }
                            break;
                            case 96:
                            {
                                // Command(to, 0)
                                b.Match(PatternMatcher.MatchCommand, "to", ValueKind.Integer);
                            }
                            break;
                            case 97:
                            {
                                // Command(track, False)
                                b.Match(PatternMatcher.MatchCommand, "track", ValueKind.Boolean);
                            }
                            break;
                            case 98:
                            {
                                // Command(upstream, False)
                                b.Match(PatternMatcher.MatchCommand, "upstream", ValueKind.Boolean);
                            }
                            break;
                            case 99:
                            {
                                // Option(-b,,0,False)
                                b.Match(PatternMatcher.MatchOption, "-b", ValueKind.Boolean);
                            }
                            break;
                            case 100:
                            {
                                // Option(-,,,0,0)
                                b.Match(PatternMatcher.MatchOption, "-,", ValueKind.Integer);
                            }
                            break;
                            case 101:
                            {
                                // Option(,--branch,0,False)
                                b.Match(PatternMatcher.MatchOption, "--branch", ValueKind.Boolean);
                            }
                            break;
                            case 102:
                            {
                                // Argument(<branch>, [])
                                b.Match(PatternMatcher.MatchArgument, "<branch>", ValueKind.StringList);
                            }
                            break;
                            case 103:
                            {
                                // Command(checkout, 0)
                                b.Match(PatternMatcher.MatchCommand, "checkout", ValueKind.Integer);
                            }
                            break;
                            case 104:
                            {
                                // Argument(<branch>, [])
                                b.Match(PatternMatcher.MatchArgument, "<branch>", ValueKind.StringList);
                            }
                            break;
                            case 105:
                            {
                                // Command(instead, 0)
                                b.Match(PatternMatcher.MatchCommand, "instead", ValueKind.Integer);
                            }
                            break;
                            case 106:
                            {
                                // Command(of, 0)
                                b.Match(PatternMatcher.MatchCommand, "of", ValueKind.Integer);
                            }
                            break;
                            case 107:
                            {
                                // Command(the, 0)
                                b.Match(PatternMatcher.MatchCommand, "the", ValueKind.Integer);
                            }
                            break;
                            case 108:
                            {
                                // Command(remote's, False)
                                b.Match(PatternMatcher.MatchCommand, "remote's", ValueKind.Boolean);
                            }
                            break;
                            case 109:
                            {
                                // Argument(HEAD, )
                                b.Match(PatternMatcher.MatchArgument, "HEAD", ValueKind.None);
                            }
                            break;
                            case 110:
                            {
                                // Option(-u,,0,False)
                                b.Match(PatternMatcher.MatchOption, "-u", ValueKind.Boolean);
                            }
                            break;
                            case 111:
                            {
                                // Option(-,,,0,0)
                                b.Match(PatternMatcher.MatchOption, "-,", ValueKind.Integer);
                            }
                            break;
                            case 112:
                            {
                                // Option(,--upload-pack,0,False)
                                b.Match(PatternMatcher.MatchOption, "--upload-pack", ValueKind.Boolean);
                            }
                            break;
                            case 113:
                            {
                                // Argument(<path>, )
                                b.Match(PatternMatcher.MatchArgument, "<path>", ValueKind.None);
                            }
                            break;
                            case 114:
                            {
                                // Command(path, False)
                                b.Match(PatternMatcher.MatchCommand, "path", ValueKind.Boolean);
                            }
                            break;
                            case 115:
                            {
                                // Command(to, 0)
                                b.Match(PatternMatcher.MatchCommand, "to", ValueKind.Integer);
                            }
                            break;
                            case 116:
                            {
                                // Command(git-upload-pack, False)
                                b.Match(PatternMatcher.MatchCommand, "git-upload-pack", ValueKind.Boolean);
                            }
                            break;
                            case 117:
                            {
                                // Command(on, False)
                                b.Match(PatternMatcher.MatchCommand, "on", ValueKind.Boolean);
                            }
                            break;
                            case 118:
                            {
                                // Command(the, 0)
                                b.Match(PatternMatcher.MatchCommand, "the", ValueKind.Integer);
                            }
                            break;
                            case 119:
                            {
                                // Command(remote, False)
                                b.Match(PatternMatcher.MatchCommand, "remote", ValueKind.Boolean);
                            }
                            break;
                            case 120:
                            {
                                // Option(,--depth,0,False)
                                b.Match(PatternMatcher.MatchOption, "--depth", ValueKind.Boolean);
                            }
                            break;
                            case 121:
                            {
                                // Argument(<depth>, )
                                b.Match(PatternMatcher.MatchArgument, "<depth>", ValueKind.None);
                            }
                            break;
                            case 122:
                            {
                                // Command(create, 0)
                                b.Match(PatternMatcher.MatchCommand, "create", ValueKind.Integer);
                            }
                            break;
                            case 123:
                            {
                                // Command(a, 0)
                                b.Match(PatternMatcher.MatchCommand, "a", ValueKind.Integer);
                            }
                            break;
                            case 124:
                            {
                                // Command(shallow, False)
                                b.Match(PatternMatcher.MatchCommand, "shallow", ValueKind.Boolean);
                            }
                            break;
                            case 125:
                            {
                                // Command(clone, 0)
                                b.Match(PatternMatcher.MatchCommand, "clone", ValueKind.Integer);
                            }
                            break;
                            case 126:
                            {
                                // Command(of, 0)
                                b.Match(PatternMatcher.MatchCommand, "of", ValueKind.Integer);
                            }
                            break;
                            case 127:
                            {
                                // Command(that, False)
                                b.Match(PatternMatcher.MatchCommand, "that", ValueKind.Boolean);
                            }
                            break;
                            case 128:
                            {
                                // Command(depth, False)
                                b.Match(PatternMatcher.MatchCommand, "depth", ValueKind.Boolean);
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
            yield return KeyValuePair.Create("clone", (object?)CmdClone);
            yield return KeyValuePair.Create("--", (object?)Cmd);
            yield return KeyValuePair.Create("<repo>", (object?)ArgRepo);
            yield return KeyValuePair.Create("<dir>", (object?)ArgDir);
            yield return KeyValuePair.Create("-v", (object?)OptV);
            yield return KeyValuePair.Create("-,", (object?)OptComma);
            yield return KeyValuePair.Create("--verbose", (object?)OptVerbose);
            yield return KeyValuePair.Create("be", (object?)CmdBe);
            yield return KeyValuePair.Create("more", (object?)CmdMore);
            yield return KeyValuePair.Create("verbose", (object?)CmdVerbose);
            yield return KeyValuePair.Create("-q", (object?)OptQ);
            yield return KeyValuePair.Create("--quiet", (object?)OptQuiet);
            yield return KeyValuePair.Create("quiet", (object?)CmdQuiet);
            yield return KeyValuePair.Create("--progress", (object?)OptProgress);
            yield return KeyValuePair.Create("force", (object?)CmdForce);
            yield return KeyValuePair.Create("progress", (object?)CmdProgress);
            yield return KeyValuePair.Create("reporting", (object?)CmdReporting);
            yield return KeyValuePair.Create("-n", (object?)OptN);
            yield return KeyValuePair.Create("--no-checkout", (object?)OptNoCheckout);
            yield return KeyValuePair.Create("don't", (object?)CmdDont);
            yield return KeyValuePair.Create("create", (object?)CmdCreate);
            yield return KeyValuePair.Create("a", (object?)CmdA);
            yield return KeyValuePair.Create("checkout", (object?)CmdCheckout);
            yield return KeyValuePair.Create("--bare", (object?)OptBare);
            yield return KeyValuePair.Create("bare", (object?)CmdBare);
            yield return KeyValuePair.Create("repository", (object?)CmdRepository);
            yield return KeyValuePair.Create("--mirror", (object?)OptMirror);
            yield return KeyValuePair.Create("mirror", (object?)CmdMirror);
            yield return KeyValuePair.Create("implies", (object?)CmdImplies);
            yield return KeyValuePair.Create("-l", (object?)OptL);
            yield return KeyValuePair.Create("--local", (object?)OptLocal);
            yield return KeyValuePair.Create("to", (object?)CmdTo);
            yield return KeyValuePair.Create("from", (object?)CmdFrom);
            yield return KeyValuePair.Create("local", (object?)CmdLocal);
            yield return KeyValuePair.Create("--no-hardlinks", (object?)OptNoHardlinks);
            yield return KeyValuePair.Create("use", (object?)CmdUse);
            yield return KeyValuePair.Create("hardlinks,", (object?)CmdHardlinksComma);
            yield return KeyValuePair.Create("always", (object?)CmdAlways);
            yield return KeyValuePair.Create("copy", (object?)CmdCopy);
            yield return KeyValuePair.Create("-s", (object?)OptS);
            yield return KeyValuePair.Create("--shared", (object?)OptShared);
            yield return KeyValuePair.Create("setup", (object?)CmdSetup);
            yield return KeyValuePair.Create("as", (object?)CmdAs);
            yield return KeyValuePair.Create("shared", (object?)CmdShared);
            yield return KeyValuePair.Create("--recursive", (object?)OptRecursive);
            yield return KeyValuePair.Create("initialize", (object?)CmdInitialize);
            yield return KeyValuePair.Create("submodules", (object?)CmdSubmodules);
            yield return KeyValuePair.Create("in", (object?)CmdIn);
            yield return KeyValuePair.Create("the", (object?)CmdThe);
            yield return KeyValuePair.Create("--recurse-submodules", (object?)OptRecurseSubmodules);
            yield return KeyValuePair.Create("--template", (object?)OptTemplate);
            yield return KeyValuePair.Create("<template-directory>", (object?)ArgTemplateDirectory);
            yield return KeyValuePair.Create("directory", (object?)CmdDirectory);
            yield return KeyValuePair.Create("which", (object?)CmdWhich);
            yield return KeyValuePair.Create("templates", (object?)CmdTemplates);
            yield return KeyValuePair.Create("will", (object?)CmdWill);
            yield return KeyValuePair.Create("used", (object?)CmdUsed);
            yield return KeyValuePair.Create("--reference", (object?)OptReference);
            yield return KeyValuePair.Create("reference", (object?)CmdReference);
            yield return KeyValuePair.Create("-o", (object?)OptO);
            yield return KeyValuePair.Create("--origin", (object?)OptOrigin);
            yield return KeyValuePair.Create("<branch>", (object?)ArgBranch);
            yield return KeyValuePair.Create("instead", (object?)CmdInstead);
            yield return KeyValuePair.Create("of", (object?)CmdOf);
            yield return KeyValuePair.Create("'origin'", (object?)CmdOrigin);
            yield return KeyValuePair.Create("track", (object?)CmdTrack);
            yield return KeyValuePair.Create("upstream", (object?)CmdUpstream);
            yield return KeyValuePair.Create("-b", (object?)OptB);
            yield return KeyValuePair.Create("--branch", (object?)OptBranch);
            yield return KeyValuePair.Create("remote's", (object?)CmdRemotes);
            yield return KeyValuePair.Create("HEAD", (object?)ArgHead);
            yield return KeyValuePair.Create("-u", (object?)OptU);
            yield return KeyValuePair.Create("--upload-pack", (object?)OptUploadPack);
            yield return KeyValuePair.Create("<path>", (object?)ArgPath);
            yield return KeyValuePair.Create("path", (object?)CmdPath);
            yield return KeyValuePair.Create("git-upload-pack", (object?)CmdGitUploadPack);
            yield return KeyValuePair.Create("on", (object?)CmdOn);
            yield return KeyValuePair.Create("remote", (object?)CmdRemote);
            yield return KeyValuePair.Create("--depth", (object?)OptDepth);
            yield return KeyValuePair.Create("<depth>", (object?)ArgDepth);
            yield return KeyValuePair.Create("shallow", (object?)CmdShallow);
            yield return KeyValuePair.Create("that", (object?)CmdThat);
            yield return KeyValuePair.Create("depth", (object?)CmdDepth);
        }

        IEnumerator<KeyValuePair<string, object?>> IEnumerable<KeyValuePair<string, object?>>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary><c>Command(clone, 0)</c></summary>
        public int CmdClone { get; private set; }

        /// <summary><c>Command(--, False)</c></summary>
        public bool Cmd { get; private set; }

        /// <summary><c>Argument(&lt;repo&gt;, [])</c></summary>
        public StringList ArgRepo { get; private set; } = StringList.Empty;

        /// <summary><c>Argument(&lt;dir&gt;, )</c></summary>
        public string? ArgDir { get; private set; }

        /// <summary><c>Option(-v,,0,False)</c></summary>
        public bool OptV { get; private set; }

        /// <summary><c>Option(-,,,0,0)</c></summary>
        public int OptComma { get; private set; }

        /// <summary><c>Option(,--verbose,0,False)</c></summary>
        public bool OptVerbose { get; private set; }

        /// <summary><c>Command(be, 0)</c></summary>
        public int CmdBe { get; private set; }

        /// <summary><c>Command(more, 0)</c></summary>
        public int CmdMore { get; private set; }

        /// <summary><c>Command(verbose, False)</c></summary>
        public bool CmdVerbose { get; private set; }

        /// <summary><c>Option(-q,,0,False)</c></summary>
        public bool OptQ { get; private set; }

        /// <summary><c>Option(,--quiet,0,False)</c></summary>
        public bool OptQuiet { get; private set; }

        /// <summary><c>Command(quiet, False)</c></summary>
        public bool CmdQuiet { get; private set; }

        /// <summary><c>Option(,--progress,0,False)</c></summary>
        public bool OptProgress { get; private set; }

        /// <summary><c>Command(force, False)</c></summary>
        public bool CmdForce { get; private set; }

        /// <summary><c>Command(progress, False)</c></summary>
        public bool CmdProgress { get; private set; }

        /// <summary><c>Command(reporting, False)</c></summary>
        public bool CmdReporting { get; private set; }

        /// <summary><c>Option(-n,,0,False)</c></summary>
        public bool OptN { get; private set; }

        /// <summary><c>Option(,--no-checkout,0,False)</c></summary>
        public bool OptNoCheckout { get; private set; }

        /// <summary><c>Command(don't, 0)</c></summary>
        public int CmdDont { get; private set; }

        /// <summary><c>Command(create, 0)</c></summary>
        public int CmdCreate { get; private set; }

        /// <summary><c>Command(a, 0)</c></summary>
        public int CmdA { get; private set; }

        /// <summary><c>Command(checkout, 0)</c></summary>
        public int CmdCheckout { get; private set; }

        /// <summary><c>Option(,--bare,0,False)</c></summary>
        public bool OptBare { get; private set; }

        /// <summary><c>Command(bare, 0)</c></summary>
        public int CmdBare { get; private set; }

        /// <summary><c>Command(repository, 0)</c></summary>
        public int CmdRepository { get; private set; }

        /// <summary><c>Option(,--mirror,0,False)</c></summary>
        public bool OptMirror { get; private set; }

        /// <summary><c>Command(mirror, False)</c></summary>
        public bool CmdMirror { get; private set; }

        /// <summary><c>Command(implies, False)</c></summary>
        public bool CmdImplies { get; private set; }

        /// <summary><c>Option(-l,,0,False)</c></summary>
        public bool OptL { get; private set; }

        /// <summary><c>Option(,--local,0,False)</c></summary>
        public bool OptLocal { get; private set; }

        /// <summary><c>Command(to, 0)</c></summary>
        public int CmdTo { get; private set; }

        /// <summary><c>Command(from, 0)</c></summary>
        public int CmdFrom { get; private set; }

        /// <summary><c>Command(local, 0)</c></summary>
        public int CmdLocal { get; private set; }

        /// <summary><c>Option(,--no-hardlinks,0,False)</c></summary>
        public bool OptNoHardlinks { get; private set; }

        /// <summary><c>Command(use, 0)</c></summary>
        public int CmdUse { get; private set; }

        /// <summary><c>Command(hardlinks,, False)</c></summary>
        public bool CmdHardlinksComma { get; private set; }

        /// <summary><c>Command(always, False)</c></summary>
        public bool CmdAlways { get; private set; }

        /// <summary><c>Command(copy, False)</c></summary>
        public bool CmdCopy { get; private set; }

        /// <summary><c>Option(-s,,0,False)</c></summary>
        public bool OptS { get; private set; }

        /// <summary><c>Option(,--shared,0,False)</c></summary>
        public bool OptShared { get; private set; }

        /// <summary><c>Command(setup, False)</c></summary>
        public bool CmdSetup { get; private set; }

        /// <summary><c>Command(as, False)</c></summary>
        public bool CmdAs { get; private set; }

        /// <summary><c>Command(shared, False)</c></summary>
        public bool CmdShared { get; private set; }

        /// <summary><c>Option(,--recursive,0,False)</c></summary>
        public bool OptRecursive { get; private set; }

        /// <summary><c>Command(initialize, 0)</c></summary>
        public int CmdInitialize { get; private set; }

        /// <summary><c>Command(submodules, 0)</c></summary>
        public int CmdSubmodules { get; private set; }

        /// <summary><c>Command(in, 0)</c></summary>
        public int CmdIn { get; private set; }

        /// <summary><c>Command(the, 0)</c></summary>
        public int CmdThe { get; private set; }

        /// <summary><c>Option(,--recurse-submodules,0,False)</c></summary>
        public bool OptRecurseSubmodules { get; private set; }

        /// <summary><c>Option(,--template,0,False)</c></summary>
        public bool OptTemplate { get; private set; }

        /// <summary><c>Argument(&lt;template-directory&gt;, )</c></summary>
        public string? ArgTemplateDirectory { get; private set; }

        /// <summary><c>Command(directory, False)</c></summary>
        public bool CmdDirectory { get; private set; }

        /// <summary><c>Command(which, False)</c></summary>
        public bool CmdWhich { get; private set; }

        /// <summary><c>Command(templates, False)</c></summary>
        public bool CmdTemplates { get; private set; }

        /// <summary><c>Command(will, False)</c></summary>
        public bool CmdWill { get; private set; }

        /// <summary><c>Command(used, False)</c></summary>
        public bool CmdUsed { get; private set; }

        /// <summary><c>Option(,--reference,0,False)</c></summary>
        public bool OptReference { get; private set; }

        /// <summary><c>Command(reference, False)</c></summary>
        public bool CmdReference { get; private set; }

        /// <summary><c>Option(-o,,0,False)</c></summary>
        public bool OptO { get; private set; }

        /// <summary><c>Option(,--origin,0,False)</c></summary>
        public bool OptOrigin { get; private set; }

        /// <summary><c>Argument(&lt;branch&gt;, [])</c></summary>
        public StringList ArgBranch { get; private set; } = StringList.Empty;

        /// <summary><c>Command(instead, 0)</c></summary>
        public int CmdInstead { get; private set; }

        /// <summary><c>Command(of, 0)</c></summary>
        public int CmdOf { get; private set; }

        /// <summary><c>Command('origin', False)</c></summary>
        public bool CmdOrigin { get; private set; }

        /// <summary><c>Command(track, False)</c></summary>
        public bool CmdTrack { get; private set; }

        /// <summary><c>Command(upstream, False)</c></summary>
        public bool CmdUpstream { get; private set; }

        /// <summary><c>Option(-b,,0,False)</c></summary>
        public bool OptB { get; private set; }

        /// <summary><c>Option(,--branch,0,False)</c></summary>
        public bool OptBranch { get; private set; }

        /// <summary><c>Command(remote's, False)</c></summary>
        public bool CmdRemotes { get; private set; }

        /// <summary><c>Argument(HEAD, )</c></summary>
        public string? ArgHead { get; private set; }

        /// <summary><c>Option(-u,,0,False)</c></summary>
        public bool OptU { get; private set; }

        /// <summary><c>Option(,--upload-pack,0,False)</c></summary>
        public bool OptUploadPack { get; private set; }

        /// <summary><c>Argument(&lt;path&gt;, )</c></summary>
        public string? ArgPath { get; private set; }

        /// <summary><c>Command(path, False)</c></summary>
        public bool CmdPath { get; private set; }

        /// <summary><c>Command(git-upload-pack, False)</c></summary>
        public bool CmdGitUploadPack { get; private set; }

        /// <summary><c>Command(on, False)</c></summary>
        public bool CmdOn { get; private set; }

        /// <summary><c>Command(remote, False)</c></summary>
        public bool CmdRemote { get; private set; }

        /// <summary><c>Option(,--depth,0,False)</c></summary>
        public bool OptDepth { get; private set; }

        /// <summary><c>Argument(&lt;depth&gt;, )</c></summary>
        public string? ArgDepth { get; private set; }

        /// <summary><c>Command(shallow, False)</c></summary>
        public bool CmdShallow { get; private set; }

        /// <summary><c>Command(that, False)</c></summary>
        public bool CmdThat { get; private set; }

        /// <summary><c>Command(depth, False)</c></summary>
        public bool CmdDepth { get; private set; }
    }
}
