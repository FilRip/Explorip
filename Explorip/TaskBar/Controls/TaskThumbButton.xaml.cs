using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

using ManagedShell.Common.Helpers;
using ManagedShell.Interop;

namespace Explorip.TaskBar.Controls;

/// <summary>
/// Logique d'interaction pour TaskThumbButton.xaml
/// </summary>
public partial class TaskThumbButton : Window
{
    private readonly TaskButton _parent;
    private IntPtr _handle;
    private readonly List<IntPtr> _thumbPtr;
    private const int ThumbWidth = 250;
    private IntPtr _lastPeek;
    private bool _showContextMenu;

    public TaskThumbButton(TaskButton parent)
    {
        InitializeComponent();
        _thumbPtr = [];
        Width = ThumbWidth;
        TitleFirst.Width = ThumbWidth;
        if (parent.ApplicationWindow.Handle == IntPtr.Zero && parent.ApplicationWindow.ListWindows.Count > 0)
        {
            Width *= parent.ApplicationWindow.ListWindows.Count;
        }
        _parent = parent;
        Owner = _parent.TaskbarParent;
        Point positionParent = _parent.PointToScreen(Mouse.GetPosition(this));
        Left = (int)((positionParent.X - (Width / 2)) / VisualTreeHelper.GetDpi(this).DpiScaleX);
        Top = _parent.TaskbarParent.Top - Height;
    }

    private void Window_MouseLeave(object sender, MouseEventArgs e)
    {
        MouseIn = false;
        if (_lastPeek != IntPtr.Zero)
            WindowHelper.PeekWindow(false, _lastPeek, _parent.TaskbarParent.Handle);
        Close();
    }

    public bool MouseIn { get; private set; }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        if (_showContextMenu)
        {
            e.Cancel = true;
            return;
        }
        foreach (IntPtr thumb in _thumbPtr)
            NativeMethods.DwmUnregisterThumbnail(thumb);
    }

    private void Window_ContentRendered(object sender, EventArgs e)
    {
        try
        {
            _handle = new WindowInteropHelper(this).EnsureHandle();
            WindowHelper.ExcludeWindowFromPeek(_handle);
            if (_parent.ApplicationWindow.Handle != IntPtr.Zero)
            {
                int result = NativeMethods.DwmRegisterThumbnail(_handle, _parent.ApplicationWindow.Handle, out IntPtr thumbPtr);
                if (result == (int)NativeMethods.HResult.SUCCESS)
                {
                    NativeMethods.DwmThumbnailProperties thumbProp = new()
                    {
                        dwFlags = NativeMethods.DWM_TNP.VISIBLE | NativeMethods.DWM_TNP.RECTDESTINATION | NativeMethods.DWM_TNP.OPACITY,
                        fVisible = true,
                        opacity = 255,
                        rcDestination = new NativeMethods.Rect() { Left = 0, Top = (int)(TitleFirst.ActualHeight * VisualTreeHelper.GetDpi(this).DpiScaleY), Right = (int)(Width * VisualTreeHelper.GetDpi(this).DpiScaleX), Bottom = (int)(Height * VisualTreeHelper.GetDpi(this).DpiScaleY) },
                    };
                    TitleFirst.Text = _parent.ApplicationWindow.Title;
                    NativeMethods.DwmUpdateThumbnailProperties(thumbPtr, ref thumbProp);
                    _thumbPtr.Add(thumbPtr);
                }
            }
            else if (_parent.ApplicationWindow.ListWindows.Count > 0)
            {
                for (int i = 0; i < _parent.ApplicationWindow.ListWindows.Count; i++)
                {
                    int result = NativeMethods.DwmRegisterThumbnail(_handle, _parent.ApplicationWindow.ListWindows[i], out IntPtr thumbPtr);
                    if (result == (int)NativeMethods.HResult.SUCCESS)
                    {
                        NativeMethods.DwmThumbnailProperties thumbProp = new()
                        {
                            dwFlags = NativeMethods.DWM_TNP.VISIBLE | NativeMethods.DWM_TNP.RECTDESTINATION | NativeMethods.DWM_TNP.OPACITY,
                            fVisible = true,
                            opacity = 255,
                            rcDestination = new NativeMethods.Rect() { Left = (int)(ThumbWidth * VisualTreeHelper.GetDpi(this).DpiScaleX * i), Top = (int)(TitleFirst.ActualHeight * VisualTreeHelper.GetDpi(this).DpiScaleY), Right = (int)(ThumbWidth * VisualTreeHelper.GetDpi(this).DpiScaleX) + (int)(ThumbWidth * VisualTreeHelper.GetDpi(this).DpiScaleX * i), Bottom = (int)(Height * VisualTreeHelper.GetDpi(this).DpiScaleY) },
                        };
                        StringBuilder sb = new(255);
                        NativeMethods.GetWindowText(_parent.ApplicationWindow.ListWindows[i], sb, 255);
                        if (i > 0)
                        {
                            TextBlock txtTitle = new()
                            {
                                Text = sb.ToString(),
                                Width = TitleFirst.Width,
                                HorizontalAlignment = HorizontalAlignment.Left,
                                Margin = new Thickness(TitleFirst.Width * i, 0, 0, 0),
                                Background = Constants.Colors.BackgroundColorBrush,
                                Foreground = Constants.Colors.ForegroundColorBrush,
                            };
                            MainGrid.Children.Add(txtTitle);
                        }
                        else
                            TitleFirst.Text = sb.ToString();
                        NativeMethods.DwmUpdateThumbnailProperties(thumbPtr, ref thumbProp);
                        _thumbPtr.Add(thumbPtr);
                    }
                }
            }
        }
        catch (Exception) { /* Ignore errors */ }
    }

    private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        _parent.ApplicationWindow.BringToFront(_lastPeek);
        this.Close();
    }

    private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        try
        {
            _showContextMenu = true;
            if (_lastPeek != IntPtr.Zero)
                WindowHelper.PeekWindow(false, _lastPeek, _parent.TaskbarParent.Handle);
            IntPtr wMenu = NativeMethods.GetSystemMenu(_lastPeek, false);
            // Display the menu
            Point posMouse = PointToScreen(Mouse.GetPosition(this));
            uint command = NativeMethods.TrackPopupMenuEx(wMenu,
                NativeMethods.TPM.RIGHTBUTTON | NativeMethods.TPM.RETURNCMD, (int)posMouse.X, (int)posMouse.Y, _handle, IntPtr.Zero);
            if (command != 0)
                NativeMethods.PostMessage(_lastPeek, NativeMethods.WM.SYSCOMMAND, new IntPtr(command), IntPtr.Zero);
        }
        finally
        {
            _lastPeek = IntPtr.Zero;
            _showContextMenu = false;
        }
        Close();
    }

    private void Window_MouseMove(object sender, MouseEventArgs e)
    {
        if (_showContextMenu)
            return;
        MouseIn = true;
        IntPtr newPeek = IntPtr.Zero;
        if (_parent.ApplicationWindow.Handle != IntPtr.Zero)
            newPeek = _parent.ApplicationWindow.Handle;
        else
        {
            Point p = Mouse.GetPosition(this);
            int index = (int)Math.Floor(p.X / ThumbWidth);
            if (index < _parent.ApplicationWindow.ListWindows.Count)
                newPeek = _parent.ApplicationWindow.ListWindows[index];
        }
        if (newPeek != _lastPeek)
        {
            if (_lastPeek != IntPtr.Zero)
                WindowHelper.PeekWindow(false, _lastPeek, _parent.TaskbarParent.Handle);
            _lastPeek = newPeek;
            if (newPeek != IntPtr.Zero)
                WindowHelper.PeekWindow(true, _lastPeek, _parent.TaskbarParent.Handle);
        }
    }
}
