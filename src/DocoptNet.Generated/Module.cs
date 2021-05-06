namespace DocoptNet.Generated
{
    using System.Linq;
    using Leaves = ReadOnlyList<LeafPattern>;

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
                {
                    match.Value = increment;
                    return (true, left_, collected.Append(match));
                }

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
