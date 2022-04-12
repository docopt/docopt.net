// Licensed under terms of MIT license (see LICENSE-MIT)
// Copyright 2012 Vladimir Keleshev, 2013 Dinh Doan Van Bien, 2021 Atif Aziz

namespace DocoptNet.Internals
{
    sealed class Argument : LeafPattern
    {
        public Argument(string name) : base(name, ArgValue.None) { }
        public Argument(string name, string value) : base(name, value) { }
        public Argument(string name, StringList value) : base(name, value) { }

        public static Argument Unnamed(string value) => new(string.Empty, value);
        public static Argument Unnamed(StringList value) => new(string.Empty, value);
    }
}
