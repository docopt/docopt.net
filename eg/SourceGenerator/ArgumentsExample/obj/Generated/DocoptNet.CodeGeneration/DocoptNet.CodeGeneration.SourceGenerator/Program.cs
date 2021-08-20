using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DocoptNet.Generated;
using Leaves = DocoptNet.Generated.ReadOnlyList<DocoptNet.Generated.LeafPattern>;

namespace ArgumentsExample
{
    partial class Program
    {
        public const string Usage = @"Usage: ArgumentsExample [-vqrh] [FILE] ...
       ArgumentsExample (--left | --right) CORRECTION FILE

Process FILE and optionally apply correction to either left-hand side or
right-hand side.

Arguments:
  FILE        optional input file
  CORRECTION  correction angle, needs FILE, --left or --right to be present

Options:
  -h --help
  -v       verbose mode
  -q       quiet mode
  -r       make report
  --left   use left-hand side
  --right  use right-hand side
";

        // Required:
        //   Either:
        //     Required:
        //       Optional:
        //         Option(-v,,0,False) -> OptionNode v Bool
        //         Option(-q,,0,False) -> OptionNode q Bool
        //         Option(-r,,0,False) -> OptionNode r Bool
        //         Option(-h,--help,0,False) -> OptionNode help Bool
        //       OneOrMore:
        //         Optional:
        //           Argument(FILE, []) -> ArgumentNode FILE List
        //     Required:
        //       Required:
        //         Either:
        //           Option(,--left,0,False) -> OptionNode left Bool
        //           Option(,--right,0,False) -> OptionNode right Bool
        //       Argument(CORRECTION, ) -> ArgumentNode CORRECTION String
        //       Argument(FILE, []) -> ArgumentNode FILE List

        static readonly Pattern Pattern =
            new Required(new Pattern[]
            {
                new Either(new Pattern[]
                {
                    new Required(new Pattern[]
                    {
                        new Optional(new Pattern[]
                        {
                            new Option("-v", null, 0, false),
                            new Option("-q", null, 0, false),
                            new Option("-r", null, 0, false),
                            new Option("-h", "--help", 0, false)
                        }),
                        new OneOrMore(
                        new Optional(new Pattern[]
                        {
                            new Argument("FILE")
                        }))
                    }),
                    new Required(new Pattern[]
                    {
                        new Required(new Pattern[]
                        {
                            new Either(new Pattern[]
                            {
                                new Option(null, "--left", 0, false),
                                new Option(null, "--right", 0, false)
                            })
                        }),
                        new Argument("CORRECTION"),
                        new Argument("FILE")
                    })
                })
            });

        static readonly ICollection<Option> Options = new Option[]
        {
            new Option("-h", "--help", 0, false),
            new Option("-v", null, 0, false),
            new Option("-q", null, 0, false),
            new Option("-r", null, 0, false),
            new Option(null, "--left", 0, false),
            new Option(null, "--right", 0, false),
        };

        static Dictionary<string, Value> Apply(IEnumerable<string> args, bool help = true, object version = null, bool optionsFirst = false, bool exit = false)
        {
            var tokens = new Tokens(args, typeof(DocoptInputErrorException));
            var options = Options.Select(e => new Option(e.ShortName, e.LongName, e.ArgCount, e.Value)).ToList();
            var arguments = Docopt.ParseArgv(tokens, options, optionsFirst).AsReadOnly();
            if (help && arguments.Any(o => o is { Name: "-h" or "--help", Value: { IsTrue: true } }))
            {
                throw new DocoptExitException(Usage);
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
                // Required(Either(Required(Optional(Option(-v,,0,False), Option(-q,,0,False), Option(-r,,0,False), Option(-h,--help,0,False)), OneOrMore(Optional(Argument(FILE, [])))), Required(Required(Either(Option(,--left,0,False), Option(,--right,0,False))), Argument(CORRECTION, ), Argument(FILE, []))))
                var b = new RequiredMatcher(1, a.Left, a.Collected);
                while (b.Next())
                {
                    // Either(Required(Optional(Option(-v,,0,False), Option(-q,,0,False), Option(-r,,0,False), Option(-h,--help,0,False)), OneOrMore(Optional(Argument(FILE, [])))), Required(Required(Either(Option(,--left,0,False), Option(,--right,0,False))), Argument(CORRECTION, ), Argument(FILE, [])))
                    var c = new EitherMatcher(2, b.Left, b.Collected);
                    while (c.Next())
                    {
                        switch (c.Index)
                        {
                            case 0:
                            {
                                // Required(Optional(Option(-v,,0,False), Option(-q,,0,False), Option(-r,,0,False), Option(-h,--help,0,False)), OneOrMore(Optional(Argument(FILE, []))))
                                var d = new RequiredMatcher(2, c.Left, c.Collected);
                                while (d.Next())
                                {
                                    switch (d.Index)
                                    {
                                        case 0:
                                        {
                                            // Optional(Option(-v,,0,False), Option(-q,,0,False), Option(-r,,0,False), Option(-h,--help,0,False))
                                            var e = new OptionalMatcher(4, d.Left, d.Collected);
                                            while (e.Next())
                                            {
                                                switch (e.Index)
                                                {
                                                    case 0:
                                                    {
                                                        // Option(-v,,0,False)
                                                        e.Match(PatternMatcher.MatchOption, "-v", value: false, isList: false, isInt: false);
                                                        break;
                                                    }
                                                    case 1:
                                                    {
                                                        // Option(-q,,0,False)
                                                        e.Match(PatternMatcher.MatchOption, "-q", value: false, isList: false, isInt: false);
                                                        break;
                                                    }
                                                    case 2:
                                                    {
                                                        // Option(-r,,0,False)
                                                        e.Match(PatternMatcher.MatchOption, "-r", value: false, isList: false, isInt: false);
                                                        break;
                                                    }
                                                    case 3:
                                                    {
                                                        // Option(-h,--help,0,False)
                                                        e.Match(PatternMatcher.MatchOption, "--help", value: false, isList: false, isInt: false);
                                                        break;
                                                    }
                                                }
                                                if (!e.LastMatched)
                                                    break;
                                            }
                                            d.Fold(e.Result);
                                            break;
                                        }
                                        case 1:
                                        {
                                            // OneOrMore(Optional(Argument(FILE, [])))
                                            var e = new OneOrMoreMatcher(1, d.Left, d.Collected);
                                            while (e.Next())
                                            {
                                                // Optional(Argument(FILE, []))
                                                var f = new OptionalMatcher(1, e.Left, e.Collected);
                                                while (f.Next())
                                                {
                                                    // Argument(FILE, [])
                                                    f.Match(PatternMatcher.MatchArgument, "FILE", value: new ArrayList(), isList: true, isInt: false);
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
                                // Required(Required(Either(Option(,--left,0,False), Option(,--right,0,False))), Argument(CORRECTION, ), Argument(FILE, []))
                                var d = new RequiredMatcher(3, c.Left, c.Collected);
                                while (d.Next())
                                {
                                    switch (d.Index)
                                    {
                                        case 0:
                                        {
                                            // Required(Either(Option(,--left,0,False), Option(,--right,0,False)))
                                            var e = new RequiredMatcher(1, d.Left, d.Collected);
                                            while (e.Next())
                                            {
                                                // Either(Option(,--left,0,False), Option(,--right,0,False))
                                                var f = new EitherMatcher(2, e.Left, e.Collected);
                                                while (f.Next())
                                                {
                                                    switch (f.Index)
                                                    {
                                                        case 0:
                                                        {
                                                            // Option(,--left,0,False)
                                                            f.Match(PatternMatcher.MatchOption, "--left", value: false, isList: false, isInt: false);
                                                            break;
                                                        }
                                                        case 1:
                                                        {
                                                            // Option(,--right,0,False)
                                                            f.Match(PatternMatcher.MatchOption, "--right", value: false, isList: false, isInt: false);
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
                                        case 1:
                                        {
                                            // Argument(CORRECTION, )
                                            d.Match(PatternMatcher.MatchArgument, "CORRECTION", value: null, isList: false, isInt: false);
                                            break;
                                        }
                                        case 2:
                                        {
                                            // Argument(FILE, [])
                                            d.Match(PatternMatcher.MatchArgument, "FILE", value: new ArrayList(), isList: true, isInt: false);
                                            break;
                                        }
                                    }
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
                const string exitUsage = @"Usage: ArgumentsExample [-vqrh] [FILE] ...
       ArgumentsExample (--left | --right) CORRECTION FILE";
                throw new DocoptInputErrorException(exitUsage);
            }

            var dict = new Dictionary<string, Value>
            {
                [@"-v"] = false,
                [@"-q"] = false,
                [@"-r"] = false,
                [@"--help"] = false,
                [@"FILE"] = StringList.Empty,
                [@"--left"] = false,
                [@"--right"] = false,
                [@"CORRECTION"] = Value.None,
                [@"FILE"] = StringList.Empty,
            };

            collected = a.Collected;
            foreach (var p in collected)
            {
                dict[p.Name] = p.Value is { IsStringList: true } ? ((StringList)p.Value).Reverse() : p.Value;
            }

            return dict;
        }

        public bool OptV => _args["-v"].Object is true or (int and > 0);
        public bool OptQ => _args["-q"].Object is true or (int and > 0);
        public bool OptR => _args["-r"].Object is true or (int and > 0);
        public bool OptHelp => _args["--help"].Object is true or (int and > 0);
        public StringList ArgFile => (StringList)_args["FILE"];
        public bool OptLeft => _args["--left"].Object is true or (int and > 0);
        public bool OptRight => _args["--right"].Object is true or (int and > 0);
        public string ArgCorrection => _args["CORRECTION"].Object as string;
    }
}
