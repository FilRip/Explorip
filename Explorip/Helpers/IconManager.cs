using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using ManagedShell.Interop;

namespace Explorip.Helpers;

internal static class IconManager
{
    internal static Icon Extract(string file, int number, bool largeIcon)
    {
        try
        {
            NativeMethods.ExtractIconEx(file, number, out IntPtr large, out IntPtr small, 1);
            Icon icon = null;
            if ((largeIcon && large != IntPtr.Zero) ||
                (!largeIcon && small != IntPtr.Zero))
            {
                icon = Icon.FromHandle(largeIcon ? large : small);
            }
            return icon;
        }
        catch (Exception)
        {
            return null;
        }
    }

    internal static BitmapSource Convert(Icon icon, bool disposeIcon = true)
    {
        try
        {
            BitmapSource result = Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            if (disposeIcon)
                icon.Dispose();
            return result;
        }
        catch (Exception)
        {
            return null;
        }
    }

    internal static ImageSource GetIconFromFile(string filename, int index)
    {
        Icon icon = Extract(filename, index, true);
        if (icon != null)
            return Convert(icon);
        return null;
    }

    internal static ImageSource GetIconFromFile(string filename, bool withOverlay)
    {
        // TODO : https://www.lluisfranco.com/extract-icons-win32/
        NativeMethods.SHGFI flags = NativeMethods.SHGFI.OpenIcon;
        if (withOverlay)
            flags |= NativeMethods.SHGFI.AddOverlays;
        if (Path.GetExtension(filename).ToLower() == ".lnk")
            flags |= NativeMethods.SHGFI.LinkOverlay;
        NativeMethods.ShFileInfo fi = new();
        NativeMethods.SHGetFileInfo(filename, NativeMethods.FILE_ATTRIBUTE.NORMAL, ref fi, (uint)Marshal.SizeOf(fi), flags);
        if (fi.hIcon != IntPtr.Zero)
            return Convert(Icon.FromHandle(fi.hIcon));
        return null;
    }
}
