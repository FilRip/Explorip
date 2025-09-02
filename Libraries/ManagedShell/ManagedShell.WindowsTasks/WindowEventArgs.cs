using System;

namespace ManagedShell.WindowsTasks;

public class WindowEventArgs : EventArgs
{
    public ApplicationWindow Window { get; set; } = null;
    public IntPtr Handle { get; set; } = IntPtr.Zero;
}
