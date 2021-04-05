namespace DocoptNet
{
    using System;

    partial class DocoptBaseException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public DocoptBaseException()
        {
        }

        public DocoptBaseException(string message)
            : base(message)
        {
        }

        public DocoptBaseException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public int ErrorCode
        {
            get { return 1; }
        }
    }
}

#if RUNTIME_SERIALIZATION

namespace DocoptNet
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    partial class DocoptBaseException
    {
        protected DocoptBaseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

#endif
