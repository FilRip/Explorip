using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Explorip.Exceptions;

using ManagedShell.Interop;

using WpfScreenHelper;

namespace Explorip.Helpers;

public static class ScreenManager
{
    public static int NumberOfScreen()
    {
        return Screen.AllScreens.Count();
    }

    public static Screen MainScreen()
    {
        return Screen.PrimaryScreen;
    }

    public static bool ScreenConnected
    {
        get { return Screen.AllScreens.Any(); }
    }

    private static void ThrowIfNoScreenConnected()
    {
        if (!Screen.AllScreens.Any())
            throw new ScreenManagerException(Constants.Localization.NO_MONITOR_CONNECTED);
    }

    public static Screen[] ListScreenExceptPrimary()
    {
        IEnumerable<Screen> result = Screen.AllScreens.Where(s => !s.Primary);
        return [.. result];
    }

    public static Screen GetLeftScreen()
    {
        ThrowIfNoScreenConnected();
        if (Screen.AllScreens.Count() == 1)
            return Screen.AllScreens.ElementAt(0);
        int posX = int.MaxValue;
        Screen last = null;
        foreach (Screen s in Screen.AllScreens)
            if (s.WorkingArea.Location.X < posX)
            {
                last = s;
                posX = (int)s.WorkingArea.Location.X;
            }
        return last;
    }

    public static Screen GetRightScreen()
    {
        ThrowIfNoScreenConnected();
        if (Screen.AllScreens.Count() == 1)
            return Screen.AllScreens.ElementAt(0);
        int posX = -1;
        Screen last = null;
        foreach (Screen s in Screen.AllScreens)
            if (s.WorkingArea.Location.X > posX)
            {
                last = s;
                posX = (int)s.WorkingArea.Location.X;
            }
        return last;
    }

    public static Screen GetCenterScreen()
    {
        ThrowIfNoScreenConnected();
        if (Screen.AllScreens.Count() == 1)
            return Screen.AllScreens.ElementAt(0);
        if (Screen.AllScreens.Count() == 2)
            return Screen.PrimaryScreen;
        Math.DivRem(Screen.AllScreens.Count(), 2, out int nbEvenScreen);
        if (nbEvenScreen == 0)
            return Screen.PrimaryScreen; // A voir si on retourne une exception ou l'écran principal quand il n'y a pas d'écran central (nombre paire d'écran)
        List<Screen> OrderedScreen;
        OrderedScreen = [.. Screen.AllScreens.OrderBy(item => item.WorkingArea.Location.X)];
        while (OrderedScreen.Count > 1)
        {
            OrderedScreen.RemoveAt(0);
            OrderedScreen.RemoveAt(OrderedScreen.Count - 1);
        }
        return OrderedScreen[0];
    }

    /// <summary>
    /// Return a screen, from base zero
    /// </summary>
    /// <param name="id">Number of screen</param>
    public static Screen GetScreen(int id)
    {
        ThrowIfNoScreenConnected();
        if (id > Screen.AllScreens.Count() - 1)
            throw new ArgumentOutOfRangeException(nameof(id));
        return Screen.AllScreens.ElementAt(id);
    }

    public static Screen GetScreen(ScreenPosition positionScreen)
    {
        return positionScreen switch
        {
            ScreenPosition.LEFT => GetLeftScreen(),
            ScreenPosition.CENTER => GetCenterScreen(),
            ScreenPosition.RIGHT => GetRightScreen(),
            _ => throw new ScreenManagerException("Unknown position of requested screen"),
        };
    }

    public enum ScreenPosition
    {
        LEFT = 1,
        CENTER = 2,
        RIGHT = 3
    }

    public static BitmapSource TakeScreenShot(int numScreen = -1, int x = -1, int y = -1, int width = -1, int height = -1, int dpiX = 1, int dpiY = 1, PixelFormat? pf = null)
    {
        if (numScreen < 0 && x < 0)
            throw new ArgumentNullException(nameof(numScreen));
        if (!pf.HasValue)
            pf = PixelFormats.Bgr24;
        if (numScreen >= 0)
        {
            Screen screen = Screen.AllScreens.SingleOrDefault(s => s.DisplayNumber == numScreen) ?? throw new ArgumentNullException(nameof(numScreen));
            x = (int)screen.Bounds.X;
            y = (int)screen.Bounds.Y;
            if (width < 0)
                width = (int)screen.Bounds.Width;
            if (height < 0)
                height = (int)screen.Bounds.Height;
        }

        using Bitmap bmp = new(width, height);
        using Graphics g = Graphics.FromImage(bmp);
        g.CopyFromScreen(x, y, 0, 0, bmp.Size);

        System.Drawing.Imaging.BitmapData data = bmp.LockBits(new Rectangle(0, 0, width, height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);

        return BitmapSource.Create(width, height, dpiX, dpiY, pf.Value, null, data.Scan0, data.Stride * height, data.Stride);
    }

    public static ImageSource CaptureScreen()
    {
        return CaptureWindow(NativeMethods.GetDesktopWindow());
    }

    public static ImageSource CaptureWindow(IntPtr handle)
    {
        IntPtr hdcSrc = NativeMethods.GetWindowDC(handle);

        NativeMethods.GetWindowRect(handle, out NativeMethods.Rect windowRect);

        int width = windowRect.Right - windowRect.Left;
        int height = windowRect.Bottom - windowRect.Top;

        IntPtr hdcDest = NativeMethods.CreateCompatibleDC(hdcSrc);
        IntPtr hBitmap = NativeMethods.CreateCompatibleBitmap(hdcSrc, width, height);

        IntPtr hOld = NativeMethods.SelectObject(hdcDest, hBitmap);
        NativeMethods.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, NativeMethods.Rop.SRCCOPY);
        NativeMethods.SelectObject(hdcDest, hOld);
        NativeMethods.DeleteDC(hdcDest);
        NativeMethods.ReleaseDC(handle, hdcSrc);

        ImageSource result = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        NativeMethods.DeleteObject(hBitmap);

        return result;
    }

    public static ImageSource RenderTargetToBitmap(Visual visualControl, int width, int height, int dpiX = 1, int dpiY = 1, PixelFormat? pf = null)
    {
        if (!pf.HasValue)
            pf = PixelFormats.Pbgra32;
        RenderTargetBitmap rtb = new(width, height, dpiX, dpiY, pf.Value);
        rtb.Render(visualControl);
        return rtb;
    }

    #region WPF

    public static void MoveWindowToScreen(Window window, int numScreen)
    {
        MoveWindowToScreen(window, numScreen, false);
    }

    public static void MoveWindowToScreen(Window window, int numScreen, bool fullScreen)
    {
        if (numScreen > Screen.AllScreens.Count() - 1)
            throw new ArgumentOutOfRangeException(nameof(numScreen));
        MoveWindowToScreen(window, Screen.AllScreens.ElementAt(numScreen), fullScreen);
    }

    public static void MoveWindowToScreen(Window window, Screen screen)
    {
        MoveWindowToScreen(window, screen, false);
    }

    public static void MoveWindowToScreen(Window window, Screen screen, bool fullScreen)
    {
        window.Left = screen.WorkingArea.Location.X;
        window.Top = screen.WorkingArea.Location.Y;
        if (window.Width > screen.WorkingArea.Width) window.Width = screen.WorkingArea.Width;
        if (window.Height > screen.WorkingArea.Height) window.Height = screen.WorkingArea.Height;
        if (fullScreen)
            window.WindowState = WindowState.Maximized;
    }

    public static void MoveWindowToScreen(Window window, ScreenPosition screen)
    {
        MoveWindowToScreen(window, screen, false);
    }

    public static void MoveWindowToScreen(Window window, ScreenPosition screen, bool fullScreen)
    {
        Screen s = GetScreen(screen);
        MoveWindowToScreen(window, s, fullScreen);
    }

    /// <summary>
    /// Return the main window of current application
    /// </summary>
    public static Screen GetCurrentScreen()
    {
        Screen currentScreen = Application.Current.Dispatcher.Invoke(() =>
        {
            Window currentWindow = Application.Current.Windows.OfType<Window>().FirstOrDefault();
            Screen ecran = Screen.FromHandle(new WindowInteropHelper(currentWindow).EnsureHandle());
            return ecran;
        });

        return currentScreen;
    }

    /// <summary>
    /// Display window on a specified screen and center it on this screen
    /// </summary>
    public static void CenterOnScreen(Window targetWindow, int numScreen = -1)
    {
        Screen screen;
        if (numScreen < 0)
            screen = GetCurrentScreen();
        else
            screen = GetScreen(numScreen);

        Rect r = screen.WorkingArea;
        targetWindow.Top = r.Top + r.Height / 2 - targetWindow.ActualHeight / 2;
        targetWindow.Left = r.Left + r.Width / 2 - targetWindow.ActualWidth / 2;
    }

    #endregion
}
