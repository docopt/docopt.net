#nullable enable

namespace DocoptNet.Tests.CodeGeneration
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using DocoptNet.CodeGeneration;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;

    [TestFixture]
    public class DocoptArgumentsAttributeTests
    {
        [Test]
        public void FromAttributeData()
        {
            var compilation = CreateCompilation(@"
                using System.Reflection;
                using DocoptNet;
                [DocoptArguments(HelpFile = ""path/to/file.txt"",
                                 HelpConstAccessibility = FieldAttributes.Private,
                                 HelpConstName = ""HelpText"")]
                class Arguments { }");

            var tree = compilation.SyntaxTrees[0];
            var cds = tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().Single();
            var symbol = compilation.GetSemanticModel(tree).GetDeclaredSymbol(cds);
            Assert.That(symbol, Is.Not.Null);
            var data = symbol!.GetAttributes()[0];
            var attribute = DocoptArgumentsAttribute.From(data);
            Assert.That(attribute.HelpFile, Is.EqualTo("path/to/file.txt"));
            Assert.That(attribute.HelpConstName, Is.EqualTo("HelpText"));
            Assert.That(attribute.HelpConstAccessibility, Is.EqualTo(FieldAttributes.Private));
        }

        static int _assemblyUniqueCounter;

        static CSharpCompilation CreateCompilation(string source, [CallerMemberName]string? callerMember = null)
        {
            //_ = new SourceGenerator();

            var references =
                from asm in AppDomain.CurrentDomain.GetAssemblies()
                where !asm.IsDynamic && !string.IsNullOrWhiteSpace(asm.Location)
                select MetadataReference.CreateFromFile(asm.Location);

            var compilation =
                CSharpCompilation.Create(FormattableString.Invariant($"{callerMember}{Interlocked.Increment(ref _assemblyUniqueCounter)}"),
                                         new[] { CSharpSyntaxTree.ParseText(source) },
                                         references,
                                         new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var diagnostics = compilation.GetDiagnostics();

            Assert.False(diagnostics.Any(d => d.Severity == DiagnosticSeverity.Error),
                         "Failed: " + diagnostics.FirstOrDefault());

            return compilation;
        }
    }
}
