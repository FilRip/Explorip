using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

using ExploripConfig.Helpers;

using Microsoft.Win32;

using WpfScreenHelper;

using static ExploripConfig.Helpers.ExtensionsCommandLineArguments;

namespace ExploripConfig.Configuration;

public static class ConfigManager
{
    internal const string HKeyRoot = "Software\\CoolBytes\\Explorip";
    internal const string ToolBarNameInRegistry = "Toolbar";

    public static bool AllowWrite { get; set; }
    private static RegistryKey _registryKeyExplorer, _registryDesktop, _registryRootTaskbar, _registryStartMenu;
    private static readonly List<TaskbarConfig> _listScreens = [];

    public static void Init(bool allowWrite = true)
    {
        AllowWrite = allowWrite && !ArgumentExists("disablewriteconfig");
        _registryKeyExplorer = Registry.CurrentUser.CreateSubKey(HKeyRoot, true);
        if (_registryKeyExplorer == null)
            AllowWrite = false;
        _registryDesktop = _registryKeyExplorer?.CreateSubKey("Desktop");
        _registryRootTaskbar = _registryKeyExplorer?.CreateSubKey("Taskbars");
        _registryStartMenu = _registryKeyExplorer?.CreateSubKey("StartMenu");

        if (allowWrite && _registryKeyExplorer != null)
        {
            // If empty config in registry, set default settings
            if (string.IsNullOrWhiteSpace(_registryKeyExplorer.GetValue("Theme", "").ToString()))
                _registryKeyExplorer.SetValue("Theme", "System");
            if (string.IsNullOrWhiteSpace(_registryKeyExplorer.GetValue("Language", "").ToString()))
                _registryKeyExplorer.SetValue("Language", "System");
            if (string.IsNullOrWhiteSpace(_registryKeyExplorer.GetValue("HookCopy", "").ToString()))
                _registryKeyExplorer.SetValue("HookCopy", "True");
            if (string.IsNullOrWhiteSpace(_registryKeyExplorer.GetValue("UseOwnCopier", "").ToString()))
                _registryKeyExplorer.SetValue("UseOwnCopier", "True");
            if (string.IsNullOrWhiteSpace(_registryKeyExplorer.GetValue("StartTwoExplorer", "").ToString()))
                _registryKeyExplorer.SetValue("StartTwoExplorer", "True");
            if (string.IsNullOrWhiteSpace(_registryKeyExplorer.GetValue("ShowNotificationCopyOperation", "").ToString()))
                _registryKeyExplorer.SetValue("ShowNotificationCopyOperation", "True");
            string ws = _registryKeyExplorer.GetValue("ExplorerWindowState", "").ToString();
            if (string.IsNullOrWhiteSpace(ws) || !Enum.TryParse<WindowState>(ws, out _))
                ExplorerWindowState = WindowState.Normal;
            if (string.IsNullOrWhiteSpace(_registryKeyExplorer.GetValue("ExplorerPosX", "").ToString()))
                _registryKeyExplorer.SetValue("ExplorerPosX", (Screen.PrimaryScreen.WpfWorkingArea.X + 50).ToString());
            if (string.IsNullOrWhiteSpace(_registryKeyExplorer.GetValue("ExplorerPosY", "").ToString()))
                _registryKeyExplorer.SetValue("ExplorerPosY", (Screen.PrimaryScreen.WpfWorkingArea.Y + 50).ToString());
            if (string.IsNullOrWhiteSpace(_registryKeyExplorer.GetValue("ExplorerSizeX", "").ToString()))
                _registryKeyExplorer.SetValue("ExplorerSizeX", (Screen.PrimaryScreen.WpfWorkingArea.Width - 100).ToString());
            if (string.IsNullOrWhiteSpace(_registryKeyExplorer.GetValue("ExplorerSizeY", "").ToString()))
                _registryKeyExplorer.SetValue("ExplorerSizeY", (Screen.PrimaryScreen.WpfWorkingArea.Height - 100).ToString());
            if (string.IsNullOrWhiteSpace(_registryStartMenu.GetValue("PinnedAppPath", "").ToString()))
                _registryStartMenu.SetValue("PinnedAppPath", @"%APPDATA%\CoolBytes\Explorip\StartMenu\PinnedApp");
            if (string.IsNullOrWhiteSpace(_registryStartMenu.GetValue("IconSizeWidth", "").ToString()))
                _registryStartMenu.SetValue("IconSizeWidth", "100");
            if (string.IsNullOrWhiteSpace(_registryStartMenu.GetValue("IconSizeHeight", "").ToString()))
                _registryStartMenu.SetValue("IconSizeHeight", "100");
            if (string.IsNullOrWhiteSpace(_registryStartMenu.GetValue("PinnedAppPath2", "").ToString()))
                _registryStartMenu.SetValue("PinnedAppPath2", @"%APPDATA%\CoolBytes\Explorip\StartMenu\PinnedApp2");
            if (string.IsNullOrWhiteSpace(_registryStartMenu.GetValue("IconSizeWidth2", "").ToString()))
                _registryStartMenu.SetValue("IconSizeWidth2", "50");
            if (string.IsNullOrWhiteSpace(_registryStartMenu.GetValue("IconSizeHeight2", "").ToString()))
                _registryStartMenu.SetValue("IconSizeHeight2", "50");
            if (string.IsNullOrWhiteSpace(_registryStartMenu.GetValue("ShowPinnedApp2", "").ToString()))
                _registryStartMenu.SetValue("ShowPinnedApp2", "True");
            if (string.IsNullOrWhiteSpace(_registryStartMenu.GetValue("ShowApplicationsPrograms", "").ToString()))
                _registryStartMenu.SetValue("ShowApplicationsPrograms", "True");
            if (string.IsNullOrWhiteSpace(_registryRootTaskbar.GetValue("DelayBeforeShowThumbnail", "").ToString()))
                _registryRootTaskbar.SetValue("DelayBeforeShowThumbnail", "1000");

            foreach (string screenName in Screen.AllScreens.Select(s => s.DeviceName.TrimStart('.', '\\')))
            {
                TaskbarConfig tbc = new();
                tbc.Init(screenName, _registryRootTaskbar, true);
                _listScreens.Add(tbc);
            }
        }
    }

    public static TaskbarConfig GetTaskbarConfig(string screenName)
    {
        TaskbarConfig tbc = _listScreens.SingleOrDefault(tbc => tbc.TaskbarName == screenName);
        if (tbc != null)
            return tbc;
        tbc = new();
        tbc.Init(screenName, _registryRootTaskbar, AllowWrite);
        _listScreens.Add(tbc);
        return tbc;
    }

    public static string Theme
    {
        get { return _registryKeyExplorer.GetValue("Theme", "").ToString(); }
        set
        {
            if (Theme != value && AllowWrite)
                _registryKeyExplorer.SetValue("Theme", value);
        }
    }

    public static string Language
    {
        get { return _registryKeyExplorer.GetValue("Language", "").ToString(); }
        set
        {
            if (Language != value && AllowWrite)
                _registryKeyExplorer.SetValue("Language", value);
        }
    }

    public static bool ShowNotificationCopyOperation
    {
        get { return _registryKeyExplorer.ReadBoolean("ShowNotificationCopyOperation"); }
        set
        {
            if (ShowNotificationCopyOperation != value && AllowWrite)
                _registryKeyExplorer.SetValue("ShowNotificationCopyOperation", value.ToString());
        }
    }

    public static bool HookCopy
    {
        get { return _registryKeyExplorer.ReadBoolean("HookCopy") && !ArgumentExists("withouthook"); }
        set
        {
            if (HookCopy != value && !ArgumentExists("withouthook") && AllowWrite)
                _registryKeyExplorer.SetValue("HookCopy", value.ToString());
        }
    }

    public static bool UseOwnCopier
    {
        get { return _registryKeyExplorer.ReadBoolean("UseOwnCopier") || ArgumentExists("useowncopier"); }
        set
        {
            if (UseOwnCopier != value && !ArgumentExists("useowncopier") && AllowWrite)
                _registryKeyExplorer.SetValue("UseOwnCopier", value.ToString());
        }
    }

    public static bool StartTwoExplorer
    {
        get { return _registryKeyExplorer.ReadBoolean("StartTwoExplorer"); }
        set
        {
            if (StartTwoExplorer != value && AllowWrite)
                _registryKeyExplorer.SetValue("StartTwoExplorer", value.ToString());
        }
    }

    public static WindowState ExplorerWindowState
    {
        get { return _registryKeyExplorer.ReadEnum<WindowState>("ExplorerWindowState"); }
        set
        {
            if (ExplorerWindowState != value && AllowWrite)
                _registryKeyExplorer.SetValue("ExplorerWindowState", ((int)value).ToString());
        }
    }

    public static int ExplorerPosX
    {
        get { return _registryKeyExplorer.ReadInteger("ExplorerPosX"); }
        set
        {
            if (ExplorerPosX != value && AllowWrite)
                _registryKeyExplorer.SetValue("ExplorerPosX", value.ToString());
        }
    }

    public static int ExplorerPosY
    {
        get { return _registryKeyExplorer.ReadInteger("ExplorerPosY"); }
        set
        {
            if (ExplorerPosY != value && AllowWrite)
                _registryKeyExplorer.SetValue("ExplorerPosY", value.ToString());
        }
    }

    public static int ExplorerSizeX
    {
        get { return _registryKeyExplorer.ReadInteger("ExplorerSizeX"); }
        set
        {
            if (ExplorerSizeX != value && AllowWrite)
                _registryKeyExplorer.SetValue("ExplorerSizeX", value.ToString());
        }
    }

    public static int ExplorerSizeY
    {
        get { return _registryKeyExplorer.ReadInteger("ExplorerSizeY"); }
        set
        {
            if (ExplorerSizeY != value && AllowWrite)
                _registryKeyExplorer.SetValue("ExplorerSizeY", value.ToString());
        }
    }

    public static string[] ToolbarsPath
    {
        get
        {
            // Loop to find all toolbars path
            string[] listNames = [];
            _registryRootTaskbar.GetSubKeyNames().Where(s => s.Contains($"{ToolBarNameInRegistry}(")).ToList().ForEach(v => listNames = listNames.Add(_registryRootTaskbar.OpenSubKey(v).GetValue("Path")));
            return listNames;
        }
        set
        {
            // First, remove all olders toolbars path
            _registryRootTaskbar.GetSubKeyNames().Where(s => s.Contains($"{ToolBarNameInRegistry}(")).ToList().ForEach(v => _registryRootTaskbar.DeleteSubKey(v));
            // Then loop to save each new one
            int i = 0;
            value.ToList().ForEach(s =>
            {
                _registryRootTaskbar.CreateSubKey($"{ToolBarNameInRegistry}({i})", true).SetValue("Path", value[i]);
                i++;
            });
        }
    }

    public static int ToolbarNumber(string path)
    {
        int pos = -1;
        _registryRootTaskbar.GetSubKeyNames().Where(s => s.Contains($"{ToolBarNameInRegistry}(")).ToList().ForEach(v =>
        {
            if (_registryRootTaskbar.OpenSubKey(v).GetValue("Path")?.ToString() == path)
                pos = int.Parse(v.Replace($"{ToolBarNameInRegistry}(", "").Replace(")", ""));
        });
        return pos;
    }

    internal static int MaxToolbar()
    {
        int max = -1;
        int min = -1;
        _registryRootTaskbar.GetSubKeyNames().Where(s => s.Contains($"{ToolBarNameInRegistry}(")).ToList().ForEach(v =>
        {
            int pos = int.Parse(v.Replace($"{ToolBarNameInRegistry}(", "").Replace(")", ""));
            max = Math.Max(max, pos);
            min = Math.Min(min, pos);
        });
        if (min > 0)
            return min - 1;
        return max;
    }

    public static RegistryKey MyRegistryKey
    {
        get { return _registryKeyExplorer; }
    }

    public static RegistryKey DesktopRegistryKey
    {
        get { return _registryDesktop; }
    }

    public static RegistryKey TaskbarRegistryKey
    {
        get { return _registryRootTaskbar; }
    }

    public static string[] LeftTabs
    {
        get { return GetAllTabs("LeftTab"); }
        set
        {
            if (AllowWrite)
                SaveAllTabs("LeftTab", value);
        }
    }

    public static string[] RightTabs
    {
        get { return GetAllTabs("RightTab"); }
        set
        {
            if (AllowWrite)
                SaveAllTabs("RightTab", value);
        }
    }

    private static string[] GetAllTabs(string tabName)
    {
        List<string> allTabs = [];
        RegistryKey hkey = Registry.CurrentUser.OpenSubKey($"{HKeyRoot}\\{tabName}");
        if (hkey != null)
            foreach (string name in hkey.GetValueNames().Where(name => name.StartsWith("Tab(")))
                allTabs.Add(hkey.GetValue(name).ToString());
        return [.. allTabs];
    }

    private static void SaveAllTabs(string hkeyPath, string[] listTabs)
    {
        RegistryKey hkey = Registry.CurrentUser.CreateSubKey($"{HKeyRoot}\\{hkeyPath}", true);
        foreach (string name in hkey.GetValueNames().Where(name => name.StartsWith("Tab(")))
            hkey.DeleteValue(name);
        int i = 0;
        foreach (string path in listTabs)
            hkey.SetValue($"Tab({i++})", path);
    }

    public static string[] AllPinnedSystray()
    {
        string[] ret = [];
        RegistryKey key = _registryRootTaskbar.CreateSubKey("Systray");
        foreach (string name in key.GetValueNames())
        {
            ret = ret.Add(key.GetValue(name));
        }
        return ret;
    }

    public static void RemovePinnedSystray(string path)
    {
        RegistryKey key = _registryRootTaskbar.CreateSubKey("Systray");
        foreach (string name in key.GetValueNames())
            if (key.GetValue(name).ToString() == path)
                key.DeleteValue(name);
    }

    public static void AddPinnedSystray(string path, int order = -1)
    {
        RegistryKey key = _registryRootTaskbar.CreateSubKey("Systray", true);

        foreach (string name in key.GetValueNames())
            if (key.GetValue(name).ToString() == path)
                key.DeleteValue(name);

        if (order >= 0 && key.GetValueNames().Contains(order.ToString()))
        {
            string previousPath = key.GetValue(order.ToString()).ToString();
            int i;
            for (i = order; i <= 256; i++)
                if (!key.GetValueNames().Contains(i.ToString()))
                    break;
            if (i == 256)
                return;
            key.SetValue(i.ToString(), previousPath);
            key.SetValue(order.ToString(), path);
            return;
        }

        if (order < 0 || key.GetValueNames().Contains(order.ToString()))
            foreach (string name in key.GetValueNames())
                if (int.TryParse(name, out int current))
                    order = Math.Max(order, current);

        key.SetValue(order.ToString(), path);
    }

    public static bool TaskbarReplaceStartMenu
    {
        get { return _registryRootTaskbar.ReadBoolean("ReplaceStartMenu"); }
        set
        {
            if (TaskbarReplaceStartMenu != value && AllowWrite)
                _registryRootTaskbar.SetValue("ReplaceStartMenu", value.ToString());
        }
    }

    public static int TaskbarDelayBeforeShowThumbnail
    {
        get { return _registryRootTaskbar.ReadInteger("DelayBeforeShowThumbnail"); }
        set
        {
            if (TaskbarDelayBeforeShowThumbnail != value && AllowWrite)
                _registryRootTaskbar.SetValue("DelayBeforeShowThumbnail", value.ToString());
        }
    }

    public static bool StartMenuShowPinnedApp2
    {
        get { return _registryStartMenu.ReadBoolean("ShowPinnedApp2"); }
        set
        {
            if (StartMenuShowPinnedApp2 != value && AllowWrite)
                _registryStartMenu.SetValue("ShowPinnedApp2", value.ToString());
        }
    }

    public static bool ShowApplicationsPrograms
    {
        get { return _registryStartMenu.ReadBoolean("ShowApplicationsPrograms"); }
        set
        {
            if (ShowApplicationsPrograms != value && AllowWrite)
                _registryStartMenu.SetValue("ShowApplicationsPrograms", value.ToString());
        }
    }

    public static string StartMenuPinnedShortcutPath
    {
        get { return _registryStartMenu.GetValue("PinnedAppPath", "").ToString(); }
        set
        {
            if (StartMenuPinnedShortcutPath != value && AllowWrite)
                _registryStartMenu.SetValue("PinnedAppPath", value);
        }
    }

    public static string StartMenuPinnedShortcutPath2
    {
        get { return _registryStartMenu.GetValue("PinnedAppPath2", "").ToString(); }
        set
        {
            if (StartMenuPinnedShortcutPath2 != value && AllowWrite)
                _registryStartMenu.SetValue("PinnedAppPath2", value);
        }
    }

    public static double StartMenuIconSizeWidth
    {
        get { return _registryStartMenu.ReadDouble("IconSizeWidth"); }
        set
        {
            if (StartMenuIconSizeWidth != value && AllowWrite)
                _registryStartMenu.SetValue("IconSizeWidth", value.ToString());
        }
    }

    public static double StartMenuIconSizeHeight
    {
        get { return _registryStartMenu.ReadDouble("IconSizeHeight"); }
        set
        {
            if (StartMenuIconSizeHeight != value && AllowWrite)
                _registryStartMenu.SetValue("IconSizeHeight", value.ToString());
        }
    }

    public static double StartMenuIconSizeWidth2
    {
        get { return _registryStartMenu.ReadDouble("IconSizeWidth2"); }
        set
        {
            if (StartMenuIconSizeWidth2 != value && AllowWrite)
                _registryStartMenu.SetValue("IconSizeWidth2", value.ToString());
        }
    }

    public static double StartMenuIconSizeHeight2
    {
        get { return _registryStartMenu.ReadDouble("IconSizeHeight2"); }
        set
        {
            if (StartMenuIconSizeHeight2 != value && AllowWrite)
                _registryStartMenu.SetValue("IconSizeHeight2", value.ToString());
        }
    }

    public static SolidColorBrush StartMenuBackground
    {
        get
        {
            string bgColor = _registryStartMenu.GetValue("BackgroundColor")?.ToString();
            if (!string.IsNullOrWhiteSpace(bgColor))
            {
                return new SolidColorBrush(_registryStartMenu.ReadColor("BackgroundColor", ExploripSharedCopy.Constants.Colors.BackgroundColor));
            }
            return null;
        }
        set
        {
            if (AllowWrite)
                _registryStartMenu.SetValue("BackgroundColor", $"{value.Color.A},{value.Color.R},{value.Color.G},{value.Color.B}");
        }
    }
}
