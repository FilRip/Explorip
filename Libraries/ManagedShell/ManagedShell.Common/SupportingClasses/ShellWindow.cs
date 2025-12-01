using System;
using System.Windows.Forms;

using ManagedShell.Interop;

namespace ManagedShell.Common.SupportingClasses;

public class ShellWindow : NativeWindowEx, IDisposable
{
    public bool IsShellWindow { get; set; }
    public EventHandler WallpaperChanged { get; set; }
    public EventHandler WorkAreaChanged { get; set; }

    public ShellWindow()
    {
        CreateParams cp = new();
        cp.Style |= (int)NativeMethods.WindowStyles.WS_VISIBLE;
        cp.Style |= unchecked((int)NativeMethods.WindowStyles.WS_POPUP);
        cp.ExStyle |= (int)NativeMethods.ExtendedWindowStyles.WS_EX_TOOLWINDOW |
            (int)NativeMethods.ExtendedWindowStyles.WS_EX_NOACTIVATE;
        cp.Height = SystemInformation.VirtualScreen.Height;
        cp.Width = SystemInformation.VirtualScreen.Width;
        cp.X = SystemInformation.VirtualScreen.Left;
        cp.Y = SystemInformation.VirtualScreen.Top;

        Init(cp);
        MessageReceived += WndProc;
        NativeMethods.SetWindowLong(Handle, NativeMethods.EGetWindowLong.GWL_EXSTYLE,
            NativeMethods.GetWindowLong(Handle, NativeMethods.EGetWindowLong.GWL_EXSTYLE) &
            ~(int)NativeMethods.ExtendedWindowStyles.WS_EX_NOACTIVATE);

        if (NativeMethods.SetShellWindow(Handle) == 1)
        {
            // we did it
            IsShellWindow = true;
        }
    }

    private void Init(CreateParams cp)
    {
        CreateHandle(cp);
    }

    public void SetSize()
    {
        NativeMethods.SetWindowPos(Handle, IntPtr.Zero, SystemInformation.VirtualScreen.Left,
            SystemInformation.VirtualScreen.Top, SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height,
            NativeMethods.EShowWindowPos.SWP_NOZORDER | NativeMethods.EShowWindowPos.SWP_NOACTIVATE);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public bool IsDisposed
    {
        get { return _isDisposed; }
    }

    private bool _isDisposed;
    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                NativeMethods.DestroyWindow(Handle);
            }
            _isDisposed = disposing;
        }
    }

    private void WndProc(Message msg)
    {
        // Window procedure for the native window
        // Because other desktop windows are children, we need to pass them some received events.
        if (msg.Msg == (int)NativeMethods.WM.SETTINGCHANGE && msg.WParam.ToInt32() == (int)NativeMethods.ESystemParametersInfo.SETWORKAREA)
        {
            WorkAreaChanged?.Invoke(this, new EventArgs());
        }
        else if (msg.Msg == (int)NativeMethods.WM.SETTINGCHANGE && msg.WParam.ToInt32() == (int)NativeMethods.ESystemParametersInfo.SETDESKWALLPAPER)
        {
            WallpaperChanged?.Invoke(this, new EventArgs());
            msg.Result = new IntPtr(NativeMethods.MA_NOACTIVATE);
        }
    }
}
