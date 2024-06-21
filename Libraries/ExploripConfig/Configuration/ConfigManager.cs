using System;
using System.IO;
using System.Windows;

using ExploripConfig.Helpers;

using WpfScreenHelper;

using static ExploripConfig.Helpers.ExtensionsCommandLineArguments;

namespace ExploripConfig.Configuration;

public static class ConfigManager
{
    public static string PATH_TO_INI { get; private set; }
    private static ManageIniFile _ini;
    private const string Explorip = "Explorip";
    private const string ExploripCopy = "ExploripCopy";
    private const string ExploripDesktop = "ExploripDesktop";
    private const string ExploripTaskbar = "ExploripTaskbar";

    public static void Init()
    {
        PATH_TO_INI = Path.Combine(Environment.SpecialFolder.LocalApplicationData.FullPath(), "CoolBytes", "Explorip", "ExploripConfig.ini");
        _ini = ManageIniFile.OpenIniFile(PATH_TO_INI);

        // If empty config ini file, set default settings
        if (string.IsNullOrWhiteSpace(_ini.ReadString(Explorip, "Theme")))
            _ini.WriteString(Explorip, "Theme", "System");
        if (string.IsNullOrWhiteSpace(_ini.ReadString(ExploripTaskbar, "ShowClock")))
            _ini.WriteString(ExploripTaskbar, "ShowClock", "True");
        if (string.IsNullOrWhiteSpace(_ini.ReadString(ExploripTaskbar, "ShowQuickLaunch")))
            _ini.WriteString(ExploripTaskbar, "ShowQuickLaunch", "True");
        if (string.IsNullOrWhiteSpace(_ini.ReadString(ExploripTaskbar, "QuickLaunchPath")))
            _ini.WriteString(ExploripTaskbar, "QuickLaunchPath", "%USERPROFILE%\\QuickLaunch");
        if (string.IsNullOrWhiteSpace(_ini.ReadString(ExploripTaskbar, "CollapseNotifyIcons")))
            _ini.WriteString(ExploripTaskbar, "CollapseNotifyIcons", "True");
        if (string.IsNullOrWhiteSpace(_ini.ReadString(Explorip, "AllowFontSmoothing")))
            _ini.WriteString(Explorip, "AllowFontSmoothing", "True");
        if (string.IsNullOrWhiteSpace(_ini.ReadString(Explorip, "Language")))
            _ini.WriteString(Explorip, "Language", "System");
        if (string.IsNullOrWhiteSpace(_ini.ReadString(ExploripTaskbar, "Edge")))
            _ini.WriteString(ExploripTaskbar, "Edge", "3");
        if (string.IsNullOrWhiteSpace(_ini.ReadString(Explorip, "HookCopy")))
            _ini.WriteString(Explorip, "HookCopy", "True");
        if (string.IsNullOrWhiteSpace(_ini.ReadString(Explorip, "UseOwnCopier")))
            _ini.WriteString(Explorip, "UseOwnCopier", "True");
        if (string.IsNullOrWhiteSpace(_ini.ReadString(Explorip, "StartTwoExplorer")))
            _ini.WriteString(Explorip, "StartTwoExplorer", "True");
        if (string.IsNullOrWhiteSpace(_ini.ReadString(ExploripCopy, "ShowNotificationCopyOperation")))
            _ini.WriteString(ExploripCopy, "ShowNotificationCopyOperation", "True");
        if (string.IsNullOrWhiteSpace(_ini.ReadString(ExploripDesktop, "HideDesktopBackground")))
            _ini.WriteString(ExploripDesktop, "HideDesktopBackground", "False");
        if (string.IsNullOrWhiteSpace(_ini.ReadString(Explorip, "WindowState")))
            ExplorerWindowState = WindowState.Normal;
        if (string.IsNullOrWhiteSpace(_ini.ReadString(Explorip, "PosX")))
            _ini.WriteString(Explorip, "PosX", (Screen.PrimaryScreen.WpfWorkingArea.X + 50).ToString());
        if (string.IsNullOrWhiteSpace(_ini.ReadString(Explorip, "PosY")))
            _ini.WriteString(Explorip, "PosY", (Screen.PrimaryScreen.WpfWorkingArea.Y + 50).ToString());
        if (string.IsNullOrWhiteSpace(_ini.ReadString(Explorip, "SizeX")))
            _ini.WriteString(Explorip, "SizeX", (Screen.PrimaryScreen.WpfWorkingArea.Width - 100).ToString());
        if (string.IsNullOrWhiteSpace(_ini.ReadString(Explorip, "SizeY")))
            _ini.WriteString(Explorip, "SizeY", (Screen.PrimaryScreen.WpfWorkingArea.Height - 100).ToString());
    }

    public static string Theme
    {
        get { return _ini.ReadString(Explorip, "Theme"); }
        set { _ini.WriteString(Explorip, "Theme", value); }
    }

    public static bool ShowClock
    {
        get { return _ini.ReadBoolean(ExploripTaskbar, "ShowClock"); }
        set { _ini.WriteString(ExploripTaskbar, "ShowClock", value.ToString()); }
    }

    public static bool ShowQuickLaunch
    {
        get { return _ini.ReadBoolean(ExploripTaskbar, "ShowQuickLaunch"); }
        set { _ini.WriteString(ExploripTaskbar, "ShowQuickLaunch", value.ToString()); }
    }

    public static string QuickLaunchPath
    {
        get { return _ini.ReadString(ExploripTaskbar, "QuickLaunchPath"); }
        set { _ini.WriteString(ExploripTaskbar, "QuickLaunchPath", value); }
    }

    public static bool CollapseNotifyIcons
    {
        get { return _ini.ReadBoolean(ExploripTaskbar, "CollapseNotifyIcons"); }
        set { _ini.WriteString(ExploripTaskbar, "CollapseNotifyIcons", value.ToString()); }
    }

    public static bool AllowFontSmoothing
    {
        get { return _ini.ReadBoolean(Explorip, "AllowFontSmoothing"); }
        set { _ini.WriteString(Explorip, "AllowFontSmoothing", value.ToString()); }
    }

    public static string Language
    {
        get { return _ini.ReadString(Explorip, "Language"); }
        set { _ini.WriteString(Explorip, "Language", value); }
    }

    public static int Edge
    {
        get { return _ini.ReadInteger(ExploripTaskbar, "Edge"); }
        set { _ini.WriteString(ExploripTaskbar, "Edge", value.ToString()); }
    }

    public static bool ShowNotificationCopyOperation
    {
        get { return _ini.ReadBoolean(ExploripCopy, "ShowNotificationCopyOperation"); }
        set { _ini.WriteString(ExploripCopy, "ShowNotificationCopyOperation", value.ToString()); }
    }

    public static bool HookCopy
    {
        get { return _ini.ReadBoolean(Explorip, "HookCopy") && !ArgumentExists("withouthook"); }
        set { _ini.WriteString(Explorip, "HookCopy", value.ToString()); }
    }

    public static bool UseOwnCopier
    {
        get { return _ini.ReadBoolean(Explorip, "UseOwnCopier") || ArgumentExists("useowncopier"); }
        set { _ini.WriteString(Explorip, "UseOwnCopier", value.ToString()); }
    }

    public static bool StartTwoExplorer
    {
        get { return _ini.ReadBoolean(Explorip, "StartTwoExplorer"); }
        set { _ini.WriteString(Explorip, "StartTwoExplorer", value.ToString()); }
    }

    public static bool HideDesktopBackground
    {
        get { return _ini.ReadBoolean(ExploripDesktop, "HideDesktopBackground"); }
        set { _ini.WriteString(ExploripDesktop, "HideDesktopBackground", value.ToString()); }
    }

    public static WindowState ExplorerWindowState
    {
        get { return (WindowState)_ini.ReadInteger(Explorip, "WindowState"); }
        set { _ini.WriteString(Explorip, "WindowState", ((int)value).ToString()); }
    }

    public static int ExplorerPosX
    {
        get { return _ini.ReadInteger(Explorip, "PosX"); }
        set { _ini.WriteString(Explorip, "PosX", value.ToString()); }
    }

    public static int ExplorerPosY
    {
        get { return _ini.ReadInteger(Explorip, "PosY"); }
        set { _ini.WriteString(Explorip, "PosY", value.ToString()); }
    }

    public static int ExplorerSizeX
    {
        get { return _ini.ReadInteger(Explorip, "SizeX"); }
        set { _ini.WriteString(Explorip, "SizeX", value.ToString()); }
    }

    public static int ExplorerSizeY
    {
        get { return _ini.ReadInteger(Explorip, "SizeY"); }
        set { _ini.WriteString(Explorip, "SizeY", value.ToString()); }
    }

    public static ManageIniFile ManageIniFile
    {
        get { return _ini; }
    }
}
