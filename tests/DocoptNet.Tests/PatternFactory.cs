namespace DocoptNet.Tests
{
    static class PatternFactory
    {
        static readonly LeafPattern[] ZeroLeaves = new LeafPattern[0];

        public static LeafPattern[] Leaves() => ZeroLeaves;
        public static LeafPattern[] Leaves(params LeafPattern[] leaves) => leaves;
    }
}
