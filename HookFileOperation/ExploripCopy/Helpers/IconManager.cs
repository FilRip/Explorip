using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

using ManagedShell.Interop;

namespace ExploripCopy.Helpers;

internal static class IconManager
{
    internal static Icon Extract(string file, int number, bool largeIcon)
    {
        try
        {
            NativeMethods.ExtractIconEx(file, number, out IntPtr large, out IntPtr small, 1);
            Icon icon = Icon.FromHandle(largeIcon ? large : small);
            if (largeIcon && small != IntPtr.Zero)
                NativeMethods.DestroyIcon(small);
            else if (!largeIcon && large != IntPtr.Zero)
                NativeMethods.DestroyIcon(large);
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

    internal static BitmapSource InverseRedGreenColor(BitmapSource source)
    {
        try
        {
            FormatConvertedBitmap tmp = new();
            tmp.BeginInit();
            tmp.Source = source;
            tmp.DestinationFormat = System.Windows.Media.PixelFormats.Bgra32;
            tmp.EndInit();
            Bitmap newImage = new(tmp.PixelWidth, tmp.PixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            for (int i = 0; i < newImage.Width; i++)
                for (int j = 0; j < newImage.Height; j++)
                {
                    Color pixel = newImage.GetPixel(i, j);
                    newImage.SetPixel(i, j, Color.FromArgb(pixel.A, pixel.G, pixel.R, pixel.B));
                }
            BitmapSource result = Imaging.CreateBitmapSourceFromHBitmap(newImage.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            newImage.Dispose();
            return result;
        }
        catch (Exception)
        {
            return null;
        }
    }
}
