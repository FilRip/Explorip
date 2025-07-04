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
using ManagedShell.Interop;

namespace Explorip.TaskBar.ViewModels;

public partial class TaskThumbButtonViewModel : ObservableObject
{
    private readonly Timer _timerBeforePeek;

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

    private void ShowPreviewWindow(object userData)
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

    public void MouseLeftButtonUp()
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
        if (window != IntPtr.Zero)
            ParentTask.ApplicationWindow.BringToFront(window);
        CloseThumbnail();
    }

    public void MouseRightButtonDown()
    {
        try
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
            ShowContextMenu = true;
            UnPeek();
            IntPtr wMenu = NativeMethods.GetSystemMenu(window, false);
            // Display the menu
            System.Drawing.Point posMouse = new();
            NativeMethods.GetCursorPos(ref posMouse);
            uint command = NativeMethods.TrackPopupMenuEx(wMenu,
                NativeMethods.TPM.RIGHTBUTTON | NativeMethods.TPM.RETURNCMD, posMouse.X, posMouse.Y, WindowHandle, IntPtr.Zero);
            if (command != 0)
                NativeMethods.PostMessage(window, NativeMethods.WM.SYSCOMMAND, new IntPtr(command), IntPtr.Zero);
        }
        finally
        {
            ShowContextMenu = false;
        }
        CloseThumbnail();
    }

    public void Close()
    {
        _timerBeforePeek.Dispose();
    }
}
