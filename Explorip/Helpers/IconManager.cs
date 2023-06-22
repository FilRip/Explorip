using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

using ManagedShell.Interop;

namespace Explorip.Helpers
{
    internal static class IconManager
    {
        internal static Icon Extract(string file, int number, bool largeIcon)
        {
            try
            {
                NativeMethods.ExtractIconEx(file, number, out IntPtr large, out IntPtr small, 1);
                Icon icon = Icon.FromHandle(largeIcon ? large : small);
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
                    icon?.Dispose();
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
