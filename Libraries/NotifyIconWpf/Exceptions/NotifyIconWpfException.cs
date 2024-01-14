using System;
using System.Runtime.Serialization;

namespace Hardcodet.Wpf.TaskbarNotification.Exceptions;

[Serializable()]
public class NotifyIconWpfException : Exception
{
    public NotifyIconWpfException(string message) : base(message) { }

    protected NotifyIconWpfException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}
