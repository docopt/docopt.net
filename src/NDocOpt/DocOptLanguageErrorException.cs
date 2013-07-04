using System;
using System.Runtime.Serialization;

namespace NDocOpt
{
    [Serializable]
    public class DocOptLanguageErrorException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public DocOptLanguageErrorException()
        {
        }
        public DocOptLanguageErrorException(string message) : base(message)
        {
        }
        public DocOptLanguageErrorException(string message, Exception inner) : base(message, inner)
        {
        }
        protected DocOptLanguageErrorException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}