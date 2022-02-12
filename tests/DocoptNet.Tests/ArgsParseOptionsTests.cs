#nullable enable

namespace DocoptNet.Tests;

using NUnit.Framework;

[TestFixture]
public class ArgsParseOptionsTests
{
    [Test]
    public void Default_OptionsFirst_is_false()
    {
        Assert.That(ArgsParseOptions.Default.OptionsFirst, Is.False);
    }

    [Test]
    public void OptionsFirst_with_same_value_returns_same_object()
    {
        Assert.That(ArgsParseOptions.Default.WithOptionsFirst(false),
                    Is.SameAs(ArgsParseOptions.Default));
    }

    [Test]
    public void OptionsFirst_with_true_returns_new_options_with_new_value()
    {
        var updated = ArgsParseOptions.Default.WithOptionsFirst(true);
        Assert.That(updated.OptionsFirst, Is.True);
    }

    [Test]
    public void OptionsFirst_with_true_returns_same_object()
    {
        var @default = ArgsParseOptions.Default;
        var updated1 = @default.WithOptionsFirst(true);
        var updated2 = @default.WithOptionsFirst(true);
        Assert.That(updated1, Is.SameAs(updated2));
    }
}
