using System;
using System.Runtime.Serialization;

namespace WindowsDesktop.Exceptions
{
    [Serializable()]
    public class VirtualDesktopException : Exception
    {
        public VirtualDesktopException(string message) : base(message) { }

        protected VirtualDesktopException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
