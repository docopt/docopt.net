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
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Loader;
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
            var dict = GetGeneratedProgramArgs(NavalFateUsage, new[] { "ship", "new", "foo", "bar" });

            Assert.That(dict.Count, Is.EqualTo(15));

            Assert.That(Arg("ship", v => (bool?)v), Is.True);
            Assert.That(Arg("new", v => (bool?)v), Is.True);

            var name = Arg("<name>", v => (dynamic)v!);
            Assert.That(name, Is.Not.Null);
            Assert.That((int)name.Count, Is.EqualTo(2));
            Assert.That((string)name[0].Value, Is.EqualTo("foo"));
            Assert.That((string)name[1].Value, Is.EqualTo("bar"));

            Assert.That(Arg("move", v => (bool?)v), Is.False);
            Assert.That(Arg("<x>", v => v), Is.Null);
            Assert.That(Arg("<y>", v => v), Is.Null);
            Assert.That(Arg("--speed", v => (int?)v), Is.EqualTo(10));
            Assert.That(Arg("shoot", v => (bool?)v), Is.False);
            Assert.That(Arg("mine", v => (bool?)v), Is.False);
            Assert.That(Arg("set", v => (bool?)v), Is.False);
            Assert.That(Arg("remove", v => (bool?)v), Is.False);
            Assert.That(Arg("--moored", v => (bool?)v), Is.False);
            Assert.That(Arg("--drifting", v => (bool?)v), Is.False);
            Assert.That(Arg("--help", v => (bool?)v), Is.False);
            Assert.That(Arg("--version", v => (bool?)v), Is.False);

            T Arg<T>(string key, Func<object?, T> selector) =>
                dict.Contains(key)
                ? dict[key] switch
                  {
                      null => throw new NullReferenceException(),
                      {} v => selector((object?)((dynamic)v).Value),
                  }
                : throw new KeyNotFoundException("Key was not present in the dictionary: " + key);
        }

        static IDictionary GetGeneratedProgramArgs(string source, IList<string> argv,
                                              bool help = true, object? version = null,
                                              bool optionsFirst = false, bool exit = false)
        {
            var references =
                from asm in AppDomain.CurrentDomain.GetAssemblies()
                where !asm.IsDynamic && !string.IsNullOrWhiteSpace(asm.Location)
                select MetadataReference.CreateFromFile(asm.Location);

            var compilation =
                CSharpCompilation.Create("test.dll",
                                         new[] { CSharpSyntaxTree.ParseText(source) },
                                         references,
                                         new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            ISourceGenerator generator = new SourceGenerator();

            AdditionalText additionalText = new AdditionalTextString("Program.docopt.txt", source);

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

            using var ms = new MemoryStream();

            var syntaxTrees = from t in outputCompilation.SyntaxTrees
                              where !t.FilePath.EndsWith(".cs", StringComparison.OrdinalIgnoreCase)
                              select t;

            const string main = @"
using System.Collections.Generic;
using DocoptNet.Generated;

public partial class Program
{
    readonly IDictionary<string, ValueObject> _args;

    public Program(IList<string> argv, bool help = true,
                   object version = null, bool optionsFirst = false, bool exit = false)
    {
        _args = Apply(argv, help, version, optionsFirst);
    }

    public IDictionary<string, ValueObject> Args => _args;
}

namespace DocoptNet.Generated
{
    public partial class ValueObject {}
}
";
            var emitResult = outputCompilation.RemoveSyntaxTrees(syntaxTrees)
                                              .AddSyntaxTrees(CSharpSyntaxTree.ParseText(main))
                                              .Emit(ms);

            Assert.False(emitResult.Diagnostics.Any(d => d.Severity == DiagnosticSeverity.Error),
                         "Failed: " + emitResult.Diagnostics.FirstOrDefault());

            ms.Position = 0;

            var assembly = AssemblyLoadContext.Default.LoadFromStream(ms);

            var programType = assembly.GetType("Program")!;
            Assert.That(programType, Is.Not.Null);

            dynamic program = Activator.CreateInstance(programType, argv,
                                                       help, version, optionsFirst, exit)!;
            Assert.That(program, Is.Not.Null);

            var args = program.Args;
            Assert.That(args, Is.Not.Null);
            return (IDictionary)args;
        }
    }
}

#endif
