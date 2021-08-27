#nullable enable annotations

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DocoptNet.Generated;
using Leaves = DocoptNet.Generated.ReadOnlyList<DocoptNet.Generated.LeafPattern>;

namespace QuickExample
{
    partial class ProgramArguments : IEnumerable<KeyValuePair<string, object?>>
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

        public static ProgramArguments Apply(IEnumerable<string> args, bool help = true, object? version = null, bool optionsFirst = false, bool exit = false)
        {
            var tokens = new Tokens(args, typeof(DocoptInputErrorException));
            var options = new List<Option>
            {
                new Option(null, "--timeout", 1, null),
                new Option(null, "--baud", 1, null),
                new Option("-h", null, 0, false),
                new Option(null, "--help", 0, false),
                new Option(null, "--version", 0, false),
            };
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
                                            d.Match(PatternMatcher.MatchCommand, "tcp", false);
                                            break;
                                        }
                                        case 1:
                                        {
                                            // Argument(<host>, )
                                            d.Match(PatternMatcher.MatchArgument, "<host>", Value.None);
                                            break;
                                        }
                                        case 2:
                                        {
                                            // Argument(<port>, )
                                            d.Match(PatternMatcher.MatchArgument, "<port>", Value.None);
                                            break;
                                        }
                                        case 3:
                                        {
                                            // Optional(Option(,--timeout,1,))
                                            var e = new OptionalMatcher(1, d.Left, d.Collected);
                                            while (e.Next())
                                            {
                                                // Option(,--timeout,1,)
                                                e.Match(PatternMatcher.MatchOption, "--timeout", Value.None);
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
                                            d.Match(PatternMatcher.MatchCommand, "serial", false);
                                            break;
                                        }
                                        case 1:
                                        {
                                            // Argument(<port>, )
                                            d.Match(PatternMatcher.MatchArgument, "<port>", Value.None);
                                            break;
                                        }
                                        case 2:
                                        {
                                            // Optional(Option(,--baud,1,))
                                            var e = new OptionalMatcher(1, d.Left, d.Collected);
                                            while (e.Next())
                                            {
                                                // Option(,--baud,1,)
                                                e.Match(PatternMatcher.MatchOption, "--baud", Value.None);
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
                                                e.Match(PatternMatcher.MatchOption, "--timeout", Value.None);
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
                                                e.Match(PatternMatcher.MatchOption, "-h", false);
                                                break;
                                            }
                                            case 1:
                                            {
                                                // Option(,--help,0,False)
                                                e.Match(PatternMatcher.MatchOption, "--help", false);
                                                break;
                                            }
                                            case 2:
                                            {
                                                // Option(,--version,0,False)
                                                e.Match(PatternMatcher.MatchOption, "--version", false);
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

            collected = a.Collected;
            var result = new ProgramArguments();

            foreach (var p in collected)
            {
                var value = p.Value is { IsStringList: true } ? ((StringList)p.Value).Reverse() : p.Value;
                switch (p.Name)
                {
                    case @"tcp": result.CmdTcp = (bool)value; break;
                    case @"<host>": result.ArgHost = (string?)value; break;
                    case @"<port>": result.ArgPort = (string?)value; break;
                    case @"--timeout": result.OptTimeout = (string?)value; break;
                    case @"serial": result.CmdSerial = (bool)value; break;
                    case @"--baud": result.OptBaud = (string?)value; break;
                    case @"-h": result.OptH = (bool)value; break;
                    case @"--help": result.OptHelp = (bool)value; break;
                    case @"--version": result.OptVersion = (bool)value; break;
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

        public bool CmdTcp { get; private set; }
        public string? ArgHost { get; private set; }
        public string? ArgPort { get; private set; }
        public string? OptTimeout { get; private set; }
        public bool CmdSerial { get; private set; }
        public string? OptBaud { get; private set; }
        public bool OptH { get; private set; }
        public bool OptHelp { get; private set; }
        public bool OptVersion { get; private set; }
    }
}
