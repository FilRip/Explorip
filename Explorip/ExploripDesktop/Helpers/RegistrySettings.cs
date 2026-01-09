using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

using Explorip.Desktop.ViewModels;

using ManagedShell.Common.Enums;
using ManagedShell.Common.Helpers;
using ManagedShell.Interop;

using Microsoft.Win32;

namespace ExploripDesktop.Helpers;

internal class RegistrySettings
{
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
}
