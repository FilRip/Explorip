using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Explorip.TaskBar.Controls;

using ExploripConfig.Configuration;

using ManagedShell.Common.Helpers;
using ManagedShell.Interop;

namespace Explorip.TaskBar.ViewModels;

public partial class TaskThumbButtonViewModel : ObservableObject, IDisposable
{
    private readonly Timer _timerBeforePeek;
    private bool disposedValue;
    private readonly int _delayBeforePreview;
    private IntPtr _lastPeeked = IntPtr.Zero;
    private int _currentWindow = -1;
    private readonly List<IntPtr> _thumbPtr;

    public TaskThumbButtonViewModel()
    {
        _timerBeforePeek = new Timer(ShowPreviewWindow, null, Timeout.Infinite, Timeout.Infinite);
        _delayBeforePreview = ConfigManager.TaskbarDelayBeforeShowThumbnail;
        _thumbPtr = [];
        ListThumbnailButtons = [];
    }

    [ObservableProperty()]
    private double _thumbHeight, _thumbWidth;

    public IntPtr WindowHandle { get; set; }
    public bool MouseIn { get; private set; }
    public bool ShowContextMenu { get; private set; }
    public TaskButton ParentTask { get; set; }
    public double SpaceBetweenThumbnail { get; set; }
    public List<Button> ListThumbnailButtons { get; private set; }
    public TaskThumbButton ParentControl { get; set; }

    public void ShowPreviewWindow(object userData)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            if (_currentWindow < 0)
            {
                _timerBeforePeek.Change(_delayBeforePreview, Timeout.Infinite);
                return;
            }
            IntPtr newPeek;
            newPeek = ParentTask.ApplicationWindow.ListWindows[_currentWindow];
            if (newPeek != _lastPeeked)
            {
                UnPeek();
                _lastPeeked = newPeek;
                if (newPeek != IntPtr.Zero)
                    WindowHelper.PeekWindow(true, _lastPeeked, ParentTask.TaskbarParent.Handle);
            }
        });
    }

    public void UnPeek()
    {
        if (_lastPeeked != IntPtr.Zero)
            WindowHelper.PeekWindow(false, _lastPeeked, ParentTask.TaskbarParent.Handle);
    }

    public void CloseWindow(int numWindow)
    {
        IntPtr windowHandle = ParentTask.ApplicationWindow.ListWindows[numWindow];
        if (windowHandle == IntPtr.Zero)
            return;
        UnPeek();
        NativeMethods.SendMessage(windowHandle, NativeMethods.WM.CLOSE, 0, 0);
        ParentControl.Close();
    }

    [RelayCommand()]
    private void MouseLeave()
    {
        if (ShowContextMenu)
            return;
        MouseIn = false;
        UnPeek();
        ParentControl.Close();
    }

    [RelayCommand()]
    private void ClickWindow()
    {
        _timerBeforePeek.Change(Timeout.Infinite, Timeout.Infinite);
        if (_currentWindow < 0)
        {
            IInputElement control = Mouse.DirectlyOver;
            if (control is Button btn &&
                btn.Tag is int numWindow)
            {
                _currentWindow = numWindow;
            }
            else
                return;
        }
        IntPtr window;
        window = ParentTask.ApplicationWindow.ListWindows[_currentWindow];
        UnPeek();
        if (window != IntPtr.Zero)
            ParentTask.ApplicationWindow.BringToFront(window);
        ParentControl.Close();
    }

    public void MouseRightButtonDown()
    {
        try
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (_currentWindow < 0)
                {
                    IInputElement control = Mouse.DirectlyOver;
                    if (control is Button btn &&
                        btn.Tag is int numWindow)
                    {
                        _currentWindow = numWindow;
                    }
                    else
                        return;
                }
                _timerBeforePeek.Change(Timeout.Infinite, Timeout.Infinite);
                IntPtr window;
                window = ParentTask.ApplicationWindow.ListWindows[_currentWindow];
                ShowContextMenu = true;
                UnPeek();
                System.Drawing.Point posMouse = new();
                NativeMethods.GetCursorPos(ref posMouse);
                IntPtr wMenu = NativeMethods.GetSystemMenu(window, false);
                // Display the menu
                uint command = NativeMethods.TrackPopupMenuEx(wMenu,
                    NativeMethods.TrackPopUpMenuActions.RIGHTBUTTON | NativeMethods.TrackPopUpMenuActions.RETURNCMD | NativeMethods.TrackPopUpMenuActions.NONOTIFY, posMouse.X, posMouse.Y, WindowHandle, IntPtr.Zero);
                if (command != 0)
                    NativeMethods.PostMessage(window, NativeMethods.WM.SYSCOMMAND, new IntPtr(command), IntPtr.Zero);
            });
        }
        finally
        {
            ShowContextMenu = false;
        }
        ParentControl.Close();
    }

    public void MouseEnter(int numWindow)
    {
        MouseIn = true;
        _currentWindow = numWindow;
        if (_lastPeeked != IntPtr.Zero)
            ShowPreviewWindow(null);
        else
            _timerBeforePeek.Change(_delayBeforePreview, Timeout.Infinite);
    }

    [RelayCommand()]
    private void ContentRendered()
    {
        try
        {
            WindowHelper.ExcludeWindowFromPeek(WindowHandle);
            if (ParentTask.ApplicationWindow.ListWindows.Count > 0)
            {
                double currentLeft = 0;
                for (int i = 0; i < ParentTask.ApplicationWindow.ListWindows.Count; i++)
                {
                    currentLeft += SpaceBetweenThumbnail;
                    int result = NativeMethods.DwmRegisterThumbnail(WindowHandle, ParentTask.ApplicationWindow.ListWindows[i], out IntPtr thumbPtr);
                    if (result == (int)NativeMethods.HResult.SUCCESS)
                    {
                        Point buttonPosition = ListThumbnailButtons[i].TransformToAncestor(ParentControl).Transform(new Point(0, 0));

                        NativeMethods.DwmThumbnailProperties thumbProp = new()
                        {
                            dwFlags = NativeMethods.DWM_TNP.VISIBLE | NativeMethods.DWM_TNP.RECTDESTINATION | NativeMethods.DWM_TNP.OPACITY,
                            fVisible = true,
                            opacity = 255,
                            rcDestination = new NativeMethods.Rect()
                            {
                                Left = (int)(currentLeft * VisualTreeHelper.GetDpi(ParentControl).DpiScaleX),
                                Top = (int)(buttonPosition.Y * VisualTreeHelper.GetDpi(ParentControl).DpiScaleY),
                                Right = (int)(ThumbWidth * VisualTreeHelper.GetDpi(ParentControl).DpiScaleX) + (int)(currentLeft * VisualTreeHelper.GetDpi(ParentControl).DpiScaleX),
                                Bottom = (int)(ThumbHeight * VisualTreeHelper.GetDpi(ParentControl).DpiScaleY) + (int)(buttonPosition.Y * VisualTreeHelper.GetDpi(ParentControl).DpiScaleY),
                            }
                        };

                        currentLeft += ThumbWidth + SpaceBetweenThumbnail;

                        NativeMethods.DwmUpdateThumbnailProperties(thumbPtr, ref thumbProp);
                        _thumbPtr.Add(thumbPtr);
                    }
                }
            }
        }
        catch (Exception) { /* Ignore errors */ }
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
                foreach (IntPtr thumb in _thumbPtr)
                    NativeMethods.DwmUnregisterThumbnail(thumb);
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
