using System;
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
        private IntPtr _thumbPtr;
        private WinAPI.Modeles.DwmThumbnailProperties _thumb;

        public TaskThumbButton(TaskButton parent)
        {
            InitializeComponent();
            _parent = parent;
            Owner = _parent.TaskbarParent;
            Point positionParent = _parent.PointToScreen(new Point(0, 0));
            Left = positionParent.X - (Width / 2);
            Top = positionParent.Y - (Height);
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
        }

        public bool MouseIn { get; private set; }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_thumbPtr != IntPtr.Zero)
            {
                Dwmapi.DwmUnregisterThumbnail(_thumbPtr);
            }
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            WindowInteropHelper helper = new(this);
            _handle = helper.Handle;
            WindowHelper.ExcludeWindowFromPeek(_handle);
            int result = Dwmapi.DwmRegisterThumbnail(_handle, _parent.ApplicationWindow.Handle, out _thumbPtr);
            if (result == (int)Commun.HRESULT.S_OK)
            {
                _thumb = new WinAPI.Modeles.DwmThumbnailProperties()
                {
                    dwFlags = WinAPI.Modeles.DWM_TNP.VISIBLE | WinAPI.Modeles.DWM_TNP.RECTDESTINATION | WinAPI.Modeles.DWM_TNP.OPACITY,
                    fVisible = true,
                    opacity = 255,
                    rcDestination = new WinAPI.Modeles.Rect() { left = 0, top = 0, right = (int)Width, bottom = (int)Height },
                };
                Dwmapi.DwmUpdateThumbnailProperties(_thumbPtr, ref _thumb);
            }
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _parent.ApplicationWindow.BringToFront();
            this.Close();
        }

        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Display system menu for the selected window/app
        }
    }
}
