#nullable enable annotations

using System.Collections;
using System.Collections.Generic;
using DocoptNet;
using DocoptNet.Internals;
using Leaves = DocoptNet.Internals.ReadOnlyList<DocoptNet.Internals.LeafPattern>;

namespace MyConsoleApp
{
    partial class Program
    {
        partial class Arguments : IEnumerable<KeyValuePair<string, object?>>
        {
            public const string Usage = "Usage: program";

            static readonly IBaselineParser<Arguments> Parser = GeneratedSourceModule.CreateParser(Help, Parse);

            public static IBaselineParser<Arguments> CreateParser() => Parser;

            static IParser<Arguments>.IResult Parse(IEnumerable<string> args, ParseFlags flags, string? version)
            {
                var options = new List<Option>
                {
                };

                return GeneratedSourceModule.Parse(Help, Usage, args, options, flags, version, Parse);

                static IParser<Arguments>.IResult Parse(Leaves left)
                {
                    var required = new RequiredMatcher(1, left, new Leaves());
                    Match(ref required);
                    if (!required.Result || required.Left.Count > 0)
                    {
                        return GeneratedSourceModule.CreateInputErrorResult<Arguments>(string.Empty, Usage);
                    }
                    var collected = required.Collected;
                    var result = new Arguments();

                    return GeneratedSourceModule.CreateArgumentsResult(result);
                }

                static void Match(ref RequiredMatcher required)
                {
                    // Required(Required())
                    var a = new RequiredMatcher(1, required.Left, required.Collected);
                    while (a.Next())
                    {
                        // Required()
                        var b = new RequiredMatcher(0, a.Left, a.Collected);
                        while (b.Next())
                        {
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
                yield break;
            }

            IEnumerator<KeyValuePair<string, object?>> IEnumerable<KeyValuePair<string, object?>>.GetEnumerator() => GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
