// Licensed under terms of MIT license (see LICENSE-MIT)
// Copyright 2012 Vladimir Keleshev, 2013 Dinh Doan Van Bien, 2021 Atif Aziz

namespace DocoptNet.Internals
{
    sealed class Command(string name, bool value = false)
        : LeafPattern(name, value ? ArgValue.True : ArgValue.False);
}
