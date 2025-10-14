using System;
using System.Windows;

namespace Monitorian.Models;

public class HandleItem(
    int displayIndex,
    Rect monitorRect,
    IntPtr monitorHandle)
{
    public int DisplayIndex { get; } = displayIndex;

    public Rect MonitorRect { get; } = monitorRect;

    public IntPtr MonitorHandle { get; } = monitorHandle;
}
