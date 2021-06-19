#nullable enable

namespace DocoptNet.Tests.CodeGeneration
{
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Text;

    sealed class AdditionalTextString : AdditionalText
    {
        readonly SourceText _text;

        public AdditionalTextString(string path, string text)
        {
            Path = path;
            _text = SourceText.From(text);
        }

        public override SourceText? GetText(CancellationToken cancellationToken = default) =>
            _text;

        public override string Path { get; }
    }
}
