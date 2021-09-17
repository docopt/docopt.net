#nullable enable

namespace DocoptNet.Tests.CodeGeneration
{
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Text;

    class AdditionalTextSource : AdditionalText
    {
        readonly SourceText _text;

        public AdditionalTextSource(string path, SourceText text)
        {
            Path = path;
            _text = text;
        }

        public override SourceText? GetText(CancellationToken cancellationToken = default) =>
            _text;

        public override string Path { get; }
    }

    sealed class AdditionalTextString : AdditionalTextSource
    {
        public AdditionalTextString(string path, string text) :
            base(path, SourceText.From(text)) { }
    }
}
