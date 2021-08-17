using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DocoptNet.Generated;
using Leaves = DocoptNet.Generated.ReadOnlyList<DocoptNet.Generated.LeafPattern>;

namespace CalculatorExample
{
    partial class Program
    {
        public const string Usage = @"Not a serious example.

Usage:
  calculator_example.py <value> ( ( + | - | * | / ) <value> )...
  calculator_example.py <function> <value> [( , <value> )]...
  calculator_example.py (-h | --help)

Examples:
  calculator_example.py 1 + 2 + 3 + 4 + 5
  calculator_example.py 1 + 2 '*' 3 / 4 - 5    # note quotes around '*'
  calculator_example.py sum 10 , 20 , 30 , 40

Options:
  -h, --help
";

        // Required:
        //   Either:
        //     Required:
        //       Argument(<value>, []) -> ArgumentNode <value> List
        //       OneOrMore:
        //         Required:
        //           Required:
        //             Either:
        //               Command(+, 0) -> CommandNode + Bool
        //               Command(-, 0) -> CommandNode - Bool
        //               Command(*, 0) -> CommandNode * Bool
        //               Command(/, 0) -> CommandNode / Bool
        //           Argument(<value>, []) -> ArgumentNode <value> List
        //     Required:
        //       Argument(<function>, ) -> ArgumentNode <function> String
        //       Argument(<value>, []) -> ArgumentNode <value> List
        //       OneOrMore:
        //         Optional:
        //           Required:
        //             Command(,, 0) -> CommandNode , Bool
        //             Argument(<value>, []) -> ArgumentNode <value> List
        //     Required:
        //       Required:
        //         Option(-h,--help,0,False) -> OptionNode help Bool

        static readonly Pattern Pattern =
            new Required(new Pattern[]
            {
                new Either(new Pattern[]
                {
                    new Required(new Pattern[]
                    {
                        new Argument("<value>"),
                        new OneOrMore(
                        new Required(new Pattern[]
                        {
                            new Required(new Pattern[]
                            {
                                new Either(new Pattern[]
                                {
                                    new Command("+"),
                                    new Command("-"),
                                    new Command("*"),
                                    new Command("/")
                                })
                            }),
                            new Argument("<value>")
                        }))
                    }),
                    new Required(new Pattern[]
                    {
                        new Argument("<function>"),
                        new Argument("<value>"),
                        new OneOrMore(
                        new Optional(new Pattern[]
                        {
                            new Required(new Pattern[]
                            {
                                new Command(","),
                                new Argument("<value>")
                            })
                        }))
                    }),
                    new Required(new Pattern[]
                    {
                        new Required(new Pattern[]
                        {
                            new Option("-h", "--help", 0, false)
                        })
                    })
                })
            });

        static readonly ICollection<Option> Options = new Option[]
        {
            new Option("-h", "--help", 0, false),
        };

        static Dictionary<string, ValueObject> Apply(IEnumerable<string> args, bool help = true, object version = null, bool optionsFirst = false, bool exit = false)
        {
            var tokens = new Tokens(args, typeof(DocoptInputErrorException));
            var options = Options.Select(e => new Option(e.ShortName, e.LongName, e.ArgCount, e.Value)).ToList();
            var arguments = Docopt.ParseArgv(tokens, options, optionsFirst).AsReadOnly();
            if (help && arguments.Any(o => o is { Name: "-h" or "--help", Value: { Box: null or string { Length: 0 } } }))
            {
                throw new DocoptExitException(Usage);
            }
            if (version is not null && arguments.Any(o => o is { Name: "--version", Value: { Box: null or string { Length: 0 } } }))
            {
                throw new DocoptExitException(version.ToString());
            }
            var left = arguments;
            var collected = new Leaves();
            var a = new RequiredMatcher(1, left, collected);
            do
            {
                // Required(Either(Required(Argument(<value>, []), OneOrMore(Required(Required(Either(Command(+, 0), Command(-, 0), Command(*, 0), Command(/, 0))), Argument(<value>, [])))), Required(Argument(<function>, ), Argument(<value>, []), OneOrMore(Optional(Required(Command(,, 0), Argument(<value>, []))))), Required(Required(Option(-h,--help,0,False)))))
                var b = new RequiredMatcher(1, a.Left, a.Collected);
                while (b.Next())
                {
                    // Either(Required(Argument(<value>, []), OneOrMore(Required(Required(Either(Command(+, 0), Command(-, 0), Command(*, 0), Command(/, 0))), Argument(<value>, [])))), Required(Argument(<function>, ), Argument(<value>, []), OneOrMore(Optional(Required(Command(,, 0), Argument(<value>, []))))), Required(Required(Option(-h,--help,0,False))))
                    var c = new EitherMatcher(3, b.Left, b.Collected);
                    while (c.Next())
                    {
                        switch (c.Index)
                        {
                            case 0:
                            {
                                // Required(Argument(<value>, []), OneOrMore(Required(Required(Either(Command(+, 0), Command(-, 0), Command(*, 0), Command(/, 0))), Argument(<value>, []))))
                                var d = new RequiredMatcher(2, c.Left, c.Collected);
                                while (d.Next())
                                {
                                    switch (d.Index)
                                    {
                                        case 0:
                                        {
                                            // Argument(<value>, [])
                                            d.Match(PatternMatcher.MatchArgument, "<value>", value: new ArrayList(), isList: true, isInt: false);
                                            break;
                                        }
                                        case 1:
                                        {
                                            // OneOrMore(Required(Required(Either(Command(+, 0), Command(-, 0), Command(*, 0), Command(/, 0))), Argument(<value>, [])))
                                            var e = new OneOrMoreMatcher(1, d.Left, d.Collected);
                                            while (e.Next())
                                            {
                                                // Required(Required(Either(Command(+, 0), Command(-, 0), Command(*, 0), Command(/, 0))), Argument(<value>, []))
                                                var f = new RequiredMatcher(2, e.Left, e.Collected);
                                                while (f.Next())
                                                {
                                                    switch (f.Index)
                                                    {
                                                        case 0:
                                                        {
                                                            // Required(Either(Command(+, 0), Command(-, 0), Command(*, 0), Command(/, 0)))
                                                            var g = new RequiredMatcher(1, f.Left, f.Collected);
                                                            while (g.Next())
                                                            {
                                                                // Either(Command(+, 0), Command(-, 0), Command(*, 0), Command(/, 0))
                                                                var h = new EitherMatcher(4, g.Left, g.Collected);
                                                                while (h.Next())
                                                                {
                                                                    switch (h.Index)
                                                                    {
                                                                        case 0:
                                                                        {
                                                                            // Command(+, 0)
                                                                            h.Match(PatternMatcher.MatchCommand, "+", value: 0, isList: false, isInt: true);
                                                                            break;
                                                                        }
                                                                        case 1:
                                                                        {
                                                                            // Command(-, 0)
                                                                            h.Match(PatternMatcher.MatchCommand, "-", value: 0, isList: false, isInt: true);
                                                                            break;
                                                                        }
                                                                        case 2:
                                                                        {
                                                                            // Command(*, 0)
                                                                            h.Match(PatternMatcher.MatchCommand, "*", value: 0, isList: false, isInt: true);
                                                                            break;
                                                                        }
                                                                        case 3:
                                                                        {
                                                                            // Command(/, 0)
                                                                            h.Match(PatternMatcher.MatchCommand, "/", value: 0, isList: false, isInt: true);
                                                                            break;
                                                                        }
                                                                    }
                                                                    if (!h.LastMatched)
                                                                        break;
                                                                }
                                                                g.Fold(h.Result);
                                                                if (!g.LastMatched)
                                                                    break;
                                                            }
                                                            f.Fold(g.Result);
                                                            break;
                                                        }
                                                        case 1:
                                                        {
                                                            // Argument(<value>, [])
                                                            f.Match(PatternMatcher.MatchArgument, "<value>", value: new ArrayList(), isList: true, isInt: false);
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
                            case 1:
                            {
                                // Required(Argument(<function>, ), Argument(<value>, []), OneOrMore(Optional(Required(Command(,, 0), Argument(<value>, [])))))
                                var d = new RequiredMatcher(3, c.Left, c.Collected);
                                while (d.Next())
                                {
                                    switch (d.Index)
                                    {
                                        case 0:
                                        {
                                            // Argument(<function>, )
                                            d.Match(PatternMatcher.MatchArgument, "<function>", value: null, isList: false, isInt: false);
                                            break;
                                        }
                                        case 1:
                                        {
                                            // Argument(<value>, [])
                                            d.Match(PatternMatcher.MatchArgument, "<value>", value: new ArrayList(), isList: true, isInt: false);
                                            break;
                                        }
                                        case 2:
                                        {
                                            // OneOrMore(Optional(Required(Command(,, 0), Argument(<value>, []))))
                                            var e = new OneOrMoreMatcher(1, d.Left, d.Collected);
                                            while (e.Next())
                                            {
                                                // Optional(Required(Command(,, 0), Argument(<value>, [])))
                                                var f = new OptionalMatcher(1, e.Left, e.Collected);
                                                while (f.Next())
                                                {
                                                    // Required(Command(,, 0), Argument(<value>, []))
                                                    var g = new RequiredMatcher(2, f.Left, f.Collected);
                                                    while (g.Next())
                                                    {
                                                        switch (g.Index)
                                                        {
                                                            case 0:
                                                            {
                                                                // Command(,, 0)
                                                                g.Match(PatternMatcher.MatchCommand, ",", value: 0, isList: false, isInt: true);
                                                                break;
                                                            }
                                                            case 1:
                                                            {
                                                                // Argument(<value>, [])
                                                                g.Match(PatternMatcher.MatchArgument, "<value>", value: new ArrayList(), isList: true, isInt: false);
                                                                break;
                                                            }
                                                        }
                                                        if (!g.LastMatched)
                                                            break;
                                                    }
                                                    f.Fold(g.Result);
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
                            case 2:
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
                const string exitUsage = @"Usage:
  calculator_example.py <value> ( ( + | - | * | / ) <value> )...
  calculator_example.py <function> <value> [( , <value> )]...
  calculator_example.py (-h | --help)";
                throw new DocoptInputErrorException(exitUsage);
            }

            var dict = new Dictionary<string, ValueObject>
            {
                [@"<value>"] = new ValueObject(new ArrayList()),
                [@"+"] = new ValueObject(0),
                [@"-"] = new ValueObject(0),
                [@"*"] = new ValueObject(0),
                [@"/"] = new ValueObject(0),
                [@"<value>"] = new ValueObject(new ArrayList()),
                [@"<function>"] = new ValueObject(null),
                [@"<value>"] = new ValueObject(new ArrayList()),
                [@","] = new ValueObject(0),
                [@"<value>"] = new ValueObject(new ArrayList()),
                [@"--help"] = new ValueObject(false),
            };

            collected = a.Collected;
            foreach (var p in collected)
            {
                dict[p.Name] = (p.Value.Box is StringList list ? list.Reverse() : p.Value).ToValueObject();
            }

            return dict;
        }

        public ArrayList ArgValue { get { return _args["<value>"].AsList; } }
        public bool CmdPlus { get { ValueObject v = _args["+"]; return v.IsTrue || v.IsOfTypeInt && v.AsInt > 0; } }
        public bool CmdMinus { get { ValueObject v = _args["-"]; return v.IsTrue || v.IsOfTypeInt && v.AsInt > 0; } }
        public bool CmdStar { get { ValueObject v = _args["*"]; return v.IsTrue || v.IsOfTypeInt && v.AsInt > 0; } }
        public bool CmdSlash { get { ValueObject v = _args["/"]; return v.IsTrue || v.IsOfTypeInt && v.AsInt > 0; } }
        public string ArgFunction { get { return null == _args["<function>"] ? null : _args["<function>"].ToString(); } }
        public bool CmdComma { get { ValueObject v = _args[","]; return v.IsTrue || v.IsOfTypeInt && v.AsInt > 0; } }
        public bool OptHelp { get { ValueObject v = _args["--help"]; return v.IsTrue || v.IsOfTypeInt && v.AsInt > 0; } }
    }
}
