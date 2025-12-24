// Licensed under terms of MIT license (see LICENSE-MIT)
// Copyright 2021 Atif Aziz, Dinh Doan Van Bien

namespace DocoptNet.CodeGeneration
{
    using System.Text;
    using Microsoft.CodeAnalysis.Text;

    sealed class StringBuilderSourceText(StringBuilder builder, Encoding? encoding = null) : SourceText
    {
        public override Encoding? Encoding { get; } = encoding;
        public override int Length => builder.Length;
        public override char this[int position] => builder[position];

        public override string ToString(TextSpan span) => builder.ToString(span.Start, span.Length);

        public override void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count) =>
            builder.CopyTo(sourceIndex, destination, destinationIndex, count);
    }
}
