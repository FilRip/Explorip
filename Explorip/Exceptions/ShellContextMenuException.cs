using System;

namespace Explorip.Exceptions
{
    [Serializable()]
    public class ShellContextMenuException : ExploripException
    {
        public ShellContextMenuException(string message) : base(message)
        {
        }
    }
}
