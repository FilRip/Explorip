using System;
using System.Runtime.InteropServices;

namespace ManagedShell.Interop
{
    public partial class NativeMethods
    {
        public enum DPI_TYPE
        {
            MDT_EFFECTIVE_DPI = 0,
            MDT_ANGULAR_DPI = 1,
            MDT_RAW_DPI = 2,
        }

        [DllImport("shcore.dll")]
        internal static extern int GetDpiForMonitor(IntPtr hwndMoniteur, DPI_TYPE dpiType, out uint x, out uint y);
    }
}
