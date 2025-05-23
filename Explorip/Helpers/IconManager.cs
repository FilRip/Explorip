﻿using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using ManagedShell.Interop;

namespace Explorip.Helpers;

public static class IconManager
{
    public static Icon Extract(string file, int number, bool largeIcon)
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

    public static BitmapSource Convert(Icon icon, bool disposeIcon = true)
    {
        try
        {
            if (icon == null)
                return null;
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

    public static ImageSource GetIconFromFile(string filename, int index, bool large = true)
    {
        Icon icon = Extract(filename, index, large);
        if (icon != null)
            return Convert(icon);
        return null;
    }
}
