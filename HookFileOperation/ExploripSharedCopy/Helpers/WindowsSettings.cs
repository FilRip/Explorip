using System;
using System.Drawing;

using Microsoft.Win32;

namespace ExploripSharedCopy.Helpers
{
    public static class WindowsSettings
    {
        // Ecrire sur le bureau : https://social.msdn.microsoft.com/Forums/vstudio/en-US/99354676-56c4-48b7-be62-ec34d53a073f/how-to-write-text-to-desktop-wallpaper?forum=csharpgeneral

        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19;

        public static bool IsWindowsApplicationInDarkMode()
        {
            try
            {
                RegistryKey reg = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", false);
                if (reg != null)
                {
                    int lightMode = int.Parse(reg.GetValue("AppsUseLightTheme").ToString());
                    return (lightMode == 0);
                }
            }
            catch (Exception) { /* Ignore errors */ }
            return false;
        }

        public static bool IsWindows10OrGreater(int build = 0)
        {
            return Environment.OSVersion.Version.Major >= 10 && Environment.OSVersion.Version.Build >= build;
        }

        public static bool IsWindows11OrGreater(int build = 22000)
        {
            if (build < 22000)
                return false;
            return IsWindows10OrGreater(build);
        }

        public static bool UseImmersiveDarkMode(IntPtr pointeurFenetre, bool activerDarkMode)
        {
            if (IsWindows10OrGreater())
            {
                int attribut = DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1;
                if (IsWindows10OrGreater(18985))
                {
                    attribut = DWMWA_USE_IMMERSIVE_DARK_MODE;
                }
                int immersiveActif = (activerDarkMode ? 1 : 0);
                return WinAPI.Dwmapi.DwmSetWindowAttribute(pointeurFenetre, attribut, ref immersiveActif, IntPtr.Size);
            }
            return false;
        }

        public static Color GetWindowsAccentColor()
        {
            RegistryKey cle = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\Dwm", false);
            if (cle != null)
            {
                object valeurCle = cle.GetValue("AccentColor");
                if (valeurCle is Int32 accentColor)
                {
                    return ParseDWordColor(accentColor);
                }
            }
            return Color.CadetBlue;
        }

        private static Color ParseDWordColor(Int32 color)
        {
            byte a = (byte)((color >> 24) & 0xFF),
                b = (byte)((color >> 16) & 0xFF),
                g = (byte)((color >> 8) & 0xFF),
                r = (byte)((color >> 0) & 0xFF);

            return Color.FromArgb(a, r, g, b);
        }
    }
}
