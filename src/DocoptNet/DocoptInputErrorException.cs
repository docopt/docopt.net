// Licensed under terms of MIT license (see LICENSE-MIT)
// Copyright 2013 Dinh Doan Van Bien, 2021 Atif Aziz

namespace DocoptNet
{
    using System;

    partial class DocoptInputErrorException : DocoptBaseException
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public DocoptInputErrorException() { }
        public DocoptInputErrorException(string? message) : base(message) { }
        public DocoptInputErrorException(string? message, Exception? inner) : base(message, inner) { }
    }
}

#if RUNTIME_SERIALIZATION

namespace DocoptNet
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    partial class DocoptInputErrorException
    {
        protected DocoptInputErrorException(SerializationInfo info, StreamingContext context) :
            base(info, context) { }
    }
}

#endif
