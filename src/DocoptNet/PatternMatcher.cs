#nullable enable

namespace DocoptNet
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using Leaves = ReadOnlyList<LeafPattern>;

    interface IMatcher
    {
        bool Match(Pattern pattern);
        MatchResult Result { get; }
    }

    struct RequiredMatcher : IMatcher
    {
        readonly Leaves left;
        readonly Leaves collected;
        Leaves l;
        Leaves c;
        MatchResult? result;

        public RequiredMatcher(Leaves left, Leaves collected) : this() =>
            (this.left, this.collected, l, c) = (left, collected, left, collected);

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
        readonly Leaves left;
        readonly Leaves collected;
        MatchResult match;

        public EitherMatcher(Leaves left, Leaves collected) : this()
        {
            (this.left, this.collected) = (left, collected);
            match = new MatchResult(false, left, collected);
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
        Leaves l, c;

        public OptionalMatcher(Leaves left, Leaves collected) : this() =>
            (l, c) = (left, collected);

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

        public OneOrMoreMatcher(Leaves left, Leaves collected) : this()
        {
            (this.left, this.collected) = (left, collected);
            (l, c) = (left, collected);
        }

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
                case Required required:
                {
                    var m = new RequiredMatcher(left, collected);
                    foreach (var child in required.Children)
                    {
                        if (!m.Match(child))
                            break;
                    }
                    return m.Result;
                }
                case Either either:
                {
                    var m = new EitherMatcher(left, collected);
                    foreach (var child in either.Children)
                    {
                        if (!m.Match(child))
                            break;
                    }
                    return m.Result;
                }
                case Optional optional:
                {
                    var m = new OptionalMatcher(left, collected);
                    foreach (var child in optional.Children)
                    {
                        if (!m.Match(child))
                            break;
                    }
                    return m.Result;
                }
                case OneOrMore oneOrMore:
                {
                    Debug.Assert(oneOrMore.Children.Count == 1);
                    var child = oneOrMore.Children[0];
                    var m = new OneOrMoreMatcher(left, collected);
                    while (true)
                    {
                        if (!m.Match(child))
                            break;
                    }
                    return m.Result;
                }
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
