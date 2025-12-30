using System;

namespace ManagedShell.WindowsTasks;

public class FullScreenEventArgs : EventArgs
{
    public IntPtr Handle { get; set; }
    public bool IsEntering { get; set; }

    public bool IsExiting
    {
        get { return !IsEntering; }
    }

    public string Title { get; set; }

    public uint ProcessId { get; set; }
}
