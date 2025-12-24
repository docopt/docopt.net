#nullable enable

namespace DocoptNet.Tests.CodeGeneration
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using RsAnalyzerConfigOptions = Microsoft.CodeAnalysis.Diagnostics.AnalyzerConfigOptions;

    sealed class AnalyzerConfigOptionsProvider(RsAnalyzerConfigOptions globalOptions,
                                               ImmutableDictionary<AdditionalText, RsAnalyzerConfigOptions> optionsByAdditionalText) :
        Microsoft.CodeAnalysis.Diagnostics.AnalyzerConfigOptionsProvider
    {
        public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) =>
            AnalyzerConfigOptions.Empty;

        public override RsAnalyzerConfigOptions GetOptions(AdditionalText textFile) =>
            optionsByAdditionalText.TryGetValue(textFile, out var options) ? options : AnalyzerConfigOptions.Empty;

        public override RsAnalyzerConfigOptions GlobalOptions { get; } = globalOptions;
    }
}
