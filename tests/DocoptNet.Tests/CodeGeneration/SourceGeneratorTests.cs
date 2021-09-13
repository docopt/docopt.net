#nullable enable

namespace DocoptNet.Tests.CodeGeneration
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Loader;
    using System.Threading;
    using DocoptNet.CodeGeneration;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Text;
    using NUnit.Framework;
    using RsAnalyzerConfigOptions = Microsoft.CodeAnalysis.Diagnostics.AnalyzerConfigOptions;

    [TestFixture]
    public class SourceGeneratorTests
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
            var source = SourceGenerator.Generate("NavalFate", null, "Program", SourceText.From(NavalFateUsage)).ToString();
            Assert.That(source, Is.Not.Empty);
        }

        Program? _generatedProgram;

        Program GeneratedProgram
        {
            get { return _generatedProgram ?? throw new InvalidOperationException(); }
            set { _generatedProgram = value; }
        }

        [OneTimeSetUp]
        public void SetUp()
        {
            _generatedProgram = GenerateProgram(NavalFateUsage);
        }

        [Test]
        public void Generate_via_driver()
        {
            var args = GeneratedProgram.Run(args => new NavalFateArgs(args), "ship", "new", "foo", "bar");

            Assert.That(args.Count, Is.EqualTo(15));

            Assert.That(args.CmdShip, Is.True);
            Assert.That(args.CmdNew, Is.True);

            var name = args.ArgName!.Cast<string>().ToList();
            Assert.That(name, Is.Not.Null);
            Assert.That(name.Count, Is.EqualTo(2));
            Assert.That(name[0], Is.EqualTo("foo"));
            Assert.That(name[1], Is.EqualTo("bar"));

            Assert.That(args.CmdMove, Is.False);
            Assert.That(args.ArgX, Is.Null);
            Assert.That(args.ArgY, Is.Null);
            Assert.That(args.OptSpeed, Is.EqualTo("10"));
            Assert.That(args.CmdShoot, Is.False);
            Assert.That(args.CmdMine, Is.False);
            Assert.That(args.CmdSet, Is.False);
            Assert.That(args.CmdRemove, Is.False);
            Assert.That(args.OptMoored, Is.False);
            Assert.That(args.OptDrifting, Is.False);
            Assert.That(args.OptHelp, Is.False);
            Assert.That(args.OptVersion, Is.False);
        }

        sealed record DocoptOptions(bool Help = true,
                                    object? Version = null,
                                    bool OptionsFirst = false,
                                    bool Exit = false)
        {
            public static readonly DocoptOptions Default = new();
        }

        class ProgramArgs
        {
            readonly IDictionary _args;

            public ProgramArgs(IDictionary args) =>
                _args = args ?? throw new ArgumentNullException("args");

            public int Count => _args.Count;

            public T Get<T>(string key, Func<object?, T> selector) =>
                _args.Contains(key)
                ? selector(_args[key])
                : throw new KeyNotFoundException("Key was not present in the dictionary: " + key);
        }

        sealed class NavalFateArgs : ProgramArgs
        {
            public NavalFateArgs(IDictionary args) : base(args) {}

            public bool?        CmdShip        => Get("ship", v => (bool?)v);
            public bool?        CmdNew         => Get("new", v => (bool?)v);
            public IEnumerable? ArgName        => Get("<name>", v => (IEnumerable?)v);
            public bool?        CmdMove        => Get("move", v => (bool?)v);
            public int?         ArgX           => Get("<x>", v => (int?)v);
            public int?         ArgY           => Get("<y>", v => (int?)v);
            public string?      OptSpeed       => Get("--speed", v => (string?)v);
            public bool?        CmdShoot       => Get("shoot", v => (bool?)v);
            public bool?        CmdMine        => Get("mine", v => (bool?)v);
            public bool?        CmdSet         => Get("set", v => (bool?)v);
            public bool?        CmdRemove      => Get("remove", v => (bool?)v);
            public bool?        OptMoored      => Get("--moored", v => (bool?)v);
            public bool?        OptDrifting    => Get("--drifting", v => (bool?)v);
            public bool?        OptHelp        => Get("--help", v => (bool?)v);
            public bool?        OptVersion     => Get("--version", v => (bool?)v);
        }

        sealed class Program
        {
            readonly Type _type;

            public Program(Type type) => _type = type ?? throw new ArgumentNullException(nameof(type));

            public ProgramArgs Run(params string[] argv) =>
                Run(DocoptOptions.Default, argv);

            public ProgramArgs Run(DocoptOptions options, params string[] argv) =>
                Run(options, args => new ProgramArgs(args), argv);

            public T Run<T>(Func<IDictionary, T> selector, params string[] argv) =>
                Run<T>(DocoptOptions.Default, selector, argv);

            public T Run<T>(DocoptOptions options, Func<IDictionary, T> selector, params string[] argv)
            {
                const string applyMethodName = "Apply";
                var method = _type.GetMethod(applyMethodName, BindingFlags.Public | BindingFlags.Static);
                if (method == null)
                    throw new MissingMethodException(_type.Name, applyMethodName);
                var args = (IEnumerable<KeyValuePair<string, object?>>)
                    method.Invoke(null, new[]
                    {
                        argv,
                        options.Help, options.Version,
                        options.OptionsFirst
                    })!;
                Assert.That(args, Is.Not.Null);
                return selector(args.ToDictionary(e => e.Key, e => e.Value));
            }
        }

        static Program GenerateProgram(string usage)
        {
            const string main = @"
using System.Collections.Generic;
using DocoptNet;

public partial class " + nameof(Program) + @"Arguments { }

namespace DocoptNet
{
    public partial class StringList {}
}
";

            var assembly = GenerateProgram(usage, main);
            return new Program(assembly.GetType(nameof(Program) + "Arguments")!);
        }

        static int _assemblyUniqueCounter;

        internal static Assembly GenerateProgram(string usage, string source)
        {
            var references =
                from asm in AppDomain.CurrentDomain.GetAssemblies()
                where !asm.IsDynamic && !string.IsNullOrWhiteSpace(asm.Location)
                select MetadataReference.CreateFromFile(asm.Location);

            var compilation =
                CSharpCompilation.Create(FormattableString.Invariant($"test{Interlocked.Increment(ref _assemblyUniqueCounter)}"),
                                         new[] { CSharpSyntaxTree.ParseText(source) },
                                         references,
                                         new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            ISourceGenerator generator = new SourceGenerator();

            AdditionalText additionalText = new AdditionalTextString("Program.docopt.txt", usage);

            RsAnalyzerConfigOptions options =
                new AnalyzerConfigOptions(
                    KeyValuePair.Create("build_metadata.AdditionalFiles.SourceItemType", "Docopt"));

            var driver =
                CSharpGeneratorDriver.Create(new[] { generator },
                                             new[] { additionalText },
                                             optionsProvider: new AnalyzerConfigOptionsProvider(KeyValuePair.Create(additionalText, options)));

            driver.RunGeneratorsAndUpdateCompilation(compilation,
                                                     out var outputCompilation,
                                                     out var generateDiagnostics);

            Assert.False(generateDiagnostics.Any(d => d.Severity == DiagnosticSeverity.Error),
                         "Failed: " + generateDiagnostics.FirstOrDefault()?.GetMessage());

            using var ms = new MemoryStream();
            var emitResult = outputCompilation.Emit(ms);

            Assert.False(emitResult.Diagnostics.Any(d => d.Severity == DiagnosticSeverity.Error),
                         "Failed: " + emitResult.Diagnostics.FirstOrDefault());

            ms.Position = 0;

            return AssemblyLoadContext.Default.LoadFromStream(ms);
        }
    }
}
