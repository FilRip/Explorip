using System;

namespace CoolBytes.JumpList;

public class ExtensionsJumpListException : Exception
{
    public ExtensionsJumpListException() : base() { }

    public ExtensionsJumpListException(string message) : base(message) { }

    public ExtensionsJumpListException(string message, Exception innerException) : base(message, innerException) { }
}
