using System;
using System.Runtime.Serialization;

namespace CoolBytes.Scripting.Exceptions;

[Serializable()]
public class CoolBytesScriptingException : Exception
{
    public CoolBytesScriptingException(string message) : base(message) { }

    protected CoolBytesScriptingException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}
