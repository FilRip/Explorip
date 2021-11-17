﻿using System;

using Microsoft.Win32;

namespace Explorip.Helpers
{
    public static class WindowsSettings
    {
        // Docs : https://github.com/dbarros/WindowsAPICodePack
        // https://stackoverflow.com/questions/24081665/windows-api-code-pack-where-is-it/25048686#25048686
        // Windows 11 (to 10) : https://github.com/valinet/ExplorerPatcher
        // Ecrire sur le bureau : https://social.msdn.microsoft.com/Forums/vstudio/en-US/99354676-56c4-48b7-be62-ec34d53a073f/how-to-write-text-to-desktop-wallpaper?forum=csharpgeneral

        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19;

        public static bool IsWindowsApplicationInDarkMode()
        {
            try
            {
                RegistryKey reg = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
                if (reg != null)
                {
                    int lightMode = int.Parse(reg.GetValue("AppsUseLightTheme").ToString());
                    return (lightMode == 0);
                }
            }
            catch (Exception) { }
            return false;
        }

        public static bool IsWindows10OrGreater(int build = 0)
        {
            return (Environment.OSVersion.Version.Major >= 10 && Environment.OSVersion.Version.Build >= build);
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
    }
}
