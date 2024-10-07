using System;

namespace ExploripComponents;

public class ExploripCommonException : Exception
{
    public ExploripCommonException() : base() { }

    public ExploripCommonException(string message) : base(message) { }

    public ExploripCommonException(string message, Exception ex) : base(message, ex) { }
}
