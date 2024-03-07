using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;

using Explorip.Constants;
using Explorip.Desktop.ViewModels;
using Explorip.Helpers;

using Microsoft.Win32;

namespace Explorip.Configuration;

internal static class RegistrySettings
{
    private static bool? _showVersion;
    private static string _currentVersion;

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
                object name = key.GetValue("ProductName");
                object build = key.GetValue("BuildLab");
                _currentVersion = name + Environment.NewLine + build;
            }
        }
        return _currentVersion;
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

    public static List<OneDesktopItemViewModel> ListDesktopSystemIcons()
    {
        List<OneDesktopItemViewModel> result = [];
        RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\HideDesktopIcons\NewStartPanel", false);
        foreach (string name in key.GetValueNames())
            if (key.GetValue(name).ToString() != "1")
            {
                string label = "";
                ImageSource icon = null;
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
                        RegistryKey keyIcon = clSidKey.OpenSubKey("DefaultIcon", false);
                        if (keyIcon != null)
                        {
                            string iconLocation;
                            iconLocation = keyIcon.GetValue("").ToString();
                            if (!string.IsNullOrWhiteSpace(iconLocation))
                            {
                                int iconIndex = 0;
                                string res;
                                if (iconLocation.IndexOf(',') >= 0)
                                {
                                    res = iconLocation.Substring(0, iconLocation.LastIndexOf(','));
                                    iconIndex = int.Parse(iconLocation.Substring(iconLocation.LastIndexOf(',') + 1));
                                }
                                else
                                    res = iconLocation;
                                icon = IconManager.GetIconFromFile(Path.GetFullPath(res), iconIndex);
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
                };
                result.Add(item);
            }

        return result;
    }
}
