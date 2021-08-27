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
    using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
    using Argument = DocoptNet.Argument;

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

            var usage = text.ToString();
            var code = new CSharpSourceBuilder();

            _ = code["#nullable enable annotations"].NewLine
                    .NewLine
                    .Using("System.Collections")
                    .Using("System.Collections.Generic")
                    .Using("System.Linq")
                    .Using("DocoptNet.Generated")
                    .Using("Leaves", "DocoptNet.Generated.ReadOnlyList<DocoptNet.Generated.LeafPattern>")
                    .NewLine;

            var isNamespaced = !string.IsNullOrEmpty(ns);
            if (isNamespaced)
                code.Namespace(ns);

            _ = code["partial class "][name]["Arguments : IEnumerable<KeyValuePair<string, object?>>"].NewLine.Block;

            _ = code["public const string Usage = "].Literal(usage).EndStatement;

            void GeneratePatternMatchingCode(Pattern pattern, string pm, int level = 0)
            {
                if (level >= 26) // todo proper diagnostics reporting
                    throw new NotSupportedException();

                _ = code["// "][pattern?.ToString() ?? string.Empty].NewLine;

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
                        level++;
                        var m = Vars[level];
                        _ = code["var "][m][" = "]["new "][matcher]['('][children.Count][", "][pm][".Left, "][pm][".Collected)"].EndStatement;
                        _ = code["while ("][m][".Next())"].NewLine.Block;
                        switch (pattern.Children.Count)
                        {
                            case > 1:
                                _ = code["switch ("][m][".Index)"].NewLine.Block;
                                var i = 0;
                                foreach (var child in children)
                                {
                                    _ = code.Case(i).Block;
                                    GeneratePatternMatchingCode(child, m, level);
                                    _ = code.Break.BlockEnd;
                                    i++;
                                }
                                _ = code.BlockEnd;
                                break;
                            case 1:
                                GeneratePatternMatchingCode(children[0], m, level);
                                break;
                        }
                        _ = code["if (!"][m][".LastMatched)"].NewLine.Indent.Break.Outdent;
                        _ = code.BlockEnd;
                        _ = code[pm][".Fold("][m][".Result)"].EndStatement;
                        break;
                    }
                    case LeafPattern { Name: var name } leaf:
                    {
                        var lfn = leaf switch
                        {
                            Command => "MatchCommand",
                            Argument => "MatchArgument",
                            Option => "MatchOption",
                            _ => throw new NotImplementedException()
                        };
                        _ = code[pm][".Match("]["PatternMatcher."][lfn][", "][Literal(name)][", ValueKind."][leaf.Value.Kind.ToString()][')'].EndStatement;
                        break;
                    }
                }
            }

            _ = code.NewLine;

            var (pattern, options, exitUsage) = Docopt.ParsePattern(usage);

            _ = code["public static "][name]["Arguments Apply(IEnumerable<string> args, bool help = true, object? version = null, bool optionsFirst = false, bool exit = false)"].NewLine.Block
                .DeclareAssigned("tokens", "new Tokens(args, typeof(DocoptInputErrorException))")
                ["var options = new List<Option>"].NewLine.Block;
            foreach (var option in options)
            {
                _ = code["new Option("][option.ShortName is {} sn ? Literal(sn) : "null"][", "]
                                       [option.LongName is {} ln ? Literal(ln) : "null"][", "]
                                       [option.ArgCount][", "]
                                       [option.Value]["),"].NewLine;
            }
            _ = code.SkipNextNewLine.BlockEnd.EndStatement
                .DeclareAssigned("arguments", "Docopt.ParseArgv(tokens, options, optionsFirst).AsReadOnly()")
                .If(@"help && arguments.Any(o => o is { Name: ""-h"" or ""--help"", Value: { IsTrue: true } })")
                .Block
                .Throw("new DocoptExitException(Usage)").BlockEnd
                .If(@"version is not null && arguments.Any(o => o is { Name: ""--version"", Value: { IsTrue: true } })")
                .Block
                .Throw("new DocoptExitException(version.ToString())").BlockEnd
                .DeclareAssigned("left", "arguments")
                .DeclareAssigned("collected", "new Leaves()")
                .DeclareAssigned("a", "new RequiredMatcher(1, left, collected)")
                .Do;
            GeneratePatternMatchingCode(pattern, "a");
            _ = code.DoWhile("false")
                    .NewLine
                    .If("!a.Result || a.Left.Count > 0").Block
                    .Const("exitUsage", exitUsage)
                    .Throw("new DocoptInputErrorException(exitUsage)").BlockEnd
                    .NewLine;

            _ = code.Assign("collected", "a.Collected")
                    .DeclareAssigned("result", $"new {name}Arguments()");

            var leaves = Docopt.GetFlatPatterns(usage)
                               .GroupBy(p => p.Name)
                               .Select(g => (LeafPattern)g.First())
                               .ToList();

            if (leaves.Any())
            {
                _ = code.NewLine
                        .ForEach("p", "collected").Block
                        .DeclareAssigned("value", "p.Value is { IsStringList: true } ? ((StringList)p.Value).Reverse() : p.Value")
                        ["switch (p.Name)"].NewLine
                        .Block;

                foreach (var p in leaves)
                {
                    _ = code.SkipNextNewLine.Case(p.Name)
                            [" result."][InferPropertyName(p)][" = ("][p switch { Option   { Value: { IsString: true } } => "string",
                                                                                  Argument { Value: { IsNone: true } } or Option { ArgCount: not 0, Value: { Kind: not ValueKind.StringList } } => "string?",
                                                                                  { Value: { Kind: var kind } } => MapType(kind) }][")value"]
                            .SkipNextNewLine.EndStatement[' ']
                            .Break;
                }

                _ = code.BlockEnd   // switch
                        .BlockEnd;  // foreach

            }

            _ = code.NewLine
                    .Return("result")
                    .BlockEnd;  // Apply

            _ = code.NewLine;
            _ = code["IEnumerator<KeyValuePair<string, object?>> GetEnumerator()"].NewLine.Block;
            if (leaves.Any())
            {
                foreach (var line in from p in leaves
                                     select $"yield return KeyValuePair.Create({Literal(p.Name)}, (object?){InferPropertyName(p)})")
                {
                    _ = code[line].EndStatement;
                }
            }
            else
            {
                _ = code["yield break"].EndStatement;
            }

            _ = code.BlockEnd;

            _ = code.NewLine
                    ["IEnumerator<KeyValuePair<string, object?>> IEnumerable<KeyValuePair<string, object?>>.GetEnumerator() => GetEnumerator()"].EndStatement
                    ["IEnumerator IEnumerable.GetEnumerator() => GetEnumerator()"].EndStatement;

            foreach (var (leaf, generator) in
                from p in leaves
                select (Name: InferPropertyName(p), Pattern: p) into e
                select (e.Pattern, (Func<CSharpSourceBuilder, CSharpSourceBuilder>)(e.Pattern switch
                {
                    Option { Value: { IsString: true } str } => c => c["string "][e.Name][" { get; private set; } = "][str].SkipNextNewLine.EndStatement,
                    Argument { Value: { IsNone: true } } or Option { ArgCount: not 0, Value: { Kind: not ValueKind.StringList } } => c => c["string? "][e.Name][" { get; private set; }"],
                    { Value: { Object: StringList list } } => c => c["StringList "][e.Name][" { get; private set; } = "][list.Reverse()].SkipNextNewLine.EndStatement,
                    { Value: { Kind: var kind } } => c => c[MapType(kind)][' '][e.Name][" { get; private set; }"],
                })))
            {
                _ = generator(code.NewLine["/// <summary><c>"][leaf.ToString().EncodeXmlText()]["</c></summary>"].NewLine["public "]).NewLine;
            }

            _ = code.BlockEnd;

            if (isNamespaced)
                _ = code.BlockEnd;

            return new StringBuilderSourceText(code.StringBuilder, outputEncoding ?? text.Encoding ?? Utf8BomlessEncoding);

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
        }
    }
}
