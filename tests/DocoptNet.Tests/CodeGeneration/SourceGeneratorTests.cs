#nullable enable

namespace DocoptNet.Tests.CodeGeneration
{
    using DocoptNet.CodeGeneration;
    using Microsoft.CodeAnalysis.Text;
    using NUnit.Framework;

    [TestFixture]
    public partial class SourceGeneratorTests
    {
        const string NavalFateUsage = @"
Naval Fate.

    Usage:
      naval_fate.exe ship new <name>...
      naval_fate.exe ship <name> move <x> <y> [--speed=<kn>]
      naval_fate.exe ship shoot <x> <y>
      naval_fate.exe mine (set|remove) <x> <y> [--moored | --drifting]
      naval_fate.exe (-h | --help)
      naval_fate.exe --version

    Options:
      -h --help     Show this screen.
      --version     Show version.
      --speed=<kn>  Speed in knots [default: 10].
      --moored      Moored (anchored) mine.
      --drifting    Drifting mine.
";

        [Test]
        public void Generate()
        {
            var source = SourceGenerator.Generate("NavalFate", "Program", SourceText.From(NavalFateUsage)).ToString();
            Assert.That(source, Is.Not.Empty);
        }
    }
}

#if SOURCE_GENERATION

namespace DocoptNet.Tests.CodeGeneration
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using DocoptNet.CodeGeneration;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;
    using RsAnalyzerConfigOptions = Microsoft.CodeAnalysis.Diagnostics.AnalyzerConfigOptions;

    [TestFixture]
    public partial class SourceGeneratorTests
    {
        [Test]
        public void GenerateViaDriver()
        {
            var source = GetGeneratedOutput(NavalFateUsage);
            Assert.That(source, Is.Not.Empty);
        }

        static string GetGeneratedOutput(string source)
        {
            var references =
                from assembly in AppDomain.CurrentDomain.GetAssemblies()
                where !assembly.IsDynamic && !string.IsNullOrWhiteSpace(assembly.Location)
                select MetadataReference.CreateFromFile(assembly.Location);

            var compilation =
                CSharpCompilation.Create("test.dll",
                                         new[] { CSharpSyntaxTree.ParseText(source) },
                                         references,
                                         new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            ISourceGenerator generator = new SourceGenerator();

            AdditionalText additionalText = new AdditionalTextString("Usage.docopt.txt", source);

            RsAnalyzerConfigOptions options =
                new AnalyzerConfigOptions(
                    KeyValuePair.Create("build_metadata.AdditionalFiles.SourceItemType", "Docopt"));

            var driver =
                CSharpGeneratorDriver.Create(new[] { generator },
                                             new[] { additionalText },
                                             parseOptions: CSharpParseOptions.Default.WithPreprocessorSymbols("DOCNETOPT_GENERATED"),
                                             new AnalyzerConfigOptionsProvider(KeyValuePair.Create(additionalText, options)));

            driver.RunGeneratorsAndUpdateCompilation(compilation,
                                                     out var outputCompilation,
                                                     out var generateDiagnostics);

            Assert.False(generateDiagnostics.Any(d => d.Severity == DiagnosticSeverity.Error),
                         "Failed: " + generateDiagnostics.FirstOrDefault()?.GetMessage());

            return outputCompilation.SyntaxTrees
                                    .Single(t => "Usage.cs".Equals(Path.GetFileName(t.FilePath), StringComparison.OrdinalIgnoreCase))
                                    .ToString();
        }

    }
}

#endif
