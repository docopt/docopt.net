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

        const int I = 0, L = 1, C = 2, EM = 3, EL = 4, EC = 5, T = 6, LL = 7;
        static readonly string[][] Vars =
            "abcdefghijklmnopqrstuvwxyz".Select(x => new[] { "ii", "l", "c", "em", "el", "ec", "t", "ll" }.Select(y => y + x).ToArray())
                                        .ToArray();

        public static SourceText Generate(string? ns, string name, SourceText text) =>
            Generate(ns, name, text, null);

        public static SourceText Generate(string? ns, string name, SourceText text, Encoding? outputEncoding)
        {
            if (text.Length == 0)
                return EmptySourceText;

            var usage = text.ToString();
            var sb = new IndentingStringBuilder();

            sb.Append("using System.Collections;").AppendLine()
              .Append("using System.Collections.Generic;").AppendLine()
              .Append("using System.Linq;").AppendLine()
              .Append("using DocoptNet.Generated;").AppendLine()
              .Append("using Leaves = DocoptNet.Generated.ReadOnlyList<DocoptNet.Generated.LeafPattern>;").AppendLine()
              .Append("using static DocoptNet.Generated.Module;").AppendLine()
              .AppendLine();

            var isNamespaced = !string.IsNullOrEmpty(ns);
            if (isNamespaced)
            {
                sb.Append("namespace ").Append(ns).AppendLine();
                sb.BlockStart();
            }

            sb.Append("partial class ").Append(name).AppendLine();
            sb.BlockStart();

            sb.Append("public const string Usage = ").Literal(usage).Append(';').AppendLine();

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

            void AppendTreeCode(Pattern pattern)
            {
                switch (pattern)
                {
                    case OneOrMore { Children: { Count: 1 } children }:
                        sb.Append("new OneOrMore(").AppendLine();
                        AppendTreeCode(children[0]);
                        sb.Append(")");
                        break;
                    case BranchPattern { Children: { Count: > 0 } children } branch:
                        sb.Append("new ").Append(branch.GetType().Name).Append("(new Pattern[]").AppendLine().Append('{').AppendLine().Indent();
                        var i = 0;
                        foreach (var child in children)
                        {
                            AppendTreeCode(child);
                            if (++i < children.Count)
                            {
                                sb.Append(',');
                                sb.AppendLine();
                            }
                        }
                        sb.AppendLine().Outdent().Append("})");
                        break;
                    case Command command:
                        sb.Append("new Command(")
                          .Append(Literal(command.Name).ToString())
                          .Append(')');
                        break;
                    case Argument { Name: var name }:
                        sb.Append("new Argument(")
                          .Append(Literal(name).ToString())
                          .Append(", (ValueObject)null)");
                        break;
                    case Option option:
                        sb.Append("new Option(")
                          .Append(Literal(option.ShortName ?? string.Empty).ToString())
                          .Append(", ")
                          .Append(Literal(option.LongName ?? string.Empty).ToString())
                          .Append(", ")
                          .Append(option.ArgCount)
                          .Append(", new ValueObject(")
                          .Append(option.Value switch
                           {
                               { IsInt: true, AsInt: var n } => Literal(n).ToString(),
                               { Value: string v } => Literal(v).ToString(),
                               { IsTrue: true } => "true",
                               { IsFalse: true } => "false",
                               _ => throw new NotSupportedException(), // todo emit diagnostic
                           })
                          .Append("))");
                        break;
                }
            }

            sb.AppendLine()
              .Append("static readonly Pattern Pattern =").AppendLine()
              .Indent();
            AppendTreeCode(pattern);
            sb.Append(';').AppendLine()
              .Outdent();

            void AppendCode(Pattern pattern, string left, string collected, int level = 0)
            {
                if (level >= 26) // todo proper diagnostics reporting
                    throw new NotSupportedException();

                sb.Append("// ").Append(pattern?.ToString() ?? string.Empty).AppendLine();

                switch (pattern)
                {
                    case BranchPattern { Children: { } children }:
                    {
                        var iv = Vars[level][I];
                        var (l, c) = (Vars[level][L], Vars[level][C]);
                        var (em, el, ec) = (Vars[level][EM], Vars[level][EL], Vars[level][EC]);
                        var times = Vars[level][T];
                        var l_ = Vars[level][LL];
                        level++;
                        void BeginChildrenLoop(string left, string collected)
                        {
                            if (children.Count == 1)
                            {
                                sb.BlockStart();
                                AppendCode(children[0], left, collected, level);
                                return;
                            }
                            sb.Append("for (var ").Append(iv).Append(" = 0; ")
                              .Append(iv).Append(" < ").Append(children.Count).Append("; ")
                              .Append(iv).Append("++)").AppendLine()
                              .BlockStart();
                            sb.Switch(iv)
                              .BlockStart();
                            var i = 0;
                            foreach (var child in children)
                            {
                                sb.Case(i)
                                  .BlockStart();
                                AppendCode(child, left, collected, level);
                                sb.Break()
                                  .BlockEnd();
                                i++;
                            }
                            sb.BlockEnd();
                        }
                        switch (pattern)
                        {
                            case Required:
                            {
                                sb.DeclareAssigned(l, left).DeclareAssigned(c, collected);
                                BeginChildrenLoop(l, c);
                                sb.Append("if (!rm)").AppendLine()
                                  .BlockStart()
                                  .Assign("rl", left).Assign("rc", collected)
                                  .Break()
                                  .BlockEnd();
                                sb.Assign(l, "rl").Assign(c, "rc");
                                sb.BlockEnd();
                                break;
                            }
                            case Optional:
                            {
                                sb.DeclareAssigned(l, left).DeclareAssigned(c, collected);
                                BeginChildrenLoop(l, c);
                                sb.Assign(l, "rl").Assign(c, "rc");
                                sb.BlockEnd();
                                sb.Assign("rm", "true").Assign("rl", l).Assign("rc", c);
                                break;
                            }
                            case Either:
                            {
                                sb.DeclareAssigned(em, "false");
                                sb.DeclareAssigned(el, left);
                                sb.DeclareAssigned(ec, collected);
                                BeginChildrenLoop(left, collected);
                                sb.Append("if (rm && (").Append(em).Append(" || rl.Count < ").Append(el).Append(".Count))").AppendLine()
                                  .BlockStart()
                                  .Assign(em, "true")
                                  .Assign(el, "rl")
                                  .Assign(ec, "rc")
                                  .BlockEnd();
                                sb.BlockEnd();
                                sb.Assign("rm", em).Assign("rl", el).Assign("rc", ec);
                                break;
                            }
                            case OneOrMore:
                            {
                                sb.DeclareAssigned(l, left).DeclareAssigned(c, collected);
                                sb.DeclareAssigned(times, "0");
                                sb.DeclareAssigned(l_, "default(Leaves?)");
                                sb.AppendLine("while (true)")
                                  .BlockStart();
                                AppendCode(pattern.Children[0], l, c, level);
                                sb.Append(times).AppendLine(" += rm ? 0 : 1;");
                                sb.Append("if (").Append(l_).Append(" is {} l_ && l_.Equals(").Append(l).AppendLine("))")
                                  .Indent().Break().Outdent();
                                sb.BlockEnd(); // while
                                sb.Append("if (").Append(times).Append(" >= 0)").AppendLine()
                                  .BlockStart()
                                  .Assign("rm", "true")
                                  .Assign("rl", l)
                                  .Assign("rc", c)
                                  .BlockEnd()
                                  .AppendLine("else")
                                  .BlockStart()
                                  .Assign("rm", "true")
                                  .Assign("rl", left)
                                  .Assign("rc", collected)
                                  .BlockEnd();
                                break;
                            }
                            default:
                                throw new NotSupportedException($"Unsupported pattern: {pattern}");
                        }
                        break;
                    }
                    case LeafPattern { Name: var name } leaf:
                    {
                        var lfn = leaf switch
                        {
                            Command => "Command",
                            DocoptNet.Argument => "Argument",
                            Option => "Option",
                            _ => throw new NotImplementedException()
                        };
                        sb.Append("var (i, match) = ").Append(lfn).Append("(").Append(left).Append(", ").Append(Literal(name).ToString()).Append(");").AppendLine();
                        sb.Append("(rm, rl, rc) = Leaf(")
                          .Append(left).Append(", ")
                          .Append(collected).Append(", ")
                          .Append(Literal(name).ToString()).Append(", ")
                          .Append("value: ").Append(leaf.Value switch
                           {
                               null => "null",
                               { Value: null     } => "null",
                               { IsList: true    } => "new ArrayList()",
                               { IsInt: true, AsInt: var n } => Literal(n).ToString(),
                               { Value: string v } => Literal(v).ToString(),
                               { IsTrue: true    } => "true",
                               { IsFalse: true   } => "false",
                               _ => throw new NotSupportedException(leaf.Value?.ToString() ?? "(null)"), // todo emit diagnostic
                           }).Append(", ")
                          .Append("isList: ").Append(leaf.Value is { IsList: true } ? "true" : "false").Append(", ")
                          .Append("isInt: ").Append(leaf.Value is { IsOfTypeInt: true } ? "true" : "false")
                          .Append(", i, match);").AppendLine();
                        break;
                    }
                }
            }

            sb.AppendLine();
            sb.AppendLine("static readonly ICollection<Option> Options = new[]")
              .BlockStart();
            foreach (var option in Docopt.ParseDefaults(usage))
            {
                AppendTreeCode(option);
                sb.AppendLine(",");
            }
            sb.BlockEnd().AppendLine(";");

            sb.AppendLine();
            sb.Append("static void Apply(string[] args, bool help = true, object version = null, bool optionsFirst = false, bool exit = false)").AppendLine()
              .BlockStart()
              .DeclareAssigned("tokens", "new Tokens(args, typeof(DocoptInputErrorException))")
              .DeclareAssigned("arguments", "Docopt.ParseArgv(tokens, Options, optionsFirst).AsReadOnly();")
              .AppendLine(@"if (help && arguments.Any(o => o is { Name: ""-h"" or ""--help"", Value: { IsNullOrEmpty: false } }))")
              .BlockStart()
              .Throw("new DocoptExitException(Usage)")
              .BlockEnd()
              .AppendLine(@"if (version is not null && arguments.Any(o => o is { Name: ""--version"", Value: { IsNullOrEmpty: false } }))")
              .BlockStart()
              .Throw("new DocoptExitException(version.ToString())")
              .BlockEnd()
              .DeclareAssigned("left", "arguments")
              .DeclareAssigned("collected", "new Leaves()")
              .Append("var rm = false; var rl = left; var rc = collected;").AppendLine()
              .AppendLine("do")
              .BlockStart();
            AppendCode(pattern, "left", "collected");
            sb.BlockEnd()
              .AppendLine("while (false);")
              .BlockEnd();

            sb.AppendLine();
            using (var reader = new StringReader(new Docopt().GenerateCode(usage)))
            {
                while (reader.ReadLine() is { } line)
                    sb.Append(line).AppendLine();
            }

            sb.BlockEnd();

            if (isNamespaced)
                sb.BlockEnd();

            return new StringBuilderSourceText(sb.StringBuilder, outputEncoding ?? text.Encoding ?? Utf8BomlessEncoding);
        }
    }
}
