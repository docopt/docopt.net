#nullable enable

namespace DocoptNet.CodeGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;

    sealed class CSharpSourceBuilder :
        CSharpSourceBuilder.IStatementFlow,
        CSharpSourceBuilder.IBlockFlow,
        CSharpSourceBuilder.IIfFlow,
        CSharpSourceBuilder.IControlBlockFlow,
        CSharpSourceBuilder.ISwitchFlow,
        CSharpSourceBuilder.ISwitchCasesFlow,
        CSharpSourceBuilder.IForEachFlow
    {
        readonly StringBuilder _sb;
        bool _skipNextNewLine;

        public CSharpSourceBuilder() : this(new()) { }
        public CSharpSourceBuilder(StringBuilder sb) => _sb = sb;

        public StringBuilder StringBuilder => _sb;

        public int Level { get; private set; }
        public bool IsNewLine { get; private set; } = true;

        public CSharpSourceBuilder Indent
        {
            get
            {
                Level++;
                return this;
            }
        }

        public CSharpSourceBuilder Outdent
        {
            get
            {
                if (Level == 0)
                    throw new InvalidOperationException();
                Level--;
                return this;
            }
        }

        StringBuilder OnAppending()
        {
            if (!IsNewLine)
                return _sb;
            IsNewLine = false;
            _sb.Append(' ', Level * 4);
            return _sb;
        }

        void Append(char value) => OnAppending().Append(value);
        void Append(string value) => OnAppending().Append(value);
        void Append(int value) => OnAppending().Append(value);

        public void AppendLine()
        {
            if (_skipNextNewLine)
            {
                _skipNextNewLine = false;
                return;
            }

            _sb.AppendLine();
            IsNewLine = true;
        }

        public override string ToString() => _sb.ToString();

        public CSharpSourceBuilder SkipNextNewLine { get { _skipNextNewLine = true; return this; } }

        [Conditional("DEBUG")]
        void AssertSame(CSharpSourceBuilder other) => Debug.Assert(ReferenceEquals(this, other));

        public CSharpSourceBuilder this[string code] { get { Append(code); return this; } }
        public CSharpSourceBuilder this[char code] { get { Append(code); return this; } }
        public CSharpSourceBuilder this[CSharpSourceBuilder code] { get { AssertSame(code); return this; } }

        public CSharpSourceBuilder Blank() => this;
        public CSharpSourceBuilder NewLine { get { AppendLine(); return this; } }

        static readonly char[] ForbiddenRegularStringLiteralChars =
        {
            '"', '\\', '\r', '\n',
            '\u0085', // Next line character
            '\u2028', // Line separator character
            '\u2029', // Paragraph separator character
        };

        const string Quote = "\"";

        public CSharpSourceBuilder Literal(string value) =>
            //
            // https://github.com/dotnet/csharplang/blob/f1533732b093e54665471fcf92aba5c77f0acedd/spec/lexical-structure.md
            //
            // single_regular_string_literal_character
            //     : '<Any character except " (U+0022), \\ (U+005C), and new_line_character>'
            //     ;
            // new_line_character
            //     : '<Carriage return character (U+000D)>'
            //     | '<Line feed character (U+000A)>'
            //     | '<Next line character (U+0085)>'
            //     | '<Line separator character (U+2028)>'
            //     | '<Paragraph separator character (U+2029)>'
            //     ;
            //
            _ = value.IndexOfAny(ForbiddenRegularStringLiteralChars) >= 0
              ? this['@'][Quote][value.Replace(Quote, Quote + Quote)][Quote]
              : this[Quote][value][Quote];

        public CSharpSourceBuilder Literal(int value) { Append(value); return this; }

        public CSharpSourceBuilder Each<T>(IEnumerable<T> source, Func<CSharpSourceBuilder, T, int, CSharpSourceBuilder> builder)
        {
            var i = 0;
            foreach (var item in source)
                builder(this, item, i++);
            return this;
        }

        public CSharpSourceBuilder Using(string ns) =>
            this["using "][ns].EndStatement;

        public CSharpSourceBuilder Using(string alias, string typeName) =>
            this["using "][alias].Equal[typeName].EndStatement;

        public CSharpSourceBuilder UsingStatic(string typeName) =>
            this["using "].Static[typeName].EndStatement;

        public CSharpSourceBuilder Namespace(string name) =>
            this["namespace "][name].NewLine.BlockStart;

        public CSharpSourceBuilder Null    => this["null"];
        public CSharpSourceBuilder True    => this["true"];
        public CSharpSourceBuilder False   => this["false"];

        public CSharpSourceBuilder Break   => this["break"].EndStatement;

        public CSharpSourceBuilder Partial => this["partial "];
        public CSharpSourceBuilder Static  => this["static "];
        public CSharpSourceBuilder Class   => this["class "];
        public CSharpSourceBuilder Public  => this["public "];
        public CSharpSourceBuilder Yield   => this["yield "];
        public CSharpSourceBuilder New     => this["new "];

        public CSharpSourceBuilder Equal   => this[" = "];

        public CSharpSourceBuilder BlockStart => this['{'].NewLine.Indent;
        public CSharpSourceBuilder BlockEnd   => Outdent['}'].NewLine;

        public CSharpSourceBuilder EndStatement => this[';'].NewLine;

        public CSharpSourceBuilder LineComment(string comment) =>
            this["// "][comment].NewLine;

        public CSharpSourceBuilder Const(string name, string value) =>
            this["const string "][name].Equal.Literal(value).EndStatement;

        public IStatementFlow Return => this["return "];
        public IStatementFlow Var(string name) => this["var "].Assign(name);
        public IStatementFlow Assign(string name) => this[name].Equal;
        public IStatementFlow Assign(CSharpSourceBuilder code) { AssertSame(code); return Equal; }

        public IBlockFlow Block => BlockStart;

        public interface IBlockFlow
        {
            CSharpSourceBuilder this[string code] { get; }
            CSharpSourceBuilder this[CSharpSourceBuilder code] { get; }
        }

        CSharpSourceBuilder IBlockFlow.this[string code] => BlockFlow(this[code]);
        CSharpSourceBuilder IBlockFlow.this[CSharpSourceBuilder code] => BlockFlow(code);
        CSharpSourceBuilder BlockFlow(CSharpSourceBuilder code) { AssertSame(code); return BlockEnd; }

        public interface IStatementFlow
        {
            CSharpSourceBuilder this[string code] { get; }
            CSharpSourceBuilder this[CSharpSourceBuilder code] { get; }
        }

        CSharpSourceBuilder IStatementFlow.this[string code] => StatementFlow(this[code]);
        CSharpSourceBuilder IStatementFlow.this[CSharpSourceBuilder code] => StatementFlow(code);
        CSharpSourceBuilder StatementFlow(CSharpSourceBuilder code) { AssertSame(code); return EndStatement; }

        public IIfFlow If => this["if ("];

        public interface IIfFlow
        {
            IBlockFlow this[string code] { get; }
            IBlockFlow this[CSharpSourceBuilder code] { get; }
        }

        IBlockFlow IIfFlow.this[string code] => IfFlow(this[code]);
        IBlockFlow IIfFlow.this[CSharpSourceBuilder code] => IfFlow(code);
        CSharpSourceBuilder IfFlow(CSharpSourceBuilder code) { AssertSame(code); return this[')'].NewLine.BlockStart; }

        public IControlBlockFlow While => this["while ("];

        public interface IControlBlockFlow
        {
            IBlockFlow this[string code] { get; }
            IBlockFlow this[CSharpSourceBuilder code] { get; }
        }

        IBlockFlow IControlBlockFlow.this[string code] => ControlBlockFlow(this[code]);
        IBlockFlow IControlBlockFlow.this[CSharpSourceBuilder code] => ControlBlockFlow(code);
        IBlockFlow ControlBlockFlow(CSharpSourceBuilder code) { AssertSame(code); return this[')'].NewLine.Block; }

        public IForEachFlow ForEach => this["foreach (var "];

        public interface IForEachFlow
        {
            IControlBlockFlow this[string code] { get; }
            IControlBlockFlow this[CSharpSourceBuilder code] { get; }
        }

        IControlBlockFlow IForEachFlow.this[string code] => ForEachFlow(this[code]);
        IControlBlockFlow IForEachFlow.this[CSharpSourceBuilder code] => ForEachFlow(code);
        CSharpSourceBuilder ForEachFlow(CSharpSourceBuilder code) { AssertSame(code); return this[" in "]; }

        public ISwitchFlow Switch => this["switch ("];

        public interface ISwitchFlow
        {
            ISwitchCasesFlow this[string code] { get; }
            ISwitchCasesFlow this[CSharpSourceBuilder code] { get; }
        }

        ISwitchCasesFlow ISwitchFlow.this[string code] => SwitchFlow(this[code]);
        ISwitchCasesFlow ISwitchFlow.this[CSharpSourceBuilder code] => SwitchFlow(code);
        CSharpSourceBuilder SwitchFlow(CSharpSourceBuilder code) { AssertSame(code); return this[')'].NewLine.BlockStart; }

        public interface ISwitchCasesFlow
        {
            CSharpSourceBuilder Cases<T, TArg>(IEnumerable<T> source,
                                               TArg arg,
                                               Func<TArg, T, int, SwitchCaseChoice> caseChooser,
                                               Func<CSharpSourceBuilder, TArg, T, CSharpSourceBuilder> caseBlock);
        }

        CSharpSourceBuilder ISwitchCasesFlow.Cases<T, TArg>(
            IEnumerable<T> source,
            TArg arg,
            Func<TArg, T, int, SwitchCaseChoice> caseChooser,
            Func<CSharpSourceBuilder, TArg, T, CSharpSourceBuilder> caseBlock)
        {
            var i = 0;
            foreach (var item in source)
            {
                _ = this["case "][caseChooser(arg, item, i++) switch
                    {
                        { IsInteger: true, Integer: var n } => Literal(n),
                        { IsString: true, String: var str } => Literal(str),
                        _ => throw new NotSupportedException(),
                    }]
                    [':'][caseBlock(this, arg, item)].Break;
            }

            return BlockEnd;
        }

        public readonly struct SwitchCaseChoice
        {
            readonly int _num;
            readonly string? _str;

            SwitchCaseChoice(int num, string? str) => (_num, _str) = (num, str);

            public static SwitchCaseChoice Choose(int num) => new(num, null);
            public static SwitchCaseChoice Choose(string str) => new(0, str);

            public bool IsInteger => !IsString;
            public bool IsString => _str is not null;

            public int Integer => IsInteger ? _num : throw new InvalidOperationException();
            public string String => _str ?? throw new InvalidOperationException();

            public override string ToString() => IsInteger ? Integer.ToString() : String;
        }
    }
}
