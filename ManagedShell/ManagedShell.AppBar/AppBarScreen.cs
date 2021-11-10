using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ManagedShell.AppBar
{
    public class AppBarScreen
    {
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

        public IntPtr Handle { get; set; }

        public int BitsPerPixel { get; set; }

        public static AppBarScreen FromScreen(Screen screen)
        {
            AppBarScreen appBarScreen = new AppBarScreen()
            {
                Bounds = screen.Bounds,
                DeviceName = screen.DeviceName,
                Primary = screen.Primary,
                WorkingArea = screen.WorkingArea,
                BitsPerPixel = screen.BitsPerPixel,
            };
            appBarScreen.Handle = (IntPtr)typeof(Screen).GetField("hmonitor", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(screen);
            Interop.NativeMethods.GetDpiForMonitor(appBarScreen.Handle, Interop.NativeMethods.DPI_TYPE.MDT_EFFECTIVE_DPI, out uint x, out uint y);
            appBarScreen.DpiX = x;
            appBarScreen.DpiY = y;
            return appBarScreen;
        }

        public static AppBarScreen FromPrimaryScreen()
        {
            return FromScreen(Screen.PrimaryScreen);
        }

        public static List<AppBarScreen> FromAllOthersScreen()
        {
            List<AppBarScreen> screens = new List<AppBarScreen>();

            foreach (Screen screen in Screen.AllScreens)
            {
                if (!screen.Primary)
                    screens.Add(FromScreen(screen));
            }

            return screens;
        }

        public static List<AppBarScreen> FromAllScreens()
        {
            List<AppBarScreen> screens = new List<AppBarScreen>();

            foreach (Screen screen in Screen.AllScreens)
            {
                screens.Add(FromScreen(screen));
            }

            return screens;
        }
    }
}
