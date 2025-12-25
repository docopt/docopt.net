// Licensed under terms of MIT license (see LICENSE-MIT)
// Copyright 2021 Atif Aziz, Dinh Doan Van Bien

namespace DocoptNet
{
    static class Extensions
    {
        /// <summary>
        /// Split the <paramref name="input"/> at the first occurrence of <paramref name="separator"/>,
        /// and stores the part before the separator, the separator itself, and the part after the
        /// separator. If the separator is not found, stores the string itself, and two empty strings.
        /// </summary>

        public static (string, string, string) Partition(this string input, string separator)
        {
            return input.IndexOf(separator, System.StringComparison.Ordinal) is var i and >= 0
                 ? (input[..i], separator, input[(i + separator.Length)..])
                 : (input, string.Empty, string.Empty);
        }
    }
}
