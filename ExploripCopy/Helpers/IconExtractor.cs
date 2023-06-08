using System;
using System.Drawing;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

using ManagedShell.Interop;

namespace ExploripCopy.Helpers
{
    internal static class IconExtractor
    {
        internal static BitmapSource Extract(string file, int number, bool largeIcon)
        {
            NativeMethods.ExtractIconEx(file, number, out IntPtr large, out IntPtr small, 1);
            try
            {
                Icon icon = Icon.FromHandle(largeIcon ? large : small);
                BitmapSource bitmap = Imaging.CreateBitmapSourceFromHIcon(icon.Handle, System.Windows.Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                icon.Dispose();
                return bitmap;
            }
            catch
            {
                return null;
            }
        }
    }
}
