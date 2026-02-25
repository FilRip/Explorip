using System;
using System.Runtime.InteropServices;

using ManagedShell.Common.Enums;
using ManagedShell.Common.Interfaces;

namespace ManagedShell.Common.SupportingClasses;

internal class AppVisibilityEvents : IAppVisibilityEvents
{
    internal event EventHandler<AppVisibilityEventArgs> AppVisibilityChanged;
    internal event EventHandler<LauncherVisibilityEventArgs> LauncherVisibilityChanged;

    public AppVisibilityEvents() { }

    public long AppVisibilityOnMonitorChanged([In()] IntPtr hMonitor, [In()] MonitorAppVisibility previousMode, [In()] MonitorAppVisibility currentMode)
    {
        AppVisibilityEventArgs args = new()
        {
            MonitorHandle = hMonitor,
            PreviousMode = previousMode,
            CurrentMode = currentMode
        };

        AppVisibilityChanged?.Invoke(this, args);
        return 0;
    }

    public long LauncherVisibilityChange([In()] bool currentVisibleState)
    {
        LauncherVisibilityEventArgs args = new()
        {
            Visible = currentVisibleState
        };

        LauncherVisibilityChanged?.Invoke(this, args);
        return 0;
    }
}
