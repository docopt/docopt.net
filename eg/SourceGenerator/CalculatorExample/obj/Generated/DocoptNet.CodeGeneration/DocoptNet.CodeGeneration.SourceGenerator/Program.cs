#nullable enable annotations

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DocoptNet.Generated;
using Leaves = DocoptNet.Generated.ReadOnlyList<DocoptNet.Generated.LeafPattern>;

namespace CalculatorExample
{
    partial class ProgramArguments : IEnumerable<KeyValuePair<string, object?>>
    {
        public const string Usage = @"Not a serious example.

Usage:
  calculator_example.py <value> ( ( + | - | * | / ) <value> )...
  calculator_example.py <function> <value> [( , <value> )]...
  calculator_example.py (-h | --help)

Examples:
  calculator_example.py 1 + 2 + 3 + 4 + 5
  calculator_example.py 1 + 2 '*' 3 / 4 - 5    # note quotes around '*'
  calculator_example.py sum 10 , 20 , 30 , 40

Options:
  -h, --help
";

        // Required:
        //   Either:
        //     Required:
        //       Argument(<value>, []) -> ArgumentNode <value> List
        //       OneOrMore:
        //         Required:
        //           Required:
        //             Either:
        //               Command(+, 0) -> CommandNode + Bool
        //               Command(-, 0) -> CommandNode - Bool
        //               Command(*, 0) -> CommandNode * Bool
        //               Command(/, 0) -> CommandNode / Bool
        //           Argument(<value>, []) -> ArgumentNode <value> List
        //     Required:
        //       Argument(<function>, ) -> ArgumentNode <function> String
        //       Argument(<value>, []) -> ArgumentNode <value> List
        //       OneOrMore:
        //         Optional:
        //           Required:
        //             Command(,, 0) -> CommandNode , Bool
        //             Argument(<value>, []) -> ArgumentNode <value> List
        //     Required:
        //       Required:
        //         Option(-h,--help,0,False) -> OptionNode help Bool

        public static ProgramArguments Apply(IEnumerable<string> args, bool help = true, object? version = null, bool optionsFirst = false, bool exit = false)
        {
            var tokens = new Tokens(args, typeof(DocoptInputErrorException));
            var options = new List<Option>
            {
                new Option("-h", "--help", 0, false),
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
                // Required(Either(Required(Argument(<value>, []), OneOrMore(Required(Required(Either(Command(+, 0), Command(-, 0), Command(*, 0), Command(/, 0))), Argument(<value>, [])))), Required(Argument(<function>, ), Argument(<value>, []), OneOrMore(Optional(Required(Command(,, 0), Argument(<value>, []))))), Required(Required(Option(-h,--help,0,False)))))
                var b = new RequiredMatcher(1, a.Left, a.Collected);
                while (b.Next())
                {
                    // Either(Required(Argument(<value>, []), OneOrMore(Required(Required(Either(Command(+, 0), Command(-, 0), Command(*, 0), Command(/, 0))), Argument(<value>, [])))), Required(Argument(<function>, ), Argument(<value>, []), OneOrMore(Optional(Required(Command(,, 0), Argument(<value>, []))))), Required(Required(Option(-h,--help,0,False))))
                    var c = new EitherMatcher(3, b.Left, b.Collected);
                    while (c.Next())
                    {
                        switch (c.Index)
                        {
                            case 0:
                            {
                                // Required(Argument(<value>, []), OneOrMore(Required(Required(Either(Command(+, 0), Command(-, 0), Command(*, 0), Command(/, 0))), Argument(<value>, []))))
                                var d = new RequiredMatcher(2, c.Left, c.Collected);
                                while (d.Next())
                                {
                                    switch (d.Index)
                                    {
                                        case 0:
                                        {
                                            // Argument(<value>, [])
                                            d.Match(PatternMatcher.MatchArgument, "<value>", StringList.Empty);
                                            break;
                                        }
                                        case 1:
                                        {
                                            // OneOrMore(Required(Required(Either(Command(+, 0), Command(-, 0), Command(*, 0), Command(/, 0))), Argument(<value>, [])))
                                            var e = new OneOrMoreMatcher(1, d.Left, d.Collected);
                                            while (e.Next())
                                            {
                                                // Required(Required(Either(Command(+, 0), Command(-, 0), Command(*, 0), Command(/, 0))), Argument(<value>, []))
                                                var f = new RequiredMatcher(2, e.Left, e.Collected);
                                                while (f.Next())
                                                {
                                                    switch (f.Index)
                                                    {
                                                        case 0:
                                                        {
                                                            // Required(Either(Command(+, 0), Command(-, 0), Command(*, 0), Command(/, 0)))
                                                            var g = new RequiredMatcher(1, f.Left, f.Collected);
                                                            while (g.Next())
                                                            {
                                                                // Either(Command(+, 0), Command(-, 0), Command(*, 0), Command(/, 0))
                                                                var h = new EitherMatcher(4, g.Left, g.Collected);
                                                                while (h.Next())
                                                                {
                                                                    switch (h.Index)
                                                                    {
                                                                        case 0:
                                                                        {
                                                                            // Command(+, 0)
                                                                            h.Match(PatternMatcher.MatchCommand, "+", 0);
                                                                            break;
                                                                        }
                                                                        case 1:
                                                                        {
                                                                            // Command(-, 0)
                                                                            h.Match(PatternMatcher.MatchCommand, "-", 0);
                                                                            break;
                                                                        }
                                                                        case 2:
                                                                        {
                                                                            // Command(*, 0)
                                                                            h.Match(PatternMatcher.MatchCommand, "*", 0);
                                                                            break;
                                                                        }
                                                                        case 3:
                                                                        {
                                                                            // Command(/, 0)
                                                                            h.Match(PatternMatcher.MatchCommand, "/", 0);
                                                                            break;
                                                                        }
                                                                    }
                                                                    if (!h.LastMatched)
                                                                        break;
                                                                }
                                                                g.Fold(h.Result);
                                                                if (!g.LastMatched)
                                                                    break;
                                                            }
                                                            f.Fold(g.Result);
                                                            break;
                                                        }
                                                        case 1:
                                                        {
                                                            // Argument(<value>, [])
                                                            f.Match(PatternMatcher.MatchArgument, "<value>", StringList.Empty);
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
                                    }
                                    if (!d.LastMatched)
                                        break;
                                }
                                c.Fold(d.Result);
                                break;
                            }
                            case 1:
                            {
                                // Required(Argument(<function>, ), Argument(<value>, []), OneOrMore(Optional(Required(Command(,, 0), Argument(<value>, [])))))
                                var d = new RequiredMatcher(3, c.Left, c.Collected);
                                while (d.Next())
                                {
                                    switch (d.Index)
                                    {
                                        case 0:
                                        {
                                            // Argument(<function>, )
                                            d.Match(PatternMatcher.MatchArgument, "<function>", Value.None);
                                            break;
                                        }
                                        case 1:
                                        {
                                            // Argument(<value>, [])
                                            d.Match(PatternMatcher.MatchArgument, "<value>", StringList.Empty);
                                            break;
                                        }
                                        case 2:
                                        {
                                            // OneOrMore(Optional(Required(Command(,, 0), Argument(<value>, []))))
                                            var e = new OneOrMoreMatcher(1, d.Left, d.Collected);
                                            while (e.Next())
                                            {
                                                // Optional(Required(Command(,, 0), Argument(<value>, [])))
                                                var f = new OptionalMatcher(1, e.Left, e.Collected);
                                                while (f.Next())
                                                {
                                                    // Required(Command(,, 0), Argument(<value>, []))
                                                    var g = new RequiredMatcher(2, f.Left, f.Collected);
                                                    while (g.Next())
                                                    {
                                                        switch (g.Index)
                                                        {
                                                            case 0:
                                                            {
                                                                // Command(,, 0)
                                                                g.Match(PatternMatcher.MatchCommand, ",", 0);
                                                                break;
                                                            }
                                                            case 1:
                                                            {
                                                                // Argument(<value>, [])
                                                                g.Match(PatternMatcher.MatchArgument, "<value>", StringList.Empty);
                                                                break;
                                                            }
                                                        }
                                                        if (!g.LastMatched)
                                                            break;
                                                    }
                                                    f.Fold(g.Result);
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
                                    }
                                    if (!d.LastMatched)
                                        break;
                                }
                                c.Fold(d.Result);
                                break;
                            }
                            case 2:
                            {
                                // Required(Required(Option(-h,--help,0,False)))
                                var d = new RequiredMatcher(1, c.Left, c.Collected);
                                while (d.Next())
                                {
                                    // Required(Option(-h,--help,0,False))
                                    var e = new RequiredMatcher(1, d.Left, d.Collected);
                                    while (e.Next())
                                    {
                                        // Option(-h,--help,0,False)
                                        e.Match(PatternMatcher.MatchOption, "--help", false);
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
  calculator_example.py <value> ( ( + | - | * | / ) <value> )...
  calculator_example.py <function> <value> [( , <value> )]...
  calculator_example.py (-h | --help)";
                throw new DocoptInputErrorException(exitUsage);
            }

            collected = a.Collected;
            var result = new ProgramArguments();

            foreach (var p in collected)
            {
                var value = p.Value is { IsStringList: true } ? ((StringList)p.Value).Reverse() : p.Value;
                switch (p.Name)
                {
                    case @"<value>": result.ArgValue = (StringList)value; break;
                    case @"+": result.CmdPlus = (int)value; break;
                    case @"-": result.CmdMinus = (int)value; break;
                    case @"*": result.CmdStar = (int)value; break;
                    case @"/": result.CmdSlash = (int)value; break;
                    case @"<function>": result.ArgFunction = (string?)value; break;
                    case @",": result.CmdComma = (int)value; break;
                    case @"--help": result.OptHelp = (bool)value; break;
                }
            }

            return result;
        }

        IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
        {
            yield return KeyValuePair.Create("<value>", (object?)ArgValue);
            yield return KeyValuePair.Create("+", (object?)CmdPlus);
            yield return KeyValuePair.Create("-", (object?)CmdMinus);
            yield return KeyValuePair.Create("*", (object?)CmdStar);
            yield return KeyValuePair.Create("/", (object?)CmdSlash);
            yield return KeyValuePair.Create("<function>", (object?)ArgFunction);
            yield return KeyValuePair.Create(",", (object?)CmdComma);
            yield return KeyValuePair.Create("--help", (object?)OptHelp);
        }

        IEnumerator<KeyValuePair<string, object?>> IEnumerable<KeyValuePair<string, object?>>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public StringList ArgValue { get; private set; } = StringList.Empty;
        public int CmdPlus { get; private set; }
        public int CmdMinus { get; private set; }
        public int CmdStar { get; private set; }
        public int CmdSlash { get; private set; }
        public string? ArgFunction { get; private set; }
        public int CmdComma { get; private set; }
        public bool OptHelp { get; private set; }
    }
}
