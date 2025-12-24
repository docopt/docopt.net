#nullable enable

namespace DocoptNet.Tests.CodeGeneration
{
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Text;

    class AdditionalTextSource(string path, SourceText text) : AdditionalText
    {
        public override SourceText? GetText(CancellationToken cancellationToken = default) => text;

        public override string Path { get; } = path;
    }

    sealed class AdditionalTextString(string path, string text) :
        AdditionalTextSource(path, SourceText.From(text));
}
