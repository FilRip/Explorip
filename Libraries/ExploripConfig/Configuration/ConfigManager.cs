using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

using ExploripConfig.Helpers;

using ManagedShell.AppBar;

using Microsoft.Win32;

using WpfScreenHelper;

using static ExploripConfig.Helpers.ExtensionsCommandLineArguments;

namespace ExploripConfig.Configuration;

public static class ConfigManager
{
    private const string HKeyRoot = "Software\\CoolBytes\\Explorip";

    public static bool AllowWrite { get; set; }
    private static RegistryKey _registryKeyExplorer, _registryDesktop, _registryTaskbar;

    public static void Init(bool allowWrite = true)
    {
        AllowWrite = allowWrite && !ArgumentExists("disablewriteconfig");
        _registryKeyExplorer = Registry.CurrentUser.CreateSubKey(HKeyRoot, true);
        if (_registryKeyExplorer == null)
            AllowWrite = false;
        _registryDesktop = _registryKeyExplorer?.CreateSubKey("Desktop");
        _registryTaskbar = _registryKeyExplorer?.CreateSubKey("Taskbar");

        if (allowWrite && _registryKeyExplorer != null)
        {
            // If empty config in registry, set default settings
            if (string.IsNullOrWhiteSpace(_registryKeyExplorer.GetValue("Theme", "").ToString()))
                _registryKeyExplorer.SetValue("Theme", "System");
            if (string.IsNullOrWhiteSpace(_registryTaskbar.GetValue("ShowClock", "").ToString()))
                _registryTaskbar.SetValue("ShowClock", "True");
            if (string.IsNullOrWhiteSpace(_registryTaskbar.GetValue("CollapseNotifyIcons", "").ToString()))
                _registryTaskbar.SetValue("CollapseNotifyIcons", "True");
            if (string.IsNullOrWhiteSpace(_registryKeyExplorer.GetValue("AllowFontSmoothing", "").ToString()))
                _registryKeyExplorer.SetValue("AllowFontSmoothing", "True");
            if (string.IsNullOrWhiteSpace(_registryKeyExplorer.GetValue("Language", "").ToString()))
                _registryKeyExplorer.SetValue("Language", "System");
            string edge = _registryTaskbar.GetValue("Edge", "").ToString();
            if (string.IsNullOrWhiteSpace(edge) || !Enum.TryParse<AppBarEdge>(edge, out _))
                Edge = AppBarEdge.Bottom;
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
        }
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

    public static bool ShowClock
    {
        get { return _registryTaskbar.ReadBoolean("ShowClock", true); }
        set
        {
            if (ShowClock != value && AllowWrite)
                _registryTaskbar.SetValue("ShowClock", value.ToString());
        }
    }

    public static bool CollapseNotifyIcons
    {
        get { return _registryTaskbar.ReadBoolean("CollapseNotifyIcons"); }
        set
        {
            if (CollapseNotifyIcons != value && AllowWrite)
                _registryTaskbar.SetValue("CollapseNotifyIcons", value.ToString());
        }
    }

    public static bool AllowFontSmoothing
    {
        get { return _registryKeyExplorer.ReadBoolean("AllowFontSmoothing"); }
        set
        {
            if (AllowFontSmoothing != value && AllowWrite)
                _registryKeyExplorer.SetValue("AllowFontSmoothing", value.ToString());
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

    public static AppBarEdge Edge
    {
        get { return _registryTaskbar.ReadEnum<AppBarEdge>("Edge"); }
        set
        {
            if (Edge != value && AllowWrite)
                _registryTaskbar.SetValue("Edge", ((int)value).ToString());
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

    public static int NbToolBar
    {
        get { return _registryTaskbar.ReadInteger("NbToolbar"); }
        set
        {
            if (NbToolBar != value && AllowWrite)
                _registryTaskbar.SetValue("NbToolbar", value.ToString());
        }
    }

    public static double TaskbarHeight
    {
        get { return _registryTaskbar.ReadDouble("TaskbarHeight"); }
        set
        {
            if (TaskbarHeight != value && AllowWrite)
                _registryTaskbar.SetValue("TaskbarHeight", value.ToString());
        }
    }

    public static double TaskbarWidth
    {
        get { return _registryTaskbar.ReadDouble("TaskbarWidth"); }
        set
        {
            if (TaskbarWidth != value && AllowWrite)
                _registryTaskbar.SetValue("TaskbarWidth", value.ToString());
        }
    }

    public static bool ShowTaskbarOnAllScreens
    {
        get { return _registryTaskbar.ReadBoolean("ShowTaskbarOnAllScreens"); }
        set
        {
            if (ShowTaskbarOnAllScreens != value && AllowWrite)
                _registryTaskbar.SetValue("ShowTaskbarOnAllScreens", value.ToString());
        }
    }

    public static bool ShowTaskManButton
    {
        get { return _registryTaskbar.ReadBoolean("ShowTaskMan"); }
        set
        {
            if (ShowTaskManButton != value && AllowWrite)
                _registryTaskbar.SetValue("ShowTaskMan", value.ToString());
        }
    }

    public static bool ShowSearchButton
    {
        get { return _registryTaskbar.ReadBoolean("ShowSearch"); }
        set
        {
            if (ShowSearchButton != value && AllowWrite)
                _registryTaskbar.SetValue("ShowSearch", value.ToString());
        }
    }

    public static bool ShowWidgetButton
    {
        get { return _registryTaskbar.ReadBoolean("ShowWidget"); }
        set
        {
            if (ShowWidgetButton != value && AllowWrite)
                _registryTaskbar.SetValue("ShowWidget", value.ToString());
        }
    }

    private const string ToolBarNameInRegistry = "Toolbar";
    public static string[] ToolbarsPath
    {
        get
        {
            // Loop to find all toolbars path
            string[] listNames = [];
            _registryTaskbar.GetSubKeyNames().Where(s => s.Contains($"{ToolBarNameInRegistry}(")).ToList().ForEach(v => listNames = listNames.Add(_registryTaskbar.OpenSubKey(v).GetValue("Path")));
            return listNames;
        }
        set
        {
            // First, remove all olders toolbars path
            _registryTaskbar.GetSubKeyNames().Where(s => s.Contains($"{ToolBarNameInRegistry}(")).ToList().ForEach(v => _registryTaskbar.DeleteSubKey(v));
            // Then loop to save each new one
            int i = 0;
            value.ToList().ForEach(s =>
            {
                _registryTaskbar.CreateSubKey($"{ToolBarNameInRegistry}({i})", true).SetValue("Path", value[i]);
                i++;
            });
        }
    }

    public static void ToolbarPosition(string path, Point position)
    {
        int i = ToolbarNumber(path);
        if (i < 0)
        {
            i = MaxToolbar();
            i++;
            _registryTaskbar.CreateSubKey($"{ToolBarNameInRegistry}({i})", true).SetValue("Path", path);
        }
        RegistryKey currentToolbar = _registryTaskbar.OpenSubKey($"{ToolBarNameInRegistry}({i})", true);
        currentToolbar.SetValue("X", position.X.ToString());
        currentToolbar.SetValue("Y", position.Y.ToString());
    }

    public static Point ToolbarPosition(string path)
    {
        Point ret = new();
        int i = ToolbarNumber(path);
        if (i >= 0)
        {
            RegistryKey currentToolbar = _registryTaskbar.OpenSubKey($"{ToolBarNameInRegistry}({i})");
            ret.X = currentToolbar.ReadDouble("X");
            ret.Y = currentToolbar.ReadDouble("Y");
        }
        return ret;
    }

    public static void ToolbarSmallSizeIcon(string path, bool smallSize)
    {
        int i = ToolbarNumber(path);
        if (i < 0)
        {
            i = MaxToolbar();
            i++;
            _registryTaskbar.CreateSubKey($"{ToolBarNameInRegistry}({i})", true).SetValue("Path", path);
        }
        RegistryKey currentToolbar = _registryTaskbar.OpenSubKey($"{ToolBarNameInRegistry}({i})", true);
        currentToolbar.SetValue("SmallSizeIcon", smallSize.ToString());
    }

    public static bool ToolbarSmallSizeIcon(string path)
    {
        bool ret = true;
        int i = ToolbarNumber(path);
        if (i >= 0)
        {
            RegistryKey currentToolbar = _registryTaskbar.OpenSubKey($"{ToolBarNameInRegistry}({i})");
            ret = currentToolbar.ReadBoolean("SmallSizeIcon");
        }
        return ret;
    }

    public static void ToolbarShowTitle(string path, bool showTitle)
    {
        int i = ToolbarNumber(path);
        if (i < 0)
        {
            i = MaxToolbar();
            i++;
            _registryTaskbar.CreateSubKey($"{ToolBarNameInRegistry}({i})", true).SetValue("Path", path);
        }
        RegistryKey currentToolbar = _registryTaskbar.OpenSubKey($"{ToolBarNameInRegistry}({i})", true);
        currentToolbar.SetValue("ShowTitle", showTitle.ToString());
    }

    public static bool ToolbarShowTitle(string path)
    {
        bool ret = true;
        int i = ToolbarNumber(path);
        if (i >= 0)
        {
            RegistryKey currentToolbar = _registryTaskbar.OpenSubKey($"{ToolBarNameInRegistry}({i})");
            ret = currentToolbar.ReadBoolean("ShowTitle");
        }
        return ret;
    }

    private static int ToolbarNumber(string path)
    {
        int pos = -1;
        _registryTaskbar.GetSubKeyNames().Where(s => s.Contains($"{ToolBarNameInRegistry}(")).ToList().ForEach(v =>
        {
            if (_registryTaskbar.OpenSubKey(v).GetValue("Path")?.ToString() == path)
                pos = int.Parse(v.Replace($"{ToolBarNameInRegistry}(", "").Replace(")", ""));
        });
        return pos;
    }

    private static int MaxToolbar()
    {
        int max = -1;
        _registryTaskbar.GetSubKeyNames().Where(s => s.Contains($"{ToolBarNameInRegistry}(")).ToList().ForEach(v =>
        {
            int pos = int.Parse(v.Replace($"{ToolBarNameInRegistry}(", "").Replace(")", ""));
            max = Math.Max(max, pos);
        });
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
        get { return _registryTaskbar; }
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
}
