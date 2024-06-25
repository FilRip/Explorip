using System;
using System.Windows;

using ExploripConfig.Helpers;

using Microsoft.Win32;

using WpfScreenHelper;

using static ExploripConfig.Helpers.ExtensionsCommandLineArguments;

namespace ExploripConfig.Configuration;

public static class ConfigManager
{
    public static bool AllowWrite { get; set; }
    private static RegistryKey _registryKey;

    public static void Init(bool allowWrite = true)
    {
        AllowWrite = allowWrite && !ArgumentExists("disablewriteconfig");
        _registryKey = Registry.CurrentUser.CreateSubKey("Software\\CoolBytes\\Explorip", true);
        if (_registryKey == null)
            AllowWrite = false;

        if (allowWrite && _registryKey != null)
        {
            // If empty config ini file, set default settings
            if (string.IsNullOrWhiteSpace(_registryKey.GetValue("Theme", "").ToString()))
                _registryKey.SetValue("Theme", "System");
            if (string.IsNullOrWhiteSpace(_registryKey.GetValue("ShowClock", "").ToString()))
                _registryKey.SetValue("ShowClock", "True");
            if (string.IsNullOrWhiteSpace(_registryKey.GetValue("ShowQuickLaunch", "").ToString()))
                _registryKey.SetValue("ShowQuickLaunch", "True");
            if (string.IsNullOrWhiteSpace(_registryKey.GetValue("QuickLaunchPath", "").ToString()))
                _registryKey.SetValue("QuickLaunchPath", "%USERPROFILE%\\QuickLaunch");
            if (string.IsNullOrWhiteSpace(_registryKey.GetValue("CollapseNotifyIcons", "").ToString()))
                _registryKey.SetValue("CollapseNotifyIcons", "True");
            if (string.IsNullOrWhiteSpace(_registryKey.GetValue("AllowFontSmoothing", "").ToString()))
                _registryKey.SetValue("AllowFontSmoothing", "True");
            if (string.IsNullOrWhiteSpace(_registryKey.GetValue("Language", "").ToString()))
                _registryKey.SetValue("Language", "System");
            if (string.IsNullOrWhiteSpace(_registryKey.GetValue("Edge", "").ToString()))
                _registryKey.SetValue("Edge", "3");
            if (string.IsNullOrWhiteSpace(_registryKey.GetValue("HookCopy", "").ToString()))
                _registryKey.SetValue("HookCopy", "True");
            if (string.IsNullOrWhiteSpace(_registryKey.GetValue("UseOwnCopier", "").ToString()))
                _registryKey.SetValue("UseOwnCopier", "True");
            if (string.IsNullOrWhiteSpace(_registryKey.GetValue("StartTwoExplorer", "").ToString()))
                _registryKey.SetValue("StartTwoExplorer", "True");
            if (string.IsNullOrWhiteSpace(_registryKey.GetValue("ShowNotificationCopyOperation", "").ToString()))
                _registryKey.SetValue("ShowNotificationCopyOperation", "True");
            if (string.IsNullOrWhiteSpace(_registryKey.GetValue("HideDesktopBackground", "").ToString()))
                _registryKey.SetValue("HideDesktopBackground", "False");
            string ws = _registryKey.GetValue("ExplorerWindowState", "").ToString();
            if (string.IsNullOrWhiteSpace(ws) || !Enum.TryParse<WindowState>(ws, out _))
                ExplorerWindowState = WindowState.Normal;
            if (string.IsNullOrWhiteSpace(_registryKey.GetValue("ExplorerPosX", "").ToString()))
                _registryKey.SetValue("ExplorerPosX", (Screen.PrimaryScreen.WpfWorkingArea.X + 50).ToString());
            if (string.IsNullOrWhiteSpace(_registryKey.GetValue("ExplorerPosY", "").ToString()))
                _registryKey.SetValue("ExplorerPosY", (Screen.PrimaryScreen.WpfWorkingArea.Y + 50).ToString());
            if (string.IsNullOrWhiteSpace(_registryKey.GetValue("ExplorerSizeX", "").ToString()))
                _registryKey.SetValue("ExplorerSizeX", (Screen.PrimaryScreen.WpfWorkingArea.Width - 100).ToString());
            if (string.IsNullOrWhiteSpace(_registryKey.GetValue("ExplorerSizeY", "").ToString()))
                _registryKey.SetValue("ExplorerSizeY", (Screen.PrimaryScreen.WpfWorkingArea.Height - 100).ToString());
        }
    }

    public static string Theme
    {
        get { return _registryKey.GetValue("Theme", "").ToString(); }
        set
        {
            if (Theme != value && AllowWrite)
                _registryKey.SetValue("Theme", value);
        }
    }

    public static bool ShowClock
    {
        get { return _registryKey.ReadBoolean("ShowClock", true); }
        set
        {
            if (ShowClock != value && AllowWrite)
                _registryKey.SetValue("ShowClock", value.ToString());
        }
    }

    public static bool ShowQuickLaunch
    {
        get { return _registryKey.ReadBoolean("ShowQuickLaunch", true); }
        set
        {
            if (ShowQuickLaunch != value && AllowWrite)
                _registryKey.SetValue("ShowQuickLaunch", value.ToString());
        }
    }

    public static string QuickLaunchPath
    {
        get { return _registryKey.GetValue("QuickLaunchPath", "").ToString(); }
        set
        {
            if (QuickLaunchPath != value && AllowWrite)
                _registryKey.SetValue("QuickLaunchPath", value);
        }
    }

    public static bool CollapseNotifyIcons
    {
        get { return _registryKey.ReadBoolean("CollapseNotifyIcons"); }
        set
        {
            if (CollapseNotifyIcons != value && AllowWrite)
                _registryKey.SetValue("CollapseNotifyIcons", value.ToString());
        }
    }

    public static bool AllowFontSmoothing
    {
        get { return _registryKey.ReadBoolean("AllowFontSmoothing"); }
        set
        {
            if (AllowFontSmoothing != value && AllowWrite)
                _registryKey.SetValue("AllowFontSmoothing", value.ToString());
        }
    }

    public static string Language
    {
        get { return _registryKey.GetValue("Language", "").ToString(); }
        set
        {
            if (Language != value && AllowWrite)
                _registryKey.SetValue("Language", value);
        }
    }

    public static int Edge // TODO : Enum ?
    {
        get { return _registryKey.ReadInteger("Edge"); }
        set
        {
            if (Edge != value && AllowWrite)
                _registryKey.SetValue("Edge", value.ToString());
        }
    }

    public static bool ShowNotificationCopyOperation
    {
        get { return _registryKey.ReadBoolean("ShowNotificationCopyOperation"); }
        set
        {
            if (ShowNotificationCopyOperation != value && AllowWrite)
                _registryKey.SetValue("ShowNotificationCopyOperation", value.ToString());
        }
    }

    public static bool HookCopy
    {
        get { return _registryKey.ReadBoolean("HookCopy") && !ArgumentExists("withouthook"); }
        set
        {
            if (HookCopy != value && !ArgumentExists("withouthook") && AllowWrite)
                _registryKey.SetValue("HookCopy", value.ToString());
        }
    }

    public static bool UseOwnCopier
    {
        get { return _registryKey.ReadBoolean("UseOwnCopier") || ArgumentExists("useowncopier"); }
        set
        {
            if (UseOwnCopier != value && !ArgumentExists("useowncopier") && AllowWrite)
                _registryKey.SetValue("UseOwnCopier", value.ToString());
        }
    }

    public static bool StartTwoExplorer
    {
        get { return _registryKey.ReadBoolean("StartTwoExplorer"); }
        set
        {
            if (StartTwoExplorer != value && AllowWrite)
                _registryKey.SetValue("StartTwoExplorer", value.ToString());
        }
    }

    public static bool HideDesktopBackground
    {
        get { return _registryKey.ReadBoolean("HideDesktopBackground"); }
        set
        {
            if (HideDesktopBackground != value && AllowWrite)
                _registryKey.SetValue("HideDesktopBackground", value.ToString());
        }
    }

    public static WindowState ExplorerWindowState
    {
        get { return _registryKey.ReadEnum<WindowState>("ExplorerWindowState"); }
        set
        {
            if (ExplorerWindowState != value && AllowWrite)
                _registryKey.SetValue("ExplorerWindowState", ((int)value).ToString());
        }
    }

    public static int ExplorerPosX
    {
        get { return _registryKey.ReadInteger("ExplorerPosX"); }
        set
        {
            if (ExplorerPosX != value && AllowWrite)
                _registryKey.SetValue("ExplorerPosX", value.ToString());
        }
    }

    public static int ExplorerPosY
    {
        get { return _registryKey.ReadInteger("ExplorerPosY"); }
        set
        {
            if (ExplorerPosY != value && AllowWrite)
                _registryKey.SetValue("ExplorerPosY", value.ToString());
        }
    }

    public static int ExplorerSizeX
    {
        get { return _registryKey.ReadInteger("ExplorerSizeX"); }
        set
        {
            if (ExplorerSizeX != value && AllowWrite)
                _registryKey.SetValue("ExplorerSizeX", value.ToString());
        }
    }

    public static int ExplorerSizeY
    {
        get { return _registryKey.ReadInteger("ExplorerSizeY"); }
        set
        {
            if (ExplorerSizeY != value && AllowWrite)
                _registryKey.SetValue("ExplorerSizeY", value.ToString());
        }
    }

    public static bool ShowLargeIcon
    {
        get { return _registryKey.ReadBoolean("ShowLargeIcon"); }
        set
        {
            if (ShowLargeIcon != value && AllowWrite)
                _registryKey.SetValue("ShowLargeIcon", value.ToString());
        }
    }

    public static bool ShowTitle
    {
        get { return _registryKey.ReadBoolean("ShowTitle"); }
        set
        {
            if (ShowTitle != value && AllowWrite)
                _registryKey.SetValue("ShowTitle", value.ToString());
        }
    }
}
