using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ManagedShell.AppBar
{
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

        public static AppBarScreen FromScreen(Screen screen, int numScreen)
        {
            AppBarScreen appBarScreen = new()
            {
                Bounds = screen.Bounds,
                DeviceName = screen.DeviceName,
                Primary = screen.Primary,
                WorkingArea = screen.WorkingArea,
                BitsPerPixel = screen.BitsPerPixel,
                NumScreen = numScreen,
#pragma warning disable S3011
                Handle = (IntPtr)typeof(Screen).GetField("hmonitor", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(screen)
            };
#pragma warning restore S3011
            appBarScreen.ChangeDpi();
            return appBarScreen;
        }

        public static AppBarScreen FromScreen(int numScreen)
        {
            return FromScreen(Screen.AllScreens[numScreen], numScreen);
        }

        public static AppBarScreen FromPrimaryScreen()
        {
            return FromScreen(Screen.PrimaryScreen, 1);
        }

        public static List<AppBarScreen> FromAllOthersScreen()
        {
            List<AppBarScreen> screens = new();
            int i = 0;

            foreach (Screen screen in Screen.AllScreens)
            {
                if (!screen.Primary)
                    screens.Add(FromScreen(screen, i++));
            }

            return screens;
        }

        public static List<AppBarScreen> FromAllScreens()
        {
            List<AppBarScreen> screens = new();
            int i = 0;

            foreach (Screen screen in Screen.AllScreens)
            {
                screens.Add(FromScreen(screen, i++));
            }

            return screens;
        }
    }
}
