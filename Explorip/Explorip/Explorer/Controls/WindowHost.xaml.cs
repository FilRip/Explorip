using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

using Explorip.Helpers;

using ManagedShell.Interop;

namespace Explorip.Explorer.Controls;

/// <summary>
/// Logique d'interaction pour ConsoleHost.xaml
/// </summary>
public partial class WindowHost : UserControl, IDisposable
{
    private const int OFFSET_X = 6;
    private const int OFFSET_Y = 62;
    private const int OFFSET_SIZE_HEIGHT = 24;
    private IntPtr _srcPtr, _destPtr;
    private int _previousStyle;

    public WindowHost()
    {
        InitializeComponent();
    }

    public void InitRedirectWindow(IntPtr ptr)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            if (ptr == IntPtr.Zero)
                return;
            _srcPtr = ptr;
            _destPtr = (PresentationSource.FromVisual(this) as HwndSource).Handle;
            NativeMethods.SetParent(_srcPtr, _destPtr);
            UserControl_SizeChanged(this, null);
            int currentStyle = NativeMethods.GetWindowLong(_srcPtr, NativeMethods.EGetWindowLong.GWL_STYLE);
            _previousStyle = currentStyle;
            NativeMethods.SetWindowLong(_srcPtr, NativeMethods.EGetWindowLong.GWL_STYLE, (currentStyle & ~(int)NativeMethods.WindowStyles.WS_BORDER & ~(int)NativeMethods.WindowStyles.WS_SIZEBOX));
            NativeMethods.GetWindowThreadProcessId(_srcPtr, out uint pid);
            Task.Run(() => CheckExited((int)pid));
        });
    }

    private void CheckExited(int pid)
    {
        Application.Current.Dispatcher.Invoke(async () =>
        {
            TabItemWindowEmbedded myTab = this.FindControlParent<TabItemWindowEmbedded>();
            while (!myTab.IsDisposed && !myTab.MyDataContext.IsDisposed && myTab.MyDataContext.Enabled)
            {
                try
                {
                    if (Process.GetProcessById(pid) == null)
                    {
                        throw new Exceptions.ExploripException();
                    }
                }
                catch (Exception)
                {
                    myTab.Reset();
                    break;
                }
                await Task.Delay(10);
            }
        });
    }

    public void SetFocus()
    {
        Task.Run(() =>
        {
            Thread.Sleep(100);
            NativeMethods.SetForegroundWindow(_srcPtr);
            NativeMethods.SetCapture(_srcPtr);
            NativeMethods.SetFocus(_srcPtr);
            NativeMethods.SetActiveWindow(_srcPtr);
            NativeMethods.EnableWindow(_srcPtr, 1);
        });
    }

    private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (_srcPtr != IntPtr.Zero)
            NativeMethods.SetWindowPos(_srcPtr, IntPtr.Zero, (int)(OFFSET_X * VisualTreeHelper.GetDpi(this).DpiScaleX) + (int)((this.FindVisualParent<TabExplorerBrowser>().GetVisualOffset().X) * VisualTreeHelper.GetDpi(this).DpiScaleX), (int)(OFFSET_Y * VisualTreeHelper.GetDpi(this).DpiScaleY), (int)((ActualWidth - OFFSET_X) * VisualTreeHelper.GetDpi(this).DpiScaleX), (int)((ActualHeight - OFFSET_SIZE_HEIGHT) * VisualTreeHelper.GetDpi(this).DpiScaleY), NativeMethods.EShowWindowPos.SWP_SHOWWINDOW | NativeMethods.EShowWindowPos.SWP_NOACTIVATE);
    }

    public void Show()
    {
        NativeMethods.ShowWindow(_srcPtr, NativeMethods.WindowShowStyle.ShowNormal);
        SetFocus();
    }

    public void Hide()
    {
        NativeMethods.ShowWindow(_srcPtr, NativeMethods.WindowShowStyle.Hide);
    }

    public IntPtr WindowSource
    {
        get { return _srcPtr; }
    }

    public IntPtr WindowDestination
    {
        get { return _destPtr; }
    }

    public void Detach()
    {
        if (_srcPtr != IntPtr.Zero)
        {
            NativeMethods.SetWindowLong(_srcPtr, NativeMethods.EGetWindowLong.GWL_STYLE, _previousStyle);
            NativeMethods.SetParent(_srcPtr, IntPtr.Zero);
            _srcPtr = IntPtr.Zero;
        }
    }

    #region IDisposable

    private bool disposedValue;

    public bool IsDisposed
    {
        get { return disposedValue; }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                Detach();
            }
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion
}
