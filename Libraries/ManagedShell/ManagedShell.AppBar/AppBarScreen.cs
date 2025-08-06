using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using WpfScreenHelper;

namespace ManagedShell.AppBar;

public class AppBarScreen
{
    public int NumScreen { get; set; }

    public Rectangle Bounds { get; set; }

    public string DeviceName { get; set; }

    public bool Primary { get; set; }

    public Rectangle WorkingArea { get; set; }

    public uint DpiX { get; set; }

    public uint DpiY { get; set; }

    public int DpiScalePercent
    {
        get
        {
            if (DpiX <= 96)
            {
                return 100;
            }
            else
            {
                int nbDiv = (int)Math.Round((double)(DpiX - 96) / 24);
                return 100 + (nbDiv * 25);
            }
        }
    }

    public double DpiScale
    {
        get
        {
            return (double)DpiScalePercent / 100;
        }
    }

    public void ChangeDpi()
    {
        Interop.NativeMethods.GetDpiForMonitor(Handle, Interop.NativeMethods.DPI_TYPE.MDT_EFFECTIVE_DPI, out uint x, out uint y);
        DpiX = x;
        DpiY = y;
    }

    public IntPtr Handle { get; set; }

    public int BitsPerPixel { get; set; }

    private static Rectangle ToRectangle(System.Windows.Rect rect)
    {
        return new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
    }

    public static AppBarScreen FromScreen(Screen screen, int numScreen)
    {
        if (screen == null)
            return null;
        AppBarScreen appBarScreen = new()
        {
            Bounds = ToRectangle(screen.Bounds),
            DeviceName = screen.DeviceName,
            Primary = screen.Primary,
            WorkingArea = ToRectangle(screen.WorkingArea),
            //BitsPerPixel = screen.BitsPerPixel,
            NumScreen = numScreen,
#pragma warning disable S3011
            Handle = screen.MonitorHandle,
        };
#pragma warning restore S3011
        appBarScreen.ChangeDpi();
        return appBarScreen;
    }

    public static AppBarScreen FromScreen(int numScreen)
    {
        return FromScreen(Screen.AllScreens.FirstOrDefault(s => s.DisplayNumber == numScreen), numScreen);
    }

    public static AppBarScreen FromPrimaryScreen()
    {
        Screen screen = Screen.PrimaryScreen;
        return FromScreen(screen, screen.DisplayNumber);
    }

    public static List<AppBarScreen> FromAllOthersScreen()
    {
        List<AppBarScreen> screens = [];

        foreach (Screen screen in Screen.AllScreens)
        {
            if (!screen.Primary)
                screens.Add(FromScreen(screen, screen.DisplayNumber));
        }

        return screens;
    }

    public static List<AppBarScreen> FromAllScreens()
    {
        List<AppBarScreen> screens = [];

        foreach (Screen screen in Screen.AllScreens)
        {
            screens.Add(FromScreen(screen, screen.DisplayNumber));
        }

        return screens;
    }
}
