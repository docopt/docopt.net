namespace DocoptNet.CodeGeneration
{
    static class IndentingStringBuilderCSharpExtensions
    {
        const string Quote = "\"";

        public static IndentingStringBuilder Literal(this IndentingStringBuilder sb, string value) =>
            sb.Append('@').Append(Quote).Append(value.Replace(Quote, Quote + Quote)).Append(Quote);

        static IndentingStringBuilder EndStatement(this IndentingStringBuilder sb, bool no) =>
            (no ? sb : sb.Append(';')).AppendLine(no);

        public static IndentingStringBuilder EndStatement(this IndentingStringBuilder sb) =>
            sb.Append(';').AppendLine();

        public static IndentingStringBuilder Const(this IndentingStringBuilder sb, string name, string value) =>
            sb.Append("const string ").Append(name).Append(" = ").Literal(value).EndStatement();

        public static IndentingStringBuilder Return(this IndentingStringBuilder sb, string expression) =>
            sb.Append("return ").Append(expression).EndStatement();

        public static IndentingStringBuilder DeclareAssigned(this IndentingStringBuilder sb, string name, string rhs, bool unterminated = false) =>
            sb.Append("var ").Append(name).Append(" = ").Append(rhs).EndStatement(unterminated);

        public static IndentingStringBuilder Assign(this IndentingStringBuilder sb, string name, string rhs) =>
            sb.Append(name).Append(" = ").Append(rhs).EndStatement();

        static IndentingStringBuilder AppendLine(this IndentingStringBuilder sb, bool no) =>
            no ? sb : sb.AppendLine();

        public static IndentingStringBuilder BlockStart(this IndentingStringBuilder sb) =>
            sb.Append('{').AppendLine().Indent();

        public static IndentingStringBuilder BlockEnd(this IndentingStringBuilder sb, bool noNewLine = false) =>
            sb.Outdent().Append('}').AppendLine(noNewLine);

        public static IndentingStringBuilder Break(this IndentingStringBuilder sb) =>
            sb.Append("break").EndStatement();

        public static IndentingStringBuilder Throw(this IndentingStringBuilder sb, string expression) =>
            sb.Append("throw ").Append(expression).EndStatement();

        public static IndentingStringBuilder Switch(this IndentingStringBuilder sb, string expr) =>
            sb.Append("switch (").Append(expr).Append(')').AppendLine();

        public static IndentingStringBuilder Case(this IndentingStringBuilder sb, int n) =>
            sb.Append("case ").Append(n).Append(':').AppendLine();

        public static IndentingStringBuilder For(this IndentingStringBuilder sb, string init, string condition, string iter) =>
            sb.Append("for (").Append(init).Append("; ")
              .Append(condition).Append("; ")
              .Append(iter).Append(')')
              .AppendLine();
    }
}
