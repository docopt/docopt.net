namespace DocoptNet.Generated
{
    using System.Collections.Immutable;
    using System.Linq;
    using Leaves = ReadOnlyList<LeafPattern>;

    abstract record Pattern;

    abstract record BranchPattern(ImmutableArray<Pattern> Children) : Pattern;
    sealed record Required(ImmutableArray<Pattern> Children) : BranchPattern(Children);
    sealed record Optional(ImmutableArray<Pattern> Children) : BranchPattern(Children);
    sealed record Either(ImmutableArray<Pattern> Children) : BranchPattern(Children);
    sealed record OneOrMore(Pattern Pattern) : BranchPattern(ImmutableArray.Create(Pattern));
    abstract record LeafPattern(string Name, ValueObject Value) : Pattern;

    sealed record Command(string Name) : LeafPattern(Name, new ValueObject(true));
    sealed record Argument(string Name, ValueObject Value) : LeafPattern(Name, Value);
    sealed record Option(string ShortName, string LongName, int ArgCount, ValueObject Value) : LeafPattern(LongName ?? ShortName, Value);

    static class Module
    {
        public static (bool Matched, Leaves Left, Leaves Collected)
            Leaf(Leaves left, Leaves collected,
                 string name, object value, bool isList, bool isInt,
                 int index, LeafPattern match)
        {
            if (match == null)
            {
                return (false, left, collected);
            }
            var left_ = left.RemoveAt(index);
            var sameName = collected.Where(a => a.Name == name).ToList();
            if (value != null && (isList || isInt))
            {
                var increment = new ValueObject(1);
                if (!isInt)
                    increment = match.Value.IsString ? new ValueObject(new [] {match.Value})  : match.Value;
                if (sameName.Count == 0)
                    return (true, left_, collected.Append(match with { Value = increment }));
                sameName[0].Value.Add(increment);
                return (true, left_, collected);
            }
            return (true, left_, collected.Append(match));
        }

        public static (int, LeafPattern) Command(Leaves left, string command)
        {
            for (var i = 0; i < left.Count; i++)
            {
                if (left[i] is Argument { Value: { } value })
                {
                    if (value.ToString() == command)
                        return (i, new Command(command));
                    break;
                }
            }
            return default;
        }

        public static (int, LeafPattern) Argument(Leaves left, string name)
        {
            for (var i = 0; i < left.Count; i++)
            {
                if (left[i] is Argument { Value: var value })
                    return (i, new Argument(name, value));
            }
            return default;
        }

        public static (int, LeafPattern) Option(Leaves left, string name)
        {
            for (var i = 0; i < left.Count; i++)
            {
                if (left[i].Name == name)
                    return (i, left[i]);
            }
            return default;
        }
    }
}
