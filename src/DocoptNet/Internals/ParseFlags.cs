// Licensed under terms of MIT license (see LICENSE-MIT)
// Copyright 2021 Atif Aziz, Dinh Doan Van Bien

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
