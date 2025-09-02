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

            // Explorer
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

            // Start Menu
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
            if (string.IsNullOrWhiteSpace(_registryStartMenu.GetValue("StartMenuHeight", "").ToString()))
                _registryStartMenu.SetValue("StartMenuHeight", "640");
            if (string.IsNullOrWhiteSpace(_registryStartMenu.GetValue("CornerRadius", "").ToString()))
                _registryStartMenu.SetValue("CornerRadius", "10");

            // Taskbar
            if (string.IsNullOrWhiteSpace(_registryRootTaskbar.GetValue("DelayBeforeShowThumbnail", "").ToString()))
                _registryRootTaskbar.SetValue("DelayBeforeShowThumbnail", "1000");
            if (string.IsNullOrWhiteSpace(_registryRootTaskbar.GetValue("GroupedApplicationWindow", "").ToString()))
                _registryRootTaskbar.SetValue("GroupedApplicationWindow", "True");
            if (string.IsNullOrWhiteSpace(_registryRootTaskbar.GetValue("MaxWidthTitleApplicationWindow", "").ToString()))
                _registryRootTaskbar.SetValue("MaxWidthTitleApplicationWindow", "100");
            if (string.IsNullOrWhiteSpace(_registryRootTaskbar.GetValue("MaxRecursiveSubFolderInToolbar", "").ToString()))
                _registryRootTaskbar.SetValue("MaxRecursiveSubFolderInToolbar", "5");
            if (string.IsNullOrWhiteSpace(_registryRootTaskbar.GetValue("ProgressBarHeight", "").ToString()))
                _registryRootTaskbar.SetValue("ProgressBarHeight", "5");
            if (string.IsNullOrWhiteSpace(_registryRootTaskbar.GetValue("ReduceTitleWidthWhenTaskbarFull", "").ToString()))
                _registryRootTaskbar.SetValue("ReduceTitleWidthWhenTaskbarFull", "True");
            if (string.IsNullOrWhiteSpace(_registryRootTaskbar.GetValue("MarginTitleApplicationWindow", "").ToString()))
                _registryRootTaskbar.SetValue("MarginTitleApplicationWindow", "5");
            if (string.IsNullOrWhiteSpace(_registryRootTaskbar.GetValue("TaskButtonSelectedColor", "").ToString()))
                _registryRootTaskbar.SetValue("TaskButtonSelectedColor", $"128,{ExploripSharedCopy.Constants.Colors.SelectedBackgroundShellObject.Color.R},{ExploripSharedCopy.Constants.Colors.SelectedBackgroundShellObject.Color.G},{ExploripSharedCopy.Constants.Colors.SelectedBackgroundShellObject.Color.B}");
            if (string.IsNullOrWhiteSpace(_registryRootTaskbar.GetValue("TaskButtonProgressBarColor", "").ToString()))
                _registryRootTaskbar.SetValue("TaskButtonProgressBarColor", $"255,{Colors.Green.R},{Colors.Green.G},{Colors.Green.B}");
            if (string.IsNullOrWhiteSpace(_registryRootTaskbar.GetValue("TaskButtonCornerRadius", "").ToString()))
                _registryRootTaskbar.SetValue("TaskButtonCornerRadius", "6");
            if (string.IsNullOrWhiteSpace(_registryRootTaskbar.GetValue("DateFormat", "").ToString()))
                _registryRootTaskbar.SetValue("DateFormat", "<%dayofweek%>\\r<%t%>\\r<%d%>");
            if (string.IsNullOrWhiteSpace(_registryRootTaskbar.GetValue("PopUpCornerRadius", "").ToString()))
                _registryRootTaskbar.SetValue("PopUpCornerRadius", "6");
            if (string.IsNullOrWhiteSpace(_registryRootTaskbar.GetValue("ThumbnailCornerRadius", "").ToString()))
                _registryRootTaskbar.SetValue("ThumbnailCornerRadius", "10");
            if (string.IsNullOrWhiteSpace(_registryRootTaskbar.GetValue("SpaceBetweenThumbnail", "").ToString()))
                _registryRootTaskbar.SetValue("SpaceBetweenThumbnail", "10");
            if (string.IsNullOrWhiteSpace(_registryRootTaskbar.GetValue("LockOnMonitorPowerOff", "").ToString()))
                _registryRootTaskbar.SetValue("LockOnMonitorPowerOff", "False");
            if (string.IsNullOrWhiteSpace(_registryRootTaskbar.GetValue("DelayBeforeCloseThumbnail", "").ToString()))
                _registryRootTaskbar.SetValue("DelayBeforeCloseThumbnail", "1000");
            if (string.IsNullOrWhiteSpace(_registryRootTaskbar.GetValue("HookTaskbarList", "").ToString()))
                _registryRootTaskbar.SetValue("HookTaskbarList", "True");

            foreach (int numScreen in Screen.AllScreens.Select(s => s.DisplayNumber))
            {
                TaskbarConfig tbc = new();
                tbc.Init(numScreen, _registryRootTaskbar, true);
                _listScreens.Add(tbc);
            }
        }
    }

    public static TaskbarConfig GetTaskbarConfig(int numScreen)
    {
        TaskbarConfig tbc = _listScreens.SingleOrDefault(tbc => tbc.NumScreen == numScreen);
        if (tbc != null)
            return tbc;
        tbc = new();
        tbc.Init(numScreen, _registryRootTaskbar, AllowWrite);
        _listScreens.Add(tbc);
        return tbc;
    }

    public static bool AutoLockOnMonitorPowerOff
    {
        get { return _registryRootTaskbar.ReadBoolean("LockOnMonitorPowerOff"); }
        set
        {
            if (AutoLockOnMonitorPowerOff != value && AllowWrite)
                _registryRootTaskbar.SetValue("LockOnMonitorPowerOff", value.ToString());
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

    #region Toolbars

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

    #endregion

    #region Properties Root Registry Key

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

    #endregion

    #region File Explorer tabs

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

    #endregion

    #region Systray

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
        RegistryKey key = _registryRootTaskbar.CreateSubKey("Systray", true);
        foreach (string name in key.GetValueNames().Where(name => key.GetValue(name).ToString() == path))
            key.DeleteValue(name);
    }

    public static void AddPinnedSystray(string path, int order = -1)
    {
        RegistryKey key = _registryRootTaskbar.CreateSubKey("Systray", true);

        foreach (string name in key.GetValueNames().Where(name => key.GetValue(name).ToString() == path))
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

    #endregion

    #region Taskbar

    public static string DateFormat
    {
        get { return _registryRootTaskbar.GetValue("DateFormat", "").ToString(); }
        set
        {
            if (DateFormat != value && AllowWrite)
                _registryRootTaskbar.SetValue("DateFormat", value.ToString());
        }
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

    public static int TaskbarDelayBeforeCloseThumbnail
    {
        get { return _registryRootTaskbar.ReadInteger("DelayBeforeCloseThumbnail"); }
        set
        {
            if (TaskbarDelayBeforeShowThumbnail != value && AllowWrite)
                _registryRootTaskbar.SetValue("DelayBeforeCloseThumbnail", value.ToString());
        }
    }

    public static bool GroupedApplicationWindow
    {
        get { return _registryRootTaskbar.ReadBoolean("GroupedApplicationWindow"); }
        set
        {
            if (GroupedApplicationWindow != value && AllowWrite)
                _registryRootTaskbar.SetValue("GroupedApplicationWindow", value.ToString());
        }
    }

    public static bool ShowTitleApplicationWindow
    {
        get { return _registryRootTaskbar.ReadBoolean("ShowTitleApplicationWindow"); }
        set
        {
            if (ShowTitleApplicationWindow != value && AllowWrite)
                _registryRootTaskbar.SetValue("ShowTitleApplicationWindow", value.ToString());
        }
    }

    public static double MaxWidthTitleApplicationWindow
    {
        get { return _registryRootTaskbar.ReadDouble("MaxWidthTitleApplicationWindow"); }
        set
        {
            if (MaxWidthTitleApplicationWindow != value && AllowWrite)
                _registryRootTaskbar.SetValue("MaxWidthTitleApplicationWindow", value.ToString());
        }
    }

    public static double MarginTitleApplicationWindow
    {
        get { return _registryRootTaskbar.ReadDouble("MarginTitleApplicationWindow"); }
        set
        {
            if (MarginTitleApplicationWindow != value && AllowWrite)
                _registryRootTaskbar.SetValue("MarginTitleApplicationWindow", value.ToString());
        }
    }

    public static SolidColorBrush TaskButtonSelectedColor
    {
        get
        {
            string bgColor = _registryStartMenu.GetValue("TaskButtonSelectedColor")?.ToString();
            if (!string.IsNullOrWhiteSpace(bgColor))
            {
                return new SolidColorBrush(_registryStartMenu.ReadColor("TaskButtonSelectedColor", ExploripSharedCopy.Constants.Colors.SelectedBackgroundShellObject.Color));
            }
            return null;
        }
        set
        {
            if (AllowWrite)
                _registryStartMenu.SetValue("TaskButtonSelectedColor", $"{value.Color.A},{value.Color.R},{value.Color.G},{value.Color.B}");
        }
    }

    public static SolidColorBrush TaskButtonProgressBarColor
    {
        get
        {
            string bgColor = _registryRootTaskbar.GetValue("TaskButtonProgressBarColor")?.ToString();
            if (!string.IsNullOrWhiteSpace(bgColor))
            {
                return new SolidColorBrush(_registryRootTaskbar.ReadColor("TaskButtonProgressBarColor", Colors.Green));
            }
            return null;
        }
        set
        {
            if (AllowWrite)
                _registryRootTaskbar.SetValue("TaskButtonProgressBarColor", $"{value.Color.A},{value.Color.R},{value.Color.G},{value.Color.B}");
        }
    }

    public static CornerRadius TaskButtonCornerRadius
    {
        get { return new CornerRadius(_registryRootTaskbar.ReadDouble("TaskButtonCornerRadius")); }
        set
        {
            if (TaskButtonCornerRadius.TopLeft != value.TopLeft && AllowWrite)
                _registryRootTaskbar.SetValue("TaskButtonCornerRadius", value.TopLeft.ToString());
        }
    }

    public static bool ReduceTitleWidthWhenTaskbarFull
    {
        get { return _registryRootTaskbar.ReadBoolean("ReduceTitleWidthWhenTaskbarFull"); }
        set
        {
            if (ReduceTitleWidthWhenTaskbarFull != value && AllowWrite)
                _registryRootTaskbar.SetValue("ReduceTitleWidthWhenTaskbarFull", value.ToString());
        }
    }

    public static int MaxRecursiveSubFolderInToolbar
    {
        get { return _registryRootTaskbar.ReadInteger("MaxRecursiveSubFolderInToolbar"); }
        set
        {
            if (MaxRecursiveSubFolderInToolbar != value && AllowWrite)
                _registryRootTaskbar.SetValue("MaxRecursiveSubFolderInToolbar", value.ToString());
        }
    }

    public static double TaskbarProgressBarHeight
    {
        get { return _registryRootTaskbar.ReadDouble("ProgressBarHeight"); }
        set
        {
            if (TaskbarProgressBarHeight != value && AllowWrite)
                _registryRootTaskbar.SetValue("ProgressBarHeight", value.ToString());
        }
    }

    public static CornerRadius PopUpCornerRadius
    {
        get { return new CornerRadius(_registryRootTaskbar.ReadDouble("PopUpCornerRadius")); }
        set
        {
            if (PopUpCornerRadius.TopLeft != value.TopLeft && AllowWrite)
                _registryRootTaskbar.SetValue("PopUpCornerRadius", value.TopLeft.ToString());
        }
    }

    public static CornerRadius ThumbnailCornerRadius
    {
        get { return new CornerRadius(_registryRootTaskbar.ReadDouble("ThumbnailCornerRadius")); }
        set
        {
            if (PopUpCornerRadius.TopLeft != value.TopLeft && AllowWrite)
                _registryRootTaskbar.SetValue("ThumbnailCornerRadius", value.TopLeft.ToString());
        }
    }

    public static double SpaceBetweenThumbnail
    {
        get { return _registryRootTaskbar.ReadDouble("SpaceBetweenThumbnail"); }
        set
        {
            if (SpaceBetweenThumbnail != value && AllowWrite)
                _registryRootTaskbar.SetValue("SpaceBetweenThumbnail", value.ToString());
        }
    }

    public static bool HookTaskbarList
    {
        get { return _registryRootTaskbar.ReadBoolean("HookTaskbarList"); }
        set
        {
            if (HookTaskbarList != value && AllowWrite)
                _registryRootTaskbar.SetValue("HookTaskbarList", value.ToString());
        }
    }

    #endregion

    #region StartMenu

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

    public static CornerRadius StartMenuCornerRadius
    {
        get { return new CornerRadius(_registryStartMenu.ReadDouble("CornerRadius")); }
        set
        {
            if (StartMenuCornerRadius != value && AllowWrite)
                _registryStartMenu.SetValue("CornerRadius", value.TopRight.ToString());
        }
    }

    public static double StartMenuHeight
    {
        get { return _registryStartMenu.ReadDouble("StartMenuHeight"); }
        set
        {
            if (StartMenuHeight != value && AllowWrite)
                _registryStartMenu.SetValue("StartMenuHeight", value.ToString());
        }
    }

    public static string CommonProgramsPath
    {
        get { return _registryStartMenu.GetValue("CommonProgramsPath", "")?.ToString(); }
        set
        {
            if (CommonProgramsPath != value && AllowWrite)
                _registryStartMenu.SetValue("CommonProgramsPath", value);
        }
    }

    public static string CurrentUserProgramsPath
    {
        get { return _registryStartMenu.GetValue("CurrentUserProgramsPath", "")?.ToString(); }
        set
        {
            if (CommonProgramsPath != value && AllowWrite)
                _registryStartMenu.SetValue("CurrentUserProgramsPath", value);
        }
    }

    #endregion
}
