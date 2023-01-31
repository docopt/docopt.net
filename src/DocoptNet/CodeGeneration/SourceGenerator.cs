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

    [Generator(LanguageNames.CSharp)]
    public sealed class SourceGenerator : IIncrementalGenerator
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

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // FIXME re-instate launching debugger
            // context.LaunchDebuggerIfFlagged(nameof(DocoptNet));

            const string attributeName = $"{nameof(DocoptNet)}.{nameof(DocoptArgumentsAttribute)}";

            var docoptTypeResults =
                context.SyntaxProvider
                       .ForAttributeWithMetadataName(attributeName,
                            static (node, _) => node is ClassDeclarationSyntax cds && cds.HasOrPotentiallyHasAttributes(),
                            static (ctx, _) =>
                            {
                                var cds = (ClassDeclarationSyntax)ctx.TargetNode;
                                var symbol = (INamedTypeSymbol)ctx.TargetSymbol;

                                // TODO emit diagnostics on nested types? symbol.ContainingType != null
                                var className = symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
                                var namespaceName =
                                    symbol.ContainingNamespace.ToDisplayString(FullyQualifiedFormatWithoutGlobalNamespace)
                                        is { Length: > 0 } s ? s : null;
                                var attributeData = ctx.Attributes[0];
                                var attribute = DocoptArgumentsAttribute.From(attributeData);
                                attribute.HelpConstName ??= DefaultHelpConstName;
                                var help =
                                    symbol.GetMembers()
                                          .Choose(s => s is IFieldSymbol { IsConst: true, Name: var name, ConstantValue: string help }
                                                    && name == attribute.HelpConstName ? Some(help) : default)
                                          .FirstOrDefault();

                                if (help is { } someHelp)
                                {
                                    return Result.Create((DocoptType?)new()
                                    {
                                        Namespace = namespaceName,
                                        Name = className,
                                        Parents = cds.GetParents()
                                                     .Where(tds => tds is ClassDeclarationSyntax)
                                                     .Reverse()
                                                     .Select(p => p.Identifier.ToString())
                                                     .ToImmutableArray(),
                                        HelpConstName = attribute.HelpConstName,
                                        Help = SourceText.From(someHelp),
                                        Options = GenerationOptions.SkipHelpConst
                                    });
                                }

                                var diagnostics = ImmutableArray.Create(DiagnosticInfo.Create(MissingHelpConstError, cds, symbol, attribute.HelpConstName));

                                return new Result<DocoptType?>(null, diagnostics);
                            });

            context.RegisterSourceOutput(docoptTypeResults.Select((r, _) => r.Errors),
                                         static (context, diagnostics) =>
                                         {
                                             foreach (var diagnostic in diagnostics)
                                                 context.ReportDiagnostic(diagnostic.ToDiagnostic());
                                         });

            var docoptTypes = docoptTypeResults.Where(r => r.Value is not null)
                                               .Select((r, _) => r.Value!);

            var rootNamespace =
                context.AnalyzerConfigOptionsProvider
                       .Select((e, _) => e.GlobalOptions.TryGetValue("build_property.RootNamespace", out var rootNamespace) ? rootNamespace : null);

            var docoptSources =
                context.AdditionalTextsProvider
                       .Combine(context.AnalyzerConfigOptionsProvider)
                       .Combine(rootNamespace)
                       .Select((e, _) =>
                       {
                           var ((at, analyzerConfigOptions), ns) = e;
                           return analyzerConfigOptions.GetOptions(at) is var options
                                  && options.TryGetValue(Metadata.SourceItemType, out var type)
                                  && "Docopt".Equals(type, StringComparison.OrdinalIgnoreCase)
                                  && at.GetText() is { } text
                                  ? (DocoptType?)new()
                                    {
                                        Namespace = ns,
                                        Name = options.TryGetValue(Metadata.Name, out var name)
                                               && !string.IsNullOrWhiteSpace(name)
                                             ? name
                                             : Path.GetFileName(at.Path).Partition(".").Item1 + "Arguments",
                                        Parents = ImmutableArray<string>.Empty,
                                        HelpConstName = null,
                                        Help = text,
                                        Options = GenerationOptions.None
                                    }
                                  : default;
                       })
                       .Where(e => e is not null);

            context.RegisterSourceOutput(docoptSources, Generate!);
            context.RegisterSourceOutput(docoptTypes, Generate);

            static void Generate(SourceProductionContext context, DocoptType dt)
            {
                var hintNameBuilder = new StringBuilder();

                try
                {
                    var parentNames = dt.Parents;
                    if (SourceGenerator.Generate(dt.Namespace, dt.Name, parentNames, dt.HelpConstName, dt.Help, dt.Options)
                        is { Length: > 0 } source)
                    {
                        if (dt.Namespace is { } someNamespace) hintNameBuilder.Append(someNamespace).Append('.');
                        if (parentNames.AsSpan().Length > 0)
                        {
                            foreach (var pn in parentNames)
                            {
                                // NOTE! Microsoft.CodeAnalysis.CSharp 3.10 does not allow use of "+"
                                // as is conventional for nested types. It is allowed later versions;
                                // see: https://github.com/dotnet/roslyn/issues/58476
                                hintNameBuilder.Append(pn).Append('-');
                            }
                        }

                        hintNameBuilder.Append(dt.Name);
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

        sealed record DocoptType
        {
            public required string? Namespace { get; init; }
            public required string Name { get; init; }
            public required EquatableArray<string> Parents { get; init; }
            public required string? HelpConstName { get; init; }
            public required SourceText Help { get; init; }
            public required GenerationOptions Options { get; init; }
        }

        static class Result
        {
            public static Result<TValue> Create<TValue>(TValue value)
                where TValue : IEquatable<TValue>? =>
                Create(value, ImmutableArray<DiagnosticInfo>.Empty);

            public static Result<TValue> Create<TValue>(TValue value, EquatableArray<DiagnosticInfo> errors)
                where TValue : IEquatable<TValue>? =>
                new(value, errors);
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

            code["#nullable enable"].NewLine

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
