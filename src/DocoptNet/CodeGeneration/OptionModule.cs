#nullable enable

static class OptionModule
{
    public static (bool HasValue, T Value) Some<T>(T value) => (true, value);
}
