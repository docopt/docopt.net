#nullable enable

namespace DocoptNet.Tests.CodeGeneration
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    sealed class AnalyzerConfigOptions(ImmutableDictionary<string, string> options) :
        Microsoft.CodeAnalysis.Diagnostics.AnalyzerConfigOptions
    {
        public static readonly AnalyzerConfigOptions Empty = new(ImmutableDictionary<string, string>.Empty);

        public AnalyzerConfigOptions(params KeyValuePair<string, string>[] options) :
            this(options.AsEnumerable()) { }

        public AnalyzerConfigOptions(IEnumerable<KeyValuePair<string, string>> options) :
            this(ImmutableDictionary.CreateRange(options)) { }

        public override bool TryGetValue(string key, [NotNullWhen(true)] out string? value) =>
            options.TryGetValue(key, out value);
    }
}
