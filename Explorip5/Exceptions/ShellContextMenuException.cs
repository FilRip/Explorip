using System;
using System.Runtime.Serialization;

namespace Explorip.Exceptions
{
    [Serializable()]
    public class ShellContextMenuException : ExploripException
    {
        public ShellContextMenuException(string message) : base(message)
        {
        }

        protected ShellContextMenuException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
