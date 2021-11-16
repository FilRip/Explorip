using System;
using System.Windows;
using System.Windows.Controls;
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
        private WinAPI.Modeles.DWM_THUMBNAIL_PROPERTIES _thumb;

        public TaskThumbButton(TaskButton parent)
        {
            InitializeComponent();
            _parent = parent;
            Point positionParent = _parent.PointToScreen(new Point(0, 0));
            Left = positionParent.X - (Width / 2);
            Top = positionParent.Y - (Height);
        }

        private void Window_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // TODO : Bascule 'temporaire' sur cette fenetre (premier plan en minimisant toutes les autres)
            //WindowHelper.PeekWindow(true, _parent.ApplicationWindow.Handle, _parent.TaskbarParent.Handle);
        }

        private void Window_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // TODO : Annuler bascule 'temporaire' sur cette fenetre
            //WindowHelper.PeekWindow(false, _parent.ApplicationWindow.Handle, _parent.TaskbarParent.Handle);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_thumbPtr != IntPtr.Zero)
            {
                Dwmapi.DwmUnregisterThumbnail(_thumbPtr);
            }
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            WindowInteropHelper helper = new WindowInteropHelper(this);
            _handle = helper.Handle;
            int result = Dwmapi.DwmRegisterThumbnail(_handle, _parent.ApplicationWindow.Handle, out _thumbPtr);
            if (result == (int)Commun.HRESULT.S_OK)
            {
                _thumb = new WinAPI.Modeles.DWM_THUMBNAIL_PROPERTIES()
                {
                    dwFlags = WinAPI.Modeles.DWM_TNP.VISIBLE | WinAPI.Modeles.DWM_TNP.RECTDESTINATION | WinAPI.Modeles.DWM_TNP.OPACITY,
                    fVisible = true,
                    opacity = 255,
                    rcDestination = new WinAPI.Modeles.RECT() { left = 0, top = 0, right = (int)Width, bottom = (int)Height }
                };
                Dwmapi.DwmUpdateThumbnailProperties(_thumbPtr, ref _thumb);
            }
        }
    }
}
