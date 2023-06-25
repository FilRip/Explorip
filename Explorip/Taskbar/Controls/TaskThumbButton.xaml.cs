using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

using Explorip.WinAPI;

using ManagedShell.Common.Helpers;
using ManagedShell.Interop;

namespace Explorip.TaskBar.Controls
{
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

        public TaskThumbButton(TaskButton parent)
        {
            InitializeComponent();
            _thumbPtr = new List<IntPtr>();
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
            foreach (IntPtr thumb in _thumbPtr)
                Dwmapi.DwmUnregisterThumbnail(thumb);
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            _handle = new WindowInteropHelper(this).Handle;
            WindowHelper.ExcludeWindowFromPeek(_handle);
            if (_parent.ApplicationWindow.Handle != IntPtr.Zero)
            {
                int result = Dwmapi.DwmRegisterThumbnail(_handle, _parent.ApplicationWindow.Handle, out IntPtr thumbPtr);
                if (result == (int)Commun.HRESULT.S_OK)
                {
                    WinAPI.Modeles.DwmThumbnailProperties thumbProp = new()
                    {
                        dwFlags = WinAPI.Modeles.DWM_TNP.VISIBLE | WinAPI.Modeles.DWM_TNP.RECTDESTINATION | WinAPI.Modeles.DWM_TNP.OPACITY,
                        fVisible = true,
                        opacity = 255,
                        rcDestination = new WinAPI.Modeles.Rect() { left = 0, top = (int)(TitleFirst.ActualHeight * VisualTreeHelper.GetDpi(this).DpiScaleY), right = (int)(Width * VisualTreeHelper.GetDpi(this).DpiScaleX), bottom = (int)(Height * VisualTreeHelper.GetDpi(this).DpiScaleY) },
                    };
                    TitleFirst.Text = _parent.ApplicationWindow.Title;
                    Dwmapi.DwmUpdateThumbnailProperties(thumbPtr, ref thumbProp);
                    _thumbPtr.Add(thumbPtr);
                }
            }
            else if (_parent.ApplicationWindow.ListWindows.Count > 0)
            {
                for (int i = 0; i < _parent.ApplicationWindow.ListWindows.Count; i++)
                {
                    int result = Dwmapi.DwmRegisterThumbnail(_handle, _parent.ApplicationWindow.ListWindows[i], out IntPtr thumbPtr);
                    if (result == (int)Commun.HRESULT.S_OK)
                    {
                        WinAPI.Modeles.DwmThumbnailProperties thumbProp = new()
                        {
                            dwFlags = WinAPI.Modeles.DWM_TNP.VISIBLE | WinAPI.Modeles.DWM_TNP.RECTDESTINATION | WinAPI.Modeles.DWM_TNP.OPACITY,
                            fVisible = true,
                            opacity = 255,
                            rcDestination = new WinAPI.Modeles.Rect() { left = (int)(ThumbWidth * VisualTreeHelper.GetDpi(this).DpiScaleX * i), top = (int)(TitleFirst.ActualHeight * VisualTreeHelper.GetDpi(this).DpiScaleY), right = (int)(ThumbWidth * VisualTreeHelper.GetDpi(this).DpiScaleX) + (int)(ThumbWidth * VisualTreeHelper.GetDpi(this).DpiScaleX * i), bottom = (int)(Height * VisualTreeHelper.GetDpi(this).DpiScaleY) },
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
                        Dwmapi.DwmUpdateThumbnailProperties(thumbPtr, ref thumbProp);
                        _thumbPtr.Add(thumbPtr);
                    }
                }
            }
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _parent.ApplicationWindow.BringToFront(_lastPeek);
            this.Close();
        }

        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            IntPtr wMenu = User32.GetSystemMenu(_lastPeek, false);
            // Display the menu
            Point posMouse = PointToScreen(Mouse.GetPosition(this));
            uint command = User32.TrackPopupMenuEx(wMenu,
                User32.TPM.LEFTBUTTON | User32.TPM.RETURNCMD, (int)posMouse.X, (int)posMouse.Y, _handle, IntPtr.Zero);
            if (command == 0)
                return;

            User32.PostMessage(_lastPeek, (uint)Commun.WM.SYSCOMMAND, new IntPtr(command), IntPtr.Zero);
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            MouseIn = true;
            IntPtr newPeek = IntPtr.Zero;
            if (_parent.ApplicationWindow.Handle != IntPtr.Zero)
                newPeek = _parent.ApplicationWindow.Handle;
            else
            {
                Point p = Mouse.GetPosition(this);
                int index = (int)Math.Floor(p.X / ThumbWidth);
                if (index <= _parent.ApplicationWindow.ListWindows.Count)
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
}
