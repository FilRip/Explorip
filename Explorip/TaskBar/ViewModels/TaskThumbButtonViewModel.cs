using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Explorip.TaskBar.Controls;

using ExploripConfig.Configuration;

using ManagedShell.Common.Helpers;
using ManagedShell.Common.Logging;
using ManagedShell.Interop;

namespace Explorip.TaskBar.ViewModels;

public partial class TaskThumbButtonViewModel : ObservableObject, IDisposable
{
    private readonly Timer _timerBeforePeek;
    private bool disposedValue;

    public TaskThumbButtonViewModel()
    {
        _timerBeforePeek = new Timer(ShowPreviewWindow, null, Timeout.Infinite, Timeout.Infinite);
    }

    [ObservableProperty()]
    private double _thumbHeight, _thumbWidth;

    public Action CloseThumbnail { get; set; }
    public IntPtr WindowHandle { get; set; }
    public bool MouseIn { get; private set; }
    public bool ShowContextMenu { get; private set; }
    public TaskButton ParentTask { get; set; }
    public IntPtr LastPeeked { get; set; } = IntPtr.Zero;
    public int CurrentWindow { get; set; } = -1;

    public void ShowPreviewWindow(object userData)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            if (CurrentWindow < 0)
            {
                _timerBeforePeek.Change(ConfigManager.TaskbarDelayBeforeShowThumbnail, Timeout.Infinite);
                return;
            }
            IntPtr newPeek;
            newPeek = ParentTask.ApplicationWindow.ListWindows[CurrentWindow];
            if (newPeek != LastPeeked)
            {
                UnPeek();
                LastPeeked = newPeek;
                if (newPeek != IntPtr.Zero)
                    WindowHelper.PeekWindow(true, LastPeeked, ParentTask.TaskbarParent.Handle);
            }
        });
    }

    public void UnPeek()
    {
        if (LastPeeked != IntPtr.Zero)
            WindowHelper.PeekWindow(false, LastPeeked, ParentTask.TaskbarParent.Handle);
    }

    [RelayCommand()]
    private void MouseEnter()
    {
        MouseIn = true;
        if (ShowContextMenu)
            return;
        if (LastPeeked == IntPtr.Zero)
            _timerBeforePeek.Change(ConfigManager.TaskbarDelayBeforeShowThumbnail, Timeout.Infinite);
        else
            ShowPreviewWindow(null);
    }

    [RelayCommand()]
    private void MouseLeave()
    {
        if (ShowContextMenu)
            return;
        MouseIn = false;
        UnPeek();
        CloseThumbnail();
    }

    public void ClickWindow()
    {
        _timerBeforePeek.Change(Timeout.Infinite, Timeout.Infinite);
        if (CurrentWindow < 0)
        {
            IInputElement control = Mouse.DirectlyOver;
            if (control is Button btn &&
                btn.Tag is int numWindow)
            {
                CurrentWindow = numWindow;
            }
            else
                return;
        }
        IntPtr window;
        window = ParentTask.ApplicationWindow.ListWindows[CurrentWindow];
        UnPeek();
        if (window != IntPtr.Zero)
            ParentTask.ApplicationWindow.BringToFront(window);
        CloseThumbnail();
    }

    public void MouseRightButtonDown()
    {
        try
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (CurrentWindow < 0)
                {
                    IInputElement control = Mouse.DirectlyOver;
                    if (control is Button btn &&
                        btn.Tag is int numWindow)
                    {
                        CurrentWindow = numWindow;
                    }
                    else
                        return;
                }
                _timerBeforePeek.Change(Timeout.Infinite, Timeout.Infinite);
                IntPtr window;
                window = ParentTask.ApplicationWindow.ListWindows[CurrentWindow];
                ShowContextMenu = true;
                UnPeek();
                System.Drawing.Point posMouse = new();
                NativeMethods.GetCursorPos(ref posMouse);
                IntPtr wMenu = NativeMethods.GetSystemMenu(window, false);
                // Display the menu
                uint command = NativeMethods.TrackPopupMenuEx(wMenu,
                    NativeMethods.TPM.RIGHTBUTTON | NativeMethods.TPM.RETURNCMD | NativeMethods.TPM.NONOTIFY, posMouse.X, posMouse.Y, WindowHandle, IntPtr.Zero);
                ShellLogger.Debug("GetLastError=" + NativeMethods.GetLastError());
                if (command != 0)
                    NativeMethods.PostMessage(window, NativeMethods.WM.SYSCOMMAND, new IntPtr(command), IntPtr.Zero);
            });
        }
        finally
        {
            ShowContextMenu = false;
        }
        CloseThumbnail();
    }

    #region IDisposable Support

    public bool IsDisposed
    {
        get => disposedValue;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                UnPeek();
                _timerBeforePeek.Dispose();
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
