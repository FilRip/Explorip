using System;
using System.Runtime.Serialization;

namespace CoolBytes.Scripting.Exceptions;

[Serializable()]
public class LanguageNotSupportedException : CoolBytesScriptingException
{
    /// <inheritdoc/>
    public LanguageNotSupportedException() : base(Properties.Resources.LANGUAGE_NOT_SUPPORTED) { }

    /// <inheritdoc/>
    protected LanguageNotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}
