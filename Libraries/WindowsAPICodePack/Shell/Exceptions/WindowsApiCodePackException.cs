using System;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAPICodePack.Shell.Exceptions;

[Serializable()]
public class WindowsApiCodePackException : Exception
{
    public WindowsApiCodePackException(string message) : base(message) { }

    protected WindowsApiCodePackException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}
