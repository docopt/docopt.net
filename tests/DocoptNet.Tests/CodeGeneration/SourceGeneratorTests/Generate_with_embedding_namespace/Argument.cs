namespace DocoptNet.Generated
{
    using System.Globalization;

    class Argument: LeafPattern
    {
        public Argument(string name) : base(name, Value.None)
        {
        }

        public Argument(string name, string value)
            : base(name, value)
        {
        }

        public Argument(string name, string[] values)
            : base(name, StringList.BottomTop(values))
        {
        }

        /// <remarks>
        /// This is only used by tests as a convenience. The instantiated
        /// <see cref="Value"/> is a string representation of the integer.
        /// </remarks>

        public Argument(string name, int value)
            : this(name, value.ToString(CultureInfo.InvariantCulture))
        {
        }
    }
}
