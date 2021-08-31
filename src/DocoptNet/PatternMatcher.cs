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
        bool Match(LeafPatternMatcher matcher, string name, ValueKind kind);
        bool Match(Pattern pattern);
        bool Fold(MatchResult match);
        bool LastMatched { get; }
        MatchResult Result { get; }
    }

    static class LeafMatcherExtensions
    {
        public static MatchResult Match(this LeafPatternMatcher matcher,
                                        Leaves left, Leaves collected,
                                        string name, ValueKind kind)
        {
            var (index, match) = matcher(left, name);
            return PatternMatcher.MatchLeaf(left, collected, name, kind, index, match);
        }
    }

    static class BranchPatternMatcher
    {
        public static bool Next(ref int i, int count)
        {
            if (i == count)
                return false;
            i++;
            return true;
        }
    }

    struct RequiredMatcher : IBranchPatternMatcher
    {
        readonly int _count;
        readonly Leaves _initLeft, _initCollected;
        int _i;
        MatchResult? _result;

        public RequiredMatcher(int count, Leaves left, Leaves collected) : this()
        {
            _count = count;
            Left = _initLeft = left;
            Collected = _initCollected = collected;
        }

        public int Index => _i - 1;

        public bool Next() => BranchPatternMatcher.Next(ref _i, _count);

        public Leaves Left { get; private set; }
        public Leaves Collected { get; private set; }

        public bool Match(LeafPatternMatcher matcher, string name, ValueKind kind) =>
            Fold(matcher.Match(Left, Collected, name, kind));

        public bool Match(Pattern pattern) =>
            Fold(pattern.Match(Left, Collected));

        public bool Fold(MatchResult match)
        {
            if (!match)
            {
                _result = new MatchResult(false, _initLeft, _initCollected);
                return LastMatched = false;
            }
            (_, Left, Collected) = match;
            return LastMatched = true;
        }

        public bool LastMatched { get; private set; }

        public MatchResult Result => _result ?? new MatchResult(true, Left, Collected);
    }

    struct EitherMatcher : IBranchPatternMatcher
    {
        readonly int _count;
        int _i;
        MatchResult _match;

        public EitherMatcher(int count, Leaves left, Leaves collected) : this()
        {
            _count = count;
            (Left, Collected) = (left, collected);
            _match = new MatchResult(false, left, collected);
        }

        public int Index => _i - 1;

        public bool Next() => BranchPatternMatcher.Next(ref _i, _count);

        public Leaves Left { get; }
        public Leaves Collected { get; }

        public bool Match(LeafPatternMatcher matcher, string name, ValueKind kind) =>
            Fold(matcher.Match(Left, Collected, name, kind));

        public bool Match(Pattern pattern) =>
            Fold(pattern.Match(Left, Collected));

        public bool Fold(MatchResult match)
        {
            if (match is (true, var left, var collected) && (!_match || left.Count < _match.Left.Count))
                _match = new MatchResult(true, left, collected);
            return true;
        }

        public bool LastMatched => true;

        public MatchResult Result => _match;
    }

    struct OptionalMatcher : IBranchPatternMatcher
    {
        readonly int _count;
        int _i;

        public OptionalMatcher(int count, Leaves left, Leaves collected) : this() =>
            (_count, Left, Collected) = (count, left, collected);

        public int Index => _i - 1;

        public bool Next() => BranchPatternMatcher.Next(ref _i, _count);

        public Leaves Left { get; private set; }
        public Leaves Collected { get; private set; }

        public bool Match(LeafPatternMatcher matcher, string name, ValueKind kind) =>
            Fold(matcher.Match(Left, Collected, name, kind));

        public bool Match(Pattern pattern) =>
            Fold(pattern.Match(Left, Collected));

        public bool Fold(MatchResult match)
        {
            (_, Left, Collected) = match;
            return true;
        }

        public bool LastMatched => true;

        public MatchResult Result => new(true, Left, Collected);
    }

    struct OneOrMoreMatcher : IBranchPatternMatcher
    {
        readonly Leaves _initLeft, _initCollected;
        int _times;
        Leaves? _lastLeft;

        public OneOrMoreMatcher(int count, Leaves left, Leaves collected) : this()
        {
            Left = _initLeft = left;
            Collected = _initCollected = collected;
        }

        public int Index => 0;
        public bool Next() => true;

        public Leaves Left { get; private set; }
        public Leaves Collected { get; private set; }

        public bool Match(LeafPatternMatcher matcher, string name, ValueKind kind) =>
            Fold(matcher.Match(Left, Collected, name, kind));

        public bool Match(Pattern pattern) =>
            Fold(pattern.Match(Left, Collected));

        public bool Fold(MatchResult match)
        {
            bool matched;
            (matched, Left, Collected) = match;
            _times += matched ? 1 : 0;
            if (_lastLeft != null && _lastLeft.Equals(Left))
                return LastMatched = false;
            _lastLeft = Left;
            return LastMatched = true;
        }

        public bool LastMatched { get; private set; }

        public MatchResult Result => _times >= 1 ? new MatchResult(true, Left, Collected) : new MatchResult(false, _initLeft, _initCollected);
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

                    return matcher.Match(left, collected, leaf.Name, leaf.Value.Kind);
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

        public static MatchResult MatchLeaf(Leaves left, Leaves collected,
                                            string name, ValueKind kind,
                                            int index, LeafPattern? match)
        {
            if (match == null)
            {
                return new MatchResult(false, left, collected);
            }
            var left_ = left.RemoveAt(index);
            if (kind is ValueKind.StringList or ValueKind.Integer)
            {
                var sameNames = collected.Where(a => a.Name == name).ToList();
                if (sameNames.Count == 0)
                {
                    match.Value = kind == ValueKind.Integer ? 1
                                : match.Value.TryAsString(out var s) ? StringList.Empty.Push(s)
                                : match.Value;
                }
                else
                {
                    var sameName = sameNames[0];
                    sameName.Value = sameName.Value.TryAsInteger(out var n)
                                   ? n + 1
                                   : ((StringList)sameName.Value).Push((string)match.Value);

                    return new MatchResult(true, left_, collected);
                }
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
                if (left[i] is Argument { Value: var value })
                {
                    if ((string)value == command)
                        return (i, new Command(command, true));
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
                    return (i, new Argument(name) { Value = value });
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
