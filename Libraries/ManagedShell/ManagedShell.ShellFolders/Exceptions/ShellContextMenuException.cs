using System;
using System.Runtime.Serialization;

namespace ManagedShell.ShellFolders.Exceptions
{
    [Serializable()]
    public class ShellContextMenuException : Exception
    {
        public ShellContextMenuException(string message) : base(message) { }

        protected ShellContextMenuException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
