namespace DocoptNet.CodeGeneration
{
    static class IndentingStringBuilderCSharpExtensions
    {
        public static IndentingStringBuilder DeclareAssigned(this IndentingStringBuilder sb, string name, string rhs) =>
            sb.Append("var ").Append(name).Append(" = ").Append(rhs).Append(";").AppendLine();

        public static IndentingStringBuilder Assign(this IndentingStringBuilder sb, string name, string rhs) =>
            sb.Append(name).Append(" = ").Append(rhs).Append(";").AppendLine();

        public static IndentingStringBuilder BlockStart(this IndentingStringBuilder sb) =>
            sb.Append('{').AppendLine().Indent();

        public static IndentingStringBuilder BlockEnd(this IndentingStringBuilder sb) =>
            sb.Outdent().Append('}').AppendLine();

        public static IndentingStringBuilder Break(this IndentingStringBuilder sb) =>
            sb.Append("break;").AppendLine();

        public static IndentingStringBuilder Throw(this IndentingStringBuilder sb, string expression) =>
            sb.Append("throw ").Append(expression).Append(';').AppendLine();

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
