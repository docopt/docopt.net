namespace DocoptNet.Tests
{
    using System.Globalization;

    static class ArgumentFactory
    {
        public static Argument Argument(string name, string value) => new(name, value);
        public static Argument Argument(string name, int value) => new(name, value.ToString(CultureInfo.InvariantCulture));
        public static Argument Argument(string name, StringList value) => new(name, value);
        public static Argument Argument(string value) => Internals.Argument.Unnamed(value);
        public static Argument Argument(string[] values) => Internals.Argument.Unnamed(StringList.BottomTop(values));
        public static Argument Argument(int value) => Argument(value.ToString(CultureInfo.InvariantCulture));
    }
}
