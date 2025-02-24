using System;

namespace WindowsDesktop.Interop
{
    [ComInterfaceWrapper]
    internal class ApplicationViewCollection(ComInterfaceAssembly assembly) : ComInterfaceWrapperBase(assembly)
    {
        public ApplicationView GetViewForHwnd(IntPtr hWnd)
        {
            object[] param = Args(hWnd, null);
            this.Invoke(param);

            return new ApplicationView(this.ComInterfaceAssembly, param[1]);
        }
    }
}
