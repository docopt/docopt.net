using System;
using System.Runtime.Serialization;

namespace DocoptNet
{
#if NET40
    [Serializable]
#endif
    public class DocoptBaseException : Exception
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

#if NET40
        protected DocoptBaseException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
#endif

        public int ErrorCode
        {
            get { return 1; }
        }
    }
}