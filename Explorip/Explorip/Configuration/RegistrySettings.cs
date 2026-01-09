using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Media;

using Explorip.Constants;
using Explorip.Desktop.ViewModels;
using Explorip.Helpers;

using ManagedShell.Common.Enums;
using ManagedShell.Common.Helpers;
using ManagedShell.Interop;

using Microsoft.Win32;

namespace Explorip.Configuration;

internal static class RegistrySettings
{
    private static bool? _showVersion;
    private static string _currentVersion;
    private static string _currentBuild;
    private static string _currentBuildName;

    public static bool DrawWindowsVersionOnDesktop()
    {
        if (!_showVersion.HasValue)
        {
            _showVersion = false;
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", false);
            if (key != null)
            {
                object value = key.GetValue("PaintDesktopVersion");
                if (value != null)
                    _showVersion = value.ToString() == "1";
            }
        }
        return _showVersion.Value;
    }

    public static string CurrentVersion()
    {
        if (string.IsNullOrWhiteSpace(_currentVersion))
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows NT\CurrentVersion", false);
            if (key != null)
            {
                string name = key.GetValue("ProductName").ToString();
                string build = key.GetValue("BuildLab").ToString();
                _currentVersion = name + Environment.NewLine + build;
            }
        }
        return _currentVersion;
    }

    public static string CurrentPreciseBuild()
    {
        if (string.IsNullOrWhiteSpace(_currentBuild))
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows NT\CurrentVersion", false);
            if (key != null)
            {
                string name = key.GetValue("CurrentBuildNumber").ToString();
                string subBuild = key.GetValue("UBR").ToString();
                _currentBuild = name + "." + subBuild;
            }
        }
        return _currentBuild;
    }

    public static string CurrentBuildName()
    {
        if (string.IsNullOrWhiteSpace(_currentBuildName))
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows NT\CurrentVersion", false);
            if (key != null)
                _currentBuildName = key.GetValue("DisplayVersion").ToString();
        }
        return _currentBuildName;
    }

    public static string GetCurrentShell()
    {
        string shell = null;
        RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows NT\CurrentVersion\Winlogon", false);
        if (key != null && key.GetValueNames().Contains("Shell"))
            shell = key.GetValue("Shell").ToString();
        if (string.IsNullOrWhiteSpace(shell))
        {
            key = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows NT\CurrentVersion\Winlogon", false);
            if (key != null && key.GetValueNames().Contains("Shell"))
                shell = key.GetValue("Shell").ToString();
        }
        return shell;
    }

    public static List<OneDesktopItemViewModel> ListDesktopSystemIcons(IconSize iconSize = IconSize.ExtraLarge)
    {
        List<OneDesktopItemViewModel> result = [];
        RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\HideDesktopIcons\NewStartPanel", false);
        foreach (string name in key.GetValueNames().Where(name => key.GetValue(name).ToString() != "1"))
        {
            string label = "";
            ImageSource icon = null, iconOverlay = null;
            RegistryKey clSidKey = Registry.ClassesRoot.OpenSubKey(@$"CLSID\{name}", false);
            if (clSidKey != null)
            {
                label = clSidKey.GetValue("").ToString();
                try
                {
                    if (clSidKey.GetValueNames().Contains("LocalizedString"))
                    {
                        string fullIconString = clSidKey.GetValue("LocalizedString").ToString();
                        if (!string.IsNullOrWhiteSpace(fullIconString))
                        {
                            uint stringIndex = 0;
                            string res;
                            if (fullIconString.IndexOf(',') >= 0)
                            {
                                res = fullIconString.Substring(0, fullIconString.LastIndexOf(','));
                                stringIndex = uint.Parse(fullIconString.Substring(fullIconString.LastIndexOf(',') + 1).Replace("-", ""));
                            }
                            else
                                res = fullIconString;
                            if (res.StartsWith("@"))
                                res = res.Substring(1);
                            label = Localization.Load(Path.GetFullPath(res), stringIndex, clSidKey.GetValue("").ToString());
                        }
                    }
                }
                catch (Exception) { /* Can't get localized string, ignore errors */ }
                try
                {
                    IntPtr pidl;
                    pidl = NativeMethods.ILCreateFromPath($"shell:::{name}");
                    if (pidl != IntPtr.Zero)
                    {
                        IntPtr hIcon;
                        hIcon = IconHelper.GetIconByPidl(pidl, iconSize, out IntPtr hOverlay);
                        NativeMethods.ILFree(pidl);
                        if (hIcon != IntPtr.Zero)
                        {
                            icon = IconManager.Convert(Icon.FromHandle(hIcon));
                            if (hOverlay != IntPtr.Zero)
                                iconOverlay = IconManager.Convert(Icon.FromHandle(hOverlay));
                        }
                    }
                }
                catch { /* Can't get icon, ignore errors */ }
            }
            OneDesktopItemViewModel item = new()
            {
                Name = label,
                Icon = icon,
                FullPath = $"shell:::{name}",
                SpecialFolder = true,
                OverlayIcon = iconOverlay,
            };
            result.Add(item);
        }

        return result;
    }

    public static void ChangeShellCurrentUser(bool remove = false)
    {
        RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows NT\CurrentVersion\Winlogon", false);
        if (remove)
        {
            if (key.GetValueNames().Contains("Shell"))
                key.DeleteValue("Shell");
        }
        else
            key.SetValue("Shell", System.Reflection.Assembly.GetEntryAssembly().Location);
    }
}
