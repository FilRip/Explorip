using System;

using ManagedShell.Interop;

namespace ManagedShell.AppBar;

public class FullScreenApp
{
    public IntPtr HWnd { get; set; }
    public ScreenInfo Screen { get; set; }
    public NativeMethods.Rect Rect { get; set; }
    public string Title { get; set; }
    public bool FromTasksService { get; set; }
}
