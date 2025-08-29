using System;

namespace ManagedShell.WindowsTasks;

public class FullScreenEventArgs : EventArgs
{
    public IntPtr Handle { get; set; }
    public bool IsEntering { get; set; }
}
