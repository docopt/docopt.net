namespace DocoptNet.Tests
{
    using System;
    using System.Collections.Generic;

    static class Extensions
    {
        public static MatchResult Match(this Pattern pattern, params LeafPattern[] left)
        {
            return PatternMatcher.Match(pattern, left.AsReadOnly());
        }

        public static IDictionary<string, ValueObject> Apply(this Docopt docopt,
                                                             string doc, string cmdLine,
                                                             bool help = true,
                                                             object version = null,
                                                             bool optionsFirst = false,
                                                             bool exit = false)
        {
            // A very naive way to split a command line into individual
            // arguments but good enough for the test cases so far:
            var argv = cmdLine.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
            return docopt.Apply(doc, argv, help, version, optionsFirst, exit);
        }
    }
}
