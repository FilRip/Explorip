using System;
using System.Windows.Interop;
using System.Windows.Threading;

using WindowsDesktop.Interop;

namespace WindowsDesktop.Internal
{
    internal abstract class RawWindow
    {
        public string Name { get; set; }

        public HwndSource Source { get; private set; }

        public IntPtr Handle => Source?.Handle ?? IntPtr.Zero;

        public virtual void Show()
        {
            Show(new HwndSourceParameters(Name));
        }

        protected void Show(HwndSourceParameters parameters)
        {
            Source = new HwndSource(parameters);
            Source.AddHook(WndProc);
        }

        public virtual void Close()
        {
            Source?.RemoveHook(WndProc);
            // Source could have been created on a different thread, which means we 
            // have to Dispose of it on the UI thread or it will crash.
            Source?.Dispatcher?.BeginInvoke(DispatcherPriority.Send, (Action)(() => Source?.Dispose()));
            Source = null;

            NativeMethods.CloseWindow(Handle);
        }

        protected virtual IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            return IntPtr.Zero;
        }
    }
}
