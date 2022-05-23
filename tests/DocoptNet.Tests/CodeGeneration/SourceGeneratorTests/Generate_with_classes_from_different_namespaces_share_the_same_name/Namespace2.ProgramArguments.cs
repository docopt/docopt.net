#nullable enable

using System.Collections;
using System.Collections.Generic;
using DocoptNet;
using DocoptNet.Internals;
using Leaves = DocoptNet.Internals.ReadOnlyList<DocoptNet.Internals.LeafPattern>;

namespace Namespace2
{
    partial class ProgramArguments : IEnumerable<KeyValuePair<string, object?>>
    {
        public const string Usage = "Usage: program";

        static readonly IBaselineParser<ProgramArguments> Parser = GeneratedSourceModule.CreateParser(Help, Parse);

        public static IBaselineParser<ProgramArguments> CreateParser() => Parser;

        static IParser<ProgramArguments>.IResult Parse(IEnumerable<string> args, ParseFlags flags, string? version)
        {
            var options = new List<Option>
            {
            };

            return GeneratedSourceModule.Parse(Help, Usage, args, options, flags, version, Parse);

            static IParser<ProgramArguments>.IResult Parse(Leaves left)
            {
                var required = new RequiredMatcher(1, left, new Leaves());
                Match(ref required);
                if (!required.Result || required.Left.Count > 0)
                {
                    return GeneratedSourceModule.CreateInputErrorResult<ProgramArguments>(string.Empty, Usage);
                }
                var collected = required.Collected;
                var result = new ProgramArguments();

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
