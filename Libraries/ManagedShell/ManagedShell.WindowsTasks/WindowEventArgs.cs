using System;

namespace ManagedShell.WindowsTasks;

public class WindowEventArgs : EventArgs
{
    public ApplicationWindow Window { get; set; }
    public IntPtr Handle { get; set; }
}
