#nullable enable annotations

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DocoptNet.Generated;
using Leaves = DocoptNet.Generated.ReadOnlyList<DocoptNet.Generated.LeafPattern>;

namespace NavalFate
{
    partial class ProgramArguments : IEnumerable<KeyValuePair<string, object?>>
    {
        public const string HelpText = @"Naval Fate.

    Usage:
      naval_fate.exe ship new <name>...
      naval_fate.exe ship <name> move <x> <y> [--speed=<kn>]
      naval_fate.exe ship shoot <x> <y>
      naval_fate.exe mine (set|remove) <x> <y> [--moored | --drifting]
      naval_fate.exe (-h | --help)
      naval_fate.exe --version

    Options:
      -h --help     Show this screen.
      --version     Show version.
      --speed=<kn>  Speed in knots [default: 10].
      --moored      Moored (anchored) mine.
      --drifting    Drifting mine.
";

        public const string Usage = @"Usage:
      naval_fate.exe ship new <name>...
      naval_fate.exe ship <name> move <x> <y> [--speed=<kn>]
      naval_fate.exe ship shoot <x> <y>
      naval_fate.exe mine (set|remove) <x> <y> [--moored | --drifting]
      naval_fate.exe (-h | --help)
      naval_fate.exe --version";

        public static ProgramArguments Apply(IEnumerable<string> args, bool help = true, object? version = null, bool optionsFirst = false, bool exit = false)
        {
            var tokens = new Tokens(args, typeof(DocoptInputErrorException));
            var options = new List<Option>
            {
                new Option("-h", "--help", 0, false),
                new Option(null, "--version", 0, false),
                new Option(null, "--speed", 1, "10"),
                new Option(null, "--moored", 0, false),
                new Option(null, "--drifting", 0, false),
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
                // Required(Either(Required(Command(ship, False), Command(new, False), OneOrMore(Argument(<name>, []))), Required(Command(ship, False), Argument(<name>, []), Command(move, False), Argument(<x>, ), Argument(<y>, ), Optional(Option(,--speed,1,10))), Required(Command(ship, False), Command(shoot, False), Argument(<x>, ), Argument(<y>, )), Required(Command(mine, False), Required(Either(Command(set, False), Command(remove, False))), Argument(<x>, ), Argument(<y>, ), Optional(Either(Option(,--moored,0,False), Option(,--drifting,0,False)))), Required(Required(Option(-h,--help,0,False))), Required(Option(,--version,0,False))))
                var b = new RequiredMatcher(1, a.Left, a.Collected);
                while (b.Next())
                {
                    // Either(Required(Command(ship, False), Command(new, False), OneOrMore(Argument(<name>, []))), Required(Command(ship, False), Argument(<name>, []), Command(move, False), Argument(<x>, ), Argument(<y>, ), Optional(Option(,--speed,1,10))), Required(Command(ship, False), Command(shoot, False), Argument(<x>, ), Argument(<y>, )), Required(Command(mine, False), Required(Either(Command(set, False), Command(remove, False))), Argument(<x>, ), Argument(<y>, ), Optional(Either(Option(,--moored,0,False), Option(,--drifting,0,False)))), Required(Required(Option(-h,--help,0,False))), Required(Option(,--version,0,False)))
                    var c = new EitherMatcher(6, b.Left, b.Collected);
                    while (c.Next())
                    {
                        switch (c.Index)
                        {
                            case 0:
                            {
                                // Required(Command(ship, False), Command(new, False), OneOrMore(Argument(<name>, [])))
                                var d = new RequiredMatcher(3, c.Left, c.Collected);
                                while (d.Next())
                                {
                                    switch (d.Index)
                                    {
                                        case 0:
                                        {
                                            // Command(ship, False)
                                            d.Match(PatternMatcher.MatchCommand, "ship", ValueKind.Boolean);
                                            break;
                                        }
                                        case 1:
                                        {
                                            // Command(new, False)
                                            d.Match(PatternMatcher.MatchCommand, "new", ValueKind.Boolean);
                                            break;
                                        }
                                        case 2:
                                        {
                                            // OneOrMore(Argument(<name>, []))
                                            var e = new OneOrMoreMatcher(1, d.Left, d.Collected);
                                            while (e.Next())
                                            {
                                                // Argument(<name>, [])
                                                e.Match(PatternMatcher.MatchArgument, "<name>", ValueKind.StringList);
                                                if (!e.LastMatched)
                                                    break;
                                            }
                                            d.Fold(e.Result);
                                            break;
                                        }
                                    }
                                    if (!d.LastMatched)
                                        break;
                                }
                                c.Fold(d.Result);
                                break;
                            }
                            case 1:
                            {
                                // Required(Command(ship, False), Argument(<name>, []), Command(move, False), Argument(<x>, ), Argument(<y>, ), Optional(Option(,--speed,1,10)))
                                var d = new RequiredMatcher(6, c.Left, c.Collected);
                                while (d.Next())
                                {
                                    switch (d.Index)
                                    {
                                        case 0:
                                        {
                                            // Command(ship, False)
                                            d.Match(PatternMatcher.MatchCommand, "ship", ValueKind.Boolean);
                                            break;
                                        }
                                        case 1:
                                        {
                                            // Argument(<name>, [])
                                            d.Match(PatternMatcher.MatchArgument, "<name>", ValueKind.StringList);
                                            break;
                                        }
                                        case 2:
                                        {
                                            // Command(move, False)
                                            d.Match(PatternMatcher.MatchCommand, "move", ValueKind.Boolean);
                                            break;
                                        }
                                        case 3:
                                        {
                                            // Argument(<x>, )
                                            d.Match(PatternMatcher.MatchArgument, "<x>", ValueKind.None);
                                            break;
                                        }
                                        case 4:
                                        {
                                            // Argument(<y>, )
                                            d.Match(PatternMatcher.MatchArgument, "<y>", ValueKind.None);
                                            break;
                                        }
                                        case 5:
                                        {
                                            // Optional(Option(,--speed,1,10))
                                            var e = new OptionalMatcher(1, d.Left, d.Collected);
                                            while (e.Next())
                                            {
                                                // Option(,--speed,1,10)
                                                e.Match(PatternMatcher.MatchOption, "--speed", ValueKind.String);
                                                if (!e.LastMatched)
                                                    break;
                                            }
                                            d.Fold(e.Result);
                                            break;
                                        }
                                    }
                                    if (!d.LastMatched)
                                        break;
                                }
                                c.Fold(d.Result);
                                break;
                            }
                            case 2:
                            {
                                // Required(Command(ship, False), Command(shoot, False), Argument(<x>, ), Argument(<y>, ))
                                var d = new RequiredMatcher(4, c.Left, c.Collected);
                                while (d.Next())
                                {
                                    switch (d.Index)
                                    {
                                        case 0:
                                        {
                                            // Command(ship, False)
                                            d.Match(PatternMatcher.MatchCommand, "ship", ValueKind.Boolean);
                                            break;
                                        }
                                        case 1:
                                        {
                                            // Command(shoot, False)
                                            d.Match(PatternMatcher.MatchCommand, "shoot", ValueKind.Boolean);
                                            break;
                                        }
                                        case 2:
                                        {
                                            // Argument(<x>, )
                                            d.Match(PatternMatcher.MatchArgument, "<x>", ValueKind.None);
                                            break;
                                        }
                                        case 3:
                                        {
                                            // Argument(<y>, )
                                            d.Match(PatternMatcher.MatchArgument, "<y>", ValueKind.None);
                                            break;
                                        }
                                    }
                                    if (!d.LastMatched)
                                        break;
                                }
                                c.Fold(d.Result);
                                break;
                            }
                            case 3:
                            {
                                // Required(Command(mine, False), Required(Either(Command(set, False), Command(remove, False))), Argument(<x>, ), Argument(<y>, ), Optional(Either(Option(,--moored,0,False), Option(,--drifting,0,False))))
                                var d = new RequiredMatcher(5, c.Left, c.Collected);
                                while (d.Next())
                                {
                                    switch (d.Index)
                                    {
                                        case 0:
                                        {
                                            // Command(mine, False)
                                            d.Match(PatternMatcher.MatchCommand, "mine", ValueKind.Boolean);
                                            break;
                                        }
                                        case 1:
                                        {
                                            // Required(Either(Command(set, False), Command(remove, False)))
                                            var e = new RequiredMatcher(1, d.Left, d.Collected);
                                            while (e.Next())
                                            {
                                                // Either(Command(set, False), Command(remove, False))
                                                var f = new EitherMatcher(2, e.Left, e.Collected);
                                                while (f.Next())
                                                {
                                                    switch (f.Index)
                                                    {
                                                        case 0:
                                                        {
                                                            // Command(set, False)
                                                            f.Match(PatternMatcher.MatchCommand, "set", ValueKind.Boolean);
                                                            break;
                                                        }
                                                        case 1:
                                                        {
                                                            // Command(remove, False)
                                                            f.Match(PatternMatcher.MatchCommand, "remove", ValueKind.Boolean);
                                                            break;
                                                        }
                                                    }
                                                    if (!f.LastMatched)
                                                        break;
                                                }
                                                e.Fold(f.Result);
                                                if (!e.LastMatched)
                                                    break;
                                            }
                                            d.Fold(e.Result);
                                            break;
                                        }
                                        case 2:
                                        {
                                            // Argument(<x>, )
                                            d.Match(PatternMatcher.MatchArgument, "<x>", ValueKind.None);
                                            break;
                                        }
                                        case 3:
                                        {
                                            // Argument(<y>, )
                                            d.Match(PatternMatcher.MatchArgument, "<y>", ValueKind.None);
                                            break;
                                        }
                                        case 4:
                                        {
                                            // Optional(Either(Option(,--moored,0,False), Option(,--drifting,0,False)))
                                            var e = new OptionalMatcher(1, d.Left, d.Collected);
                                            while (e.Next())
                                            {
                                                // Either(Option(,--moored,0,False), Option(,--drifting,0,False))
                                                var f = new EitherMatcher(2, e.Left, e.Collected);
                                                while (f.Next())
                                                {
                                                    switch (f.Index)
                                                    {
                                                        case 0:
                                                        {
                                                            // Option(,--moored,0,False)
                                                            f.Match(PatternMatcher.MatchOption, "--moored", ValueKind.Boolean);
                                                            break;
                                                        }
                                                        case 1:
                                                        {
                                                            // Option(,--drifting,0,False)
                                                            f.Match(PatternMatcher.MatchOption, "--drifting", ValueKind.Boolean);
                                                            break;
                                                        }
                                                    }
                                                    if (!f.LastMatched)
                                                        break;
                                                }
                                                e.Fold(f.Result);
                                                if (!e.LastMatched)
                                                    break;
                                            }
                                            d.Fold(e.Result);
                                            break;
                                        }
                                    }
                                    if (!d.LastMatched)
                                        break;
                                }
                                c.Fold(d.Result);
                                break;
                            }
                            case 4:
                            {
                                // Required(Required(Option(-h,--help,0,False)))
                                var d = new RequiredMatcher(1, c.Left, c.Collected);
                                while (d.Next())
                                {
                                    // Required(Option(-h,--help,0,False))
                                    var e = new RequiredMatcher(1, d.Left, d.Collected);
                                    while (e.Next())
                                    {
                                        // Option(-h,--help,0,False)
                                        e.Match(PatternMatcher.MatchOption, "--help", ValueKind.Boolean);
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
                            case 5:
                            {
                                // Required(Option(,--version,0,False))
                                var d = new RequiredMatcher(1, c.Left, c.Collected);
                                while (d.Next())
                                {
                                    // Option(,--version,0,False)
                                    d.Match(PatternMatcher.MatchOption, "--version", ValueKind.Boolean);
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
                    case @"ship": result.CmdShip = (bool)value; break;
                    case @"new": result.CmdNew = (bool)value; break;
                    case @"<name>": result.ArgName = (StringList)value; break;
                    case @"move": result.CmdMove = (bool)value; break;
                    case @"<x>": result.ArgX = (string?)value; break;
                    case @"<y>": result.ArgY = (string?)value; break;
                    case @"--speed": result.OptSpeed = (string)value; break;
                    case @"shoot": result.CmdShoot = (bool)value; break;
                    case @"mine": result.CmdMine = (bool)value; break;
                    case @"set": result.CmdSet = (bool)value; break;
                    case @"remove": result.CmdRemove = (bool)value; break;
                    case @"--moored": result.OptMoored = (bool)value; break;
                    case @"--drifting": result.OptDrifting = (bool)value; break;
                    case @"--help": result.OptHelp = (bool)value; break;
                    case @"--version": result.OptVersion = (bool)value; break;
                }
            }

            return result;
        }

        IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
        {
            yield return KeyValuePair.Create("ship", (object?)CmdShip);
            yield return KeyValuePair.Create("new", (object?)CmdNew);
            yield return KeyValuePair.Create("<name>", (object?)ArgName);
            yield return KeyValuePair.Create("move", (object?)CmdMove);
            yield return KeyValuePair.Create("<x>", (object?)ArgX);
            yield return KeyValuePair.Create("<y>", (object?)ArgY);
            yield return KeyValuePair.Create("--speed", (object?)OptSpeed);
            yield return KeyValuePair.Create("shoot", (object?)CmdShoot);
            yield return KeyValuePair.Create("mine", (object?)CmdMine);
            yield return KeyValuePair.Create("set", (object?)CmdSet);
            yield return KeyValuePair.Create("remove", (object?)CmdRemove);
            yield return KeyValuePair.Create("--moored", (object?)OptMoored);
            yield return KeyValuePair.Create("--drifting", (object?)OptDrifting);
            yield return KeyValuePair.Create("--help", (object?)OptHelp);
            yield return KeyValuePair.Create("--version", (object?)OptVersion);
        }

        IEnumerator<KeyValuePair<string, object?>> IEnumerable<KeyValuePair<string, object?>>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary><c>Command(ship, False)</c></summary>
        public bool CmdShip { get; private set; }

        /// <summary><c>Command(new, False)</c></summary>
        public bool CmdNew { get; private set; }

        /// <summary><c>Argument(&lt;name&gt;, [])</c></summary>
        public StringList ArgName { get; private set; } = StringList.Empty;

        /// <summary><c>Command(move, False)</c></summary>
        public bool CmdMove { get; private set; }

        /// <summary><c>Argument(&lt;x&gt;, )</c></summary>
        public string? ArgX { get; private set; }

        /// <summary><c>Argument(&lt;y&gt;, )</c></summary>
        public string? ArgY { get; private set; }

        /// <summary><c>Option(,--speed,1,10)</c></summary>
        public string OptSpeed { get; private set; } = "10";

        /// <summary><c>Command(shoot, False)</c></summary>
        public bool CmdShoot { get; private set; }

        /// <summary><c>Command(mine, False)</c></summary>
        public bool CmdMine { get; private set; }

        /// <summary><c>Command(set, False)</c></summary>
        public bool CmdSet { get; private set; }

        /// <summary><c>Command(remove, False)</c></summary>
        public bool CmdRemove { get; private set; }

        /// <summary><c>Option(,--moored,0,False)</c></summary>
        public bool OptMoored { get; private set; }

        /// <summary><c>Option(,--drifting,0,False)</c></summary>
        public bool OptDrifting { get; private set; }

        /// <summary><c>Option(-h,--help,0,False)</c></summary>
        public bool OptHelp { get; private set; }

        /// <summary><c>Option(,--version,0,False)</c></summary>
        public bool OptVersion { get; private set; }
    }
}
