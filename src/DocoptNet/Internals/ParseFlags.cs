namespace DocoptNet.Internals
{
    using System;

    [Flags]
    public enum ParseFlags
    {
        None         = 0,
        OptionsFirst = 1 << 0,
        DisableHelp  = 1 << 1,
    }
}
