#nullable enable annotations

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DocoptNet.Generated;
using Leaves = DocoptNet.Generated.ReadOnlyList<DocoptNet.Generated.LeafPattern>;
using static DocoptNet.Generated.GeneratedSourceModule;

namespace QuickExample
{
    partial class ProgramArguments : IEnumerable<KeyValuePair<string, object?>>
    {
        public const string HelpText = @"Usage:
  QuickExample tcp <host> <port> [--timeout=<seconds>]
  QuickExample serial <port> [--baud=9600] [--timeout=<seconds>]
  QuickExample -h | --help | --version
";

        public const string Usage = @"Usage:
  QuickExample tcp <host> <port> [--timeout=<seconds>]
  QuickExample serial <port> [--baud=9600] [--timeout=<seconds>]
  QuickExample -h | --help | --version";

        public static ProgramArguments Apply(IEnumerable<string> args, bool help = true, object? version = null, bool optionsFirst = false, bool exit = false)
        {
            var options = new List<Option>
            {
                new Option(null, "--timeout", 1, null),
                new Option(null, "--baud", 1, null),
                new Option("-h", null, 0, false),
                new Option(null, "--help", 0, false),
                new Option(null, "--version", 0, false),
            };
            var left = ParseArgv(HelpText, args, options, optionsFirst, help, version);
            var collected = new Leaves();
            var a = new RequiredMatcher(1, left, collected);
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
                                            d.Match(PatternMatcher.MatchCommand, "tcp", ValueKind.Boolean);
                                        }
                                        break;
                                        case 1:
                                        {
                                            // Argument(<host>, )
                                            d.Match(PatternMatcher.MatchArgument, "<host>", ValueKind.None);
                                        }
                                        break;
                                        case 2:
                                        {
                                            // Argument(<port>, )
                                            d.Match(PatternMatcher.MatchArgument, "<port>", ValueKind.None);
                                        }
                                        break;
                                        case 3:
                                        {
                                            // Optional(Option(,--timeout,1,))
                                            var e = new OptionalMatcher(1, d.Left, d.Collected);
                                            while (e.Next())
                                            {
                                                // Option(,--timeout,1,)
                                                e.Match(PatternMatcher.MatchOption, "--timeout", ValueKind.None);
                                                if (!e.LastMatched)
                                                {
                                                    break;
                                                }
                                            }
                                            d.Fold(e.Result);
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
                                // Required(Command(serial, False), Argument(<port>, ), Optional(Option(,--baud,1,)), Optional(Option(,--timeout,1,)))
                                var d = new RequiredMatcher(4, c.Left, c.Collected);
                                while (d.Next())
                                {
                                    switch (d.Index)
                                    {
                                        case 0:
                                        {
                                            // Command(serial, False)
                                            d.Match(PatternMatcher.MatchCommand, "serial", ValueKind.Boolean);
                                        }
                                        break;
                                        case 1:
                                        {
                                            // Argument(<port>, )
                                            d.Match(PatternMatcher.MatchArgument, "<port>", ValueKind.None);
                                        }
                                        break;
                                        case 2:
                                        {
                                            // Optional(Option(,--baud,1,))
                                            var e = new OptionalMatcher(1, d.Left, d.Collected);
                                            while (e.Next())
                                            {
                                                // Option(,--baud,1,)
                                                e.Match(PatternMatcher.MatchOption, "--baud", ValueKind.None);
                                                if (!e.LastMatched)
                                                {
                                                    break;
                                                }
                                            }
                                            d.Fold(e.Result);
                                        }
                                        break;
                                        case 3:
                                        {
                                            // Optional(Option(,--timeout,1,))
                                            var e = new OptionalMatcher(1, d.Left, d.Collected);
                                            while (e.Next())
                                            {
                                                // Option(,--timeout,1,)
                                                e.Match(PatternMatcher.MatchOption, "--timeout", ValueKind.None);
                                                if (!e.LastMatched)
                                                {
                                                    break;
                                                }
                                            }
                                            d.Fold(e.Result);
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
                                                e.Match(PatternMatcher.MatchOption, "-h", ValueKind.Boolean);
                                            }
                                            break;
                                            case 1:
                                            {
                                                // Option(,--help,0,False)
                                                e.Match(PatternMatcher.MatchOption, "--help", ValueKind.Boolean);
                                            }
                                            break;
                                            case 2:
                                            {
                                                // Option(,--version,0,False)
                                                e.Match(PatternMatcher.MatchOption, "--version", ValueKind.Boolean);
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

            collected = GetSuccessfulCollection(a, Usage);
            var result = new ProgramArguments();

            foreach (var p in collected)
            {
                var value = p.Value is { IsStringList: true } ? ((StringList)p.Value).Reverse() : p.Value;
                switch (p.Name)
                {
                    case "tcp": result.CmdTcp = (bool)value; break;
                    case "<host>": result.ArgHost = (string?)value; break;
                    case "<port>": result.ArgPort = (string?)value; break;
                    case "--timeout": result.OptTimeout = (string?)value; break;
                    case "serial": result.CmdSerial = (bool)value; break;
                    case "--baud": result.OptBaud = (string?)value; break;
                    case "-h": result.OptH = (bool)value; break;
                    case "--help": result.OptHelp = (bool)value; break;
                    case "--version": result.OptVersion = (bool)value; break;
                }
            }

            return result;
        }

        IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
        {
            yield return KeyValuePair.Create("tcp", (object?)CmdTcp);
            yield return KeyValuePair.Create("<host>", (object?)ArgHost);
            yield return KeyValuePair.Create("<port>", (object?)ArgPort);
            yield return KeyValuePair.Create("--timeout", (object?)OptTimeout);
            yield return KeyValuePair.Create("serial", (object?)CmdSerial);
            yield return KeyValuePair.Create("--baud", (object?)OptBaud);
            yield return KeyValuePair.Create("-h", (object?)OptH);
            yield return KeyValuePair.Create("--help", (object?)OptHelp);
            yield return KeyValuePair.Create("--version", (object?)OptVersion);
        }

        IEnumerator<KeyValuePair<string, object?>> IEnumerable<KeyValuePair<string, object?>>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary><c>Command(tcp, False)</c></summary>
        public bool CmdTcp { get; private set; }

        /// <summary><c>Argument(&lt;host&gt;, )</c></summary>
        public string? ArgHost { get; private set; }

        /// <summary><c>Argument(&lt;port&gt;, )</c></summary>
        public string? ArgPort { get; private set; }

        /// <summary><c>Option(,--timeout,1,)</c></summary>
        public string? OptTimeout { get; private set; }

        /// <summary><c>Command(serial, False)</c></summary>
        public bool CmdSerial { get; private set; }

        /// <summary><c>Option(,--baud,1,)</c></summary>
        public string? OptBaud { get; private set; }

        /// <summary><c>Option(-h,,0,False)</c></summary>
        public bool OptH { get; private set; }

        /// <summary><c>Option(,--help,0,False)</c></summary>
        public bool OptHelp { get; private set; }

        /// <summary><c>Option(,--version,0,False)</c></summary>
        public bool OptVersion { get; private set; }
    }
}
