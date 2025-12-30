using System;

using ManagedShell.Common.Enums;

namespace ManagedShell.Common.SupportingClasses;

public class AppVisibilityEventArgs : EventArgs
{
    public IntPtr MonitorHandle { get; set; }
    public MonitorAppVisibility PreviousMode { get; set; }
    public MonitorAppVisibility CurrentMode { get; set; }
}
