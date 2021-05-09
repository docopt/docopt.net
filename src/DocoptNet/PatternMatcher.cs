#nullable enable

namespace DocoptNet
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Leaves = ReadOnlyList<LeafPattern>;

    interface IMatcher
    {
        int Index { get; }
        bool Next();
        bool Match(Pattern pattern);
        MatchResult Result { get; }
    }

    struct RequiredMatcher : IMatcher
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

        public bool Match(Pattern pattern) =>
            OnMatch(pattern.Match(l, c));

        public bool OnMatch(MatchResult match)
        {
            if (!match.Matched)
            {
                result = new MatchResult(false, left, collected);
                return false;
            }
            (_, l, c) = match;
            return true;
        }

        public MatchResult Result => result ?? new MatchResult(true, l, c);
    }

    struct EitherMatcher : IMatcher
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

        public bool Match(Pattern pattern) =>
            OnMatch(pattern.Match(left, collected));

        public bool OnMatch(MatchResult match)
        {
            if (match is (true, var l, var c) && (!this.match.Matched || l.Count < this.match.Left.Count))
                this.match = new MatchResult(true, l, c);
            return true;
        }

        public MatchResult Result => match;
    }

    struct OptionalMatcher : IMatcher
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

        public bool Match(Pattern pattern) =>
            OnMatch(pattern.Match(l, c));

        public bool OnMatch(MatchResult match)
        {
            (_, l, c) = match;
            return true;
        }

        public MatchResult Result => new(true, l, c);
    }

    struct OneOrMoreMatcher : IMatcher
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

        public bool Match(Pattern pattern) =>
            OnMatch(pattern.Match(l, c));

        public bool OnMatch(MatchResult match)
        {
            bool matched;
            (matched, l, c) = match;
            times += matched ? 1 : 0;
            if (l_ != null && l_.Equals(l))
                return false;
            l_ = l;
            return true;
        }

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
                case Command command:
                {
                    for (var i = 0; i < left.Count; i++)
                    {
                        if (left[i] is Argument { Value: { } value })
                        {
                            if (value.ToString() == command.Name)
                                return MatchLeaf(command, i, new Command(command.Name, new ValueObject(true)));
                            break;
                        }
                    }
                    return new MatchResult(false, left, collected);
                }
                case Argument argument:
                {
                    for (var i = 0; i < left.Count; i++)
                    {
                        if (left[i] is Argument { Value: var value })
                            return MatchLeaf(argument, i, new Argument(argument.Name, value));
                    }
                    return new MatchResult(false, left, collected);
                }
                case Option option:
                {
                    for (var i = 0; i < left.Count; i++)
                    {
                        if (left[i].Name == option.Name)
                            return MatchLeaf(option, i, left[i]);
                    }
                    return new MatchResult(false, left, collected);
                }
                default:
                    throw new ArgumentException(nameof(pattern));
            }

            static MatchResult MatchBranch<T>(IList<Pattern> children, T matcher) where T : IMatcher
            {
                while (matcher.Next())
                {
                    if (!matcher.Match(children[matcher.Index]))
                        break;
                }
                return matcher.Result;
            }

            MatchResult MatchLeaf(LeafPattern leaf, int index, LeafPattern match)
            {
                var left_ = left.RemoveAt(index);
                var sameName = collected.Where(a => a.Name == leaf.Name).ToList();
                if (leaf.Value is { IsList: true } or { IsOfTypeInt: true })
                {
                    var increment = new ValueObject(1);
                    if (!leaf.Value.IsOfTypeInt)
                    {
                        increment = match.Value.IsString ? new ValueObject(new [] { match.Value }) : match.Value;
                    }
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
        }
    }
}
