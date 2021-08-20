using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DocoptNet.Generated;
using Leaves = DocoptNet.Generated.ReadOnlyList<DocoptNet.Generated.LeafPattern>;

namespace OddEvenExample
{
    partial class Program
    {
        public const string Usage = @"Usage: OddEvenExample [-h | --help] (ODD EVEN)...

Example, try:
  OddEvenExample 1 2 3 4

Options:
  -h, --help
";

        // Required:
        //   Required:
        //     Optional:
        //       Option(-h,--help,0,False) -> OptionNode help Bool
        //     OneOrMore:
        //       Required:
        //         Argument(ODD, []) -> ArgumentNode ODD List
        //         Argument(EVEN, []) -> ArgumentNode EVEN List

        static readonly Pattern Pattern =
            new Required(new Pattern[]
            {
                new Required(new Pattern[]
                {
                    new Optional(new Pattern[]
                    {
                        new Option("-h", "--help", 0, false)
                    }),
                    new OneOrMore(
                    new Required(new Pattern[]
                    {
                        new Argument("ODD"),
                        new Argument("EVEN")
                    }))
                })
            });

        static readonly ICollection<Option> Options = new Option[]
        {
            new Option("-h", "--help", 0, false),
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
                // Required(Required(Optional(Option(-h,--help,0,False)), OneOrMore(Required(Argument(ODD, []), Argument(EVEN, [])))))
                var b = new RequiredMatcher(1, a.Left, a.Collected);
                while (b.Next())
                {
                    // Required(Optional(Option(-h,--help,0,False)), OneOrMore(Required(Argument(ODD, []), Argument(EVEN, []))))
                    var c = new RequiredMatcher(2, b.Left, b.Collected);
                    while (c.Next())
                    {
                        switch (c.Index)
                        {
                            case 0:
                            {
                                // Optional(Option(-h,--help,0,False))
                                var d = new OptionalMatcher(1, c.Left, c.Collected);
                                while (d.Next())
                                {
                                    // Option(-h,--help,0,False)
                                    d.Match(PatternMatcher.MatchOption, "--help", value: false, isList: false, isInt: false);
                                    if (!d.LastMatched)
                                        break;
                                }
                                c.Fold(d.Result);
                                break;
                            }
                            case 1:
                            {
                                // OneOrMore(Required(Argument(ODD, []), Argument(EVEN, [])))
                                var d = new OneOrMoreMatcher(1, c.Left, c.Collected);
                                while (d.Next())
                                {
                                    // Required(Argument(ODD, []), Argument(EVEN, []))
                                    var e = new RequiredMatcher(2, d.Left, d.Collected);
                                    while (e.Next())
                                    {
                                        switch (e.Index)
                                        {
                                            case 0:
                                            {
                                                // Argument(ODD, [])
                                                e.Match(PatternMatcher.MatchArgument, "ODD", value: new ArrayList(), isList: true, isInt: false);
                                                break;
                                            }
                                            case 1:
                                            {
                                                // Argument(EVEN, [])
                                                e.Match(PatternMatcher.MatchArgument, "EVEN", value: new ArrayList(), isList: true, isInt: false);
                                                break;
                                            }
                                        }
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
                const string exitUsage = @"Usage: OddEvenExample [-h | --help] (ODD EVEN)...";
                throw new DocoptInputErrorException(exitUsage);
            }

            var dict = new Dictionary<string, Value>
            {
                [@"--help"] = false,
                [@"ODD"] = StringList.Empty,
                [@"EVEN"] = StringList.Empty,
            };

            collected = a.Collected;
            foreach (var p in collected)
            {
                dict[p.Name] = p.Value is { IsStringList: true } ? ((StringList)p.Value).Reverse() : p.Value;
            }

            return dict;
        }

        public bool OptHelp => _args["--help"].Object is true or (int and > 0);
        public StringList ArgOdd => (StringList)_args["ODD"];
        public StringList ArgEven => (StringList)_args["EVEN"];
    }
}
