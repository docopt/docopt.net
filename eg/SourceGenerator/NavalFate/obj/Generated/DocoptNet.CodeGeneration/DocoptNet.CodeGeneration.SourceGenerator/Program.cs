using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DocoptNet.Generated;
using Leaves = DocoptNet.Generated.ReadOnlyList<DocoptNet.Generated.LeafPattern>;
using static DocoptNet.Generated.Module;

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
        }
        ;

        static void Apply(string[] args, bool help = true, object version = null, bool optionsFirst = false, bool exit = false)
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
            var rm = false; var rl = left; var rc = collected;
            do
            {
                // Required(Either(Required(Command(ship, False), Command(new, False), OneOrMore(Argument(<name>, []))), Required(Command(ship, False), Argument(<name>, []), Command(move, False), Argument(<x>, ), Argument(<y>, ), Optional(Option(,--speed,1,10))), Required(Command(ship, False), Command(shoot, False), Argument(<x>, ), Argument(<y>, )), Required(Command(mine, False), Required(Either(Command(set, False), Command(remove, False))), Argument(<x>, ), Argument(<y>, ), Optional(Either(Option(,--moored,0,False), Option(,--drifting,0,False)))), Required(Required(Option(-h,--help,0,False))), Required(Option(,--version,0,False))))
                var la = left;
                var ca = collected;
                {
                    // Either(Required(Command(ship, False), Command(new, False), OneOrMore(Argument(<name>, []))), Required(Command(ship, False), Argument(<name>, []), Command(move, False), Argument(<x>, ), Argument(<y>, ), Optional(Option(,--speed,1,10))), Required(Command(ship, False), Command(shoot, False), Argument(<x>, ), Argument(<y>, )), Required(Command(mine, False), Required(Either(Command(set, False), Command(remove, False))), Argument(<x>, ), Argument(<y>, ), Optional(Either(Option(,--moored,0,False), Option(,--drifting,0,False)))), Required(Required(Option(-h,--help,0,False))), Required(Option(,--version,0,False)))
                    var emb = false;
                    var elb = la;
                    var ecb = ca;
                    for (var iib = 0; iib < 6; iib++)
                    {
                        switch (iib)
                        {
                            case 0:
                            {
                                // Required(Command(ship, False), Command(new, False), OneOrMore(Argument(<name>, [])))
                                var lc = la;
                                var cc = ca;
                                for (var iic = 0; iic < 3; iic++)
                                {
                                    switch (iic)
                                    {
                                        case 0:
                                        {
                                            // Command(ship, False)
                                            var (i, match) = Command(lc, "ship");
                                            (rm, rl, rc) = Leaf(lc, cc, "ship", value: false, isList: false, isInt: false, i, match);
                                            break;
                                        }
                                        case 1:
                                        {
                                            // Command(new, False)
                                            var (i, match) = Command(lc, "new");
                                            (rm, rl, rc) = Leaf(lc, cc, "new", value: false, isList: false, isInt: false, i, match);
                                            break;
                                        }
                                        case 2:
                                        {
                                            // OneOrMore(Argument(<name>, []))
                                            var ld = lc;
                                            var cd = cc;
                                            var td = 0;
                                            var lld = default(Leaves?);
                                            while (true)
                                            {
                                                // Argument(<name>, [])
                                                var (i, match) = Argument(ld, "<name>");
                                                (rm, rl, rc) = Leaf(ld, cd, "<name>", value: new ArrayList(), isList: true, isInt: false, i, match);
                                                td += rm ? 0 : 1;
                                                if (lld is {} l_ && l_.Equals(ld))
                                                    break;
                                            }
                                            if (td >= 0)
                                            {
                                                rm = true;
                                                rl = ld;
                                                rc = cd;
                                            }
                                            else
                                            {
                                                rm = true;
                                                rl = lc;
                                                rc = cc;
                                            }
                                            break;
                                        }
                                    }
                                    if (!rm)
                                    {
                                        rl = la;
                                        rc = ca;
                                        break;
                                    }
                                    lc = rl;
                                    cc = rc;
                                }
                                break;
                            }
                            case 1:
                            {
                                // Required(Command(ship, False), Argument(<name>, []), Command(move, False), Argument(<x>, ), Argument(<y>, ), Optional(Option(,--speed,1,10)))
                                var lc = la;
                                var cc = ca;
                                for (var iic = 0; iic < 6; iic++)
                                {
                                    switch (iic)
                                    {
                                        case 0:
                                        {
                                            // Command(ship, False)
                                            var (i, match) = Command(lc, "ship");
                                            (rm, rl, rc) = Leaf(lc, cc, "ship", value: false, isList: false, isInt: false, i, match);
                                            break;
                                        }
                                        case 1:
                                        {
                                            // Argument(<name>, [])
                                            var (i, match) = Argument(lc, "<name>");
                                            (rm, rl, rc) = Leaf(lc, cc, "<name>", value: new ArrayList(), isList: true, isInt: false, i, match);
                                            break;
                                        }
                                        case 2:
                                        {
                                            // Command(move, False)
                                            var (i, match) = Command(lc, "move");
                                            (rm, rl, rc) = Leaf(lc, cc, "move", value: false, isList: false, isInt: false, i, match);
                                            break;
                                        }
                                        case 3:
                                        {
                                            // Argument(<x>, )
                                            var (i, match) = Argument(lc, "<x>");
                                            (rm, rl, rc) = Leaf(lc, cc, "<x>", value: null, isList: false, isInt: false, i, match);
                                            break;
                                        }
                                        case 4:
                                        {
                                            // Argument(<y>, )
                                            var (i, match) = Argument(lc, "<y>");
                                            (rm, rl, rc) = Leaf(lc, cc, "<y>", value: null, isList: false, isInt: false, i, match);
                                            break;
                                        }
                                        case 5:
                                        {
                                            // Optional(Option(,--speed,1,10))
                                            var ld = lc;
                                            var cd = cc;
                                            {
                                                // Option(,--speed,1,10)
                                                var (i, match) = Option(ld, "--speed");
                                                (rm, rl, rc) = Leaf(ld, cd, "--speed", value: 10, isList: false, isInt: false, i, match);
                                                ld = rl;
                                                cd = rc;
                                            }
                                            rm = true;
                                            rl = ld;
                                            rc = cd;
                                            break;
                                        }
                                    }
                                    if (!rm)
                                    {
                                        rl = la;
                                        rc = ca;
                                        break;
                                    }
                                    lc = rl;
                                    cc = rc;
                                }
                                break;
                            }
                            case 2:
                            {
                                // Required(Command(ship, False), Command(shoot, False), Argument(<x>, ), Argument(<y>, ))
                                var lc = la;
                                var cc = ca;
                                for (var iic = 0; iic < 4; iic++)
                                {
                                    switch (iic)
                                    {
                                        case 0:
                                        {
                                            // Command(ship, False)
                                            var (i, match) = Command(lc, "ship");
                                            (rm, rl, rc) = Leaf(lc, cc, "ship", value: false, isList: false, isInt: false, i, match);
                                            break;
                                        }
                                        case 1:
                                        {
                                            // Command(shoot, False)
                                            var (i, match) = Command(lc, "shoot");
                                            (rm, rl, rc) = Leaf(lc, cc, "shoot", value: false, isList: false, isInt: false, i, match);
                                            break;
                                        }
                                        case 2:
                                        {
                                            // Argument(<x>, )
                                            var (i, match) = Argument(lc, "<x>");
                                            (rm, rl, rc) = Leaf(lc, cc, "<x>", value: null, isList: false, isInt: false, i, match);
                                            break;
                                        }
                                        case 3:
                                        {
                                            // Argument(<y>, )
                                            var (i, match) = Argument(lc, "<y>");
                                            (rm, rl, rc) = Leaf(lc, cc, "<y>", value: null, isList: false, isInt: false, i, match);
                                            break;
                                        }
                                    }
                                    if (!rm)
                                    {
                                        rl = la;
                                        rc = ca;
                                        break;
                                    }
                                    lc = rl;
                                    cc = rc;
                                }
                                break;
                            }
                            case 3:
                            {
                                // Required(Command(mine, False), Required(Either(Command(set, False), Command(remove, False))), Argument(<x>, ), Argument(<y>, ), Optional(Either(Option(,--moored,0,False), Option(,--drifting,0,False))))
                                var lc = la;
                                var cc = ca;
                                for (var iic = 0; iic < 5; iic++)
                                {
                                    switch (iic)
                                    {
                                        case 0:
                                        {
                                            // Command(mine, False)
                                            var (i, match) = Command(lc, "mine");
                                            (rm, rl, rc) = Leaf(lc, cc, "mine", value: false, isList: false, isInt: false, i, match);
                                            break;
                                        }
                                        case 1:
                                        {
                                            // Required(Either(Command(set, False), Command(remove, False)))
                                            var ld = lc;
                                            var cd = cc;
                                            {
                                                // Either(Command(set, False), Command(remove, False))
                                                var eme = false;
                                                var ele = ld;
                                                var ece = cd;
                                                for (var iie = 0; iie < 2; iie++)
                                                {
                                                    switch (iie)
                                                    {
                                                        case 0:
                                                        {
                                                            // Command(set, False)
                                                            var (i, match) = Command(ld, "set");
                                                            (rm, rl, rc) = Leaf(ld, cd, "set", value: false, isList: false, isInt: false, i, match);
                                                            break;
                                                        }
                                                        case 1:
                                                        {
                                                            // Command(remove, False)
                                                            var (i, match) = Command(ld, "remove");
                                                            (rm, rl, rc) = Leaf(ld, cd, "remove", value: false, isList: false, isInt: false, i, match);
                                                            break;
                                                        }
                                                    }
                                                    if (rm && (eme || rl.Count < ele.Count))
                                                    {
                                                        eme = true;
                                                        ele = rl;
                                                        ece = rc;
                                                    }
                                                }
                                                rm = eme;
                                                rl = ele;
                                                rc = ece;
                                                if (!rm)
                                                {
                                                    rl = lc;
                                                    rc = cc;
                                                    break;
                                                }
                                                ld = rl;
                                                cd = rc;
                                            }
                                            break;
                                        }
                                        case 2:
                                        {
                                            // Argument(<x>, )
                                            var (i, match) = Argument(lc, "<x>");
                                            (rm, rl, rc) = Leaf(lc, cc, "<x>", value: null, isList: false, isInt: false, i, match);
                                            break;
                                        }
                                        case 3:
                                        {
                                            // Argument(<y>, )
                                            var (i, match) = Argument(lc, "<y>");
                                            (rm, rl, rc) = Leaf(lc, cc, "<y>", value: null, isList: false, isInt: false, i, match);
                                            break;
                                        }
                                        case 4:
                                        {
                                            // Optional(Either(Option(,--moored,0,False), Option(,--drifting,0,False)))
                                            var ld = lc;
                                            var cd = cc;
                                            {
                                                // Either(Option(,--moored,0,False), Option(,--drifting,0,False))
                                                var eme = false;
                                                var ele = ld;
                                                var ece = cd;
                                                for (var iie = 0; iie < 2; iie++)
                                                {
                                                    switch (iie)
                                                    {
                                                        case 0:
                                                        {
                                                            // Option(,--moored,0,False)
                                                            var (i, match) = Option(ld, "--moored");
                                                            (rm, rl, rc) = Leaf(ld, cd, "--moored", value: false, isList: false, isInt: false, i, match);
                                                            break;
                                                        }
                                                        case 1:
                                                        {
                                                            // Option(,--drifting,0,False)
                                                            var (i, match) = Option(ld, "--drifting");
                                                            (rm, rl, rc) = Leaf(ld, cd, "--drifting", value: false, isList: false, isInt: false, i, match);
                                                            break;
                                                        }
                                                    }
                                                    if (rm && (eme || rl.Count < ele.Count))
                                                    {
                                                        eme = true;
                                                        ele = rl;
                                                        ece = rc;
                                                    }
                                                }
                                                rm = eme;
                                                rl = ele;
                                                rc = ece;
                                                ld = rl;
                                                cd = rc;
                                            }
                                            rm = true;
                                            rl = ld;
                                            rc = cd;
                                            break;
                                        }
                                    }
                                    if (!rm)
                                    {
                                        rl = la;
                                        rc = ca;
                                        break;
                                    }
                                    lc = rl;
                                    cc = rc;
                                }
                                break;
                            }
                            case 4:
                            {
                                // Required(Required(Option(-h,--help,0,False)))
                                var lc = la;
                                var cc = ca;
                                {
                                    // Required(Option(-h,--help,0,False))
                                    var ld = lc;
                                    var cd = cc;
                                    {
                                        // Option(-h,--help,0,False)
                                        var (i, match) = Option(ld, "--help");
                                        (rm, rl, rc) = Leaf(ld, cd, "--help", value: false, isList: false, isInt: false, i, match);
                                        if (!rm)
                                        {
                                            rl = lc;
                                            rc = cc;
                                            break;
                                        }
                                        ld = rl;
                                        cd = rc;
                                    }
                                    if (!rm)
                                    {
                                        rl = la;
                                        rc = ca;
                                        break;
                                    }
                                    lc = rl;
                                    cc = rc;
                                }
                                break;
                            }
                            case 5:
                            {
                                // Required(Option(,--version,0,False))
                                var lc = la;
                                var cc = ca;
                                {
                                    // Option(,--version,0,False)
                                    var (i, match) = Option(lc, "--version");
                                    (rm, rl, rc) = Leaf(lc, cc, "--version", value: false, isList: false, isInt: false, i, match);
                                    if (!rm)
                                    {
                                        rl = la;
                                        rc = ca;
                                        break;
                                    }
                                    lc = rl;
                                    cc = rc;
                                }
                                break;
                            }
                        }
                        if (rm && (emb || rl.Count < elb.Count))
                        {
                            emb = true;
                            elb = rl;
                            ecb = rc;
                        }
                    }
                    rm = emb;
                    rl = elb;
                    rc = ecb;
                    if (!rm)
                    {
                        rl = left;
                        rc = collected;
                        break;
                    }
                    la = rl;
                    ca = rc;
                }
            }
            while (false);
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
