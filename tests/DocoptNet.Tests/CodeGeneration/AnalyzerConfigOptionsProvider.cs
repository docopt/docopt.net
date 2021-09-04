#nullable enable

namespace DocoptNet.Tests.CodeGeneration
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using RsAnalyzerConfigOptions = Microsoft.CodeAnalysis.Diagnostics.AnalyzerConfigOptions;

    sealed class AnalyzerConfigOptionsProvider :
        Microsoft.CodeAnalysis.Diagnostics.AnalyzerConfigOptionsProvider
    {
        readonly ImmutableDictionary<AdditionalText, RsAnalyzerConfigOptions> _additionalTextOptions;

        public AnalyzerConfigOptionsProvider(params KeyValuePair<AdditionalText, RsAnalyzerConfigOptions>[] options) :
            this(ImmutableDictionary.CreateRange(options)) {}

        public AnalyzerConfigOptionsProvider(ImmutableDictionary<AdditionalText, RsAnalyzerConfigOptions> optionsByAdditionalText) =>
            _additionalTextOptions = optionsByAdditionalText;

        public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) =>
            AnalyzerConfigOptions.Empty;

        public override RsAnalyzerConfigOptions GetOptions(AdditionalText textFile) =>
            _additionalTextOptions.TryGetValue(textFile, out var options) ? options : AnalyzerConfigOptions.Empty;

        public override RsAnalyzerConfigOptions GlobalOptions => AnalyzerConfigOptions.Empty;
    }
}
