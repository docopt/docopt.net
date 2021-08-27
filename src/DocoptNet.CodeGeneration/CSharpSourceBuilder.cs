namespace DocoptNet.CodeGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;

    sealed class CurrentSourceBuilder : IDisposable
    {
        [ThreadStatic]
        static CurrentSourceBuilder? _current;

        readonly StringBuilder _sb;
        bool _popped;
        CurrentSourceBuilder? _previous;
        bool _skipNextNewLine;

        CurrentSourceBuilder(StringBuilder sb, CurrentSourceBuilder? previous) =>
            (_sb, _previous) = (sb, previous);

        public static CurrentSourceBuilder Push() =>
            _current = new CurrentSourceBuilder(new StringBuilder(), _current);

        public static StringBuilder Pop()
        {
            var sb = Instance._sb;
            Instance.Dispose();
            return sb;
        }

        public static CurrentSourceBuilder Instance
        {
            get
            {
                Debug.Assert(_current is not null);
                return _current;
            }
        }

        void IDisposable.Dispose() => Dispose();

        void Dispose()
        {
            if (_popped)
                return;
            _popped = true;
            (_current, _previous) = (_previous, null);
        }

        public int Level { get; private set; }
        public bool IsNewLine { get; private set; } = true;

        public CurrentSourceBuilder Indent
        {
            get
            {
                Level++;
                return this;
            }
        }

        public CurrentSourceBuilder Outdent
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

        CurrentSourceBuilder Append(char value)
        {
            OnAppending().Append(value);
            return this;
        }

        CurrentSourceBuilder Append(string value)
        {
            OnAppending().Append(value);
            return this;
        }

        CurrentSourceBuilder Append(int value)
        {
            OnAppending().Append(value);
            return this;
        }

        public CurrentSourceBuilder AppendLine()
        {
            if (_skipNextNewLine)
            {
                _skipNextNewLine = false;
                return this;
            }
            _sb.AppendLine();
            IsNewLine = true;
            return this;
        }

        public override string ToString() => _sb.ToString();

        public CurrentSourceBuilder SkipNextNewLine { get { _skipNextNewLine = true; return this; } }

        public static CurrentSourceBuilder operator+(CurrentSourceBuilder sb, string str) => sb.Append(str);
        public static CurrentSourceBuilder operator+(CurrentSourceBuilder sb, char ch) => sb.Append(ch);
        public static CurrentSourceBuilder operator+(CurrentSourceBuilder sb, int n) => sb.Append(n);

        public static CurrentSourceBuilder operator+(CurrentSourceBuilder a, CurrentSourceBuilder b)
        {
            Debug.Assert(ReferenceEquals(a, b));
            return b;
        }
    }

    static class CSharpSourceModule
    {
        public static CurrentSourceBuilder CurrentCode => CurrentSourceBuilder.Instance;

        public static CurrentSourceBuilder Blank => CurrentCode;
        public static CurrentSourceBuilder NewLine => CurrentCode.AppendLine();

        public static CurrentSourceBuilder Code(string raw) => CurrentCode + raw;
        public static CurrentSourceBuilder Code(char raw) => CurrentCode + raw;

        static readonly char[] ForbiddenRegularStringLiteralChars =
        {
            '"', '\\', '\r', '\n',
            '\u0085', // Next line character
            '\u2028', // Line separator character
            '\u2029', // Paragraph separator character
        };

        const string Quote = "\"";

        public static CurrentSourceBuilder Literal(string value) =>
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
              ? CurrentCode + '@' + Quote + value.Replace(Quote, Quote + Quote) + Quote
              : CurrentCode + Quote + value + Quote;

        public static CurrentSourceBuilder Literal(int value) => CurrentCode + value;

        public static CurrentSourceBuilder Each<T>(IEnumerable<T> source, Func<T, int, CurrentSourceBuilder> builder)
        {
            var i = 0;
            foreach (var item in source)
                builder(item, i++);
            return CurrentCode;
        }

        public static CurrentSourceBuilder Using(string ns) =>
            Blank + "using " + ns + EndStatement;

        public static CurrentSourceBuilder Using(string alias, string typeName) =>
            Blank + "using " + alias + Equal + typeName + EndStatement;

        public static CurrentSourceBuilder Namespace(string name) =>
            Blank + "namespace " + name + NewLine + BlockStart;

        public static CurrentSourceBuilder Null    => Blank + "null";
        public static CurrentSourceBuilder True    => Blank + "true";
        public static CurrentSourceBuilder False   => Blank + "false";

        public static CurrentSourceBuilder Break   => Blank + "break" + EndStatement;

        public static CurrentSourceBuilder Partial => Blank + "partial ";
        public static CurrentSourceBuilder Static  => Blank + "static ";
        public static CurrentSourceBuilder Class   => Blank + "class ";
        public static CurrentSourceBuilder Public  => Blank + "public ";
        public static CurrentSourceBuilder Yield   => Blank + "yield ";
        public static CurrentSourceBuilder New     => Blank + "new ";

        public static CurrentSourceBuilder Equal   => Blank + " = ";

        public static CurrentSourceBuilder BlockStart => Blank + '{' + NewLine + CurrentCode.Indent;
        public static CurrentSourceBuilder BlockEnd   => CurrentCode.Outdent + '}' + NewLine;

        public static CurrentSourceBuilder Block<T>(T arg, Func<T, CurrentSourceBuilder> block) =>
            BlockStart + block(arg) + BlockEnd;

        public static CurrentSourceBuilder SkipNextNewLine => CurrentCode.SkipNextNewLine;

        public static CurrentSourceBuilder EndStatement => Blank + ';' + NewLine;

        public static CurrentSourceBuilder LineComment(string comment) =>
            Blank + "// " + comment + NewLine;

        public static CurrentSourceBuilder Const(string name, string value) =>
            Blank + "const string " + name + Equal + Literal(value) + EndStatement;

        public static CurrentSourceBuilder Return(string expression) =>
            Return(expression, static expression => Blank + expression);

        public static CurrentSourceBuilder Return<T>(T arg, Func<T, CurrentSourceBuilder> expression) =>
            Blank + "return " + expression(arg) + EndStatement;

        public static CurrentSourceBuilder Assign(string name, string rhs) =>
            Assign(name, rhs, static rhs => Blank + rhs);

        public static CurrentSourceBuilder Assign<T>(string name, T arg, Func<T, CurrentSourceBuilder> rhs) =>
            Blank + name + Equal + rhs(arg) + EndStatement;

        public static CurrentSourceBuilder Var(string name, string rhs) =>
            Var(name, rhs, static rhs => Blank + rhs);

        public static CurrentSourceBuilder Var<T>(string name, T arg, Func<T, CurrentSourceBuilder> rhs) =>
            Blank + "var " + Assign(name, arg, rhs);

        public static CurrentSourceBuilder If(string condition, Func<CurrentSourceBuilder> thenBlock) =>
            If((Condition: condition, ThenBlock: thenBlock),
               static e => Blank + e.Condition,
               static e => e.ThenBlock());

        public static CurrentSourceBuilder If<T>(T arg, Func<T, CurrentSourceBuilder> condition,
                                                Func<T, CurrentSourceBuilder> thenBlock) =>
            Blank + "if (" + condition(arg) + ')' + NewLine + Block(arg, thenBlock);

        public static CurrentSourceBuilder ThrowNew(string type, string args) =>
            Blank + "throw " + New + type + '(' + args + ')' + EndStatement;

        public static CurrentSourceBuilder DoWhile<T>(string condition, T arg, Func<T, CurrentSourceBuilder> block) =>
            Blank + "do" + NewLine + Block(arg, block) + "while (" + condition + ')' + EndStatement;

        public static CurrentSourceBuilder While<T>(T arg, Func<T, CurrentSourceBuilder> condition,
                                                   Func<T, CurrentSourceBuilder> block) =>
            Blank + "while (" + condition(arg) + ')' + NewLine + Block(arg, block);

        public static CurrentSourceBuilder ForEach<T>(string var, string expression,
                                                     T arg, Func<T, CurrentSourceBuilder> block) =>
            Blank + "foreach (var " + var + " in " + expression + ')' + NewLine + Block(arg, block);

        public static CurrentSourceBuilder
            Switch<T>(Func<CurrentSourceBuilder> expression,
                      IEnumerable<T> source,
                      Func<T, int, SwitchCaseChoice> caseChooser,
                      Func<T, CurrentSourceBuilder> caseBlock) =>
            Switch((Expression: expression, CaseChooser: caseChooser, CaseBlock: caseBlock),
                   static f => f.Expression(),
                   source,
                   static (f, e, i) => f.CaseChooser(e, i),
                   static (f, e) => f.CaseBlock(e));

        public static CurrentSourceBuilder
            Switch<TArg, TItem>(TArg arg, Func<TArg, CurrentSourceBuilder> expression,
                                IEnumerable<TItem> source,
                                Func<TArg, TItem, int, SwitchCaseChoice> caseChooser,
                                Func<TArg, TItem, CurrentSourceBuilder> caseBlock)
        {
            _ = Blank + "switch (" + expression(arg) + ')' + NewLine + BlockStart;

            var i = 0;
            foreach (var item in source)
            {
                _ = Blank
                  + "case " + caseChooser(arg, item, i++) switch
                              {
                                  { IsInteger: true, Integer: var n } => Blank + n,
                                  { IsString: true, String: var str } => Literal(str),
                                  _ => throw new NotSupportedException(),
                              }
                  + ':' + caseBlock(arg, item)
                  + Break;
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
