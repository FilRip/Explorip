using System;
using System.Runtime.Serialization;

namespace CustomWinForm
{
    [Serializable()]
    public class CustomWinFormException : Exception
    {
        public CustomWinFormException(string message) : base(message) { }

        protected CustomWinFormException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
