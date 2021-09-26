#nullable enable

namespace DocoptNet.Tests.CodeGeneration
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using RsAnalyzerConfigOptions = Microsoft.CodeAnalysis.Diagnostics.AnalyzerConfigOptions;

    sealed class AnalyzerConfigOptionsProvider :
        Microsoft.CodeAnalysis.Diagnostics.AnalyzerConfigOptionsProvider
    {
        readonly ImmutableDictionary<AdditionalText, RsAnalyzerConfigOptions> _additionalTextOptions;

        public AnalyzerConfigOptionsProvider(RsAnalyzerConfigOptions globalOptions,
                                             ImmutableDictionary<AdditionalText, RsAnalyzerConfigOptions> optionsByAdditionalText)
        {
            GlobalOptions = globalOptions;
            _additionalTextOptions = optionsByAdditionalText;
        }

        public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) =>
            AnalyzerConfigOptions.Empty;

        public override RsAnalyzerConfigOptions GetOptions(AdditionalText textFile) =>
            _additionalTextOptions.TryGetValue(textFile, out var options) ? options : AnalyzerConfigOptions.Empty;

        public override RsAnalyzerConfigOptions GlobalOptions { get; }
    }
}
