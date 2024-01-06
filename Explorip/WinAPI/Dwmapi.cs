using System;
using System.Runtime.InteropServices;

using Explorip.WinAPI.Modeles;

namespace Explorip.WinAPI
{
    public static class Dwmapi
    {
        [DllImport("dwmapi.dll")]
        internal static extern int DwmRegisterThumbnail(IntPtr dest, IntPtr src, out IntPtr thumb);

        [DllImport("dwmapi.dll")]
        internal static extern int DwmUpdateThumbnailProperties(IntPtr hThumb, ref DwmThumbnailProperties props);

        [DllImport("dwmapi.dll")]
        internal static extern int DwmUnregisterThumbnail(IntPtr thumb);

        [DllImport("dwmapi.dll", EntryPoint = "#127")]
        internal static extern void DwmGetColorizationParameters(ref DwmColorizationParams colors);
    }
}
