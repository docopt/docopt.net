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

        public static IDictionary<string, object> Apply(this Docopt docopt,
                                                        string doc, string cmdLine,
                                                        bool help = true,
                                                        object version = null,
                                                        bool optionsFirst = false,
                                                        bool exit = false)
        {
            return docopt.Apply(doc, Args.Parse(cmdLine), help, version, optionsFirst, exit);
        }

        public static IDictionary<string, object> Apply(this Docopt docopt,
                                                        string doc, Args argv,
                                                        bool help = true,
                                                        object version = null,
                                                        bool optionsFirst = false,
                                                        bool exit = false)
        {
            return docopt.Apply(doc, argv.List, ApplicationResultAccumulators.ObjectDictionary, help, version, optionsFirst, exit);
        }
    }

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
