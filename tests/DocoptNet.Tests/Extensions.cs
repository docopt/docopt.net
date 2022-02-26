namespace DocoptNet.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    static partial class Extensions
    {
        public static MatchResult Match(this Pattern pattern, params LeafPattern[] left)
        {
            return PatternMatcher.Match(pattern, left.AsReadOnly());
        }

        public static IDictionary<string, ArgValue> Apply(this Docopt docopt,
                                                       string doc, string cmdLine,
                                                       bool help = true,
                                                       object version = null,
                                                       bool optionsFirst = false,
                                                       bool exit = false)
        {
            return docopt.Apply(doc, Args.Parse(cmdLine), help, version, optionsFirst, exit);
        }

        public static IDictionary<string, ArgValue> Apply(this Docopt docopt,
                                                       string doc, Args argv,
                                                       bool help = true,
                                                       object version = null,
                                                       bool optionsFirst = false,
                                                       bool exit = false)
        {
            return Docopt.Internal.Apply(docopt, doc, argv.List.AsEnumerable(), help, version, optionsFirst, exit);
        }
    }

#if NETCOREAPP3_1_OR_GREATER

    [Flags]
    enum StreamContentComparisonFlags
    {
        None = 0,
        SkipZeroPositionCheck = 1,
        SkipLengthCheck = 2,
    }

    static partial class Extensions
    {
        public static bool ContentEquals(this Stream first, Stream second,
                                         StreamContentComparisonFlags flags = StreamContentComparisonFlags.None)
        {
            if (0 == (flags & StreamContentComparisonFlags.SkipZeroPositionCheck))
            {
                if (first.Position != 0)
                    throw new ArgumentException(null, nameof(first));

                if (second.Position != 0)
                    throw new ArgumentException(null, nameof(second));
            }

            if (0 == (flags & StreamContentComparisonFlags.SkipLengthCheck) && first.Length != second.Length)
                return false;

            // Credit & inspiration: https://stackoverflow.com/a/1359947/6682

            const int bufferSize = sizeof(long);
            Span<byte> buffer1 = stackalloc byte[bufferSize];
            Span<byte> buffer2 = stackalloc byte[bufferSize];

            var iterations = (int)Math.Ceiling((double)first.Length / bufferSize);

            for (var i = 0; i < iterations; i++)
            {
                first.Read(buffer1);
                second.Read(buffer2);

                if (BitConverter.ToInt64(buffer1) != BitConverter.ToInt64(buffer2))
                    return false;
            }

            return true;
        }
    }

#endif

    sealed class Args
    {
        public static Args Argv(params string[] args) => new(args);

        public static Args Parse(string cmdLine) =>
            // A very naive way to split a command line into individual
            // arguments but good enough for the test cases so far:
            new(cmdLine.Split((char[])null, StringSplitOptions.RemoveEmptyEntries));

        public Args(IList<string> list) => List = list;
        public IList<string> List { get; }
    }
}
