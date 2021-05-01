namespace DocoptNet.CodeGeneration
{
    using System;
    using System.Text;

    sealed class IndentingStringBuilder
    {
        public IndentingStringBuilder() : this(new StringBuilder()) { }

        public IndentingStringBuilder(StringBuilder builder) =>
            StringBuilder = builder ?? throw new ArgumentNullException(nameof(builder));

        public StringBuilder StringBuilder { get; }
        public int Level { get; private set; }
        public bool IsNewLine { get; private set; } = true;

        public IndentingStringBuilder Indent()
        {
            Level++;
            return this;
        }

        public IndentingStringBuilder Outdent()
        {
            if (Level == 0)
                throw new InvalidOperationException();
            Level--;
            return this;
        }

        StringBuilder OnAppending()
        {
            if (!IsNewLine)
                return StringBuilder;
            IsNewLine = false;
            StringBuilder.Append(' ', Level * 4);
            return StringBuilder;
        }

        public IndentingStringBuilder Append(int value)
        {
            OnAppending().Append(value); // TODO invariant formatting
            return this;
        }

        public IndentingStringBuilder Append(char value)
        {
            OnAppending().Append(value);
            return this;
        }

        public IndentingStringBuilder Append(char value, int repeatCount)
        {
            OnAppending().Append(value, repeatCount);
            return this;
        }

        public IndentingStringBuilder Append(string value)
        {
            OnAppending().Append(value);
            return this;
        }

        public IndentingStringBuilder Append(string value, int startIndex, int count)
        {
            OnAppending().Append(value, startIndex, count);
            return this;
        }

        public IndentingStringBuilder AppendLine()
        {
            StringBuilder.AppendLine();
            IsNewLine = true;
            return this;
        }

        public IndentingStringBuilder AppendLine(string value) =>
            Append(value).AppendLine();

        public override string ToString() => StringBuilder.ToString();
    }
}
