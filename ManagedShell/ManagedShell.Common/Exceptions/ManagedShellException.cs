using System;
using System.Runtime.Serialization;

namespace ManagedShell.Common.Exceptions
{
    [Serializable()]
    public class ManagedShellException : Exception
    {
        public ManagedShellException(string message) : base(message) { }

        protected ManagedShellException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
