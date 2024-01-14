using System;

using ManagedShell.Common.Enums;

namespace ManagedShell.Common.SupportingClasses;

public class AppVisibilityEventArgs : EventArgs
{
    public IntPtr MonitorHandle { get; set; }
    public MONITOR_APP_VISIBILITY PreviousMode { get; set; }
    public MONITOR_APP_VISIBILITY CurrentMode { get; set; }
}
