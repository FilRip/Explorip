using System;
using System.Runtime.Serialization;

namespace ExploripCopy.Exceptions;

[Serializable()]
public class ExploripCopyException : Exception
{
    public ExploripCopyException(string message) : base(message) { }

    protected ExploripCopyException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}
