#nullable enable

namespace DocoptNet
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Linq;

    static class PatternMatcher
    {
        public static MatchResult Match(this Pattern pattern, ReadOnlyList<LeafPattern> left)
        {
            return pattern.Match(left, new ReadOnlyList<LeafPattern>());
        }

        static MatchResult Match(this Pattern pattern,
                                 ReadOnlyList<LeafPattern> left,
                                 ReadOnlyList<LeafPattern> collected)
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
                    var match = new MatchResult(false, left, collected);
                    foreach (var child in either.Children)
                    {
                        if (child.Match(left, collected) is (true, var l, var c)
                            && (!match || l.Count < match.Left.Count))
                        {
                            match = new MatchResult(true, l, c);
                        }
                    }
                    return match;
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
                    ReadOnlyList<LeafPattern>? l_ = null;
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
                case Command command:
                {
                    for (var i = 0; i < left.Count; i++)
                    {
                        if (left[i] is Argument { Value: { } value })
                        {
                            if (value.ToString() == command.Name)
                                return MatchLeaf(command, i, new Command(command.Name, Box.True));
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
                if (leaf is { Value: ICollection } or { Value: int })
                {
                    var increment = Box.One;
                    if (leaf.Value is not int)
                    {
                        increment = match.Value is string ? new ArrayList { match.Value } : match.Value;
                    }
                    if (sameName.Count == 0)
                    {
                        match.Value = increment;
                        return new MatchResult(true, left_, collected.Append(match));
                    }
                    sameName[0].Add(increment);
                    return new MatchResult(true, left_, collected);
                }
                return new MatchResult(true, left_, collected.Append(match));
            }
        }
    }
}
