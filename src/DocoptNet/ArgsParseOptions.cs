#nullable enable

namespace DocoptNet
{
    sealed partial class ArgsParseOptions
    {
        public static readonly ArgsParseOptions Default = new(false);

        ArgsParseOptions(bool optionFirst) => OptionsFirst = optionFirst;

        public bool OptionsFirst { get; }

        static readonly ArgsParseOptions OptionsFirstTrue = new(true);

        public ArgsParseOptions WithOptionsFirst(bool value) =>
            OptionsFirst == value ? this : value ? OptionsFirstTrue : Default;
    }
}
