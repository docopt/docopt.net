#nullable enable annotations

using System.Collections;
using System.Collections.Generic;
using DocoptNet;
using DocoptNet.Internals;
using Leaves = DocoptNet.Internals.ReadOnlyList<DocoptNet.Internals.LeafPattern>;

partial class ProgramArguments : IEnumerable<KeyValuePair<string, object?>>
{
    public const string Help = @"
Naval Fate.

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

    static readonly IParserWithHelpSupport<ProgramArguments> Parser = GeneratedSourceModule.CreateParser(Help, Parse);

    public static IParserWithHelpSupport<ProgramArguments> CreateParser() => Parser;

    static IParser<ProgramArguments>.IResult Parse(IEnumerable<string> args, ParseFlags flags, string? version)
    {
        var options = new List<Option>
        {
            new Option("-h", "--help", 0, false),
            new Option(null, "--version", 0, false),
            new Option(null, "--speed", 1, "10"),
            new Option(null, "--moored", 0, false),
            new Option(null, "--drifting", 0, false),
        };
        return GeneratedSourceModule.Parse(Help,Usage, args, options, flags, version, Parse);

        static IParser<ProgramArguments>.IResult Parse(Leaves left)
        {
            var required = new RequiredMatcher(1, left, new Leaves());
            Match(ref required);
            if (!required.Result || required.Left.Count > 0)
            {
                return GeneratedSourceModule.CreateInputErrorResult<ProgramArguments>(string.Empty, Usage);
            }
            var collected = required.Collected;
            var result = new ProgramArguments();

            foreach (var leaf in collected)
            {
                var value = leaf.Value is { IsStringList: true } ? ((StringList)leaf.Value).Reverse() : leaf.Value;
                switch (leaf.Name)
                {
                    case "ship": result.CmdShip = (bool)value; break;
                    case "new": result.CmdNew = (bool)value; break;
                    case "<name>": result.ArgName = (StringList)value; break;
                    case "move": result.CmdMove = (bool)value; break;
                    case "<x>": result.ArgX = (string?)value; break;
                    case "<y>": result.ArgY = (string?)value; break;
                    case "--speed": result.OptSpeed = (string)value; break;
                    case "shoot": result.CmdShoot = (bool)value; break;
                    case "mine": result.CmdMine = (bool)value; break;
                    case "set": result.CmdSet = (bool)value; break;
                    case "remove": result.CmdRemove = (bool)value; break;
                    case "--moored": result.OptMoored = (bool)value; break;
                    case "--drifting": result.OptDrifting = (bool)value; break;
                    case "--help": result.OptHelp = (bool)value; break;
                    case "--version": result.OptVersion = (bool)value; break;
                }
            }

            return GeneratedSourceModule.CreateArgumentsResult(result);
        }

        static void Match(ref RequiredMatcher required)
        {
            // Required(Either(Required(Command(ship, False), Command(new, False), OneOrMore(Argument(<name>, []))), Required(Command(ship, False), Argument(<name>, []), Command(move, False), Argument(<x>, ), Argument(<y>, ), Optional(Option(,--speed,1,10))), Required(Command(ship, False), Command(shoot, False), Argument(<x>, ), Argument(<y>, )), Required(Command(mine, False), Required(Either(Command(set, False), Command(remove, False))), Argument(<x>, ), Argument(<y>, ), Optional(Either(Option(,--moored,0,False), Option(,--drifting,0,False)))), Required(Required(Option(-h,--help,0,False))), Required(Option(,--version,0,False))))
            var a = new RequiredMatcher(1, required.Left, required.Collected);
            while (a.Next())
            {
                // Either(Required(Command(ship, False), Command(new, False), OneOrMore(Argument(<name>, []))), Required(Command(ship, False), Argument(<name>, []), Command(move, False), Argument(<x>, ), Argument(<y>, ), Optional(Option(,--speed,1,10))), Required(Command(ship, False), Command(shoot, False), Argument(<x>, ), Argument(<y>, )), Required(Command(mine, False), Required(Either(Command(set, False), Command(remove, False))), Argument(<x>, ), Argument(<y>, ), Optional(Either(Option(,--moored,0,False), Option(,--drifting,0,False)))), Required(Required(Option(-h,--help,0,False))), Required(Option(,--version,0,False)))
                var b = new EitherMatcher(6, a.Left, a.Collected);
                while (b.Next())
                {
                    switch (b.Index)
                    {
                        case 0:
                        {
                            // Required(Command(ship, False), Command(new, False), OneOrMore(Argument(<name>, [])))
                            var c = new RequiredMatcher(3, b.Left, b.Collected);
                            while (c.Next())
                            {
                                switch (c.Index)
                                {
                                    case 0:
                                    {
                                        // Command(ship, False)
                                        c.Match(PatternMatcher.MatchCommand, "ship", ValueKind.Boolean);
                                    }
                                    break;
                                    case 1:
                                    {
                                        // Command(new, False)
                                        c.Match(PatternMatcher.MatchCommand, "new", ValueKind.Boolean);
                                    }
                                    break;
                                    case 2:
                                    {
                                        // OneOrMore(Argument(<name>, []))
                                        var d = new OneOrMoreMatcher(1, c.Left, c.Collected);
                                        while (d.Next())
                                        {
                                            // Argument(<name>, [])
                                            d.Match(PatternMatcher.MatchArgument, "<name>", ValueKind.StringList);
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
                            // Required(Command(ship, False), Argument(<name>, []), Command(move, False), Argument(<x>, ), Argument(<y>, ), Optional(Option(,--speed,1,10)))
                            var c = new RequiredMatcher(6, b.Left, b.Collected);
                            while (c.Next())
                            {
                                switch (c.Index)
                                {
                                    case 0:
                                    {
                                        // Command(ship, False)
                                        c.Match(PatternMatcher.MatchCommand, "ship", ValueKind.Boolean);
                                    }
                                    break;
                                    case 1:
                                    {
                                        // Argument(<name>, [])
                                        c.Match(PatternMatcher.MatchArgument, "<name>", ValueKind.StringList);
                                    }
                                    break;
                                    case 2:
                                    {
                                        // Command(move, False)
                                        c.Match(PatternMatcher.MatchCommand, "move", ValueKind.Boolean);
                                    }
                                    break;
                                    case 3:
                                    {
                                        // Argument(<x>, )
                                        c.Match(PatternMatcher.MatchArgument, "<x>", ValueKind.None);
                                    }
                                    break;
                                    case 4:
                                    {
                                        // Argument(<y>, )
                                        c.Match(PatternMatcher.MatchArgument, "<y>", ValueKind.None);
                                    }
                                    break;
                                    case 5:
                                    {
                                        // Optional(Option(,--speed,1,10))
                                        var d = new OptionalMatcher(1, c.Left, c.Collected);
                                        while (d.Next())
                                        {
                                            // Option(,--speed,1,10)
                                            d.Match(PatternMatcher.MatchOption, "--speed", ValueKind.String);
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
                        case 2:
                        {
                            // Required(Command(ship, False), Command(shoot, False), Argument(<x>, ), Argument(<y>, ))
                            var c = new RequiredMatcher(4, b.Left, b.Collected);
                            while (c.Next())
                            {
                                switch (c.Index)
                                {
                                    case 0:
                                    {
                                        // Command(ship, False)
                                        c.Match(PatternMatcher.MatchCommand, "ship", ValueKind.Boolean);
                                    }
                                    break;
                                    case 1:
                                    {
                                        // Command(shoot, False)
                                        c.Match(PatternMatcher.MatchCommand, "shoot", ValueKind.Boolean);
                                    }
                                    break;
                                    case 2:
                                    {
                                        // Argument(<x>, )
                                        c.Match(PatternMatcher.MatchArgument, "<x>", ValueKind.None);
                                    }
                                    break;
                                    case 3:
                                    {
                                        // Argument(<y>, )
                                        c.Match(PatternMatcher.MatchArgument, "<y>", ValueKind.None);
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
                            // Required(Command(mine, False), Required(Either(Command(set, False), Command(remove, False))), Argument(<x>, ), Argument(<y>, ), Optional(Either(Option(,--moored,0,False), Option(,--drifting,0,False))))
                            var c = new RequiredMatcher(5, b.Left, b.Collected);
                            while (c.Next())
                            {
                                switch (c.Index)
                                {
                                    case 0:
                                    {
                                        // Command(mine, False)
                                        c.Match(PatternMatcher.MatchCommand, "mine", ValueKind.Boolean);
                                    }
                                    break;
                                    case 1:
                                    {
                                        // Required(Either(Command(set, False), Command(remove, False)))
                                        var d = new RequiredMatcher(1, c.Left, c.Collected);
                                        while (d.Next())
                                        {
                                            // Either(Command(set, False), Command(remove, False))
                                            var e = new EitherMatcher(2, d.Left, d.Collected);
                                            while (e.Next())
                                            {
                                                switch (e.Index)
                                                {
                                                    case 0:
                                                    {
                                                        // Command(set, False)
                                                        e.Match(PatternMatcher.MatchCommand, "set", ValueKind.Boolean);
                                                    }
                                                    break;
                                                    case 1:
                                                    {
                                                        // Command(remove, False)
                                                        e.Match(PatternMatcher.MatchCommand, "remove", ValueKind.Boolean);
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
                                        // Argument(<x>, )
                                        c.Match(PatternMatcher.MatchArgument, "<x>", ValueKind.None);
                                    }
                                    break;
                                    case 3:
                                    {
                                        // Argument(<y>, )
                                        c.Match(PatternMatcher.MatchArgument, "<y>", ValueKind.None);
                                    }
                                    break;
                                    case 4:
                                    {
                                        // Optional(Either(Option(,--moored,0,False), Option(,--drifting,0,False)))
                                        var d = new OptionalMatcher(1, c.Left, c.Collected);
                                        while (d.Next())
                                        {
                                            // Either(Option(,--moored,0,False), Option(,--drifting,0,False))
                                            var e = new EitherMatcher(2, d.Left, d.Collected);
                                            while (e.Next())
                                            {
                                                switch (e.Index)
                                                {
                                                    case 0:
                                                    {
                                                        // Option(,--moored,0,False)
                                                        e.Match(PatternMatcher.MatchOption, "--moored", ValueKind.Boolean);
                                                    }
                                                    break;
                                                    case 1:
                                                    {
                                                        // Option(,--drifting,0,False)
                                                        e.Match(PatternMatcher.MatchOption, "--drifting", ValueKind.Boolean);
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
                        case 4:
                        {
                            // Required(Required(Option(-h,--help,0,False)))
                            var c = new RequiredMatcher(1, b.Left, b.Collected);
                            while (c.Next())
                            {
                                // Required(Option(-h,--help,0,False))
                                var d = new RequiredMatcher(1, c.Left, c.Collected);
                                while (d.Next())
                                {
                                    // Option(-h,--help,0,False)
                                    d.Match(PatternMatcher.MatchOption, "--help", ValueKind.Boolean);
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
                        case 5:
                        {
                            // Required(Option(,--version,0,False))
                            var c = new RequiredMatcher(1, b.Left, b.Collected);
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
