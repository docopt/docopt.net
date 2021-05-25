using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DocoptNet.Generated;
using Leaves = DocoptNet.Generated.ReadOnlyList<DocoptNet.Generated.LeafPattern>;

namespace CountedExample
{
    partial class Program
    {
        public const string Usage = @"Usage: CountedExample --help
       CountedExample -v...
       CountedExample go [go]
       CountedExample (--path=<path>)...
       CountedExample <file> <file>

Try: CountedExample -vvvvvvvvvv
     CountedExample go go
     CountedExample --path ./here --path ./there
     CountedExample this.txt that.txt
";

        // Required:
        //   Either:
        //     Required:
        //       Option(,--help,0,False) -> OptionNode help Bool
        //     Required:
        //       OneOrMore:
        //         Option(-v,,0,0) -> OptionNode v Bool
        //     Required:
        //       Command(go, 0) -> CommandNode go Bool
        //       Optional:
        //         Command(go, 0) -> CommandNode go Bool
        //     Required:
        //       OneOrMore:
        //         Required:
        //           Option(,--path,1,[]) -> OptionNode path String
        //     Required:
        //       Argument(<file>, []) -> ArgumentNode <file> List
        //       Argument(<file>, []) -> ArgumentNode <file> List

        static readonly Pattern Pattern =
            new Required(new Pattern[]
            {
                new Either(new Pattern[]
                {
                    new Required(new Pattern[]
                    {
                        new Option(null, "--help", 0, new ValueObject(false))
                    }),
                    new Required(new Pattern[]
                    {
                        new OneOrMore(
                        new Option("-v", null, 0, new ValueObject(0)))
                    }),
                    new Required(new Pattern[]
                    {
                        new Command("go"),
                        new Optional(new Pattern[]
                        {
                            new Command("go")
                        })
                    }),
                    new Required(new Pattern[]
                    {
                        new OneOrMore(
                        new Required(new Pattern[]
                        {
                            new Option(null, "--path", 1, new ValueObject(new ArrayList()))
                        }))
                    }),
                    new Required(new Pattern[]
                    {
                        new Argument("<file>", (ValueObject)null),
                        new Argument("<file>", (ValueObject)null)
                    })
                })
            });

        static readonly ICollection<Option> Options = new Option[]
        {
            new Option(null, "--help", 0, new ValueObject(false)),
            new Option("-v", null, 0, new ValueObject(0)),
            new Option(null, "--path", 1, new ValueObject(new ArrayList())),
        };

        static Dictionary<string, ValueObject> Apply(IEnumerable<string> args, bool help = true, object version = null, bool optionsFirst = false, bool exit = false)
        {
            var tokens = new Tokens(args, typeof(DocoptInputErrorException));
            var options = Options.Select(e => new Option(e.ShortName, e.LongName, e.ArgCount, e.Value)).ToList();
            var arguments = Docopt.ParseArgv(tokens, options, optionsFirst).AsReadOnly();
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
                // Required(Either(Required(Option(,--help,0,False)), Required(OneOrMore(Option(-v,,0,0))), Required(Command(go, 0), Optional(Command(go, 0))), Required(OneOrMore(Required(Option(,--path,1,[])))), Required(Argument(<file>, []), Argument(<file>, []))))
                var b = new RequiredMatcher(1, a.Left, a.Collected);
                while (b.Next())
                {
                    // Either(Required(Option(,--help,0,False)), Required(OneOrMore(Option(-v,,0,0))), Required(Command(go, 0), Optional(Command(go, 0))), Required(OneOrMore(Required(Option(,--path,1,[])))), Required(Argument(<file>, []), Argument(<file>, [])))
                    var c = new EitherMatcher(5, b.Left, b.Collected);
                    while (c.Next())
                    {
                        switch (c.Index)
                        {
                            case 0:
                            {
                                // Required(Option(,--help,0,False))
                                var d = new RequiredMatcher(1, c.Left, c.Collected);
                                while (d.Next())
                                {
                                    // Option(,--help,0,False)
                                    d.Match(PatternMatcher.MatchOption, "--help", value: false, isList: false, isInt: false);
                                    if (!d.LastMatched)
                                        break;
                                }
                                c.Fold(d.Result);
                                break;
                            }
                            case 1:
                            {
                                // Required(OneOrMore(Option(-v,,0,0)))
                                var d = new RequiredMatcher(1, c.Left, c.Collected);
                                while (d.Next())
                                {
                                    // OneOrMore(Option(-v,,0,0))
                                    var e = new OneOrMoreMatcher(1, d.Left, d.Collected);
                                    while (e.Next())
                                    {
                                        // Option(-v,,0,0)
                                        e.Match(PatternMatcher.MatchOption, "-v", value: 0, isList: false, isInt: true);
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
                            case 2:
                            {
                                // Required(Command(go, 0), Optional(Command(go, 0)))
                                var d = new RequiredMatcher(2, c.Left, c.Collected);
                                while (d.Next())
                                {
                                    switch (d.Index)
                                    {
                                        case 0:
                                        {
                                            // Command(go, 0)
                                            d.Match(PatternMatcher.MatchCommand, "go", value: 0, isList: false, isInt: true);
                                            break;
                                        }
                                        case 1:
                                        {
                                            // Optional(Command(go, 0))
                                            var e = new OptionalMatcher(1, d.Left, d.Collected);
                                            while (e.Next())
                                            {
                                                // Command(go, 0)
                                                e.Match(PatternMatcher.MatchCommand, "go", value: 0, isList: false, isInt: true);
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
                            case 3:
                            {
                                // Required(OneOrMore(Required(Option(,--path,1,[]))))
                                var d = new RequiredMatcher(1, c.Left, c.Collected);
                                while (d.Next())
                                {
                                    // OneOrMore(Required(Option(,--path,1,[])))
                                    var e = new OneOrMoreMatcher(1, d.Left, d.Collected);
                                    while (e.Next())
                                    {
                                        // Required(Option(,--path,1,[]))
                                        var f = new RequiredMatcher(1, e.Left, e.Collected);
                                        while (f.Next())
                                        {
                                            // Option(,--path,1,[])
                                            f.Match(PatternMatcher.MatchOption, "--path", value: new ArrayList(), isList: true, isInt: false);
                                            if (!f.LastMatched)
                                                break;
                                        }
                                        e.Fold(f.Result);
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
                            case 4:
                            {
                                // Required(Argument(<file>, []), Argument(<file>, []))
                                var d = new RequiredMatcher(2, c.Left, c.Collected);
                                while (d.Next())
                                {
                                    switch (d.Index)
                                    {
                                        case 0:
                                        {
                                            // Argument(<file>, [])
                                            d.Match(PatternMatcher.MatchArgument, "<file>", value: new ArrayList(), isList: true, isInt: false);
                                            break;
                                        }
                                        case 1:
                                        {
                                            // Argument(<file>, [])
                                            d.Match(PatternMatcher.MatchArgument, "<file>", value: new ArrayList(), isList: true, isInt: false);
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
                const string exitUsage = @"Usage: CountedExample --help
       CountedExample -v...
       CountedExample go [go]
       CountedExample (--path=<path>)...
       CountedExample <file> <file>";
                throw new DocoptInputErrorException(exitUsage);
            }

            var dict = new Dictionary<string, ValueObject>
            {
                [@"--help"] = new ValueObject(false),
                [@"-v"] = new ValueObject(0),
                [@"go"] = new ValueObject(0),
                [@"go"] = new ValueObject(0),
                [@"--path"] = new ValueObject(new ArrayList()),
                [@"<file>"] = new ValueObject(new ArrayList()),
                [@"<file>"] = new ValueObject(new ArrayList()),
            };

            collected = a.Collected;
            foreach (var p in collected)
            {
                dict[p.Name] = p.Value;
            }

            return dict;
        }

        public bool OptHelp { get { ValueObject v = _args["--help"]; return v.IsTrue || v.IsOfTypeInt && v.AsInt > 0; } }
        public bool OptV { get { ValueObject v = _args["-v"]; return v.IsTrue || v.IsOfTypeInt && v.AsInt > 0; } }
        public bool CmdGo { get { ValueObject v = _args["go"]; return v.IsTrue || v.IsOfTypeInt && v.AsInt > 0; } }
        public string OptPath { get { return null == _args["--path"] ? "[]" : _args["--path"].ToString(); } }
        public ArrayList ArgFile { get { return _args["<file>"].AsList; } }
    }
}
