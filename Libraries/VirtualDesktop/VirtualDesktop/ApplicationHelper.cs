using System;
using System.Runtime.InteropServices;

using WindowsDesktop.Interop;

namespace WindowsDesktop
{
    public static class ApplicationHelper
    {
        internal static ApplicationView GetApplicationView(this IntPtr hWnd)
        {
            return ComInterface.ApplicationViewCollection.GetViewForHwnd(hWnd);
        }

        public static string GetAppId(IntPtr hWnd)
        {
            VirtualDesktopHelper.ThrowIfNotSupported();

            try
            {
                return hWnd.GetApplicationView().GetAppUserModelId();
            }
            catch (COMException ex) when (ex.Match(HResult.TYPE_E_ELEMENTNOTFOUND))
            {
                return null;
            }
        }
    }
}
