namespace DocoptNet.Tests
{
    using Internals;

    static class PatternFactory
    {
        public static ReadOnlyList<LeafPattern> Leaves() => new ReadOnlyList<LeafPattern>();
        public static ReadOnlyList<LeafPattern> Leaves(params LeafPattern[] leaves) => leaves.AsReadOnly();
    }
}
