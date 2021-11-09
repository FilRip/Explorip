using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

using Explorip.WinAPI;

namespace Explorip.TaskBar.Controls
{
    /// <summary>
    /// Logique d'interaction pour TaskThumbButton.xaml
    /// </summary>
    public partial class TaskThumbButton : Window
    {
        private TaskButton _parent;
        private IntPtr _handle;
        private IntPtr _thumbPtr;
        private WinAPI.Modeles.DWM_THUMBNAIL_PROPERTIES _thumb;

        public TaskThumbButton(TaskButton parent)
        {
            InitializeComponent();
            _parent = parent;
        }

        private void Window_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
        }

        private void Window_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_thumbPtr != IntPtr.Zero)
            {
                Dwmapi.DwmUnregisterThumbnail(_thumbPtr);
                Marshal.FreeHGlobal(_thumbPtr);
            }
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            // TODO : Apercu de la fenetre
            // Docs : https://stackoverflow.com/questions/17484978/c-sharp-on-mouseover-at-taskbar-a-preview-is-generated-how-can-i-achieve-thi
            WindowInteropHelper helper = new WindowInteropHelper(this);
            _handle = helper.Handle;
            int result = Dwmapi.DwmRegisterThumbnail(_handle, _parent.ApplicationWindow.Handle, out _thumb);
            if (result == (int)Commun.HRESULT.S_OK)
            {
                _thumbPtr = Marshal.AllocHGlobal(Marshal.SizeOf(_thumb));
                Marshal.StructureToPtr(_thumb, _thumbPtr, true);
                _thumb.dwFlags = WinAPI.Modeles.DWM_FLAGS.DWM_TNP_VISIBLE | WinAPI.Modeles.DWM_FLAGS.DWM_TNP_RECTDESTINATION | WinAPI.Modeles.DWM_FLAGS.DWM_TNP_SOURCECLIENTAREAONLY;
                _thumb.fVisible = true;
                _thumb.opacity = (255 * 70) / 100;
                _thumb.rcDestination = new WinAPI.Modeles.RECT() { left = (int)Left, top = (int)Top, right = (int)(Left + Width), bottom = (int)(Top + Height) };
                _thumb.fSourceClientAreaOnly = false;
                Dwmapi.DwmUpdateThumbnailProperties(_thumbPtr, ref _thumb);
            }
        }
    }
}
