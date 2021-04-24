#nullable enable

namespace DocoptNet
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    static class PatternMatcher
    {
        public static MatchResult Match(this Pattern pattern, IList<LeafPattern> left)
        {
            return pattern.Match(left, new List<LeafPattern>());
        }

        static MatchResult Match(this Pattern pattern, IList<LeafPattern> left, IEnumerable<LeafPattern> collected)
        {
            switch (pattern)
            {
                case Required required:
                {
                    var l = left;
                    var c = collected;
                    foreach (var child in required.Children)
                    {
                        bool matched;
                        (matched, l, c) = child.Match(l, c);
                        if (!matched)
                            return new MatchResult(false, left, collected);
                    }
                    return new MatchResult(true, l, c);
                }
                case Either either:
                {
                    MatchResult? match = null;
                    foreach (var child in either.Children)
                    {
                        if (child.Match(left, collected) is (true, var l, var c)
                            && (match is null || l.Count < match.Left.Count))
                        {
                            match = new MatchResult(true, l, c);
                        }
                    }
                    return match ?? new MatchResult(false, left, collected);
                }
                case Optional optional:
                {
                    var l = left;
                    var c = collected;
                    foreach (var child in optional.Children)
                        (_, l, c) = child.Match(l, c);
                    return new MatchResult(true, l, c);
                }
                case OneOrMore oneOrMore:
                {
                    Debug.Assert(oneOrMore.Children.Count == 1);
                    var l = left;
                    var c = collected;
                    IList<LeafPattern>? l_ = null;
                    var matched = true;
                    var times = 0;
                    while (matched)
                    {
                        // could it be that something didn't match but changed l or c?
                        (matched, l, c) = oneOrMore.Children[0].Match(l, c);
                        times += matched ? 1 : 0;
                        if (l_ != null && l_.Equals(l))
                            break;
                        l_ = l;
                    }
                    if (times >= 1)
                    {
                        return new MatchResult(true, l, c);
                    }
                    return new MatchResult(false, left, collected);
                }
                case LeafPattern leaf:
                {
                    var (index, match) = SingleMatch(leaf, left);
                    if (match == null)
                    {
                        return new MatchResult(false, left, collected);
                    }
                    var left_ = new List<LeafPattern>();
                    left_.AddRange(left.Take(index));
                    left_.AddRange(left.Skip(index + 1));
                    var sameName = collected.Where(a => a.Name == leaf.Name).ToList();
                    if (leaf.Value is { IsList: true } or { IsOfTypeInt: true })
                    {
                        var increment = new ValueObject(1);
                        if (!leaf.Value.IsOfTypeInt)
                        {
                            increment = match.Value.IsString ? new ValueObject(new [] {match.Value})  : match.Value;
                        }
                        if (sameName.Count == 0)
                        {
                            match.Value = increment;
                            return new MatchResult(true, left_, new List<LeafPattern>(collected) { match });
                        }
                        sameName[0].Value.Add(increment);
                        return new MatchResult(true, left_, collected);
                    }
                    return new MatchResult(true, left_, new List<LeafPattern>(collected) { match });
                }
                default: throw new ArgumentException(nameof(pattern));
            }

            static (int, LeafPattern?) SingleMatch(LeafPattern pattern, IList<LeafPattern> left)
            {
                switch (pattern)
                {
                    case Command command:
                    {
                        for (var i = 0; i < left.Count; i++)
                        {
                            if (left[i] is Argument { Value: { } value })
                            {
                                if (value.ToString() == command.Name)
                                    return (i, new Command(command.Name, new ValueObject(true)));
                                break;
                            }
                        }
                        return default;
                    }
                    case Argument argument:
                    {
                        for (var i = 0; i < left.Count; i++)
                        {
                            if (left[i] is Argument { Value: var value })
                                return (i, new Argument(argument.Name, value));
                        }
                        return default;
                    }
                    case Option option:
                    {
                        for (var i = 0; i < left.Count; i++)
                        {
                            if (left[i].Name == option.Name)
                                return (i, left[i]);
                        }
                        return default;
                    }
                    default: throw new ArgumentException(nameof(pattern));
                }
            }
        }
    }
}
