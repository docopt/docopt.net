#nullable enable annotations

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DocoptNet.Generated;
using Leaves = DocoptNet.Generated.ReadOnlyList<DocoptNet.Generated.LeafPattern>;
using static DocoptNet.Generated.GeneratedSourceModule;

namespace OptionsExample
{
    partial class ProgramArguments : IEnumerable<KeyValuePair<string, object?>>
    {
        public const string HelpText = @"Example of program with many options using docopt.

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

        public const string Usage = @"Usage:
  OptionsExample [-hvqrf NAME] [--exclude=PATTERNS]
                 [--select=ERRORS | --ignore=ERRORS] [--show-source]
                 [--statistics] [--count] [--benchmark] PATH...
  OptionsExample (--doctest | --testsuite=DIR)
  OptionsExample --version";

        public static ProgramArguments Apply(IEnumerable<string> args, bool help = true, object? version = null, bool optionsFirst = false, bool exit = false)
        {
            var options = new List<Option>
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
            var left = ParseArgv(HelpText, args, options, optionsFirst, help, version);
            var collected = new Leaves();
            var required = new RequiredMatcher(1, left, collected);
            {
                // Required(Either(Required(Optional(Option(-h,--help,0,False), Option(-v,--verbose,0,False), Option(-q,--quiet,0,False), Option(-r,--repeat,0,False), Option(-f,--file,1,*.py)), Optional(Option(,--exclude,1,.svn,CVS,.bzr,.hg,.git)), Optional(Either(Option(,--select,1,), Option(,--ignore,1,))), Optional(Option(,--show-source,0,False)), Optional(Option(,--statistics,0,False)), Optional(Option(,--count,0,False)), Optional(Option(,--benchmark,0,False)), OneOrMore(Argument(PATH, []))), Required(Required(Either(Option(,--doctest,0,False), Option(,--testsuite,1,)))), Required(Option(,--version,0,False))))
                var a = new RequiredMatcher(1, required.Left, required.Collected);
                while (a.Next())
                {
                    // Either(Required(Optional(Option(-h,--help,0,False), Option(-v,--verbose,0,False), Option(-q,--quiet,0,False), Option(-r,--repeat,0,False), Option(-f,--file,1,*.py)), Optional(Option(,--exclude,1,.svn,CVS,.bzr,.hg,.git)), Optional(Either(Option(,--select,1,), Option(,--ignore,1,))), Optional(Option(,--show-source,0,False)), Optional(Option(,--statistics,0,False)), Optional(Option(,--count,0,False)), Optional(Option(,--benchmark,0,False)), OneOrMore(Argument(PATH, []))), Required(Required(Either(Option(,--doctest,0,False), Option(,--testsuite,1,)))), Required(Option(,--version,0,False)))
                    var b = new EitherMatcher(3, a.Left, a.Collected);
                    while (b.Next())
                    {
                        switch (b.Index)
                        {
                            case 0:
                            {
                                // Required(Optional(Option(-h,--help,0,False), Option(-v,--verbose,0,False), Option(-q,--quiet,0,False), Option(-r,--repeat,0,False), Option(-f,--file,1,*.py)), Optional(Option(,--exclude,1,.svn,CVS,.bzr,.hg,.git)), Optional(Either(Option(,--select,1,), Option(,--ignore,1,))), Optional(Option(,--show-source,0,False)), Optional(Option(,--statistics,0,False)), Optional(Option(,--count,0,False)), Optional(Option(,--benchmark,0,False)), OneOrMore(Argument(PATH, [])))
                                var c = new RequiredMatcher(8, b.Left, b.Collected);
                                while (c.Next())
                                {
                                    switch (c.Index)
                                    {
                                        case 0:
                                        {
                                            // Optional(Option(-h,--help,0,False), Option(-v,--verbose,0,False), Option(-q,--quiet,0,False), Option(-r,--repeat,0,False), Option(-f,--file,1,*.py))
                                            var d = new OptionalMatcher(5, c.Left, c.Collected);
                                            while (d.Next())
                                            {
                                                switch (d.Index)
                                                {
                                                    case 0:
                                                    {
                                                        // Option(-h,--help,0,False)
                                                        d.Match(PatternMatcher.MatchOption, "--help", ValueKind.Boolean);
                                                    }
                                                    break;
                                                    case 1:
                                                    {
                                                        // Option(-v,--verbose,0,False)
                                                        d.Match(PatternMatcher.MatchOption, "--verbose", ValueKind.Boolean);
                                                    }
                                                    break;
                                                    case 2:
                                                    {
                                                        // Option(-q,--quiet,0,False)
                                                        d.Match(PatternMatcher.MatchOption, "--quiet", ValueKind.Boolean);
                                                    }
                                                    break;
                                                    case 3:
                                                    {
                                                        // Option(-r,--repeat,0,False)
                                                        d.Match(PatternMatcher.MatchOption, "--repeat", ValueKind.Boolean);
                                                    }
                                                    break;
                                                    case 4:
                                                    {
                                                        // Option(-f,--file,1,*.py)
                                                        d.Match(PatternMatcher.MatchOption, "--file", ValueKind.String);
                                                    }
                                                    break;
                                                }
                                                if (!d.LastMatched)
                                                {
                                                    break;
                                                }
                                            }
                                            c.Fold(d.Result);
                                        }
                                        break;
                                        case 1:
                                        {
                                            // Optional(Option(,--exclude,1,.svn,CVS,.bzr,.hg,.git))
                                            var d = new OptionalMatcher(1, c.Left, c.Collected);
                                            while (d.Next())
                                            {
                                                // Option(,--exclude,1,.svn,CVS,.bzr,.hg,.git)
                                                d.Match(PatternMatcher.MatchOption, "--exclude", ValueKind.String);
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
                                            // Optional(Either(Option(,--select,1,), Option(,--ignore,1,)))
                                            var d = new OptionalMatcher(1, c.Left, c.Collected);
                                            while (d.Next())
                                            {
                                                // Either(Option(,--select,1,), Option(,--ignore,1,))
                                                var e = new EitherMatcher(2, d.Left, d.Collected);
                                                while (e.Next())
                                                {
                                                    switch (e.Index)
                                                    {
                                                        case 0:
                                                        {
                                                            // Option(,--select,1,)
                                                            e.Match(PatternMatcher.MatchOption, "--select", ValueKind.None);
                                                        }
                                                        break;
                                                        case 1:
                                                        {
                                                            // Option(,--ignore,1,)
                                                            e.Match(PatternMatcher.MatchOption, "--ignore", ValueKind.None);
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
                                        case 3:
                                        {
                                            // Optional(Option(,--show-source,0,False))
                                            var d = new OptionalMatcher(1, c.Left, c.Collected);
                                            while (d.Next())
                                            {
                                                // Option(,--show-source,0,False)
                                                d.Match(PatternMatcher.MatchOption, "--show-source", ValueKind.Boolean);
                                                if (!d.LastMatched)
                                                {
                                                    break;
                                                }
                                            }
                                            c.Fold(d.Result);
                                        }
                                        break;
                                        case 4:
                                        {
                                            // Optional(Option(,--statistics,0,False))
                                            var d = new OptionalMatcher(1, c.Left, c.Collected);
                                            while (d.Next())
                                            {
                                                // Option(,--statistics,0,False)
                                                d.Match(PatternMatcher.MatchOption, "--statistics", ValueKind.Boolean);
                                                if (!d.LastMatched)
                                                {
                                                    break;
                                                }
                                            }
                                            c.Fold(d.Result);
                                        }
                                        break;
                                        case 5:
                                        {
                                            // Optional(Option(,--count,0,False))
                                            var d = new OptionalMatcher(1, c.Left, c.Collected);
                                            while (d.Next())
                                            {
                                                // Option(,--count,0,False)
                                                d.Match(PatternMatcher.MatchOption, "--count", ValueKind.Boolean);
                                                if (!d.LastMatched)
                                                {
                                                    break;
                                                }
                                            }
                                            c.Fold(d.Result);
                                        }
                                        break;
                                        case 6:
                                        {
                                            // Optional(Option(,--benchmark,0,False))
                                            var d = new OptionalMatcher(1, c.Left, c.Collected);
                                            while (d.Next())
                                            {
                                                // Option(,--benchmark,0,False)
                                                d.Match(PatternMatcher.MatchOption, "--benchmark", ValueKind.Boolean);
                                                if (!d.LastMatched)
                                                {
                                                    break;
                                                }
                                            }
                                            c.Fold(d.Result);
                                        }
                                        break;
                                        case 7:
                                        {
                                            // OneOrMore(Argument(PATH, []))
                                            var d = new OneOrMoreMatcher(1, c.Left, c.Collected);
                                            while (d.Next())
                                            {
                                                // Argument(PATH, [])
                                                d.Match(PatternMatcher.MatchArgument, "PATH", ValueKind.StringList);
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
                                // Required(Required(Either(Option(,--doctest,0,False), Option(,--testsuite,1,))))
                                var c = new RequiredMatcher(1, b.Left, b.Collected);
                                while (c.Next())
                                {
                                    // Required(Either(Option(,--doctest,0,False), Option(,--testsuite,1,)))
                                    var d = new RequiredMatcher(1, c.Left, c.Collected);
                                    while (d.Next())
                                    {
                                        // Either(Option(,--doctest,0,False), Option(,--testsuite,1,))
                                        var e = new EitherMatcher(2, d.Left, d.Collected);
                                        while (e.Next())
                                        {
                                            switch (e.Index)
                                            {
                                                case 0:
                                                {
                                                    // Option(,--doctest,0,False)
                                                    e.Match(PatternMatcher.MatchOption, "--doctest", ValueKind.Boolean);
                                                }
                                                break;
                                                case 1:
                                                {
                                                    // Option(,--testsuite,1,)
                                                    e.Match(PatternMatcher.MatchOption, "--testsuite", ValueKind.None);
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

            collected = GetSuccessfulCollection(required, Usage);
            var result = new ProgramArguments();

            foreach (var p in collected)
            {
                var value = p.Value is { IsStringList: true } ? ((StringList)p.Value).Reverse() : p.Value;
                switch (p.Name)
                {
                    case "--help": result.OptHelp = (bool)value; break;
                    case "--verbose": result.OptVerbose = (bool)value; break;
                    case "--quiet": result.OptQuiet = (bool)value; break;
                    case "--repeat": result.OptRepeat = (bool)value; break;
                    case "--file": result.OptFile = (string)value; break;
                    case "--exclude": result.OptExclude = (string)value; break;
                    case "--select": result.OptSelect = (string?)value; break;
                    case "--ignore": result.OptIgnore = (string?)value; break;
                    case "--show-source": result.OptShowSource = (bool)value; break;
                    case "--statistics": result.OptStatistics = (bool)value; break;
                    case "--count": result.OptCount = (bool)value; break;
                    case "--benchmark": result.OptBenchmark = (bool)value; break;
                    case "PATH": result.ArgPath = (StringList)value; break;
                    case "--doctest": result.OptDoctest = (bool)value; break;
                    case "--testsuite": result.OptTestsuite = (string?)value; break;
                    case "--version": result.OptVersion = (bool)value; break;
                }
            }

            return result;
        }

        IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
        {
            yield return KeyValuePair.Create("--help", (object?)OptHelp);
            yield return KeyValuePair.Create("--verbose", (object?)OptVerbose);
            yield return KeyValuePair.Create("--quiet", (object?)OptQuiet);
            yield return KeyValuePair.Create("--repeat", (object?)OptRepeat);
            yield return KeyValuePair.Create("--file", (object?)OptFile);
            yield return KeyValuePair.Create("--exclude", (object?)OptExclude);
            yield return KeyValuePair.Create("--select", (object?)OptSelect);
            yield return KeyValuePair.Create("--ignore", (object?)OptIgnore);
            yield return KeyValuePair.Create("--show-source", (object?)OptShowSource);
            yield return KeyValuePair.Create("--statistics", (object?)OptStatistics);
            yield return KeyValuePair.Create("--count", (object?)OptCount);
            yield return KeyValuePair.Create("--benchmark", (object?)OptBenchmark);
            yield return KeyValuePair.Create("PATH", (object?)ArgPath);
            yield return KeyValuePair.Create("--doctest", (object?)OptDoctest);
            yield return KeyValuePair.Create("--testsuite", (object?)OptTestsuite);
            yield return KeyValuePair.Create("--version", (object?)OptVersion);
        }

        IEnumerator<KeyValuePair<string, object?>> IEnumerable<KeyValuePair<string, object?>>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary><c>Option(-h,--help,0,False)</c></summary>
        public bool OptHelp { get; private set; }

        /// <summary><c>Option(-v,--verbose,0,False)</c></summary>
        public bool OptVerbose { get; private set; }

        /// <summary><c>Option(-q,--quiet,0,False)</c></summary>
        public bool OptQuiet { get; private set; }

        /// <summary><c>Option(-r,--repeat,0,False)</c></summary>
        public bool OptRepeat { get; private set; }

        /// <summary><c>Option(-f,--file,1,*.py)</c></summary>
        public string OptFile { get; private set; } = "*.py";

        /// <summary><c>Option(,--exclude,1,.svn,CVS,.bzr,.hg,.git)</c></summary>
        public string OptExclude { get; private set; } = ".svn,CVS,.bzr,.hg,.git";

        /// <summary><c>Option(,--select,1,)</c></summary>
        public string? OptSelect { get; private set; }

        /// <summary><c>Option(,--ignore,1,)</c></summary>
        public string? OptIgnore { get; private set; }

        /// <summary><c>Option(,--show-source,0,False)</c></summary>
        public bool OptShowSource { get; private set; }

        /// <summary><c>Option(,--statistics,0,False)</c></summary>
        public bool OptStatistics { get; private set; }

        /// <summary><c>Option(,--count,0,False)</c></summary>
        public bool OptCount { get; private set; }

        /// <summary><c>Option(,--benchmark,0,False)</c></summary>
        public bool OptBenchmark { get; private set; }

        /// <summary><c>Argument(PATH, [])</c></summary>
        public StringList ArgPath { get; private set; } = StringList.Empty;

        /// <summary><c>Option(,--doctest,0,False)</c></summary>
        public bool OptDoctest { get; private set; }

        /// <summary><c>Option(,--testsuite,1,)</c></summary>
        public string? OptTestsuite { get; private set; }

        /// <summary><c>Option(,--version,0,False)</c></summary>
        public bool OptVersion { get; private set; }
    }
}
