using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

using Explorip.WinAPI;

using ManagedShell.Common.Helpers;

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

        public TaskThumbButton(TaskButton parent)
        {
            InitializeComponent();
            _thumbPtr = new List<IntPtr>();
            Width = ThumbWidth;
            if (parent.ApplicationWindow.Handle == IntPtr.Zero && parent.ApplicationWindow.ListWindows.Count > 0)
            {
                Width *= parent.ApplicationWindow.ListWindows.Count;
            }
            _parent = parent;
            Owner = _parent.TaskbarParent;
            Point positionParent = _parent.PointToScreen(new Point(0, 0));
            Left = positionParent.X - (Width / 2);
            Top = positionParent.Y - Height;
        }

        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            MouseIn = true;
            WindowHelper.PeekWindow(true, _parent.ApplicationWindow.Handle, _parent.TaskbarParent.Handle);
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            MouseIn = false;
            WindowHelper.PeekWindow(false, _parent.ApplicationWindow.Handle, _parent.TaskbarParent.Handle);
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
                        rcDestination = new WinAPI.Modeles.Rect() { left = 0, top = 18, right = (int)Width, bottom = (int)Height },
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
                            rcDestination = new WinAPI.Modeles.Rect() { left = (ThumbWidth * i), top = 18, right = ThumbWidth + (ThumbWidth * i), bottom = (int)Height },
                        };
                        //TitleFirst.Text = _parent.ApplicationWindow.Title;
                        Dwmapi.DwmUpdateThumbnailProperties(thumbPtr, ref thumbProp);
                        _thumbPtr.Add(thumbPtr);
                    }
                }
            }
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _parent.ApplicationWindow.BringToFront();
            this.Close();
        }

        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            IntPtr wMenu = User32.GetSystemMenu(_parent.ApplicationWindow.Handle, false);
            // Display the menu
            Point posMouse = PointToScreen(Mouse.GetPosition(this));
            uint command = User32.TrackPopupMenuEx(wMenu,
                User32.TPM.LEFTBUTTON | User32.TPM.RETURNCMD, (int)posMouse.X, (int)posMouse.Y, _handle, IntPtr.Zero);
            if (command == 0)
                return;

            User32.PostMessage(_parent.ApplicationWindow.Handle, (uint)Commun.WM.SYSCOMMAND, new IntPtr(command), IntPtr.Zero);
        }
    }
}
