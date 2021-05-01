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
namespace DocoptNet.Generated
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Leaves = ReadOnlyList<LeafPattern>;

    abstract record Pattern;

    abstract record BranchPattern(ImmutableArray<Pattern> Children) : Pattern;
    sealed record Required(ImmutableArray<Pattern> Children) : BranchPattern(Children);
    sealed record Optional(ImmutableArray<Pattern> Children) : BranchPattern(Children);
    sealed record Either(ImmutableArray<Pattern> Children) : BranchPattern(Children);
    sealed record OneOrMore(Pattern Pattern) : BranchPattern(ImmutableArray.Create(Pattern));
    abstract record LeafPattern(string Name, ValueObject Value) : Pattern;

    sealed record Command(string Name) : LeafPattern(Name, new ValueObject(true));
    sealed record Argument(string Name, ValueObject Value) : LeafPattern(Name, Value);
    sealed record Option(string ShortName, string LongName, int ArgCount, ValueObject Value) : LeafPattern(LongName ?? ShortName, Value);

    sealed partial class ValueObject { }

    partial class ValueObject
    {
        public object Value { get; private set; }

        internal ValueObject(object obj) =>
            Value = obj is ICollection collection ? new ArrayList(collection) : obj;

        internal ValueObject() => Value = null;

        public bool IsNullOrEmpty => Value?.ToString()?.Length is null or 0;
        public bool IsFalse => Value as bool? == false;
        public bool IsTrue => Value as bool? == true;
        public bool IsList => Value is ArrayList;
        internal bool IsOfTypeInt => Value is int;
        public bool IsInt => Value != null && (Value is int || int.TryParse(Value.ToString(), out _));
        public int AsInt => IsList ? 0 : Convert.ToInt32(Value);
        public bool IsString => Value is string;
        public ArrayList AsList => IsList ? Value as ArrayList : new ArrayList { Value };

        internal void Add(ValueObject increment)
        {
            if (increment == null) throw new ArgumentNullException(nameof(increment));
            if (increment.Value == null) throw new ArgumentException(nameof(increment));
            if (Value == null) throw new InvalidOperationException();

            if (increment.IsOfTypeInt)
            {
                if (IsList)
                    ((ArrayList)Value).Add(increment.AsInt);
                else
                    Value = increment.AsInt + AsInt;
            }
            else
            {
                var list = new ArrayList();
                if (IsList)
                    list.AddRange(AsList);
                else
                    list.Add(Value);
                if (increment.IsList)
                    list.AddRange(increment.AsList);
                else
                    list.Add(increment);
                Value = list;
            }
        }
    }

    static class Module
    {
        public static (bool Matched, Leaves Left, Leaves Collected)
            Leaf(Leaves left, Leaves collected,
                 string name, object value, bool isList, bool isInt,
                 int index, LeafPattern match)
        {
            if (match == null)
            {
                return (false, left, collected);
            }
            var left_ = left.RemoveAt(index);
            var sameName = collected.Where(a => a.Name == name).ToList();
            if (value != null && (isList || isInt))
            {
                var increment = new ValueObject(1);
                if (!isInt)
                    increment = match.Value.IsString ? new ValueObject(new [] {match.Value})  : match.Value;
                if (sameName.Count == 0)
                    return (true, left_, collected.Append(match with { Value = increment }));
                sameName[0].Value.Add(increment);
                return (true, left_, collected);
            }
            return (true, left_, collected.Append(match));
        }

        public static (int, LeafPattern) Command(Leaves left, string command)
        {
            for (var i = 0; i < left.Count; i++)
            {
                if (left[i] is Argument { Value: { } value })
                {
                    if (value.ToString() == command)
                        return (i, new Command(command));
                    break;
                }
            }
            return default;
        }

        public static (int, LeafPattern) Argument(Leaves left, string name)
        {
            for (var i = 0; i < left.Count; i++)
            {
                if (left[i] is Argument { Value: var value })
                    return (i, new Argument(name, value));
            }
            return default;
        }

        public static (int, LeafPattern) Option(Leaves left, string name)
        {
            for (var i = 0; i < left.Count; i++)
            {
                if (left[i].Name == name)
                    return (i, left[i]);
            }
            return default;
        }
    }

    static class ReadOnlyList
    {
        public static ReadOnlyList<T> AsReadOnly<T>(this IList<T> list) => new(list);
    }

    readonly struct ReadOnlyList<T> : IReadOnlyList<T>
    {
        static readonly T[] EmptyArray = new T[0];

        readonly IList<T> _list;

        public ReadOnlyList(IList<T> list) => _list = list;
        IList<T> List => _list ?? EmptyArray;
        public int Count => List.Count;
        public T this[int index] => List[index];
        public IEnumerator<T> GetEnumerator() => List.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public ReadOnlyList<T> Append(T item)
        {
            var array = new T[Count + 1];
            CopyTo(array, 0, Count);
            array[Count] = item;
            return array.AsReadOnly();
        }

        public ReadOnlyList<T> RemoveAt(int index)
        {
            var array = new T[Count - 1];
            CopyTo(array, 0, index);
            CopyTo(array, index, index + 1, Count - index - 1);
            return array.AsReadOnly();
        }

        void CopyTo(T[] array, int index, int count) =>
            CopyTo(array, 0, index, count);

        void CopyTo(T[] array, int targetIndex, int sourceIndex, int count)
        {
            var list = List;
            while (count > 0)
            {
                array[targetIndex++] = list[sourceIndex++];
                count--;
            }
        }
    }
}
");
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
              .Append("using System.Collections.Immutable;").AppendLine()
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
            sb.Append("static void Apply(Leaves left, Leaves collected)").AppendLine()
              .BlockStart()
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
