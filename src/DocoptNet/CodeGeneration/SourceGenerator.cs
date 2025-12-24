// Licensed under terms of MIT license (see LICENSE-MIT)
// Copyright 2021 Atif Aziz, Dinh Doan Van Bien

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
    using Unit = System.ValueTuple;
    using static OptionModule;

    [Generator]
    public sealed class SourceGenerator : ISourceGenerator
    {
        static readonly DiagnosticDescriptor SyntaxError =
            new DiagnosticDescriptor(id: "DCPT0001",
                title: "Syntax error",
                messageFormat: "Syntax error: {0}",
                category: "Docopt",
                DiagnosticSeverity.Error,
                isEnabledByDefault: true);

        static readonly DiagnosticDescriptor MissingHelpConstError =
            new DiagnosticDescriptor(id: "DCPT0002",
                title: "Missing member",
                messageFormat: "'{0}' is missing the help string constant named '{1}'",
                category: "Docopt",
                DiagnosticSeverity.Error,
                isEnabledByDefault: true);

        public void Initialize(GeneratorInitializationContext context) =>
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());

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
            SyntaxTree? modelSyntaxTree = null;

            var docoptTypes = new List<(string? Namespace, string Name,
                                        IEnumerable<TypeDeclarationSyntax> Parents,
                                        DocoptArgumentsAttribute? ArgumentsAttribute,
                                        SourceText Help, GenerationOptions Options)>();

            foreach (var (cds, attributeData) in syntaxReceiver.ClassAttributes)
            {
                if (model is null || modelSyntaxTree != cds.SyntaxTree)
                {
                    model = context.Compilation.GetSemanticModel(cds.SyntaxTree);
                    modelSyntaxTree = cds.SyntaxTree;
                }

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
                    docoptTypes.Add((namespaceName, className, cds.GetParents().Where(tds => tds is ClassDeclarationSyntax).Reverse(), attribute, SourceText.From(someHelp), GenerationOptions.SkipHelpConst));
                else
                    context.ReportDiagnostic(Diagnostic.Create(MissingHelpConstError, symbol.Locations.First(), symbol, attribute.HelpConstName));
            }

            var globalOptions = context.AnalyzerConfigOptions.GlobalOptions;
            globalOptions.TryGetValue("build_property.RootNamespace", out var rootNamespace);

            var docoptSources =
                context.AdditionalFiles
                       .Choose(at => context.AnalyzerConfigOptions.GetOptions(at) is var options
                                     && options.TryGetValue(Metadata.SourceItemType, out var type)
                                     && "Docopt".Equals(type, StringComparison.OrdinalIgnoreCase)
                                     && at.GetText() is { } text
                                     ? Some((rootNamespace,
                                             options.TryGetValue(Metadata.Name, out var name)
                                             && !string.IsNullOrWhiteSpace(name)
                                                 ? name
                                                 : Path.GetFileName(at.Path).Partition(".").Item1 + "Arguments",
                                             Enumerable.Empty<TypeDeclarationSyntax>(),
                                             (DocoptArgumentsAttribute?)null,
                                             text,
                                             GenerationOptions.None))
                                     : default)
                       .ToImmutableArray();

            var hintNameBuilder = new StringBuilder();

            foreach (var (ns, name, parents, attribute, help, options) in docoptSources.Concat(docoptTypes))
            {
                try
                {
                    var parentNames = parents.Select(p => p.Identifier.ToString()).ToArray();
                    if (Generate(ns, name, parentNames, attribute?.HelpConstName, help, options) is { Length: > 0 } source)
                    {
                        hintNameBuilder.Clear();
                        if (ns is { } someNamespace)
                            hintNameBuilder.Append(someNamespace).Append('.');
                        if (parentNames.Length > 0)
                        {
                            foreach (var pn in parentNames)
                            {
                                // NOTE! Microsoft.CodeAnalysis.CSharp 3.10 does not allow use of "+"
                                // as is conventional for nested types. It is allowed later versions;
                                // see: https://github.com/dotnet/roslyn/issues/58476
                                hintNameBuilder.Append(pn).Append('-');
                            }
                        }
                        hintNameBuilder.Append(name);
                        context.AddSource(hintNameBuilder.Append(".cs").ToString(), source);
                    }
                }
                catch (DocoptLanguageErrorException e)
                {
                    var args = Regex.Replace(e.Message, @"\r?\n", @"\n");
                    context.ReportDiagnostic(Diagnostic.Create(SyntaxError, Location.None, args));
                }
            }
        }

        static readonly SourceText EmptySourceText = SourceText.From(string.Empty);
        static readonly Encoding Utf8BomlessEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        static readonly string[] Vars = "abcdefghijklmnopqrstuvwxyz".Select(ch => ch.ToString()).ToArray();

        [Flags]
        enum GenerationOptions
        {
            None,
            SkipHelpConst,
        }

        public static SourceText Generate(string? ns, string name, SourceText text) =>
            Generate(ns, name, Enumerable.Empty<string>(), null, text, GenerationOptions.None);

        static SourceText Generate(string? ns, string name, IEnumerable<string> parents, string? helpConstName,
                                   SourceText text, GenerationOptions generationOptions) =>
            Generate(ns, name, parents, helpConstName, text, null, generationOptions);

        public static SourceText Generate(string? ns, string name,
                                          SourceText text, Encoding? outputEncoding) =>
            Generate(ns, name, Enumerable.Empty<string>(), null, text, outputEncoding, GenerationOptions.None);

        static SourceText Generate(string? ns, string name, IEnumerable<string> parents, string? helpConstName,
                                   SourceText text, Encoding? outputEncoding, GenerationOptions options)
        {
            if (text.Length == 0)
                return EmptySourceText;

            var helpText = text.ToString();
            var code = new CSharpSourceBuilder();

            Generate(code,
                     ns is { Length: 0 } ? null : ns,
                     name, parents, helpConstName ?? DefaultHelpConstName, helpText,
                     options);

            return new StringBuilderSourceText(code.StringBuilder, outputEncoding ?? text.Encoding ?? Utf8BomlessEncoding);
        }

        static void Generate(CSharpSourceBuilder code,
                             string? ns,
                             string name,
                             IEnumerable<string> parents,
                             string helpConstName,
                             string helpText,
                             GenerationOptions generationOptions)
        {
            var (pattern, options, usage) = Docopt.Internal.ParsePattern(helpText);

            var leaves = Docopt.Internal.GetFlatPatterns(helpText)
                                        .GroupBy(p => p.Name)
                                        .Select(g => (LeafPattern)g.First())
                                        .ToList();

            var (parserTypeName, parserConfigurationCode) =
                leaves.OfType<Option>().Any(o => o is { Name: "-h" or "--help" })
                    ? (nameof(IHelpFeaturingParser<object>), ".EnableHelp()")
                    : (nameof(IBaselineParser<object>), string.Empty);

            const string usageConstName = "Usage";

            code["// <auto-generated/>"].NewLine
                .NewLine
                ["#nullable enable"].NewLine

                .NewLine
                .Using("System.Collections")
                .Using("System.Collections.Generic")
                .Using("DocoptNet")
                .Using("DocoptNet.Internals")
                .UsingAlias("Leaves")["DocoptNet.Internals.ReadOnlyList<DocoptNet.Internals.LeafPattern>"]

                .NewLine
                [ns is not null ? code.Namespace(ns) : code.Blank()]
                [from p in parents select code.Partial.Class[p].NewLine.BlockStart]

                .Partial.Class[name][" : IEnumerable<KeyValuePair<string, object?>>"].NewLine.SkipNextNewLine.Block[code
                    [(generationOptions & GenerationOptions.SkipHelpConst) == GenerationOptions.SkipHelpConst
                         ? code.Blank()
                         : code.NewLine.Public.Const(helpConstName, helpText)]

                    .NewLine
                    .Public.Const(usageConstName, usage)

                    .NewLine
                    .Static.ReadOnly[parserTypeName]["<"][name]["> "]
                                    .Assign("Parser")[code["GeneratedSourceModule.CreateParser("][helpConstName][", Parse)"][parserConfigurationCode]]

                    .NewLine
                    .Public.Static[parserTypeName]["<"][name]["> CreateParser()"].Lambda["Parser"]

                    .NewLine
                    .Static["IParser<"][name][">.IResult Parse(IEnumerable<string> args, ParseFlags flags, string? version)"]
                    .NewLine.Block[code
                        .Var("options")[
                            code.New["List<Option>"].NewLine
                                .Block[code.Each(options,
                                                 static (code, option, _) =>
                                                     code.NewTargeted["("]
                                                         [option.ShortName is { } sn ? code.Literal(sn)[", "] : code.Blank()]
                                                         [option.LongName is { } ln ? code.Literal(ln)[", "] : code.Blank()]
                                                         .Literal(option.ArgCount)[", "]
                                                         [Value(code, option.Value)]["),"].NewLine).SkipNextNewLine]]
                        .NewLine
                        .Return[code["GeneratedSourceModule.Parse("][helpConstName][", "][usageConstName][", args, options, flags, version, Parse)"]]
                        .NewLine
                        ["static IParser<"][name][">.IResult Parse(Leaves left)"].NewLine.Block[code
                            .Var("required")[code.New["RequiredMatcher(1, left, "].New["Leaves()"][')']]
                            ["Match(ref required)"].EndStatement
                            .If["!required.Result || required.Left.Count > 0"][code
                                .Return[code["GeneratedSourceModule.CreateInputErrorResult<"][name][">(string.Empty, "][usageConstName][")"]]]
                            .Var("collected")["required.Collected"]
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
                                                                       [leaf switch { Option   { Value.IsString: true } => "string",
                                                                                      Argument { Value.IsNone: true } or Option { ArgCount: not 0, Value.Kind: not ArgValueKind.StringList } => "string?",
                                                                                      { Value.Kind: var kind } => MapType(kind) }]
                                                                       [")value"].SkipNextNewLine][' '])]
                                 : code.Blank()
                            ]

                            .NewLine
                            .Return["GeneratedSourceModule.CreateArgumentsResult(result)"]]

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
                                       Argument { Value.IsNone: true } or Option { ArgCount: not 0, Value.Kind: not ArgValueKind.StringList } => code["string? "][e.Name][" { get; private set; }"],
                                       { Value.Object: StringList list } => code["StringList "][e.Name][" { get; private set; } = "][Value(code, list.Reverse())].SkipNextNewLine.EndStatement,
                                       { Value.Kind: var kind } => code[MapType(kind)][' '][e.Name][" { get; private set; }"],
                                   }]
                                  .NewLine)
                ] // class
                [from p in parents select code.BlockEnd]
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
                                (children.Count switch
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
                        code[pmv][".Match(" + "PatternMatcher."][lfn][", "].Literal(name)[", ArgValueKind."][leaf.Value.Kind.ToString()][')'].EndStatement.Blank();
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
                    Option   { LongName: null, ShortName: { } name } when char.IsUpper(name[1]) => $"OptUpper{name[1]}",
                    Option   { Name: var name } => $"Opt{GenerateCodeHelper.ConvertToPascalCase(name.ToLowerInvariant())}",
                    var p => throw new NotSupportedException($"Unsupported pattern: {p}")
                };

            static string MapType(ArgValueKind kind) =>
                kind switch
                {
                    ArgValueKind.Boolean    => "bool",
                    ArgValueKind.Integer    => "int",
                    ArgValueKind.String     => "string?",
                    ArgValueKind.StringList => nameof(StringList),
                    _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null)
                };

            static CSharpSourceBuilder Value(CSharpSourceBuilder code, ArgValue value) =>
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
