#nullable enable

#region Copyright (c) 2021 Atif Aziz. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

namespace DocoptNet.CodeGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Text;
    using MoreLinq;
    using Argument = DocoptNet.Argument;
    using Unit = System.ValueTuple;
    using static OptionModule;

    [Generator]
    public sealed class SourceGenerator : ISourceGenerator
    {
        static readonly DiagnosticDescriptor SyntaxError =
            new(id: "DCPT0001",
                title: "Syntax error",
                messageFormat: "Syntax error: {0}",
                category: "Docopt",
                DiagnosticSeverity.Error,
                isEnabledByDefault: true);

        static readonly DiagnosticDescriptor MissingHelpConstError =
            new(id: "DCPT0002",
                title: "Missing member",
                messageFormat: "'{0}' is missing the help string constant named '{1}'",
                category: "Docopt",
                DiagnosticSeverity.Error,
                isEnabledByDefault: true);

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
            context.RegisterForPostInitialization(context =>
            {
                foreach (var (fn, source) in GetEmbeddedCSharpSources(fn => DoesFileNameEndIn(fn, "Attribute")))
                    context.AddSource(fn, source);
            });
        }

        sealed class SyntaxReceiver : ISyntaxContextReceiver
        {
            ModelSymbols? _modelSymbols;

            public List<(ClassDeclarationSyntax Class, AttributeData Attributes)> ClassAttributes { get; } = new();

            sealed class ModelSymbols
            {
                public ModelSymbols(SemanticModel model, INamedTypeSymbol attributeType)
                {
                    Model = model;
                    AttributeType = attributeType;
                }

                public SemanticModel Model { get; }
                public INamedTypeSymbol AttributeType { get; }
            }

            ModelSymbols GetModelSymbols(SemanticModel semanticModel)
            {
                if (_modelSymbols is { Model: { } model } someModelSymbols && ReferenceEquals(semanticModel, model))
                    return someModelSymbols;

                const string attributeName = nameof(DocoptNet) + "." + nameof(DocoptArgumentsAttribute);
                var attributeTypeSymbol = semanticModel.Compilation.GetTypeByMetadataName(attributeName);
                return _modelSymbols = new ModelSymbols(semanticModel, attributeTypeSymbol ?? throw new NullReferenceException());
            }

            public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
            {
                var attributeTypeSymbol = GetModelSymbols(context.SemanticModel).AttributeType;
                if (context.Node is AttributeSyntax attribute
                    && SymbolEqualityComparer.Default.Equals(attributeTypeSymbol, context.SemanticModel.GetTypeInfo(attribute).Type)
                    && attribute.FirstAncestorOrSelf((ClassDeclarationSyntax _) => true) is { } cds
                    && context.SemanticModel.GetDeclaredSymbol(cds)?.GetAttributes() is { } attributes)
                {
                    var attributeData = attributes.First(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeTypeSymbol));
                    ClassAttributes.Add((cds, attributeData));
                }
            }
        }

        static class Metadata
        {
            const string Prefix = "build_metadata.AdditionalFiles.";
            public const string SourceItemType = Prefix + nameof(SourceItemType);
            public const string Name = Prefix + nameof(Name);
        }

        static readonly SymbolDisplayFormat FullyQualifiedFormatWithoutGlobalNamespace =
            SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted);

        const string DefaultHelpConstName = "Help";

        public void Execute(GeneratorExecutionContext context)
        {
            context.LaunchDebuggerIfFlagged(nameof(DocoptNet));

            var syntaxReceiver = (SyntaxReceiver)(context.SyntaxContextReceiver ?? throw new NullReferenceException());

            SemanticModel? model = null;

            var docoptTypes = new List<(string? Namespace, string Name, DocoptArgumentsAttribute? ArgumentsAttribute,
                                        SourceText Help, GenerationOptions Options)>();

            foreach (var (cds, attributeData) in syntaxReceiver.ClassAttributes)
            {
                model ??= context.Compilation.GetSemanticModel(cds.SyntaxTree);
                var symbol = model.GetDeclaredSymbol(cds) as INamedTypeSymbol;
                if (symbol is null)
                    continue;
                // TODO emit diagnostics on nested types? symbol.ContainingType != null
                var className = symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
                var namespaceName =
                    symbol.ContainingNamespace.ToDisplayString(FullyQualifiedFormatWithoutGlobalNamespace)
                    is { Length: > 0 } s ? s : null;
                var attribute = DocoptArgumentsAttribute.From(attributeData);
                attribute.HelpConstName ??= DefaultHelpConstName;
                var help = symbol.GetMembers()
                                 .Choose(s => s is IFieldSymbol { IsConst: true, Name: var name, ConstantValue: string help }
                                           && name == attribute.HelpConstName ? Some(help) : default)
                                 .FirstOrDefault();
                if (help is { } someHelp)
                    docoptTypes.Add((namespaceName, className, attribute, SourceText.From(someHelp), GenerationOptions.SkipHelpConst));
                else
                    context.ReportDiagnostic(Diagnostic.Create(MissingHelpConstError, symbol.Locations.First(), symbol, attribute.HelpConstName));
            }

            var globalOptions = context.AnalyzerConfigOptions.GlobalOptions;
            globalOptions.TryGetValue("build_property.RootNamespace", out var rootNamespace);
            globalOptions.TryGetValue("build_property.DocoptNetNamespace", out var embeddingNamespace);

            var docoptSources =
                context.AdditionalFiles
                       .Choose(at => context.AnalyzerConfigOptions.GetOptions(at) is {} options
                                     && options.TryGetValue(Metadata.SourceItemType, out var type)
                                     && "Docopt".Equals(type, StringComparison.OrdinalIgnoreCase)
                                     && at.GetText() is {} text
                                     ? Some((rootNamespace,
                                             options.TryGetValue(Metadata.Name, out var name)
                                             && !string.IsNullOrWhiteSpace(name)
                                                 ? name
                                                 : Path.GetFileName(at.Path).Partition(".").Item1 + "Arguments",
                                             (DocoptArgumentsAttribute?)null,
                                             text,
                                             GenerationOptions.None))
                                     : default)
                       .ToImmutableArray();

            var added = false;

            foreach (var (ns, name, attribute, help, options) in docoptSources.Concat(docoptTypes))
            {
                try
                {
                    if (Generate(ns, embeddingNamespace, name, attribute?.HelpConstName, help, options) is { Length: > 0 } source)
                    {
                        added = true;
                        context.AddSource(name + ".cs", source);
                    }
                }
                catch (DocoptLanguageErrorException e)
                {
                    var args = Regex.Replace(e.Message, @"\r?\n", @"\n");
                    context.ReportDiagnostic(Diagnostic.Create(SyntaxError, Location.None, args));
                }
            }

            if (added)
            {
                var assembly = GetType().Assembly;
                foreach (var (fn, source) in GetEmbeddedCSharpSources(embeddingNamespace, fn => !DoesFileNameEndIn(fn, "Attribute")))
                    context.AddSource(fn, source);
            }
        }

        static readonly SourceText EmptySourceText = SourceText.From(string.Empty);
        static readonly Encoding Utf8BomlessEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        IEnumerable<(string, SourceText)> GetEmbeddedCSharpSources(Func<string, bool> predicate) =>
            GetEmbeddedCSharpSources(null, predicate);

        IEnumerable<(string, SourceText)> GetEmbeddedCSharpSources(string? embeddingNamespace,
                                                                   Func<string, bool> predicate)
        {
            const string resourceNamespace = nameof(DocoptNet) + ".CodeGeneration.Generated.";
            var assembly = GetType().Assembly;
            foreach (var (rn, fn) in from rn in assembly.GetManifestResourceNames()
                                     where rn.StartsWith(resourceNamespace) && rn.EndsWith(".cs")
                                     select (ResourceName: rn, FileName: rn.Substring(resourceNamespace.Length)) into e
                                     where predicate(e.FileName)
                                     select e)
            {
                using var stream = assembly.GetManifestResourceStream(rn)!;
                if (embeddingNamespace is { Length: > 0 } and not nameof(DocoptNet))
                {
                    using var reader = new StreamReader(stream);
                    var source = Regex.Replace(reader.ReadToEnd(), @"(?<=\bnamespace\s+)DocoptNet(?:\.Generated)?\b", embeddingNamespace);
                    yield return (fn, SourceText.From(source, Utf8BomlessEncoding));
                }
                else
                {
                    yield return (fn, SourceText.From(stream, canBeEmbedded: true));
                }
            }
        }

        static bool DoesFileNameEndIn(string fileName, string ending) =>
            Path.GetFileNameWithoutExtension(fileName).EndsWith(ending, StringComparison.OrdinalIgnoreCase);

        static readonly string[] Vars = "abcdefghijklmnopqrstuvwxyz".Select(ch => ch.ToString()).ToArray();

        [Flags]
        enum GenerationOptions
        {
            None,
            SkipHelpConst,
        }

        public static SourceText Generate(string? ns, string? embeddingNamespace, string name,
                                          SourceText text) =>
            Generate(ns, embeddingNamespace, name, null, text, GenerationOptions.None);

        static SourceText Generate(string? ns, string? embeddingNamespace, string name, string? helpConstName,
                                   SourceText text, GenerationOptions generationOptions) =>
            Generate(ns, embeddingNamespace, name, helpConstName, text, null, generationOptions);

        public static SourceText Generate(string? ns, string? embeddingNamespace, string name,
                                          SourceText text, Encoding? outputEncoding) =>
            Generate(ns, embeddingNamespace, name, null, text, outputEncoding, GenerationOptions.None);

        static SourceText Generate(string? ns, string? embeddingNamespace, string name, string? helpConstName,
                                   SourceText text, Encoding? outputEncoding, GenerationOptions options)
        {
            if (text.Length == 0)
                return EmptySourceText;

            var helpText = text.ToString();
            var code = new CSharpSourceBuilder();

            Generate(code,
                     ns is { Length: 0 } ? null : ns,
                     embeddingNamespace is { Length: > 0 } ? embeddingNamespace : nameof(DocoptNet),
                     name, helpConstName ?? DefaultHelpConstName, helpText,
                     options);

            return new StringBuilderSourceText(code.StringBuilder, outputEncoding ?? text.Encoding ?? Utf8BomlessEncoding);
        }

        static void Generate(CSharpSourceBuilder code,
                             string? ns,
                             string embeddingNamespace,
                             string name,
                             string helpConstName,
                             string helpText,
                             GenerationOptions generationOptions)
        {
            var (pattern, options, usage) = Docopt.ParsePattern(helpText);

            var leaves = Docopt.GetFlatPatterns(helpText)
                               .GroupBy(p => p.Name)
                               .Select(g => (LeafPattern)g.First())
                               .ToList();

            const string usageConstName = "Usage";

            code["#nullable enable annotations"].NewLine

                .NewLine
                .Using("System.Collections")
                .Using("System.Collections.Generic")
                .Using("System.Linq")
                .Using(embeddingNamespace)
                .UsingAlias("Leaves")[code[embeddingNamespace][".ReadOnlyList<"][embeddingNamespace][".LeafPattern>"]]
                .UsingStatic[code[embeddingNamespace][".GeneratedSourceModule"]]

                .NewLine
                [ns is not null ? code.Namespace(ns) : code.Blank()]

                .Partial.Class[name][" : IEnumerable<KeyValuePair<string, object?>>"].NewLine.SkipNextNewLine.Block[code
                    [(generationOptions & GenerationOptions.SkipHelpConst) == GenerationOptions.SkipHelpConst
                         ? code.Blank()
                         : code.NewLine.Public.Const(helpConstName, helpText)]

                    .NewLine
                    .Public.Const(usageConstName, usage)

                    .NewLine
                    .Public.Static[name][" Apply(IEnumerable<string> args, bool help = true, object? version = null, bool optionsFirst = false)"]
                    .NewLine.Block[code
                        .Var("options")[
                            code.New["List<Option>"].NewLine
                                .Block[code.Each(options,
                                                 static (code, option, _) =>
                                                     code.New["Option("][option.ShortName is {} sn ? code.Literal(sn) : code.Null][", "]
                                                         [option.LongName is {} ln ? code.Literal(ln) : code.Null][", "]
                                                         .Literal(option.ArgCount)[", "]
                                                         [Value(code, option.Value)]["),"].NewLine).SkipNextNewLine]]
                        .Var("left")["ParseArgv(" + helpConstName + ", args, options, optionsFirst, help, version)"]
                        .Var("required")[code.New["RequiredMatcher(1, left, "].New["Leaves()"][')']]
                        ["Match(ref required)"].EndStatement
                        .Var("collected")["GetSuccessfulCollection(required, " + usageConstName + ")"]
                        .Var("result")[code.New[name]["()"]]
                        [leaves.Any()
                             ? code.NewLine
                                   .ForEach["var leaf in collected"][
                                        code.Var("value")["leaf.Value is { IsStringList: true } ? ((StringList)leaf.Value).Reverse() : leaf.Value"]
                                            .Switch["leaf.Name"]
                                            .Cases(leaves, default(Unit),
                                                   static (_, leaf, _) => CSharpSourceBuilder.SwitchCaseChoice.Choose(leaf.Name),
                                                   static (code, _, leaf) =>
                                                       code[' ']
                                                          .Assign(code["result."][InferPropertyName(leaf)])[
                                                               code['(']
                                                                   [leaf switch { Option   { Value: { IsString: true } } => "string",
                                                                                  Argument { Value: { IsNone: true } } or Option { ArgCount: not 0, Value: { Kind: not ValueKind.StringList } } => "string?",
                                                                                  { Value: { Kind: var kind } } => MapType(kind) }]
                                                                   [")value"].SkipNextNewLine][' '])]
                             : code.Blank()
                        ]

                        .NewLine
                        .Return["result"]

                        .NewLine
                        ["static void Match(ref RequiredMatcher required)"].NewLine.Block[
                            GeneratePatternMatchingCode(code, pattern, "required")]
                    ] // Apply

                    .NewLine
                    ["IEnumerator<KeyValuePair<string, object?>> GetEnumerator()"].NewLine.Block[
                        leaves.Any() ? code.Each(leaves, static (code, p, _) => code.Yield.Return[code["KeyValuePair.Create("].Literal(p.Name)[", (object?)"][InferPropertyName(p)][')']])
                                     : code.Yield.Break]

                    .NewLine
                    ["IEnumerator<KeyValuePair<string, object?>> IEnumerable<KeyValuePair<string, object?>>.GetEnumerator() => GetEnumerator()"].EndStatement
                    ["IEnumerator IEnumerable.GetEnumerator() => GetEnumerator()"].EndStatement

                    .Each(from p in leaves
                          select (Name: InferPropertyName(p), Leaf: p),
                          static (code, e, _) =>
                              code.NewLine["/// <summary><c>"][e.Leaf.ToString().EncodeXmlText()]["</c></summary>"]
                                  .NewLine
                                  .Public[e.Leaf switch
                                   {
                                       Option { Value: { IsString: true } str } => code["string "][e.Name][" { get; private set; } = "].Literal((string)str).SkipNextNewLine.EndStatement,
                                       Argument { Value: { IsNone: true } } or Option { ArgCount: not 0, Value: { Kind: not ValueKind.StringList } } => code["string? "][e.Name][" { get; private set; }"],
                                       { Value: { Object: StringList list } } => code["StringList "][e.Name][" { get; private set; } = "][Value(code, list.Reverse())].SkipNextNewLine.EndStatement,
                                       { Value: { Kind: var kind } } => code[MapType(kind)][' '][e.Name][" { get; private set; }"],
                                   }]
                                  .NewLine)
                ] // class
                [ns is not null ? code.BlockEnd : code.Blank()]
                .Blank();

            static CSharpSourceBuilder GeneratePatternMatchingCode(CSharpSourceBuilder code,
                                                                   Pattern pattern, string pmv, int level = 0)
            {
                if (level >= 26) // todo proper diagnostics reporting
                    throw new NotSupportedException();

                code.LineComment(pattern?.ToString() ?? string.Empty);

                switch (pattern)
                {
                    case BranchPattern { Children: { } children }:
                    {
                        var matcher = pattern switch
                        {
                            Either => nameof(EitherMatcher),
                            OneOrMore => nameof(OneOrMoreMatcher),
                            Optional => nameof(OptionalMatcher),
                            Required => nameof(RequiredMatcher),
                            _ => throw new ArgumentOutOfRangeException(nameof(pattern))
                        };
                        var mv = Vars[level++];
                        code.Var(mv)[code.New[matcher]['('].Literal(children.Count)[", "][pmv][".Left, "][pmv][".Collected)"]]
                            .While[code[mv][".Next()"]][
                                (pattern.Children.Count switch
                                {
                                    > 1 => code.Switch[code[mv][".Index"]]
                                               .Cases(children, arg: (MatchVar: mv, Level: level),
                                                      static (_, _, i) => CSharpSourceBuilder.SwitchCaseChoice.Choose(i),
                                                      static (code, arg, child) =>
                                                          code.NewLine.Block[GeneratePatternMatchingCode(code, child, arg.MatchVar, arg.Level)]),
                                    1 => GeneratePatternMatchingCode(code, children[0], mv, level),
                                    _ => code.Blank(),
                                })
                                .If[code['!'][mv][".LastMatched"]][
                                    code.Break]]
                            [pmv][".Fold(" + mv + ".Result)"].EndStatement.Blank();
                        break;
                    }
                    case LeafPattern { Name: var name } leaf:
                    {
                        var lfn = leaf switch
                        {
                            Command  => nameof(PatternMatcher.MatchCommand),
                            Argument => nameof(PatternMatcher.MatchArgument),
                            Option   => nameof(PatternMatcher.MatchOption),
                            _ => throw new NotImplementedException()
                        };
                        code[pmv][".Match(" + "PatternMatcher."][lfn][", "].Literal(name)[", ValueKind."][leaf.Value.Kind.ToString()][')'].EndStatement.Blank();
                        break;
                    }
                }

                return code;
            }

            static string InferPropertyName(LeafPattern leaf) =>
                leaf switch
                {
                    Command  { Name: var name } => $"Cmd{GenerateCodeHelper.ConvertToPascalCase(name.ToLowerInvariant())}",
                    Argument { Name: var name } => $"Arg{GenerateCodeHelper.ConvertToPascalCase(name.Replace("<", "").Replace(">", "").ToLowerInvariant())}",
                    Option   { Name: var name } => $"Opt{GenerateCodeHelper.ConvertToPascalCase(name.ToLowerInvariant())}",
                    var p => throw new NotSupportedException($"Unsupported pattern: {p}")
                };

            static string MapType(ValueKind kind) =>
                kind switch
                {
                    ValueKind.Boolean    => "bool",
                    ValueKind.Integer    => "int",
                    ValueKind.String     => "string?",
                    ValueKind.StringList => nameof(StringList),
                    _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null)
                };

            static CSharpSourceBuilder Value(CSharpSourceBuilder code, Value value) =>
                value.TryAsStringList(out var items) && items.Count > 0
                ? code["StringList.TopBottom("].Each(items, static (code, item, i) => (i > 0 ? code[", "] : code.Blank()).Literal(item))[')']
                : value.Object switch
                  {
                      null => code.Null,
                      int n => code.Literal(n),
                      string s => code.Literal(s),
                      true => code.True,
                      false => code.False,
                      StringList { IsEmpty: true } => code["StringList.Empty"],
                      _ => throw new NotSupportedException(), // todo emit diagnostic
                  };
        }
    }
}
