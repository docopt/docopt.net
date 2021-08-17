using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DocoptNet.Generated;
using Leaves = DocoptNet.Generated.ReadOnlyList<DocoptNet.Generated.LeafPattern>;

namespace QuickExample
{
    partial class Program
    {
        public const string Usage = @"Usage:
  QuickExample tcp <host> <port> [--timeout=<seconds>]
  QuickExample serial <port> [--baud=9600] [--timeout=<seconds>]
  QuickExample -h | --help | --version
";

        // Required:
        //   Either:
        //     Required:
        //       Command(tcp, False) -> CommandNode tcp Bool
        //       Argument(<host>, ) -> ArgumentNode <host> String
        //       Argument(<port>, ) -> ArgumentNode <port> String
        //       Optional:
        //         Option(,--timeout,1,) -> OptionNode timeout String
        //     Required:
        //       Command(serial, False) -> CommandNode serial Bool
        //       Argument(<port>, ) -> ArgumentNode <port> String
        //       Optional:
        //         Option(,--baud,1,) -> OptionNode baud String
        //       Optional:
        //         Option(,--timeout,1,) -> OptionNode timeout String
        //     Required:
        //       Either:
        //         Option(-h,,0,False) -> OptionNode h Bool
        //         Option(,--help,0,False) -> OptionNode help Bool
        //         Option(,--version,0,False) -> OptionNode version Bool

        static readonly Pattern Pattern =
            new Required(new Pattern[]
            {
                new Either(new Pattern[]
                {
                    new Required(new Pattern[]
                    {
                        new Command("tcp"),
                        new Argument("<host>"),
                        new Argument("<port>"),
                        new Optional(new Pattern[]
                        {
                            new Option(null, "--timeout", 1, null)
                        })
                    }),
                    new Required(new Pattern[]
                    {
                        new Command("serial"),
                        new Argument("<port>"),
                        new Optional(new Pattern[]
                        {
                            new Option(null, "--baud", 1, null)
                        }),
                        new Optional(new Pattern[]
                        {
                            new Option(null, "--timeout", 1, null)
                        })
                    }),
                    new Required(new Pattern[]
                    {
                        new Either(new Pattern[]
                        {
                            new Option("-h", null, 0, false),
                            new Option(null, "--help", 0, false),
                            new Option(null, "--version", 0, false)
                        })
                    })
                })
            });

        static readonly ICollection<Option> Options = new Option[]
        {
            new Option(null, "--timeout", 1, null),
            new Option(null, "--baud", 1, null),
            new Option("-h", null, 0, false),
            new Option(null, "--help", 0, false),
            new Option(null, "--version", 0, false),
        };

        static Dictionary<string, ValueObject> Apply(IEnumerable<string> args, bool help = true, object version = null, bool optionsFirst = false, bool exit = false)
        {
            var tokens = new Tokens(args, typeof(DocoptInputErrorException));
            var options = Options.Select(e => new Option(e.ShortName, e.LongName, e.ArgCount, e.Value)).ToList();
            var arguments = Docopt.ParseArgv(tokens, options, optionsFirst).AsReadOnly();
            if (help && arguments.Any(o => o is { Name: "-h" or "--help", Value: { Object: null or string { Length: 0 } } }))
            {
                throw new DocoptExitException(Usage);
            }
            if (version is not null && arguments.Any(o => o is { Name: "--version", Value: { Object: null or string { Length: 0 } } }))
            {
                throw new DocoptExitException(version.ToString());
            }
            var left = arguments;
            var collected = new Leaves();
            var a = new RequiredMatcher(1, left, collected);
            do
            {
                // Required(Either(Required(Command(tcp, False), Argument(<host>, ), Argument(<port>, ), Optional(Option(,--timeout,1,))), Required(Command(serial, False), Argument(<port>, ), Optional(Option(,--baud,1,)), Optional(Option(,--timeout,1,))), Required(Either(Option(-h,,0,False), Option(,--help,0,False), Option(,--version,0,False)))))
                var b = new RequiredMatcher(1, a.Left, a.Collected);
                while (b.Next())
                {
                    // Either(Required(Command(tcp, False), Argument(<host>, ), Argument(<port>, ), Optional(Option(,--timeout,1,))), Required(Command(serial, False), Argument(<port>, ), Optional(Option(,--baud,1,)), Optional(Option(,--timeout,1,))), Required(Either(Option(-h,,0,False), Option(,--help,0,False), Option(,--version,0,False))))
                    var c = new EitherMatcher(3, b.Left, b.Collected);
                    while (c.Next())
                    {
                        switch (c.Index)
                        {
                            case 0:
                            {
                                // Required(Command(tcp, False), Argument(<host>, ), Argument(<port>, ), Optional(Option(,--timeout,1,)))
                                var d = new RequiredMatcher(4, c.Left, c.Collected);
                                while (d.Next())
                                {
                                    switch (d.Index)
                                    {
                                        case 0:
                                        {
                                            // Command(tcp, False)
                                            d.Match(PatternMatcher.MatchCommand, "tcp", value: false, isList: false, isInt: false);
                                            break;
                                        }
                                        case 1:
                                        {
                                            // Argument(<host>, )
                                            d.Match(PatternMatcher.MatchArgument, "<host>", value: null, isList: false, isInt: false);
                                            break;
                                        }
                                        case 2:
                                        {
                                            // Argument(<port>, )
                                            d.Match(PatternMatcher.MatchArgument, "<port>", value: null, isList: false, isInt: false);
                                            break;
                                        }
                                        case 3:
                                        {
                                            // Optional(Option(,--timeout,1,))
                                            var e = new OptionalMatcher(1, d.Left, d.Collected);
                                            while (e.Next())
                                            {
                                                // Option(,--timeout,1,)
                                                e.Match(PatternMatcher.MatchOption, "--timeout", value: null, isList: false, isInt: false);
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
                                // Required(Command(serial, False), Argument(<port>, ), Optional(Option(,--baud,1,)), Optional(Option(,--timeout,1,)))
                                var d = new RequiredMatcher(4, c.Left, c.Collected);
                                while (d.Next())
                                {
                                    switch (d.Index)
                                    {
                                        case 0:
                                        {
                                            // Command(serial, False)
                                            d.Match(PatternMatcher.MatchCommand, "serial", value: false, isList: false, isInt: false);
                                            break;
                                        }
                                        case 1:
                                        {
                                            // Argument(<port>, )
                                            d.Match(PatternMatcher.MatchArgument, "<port>", value: null, isList: false, isInt: false);
                                            break;
                                        }
                                        case 2:
                                        {
                                            // Optional(Option(,--baud,1,))
                                            var e = new OptionalMatcher(1, d.Left, d.Collected);
                                            while (e.Next())
                                            {
                                                // Option(,--baud,1,)
                                                e.Match(PatternMatcher.MatchOption, "--baud", value: null, isList: false, isInt: false);
                                                if (!e.LastMatched)
                                                    break;
                                            }
                                            d.Fold(e.Result);
                                            break;
                                        }
                                        case 3:
                                        {
                                            // Optional(Option(,--timeout,1,))
                                            var e = new OptionalMatcher(1, d.Left, d.Collected);
                                            while (e.Next())
                                            {
                                                // Option(,--timeout,1,)
                                                e.Match(PatternMatcher.MatchOption, "--timeout", value: null, isList: false, isInt: false);
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
                                // Required(Either(Option(-h,,0,False), Option(,--help,0,False), Option(,--version,0,False)))
                                var d = new RequiredMatcher(1, c.Left, c.Collected);
                                while (d.Next())
                                {
                                    // Either(Option(-h,,0,False), Option(,--help,0,False), Option(,--version,0,False))
                                    var e = new EitherMatcher(3, d.Left, d.Collected);
                                    while (e.Next())
                                    {
                                        switch (e.Index)
                                        {
                                            case 0:
                                            {
                                                // Option(-h,,0,False)
                                                e.Match(PatternMatcher.MatchOption, "-h", value: false, isList: false, isInt: false);
                                                break;
                                            }
                                            case 1:
                                            {
                                                // Option(,--help,0,False)
                                                e.Match(PatternMatcher.MatchOption, "--help", value: false, isList: false, isInt: false);
                                                break;
                                            }
                                            case 2:
                                            {
                                                // Option(,--version,0,False)
                                                e.Match(PatternMatcher.MatchOption, "--version", value: false, isList: false, isInt: false);
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
                const string exitUsage = @"Usage:
  QuickExample tcp <host> <port> [--timeout=<seconds>]
  QuickExample serial <port> [--baud=9600] [--timeout=<seconds>]
  QuickExample -h | --help | --version";
                throw new DocoptInputErrorException(exitUsage);
            }

            var dict = new Dictionary<string, ValueObject>
            {
                [@"tcp"] = new ValueObject(false),
                [@"<host>"] = new ValueObject(null),
                [@"<port>"] = new ValueObject(null),
                [@"--timeout"] = new ValueObject(null),
                [@"serial"] = new ValueObject(false),
                [@"<port>"] = new ValueObject(null),
                [@"--baud"] = new ValueObject(null),
                [@"--timeout"] = new ValueObject(null),
                [@"-h"] = new ValueObject(false),
                [@"--help"] = new ValueObject(false),
                [@"--version"] = new ValueObject(false),
            };

            collected = a.Collected;
            foreach (var p in collected)
            {
                dict[p.Name] = (p.Value.Object is StringList list ? list.Reverse() : p.Value).ToValueObject();
            }

            return dict;
        }

        public bool CmdTcp { get { ValueObject v = _args["tcp"]; return v.IsTrue || v.IsOfTypeInt && v.AsInt > 0; } }
        public string ArgHost { get { return null == _args["<host>"] ? null : _args["<host>"].ToString(); } }
        public string ArgPort { get { return null == _args["<port>"] ? null : _args["<port>"].ToString(); } }
        public string OptTimeout { get { return null == _args["--timeout"] ? null : _args["--timeout"].ToString(); } }
        public bool CmdSerial { get { ValueObject v = _args["serial"]; return v.IsTrue || v.IsOfTypeInt && v.AsInt > 0; } }
        public string OptBaud { get { return null == _args["--baud"] ? null : _args["--baud"].ToString(); } }
        public bool OptH { get { ValueObject v = _args["-h"]; return v.IsTrue || v.IsOfTypeInt && v.AsInt > 0; } }
        public bool OptHelp { get { ValueObject v = _args["--help"]; return v.IsTrue || v.IsOfTypeInt && v.AsInt > 0; } }
        public bool OptVersion { get { ValueObject v = _args["--version"]; return v.IsTrue || v.IsOfTypeInt && v.AsInt > 0; } }
    }
}
