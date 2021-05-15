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

            _ = code.Using("System.Collections")
                    .Using("System.Collections.Generic")
                    .Using("System.Linq")
                    .Using("DocoptNet.Generated")
                    .Using("Leaves", "DocoptNet.Generated.ReadOnlyList<DocoptNet.Generated.LeafPattern>")
                    .NewLine;

            var isNamespaced = !string.IsNullOrEmpty(ns);
            if (isNamespaced)
                code.Namespace(ns);

            _ = code["partial class "][name].NewLine.Block;

            _ = code["public const string Usage = "].Literal(usage).EndStatement;

            void AppendTree(Pattern pattern, int level = 0)
            {
                _ = code["// "][' ', level * 2];
                switch (pattern)
                {
                    case BranchPattern { Children: var children } branch:
                        _ = code[branch.GetType().Name][':'].NewLine;
                        foreach (var child in children)
                            AppendTree(child, level + 1);
                        break;
                    case LeafPattern leaf:
                        _ = code[leaf][" -> "][leaf.ToNode()].NewLine;
                        break;
                }
            }

            _ = code.NewLine;
            var (pattern, exitUsage) = new Docopt().ParsePattern(usage);
            AppendTree(pattern);

            void AppendTreeCode(Pattern pattern)
            {
                switch (pattern)
                {
                    case OneOrMore { Children: { Count: 1 } children }:
                        _ = code["new OneOrMore("].NewLine;
                        AppendTreeCode(children[0]);
                        _ = code[')'];
                        break;
                    case BranchPattern { Children: { Count: > 0 } children } branch:
                        _ = code["new "][branch.GetType().Name]["(new Pattern[]"].NewLine.Block;
                        var i = 0;
                        foreach (var child in children)
                        {
                            AppendTreeCode(child);
                            if (++i < children.Count)
                                _ = code[','].NewLine;
                        }
                        _ = code.NewLine.Outdent["})"];
                        break;
                    case Command command:
                        _ = code["new Command("][Literal(command.Name)][')'];
                        break;
                    case Argument { Name: var name }:
                        _ = code["new Argument("][Literal(name)][", (ValueObject)null)"];
                        break;
                    case Option option:
                        _ = code["new Option("]
                                    [option.ShortName is {} sn ? Literal(sn) : "null"][", "]
                                    [option.LongName is {} ln ? Literal(ln) : "null"][", "]
                                    [option.ArgCount][", "]
                                    ["new ValueObject("][option.Value][')']
                                [')'];
                        break;
                }
            }

            _ = code.NewLine
                ["static readonly Pattern Pattern ="].NewLine.Indent;
            AppendTreeCode(pattern);
            _ = code.EndStatement.Outdent;

            void AppendCode(Pattern pattern, string pm, int level = 0)
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
                        if (pattern.Children.Count > 1)
                        {
                            _ = code["switch ("][m][".Index)"].NewLine.Block;
                            var i = 0;
                            foreach (var child in children)
                            {
                                _ = code.Case(i).Block;
                                AppendCode(child, m, level);
                                _ = code.Break.BlockEnd;
                                i++;
                            }
                            _ = code.BlockEnd;
                        }
                        else
                        {
                            AppendCode(children[0], m, level);
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
                            DocoptNet.Argument => "MatchArgument",
                            Option => "MatchOption",
                            _ => throw new NotImplementedException()
                        };
                        _ = code[pm]
                                [".Match("]
                                    ["PatternMatcher."][lfn][", "]
                                    [Literal(name)][", "]
                                    ["value: "][leaf.Value switch
                                    {
                                        null => "null",
                                        { Value: null     } => "null",
                                        { IsList: true    } => "new ArrayList()",
                                        { IsInt: true, AsInt: var n } => Literal(n).ToString(),
                                        { Value: string v } => Literal(v).ToString(),
                                        { IsTrue: true    } => "true",
                                        { IsFalse: true   } => "false",
                                        _ => throw new NotSupportedException(leaf.Value?.ToString() ?? "(null)"), // todo emit diagnostic
                                    }][", "]
                                    ["isList: "][leaf.Value is { IsList: true } ? "true" : "false"][", "]
                                    ["isInt: "][leaf.Value is { IsOfTypeInt: true } ? "true" : "false"][')'].EndStatement;
                        break;
                    }
                }
            }

            _ = code.NewLine;
            _ = code["static readonly ICollection<Option> Options = new Option[]"].NewLine.Block;
            foreach (var option in Docopt.ParseDefaults(usage))
            {
                AppendTreeCode(option);
                _ = code[','].NewLine;
            }
            _ = code.SkipNextNewLine.BlockEnd.EndStatement;

            _ = code.NewLine;
            _ = code["static Dictionary<string, ValueObject> Apply(IEnumerable<string> args, bool help = true, object version = null, bool optionsFirst = false, bool exit = false)"].NewLine.Block
                .DeclareAssigned("tokens", "new Tokens(args, typeof(DocoptInputErrorException))")
                ["var options = Options.Select(e => new Option(e.ShortName, e.LongName, e.ArgCount, e.Value)).ToList()"].EndStatement
                .DeclareAssigned("arguments", "Docopt.ParseArgv(tokens, options, optionsFirst).AsReadOnly()")
                .If(@"help && arguments.Any(o => o is { Name: ""-h"" or ""--help"", Value: { IsNullOrEmpty: false } })")
                .Block
                .Throw("new DocoptExitException(Usage)").BlockEnd
                .If(@"version is not null && arguments.Any(o => o is { Name: ""--version"", Value: { IsNullOrEmpty: false } })")
                .Block
                .Throw("new DocoptExitException(version.ToString())").BlockEnd
                .DeclareAssigned("left", "arguments")
                .DeclareAssigned("collected", "new Leaves()")
                .DeclareAssigned("a", "new RequiredMatcher(1, left, collected)")
                .Do.Block;
            AppendCode(pattern, "a");
            _ = code.BlockEnd
                    .DoWhile("false")
                    .NewLine
                    .If("!a.Result || a.Left.Count > 0").Block
                    .Const("exitUsage", exitUsage)
                    .Throw("new DocoptInputErrorException(exitUsage)").BlockEnd
                    .NewLine;

            _ = code.SkipStatementEnd.DeclareAssigned("dict", "new Dictionary<string, ValueObject>").NewLine.Block;

            foreach (var leaf in pattern.Flat().OfType<LeafPattern>())
            {
                _ = code['['].Literal(leaf.Name)["] = "]
                        ["new ValueObject("]
                            [leaf.Value]["),"].NewLine;
            }

            _ = code.SkipNextNewLine.BlockEnd.EndStatement;

            _ = code.NewLine
                    .Assign("collected", "a.Collected")
                    .ForEach("p", "collected").Block
                    .Assign("dict[p.Name]", "p.Value").BlockEnd
                    .NewLine
                    .Return("dict").BlockEnd;

            _ = code.NewLine;
            using (var reader = new StringReader(new Docopt().GenerateCode(usage)))
            {
                while (reader.ReadLine() is { } line)
                    _ = code[line].NewLine;
            }

            _ = code.BlockEnd;

            if (isNamespaced)
                _ = code.BlockEnd;

            return new StringBuilderSourceText(code.StringBuilder, outputEncoding ?? text.Encoding ?? Utf8BomlessEncoding);
        }
    }
}
