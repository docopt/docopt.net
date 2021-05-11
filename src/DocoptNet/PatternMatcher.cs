#nullable enable

namespace DocoptNet
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Leaves = ReadOnlyList<LeafPattern>;

    delegate (int Index, LeafPattern Match) LeafPatternMatcher(Leaves left, string name);

    interface IBranchPatternMatcher
    {
        int Index { get; }
        bool Next();
        Leaves Left { get; }
        Leaves Collected { get; }
        bool Match(LeafPatternMatcher matcher, string name, object? value, bool isList, bool isInt);
        bool Match(Pattern pattern);
        bool OnMatch(MatchResult match);
        bool LastMatched { get; }
        MatchResult Result { get; }
    }

    static class LeafMatcherExtensions
    {
        public static MatchResult
            Match(this LeafPatternMatcher matcher,
                  Leaves left, Leaves collected,
                  string name, object? value, bool isList, bool isInt)
        {
            var (index, match) = matcher(left, name);
            return PatternMatcher.MatchLeaf(left, collected, name, value, isList, isInt, index, match);
        }
    }

    struct RequiredMatcher : IBranchPatternMatcher
    {
        readonly int count;
        readonly Leaves left;
        readonly Leaves collected;
        int i;
        Leaves l;
        Leaves c;
        MatchResult? result;

        public RequiredMatcher(int count, Leaves left, Leaves collected) : this() =>
            (this.count, this.left, this.collected, l, c) = (count, left, collected, left, collected);

        public int Index => i - 1;

        public bool Next()
        {
            if (i == count)
                return false;
            i++;
            return true;
        }

        public Leaves Left => l;
        public Leaves Collected => c;

        public bool Match(LeafPatternMatcher matcher, string name, object? value, bool isList, bool isInt) =>
            OnMatch(matcher.Match(l, c, name, value, isList, isInt));

        public bool Match(Pattern pattern) =>
            OnMatch(pattern.Match(l, c));

        public bool OnMatch(MatchResult match)
        {
            if (!match)
            {
                result = new MatchResult(false, left, collected);
                return LastMatched = false;
            }
            (_, l, c) = match;
            return LastMatched = true;
        }

        public bool LastMatched { get; private set; }

        public MatchResult Result => result ?? new MatchResult(true, l, c);
    }

    struct EitherMatcher : IBranchPatternMatcher
    {
        readonly int count;
        readonly Leaves left;
        readonly Leaves collected;
        int i;
        MatchResult match;

        public EitherMatcher(int count, Leaves left, Leaves collected) : this()
        {
            this.count = count;
            (this.left, this.collected) = (left, collected);
            match = new MatchResult(false, left, collected);
        }

        public int Index => i - 1;

        public bool Next()
        {
            if (i == count)
                return false;
            i++;
            return true;
        }

        public Leaves Left => left;
        public Leaves Collected => collected;

        public bool Match(LeafPatternMatcher matcher, string name, object? value, bool isList, bool isInt) =>
            OnMatch(matcher.Match(left, collected, name, value, isList, isInt));

        public bool Match(Pattern pattern) =>
            OnMatch(pattern.Match(left, collected));

        public bool OnMatch(MatchResult match)
        {
            if (match is (true, var l, var c) && (!this.match || l.Count < this.match.Left.Count))
                this.match = new MatchResult(true, l, c);
            return true;
        }

        public bool LastMatched => true;

        public MatchResult Result => match;
    }

    struct OptionalMatcher : IBranchPatternMatcher
    {
        readonly int count;
        int i;
        Leaves l, c;

        public OptionalMatcher(int count, Leaves left, Leaves collected) : this() =>
            (this.count, l, c) = (count, left, collected);

        public int Index => i - 1;

        public bool Next()
        {
            if (i == count)
                return false;
            i++;
            return true;
        }

        public Leaves Left => l;
        public Leaves Collected => c;

        public bool Match(LeafPatternMatcher matcher, string name, object? value, bool isList, bool isInt) =>
            OnMatch(matcher.Match(l, c, name, value, isList, isInt));

        public bool Match(Pattern pattern) =>
            OnMatch(pattern.Match(l, c));

        public bool OnMatch(MatchResult match)
        {
            (_, l, c) = match;
            return true;
        }

        public bool LastMatched => true;

        public MatchResult Result => new(true, l, c);
    }

    struct OneOrMoreMatcher : IBranchPatternMatcher
    {
        readonly Leaves left, collected;
        Leaves l, c;
        int times;
        Leaves? l_;

        public OneOrMoreMatcher(int count, Leaves left, Leaves collected) : this()
        {
            (this.left, this.collected) = (left, collected);
            (l, c) = (left, collected);
        }

        public int Index => 0;
        public bool Next() => true;

        public Leaves Left => l;
        public Leaves Collected => c;

        public bool Match(LeafPatternMatcher matcher, string name, object? value, bool isList, bool isInt) =>
            OnMatch(matcher.Match(l, c, name, value, isList, isInt));

        public bool Match(Pattern pattern) =>
            OnMatch(pattern.Match(l, c));

        public bool OnMatch(MatchResult match)
        {
            bool matched;
            (matched, l, c) = match;
            times += matched ? 1 : 0;
            if (l_ != null && l_.Equals(l))
                return LastMatched = false;
            l_ = l;
            return LastMatched = true;
        }

        public bool LastMatched { get; private set; }

        public MatchResult Result => times >= 1 ? new MatchResult(true, l, c) : new MatchResult(false, left, collected);
    }

    static class PatternMatcher
    {
        public static MatchResult Match(this Pattern pattern, Leaves left)
        {
            return pattern.Match(left, new Leaves());
        }

        public static MatchResult Match(this Pattern pattern, Leaves left, Leaves collected)
        {
            switch (pattern)
            {
                case Required  { Children: { Count: var count } children }: return MatchBranch(children, new RequiredMatcher(count, left, collected));
                case Either    { Children: { Count: var count } children }: return MatchBranch(children, new EitherMatcher(count, left, collected));
                case Optional  { Children: { Count: var count } children }: return MatchBranch(children, new OptionalMatcher(count, left, collected));
                case OneOrMore { Children: {} children }: return MatchBranch(children, new OneOrMoreMatcher(1, left, collected));
                case LeafPattern leaf:
                {
                    var matcher = leaf switch
                    {
                        Command  => CommandMatcher,
                        Argument => ArgumentMatcher,
                        Option   => OptionMatcher,
                        _ => throw new NotSupportedException()
                    };

                    return matcher.Match(left, collected,
                                         leaf.Name, leaf.Value,
                                         leaf.Value?.IsList ?? false,
                                         leaf.Value?.IsOfTypeInt ?? false);
                }
                default:
                    throw new ArgumentException(nameof(pattern));
            }

            static MatchResult MatchBranch<T>(IList<Pattern> children, T matcher) where T : IBranchPatternMatcher
            {
                while (matcher.Next())
                {
                    if (!matcher.Match(children[matcher.Index]))
                        break;
                }
                return matcher.Result;
            }
        }

        public static MatchResult
            MatchLeaf(Leaves left, Leaves collected,
                      string name, object? value, bool isList, bool isInt,
                      int index, LeafPattern? match)
        {
            if (match == null)
            {
                return new MatchResult(false, left, collected);
            }
            var left_ = left.RemoveAt(index);
            var sameName = collected.Where(a => a.Name == name).ToList();
            if (value != null && (isList || isInt))
            {
                var increment = new ValueObject(1);
                if (!isInt)
                    increment = match.Value.IsString ? new ValueObject(new [] {match.Value})  : match.Value;
                if (sameName.Count == 0)
                {
                    match.Value = increment;
                    return new MatchResult(true, left_, collected.Append(match));
                }

                sameName[0].Value.Add(increment);
                return new MatchResult(true, left_, collected);
            }
            return new MatchResult(true, left_, collected.Append(match));
        }

        public static readonly LeafPatternMatcher CommandMatcher  = MatchCommand;
        public static readonly LeafPatternMatcher ArgumentMatcher = MatchArgument;
        public static readonly LeafPatternMatcher OptionMatcher   = MatchOption;

        public static (int, LeafPattern) MatchCommand(Leaves left, string command)
        {
            for (var i = 0; i < left.Count; i++)
            {
                if (left[i] is Argument { Value: { } value })
                {
                    if (value.ToString() == command)
                        return (i, new Command(command, new ValueObject(true)));
                    break;
                }
            }
            return default;
        }

        public static (int, LeafPattern) MatchArgument(Leaves left, string name)
        {
            for (var i = 0; i < left.Count; i++)
            {
                if (left[i] is Argument { Value: var value })
                    return (i, new Argument(name, value));
            }
            return default;
        }

        public static (int, LeafPattern) MatchOption(Leaves left, string name)
        {
            for (var i = 0; i < left.Count; i++)
            {
                if (left[i].Name == name)
                    return (i, (Option)left[i]);
            }
            return default;
        }
    }
}
