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
                context.AddSource("Common.cs", @"
using System.Collections.Immutable;

abstract record Pattern;

abstract record BranchPattern(ImmutableArray<Pattern> Children) : Pattern;
record Required(ImmutableArray<Pattern> Children) : BranchPattern(Children);
record Optional(ImmutableArray<Pattern> Children) : BranchPattern(Children);
record OptionsShortcut() : Optional(ImmutableArray<Pattern>.Empty);
record Either(ImmutableArray<Pattern> Children) : BranchPattern(Children);
record OneOrMore(Pattern Pattern) : BranchPattern(ImmutableArray.Create(Pattern));
abstract record LeftPattern() : Pattern;

record Command(string Name) : LeftPattern;
record Argument(string Name, string Value) : LeftPattern;
record Option(string ShortName, string LongName, int ArgCount, object Value) : LeftPattern;
");
            }
        }

        static readonly SourceText EmptySourceText = SourceText.From(string.Empty);
        static readonly Encoding Utf8BomlessEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        public static SourceText Generate(string? ns, string name, SourceText text) =>
            Generate(ns, name, text, null);

        public static SourceText Generate(string? ns, string name, SourceText text, Encoding? outputEncoding)
        {
            if (text.Length == 0)
                return EmptySourceText;

            var usage = text.ToString();
            var sb = new IndentingStringBuilder();

            sb.Append("using System.Collections;").AppendLine();
            sb.Append("using System.Collections.Immutable;").AppendLine()
                .AppendLine();

            var isNamespaced = !string.IsNullOrEmpty(ns);
            if (isNamespaced)
            {
                sb.Append("namespace ").Append(ns).AppendLine();
                sb.Append("{").AppendLine().Indent();
            }

            sb.Append("partial class ").Append(name).AppendLine();
            sb.Append("{").AppendLine().Indent();

            const string quote = "\"";
            sb.Append("public const string Usage = @")
              .Append(quote).Append(usage.Replace(quote, quote + quote)).Append(quote).Append(";")
              .AppendLine();

            void AppendTree(Pattern pattern, int level = 0)
            {
                sb.Append("// ").Append(' ', level * 2);
                switch (pattern)
                {
                    case BranchPattern { Children: var children } branch:
                        sb.Append(branch.GetType().Name).Append(":").AppendLine();
                        foreach (var child in children)
                            AppendTree(child, level + 1);
                        break;
                    case LeafPattern leaf:
                        sb.Append(leaf.ToString()).Append(" -> ").Append(leaf.ToNode().ToString()).AppendLine();
                        break;
                }
            }

            sb.AppendLine();
            var pattern = new Docopt().ParsePattern(usage);
            AppendTree(pattern);

            void AppendTreeCode(Pattern pattern, int level = 0)
            {
                sb.Append(' ', level * 4);
                switch (pattern)
                {
                    case OneOrMore { Children: { Count: 1 } children }:
                        sb.Append("new OneOrMore(").AppendLine();
                        AppendTreeCode(children[0], level + 1);
                        sb.Append(")");
                        break;
                    case BranchPattern { Children: { Count: > 0 } children } branch:
                        sb.Append("new ").Append(branch.GetType().Name).Append("(ImmutableArray.Create<Pattern>(").AppendLine();
                        var i = 0;
                        foreach (var child in children)
                        {
                            AppendTreeCode(child, level + 1);
                            if (++i < children.Count)
                            {
                                sb.Append(',');
                                sb.AppendLine();
                            }
                        }
                        sb.Append("))");
                        break;
                    case Command command:
                        sb.Append("new Command(")
                          .Append(Literal(command.Name).ToString())
                          .Append(')');
                        break;
                    case Argument { Name: var name }:
                        sb.Append("new Argument(")
                          .Append(Literal(name).ToString())
                          .Append(", null)");
                        break;
                    case Option option:
                        sb.Append("new Option(")
                          .Append(Literal(option.ShortName ?? string.Empty).ToString())
                          .Append(", ")
                          .Append(Literal(option.LongName ?? string.Empty).ToString())
                          .Append(", ")
                          .Append(option.ArgCount.ToInvariantString())
                          .Append(", null")
                          .Append(')');
                        break;
                }
            }

            sb.AppendLine();
            sb.Append("static readonly Pattern Pattern =").AppendLine();
            sb.Indent();
            AppendTreeCode(pattern);
            sb.Append(';');
            sb.AppendLine();
            sb.Outdent();

            sb.AppendLine();
            using (var reader = new StringReader(new Docopt().GenerateCode(usage)))
            {
                while (reader.ReadLine() is { } line)
                    sb.Append(line).AppendLine();
            }

            sb.Outdent().Append("}").AppendLine();

            if (isNamespaced)
                sb.Outdent().Append("}").AppendLine();

            return new StringBuilderSourceText(sb.StringBuilder, outputEncoding ?? text.Encoding ?? Utf8BomlessEncoding);
        }
    }
}
