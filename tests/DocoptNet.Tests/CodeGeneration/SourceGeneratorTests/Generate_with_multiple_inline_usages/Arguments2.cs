#nullable enable annotations

using System.Collections;
using System.Collections.Generic;
using DocoptNet;
using DocoptNet.Internals;
using Leaves = DocoptNet.Internals.ReadOnlyList<DocoptNet.Internals.LeafPattern>;

partial class Arguments2 : IEnumerable<KeyValuePair<string, object?>>
{
    public const string Usage = "Usage: my_program (run [--fast] | jump [--high])";

    static readonly IParserWithHelpSupport<Arguments2> Parser = GeneratedSourceModule.CreateParser(Help, Parse);

    public static IParserWithHelpSupport<Arguments2> CreateParser() => Parser;

    static IParser<Arguments2>.IResult Parse(IEnumerable<string> args, ParseFlags flags = ParseFlags.None, string? version = null)
    {
        var options = new List<Option>
        {
            new Option(null, "--fast", 0, false),
            new Option(null, "--high", 0, false),
        };
        return GeneratedSourceModule.Parse(Help,Usage, args, options, flags, version, Parse);

        static IParser<Arguments2>.IResult Parse(Leaves left)
        {
            var required = new RequiredMatcher(1, left, new Leaves());
            Match(ref required);
            if (!required.Result || required.Left.Count > 0)
            {
                return GeneratedSourceModule.CreateInputErrorResult<Arguments2>(string.Empty, Usage);
            }
            var collected = required.Collected;
            var result = new Arguments2();

            foreach (var leaf in collected)
            {
                var value = leaf.Value is { IsStringList: true } ? ((StringList)leaf.Value).Reverse() : leaf.Value;
                switch (leaf.Name)
                {
                    case "run": result.CmdRun = (bool)value; break;
                    case "--fast": result.OptFast = (bool)value; break;
                    case "jump": result.CmdJump = (bool)value; break;
                    case "--high": result.OptHigh = (bool)value; break;
                }
            }

            return GeneratedSourceModule.CreateArgumentsResult(result);
        }

        static void Match(ref RequiredMatcher required)
        {
            // Required(Required(Required(Either(Required(Command(run, False), Optional(Option(,--fast,0,False))), Required(Command(jump, False), Optional(Option(,--high,0,False)))))))
            var a = new RequiredMatcher(1, required.Left, required.Collected);
            while (a.Next())
            {
                // Required(Required(Either(Required(Command(run, False), Optional(Option(,--fast,0,False))), Required(Command(jump, False), Optional(Option(,--high,0,False))))))
                var b = new RequiredMatcher(1, a.Left, a.Collected);
                while (b.Next())
                {
                    // Required(Either(Required(Command(run, False), Optional(Option(,--fast,0,False))), Required(Command(jump, False), Optional(Option(,--high,0,False)))))
                    var c = new RequiredMatcher(1, b.Left, b.Collected);
                    while (c.Next())
                    {
                        // Either(Required(Command(run, False), Optional(Option(,--fast,0,False))), Required(Command(jump, False), Optional(Option(,--high,0,False))))
                        var d = new EitherMatcher(2, c.Left, c.Collected);
                        while (d.Next())
                        {
                            switch (d.Index)
                            {
                                case 0:
                                {
                                    // Required(Command(run, False), Optional(Option(,--fast,0,False)))
                                    var e = new RequiredMatcher(2, d.Left, d.Collected);
                                    while (e.Next())
                                    {
                                        switch (e.Index)
                                        {
                                            case 0:
                                            {
                                                // Command(run, False)
                                                e.Match(PatternMatcher.MatchCommand, "run", ValueKind.Boolean);
                                            }
                                            break;
                                            case 1:
                                            {
                                                // Optional(Option(,--fast,0,False))
                                                var f = new OptionalMatcher(1, e.Left, e.Collected);
                                                while (f.Next())
                                                {
                                                    // Option(,--fast,0,False)
                                                    f.Match(PatternMatcher.MatchOption, "--fast", ValueKind.Boolean);
                                                    if (!f.LastMatched)
                                                    {
                                                        break;
                                                    }
                                                }
                                                e.Fold(f.Result);
                                            }
                                            break;
                                        }
                                        if (!e.LastMatched)
                                        {
                                            break;
                                        }
                                    }
                                    d.Fold(e.Result);
                                }
                                break;
                                case 1:
                                {
                                    // Required(Command(jump, False), Optional(Option(,--high,0,False)))
                                    var e = new RequiredMatcher(2, d.Left, d.Collected);
                                    while (e.Next())
                                    {
                                        switch (e.Index)
                                        {
                                            case 0:
                                            {
                                                // Command(jump, False)
                                                e.Match(PatternMatcher.MatchCommand, "jump", ValueKind.Boolean);
                                            }
                                            break;
                                            case 1:
                                            {
                                                // Optional(Option(,--high,0,False))
                                                var f = new OptionalMatcher(1, e.Left, e.Collected);
                                                while (f.Next())
                                                {
                                                    // Option(,--high,0,False)
                                                    f.Match(PatternMatcher.MatchOption, "--high", ValueKind.Boolean);
                                                    if (!f.LastMatched)
                                                    {
                                                        break;
                                                    }
                                                }
                                                e.Fold(f.Result);
                                            }
                                            break;
                                        }
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
                if (!a.LastMatched)
                {
                    break;
                }
            }
            required.Fold(a.Result);
        }
    }

    IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
    {
        yield return KeyValuePair.Create("run", (object?)CmdRun);
        yield return KeyValuePair.Create("--fast", (object?)OptFast);
        yield return KeyValuePair.Create("jump", (object?)CmdJump);
        yield return KeyValuePair.Create("--high", (object?)OptHigh);
    }

    IEnumerator<KeyValuePair<string, object?>> IEnumerable<KeyValuePair<string, object?>>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary><c>Command(run, False)</c></summary>
    public bool CmdRun { get; private set; }

    /// <summary><c>Option(,--fast,0,False)</c></summary>
    public bool OptFast { get; private set; }

    /// <summary><c>Command(jump, False)</c></summary>
    public bool CmdJump { get; private set; }

    /// <summary><c>Option(,--high,0,False)</c></summary>
    public bool OptHigh { get; private set; }
}
