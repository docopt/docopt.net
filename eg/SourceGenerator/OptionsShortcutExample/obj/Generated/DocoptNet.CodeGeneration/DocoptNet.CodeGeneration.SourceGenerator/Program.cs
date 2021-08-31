#nullable enable annotations

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DocoptNet.Generated;
using Leaves = DocoptNet.Generated.ReadOnlyList<DocoptNet.Generated.LeafPattern>;
using static DocoptNet.Generated.GeneratedSourceModule;

namespace OptionsShortcutExample
{
    partial class ProgramArguments : IEnumerable<KeyValuePair<string, object?>>
    {
        public const string HelpText = @"Example of program which uses [options] shortcut in pattern.

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

        public const string Usage = @"Usage:
  OptionsShortcutExample [options] <port>";

        public static ProgramArguments Apply(IEnumerable<string> args, bool help = true, object? version = null, bool optionsFirst = false, bool exit = false)
        {
            var options = new List<Option>
            {
                new Option("-h", "--help", 0, false),
                new Option(null, "--version", 0, false),
                new Option("-n", "--number", 1, null),
                new Option("-t", "--timeout", 1, null),
                new Option(null, "--apply", 0, false),
                new Option("-q", null, 0, false),
            };
            var left = ParseArgv(HelpText, args, options, optionsFirst, help, version);
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
                                                e.Match(PatternMatcher.MatchOption, "--help", ValueKind.Boolean);
                                            }
                                            break;
                                            case 1:
                                            {
                                                // Option(,--version,0,False)
                                                e.Match(PatternMatcher.MatchOption, "--version", ValueKind.Boolean);
                                            }
                                            break;
                                            case 2:
                                            {
                                                // Option(-n,--number,1,)
                                                e.Match(PatternMatcher.MatchOption, "--number", ValueKind.None);
                                            }
                                            break;
                                            case 3:
                                            {
                                                // Option(-t,--timeout,1,)
                                                e.Match(PatternMatcher.MatchOption, "--timeout", ValueKind.None);
                                            }
                                            break;
                                            case 4:
                                            {
                                                // Option(,--apply,0,False)
                                                e.Match(PatternMatcher.MatchOption, "--apply", ValueKind.Boolean);
                                            }
                                            break;
                                            case 5:
                                            {
                                                // Option(-q,,0,False)
                                                e.Match(PatternMatcher.MatchOption, "-q", ValueKind.Boolean);
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
                            case 1:
                            {
                                // Argument(<port>, )
                                c.Match(PatternMatcher.MatchArgument, "<port>", ValueKind.None);
                            }
                            break;
                        }
                        if (!c.LastMatched)
                        {
                            break;
                        }
                    }
                    b.Fold(c.Result);
                    if (!b.LastMatched)
                    {
                        break;
                    }
                }
                a.Fold(b.Result);
            }
            while (false);

            collected = GetSuccessfulCollection(a, Usage);
            var result = new ProgramArguments();

            foreach (var p in collected)
            {
                var value = p.Value is { IsStringList: true } ? ((StringList)p.Value).Reverse() : p.Value;
                switch (p.Name)
                {
                    case "--help": result.OptHelp = (bool)value; break;
                    case "--version": result.OptVersion = (bool)value; break;
                    case "--number": result.OptNumber = (string?)value; break;
                    case "--timeout": result.OptTimeout = (string?)value; break;
                    case "--apply": result.OptApply = (bool)value; break;
                    case "-q": result.OptQ = (bool)value; break;
                    case "<port>": result.ArgPort = (string?)value; break;
                }
            }

            return result;
        }

        IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
        {
            yield return KeyValuePair.Create("--help", (object?)OptHelp);
            yield return KeyValuePair.Create("--version", (object?)OptVersion);
            yield return KeyValuePair.Create("--number", (object?)OptNumber);
            yield return KeyValuePair.Create("--timeout", (object?)OptTimeout);
            yield return KeyValuePair.Create("--apply", (object?)OptApply);
            yield return KeyValuePair.Create("-q", (object?)OptQ);
            yield return KeyValuePair.Create("<port>", (object?)ArgPort);
        }

        IEnumerator<KeyValuePair<string, object?>> IEnumerable<KeyValuePair<string, object?>>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary><c>Option(-h,--help,0,False)</c></summary>
        public bool OptHelp { get; private set; }

        /// <summary><c>Option(,--version,0,False)</c></summary>
        public bool OptVersion { get; private set; }

        /// <summary><c>Option(-n,--number,1,)</c></summary>
        public string? OptNumber { get; private set; }

        /// <summary><c>Option(-t,--timeout,1,)</c></summary>
        public string? OptTimeout { get; private set; }

        /// <summary><c>Option(,--apply,0,False)</c></summary>
        public bool OptApply { get; private set; }

        /// <summary><c>Option(-q,,0,False)</c></summary>
        public bool OptQ { get; private set; }

        /// <summary><c>Argument(&lt;port&gt;, )</c></summary>
        public string? ArgPort { get; private set; }
    }
}
