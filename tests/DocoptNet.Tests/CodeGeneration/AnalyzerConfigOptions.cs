#if SOURCE_GENERATION
#nullable enable

namespace DocoptNet.Tests.CodeGeneration
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Diagnostics.CodeAnalysis;

    sealed class AnalyzerConfigOptions : Microsoft.CodeAnalysis.Diagnostics.AnalyzerConfigOptions
    {
        public static readonly AnalyzerConfigOptions Empty = new(ImmutableDictionary<string, string>.Empty);

        readonly ImmutableDictionary<string, string> _options;

        public AnalyzerConfigOptions(params KeyValuePair<string, string>[] options) :
            this(ImmutableDictionary.CreateRange(options)) {}

        public AnalyzerConfigOptions(ImmutableDictionary<string, string> options) =>
            _options = options;

        public override bool TryGetValue(string key, [NotNullWhen(true)] out string? value) =>
            _options.TryGetValue(key, out value);
    }
}
#endif
