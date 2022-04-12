// Licensed under terms of MIT license (see LICENSE-MIT)
// Copyright 2021 Atif Aziz, Dinh Doan Van Bien

namespace DocoptNet.CodeGeneration
{
    using System.Text;
    using Microsoft.CodeAnalysis.Text;

    sealed class StringBuilderSourceText : SourceText
    {
        readonly StringBuilder _builder;

        public StringBuilderSourceText(StringBuilder builder, Encoding? encoding = null) =>
            (_builder, Encoding) = (builder, encoding);

        public override Encoding? Encoding { get; }
        public override int Length => _builder.Length;
        public override char this[int position] => _builder[position];

        public override string ToString(TextSpan span) => _builder.ToString(span.Start, span.Length);

        public override void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count) =>
            _builder.CopyTo(sourceIndex, destination, destinationIndex, count);
    }
}
