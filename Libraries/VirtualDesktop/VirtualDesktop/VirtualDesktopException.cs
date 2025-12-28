using System;

namespace VirtualDesktop;

public class VirtualDesktopException : Exception
{
    public VirtualDesktopException()
    {
    }
    public VirtualDesktopException(string? message) : base(message)
    {
    }
    public VirtualDesktopException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
