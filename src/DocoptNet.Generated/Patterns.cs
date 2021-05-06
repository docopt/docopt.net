namespace DocoptNet.Generated
{
    using System.Collections.Immutable;

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
}
