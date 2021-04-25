namespace DocoptNet
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    partial class Tokens: IEnumerable<string>
    {
        private readonly Type _errorType;
        private readonly Queue<string> _tokens;

        public Tokens(IEnumerable<string> source, Type errorType)
        {
            _errorType = errorType ?? typeof(DocoptInputErrorException);
            _tokens = new Queue<string>(source);
        }

        public Tokens(string source, Type errorType)
        {
            _errorType = errorType ?? typeof(DocoptInputErrorException);
            _tokens = new Queue<string>(source.Split(new char[0], StringSplitOptions.RemoveEmptyEntries));
        }

        public Type ErrorType
        {
            get { return _errorType; }
        }

        public bool ThrowsInputError
        {
            get { return ErrorType == typeof (DocoptInputErrorException); }
        }

        public static Tokens FromPattern(string pattern)
        {
            var spacedOut = Regex.Replace(pattern, @"([\[\]\(\)\|]|\.\.\.)", @" $1 ");
            var source = Regex.Split(spacedOut, @"\s+|(\S*<.*?>)").Where(x => !string.IsNullOrEmpty(x));
            return new Tokens(source, typeof(DocoptLanguageErrorException));
        }

        public IEnumerator<string> GetEnumerator()
        {
            return _tokens.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public string Move()
        {
            return _tokens.Count > 0 ? _tokens.Dequeue() : null;
        }

        public string Current()
        {
            return _tokens.Count > 0 ? _tokens.Peek() : null;
        }

        public Exception CreateException(string message)
        {
            return Activator.CreateInstance(_errorType, new object[] {message}) as Exception;
        }

        public override string ToString()
        {
            return $"current={Current()},count={_tokens.Count}";
        }
    }
}
