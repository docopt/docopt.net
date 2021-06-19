namespace DocoptNet.CodeGeneration
{
    using System;
    using System.Collections;
    using System.Text;
    using Microsoft.CodeAnalysis.CSharp;

    sealed class CSharpSourceBuilder
    {
        public CSharpSourceBuilder() : this(new StringBuilder()) { }

        public CSharpSourceBuilder(StringBuilder builder) =>
            StringBuilder = builder ?? throw new ArgumentNullException(nameof(builder));

        public StringBuilder StringBuilder { get; }
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
                return StringBuilder;
            IsNewLine = false;
            StringBuilder.Append(' ', Level * 4);
            return StringBuilder;
        }

        CSharpSourceBuilder Append(int value)
        {
            OnAppending().Append(value); // TODO invariant formatting
            return this;
        }

        CSharpSourceBuilder Append(char value)
        {
            OnAppending().Append(value);
            return this;
        }

        CSharpSourceBuilder Append(char value, int repeatCount)
        {
            OnAppending().Append(value, repeatCount);
            return this;
        }

        CSharpSourceBuilder Append(string value)
        {
            OnAppending().Append(value);
            return this;
        }

        CSharpSourceBuilder AppendLine()
        {
            if (_skipNextNewLine)
            {
                _skipNextNewLine = false;
                return this;
            }
            StringBuilder.AppendLine();
            IsNewLine = true;
            return this;
        }

        public override string ToString() => StringBuilder.ToString();

        const string Quote = "\"";

        public CSharpSourceBuilder this[string value] => Append(value);
        public CSharpSourceBuilder this[int value] => Append(value);
        public CSharpSourceBuilder this[object value] => Append(value.ToString());
        public CSharpSourceBuilder this[char value] => Append(value);
        public CSharpSourceBuilder this[char value, int repeatCount] => Append(value, repeatCount);

        public CSharpSourceBuilder NewLine => AppendLine();

        bool _skipNextNewLine;

        public CSharpSourceBuilder SkipNextNewLine { get { _skipNextNewLine = true; return this; } }

        public CSharpSourceBuilder Literal(string value) =>
            this['@'][Quote][value.Replace(Quote, Quote + Quote)][Quote];

        public CSharpSourceBuilder Namespace(string @namespace) =>
            this["namespace "][@namespace].NewLine.Block;

        public CSharpSourceBuilder Using(string @namespace) =>
            this["using "][@namespace][';'].NewLine;

        public CSharpSourceBuilder Using(string alias, string typeName) =>
            this["using "][alias][" = "][typeName][';'].NewLine;

        bool _skipStatementEnd;

        public CSharpSourceBuilder SkipStatementEnd { get { _skipStatementEnd = true; return this; } }

        public CSharpSourceBuilder EndStatement
        {
            get
            {
                if (_skipStatementEnd)
                {
                    _skipStatementEnd = false;
                    return this;
                }

                return Append(';').AppendLine();
            }
        }

        CSharpSourceBuilder Equal => this[" = "];

        public CSharpSourceBuilder Const(string name, string value) =>
            this["const string "][name].Equal.Literal(value).EndStatement;

        public CSharpSourceBuilder Return(string expression) =>
            this["return "][expression].EndStatement;

        public CSharpSourceBuilder DeclareAssigned(string name, string rhs) =>
            this["var "][name].Equal[rhs].EndStatement;

        public CSharpSourceBuilder Assign(string name, string rhs) =>
            this[name].Equal[rhs].EndStatement;

        public CSharpSourceBuilder Block => this['{'].NewLine.Indent;

        public CSharpSourceBuilder BlockEnd => Outdent['}'].NewLine;

        public CSharpSourceBuilder Break => this["break"].EndStatement;

        public CSharpSourceBuilder Throw(string expression) =>
            this["throw "][expression].EndStatement;

        public CSharpSourceBuilder Case(int n) =>
            this["case "][n][':'].NewLine;

        public CSharpSourceBuilder Do =>
            this["do"].NewLine;

        public CSharpSourceBuilder DoWhile(string expression) =>
            this["while ("][expression][')'].EndStatement;

        public CSharpSourceBuilder If(string expression) =>
            this["if ("][expression][')'].NewLine;

        public CSharpSourceBuilder ForEach(string var, string expression) =>
            this["foreach (var "][var][" in "][expression][')'].NewLine;

        public CSharpSourceBuilder this[ValueObject value]
        {
            get
            {
                if (value is { IsList: true, Value: ArrayList { Count: var count and > 0 } items })
                {
                    _ = this["new ArrayList"].NewLine.Block;
                    var i = 0;
                    foreach (var item in items)
                    {
                        switch (item)
                        {
                            case string s: _ = Literal(s); break;
                            case var v: throw new NotSupportedException("Unsupported value type: " + v?.GetType());
                        }
                        if (++i < count)
                            _ = this[", "];
                    }
                    _ = NewLine.BlockEnd;
                }
                else
                {
                    _ = this[value switch
                    {
                        null => "null",
                        { IsInt: true, AsInt: var n } => SyntaxFactory.Literal(n),
                        { Value: string v } => SyntaxFactory.Literal(v),
                        { IsTrue: true } => "true",
                        { IsFalse: true } => "false",
                        { IsList: true, Value: ArrayList { Count: 0 } } => "new ArrayList()",
                        _ => throw new NotSupportedException(), // todo emit diagnostic
                    }];
                }

                return this;
            }
        }
    }
}
