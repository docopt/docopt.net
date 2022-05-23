// Licensed under terms of MIT license (see LICENSE-MIT)
// Copyright 2021 Atif Aziz, Dinh Doan Van Bien

static class OptionModule
{
    public static (bool HasValue, T Value) Some<T>(T value) => (true, value);
}
