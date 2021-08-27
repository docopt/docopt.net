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
    using static CSharpSourceModule;

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
            using (CurrentSourceBuilder.Push())
            {
                Generate(ns, name, helpText);
                return new StringBuilderSourceText(CurrentSourceBuilder.Pop(), outputEncoding ?? text.Encoding ?? Utf8BomlessEncoding);
            }
        }

        static void Generate(string? ns, string name, string helpText)
        {
            var isNamespaced = !string.IsNullOrEmpty(ns);

            _ = Code("#nullable enable annotations") + NewLine

              + NewLine
              + Using("System.Collections")
              + Using("System.Collections.Generic")
              + Using("System.Linq")
              + Using("DocoptNet.Generated")
              + Using("Leaves", "DocoptNet.Generated.ReadOnlyList<DocoptNet.Generated.LeafPattern>")

              + NewLine
              + (isNamespaced ? Namespace(ns) : Blank)

              + Partial + Class + name + "Arguments : IEnumerable<KeyValuePair<string, object?>>" + NewLine
              + BlockStart
              + Public + Const("HelpText", helpText);

            static CurrentSourceBuilder GeneratePatternMatchingCode(Pattern pattern, string pmv, int level = 0)
            {
                if (level >= 26) // todo proper diagnostics reporting
                    throw new NotSupportedException();

                LineComment(pattern?.ToString() ?? string.Empty);

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
                        var mv = Vars[++level];
                        _ = Var(mv, (Matcher: matcher, Children: children, PrevMatchVar: pmv),
                                static e => New + e.Matcher + '(' + e.Children.Count + ", " + e.PrevMatchVar + ".Left, " + e.PrevMatchVar + ".Collected)")
                          + While((Level: level, MatchVar: mv, PrevMatchVar: pmv, Pattern: pattern, Children: children),
                                  static e => Code(e.MatchVar) + ".Next()",
                                  static e => e.Pattern.Children.Count switch
                                              {
                                                  > 1 => Switch(e,
                                                                static arg => Code(arg.MatchVar) + ".Index",
                                                                e.Children,
                                                                static (_, _, i) => SwitchCaseChoice.Choose(i),
                                                                static (e, child) => NewLine +
                                                                    Block((Pattern: child, e.MatchVar, e.Level),
                                                                        static e => GeneratePatternMatchingCode(e.Pattern, e.MatchVar, e.Level))),
                                                  1 => GeneratePatternMatchingCode(e.Children[0], e.MatchVar, e.Level),
                                                  _ => Blank,
                                              }
                                            + If(e.MatchVar, static m => Code('!') + m + ".LastMatched", static _ => Break))
                          + pmv + ".Fold(" + mv + ".Result)" + EndStatement;
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
                        _ = Blank
                          + pmv + ".Match(" + "PatternMatcher." + lfn + ", " + Literal(name) + ", ValueKind." + leaf.Value.Kind.ToString() + ')' + EndStatement;
                        break;
                    }
                }

                return CurrentCode;
            }

            var (pattern, options, usage) = Docopt.ParsePattern(helpText);

            _ = NewLine
              + Public + Const("Usage", usage)

              + NewLine
              + Public + Static + name + "Arguments Apply(IEnumerable<string> args, bool help = true, object? version = null, bool optionsFirst = false, bool exit = false)" + NewLine + BlockStart
              + Var("tokens", "new Tokens(args, typeof(DocoptInputErrorException))")
              + Var("options", options, static options => New + "List<Option>" + NewLine + Block(options, static options =>
                    Each(options, static (option, _) =>
                         New + "Option(" + (option.ShortName is {} sn ? Literal(sn) : Null) + ", "
                             + (option.LongName is {} ln ? Literal(ln) : Null) + ", "
                             + option.ArgCount + ", "
                             + Value(option.Value) + ")," + NewLine) + SkipNextNewLine))
              + Var("arguments", "Docopt.ParseArgv(tokens, options, optionsFirst).AsReadOnly()")
              + If(@"help && arguments.Any(o => o is { Name: ""-h"" or ""--help"", Value: { IsTrue: true } })",
                    static () => ThrowNew(nameof(DocoptExitException), "HelpText"))
              + If(@"version is not null && arguments.Any(o => o is { Name: ""--version"", Value: { IsTrue: true } })",
                    static () => ThrowNew(nameof(DocoptExitException), "version.ToString()"))
              + Var("left", "arguments")
              + Var("collected", "new Leaves()")
              + Var("a", "new RequiredMatcher(1, left, collected)")

              + DoWhile("false", pattern, static pattern => GeneratePatternMatchingCode(pattern, "a"))

              + NewLine
              + If("!a.Result || a.Left.Count > 0", static () => ThrowNew(nameof(DocoptInputErrorException), "Usage"))

              + NewLine
              + Assign("collected", "a.Collected")
              + Var("result", name, static name => New + name + "Arguments()");

            var leaves = Docopt.GetFlatPatterns(helpText)
                               .GroupBy(p => p.Name)
                               .Select(g => (LeafPattern)g.First())
                               .ToList();

            if (leaves.Any())
            {
                _ = NewLine
                  + ForEach("p", "collected", leaves, static leaves =>
                        Var("value", "p.Value is { IsStringList: true } ? ((StringList)p.Value).Reverse() : p.Value")
                        + Switch(static () => Code("p.Name"), leaves, static (p, _) => SwitchCaseChoice.Choose(p.Name),
                                 static p => Blank
                                           + " result." + InferPropertyName(p)
                                           + Equal + '('
                                           + p switch
                                             {
                                                 Option   { Value: { IsString: true } } => "string",
                                                 Argument { Value: { IsNone: true } } or Option { ArgCount: not 0, Value: { Kind: not ValueKind.StringList } } => "string?",
                                                 { Value: { Kind: var kind } } => MapType(kind)
                                             }
                                           + ")value" + SkipNextNewLine + EndStatement + ' '));
            }

            _ = NewLine
              + Return("result")
              + BlockEnd   // Apply

              + NewLine
              + "IEnumerator<KeyValuePair<string, object?>> GetEnumerator()" + NewLine + BlockStart
              + (leaves.Any() ? Each(leaves, static (p, _) => Yield + Return(p, static p => Code("KeyValuePair.Create(") + Literal(p.Name) + ", (object?)" + InferPropertyName(p) + ')'))
                              : Yield + Break)
              + BlockEnd

              + NewLine
              + "IEnumerator<KeyValuePair<string, object?>> IEnumerable<KeyValuePair<string, object?>>.GetEnumerator() => GetEnumerator()" + EndStatement
              + "IEnumerator IEnumerable.GetEnumerator() => GetEnumerator()" + EndStatement

              + Each(from p in leaves
                     select (Name: InferPropertyName(p), Leaf: p),
                     static (e, _) =>
                         NewLine
                         + "/// <summary><c>" + e.Leaf.ToString().EncodeXmlText() + "</c></summary>"
                         + NewLine
                         + Public + e.Leaf switch
                           {
                               Option { Value: { IsString: true } str } => Code("string ") + e.Name + " { get; private set; } = " + Literal((string)str) + SkipNextNewLine + EndStatement,
                               Argument { Value: { IsNone: true } } or Option { ArgCount: not 0, Value: { Kind: not ValueKind.StringList } } => Code("string? ") + e.Name + " { get; private set; }",
                               { Value: { Object: StringList list } } => Code("StringList ") + e.Name + " { get; private set; } = " + Value(list.Reverse()) + SkipNextNewLine + EndStatement,
                               { Value: { Kind: var kind } } => Code(MapType(kind)) + ' ' + e.Name + " { get; private set; }",
                           }
                         + NewLine)

              + BlockEnd
              + (isNamespaced ? BlockEnd : Blank);

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

            static CurrentSourceBuilder Value(Value value)
            {
                if (value.TryAsStringList(out var items) && items.Count > 0)
                {
                    _ = Code("StringList.TopBottom(")
                      + Each(items, static (item, i) => (i > 0 ? Code(", ") : Blank) + Literal(item))
                      + ')';
                }
                else
                {
                    _ = value.Object switch
                    {
                        null => Null,
                        int n => Literal(n),
                        string s => Literal(s),
                        true => True,
                        false => False,
                        StringList { IsEmpty: true } => Code("StringList.Empty"),
                        _ => throw new NotSupportedException(), // todo emit diagnostic
                    };
                }

                return CurrentCode;
            }
        }
    }
}
