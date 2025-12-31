using System;
using System.Collections.Generic;
using System.Globalization;
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
    #region Constants
    private const string HKeyRoot = "Software\\CoolBytes\\Explorip";
    internal const string ToolBarNameInRegistry = "Toolbar";
    private const string ConfigOverrideDefaultColor = "OverrideDefaultColor";
    private const string ConfigDefaultBackgroundColor = "DefaultBackgroundColor";
    private const string ConfigDefaultForegroundColor = "DefaultForegroundColor";
    private const string ConfigTheme = "Theme";
    private const string ConfigLanguage = "Language";
    private const string ConfigHookCopy = "HookCopy";
    private const string ConfigUseOwnCopier = "UseOwnCopier";
    private const string ConfigStartTwoExplorer = "StartTwoExplorer";
    private const string ConfigShowNotificationCopyOperation = "ShowNotificationCopyOperation";
    private const string ConfigExplorerPosX = "ExplorerPosX";
    private const string ConfigExplorerPosY = "ExplorerPosY";
    private const string ConfigExplorerSizeX = "ExplorerSizeX";
    private const string ConfigExplorerSizeY = "ExplorerSizeY";
    private const string ConfigDelayBeforeShowThumbnail = "DelayBeforeShowThumbnail";
    private const string ConfigGroupedApplicationWindow = "GroupedApplicationWindow";
    private const string ConfigMaxRecursiveSubFolderInToolbar = "MaxRecursiveSubFolderInToolbar";
    private const string ConfigProgressBarHeight = "ProgressBarHeight";
    private const string ConfigReduceTitleWidthWhenTaskbarFull = "ReduceTitleWidthWhenTaskbarFull";
    private const string ConfigMarginTitleApplicationWindow = "MarginTitleApplicationWindow";
    private const string ConfigTaskButtonSelectedColor = "TaskButtonSelectedColor";
    private const string ConfigTaskButtonProgressBarColor = "TaskButtonProgressBarColor";
    private const string ConfigTaskButtonCornerRadius = "TaskButtonCornerRadius";
    private const string ConfigDateFormat = "DateFormat";
    private const string ConfigPopUpCornerRadius = "PopUpCornerRadius";
    private const string ConfigThumbnailCornerRadius = "ThumbnailCornerRadius";
    private const string ConfigSpaceBetweenThumbnail = "SpaceBetweenThumbnail";
    private const string ConfigLockOnMonitorPowerOff = "LockOnMonitorPowerOff";
    private const string ConfigDelayBeforeCloseThumbnail = "DelayBeforeCloseThumbnail";
    private const string ConfigHookTaskbarList = "HookTaskbarList";
    private const string ConfigShowIconOnThumbnailWindow = "ShowIconOnThumbnailWindow";
    private const string ConfigDragGhostBorderSize = "DragGhostBorderSize";
    private const string ConfigDragGhostBorderColor = "DragGhostBorderColor";
    private const string ConfigDragGhostOpacity = "DragGhostOpacity";
    private const string ConfigSwitchToDesktopWhenDragEnter = "SwitchToDesktopWhenDragEnter";
    private const string ConfigMouseOverBackgroundColor = "MouseOverBackgroundColor";
    private const string ConfigReplaceStartMenu = "ReplaceStartMenu";
    #endregion

    public static bool AllowWrite { get; set; }
    private static RegistryKey _registryKeyExplorer, _registryRootDesktop, _registryRootTaskbar;
    private static readonly List<TaskbarConfig> _listTaskbar = [];
    private static readonly List<DesktopConfig> _listDesktop = [];
    private static readonly StartMenuConfig _startMenuConfig = new();

    public static void Init(bool allowWrite = true)
    {
        RegistryKey startMenuRegistry;

        AllowWrite = allowWrite && !ArgumentExists("disablewriteconfig");
        _registryKeyExplorer = Registry.CurrentUser.CreateSubKey(HKeyRoot, true);
        if (_registryKeyExplorer == null)
            AllowWrite = false;
        _registryRootDesktop = _registryKeyExplorer?.CreateSubKey("Desktops");
        _registryRootTaskbar = _registryKeyExplorer?.CreateSubKey("Taskbars");
        startMenuRegistry = _registryKeyExplorer?.CreateSubKey("StartMenu");

        if (allowWrite && _registryKeyExplorer != null)
        {
            // If empty config in registry, set default settings
            if (string.IsNullOrWhiteSpace(_registryKeyExplorer.GetValue(ConfigOverrideDefaultColor, "").ToString()))
                _registryKeyExplorer.SetValue(ConfigOverrideDefaultColor, "False");
            if (string.IsNullOrWhiteSpace(_registryKeyExplorer.GetValue(ConfigDefaultBackgroundColor, "").ToString()))
                _registryKeyExplorer.SetValue(ConfigDefaultBackgroundColor, $"255,{ExploripSharedCopy.Constants.Colors.BackgroundColor.R},{ExploripSharedCopy.Constants.Colors.BackgroundColor.G},{ExploripSharedCopy.Constants.Colors.BackgroundColor.B}");
            if (string.IsNullOrWhiteSpace(_registryKeyExplorer.GetValue(ConfigDefaultForegroundColor, "").ToString()))
                _registryKeyExplorer.SetValue(ConfigDefaultForegroundColor, $"255,{ExploripSharedCopy.Constants.Colors.ForegroundColor.R},{ExploripSharedCopy.Constants.Colors.ForegroundColor.G},{ExploripSharedCopy.Constants.Colors.ForegroundColor.B}");

            // Explorer
            if (string.IsNullOrWhiteSpace(_registryKeyExplorer.GetValue(ConfigTheme, "").ToString()))
                _registryKeyExplorer.SetValue(ConfigTheme, "System");
            if (string.IsNullOrWhiteSpace(_registryKeyExplorer.GetValue(ConfigLanguage, "").ToString()))
                _registryKeyExplorer.SetValue(ConfigLanguage, "System");
            if (string.IsNullOrWhiteSpace(_registryKeyExplorer.GetValue(ConfigHookCopy, "").ToString()))
                _registryKeyExplorer.SetValue(ConfigHookCopy, "True");
            if (string.IsNullOrWhiteSpace(_registryKeyExplorer.GetValue(ConfigUseOwnCopier, "").ToString()))
                _registryKeyExplorer.SetValue(ConfigUseOwnCopier, "True");
            if (string.IsNullOrWhiteSpace(_registryKeyExplorer.GetValue(ConfigStartTwoExplorer, "").ToString()))
                _registryKeyExplorer.SetValue(ConfigStartTwoExplorer, "True");
            if (string.IsNullOrWhiteSpace(_registryKeyExplorer.GetValue(ConfigShowNotificationCopyOperation, "").ToString()))
                _registryKeyExplorer.SetValue(ConfigShowNotificationCopyOperation, "True");
            string ws = _registryKeyExplorer.GetValue("ExplorerWindowState", "").ToString();
            if (string.IsNullOrWhiteSpace(ws) || !Enum.TryParse<WindowState>(ws, out _))
                ExplorerWindowState = WindowState.Normal;
            if (string.IsNullOrWhiteSpace(_registryKeyExplorer.GetValue(ConfigExplorerPosX, "").ToString()))
                _registryKeyExplorer.SetValue(ConfigExplorerPosX, (Screen.PrimaryScreen.WpfWorkingArea.X + 50).ToString());
            if (string.IsNullOrWhiteSpace(_registryKeyExplorer.GetValue(ConfigExplorerPosY, "").ToString()))
                _registryKeyExplorer.SetValue(ConfigExplorerPosY, (Screen.PrimaryScreen.WpfWorkingArea.Y + 50).ToString());
            if (string.IsNullOrWhiteSpace(_registryKeyExplorer.GetValue(ConfigExplorerSizeX, "").ToString()))
                _registryKeyExplorer.SetValue(ConfigExplorerSizeX, (Screen.PrimaryScreen.WpfWorkingArea.Width - 100).ToString());
            if (string.IsNullOrWhiteSpace(_registryKeyExplorer.GetValue(ConfigExplorerSizeY, "").ToString()))
                _registryKeyExplorer.SetValue(ConfigExplorerSizeY, (Screen.PrimaryScreen.WpfWorkingArea.Height - 100).ToString());

            // Taskbar
            if (string.IsNullOrWhiteSpace(_registryRootTaskbar.GetValue(ConfigDelayBeforeShowThumbnail, "").ToString()))
                _registryRootTaskbar.SetValue(ConfigDelayBeforeShowThumbnail, "1000");
            if (string.IsNullOrWhiteSpace(_registryRootTaskbar.GetValue(ConfigGroupedApplicationWindow, "").ToString()))
                _registryRootTaskbar.SetValue(ConfigGroupedApplicationWindow, "True");
            if (string.IsNullOrWhiteSpace(_registryRootTaskbar.GetValue(ConfigMaxRecursiveSubFolderInToolbar, "").ToString()))
                _registryRootTaskbar.SetValue(ConfigMaxRecursiveSubFolderInToolbar, "5");
            if (string.IsNullOrWhiteSpace(_registryRootTaskbar.GetValue(ConfigProgressBarHeight, "").ToString()))
                _registryRootTaskbar.SetValue(ConfigProgressBarHeight, "5");
            if (string.IsNullOrWhiteSpace(_registryRootTaskbar.GetValue(ConfigReduceTitleWidthWhenTaskbarFull, "").ToString()))
                _registryRootTaskbar.SetValue(ConfigReduceTitleWidthWhenTaskbarFull, "True");
            if (string.IsNullOrWhiteSpace(_registryRootTaskbar.GetValue(ConfigMarginTitleApplicationWindow, "").ToString()))
                _registryRootTaskbar.SetValue(ConfigMarginTitleApplicationWindow, "5");
            if (string.IsNullOrWhiteSpace(_registryRootTaskbar.GetValue(ConfigTaskButtonSelectedColor, "").ToString()))
                _registryRootTaskbar.SetValue(ConfigTaskButtonSelectedColor, $"128,{ExploripSharedCopy.Constants.Colors.SelectedBackgroundShellObject.Color.R},{ExploripSharedCopy.Constants.Colors.SelectedBackgroundShellObject.Color.G},{ExploripSharedCopy.Constants.Colors.SelectedBackgroundShellObject.Color.B}");
            if (string.IsNullOrWhiteSpace(_registryRootTaskbar.GetValue(ConfigTaskButtonProgressBarColor, "").ToString()))
                _registryRootTaskbar.SetValue(ConfigTaskButtonProgressBarColor, $"255,{Colors.Green.R},{Colors.Green.G},{Colors.Green.B}");
            if (string.IsNullOrWhiteSpace(_registryRootTaskbar.GetValue(ConfigTaskButtonCornerRadius, "").ToString()))
                _registryRootTaskbar.SetValue(ConfigTaskButtonCornerRadius, "6");
            if (string.IsNullOrWhiteSpace(_registryRootTaskbar.GetValue(ConfigDateFormat, "").ToString()))
                _registryRootTaskbar.SetValue(ConfigDateFormat, "<%dayofweek%>\\r<%t%>\\r<%d%>");
            if (string.IsNullOrWhiteSpace(_registryRootTaskbar.GetValue(ConfigPopUpCornerRadius, "").ToString()))
                _registryRootTaskbar.SetValue(ConfigPopUpCornerRadius, "6");
            if (string.IsNullOrWhiteSpace(_registryRootTaskbar.GetValue(ConfigThumbnailCornerRadius, "").ToString()))
                _registryRootTaskbar.SetValue(ConfigThumbnailCornerRadius, "10");
            if (string.IsNullOrWhiteSpace(_registryRootTaskbar.GetValue(ConfigSpaceBetweenThumbnail, "").ToString()))
                _registryRootTaskbar.SetValue(ConfigSpaceBetweenThumbnail, "10");
            if (string.IsNullOrWhiteSpace(_registryRootTaskbar.GetValue(ConfigLockOnMonitorPowerOff, "").ToString()))
                _registryRootTaskbar.SetValue(ConfigLockOnMonitorPowerOff, "False");
            if (string.IsNullOrWhiteSpace(_registryRootTaskbar.GetValue(ConfigDelayBeforeCloseThumbnail, "").ToString()))
                _registryRootTaskbar.SetValue(ConfigDelayBeforeCloseThumbnail, "1000");
            if (string.IsNullOrWhiteSpace(_registryRootTaskbar.GetValue(ConfigHookTaskbarList, "").ToString()))
                _registryRootTaskbar.SetValue(ConfigHookTaskbarList, "False");
            if (string.IsNullOrWhiteSpace(_registryRootTaskbar.GetValue(ConfigShowIconOnThumbnailWindow, "").ToString()))
                _registryRootTaskbar.SetValue(ConfigShowIconOnThumbnailWindow, "True");
            if (string.IsNullOrWhiteSpace(_registryRootTaskbar.GetValue(ConfigDragGhostBorderSize, "").ToString()))
                _registryRootTaskbar.SetValue(ConfigDragGhostBorderSize, "1");
            if (string.IsNullOrWhiteSpace(_registryRootTaskbar.GetValue(ConfigDragGhostBorderColor, "").ToString()))
                _registryRootTaskbar.SetValue(ConfigDragGhostBorderColor, $"255,{Colors.Red.R},{Colors.Red.G},{Colors.Red.B}");
            if (string.IsNullOrWhiteSpace(_registryRootTaskbar.GetValue(ConfigDragGhostOpacity, "").ToString()))
                _registryRootTaskbar.SetValue(ConfigDragGhostOpacity, "0.75");
            if (string.IsNullOrWhiteSpace(_registryRootTaskbar.GetValue(ConfigSwitchToDesktopWhenDragEnter, "").ToString()))
                _registryRootTaskbar.SetValue(ConfigSwitchToDesktopWhenDragEnter, "True");
            if (string.IsNullOrWhiteSpace(_registryRootTaskbar.GetValue(ConfigMouseOverBackgroundColor, "").ToString()))
                _registryRootTaskbar.SetValue(ConfigMouseOverBackgroundColor, $"255,{ExploripSharedCopy.Constants.Colors.SelectedBackgroundShellObject.Color.R},{ExploripSharedCopy.Constants.Colors.SelectedBackgroundShellObject.Color.G},{ExploripSharedCopy.Constants.Colors.SelectedBackgroundShellObject.Color.B}");
            if (string.IsNullOrWhiteSpace(_registryRootTaskbar.GetValue(ConfigReplaceStartMenu, "").ToString()))
                _registryRootTaskbar.SetValue(ConfigReplaceStartMenu, "True");

            foreach (int numScreen in Screen.AllScreens.Select(s => s.DisplayNumber))
            {
                GetTaskbarConfig(numScreen);
                GetDesktopConfig(numScreen);
            }

            _startMenuConfig.Init(startMenuRegistry, allowWrite);
        }
    }

    public static bool OverrideDefaultColor
    {
        get { return _registryKeyExplorer.ReadBoolean(ConfigOverrideDefaultColor); }
        set
        {
            if (OverrideDefaultColor != value && AllowWrite)
                _registryKeyExplorer.SetValue(ConfigOverrideDefaultColor, value.ToString());
        }
    }

    public static Color DefaultBackgroundColor
    {
        get
        {
            string bgColor = _registryKeyExplorer.GetValue(ConfigDefaultBackgroundColor)?.ToString();
            if (!string.IsNullOrWhiteSpace(bgColor))
            {
                return _registryKeyExplorer.ReadColor(ConfigDefaultBackgroundColor, ExploripSharedCopy.Constants.Colors.BackgroundColor);
            }
            return Brushes.Transparent.Color;
        }
        set
        {
            if (AllowWrite)
                _registryKeyExplorer.SetValue(ConfigDefaultBackgroundColor, $"{value.A},{value.R},{value.G},{value.B}");
        }
    }

    public static Color DefaultForegroundColor
    {
        get
        {
            string bgColor = _registryKeyExplorer.GetValue(ConfigDefaultForegroundColor)?.ToString();
            if (!string.IsNullOrWhiteSpace(bgColor))
            {
                return _registryKeyExplorer.ReadColor(ConfigDefaultForegroundColor, ExploripSharedCopy.Constants.Colors.ForegroundColor);
            }
            return Color.FromArgb(255, 128, 128, 128);
        }
        set
        {
            if (AllowWrite)
                _registryKeyExplorer.SetValue(ConfigDefaultForegroundColor, $"{value.A},{value.R},{value.G},{value.B}");
        }
    }

    public static Color DefaultSystemIconColor
    {
        get
        {
            string bgColor = _registryKeyExplorer.GetValue("DefaultSystemIconColor")?.ToString();
            if (!string.IsNullOrWhiteSpace(bgColor))
            {
                return _registryKeyExplorer.ReadColor("DefaultSystemIconColor", ExploripSharedCopy.Constants.Colors.ForegroundColor);
            }
            return ExploripSharedCopy.Constants.Colors.ForegroundColor;
        }
        set
        {
            if (AllowWrite)
                _registryKeyExplorer.SetValue("DefaultSystemIconColor", $"{value.A},{value.R},{value.G},{value.B}");
        }
    }

    public static TaskbarConfig GetTaskbarConfig(string id)
    {
        TaskbarConfig tbc = _listTaskbar.SingleOrDefault(tbc => tbc.GetUniqueId == id);
        if (tbc != null)
            return tbc;
        foreach (string subkey in _registryRootTaskbar.GetSubKeyNames().Where(s => s.ToLower().StartsWith("display")))
        {
            string uniqueId = _registryRootTaskbar.OpenSubKey(subkey).GetValue("")?.ToString();
            if (uniqueId == id)
            {
                tbc = new();
                tbc.Init(int.Parse(subkey.Substring(7)), _registryRootTaskbar, AllowWrite, id);
                _listTaskbar.Add(tbc);
                return tbc;
            }
        }
        tbc = new();
        tbc.Init(Screen.AllScreens.Single(s => s.Id == id).DisplayNumber, _registryRootTaskbar, AllowWrite, id);
        _listTaskbar.Add(tbc);
        return tbc;
    }

    public static TaskbarConfig GetTaskbarConfig(int numScreen)
    {
        TaskbarConfig tbc;
        Screen screen = Screen.AllScreens.SingleOrDefault(s => s.DisplayNumber == numScreen);
        if (screen != null)
        {
            tbc = _listTaskbar.SingleOrDefault(tbc => tbc.GetUniqueId == screen.Id);
            if (tbc != null)
                return tbc;
        }
        return GetTaskbarConfig(Screen.AllScreens.Single(s => s.DisplayNumber == numScreen).Id);
    }

    public static DesktopConfig GetDesktopConfig(string id)
    {
        DesktopConfig dtc = _listDesktop.SingleOrDefault(tbc => tbc.GetUniqueId == id);
        if (dtc != null)
            return dtc;
        foreach (string subkey in _registryRootDesktop.GetSubKeyNames().Where(s => s.ToLower().StartsWith("display")))
        {
            string uniqueId = _registryRootDesktop.OpenSubKey(subkey).GetValue("", "").ToString();
            if (uniqueId == id)
            {
                dtc = new();
                dtc.Init(int.Parse(subkey.Substring(7)), _registryRootDesktop, AllowWrite, id);
                _listDesktop.Add(dtc);
                return dtc;
            }
        }
        dtc = new();
        dtc.Init(Screen.AllScreens.Single(s => s.Id == id).DisplayNumber, _registryRootDesktop, AllowWrite, id);
        _listDesktop.Add(dtc);
        return dtc;
    }

    public static DesktopConfig GetDesktopConfig(int numScreen)
    {
        DesktopConfig dtc;
        Screen screen = Screen.AllScreens.SingleOrDefault(s => s.DisplayNumber == numScreen);
        if (screen != null)
        {
            dtc = _listDesktop.SingleOrDefault(dtc => dtc.GetUniqueId == screen.Id);
            if (dtc != null)
                return dtc;
        }
        return GetDesktopConfig(Screen.AllScreens.Single(s => s.DisplayNumber == numScreen).Id);
    }

    public static int GetDesktopScreen(string itemName)
    {
        foreach (DesktopConfig dc in _listDesktop)
        {
            (int, int) position = dc.GetItemPosition(itemName);
            if (position.Item1 >= 0 && position.Item2 >= 0)
                return dc.NumScreen;
        }
        return -1;
    }

    public static bool AutoLockOnMonitorPowerOff
    {
        get { return _registryRootTaskbar.ReadBoolean(ConfigLockOnMonitorPowerOff); }
        set
        {
            if (AutoLockOnMonitorPowerOff != value && AllowWrite)
                _registryRootTaskbar.SetValue(ConfigLockOnMonitorPowerOff, value.ToString());
        }
    }

    public static bool ShowDesktopPreviewAllMonitors
    {
        get { return _registryRootTaskbar.ReadBoolean("ShowDesktopPreviewAllMonitors"); }
        set
        {
            if (ShowDesktopPreviewAllMonitors != value && AllowWrite)
                _registryRootTaskbar.SetValue("ShowDesktopPreviewAllMonitors", value.ToString());
        }
    }

    public static string Theme
    {
        get { return _registryKeyExplorer.GetValue(ConfigTheme, "").ToString(); }
        set
        {
            if (Theme != value && AllowWrite)
                _registryKeyExplorer.SetValue(ConfigTheme, value);
        }
    }

    public static string Language
    {
        get { return _registryKeyExplorer.GetValue(ConfigLanguage, "").ToString(); }
        set
        {
            if (Language != value && AllowWrite)
                _registryKeyExplorer.SetValue(ConfigLanguage, value);
        }
    }

    public static bool ShowNotificationCopyOperation
    {
        get { return _registryKeyExplorer.ReadBoolean(ConfigShowNotificationCopyOperation); }
        set
        {
            if (ShowNotificationCopyOperation != value && AllowWrite)
                _registryKeyExplorer.SetValue(ConfigShowNotificationCopyOperation, value.ToString());
        }
    }

    public static bool HookCopy
    {
        get { return _registryKeyExplorer.ReadBoolean(ConfigHookCopy) && !ArgumentExists("withouthook"); }
        set
        {
            if (HookCopy != value && !ArgumentExists("withouthook") && AllowWrite)
                _registryKeyExplorer.SetValue(ConfigHookCopy, value.ToString());
        }
    }

    public static bool UseOwnCopier
    {
        get { return _registryKeyExplorer.ReadBoolean(ConfigUseOwnCopier) || ArgumentExists("useowncopier"); }
        set
        {
            if (UseOwnCopier != value && !ArgumentExists("useowncopier") && AllowWrite)
                _registryKeyExplorer.SetValue(ConfigUseOwnCopier, value.ToString());
        }
    }

    public static bool StartTwoExplorer
    {
        get { return _registryKeyExplorer.ReadBoolean(ConfigStartTwoExplorer); }
        set
        {
            if (StartTwoExplorer != value && AllowWrite)
                _registryKeyExplorer.SetValue(ConfigStartTwoExplorer, value.ToString());
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
        get { return _registryKeyExplorer.ReadInteger(ConfigExplorerPosX); }
        set
        {
            if (ExplorerPosX != value && AllowWrite)
                _registryKeyExplorer.SetValue(ConfigExplorerPosX, value.ToString());
        }
    }

    public static int ExplorerPosY
    {
        get { return _registryKeyExplorer.ReadInteger(ConfigExplorerPosY); }
        set
        {
            if (ExplorerPosY != value && AllowWrite)
                _registryKeyExplorer.SetValue(ConfigExplorerPosY, value.ToString());
        }
    }

    public static int ExplorerSizeX
    {
        get { return _registryKeyExplorer.ReadInteger(ConfigExplorerSizeX); }
        set
        {
            if (ExplorerSizeX != value && AllowWrite)
                _registryKeyExplorer.SetValue(ConfigExplorerSizeX, value.ToString());
        }
    }

    public static int ExplorerSizeY
    {
        get { return _registryKeyExplorer.ReadInteger(ConfigExplorerSizeY); }
        set
        {
            if (ExplorerSizeY != value && AllowWrite)
                _registryKeyExplorer.SetValue(ConfigExplorerSizeY, value.ToString());
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
        get { return _registryRootDesktop; }
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
        get { return _registryRootTaskbar.GetValue(ConfigDateFormat, "").ToString(); }
        set
        {
            if (DateFormat != value && AllowWrite)
                _registryRootTaskbar.SetValue(ConfigDateFormat, value.ToString());
        }
    }

    public static bool TaskbarReplaceStartMenu
    {
        get { return _registryRootTaskbar.ReadBoolean(ConfigReplaceStartMenu); }
        set
        {
            if (TaskbarReplaceStartMenu != value && AllowWrite)
                _registryRootTaskbar.SetValue(ConfigReplaceStartMenu, value.ToString());
        }
    }

    public static int TaskbarDelayBeforeShowThumbnail
    {
        get { return _registryRootTaskbar.ReadInteger(ConfigDelayBeforeShowThumbnail); }
        set
        {
            if (TaskbarDelayBeforeShowThumbnail != value && AllowWrite)
                _registryRootTaskbar.SetValue(ConfigDelayBeforeShowThumbnail, value.ToString());
        }
    }

    public static int TaskbarDelayBeforeCloseThumbnail
    {
        get { return _registryRootTaskbar.ReadInteger(ConfigDelayBeforeCloseThumbnail); }
        set
        {
            if (TaskbarDelayBeforeShowThumbnail != value && AllowWrite)
                _registryRootTaskbar.SetValue(ConfigDelayBeforeCloseThumbnail, value.ToString());
        }
    }

    public static bool GroupedApplicationWindow
    {
        get { return _registryRootTaskbar.ReadBoolean(ConfigGroupedApplicationWindow); }
        set
        {
            if (GroupedApplicationWindow != value && AllowWrite)
                _registryRootTaskbar.SetValue(ConfigGroupedApplicationWindow, value.ToString());
        }
    }

    public static double MarginTitleApplicationWindow
    {
        get { return _registryRootTaskbar.ReadDouble(ConfigMarginTitleApplicationWindow); }
        set
        {
            if (MarginTitleApplicationWindow != value && AllowWrite)
                _registryRootTaskbar.SetValue(ConfigMarginTitleApplicationWindow, value.ToString(CultureInfo.InvariantCulture));
        }
    }

    public static SolidColorBrush TaskButtonSelectedColor
    {
        get
        {
            string bgColor = _registryRootTaskbar.GetValue(ConfigTaskButtonSelectedColor)?.ToString();
            if (!string.IsNullOrWhiteSpace(bgColor))
            {
                return new SolidColorBrush(_registryRootTaskbar.ReadColor(ConfigTaskButtonSelectedColor, ExploripSharedCopy.Constants.Colors.SelectedBackgroundShellObject.Color));
            }
            return null;
        }
        set
        {
            if (AllowWrite)
                _registryRootTaskbar.SetValue(ConfigTaskButtonSelectedColor, $"{value.Color.A},{value.Color.R},{value.Color.G},{value.Color.B}");
        }
    }

    public static SolidColorBrush TaskButtonProgressBarColor
    {
        get
        {
            string bgColor = _registryRootTaskbar.GetValue(ConfigTaskButtonProgressBarColor)?.ToString();
            if (!string.IsNullOrWhiteSpace(bgColor))
            {
                return new SolidColorBrush(_registryRootTaskbar.ReadColor(ConfigTaskButtonProgressBarColor, Colors.Green));
            }
            return null;
        }
        set
        {
            if (AllowWrite)
                _registryRootTaskbar.SetValue(ConfigTaskButtonProgressBarColor, $"{value.Color.A},{value.Color.R},{value.Color.G},{value.Color.B}");
        }
    }

    public static SolidColorBrush MouseOverBackgroundColor
    {
        get
        {
            string bgColor = _registryRootTaskbar.GetValue(ConfigMouseOverBackgroundColor)?.ToString();
            if (!string.IsNullOrWhiteSpace(bgColor))
            {
                return new SolidColorBrush(_registryRootTaskbar.ReadColor(ConfigMouseOverBackgroundColor, Colors.DarkGray));
            }
            return null;
        }
        set
        {
            if (AllowWrite)
                _registryRootTaskbar.SetValue(ConfigMouseOverBackgroundColor, $"{value.Color.A},{value.Color.R},{value.Color.G},{value.Color.B}");
        }
    }

    public static CornerRadius TaskButtonCornerRadius
    {
        get { return new CornerRadius(_registryRootTaskbar.ReadDouble(ConfigTaskButtonCornerRadius)); }
        set
        {
            if (TaskButtonCornerRadius.TopLeft != value.TopLeft && AllowWrite)
                _registryRootTaskbar.SetValue(ConfigTaskButtonCornerRadius, value.TopLeft.ToString(CultureInfo.InvariantCulture));
        }
    }

    public static bool ReduceTitleWidthWhenTaskbarFull
    {
        get { return _registryRootTaskbar.ReadBoolean(ConfigReduceTitleWidthWhenTaskbarFull); }
        set
        {
            if (ReduceTitleWidthWhenTaskbarFull != value && AllowWrite)
                _registryRootTaskbar.SetValue(ConfigReduceTitleWidthWhenTaskbarFull, value.ToString());
        }
    }

    public static int MaxRecursiveSubFolderInToolbar
    {
        get { return _registryRootTaskbar.ReadInteger(ConfigMaxRecursiveSubFolderInToolbar); }
        set
        {
            if (MaxRecursiveSubFolderInToolbar != value && AllowWrite)
                _registryRootTaskbar.SetValue(ConfigMaxRecursiveSubFolderInToolbar, value.ToString());
        }
    }

    public static double TaskbarProgressBarHeight
    {
        get { return _registryRootTaskbar.ReadDouble(ConfigProgressBarHeight); }
        set
        {
            if (TaskbarProgressBarHeight != value && AllowWrite)
                _registryRootTaskbar.SetValue(ConfigProgressBarHeight, value.ToString(CultureInfo.InvariantCulture));
        }
    }

    public static CornerRadius PopUpCornerRadius
    {
        get { return new CornerRadius(_registryRootTaskbar.ReadDouble(ConfigPopUpCornerRadius)); }
        set
        {
            if (PopUpCornerRadius.TopLeft != value.TopLeft && AllowWrite)
                _registryRootTaskbar.SetValue(ConfigPopUpCornerRadius, value.TopLeft.ToString(CultureInfo.InvariantCulture));
        }
    }

    public static CornerRadius ThumbnailCornerRadius
    {
        get { return new CornerRadius(_registryRootTaskbar.ReadDouble(ConfigThumbnailCornerRadius)); }
        set
        {
            if (PopUpCornerRadius.TopLeft != value.TopLeft && AllowWrite)
                _registryRootTaskbar.SetValue(ConfigThumbnailCornerRadius, value.TopLeft.ToString(CultureInfo.InvariantCulture));
        }
    }

    public static bool ShowIconOnThumbnailWindow
    {
        get { return _registryRootTaskbar.ReadBoolean(ConfigShowIconOnThumbnailWindow); }
        set
        {
            if (ShowIconOnThumbnailWindow != value && AllowWrite)
                _registryRootTaskbar.SetValue(ConfigShowIconOnThumbnailWindow, value.ToString());
        }
    }

    public static double SpaceBetweenThumbnail
    {
        get { return _registryRootTaskbar.ReadDouble(ConfigSpaceBetweenThumbnail); }
        set
        {
            if (SpaceBetweenThumbnail != value && AllowWrite)
                _registryRootTaskbar.SetValue(ConfigSpaceBetweenThumbnail, value.ToString(CultureInfo.InvariantCulture));
        }
    }

    public static bool HookTaskbarList
    {
        get { return _registryRootTaskbar.ReadBoolean(ConfigHookTaskbarList); }
        set
        {
            if (HookTaskbarList != value && AllowWrite)
                _registryRootTaskbar.SetValue(ConfigHookTaskbarList, value.ToString());
        }
    }

    public static int DragGhostBorderSize
    {
        get { return _registryRootTaskbar.ReadInteger(ConfigDragGhostBorderSize); }
        set
        {
            if (DragGhostBorderSize != value && AllowWrite)
                _registryRootTaskbar.SetValue(ConfigDragGhostBorderSize, value.ToString());
        }
    }

    public static Color DragGhostBorderColor
    {
        get { return _registryRootTaskbar.ReadColor(ConfigDragGhostBorderColor, Colors.Transparent); }
        set
        {
            if (AllowWrite)
                _registryRootTaskbar.SetValue(ConfigDragGhostBorderColor, $"{value.A},{value.R},{value.G},{value.B}");
        }
    }

    public static double DragGhostOpacity
    {
        get { return _registryRootTaskbar.ReadDouble(ConfigDragGhostOpacity); }
        set
        {
            if (DragGhostOpacity != value && AllowWrite)
                _registryRootTaskbar.SetValue(ConfigDragGhostOpacity, value.ToString(CultureInfo.InvariantCulture));
        }
    }

    public static Pen DragGhostBorder
    {
        get
        {
            Pen ret = new()
            {
                Thickness = _registryRootTaskbar.ReadInteger(ConfigDragGhostBorderSize),
                Brush = new SolidColorBrush(_registryRootTaskbar.ReadColor(ConfigDragGhostBorderColor, Colors.Transparent)),
            };
            return ret;
        }
        set
        {
            if (AllowWrite)
            {
                if (DragGhostBorderSize != (int)value.Thickness)
                    DragGhostBorderSize = (int)value.Thickness;
                DragGhostBorderColor = ((SolidColorBrush)value.Brush).Color;
            }
        }
    }

    public static bool SwitchToDesktopWhenDragEnter
    {
        get { return _registryRootTaskbar.ReadBoolean(ConfigSwitchToDesktopWhenDragEnter); }
        set
        {
            if (SwitchToDesktopWhenDragEnter != value && AllowWrite)
                _registryRootTaskbar.SetValue(ConfigSwitchToDesktopWhenDragEnter, value.ToString());
        }
    }

    #endregion

    public static StartMenuConfig StartMenu
    {
        get { return _startMenuConfig; }
    }
}
