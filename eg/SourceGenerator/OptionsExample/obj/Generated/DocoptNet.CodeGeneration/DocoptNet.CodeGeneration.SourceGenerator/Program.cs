using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DocoptNet.Generated;
using Leaves = DocoptNet.Generated.ReadOnlyList<DocoptNet.Generated.LeafPattern>;

namespace OptionsExample
{
    partial class Program
    {
        public const string Usage = @"Example of program with many options using docopt.

Usage:
  OptionsExample [-hvqrf NAME] [--exclude=PATTERNS]
                 [--select=ERRORS | --ignore=ERRORS] [--show-source]
                 [--statistics] [--count] [--benchmark] PATH...
  OptionsExample (--doctest | --testsuite=DIR)
  OptionsExample --version

Arguments:
  PATH  destination path

Options:
  -h --help            show this help message and exit
  --version            show version and exit
  -v --verbose         print status messages
  -q --quiet           report only file names
  -r --repeat          show all occurrences of the same error
  --exclude=PATTERNS   exclude files or directories which match these comma
                       separated patterns [default: .svn,CVS,.bzr,.hg,.git]
  -f NAME --file=NAME  when parsing directories, only check filenames matching
                       these comma separated patterns [default: *.py]
  --select=ERRORS      select errors and warnings (e.g. E,W6)
  --ignore=ERRORS      skip errors and warnings (e.g. E4,W)
  --show-source        show source code for each error
  --statistics         count errors and warnings
  --count              print total number of errors and warnings to standard
                       error and set exit code to 1 if total is not null
  --benchmark          measure processing speed
  --testsuite=DIR      run regression tests from dir
  --doctest            run doctest on myself
";

        // Required:
        //   Either:
        //     Required:
        //       Optional:
        //         Option(-h,--help,0,False) -> OptionNode help Bool
        //         Option(-v,--verbose,0,False) -> OptionNode verbose Bool
        //         Option(-q,--quiet,0,False) -> OptionNode quiet Bool
        //         Option(-r,--repeat,0,False) -> OptionNode repeat Bool
        //         Option(-f,--file,1,*.py) -> OptionNode file String
        //       Optional:
        //         Option(,--exclude,1,.svn,CVS,.bzr,.hg,.git) -> OptionNode exclude String
        //       Optional:
        //         Either:
        //           Option(,--select,1,) -> OptionNode select String
        //           Option(,--ignore,1,) -> OptionNode ignore String
        //       Optional:
        //         Option(,--show-source,0,False) -> OptionNode show-source Bool
        //       Optional:
        //         Option(,--statistics,0,False) -> OptionNode statistics Bool
        //       Optional:
        //         Option(,--count,0,False) -> OptionNode count Bool
        //       Optional:
        //         Option(,--benchmark,0,False) -> OptionNode benchmark Bool
        //       OneOrMore:
        //         Argument(PATH, []) -> ArgumentNode PATH List
        //     Required:
        //       Required:
        //         Either:
        //           Option(,--doctest,0,False) -> OptionNode doctest Bool
        //           Option(,--testsuite,1,) -> OptionNode testsuite String
        //     Required:
        //       Option(,--version,0,False) -> OptionNode version Bool

        static readonly Pattern Pattern =
            new Required(new Pattern[]
            {
                new Either(new Pattern[]
                {
                    new Required(new Pattern[]
                    {
                        new Optional(new Pattern[]
                        {
                            new Option("-h", "--help", 0, false),
                            new Option("-v", "--verbose", 0, false),
                            new Option("-q", "--quiet", 0, false),
                            new Option("-r", "--repeat", 0, false),
                            new Option("-f", "--file", 1, "*.py")
                        }),
                        new Optional(new Pattern[]
                        {
                            new Option(null, "--exclude", 1, ".svn,CVS,.bzr,.hg,.git")
                        }),
                        new Optional(new Pattern[]
                        {
                            new Either(new Pattern[]
                            {
                                new Option(null, "--select", 1, null),
                                new Option(null, "--ignore", 1, null)
                            })
                        }),
                        new Optional(new Pattern[]
                        {
                            new Option(null, "--show-source", 0, false)
                        }),
                        new Optional(new Pattern[]
                        {
                            new Option(null, "--statistics", 0, false)
                        }),
                        new Optional(new Pattern[]
                        {
                            new Option(null, "--count", 0, false)
                        }),
                        new Optional(new Pattern[]
                        {
                            new Option(null, "--benchmark", 0, false)
                        }),
                        new OneOrMore(
                        new Argument("PATH"))
                    }),
                    new Required(new Pattern[]
                    {
                        new Required(new Pattern[]
                        {
                            new Either(new Pattern[]
                            {
                                new Option(null, "--doctest", 0, false),
                                new Option(null, "--testsuite", 1, null)
                            })
                        })
                    }),
                    new Required(new Pattern[]
                    {
                        new Option(null, "--version", 0, false)
                    })
                })
            });

        static readonly ICollection<Option> Options = new Option[]
        {
            new Option("-h", "--help", 0, false),
            new Option(null, "--version", 0, false),
            new Option("-v", "--verbose", 0, false),
            new Option("-q", "--quiet", 0, false),
            new Option("-r", "--repeat", 0, false),
            new Option(null, "--exclude", 1, ".svn,CVS,.bzr,.hg,.git"),
            new Option("-f", "--file", 1, "*.py"),
            new Option(null, "--select", 1, null),
            new Option(null, "--ignore", 1, null),
            new Option(null, "--show-source", 0, false),
            new Option(null, "--statistics", 0, false),
            new Option(null, "--count", 0, false),
            new Option(null, "--benchmark", 0, false),
            new Option(null, "--testsuite", 1, null),
            new Option(null, "--doctest", 0, false),
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
                // Required(Either(Required(Optional(Option(-h,--help,0,False), Option(-v,--verbose,0,False), Option(-q,--quiet,0,False), Option(-r,--repeat,0,False), Option(-f,--file,1,*.py)), Optional(Option(,--exclude,1,.svn,CVS,.bzr,.hg,.git)), Optional(Either(Option(,--select,1,), Option(,--ignore,1,))), Optional(Option(,--show-source,0,False)), Optional(Option(,--statistics,0,False)), Optional(Option(,--count,0,False)), Optional(Option(,--benchmark,0,False)), OneOrMore(Argument(PATH, []))), Required(Required(Either(Option(,--doctest,0,False), Option(,--testsuite,1,)))), Required(Option(,--version,0,False))))
                var b = new RequiredMatcher(1, a.Left, a.Collected);
                while (b.Next())
                {
                    // Either(Required(Optional(Option(-h,--help,0,False), Option(-v,--verbose,0,False), Option(-q,--quiet,0,False), Option(-r,--repeat,0,False), Option(-f,--file,1,*.py)), Optional(Option(,--exclude,1,.svn,CVS,.bzr,.hg,.git)), Optional(Either(Option(,--select,1,), Option(,--ignore,1,))), Optional(Option(,--show-source,0,False)), Optional(Option(,--statistics,0,False)), Optional(Option(,--count,0,False)), Optional(Option(,--benchmark,0,False)), OneOrMore(Argument(PATH, []))), Required(Required(Either(Option(,--doctest,0,False), Option(,--testsuite,1,)))), Required(Option(,--version,0,False)))
                    var c = new EitherMatcher(3, b.Left, b.Collected);
                    while (c.Next())
                    {
                        switch (c.Index)
                        {
                            case 0:
                            {
                                // Required(Optional(Option(-h,--help,0,False), Option(-v,--verbose,0,False), Option(-q,--quiet,0,False), Option(-r,--repeat,0,False), Option(-f,--file,1,*.py)), Optional(Option(,--exclude,1,.svn,CVS,.bzr,.hg,.git)), Optional(Either(Option(,--select,1,), Option(,--ignore,1,))), Optional(Option(,--show-source,0,False)), Optional(Option(,--statistics,0,False)), Optional(Option(,--count,0,False)), Optional(Option(,--benchmark,0,False)), OneOrMore(Argument(PATH, [])))
                                var d = new RequiredMatcher(8, c.Left, c.Collected);
                                while (d.Next())
                                {
                                    switch (d.Index)
                                    {
                                        case 0:
                                        {
                                            // Optional(Option(-h,--help,0,False), Option(-v,--verbose,0,False), Option(-q,--quiet,0,False), Option(-r,--repeat,0,False), Option(-f,--file,1,*.py))
                                            var e = new OptionalMatcher(5, d.Left, d.Collected);
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
                                                        // Option(-v,--verbose,0,False)
                                                        e.Match(PatternMatcher.MatchOption, "--verbose", value: false, isList: false, isInt: false);
                                                        break;
                                                    }
                                                    case 2:
                                                    {
                                                        // Option(-q,--quiet,0,False)
                                                        e.Match(PatternMatcher.MatchOption, "--quiet", value: false, isList: false, isInt: false);
                                                        break;
                                                    }
                                                    case 3:
                                                    {
                                                        // Option(-r,--repeat,0,False)
                                                        e.Match(PatternMatcher.MatchOption, "--repeat", value: false, isList: false, isInt: false);
                                                        break;
                                                    }
                                                    case 4:
                                                    {
                                                        // Option(-f,--file,1,*.py)
                                                        e.Match(PatternMatcher.MatchOption, "--file", value: "*.py", isList: false, isInt: false);
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
                                            // Optional(Option(,--exclude,1,.svn,CVS,.bzr,.hg,.git))
                                            var e = new OptionalMatcher(1, d.Left, d.Collected);
                                            while (e.Next())
                                            {
                                                // Option(,--exclude,1,.svn,CVS,.bzr,.hg,.git)
                                                e.Match(PatternMatcher.MatchOption, "--exclude", value: ".svn,CVS,.bzr,.hg,.git", isList: false, isInt: false);
                                                if (!e.LastMatched)
                                                    break;
                                            }
                                            d.Fold(e.Result);
                                            break;
                                        }
                                        case 2:
                                        {
                                            // Optional(Either(Option(,--select,1,), Option(,--ignore,1,)))
                                            var e = new OptionalMatcher(1, d.Left, d.Collected);
                                            while (e.Next())
                                            {
                                                // Either(Option(,--select,1,), Option(,--ignore,1,))
                                                var f = new EitherMatcher(2, e.Left, e.Collected);
                                                while (f.Next())
                                                {
                                                    switch (f.Index)
                                                    {
                                                        case 0:
                                                        {
                                                            // Option(,--select,1,)
                                                            f.Match(PatternMatcher.MatchOption, "--select", value: null, isList: false, isInt: false);
                                                            break;
                                                        }
                                                        case 1:
                                                        {
                                                            // Option(,--ignore,1,)
                                                            f.Match(PatternMatcher.MatchOption, "--ignore", value: null, isList: false, isInt: false);
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
                                        case 3:
                                        {
                                            // Optional(Option(,--show-source,0,False))
                                            var e = new OptionalMatcher(1, d.Left, d.Collected);
                                            while (e.Next())
                                            {
                                                // Option(,--show-source,0,False)
                                                e.Match(PatternMatcher.MatchOption, "--show-source", value: false, isList: false, isInt: false);
                                                if (!e.LastMatched)
                                                    break;
                                            }
                                            d.Fold(e.Result);
                                            break;
                                        }
                                        case 4:
                                        {
                                            // Optional(Option(,--statistics,0,False))
                                            var e = new OptionalMatcher(1, d.Left, d.Collected);
                                            while (e.Next())
                                            {
                                                // Option(,--statistics,0,False)
                                                e.Match(PatternMatcher.MatchOption, "--statistics", value: false, isList: false, isInt: false);
                                                if (!e.LastMatched)
                                                    break;
                                            }
                                            d.Fold(e.Result);
                                            break;
                                        }
                                        case 5:
                                        {
                                            // Optional(Option(,--count,0,False))
                                            var e = new OptionalMatcher(1, d.Left, d.Collected);
                                            while (e.Next())
                                            {
                                                // Option(,--count,0,False)
                                                e.Match(PatternMatcher.MatchOption, "--count", value: false, isList: false, isInt: false);
                                                if (!e.LastMatched)
                                                    break;
                                            }
                                            d.Fold(e.Result);
                                            break;
                                        }
                                        case 6:
                                        {
                                            // Optional(Option(,--benchmark,0,False))
                                            var e = new OptionalMatcher(1, d.Left, d.Collected);
                                            while (e.Next())
                                            {
                                                // Option(,--benchmark,0,False)
                                                e.Match(PatternMatcher.MatchOption, "--benchmark", value: false, isList: false, isInt: false);
                                                if (!e.LastMatched)
                                                    break;
                                            }
                                            d.Fold(e.Result);
                                            break;
                                        }
                                        case 7:
                                        {
                                            // OneOrMore(Argument(PATH, []))
                                            var e = new OneOrMoreMatcher(1, d.Left, d.Collected);
                                            while (e.Next())
                                            {
                                                // Argument(PATH, [])
                                                e.Match(PatternMatcher.MatchArgument, "PATH", value: new ArrayList(), isList: true, isInt: false);
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
                                // Required(Required(Either(Option(,--doctest,0,False), Option(,--testsuite,1,))))
                                var d = new RequiredMatcher(1, c.Left, c.Collected);
                                while (d.Next())
                                {
                                    // Required(Either(Option(,--doctest,0,False), Option(,--testsuite,1,)))
                                    var e = new RequiredMatcher(1, d.Left, d.Collected);
                                    while (e.Next())
                                    {
                                        // Either(Option(,--doctest,0,False), Option(,--testsuite,1,))
                                        var f = new EitherMatcher(2, e.Left, e.Collected);
                                        while (f.Next())
                                        {
                                            switch (f.Index)
                                            {
                                                case 0:
                                                {
                                                    // Option(,--doctest,0,False)
                                                    f.Match(PatternMatcher.MatchOption, "--doctest", value: false, isList: false, isInt: false);
                                                    break;
                                                }
                                                case 1:
                                                {
                                                    // Option(,--testsuite,1,)
                                                    f.Match(PatternMatcher.MatchOption, "--testsuite", value: null, isList: false, isInt: false);
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
                                    if (!d.LastMatched)
                                        break;
                                }
                                c.Fold(d.Result);
                                break;
                            }
                            case 2:
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

            if (!a.Result || a.Left.Count > 0)
            {
                const string exitUsage = @"Usage:
  OptionsExample [-hvqrf NAME] [--exclude=PATTERNS]
                 [--select=ERRORS | --ignore=ERRORS] [--show-source]
                 [--statistics] [--count] [--benchmark] PATH...
  OptionsExample (--doctest | --testsuite=DIR)
  OptionsExample --version";
                throw new DocoptInputErrorException(exitUsage);
            }

            var dict = new Dictionary<string, Value>
            {
                [@"--help"] = false,
                [@"--verbose"] = false,
                [@"--quiet"] = false,
                [@"--repeat"] = false,
                [@"--file"] = "*.py",
                [@"--exclude"] = ".svn,CVS,.bzr,.hg,.git",
                [@"--select"] = Value.None,
                [@"--ignore"] = Value.None,
                [@"--show-source"] = false,
                [@"--statistics"] = false,
                [@"--count"] = false,
                [@"--benchmark"] = false,
                [@"PATH"] = StringList.Empty,
                [@"--doctest"] = false,
                [@"--testsuite"] = Value.None,
                [@"--version"] = false,
            };

            collected = a.Collected;
            foreach (var p in collected)
            {
                dict[p.Name] = p.Value is { IsStringList: true } ? ((StringList)p.Value).Reverse() : p.Value;
            }

            return dict;
        }

        public bool OptHelp => _args["--help"].Object is true or (int and > 0);
        public bool OptVerbose => _args["--verbose"].Object is true or (int and > 0);
        public bool OptQuiet => _args["--quiet"].Object is true or (int and > 0);
        public bool OptRepeat => _args["--repeat"].Object is true or (int and > 0);
        public string OptFile => (string)_args["--file"].Object ?? "*.py";
        public string OptExclude => (string)_args["--exclude"].Object ?? ".svn,CVS,.bzr,.hg,.git";
        public string OptSelect => (string)_args["--select"].Object;
        public string OptIgnore => (string)_args["--ignore"].Object;
        public bool OptShowSource => _args["--show-source"].Object is true or (int and > 0);
        public bool OptStatistics => _args["--statistics"].Object is true or (int and > 0);
        public bool OptCount => _args["--count"].Object is true or (int and > 0);
        public bool OptBenchmark => _args["--benchmark"].Object is true or (int and > 0);
        public StringList ArgPath => (StringList)_args["PATH"];
        public bool OptDoctest => _args["--doctest"].Object is true or (int and > 0);
        public string OptTestsuite => (string)_args["--testsuite"].Object;
        public bool OptVersion => _args["--version"].Object is true or (int and > 0);
    }
}
