using System;
using System.Runtime.Serialization;

namespace Explorip.Exceptions
{
    [Serializable()]
    public class ExploripException : Exception
    {
        public ExploripException() : base()
        {
        }

        public ExploripException(string message) : base(message)
        {
        }

        protected ExploripException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
