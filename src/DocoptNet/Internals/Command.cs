// Licensed under terms of MIT license (see LICENSE-MIT)
// Copyright 2012 Vladimir Keleshev, 2013 Dinh Doan Van Bien, 2021 Atif Aziz

namespace DocoptNet.Internals
{
    class Command : LeafPattern
    {
        public Command(string name, bool value = false) :
            base(name, value ? ArgValue.True : ArgValue.False) { }
    }
}
