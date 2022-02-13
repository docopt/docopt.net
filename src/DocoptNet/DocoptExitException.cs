#nullable enable

namespace DocoptNet
{
    using System;

    partial class DocoptExitException : DocoptBaseException
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public DocoptExitException()
        {
        }
        public DocoptExitException(string? message) : base(message)
        {
        }
        public DocoptExitException(string? message, Exception? inner) : base(message, inner)
        {
        }
    }
}

#if RUNTIME_SERIALIZATION

namespace DocoptNet
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    partial class DocoptExitException
    {
        protected DocoptExitException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

#endif
