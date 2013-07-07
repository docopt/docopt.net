using System;
using System.Runtime.Serialization;

namespace DocoptNet
{
    [Serializable]
    public class DocoptLanguageErrorException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public DocoptLanguageErrorException()
        {
        }
        public DocoptLanguageErrorException(string message) : base(message)
        {
        }
        public DocoptLanguageErrorException(string message, Exception inner) : base(message, inner)
        {
        }
        protected DocoptLanguageErrorException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}