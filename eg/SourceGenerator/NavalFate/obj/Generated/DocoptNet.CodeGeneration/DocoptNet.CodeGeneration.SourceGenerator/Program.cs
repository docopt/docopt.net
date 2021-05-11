using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DocoptNet.Generated;
using Leaves = DocoptNet.Generated.ReadOnlyList<DocoptNet.Generated.LeafPattern>;

namespace NavalFate
{
    partial class Program
    {
        public const string Usage = @"Naval Fate.

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

        // Required:
        //   Either:
        //     Required:
        //       Command(ship, False) -> CommandNode ship Bool
        //       Command(new, False) -> CommandNode new Bool
        //       OneOrMore:
        //         Argument(<name>, []) -> ArgumentNode <name> List
        //     Required:
        //       Command(ship, False) -> CommandNode ship Bool
        //       Argument(<name>, []) -> ArgumentNode <name> List
        //       Command(move, False) -> CommandNode move Bool
        //       Argument(<x>, ) -> ArgumentNode <x> String
        //       Argument(<y>, ) -> ArgumentNode <y> String
        //       Optional:
        //         Option(,--speed,1,10) -> OptionNode speed String
        //     Required:
        //       Command(ship, False) -> CommandNode ship Bool
        //       Command(shoot, False) -> CommandNode shoot Bool
        //       Argument(<x>, ) -> ArgumentNode <x> String
        //       Argument(<y>, ) -> ArgumentNode <y> String
        //     Required:
        //       Command(mine, False) -> CommandNode mine Bool
        //       Required:
        //         Either:
        //           Command(set, False) -> CommandNode set Bool
        //           Command(remove, False) -> CommandNode remove Bool
        //       Argument(<x>, ) -> ArgumentNode <x> String
        //       Argument(<y>, ) -> ArgumentNode <y> String
        //       Optional:
        //         Either:
        //           Option(,--moored,0,False) -> OptionNode moored Bool
        //           Option(,--drifting,0,False) -> OptionNode drifting Bool
        //     Required:
        //       Required:
        //         Option(-h,--help,0,False) -> OptionNode help Bool
        //     Required:
        //       Option(,--version,0,False) -> OptionNode version Bool

        static readonly Pattern Pattern =
            new Required(new Pattern[]
            {
                new Either(new Pattern[]
                {
                    new Required(new Pattern[]
                    {
                        new Command("ship"),
                        new Command("new"),
                        new OneOrMore(
                        new Argument("<name>", (ValueObject)null))
                    }),
                    new Required(new Pattern[]
                    {
                        new Command("ship"),
                        new Argument("<name>", (ValueObject)null),
                        new Command("move"),
                        new Argument("<x>", (ValueObject)null),
                        new Argument("<y>", (ValueObject)null),
                        new Optional(new Pattern[]
                        {
                            new Option("", "--speed", 1, new ValueObject(10))
                        })
                    }),
                    new Required(new Pattern[]
                    {
                        new Command("ship"),
                        new Command("shoot"),
                        new Argument("<x>", (ValueObject)null),
                        new Argument("<y>", (ValueObject)null)
                    }),
                    new Required(new Pattern[]
                    {
                        new Command("mine"),
                        new Required(new Pattern[]
                        {
                            new Either(new Pattern[]
                            {
                                new Command("set"),
                                new Command("remove")
                            })
                        }),
                        new Argument("<x>", (ValueObject)null),
                        new Argument("<y>", (ValueObject)null),
                        new Optional(new Pattern[]
                        {
                            new Either(new Pattern[]
                            {
                                new Option("", "--moored", 0, new ValueObject(false)),
                                new Option("", "--drifting", 0, new ValueObject(false))
                            })
                        })
                    }),
                    new Required(new Pattern[]
                    {
                        new Required(new Pattern[]
                        {
                            new Option("-h", "--help", 0, new ValueObject(false))
                        })
                    }),
                    new Required(new Pattern[]
                    {
                        new Option("", "--version", 0, new ValueObject(false))
                    })
                })
            });

        static readonly ICollection<Option> Options = new[]
        {
            new Option("-h", "--help", 0, new ValueObject(false)),
            new Option("", "--version", 0, new ValueObject(false)),
            new Option("", "--speed", 1, new ValueObject(10)),
            new Option("", "--moored", 0, new ValueObject(false)),
            new Option("", "--drifting", 0, new ValueObject(false)),
        };

        static Dictionary<string, ValueObject> Apply(IEnumerable<string> args, bool help = true, object version = null, bool optionsFirst = false, bool exit = false)
        {
            var tokens = new Tokens(args, typeof(DocoptInputErrorException));
            var arguments = Docopt.ParseArgv(tokens, Options, optionsFirst).AsReadOnly();;
            if (help && arguments.Any(o => o is { Name: "-h" or "--help", Value: { IsNullOrEmpty: false } }))
            {
                throw new DocoptExitException(Usage);
            }
            if (version is not null && arguments.Any(o => o is { Name: "--version", Value: { IsNullOrEmpty: false } }))
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
                                            d.Match(PatternMatcher.MatchCommand, "ship", value: false, isList: false, isInt: false);
                                            break;
                                        }
                                        case 1:
                                        {
                                            // Command(new, False)
                                            d.Match(PatternMatcher.MatchCommand, "new", value: false, isList: false, isInt: false);
                                            break;
                                        }
                                        case 2:
                                        {
                                            // OneOrMore(Argument(<name>, []))
                                            var e = new OneOrMoreMatcher(1, d.Left, d.Collected);
                                            while (e.Next())
                                            {
                                                // Argument(<name>, [])
                                                e.Match(PatternMatcher.MatchArgument, "<name>", value: new ArrayList(), isList: true, isInt: false);
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
                                            d.Match(PatternMatcher.MatchCommand, "ship", value: false, isList: false, isInt: false);
                                            break;
                                        }
                                        case 1:
                                        {
                                            // Argument(<name>, [])
                                            d.Match(PatternMatcher.MatchArgument, "<name>", value: new ArrayList(), isList: true, isInt: false);
                                            break;
                                        }
                                        case 2:
                                        {
                                            // Command(move, False)
                                            d.Match(PatternMatcher.MatchCommand, "move", value: false, isList: false, isInt: false);
                                            break;
                                        }
                                        case 3:
                                        {
                                            // Argument(<x>, )
                                            d.Match(PatternMatcher.MatchArgument, "<x>", value: null, isList: false, isInt: false);
                                            break;
                                        }
                                        case 4:
                                        {
                                            // Argument(<y>, )
                                            d.Match(PatternMatcher.MatchArgument, "<y>", value: null, isList: false, isInt: false);
                                            break;
                                        }
                                        case 5:
                                        {
                                            // Optional(Option(,--speed,1,10))
                                            var e = new OptionalMatcher(1, d.Left, d.Collected);
                                            while (e.Next())
                                            {
                                                // Option(,--speed,1,10)
                                                e.Match(PatternMatcher.MatchOption, "--speed", value: 10, isList: false, isInt: false);
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
                                            d.Match(PatternMatcher.MatchCommand, "ship", value: false, isList: false, isInt: false);
                                            break;
                                        }
                                        case 1:
                                        {
                                            // Command(shoot, False)
                                            d.Match(PatternMatcher.MatchCommand, "shoot", value: false, isList: false, isInt: false);
                                            break;
                                        }
                                        case 2:
                                        {
                                            // Argument(<x>, )
                                            d.Match(PatternMatcher.MatchArgument, "<x>", value: null, isList: false, isInt: false);
                                            break;
                                        }
                                        case 3:
                                        {
                                            // Argument(<y>, )
                                            d.Match(PatternMatcher.MatchArgument, "<y>", value: null, isList: false, isInt: false);
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
                                            d.Match(PatternMatcher.MatchCommand, "mine", value: false, isList: false, isInt: false);
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
                                                            f.Match(PatternMatcher.MatchCommand, "set", value: false, isList: false, isInt: false);
                                                            break;
                                                        }
                                                        case 1:
                                                        {
                                                            // Command(remove, False)
                                                            f.Match(PatternMatcher.MatchCommand, "remove", value: false, isList: false, isInt: false);
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
                                            d.Match(PatternMatcher.MatchArgument, "<x>", value: null, isList: false, isInt: false);
                                            break;
                                        }
                                        case 3:
                                        {
                                            // Argument(<y>, )
                                            d.Match(PatternMatcher.MatchArgument, "<y>", value: null, isList: false, isInt: false);
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
                                                            f.Match(PatternMatcher.MatchOption, "--moored", value: false, isList: false, isInt: false);
                                                            break;
                                                        }
                                                        case 1:
                                                        {
                                                            // Option(,--drifting,0,False)
                                                            f.Match(PatternMatcher.MatchOption, "--drifting", value: false, isList: false, isInt: false);
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
                                        e.Match(PatternMatcher.MatchOption, "--help", value: false, isList: false, isInt: false);
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
                                    d.Match(PatternMatcher.MatchOption, "--version", value: false, isList: false, isInt: false);
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

            if (!a.Result)
            {
                const string exitUsage = @"Usage:
      naval_fate.exe ship new <name>...
      naval_fate.exe ship <name> move <x> <y> [--speed=<kn>]
      naval_fate.exe ship shoot <x> <y>
      naval_fate.exe mine (set|remove) <x> <y> [--moored | --drifting]
      naval_fate.exe (-h | --help)
      naval_fate.exe --version";
                throw new DocoptInputErrorException(exitUsage);
            }

            var dict = new Dictionary<string, ValueObject>
            {
                [@"ship"] = new ValueObject(false),
                [@"new"] = new ValueObject(false),
                [@"<name>"] = new ValueObject(new ArrayList()),
                [@"ship"] = new ValueObject(false),
                [@"<name>"] = new ValueObject(new ArrayList()),
                [@"move"] = new ValueObject(false),
                [@"<x>"] = new ValueObject(null),
                [@"<y>"] = new ValueObject(null),
                [@"--speed"] = new ValueObject(10),
                [@"ship"] = new ValueObject(false),
                [@"shoot"] = new ValueObject(false),
                [@"<x>"] = new ValueObject(null),
                [@"<y>"] = new ValueObject(null),
                [@"mine"] = new ValueObject(false),
                [@"set"] = new ValueObject(false),
                [@"remove"] = new ValueObject(false),
                [@"<x>"] = new ValueObject(null),
                [@"<y>"] = new ValueObject(null),
                [@"--moored"] = new ValueObject(false),
                [@"--drifting"] = new ValueObject(false),
                [@"--help"] = new ValueObject(false),
                [@"--version"] = new ValueObject(false),
            };

            collected = a.Collected;
            foreach (var p in collected)
            {
                dict[p.Name] = p.Value;
            }

            return dict;
        }

        public bool CmdShip { get { return _args["ship"].IsTrue; } }
        public bool CmdNew { get { return _args["new"].IsTrue; } }
        public ArrayList ArgName  { get { return _args["<name>"].AsList; } }
        public bool CmdMove { get { return _args["move"].IsTrue; } }
        public string ArgX  { get { return null == _args["<x>"] ? null : _args["<x>"].ToString(); } }
        public string ArgY  { get { return null == _args["<y>"] ? null : _args["<y>"].ToString(); } }
        public string OptSpeed { get { return null == _args["--speed"] ? "10" : _args["--speed"].ToString(); } }
        public bool CmdShoot { get { return _args["shoot"].IsTrue; } }
        public bool CmdMine { get { return _args["mine"].IsTrue; } }
        public bool CmdSet { get { return _args["set"].IsTrue; } }
        public bool CmdRemove { get { return _args["remove"].IsTrue; } }
        public bool OptMoored { get { return _args["--moored"].IsTrue; } }
        public bool OptDrifting { get { return _args["--drifting"].IsTrue; } }
        public bool OptHelp { get { return _args["--help"].IsTrue; } }
        public bool OptVersion { get { return _args["--version"].IsTrue; } }
    }
}
