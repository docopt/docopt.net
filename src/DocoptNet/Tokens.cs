namespace DocoptNet
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    partial class Tokens
    {
        static Tokens New<TError>(IEnumerable<string> source, Func<string, TError> errorFactory)
            where TError : DocoptBaseException =>
            new ContextualTokens<TError>(source, errorFactory);

        public static Tokens From(IEnumerable<string> source) =>
            New(source, msg => new DocoptInputErrorException(msg));

        public static Tokens FromPattern(string pattern)
        {
            var spacedOut = Regex.Replace(pattern, @"([\[\]\(\)\|]|\.\.\.)", @" $1 ");
            var source = Regex.Split(spacedOut, @"\s+|(\S*<.*?>)").Where(x => !string.IsNullOrEmpty(x));
            return New(source, msg => new DocoptLanguageErrorException(msg));
        }
    }

    abstract partial class Tokens : IEnumerable<string>
    {
        private readonly Queue<string> _tokens;

        protected Tokens(IEnumerable<string> source)
        {
            _tokens = new Queue<string>(source);
        }

        public abstract Type ErrorType { get; }

        public bool ThrowsInputError
        {
            get { return ErrorType == typeof (DocoptInputErrorException); }
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

        public abstract Exception CreateException(string message);

        public override string ToString()
        {
            return $"current={Current()},count={_tokens.Count}";
        }

        partial class ContextualTokens<TError> : Tokens where TError : Exception
        {
            private readonly Func<string, TError> _errorFactory;

            public ContextualTokens(IEnumerable<string> source, Func<string, TError> errorFactory) :
                base(source)
            {
                _errorFactory = errorFactory;
            }

            public override Type ErrorType
            {
                get { return typeof(TError); }
            }

            public override Exception CreateException(string message)
            {
                return _errorFactory(message);
            }
        }
    }
}
