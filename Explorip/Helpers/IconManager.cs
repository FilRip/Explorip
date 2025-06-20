﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
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
                if (largeIcon && small != IntPtr.Zero)
                    NativeMethods.DestroyIcon(small);
                else if (!largeIcon && large != IntPtr.Zero)
                    NativeMethods.DestroyIcon(large);
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

    public static ImageSource GetIconFromFile(string filename, int index, bool large = true, string overlay = null, int indexOverlay = 0, int offsetOverlay = 4)
    {
        Icon icon = Extract(filename, index, large);
        if (icon != null)
        {
            if (!string.IsNullOrWhiteSpace(overlay))
            {
                Icon overlayIcon = Extract(overlay, indexOverlay, large);
                if (overlayIcon != null)
                {
                    int size = large ? 32 : 16;
                    int overlaySize = large ? 16 : 8;
                    Bitmap bmp = new(size, size);
                    using Graphics graphics = Graphics.FromImage(bmp);
                    graphics.DrawIcon(icon, new Rectangle(0, 0, size - offsetOverlay, size - offsetOverlay));
                    graphics.DrawIcon(overlayIcon, new Rectangle(overlaySize, overlaySize, overlaySize, overlaySize));
                    graphics.Save();
                    icon.Dispose();
                    overlayIcon.Dispose();
                    icon = Icon.FromHandle(bmp.GetHicon());
                    bmp.Dispose();
                }
            }
            return Convert(icon);
        }
        return null;
    }

    public static ImageSource StringToImageSource(string textToDraw, string fontFamily = "Segoe MDL2 Assets", int fontSize = 16, System.Windows.Media.Brush foregroundColor = null, double dpi = 1, int width = 16, int height = 16, System.Windows.Media.Brush backgroundColor = null, System.Windows.Point pos = new(), ImageSource overlay = null, Rect? rect = null, int offsetOverlay = 0)
    {
        DrawingVisual visual = new();
        using (DrawingContext context = visual.RenderOpen())
        {
            context.DrawRectangle(backgroundColor ?? System.Windows.Media.Brushes.Transparent, null, new Rect(0, 0, width, height));
            FormattedText text = new(
                textToDraw,
                CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                new Typeface(fontFamily),
                fontSize - offsetOverlay,
                foregroundColor ?? System.Windows.Media.Brushes.Black,
                96 * dpi);
            context.DrawText(text, pos);
            if (overlay != null)
            {
                if (rect == null)
                    rect = new Rect(width / 2, height / 2, width / 2, height / 2);
                context.DrawImage(overlay, rect.Value);
            }
        }

        RenderTargetBitmap bitmap = new(width, height, 96 * dpi, 96 * dpi, PixelFormats.Pbgra32);

        bitmap.Render(visual);
        bitmap.Freeze();

        return bitmap;
    }

    public static Bitmap ChangeOpacity(Image image, float opacity, bool disposeBmp = true)
    {
        Bitmap result = new(image.Width, image.Height);
        using Graphics graphics = Graphics.FromImage(result);
        ColorMatrix matrix = new() { Matrix33 = opacity };
        ImageAttributes imgAttrib = new();
        imgAttrib.SetColorMatrix(matrix);
        graphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imgAttrib);
        graphics.Save();
        if (disposeBmp)
            image.Dispose();
        return result;
    }
}
