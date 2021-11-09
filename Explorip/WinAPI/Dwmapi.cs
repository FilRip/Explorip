using System;
using System.Runtime.InteropServices;

using Explorip.WinAPI.Modeles;

namespace Explorip.WinAPI
{
    public static class Dwmapi
    {
        [DllImport("dwmapi.dll")]
        public static extern bool DwmSetWindowAttribute(IntPtr hwnd, int attribut, ref int attrValeur, int attrSize);

        [DllImport("dwmapi.dll")]
        public static extern int DwmRegisterThumbnail(IntPtr dest, IntPtr src, out DWM_THUMBNAIL_PROPERTIES thumb);

        [DllImport("dwmapi.dll")]
        public static extern int DwmUpdateThumbnailProperties(IntPtr hThumb, ref DWM_THUMBNAIL_PROPERTIES props);

        [DllImport("dwmapi.dll")]
        public static extern int DwmUnregisterThumbnail(IntPtr thumb);
    }
}
