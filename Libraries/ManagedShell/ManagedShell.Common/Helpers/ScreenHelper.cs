using System.Drawing;

using ManagedShell.Interop;

namespace ManagedShell.Common.Helpers;

public static class ScreenHelper
{
    public static Size PrimaryMonitorDeviceSize => new(NativeMethods.GetSystemMetrics(0), NativeMethods.GetSystemMetrics(1));
}
