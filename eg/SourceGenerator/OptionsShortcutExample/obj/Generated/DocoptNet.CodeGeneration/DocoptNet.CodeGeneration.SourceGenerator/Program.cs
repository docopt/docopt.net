using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DocoptNet.Generated;
using Leaves = DocoptNet.Generated.ReadOnlyList<DocoptNet.Generated.LeafPattern>;

namespace OptionsShortcutExample
{
    partial class Program
    {
        public const string Usage = @"Example of program which uses [options] shortcut in pattern.

Usage:
  OptionsShortcutExample [options] <port>

Options:
  -h --help                show this help message and exit
  --version                show version and exit
  -n, --number N           use N as a number
  -t, --timeout TIMEOUT    set timeout TIMEOUT seconds
  --apply                  apply changes to database
  -q                       operate in quiet mode
";

        // Required:
        //   Required:
        //     Optional:
        //       OptionsShortcut:
        //         Option(-h,--help,0,False) -> OptionNode help Bool
        //         Option(,--version,0,False) -> OptionNode version Bool
        //         Option(-n,--number,1,) -> OptionNode number String
        //         Option(-t,--timeout,1,) -> OptionNode timeout String
        //         Option(,--apply,0,False) -> OptionNode apply Bool
        //         Option(-q,,0,False) -> OptionNode q Bool
        //     Argument(<port>, ) -> ArgumentNode <port> String

        static readonly Pattern Pattern =
            new Required(new Pattern[]
            {
                new Required(new Pattern[]
                {
                    new Optional(new Pattern[]
                    {
                        new OptionsShortcut(new Pattern[]
                        {
                            new Option("-h", "--help", 0, false),
                            new Option(null, "--version", 0, false),
                            new Option("-n", "--number", 1, null),
                            new Option("-t", "--timeout", 1, null),
                            new Option(null, "--apply", 0, false),
                            new Option("-q", null, 0, false)
                        })
                    }),
                    new Argument("<port>")
                })
            });

        static readonly ICollection<Option> Options = new Option[]
        {
            new Option("-h", "--help", 0, false),
            new Option(null, "--version", 0, false),
            new Option("-n", "--number", 1, null),
            new Option("-t", "--timeout", 1, null),
            new Option(null, "--apply", 0, false),
            new Option("-q", null, 0, false),
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
                // Required(Required(Optional(OptionsShortcut(Option(-h,--help,0,False), Option(,--version,0,False), Option(-n,--number,1,), Option(-t,--timeout,1,), Option(,--apply,0,False), Option(-q,,0,False))), Argument(<port>, )))
                var b = new RequiredMatcher(1, a.Left, a.Collected);
                while (b.Next())
                {
                    // Required(Optional(OptionsShortcut(Option(-h,--help,0,False), Option(,--version,0,False), Option(-n,--number,1,), Option(-t,--timeout,1,), Option(,--apply,0,False), Option(-q,,0,False))), Argument(<port>, ))
                    var c = new RequiredMatcher(2, b.Left, b.Collected);
                    while (c.Next())
                    {
                        switch (c.Index)
                        {
                            case 0:
                            {
                                // Optional(OptionsShortcut(Option(-h,--help,0,False), Option(,--version,0,False), Option(-n,--number,1,), Option(-t,--timeout,1,), Option(,--apply,0,False), Option(-q,,0,False)))
                                var d = new OptionalMatcher(1, c.Left, c.Collected);
                                while (d.Next())
                                {
                                    // OptionsShortcut(Option(-h,--help,0,False), Option(,--version,0,False), Option(-n,--number,1,), Option(-t,--timeout,1,), Option(,--apply,0,False), Option(-q,,0,False))
                                    var e = new OptionalMatcher(6, d.Left, d.Collected);
                                    while (e.Next())
                                    {
                                        switch (e.Index)
                                        {
                                            case 0:
                                            {
                                                // Option(-h,--help,0,False)
                                                e.Match(PatternMatcher.MatchOption, "--help", value: false, isList: false, isInt: false);
                                                break;
                                            }
                                            case 1:
                                            {
                                                // Option(,--version,0,False)
                                                e.Match(PatternMatcher.MatchOption, "--version", value: false, isList: false, isInt: false);
                                                break;
                                            }
                                            case 2:
                                            {
                                                // Option(-n,--number,1,)
                                                e.Match(PatternMatcher.MatchOption, "--number", value: null, isList: false, isInt: false);
                                                break;
                                            }
                                            case 3:
                                            {
                                                // Option(-t,--timeout,1,)
                                                e.Match(PatternMatcher.MatchOption, "--timeout", value: null, isList: false, isInt: false);
                                                break;
                                            }
                                            case 4:
                                            {
                                                // Option(,--apply,0,False)
                                                e.Match(PatternMatcher.MatchOption, "--apply", value: false, isList: false, isInt: false);
                                                break;
                                            }
                                            case 5:
                                            {
                                                // Option(-q,,0,False)
                                                e.Match(PatternMatcher.MatchOption, "-q", value: false, isList: false, isInt: false);
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
                            case 1:
                            {
                                // Argument(<port>, )
                                c.Match(PatternMatcher.MatchArgument, "<port>", value: null, isList: false, isInt: false);
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
  OptionsShortcutExample [options] <port>";
                throw new DocoptInputErrorException(exitUsage);
            }

            var dict = new Dictionary<string, Value>
            {
                [@"--help"] = false,
                [@"--version"] = false,
                [@"--number"] = Value.None,
                [@"--timeout"] = Value.None,
                [@"--apply"] = false,
                [@"-q"] = false,
                [@"<port>"] = Value.None,
            };

            collected = a.Collected;
            foreach (var p in collected)
            {
                dict[p.Name] = p.Value is { IsStringList: true } ? ((StringList)p.Value).Reverse() : p.Value;
            }

            return dict;
        }

        public bool OptHelp => _args["--help"].Object is true or (int and > 0);
        public bool OptVersion => _args["--version"].Object is true or (int and > 0);
        public string OptNumber => (string)_args["--number"].Object;
        public string OptTimeout => (string)_args["--timeout"].Object;
        public bool OptApply => _args["--apply"].Object is true or (int and > 0);
        public bool OptQ => _args["-q"].Object is true or (int and > 0);
        public string ArgPort => _args["<port>"].Object as string;
    }
}
