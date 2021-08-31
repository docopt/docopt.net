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
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Text;
    using Argument = DocoptNet.Argument;
    using Unit = System.ValueTuple;

    [Generator]
    public sealed class SourceGenerator : ISourceGenerator
    {
        static readonly DiagnosticDescriptor SyntaxError =
            new(id: "OPT0001",
                title: "Syntax error",
                messageFormat: "Syntax error: {0}",
                category: "Docopt",
                DiagnosticSeverity.Error,
                isEnabledByDefault: true);

        public void Initialize(GeneratorInitializationContext context) {}

        static class Metadata
        {
            const string Prefix = "build_metadata.AdditionalFiles.";
            public const string SourceItemType = Prefix + nameof(SourceItemType);
            public const string Name = Prefix + nameof(Name);
        }

        public void Execute(GeneratorExecutionContext context)
        {
            context.LaunchDebuggerIfFlagged(nameof(DocoptNet));

            context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.RootNamespace", out var ns);

            var templates =
                from at in context.AdditionalFiles
                select context.AnalyzerConfigOptions.GetOptions(at) is {} options
                    && options.TryGetValue(Metadata.SourceItemType, out var type)
                    && "Docopt".Equals(type, StringComparison.OrdinalIgnoreCase)
                    && at.GetText() is {} text
                     ? new
                       {
                           FilePath = at.Path,
                           Name = options.TryGetValue(Metadata.Name, out var name)
                                  && !string.IsNullOrWhiteSpace(name)
                                ? name
                                : Path.GetFileName(at.Path).Partition(".").Item1,
                           Text = text,

                       }
                     : null
                into t
                where t is not null
                select t;

            var added = false;

            foreach (var t in templates)
            {
                try
                {
                    if (Generate(ns, t.Name, t.Text) is { Length: > 0 } source)
                    {
                        added = true;
                        context.AddSource(t.Name + ".cs", source);
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
                const string resourceNamespace = "DocoptNet.CodeGeneration.Generated.";
                var assembly = GetType().Assembly;
                foreach (var (rn, fn) in from rn in assembly.GetManifestResourceNames()
                                         where rn.StartsWith(resourceNamespace) && rn.EndsWith(".cs")
                                         select (rn, rn.Substring(resourceNamespace.Length)))
                {
                    using var stream = assembly.GetManifestResourceStream(rn)!;
                    using var reader = new StreamReader(stream);
                    var source = Regex.Replace(reader.ReadToEnd(), @"(?<=\bnamespace\s+)DocoptNet(?:\.Generated)?\b", "DocoptNet.Generated");
                    context.AddSource(fn, SourceText.From(source, Utf8BomlessEncoding));
                }
            }
        }

        static readonly SourceText EmptySourceText = SourceText.From(string.Empty);
        static readonly Encoding Utf8BomlessEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        static readonly string[] Vars = "abcdefghijklmnopqrstuvwxyz".Select(ch => ch.ToString()).ToArray();

        public static SourceText Generate(string? ns, string name, SourceText text) =>
            Generate(ns, name, text, null);

        public static SourceText Generate(string? ns, string name, SourceText text, Encoding? outputEncoding)
        {
            if (text.Length == 0)
                return EmptySourceText;

            var helpText = text.ToString();
            var code = new CSharpSourceBuilder();
            Generate(code, ns, name, helpText);
            return new StringBuilderSourceText(code.StringBuilder, outputEncoding ?? text.Encoding ?? Utf8BomlessEncoding);
        }

        static void Generate(CSharpSourceBuilder code, string? ns, string name, string helpText)
        {
            var (pattern, options, usage) = Docopt.ParsePattern(helpText);

            var leaves = Docopt.GetFlatPatterns(helpText)
                               .GroupBy(p => p.Name)
                               .Select(g => (LeafPattern)g.First())
                               .ToList();

            var isNamespaced = !string.IsNullOrEmpty(ns);

            const string helpConstName = "Help";
            const string usageConstName = "Usage";

            code["#nullable enable annotations"].NewLine

                .NewLine
                .Using("System.Collections")
                .Using("System.Collections.Generic")
                .Using("System.Linq")
                .Using("DocoptNet.Generated")
                .Using("Leaves", "DocoptNet.Generated.ReadOnlyList<DocoptNet.Generated.LeafPattern>")
                .UsingStatic("DocoptNet.Generated.GeneratedSourceModule")

                .NewLine
                [isNamespaced ? code.Namespace(ns) : code.Blank()]

                .Partial.Class[name]["Arguments : IEnumerable<KeyValuePair<string, object?>>"].NewLine
                .BlockStart
                .Public.Const(helpConstName, helpText)

                .NewLine
                .Public.Const(usageConstName, usage)

                .NewLine
                .Public.Static[name]["Arguments Apply(IEnumerable<string> args, bool help = true, object? version = null, bool optionsFirst = false, bool exit = false)"].NewLine.BlockStart

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
                .Var("result")[code.New[name]["Arguments()"]]
                [leaves.Any()
                     ? code.NewLine
                           .ForEach["var p in collected"][
                                code.Var("value")["p.Value is { IsStringList: true } ? ((StringList)p.Value).Reverse() : p.Value"]
                                    .Switch["p.Name"]
                                    .Cases(leaves, default(Unit),
                                           static (_, p, _) => CSharpSourceBuilder.SwitchCaseChoice.Choose(p.Name),
                                           static (code, _, p) =>
                                               code[' ']
                                                  .Assign(code["result."][InferPropertyName(p)])[
                                                       code['(']
                                                           [p switch { Option   { Value: { IsString: true } } => "string",
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
                .BlockEnd   // Apply
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

            code.NewLine
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

                .BlockEnd
                [isNamespaced ? code.BlockEnd : code.Blank()]
                .Blank();

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
