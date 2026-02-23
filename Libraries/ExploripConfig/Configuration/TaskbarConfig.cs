using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Media;

using ExploripConfig.Helpers;

using ExploripSharedCopy.Helpers;

using ManagedShell.AppBar;

using Microsoft.Win32;

namespace ExploripConfig.Configuration;

public class TaskbarConfig
{
    #region Constants
    private const string ConfigShowClock = "ShowClock";
    private const string ConfigCollapseNotifyIcons = "CollapseNotifyIcons";
    private const string ConfigAllowFontSmoothing = "AllowFontSmoothing";
    private const string ConfigTaskbarThumbHeight = "TaskbarThumbHeight";
    private const string ConfigTaskbarThumbWidth = "TaskbarThumbWidth";
    private const string ConfigShowTaskbar = "ShowTaskbar";
    private const string ConfigShowTaskMan = "ShowTaskMan";
    private const string ConfigShowSearch = "ShowSearch";
    private const string ConfigShowWidget = "ShowWidget";
    private const string ConfigTaskButtonSize = "TaskButtonSize";
    private const string ConfigSearchWidth = "SearchWidth";
    private const string ConfigSpaceBetweenTaskButton = "SpaceBetweenTaskButton";
    private const string ConfigDesktopButtonWidth = "DesktopButtonWidth";
    private const string ConfigTasklistHorizontalAlignment = "TasklistHorizontalAlignment";
    private const string ConfigTaskbarMinHeight = "TaskbarMinHeight";
    private const string ConfigTaskbarHeight = "TaskbarHeight";
    private const string ConfigTasklistVerticalAlignment = "TasklistVerticalAlignment";
    private const string ConfigToolbarColumn = "ToolbarColumn";
    private const string ConfigToolbarRow = "ToolbarRow";
    private const string ConfigTasklistColumn = "TasklistColumn";
    private const string ConfigTasklistRow = "TasklistRow";
    private const string ConfigMaxWidthTitleApplicationWindow = "MaxWidthTitleApplicationWindow";
    private const string ConfigFloatingButtonWidth = "FloatingButtonWidth";
    private const string ConfigFloatingButtonStretchMode = "FloatingButtonStretchMode";
    private const string ConfigStartFloating = "StartFloating";
    private const string ConfigFloatingButtonPosY = "FloatingButtonPosY";
    private const string ConfigFloatingButtonSide = "FloatingButtonSide";
    private const string ConfigTaskbarDisableThumb = "TaskbarDisableThumb";
    private const string ConfigPathPinnedTaskbar = "PathApplicationPinned";
    #endregion

    private RegistryKey _registryTaskbar;

    public int NumScreen { get; private set; }
    public bool AllowWrite { get; private set; }
    public string UniqueId { get; private set; }

    public void Init(int numScreen, RegistryKey rootRK, bool allowWrite, string uniqueId)
    {
        NumScreen = numScreen;
        AllowWrite = allowWrite;
        UniqueId = uniqueId;

        _registryTaskbar = rootRK.CreateSubKey($"DISPLAY{numScreen}", true);

        if (allowWrite && _registryTaskbar != null)
        {
            if (string.IsNullOrWhiteSpace(_registryTaskbar.GetValue("", "").ToString()))
                _registryTaskbar.SetValue("", uniqueId);
            if (string.IsNullOrWhiteSpace(_registryTaskbar.GetValue(ConfigShowClock, "").ToString()))
                _registryTaskbar.SetValue(ConfigShowClock, "True");
            if (string.IsNullOrWhiteSpace(_registryTaskbar.GetValue(ConfigCollapseNotifyIcons, "").ToString()))
                _registryTaskbar.SetValue(ConfigCollapseNotifyIcons, "True");
            if (string.IsNullOrWhiteSpace(_registryTaskbar.GetValue(ConfigAllowFontSmoothing, "").ToString()))
                _registryTaskbar.SetValue(ConfigAllowFontSmoothing, "True");
            string edge = _registryTaskbar.GetValue("Edge", "").ToString();
            if (string.IsNullOrWhiteSpace(edge) || !Enum.TryParse<AppBarEdge>(edge, out _))
                Edge = AppBarEdge.Bottom;
            if (string.IsNullOrWhiteSpace(_registryTaskbar.GetValue(ConfigTaskbarThumbHeight, "").ToString()))
                _registryTaskbar.SetValue(ConfigTaskbarThumbHeight, "150");
            if (string.IsNullOrWhiteSpace(_registryTaskbar.GetValue(ConfigTaskbarThumbWidth, "").ToString()))
                _registryTaskbar.SetValue(ConfigTaskbarThumbWidth, "250");
            if (string.IsNullOrWhiteSpace(_registryTaskbar.GetValue(ConfigTaskbarDisableThumb, "").ToString()))
                _registryTaskbar.SetValue(ConfigTaskbarDisableThumb, "False");
            if (string.IsNullOrWhiteSpace(_registryTaskbar.GetValue(ConfigShowTaskbar, "").ToString()))
                _registryTaskbar.SetValue(ConfigShowTaskbar, "True");
            if (string.IsNullOrWhiteSpace(_registryTaskbar.GetValue(ConfigShowTaskMan, "").ToString()))
                _registryTaskbar.SetValue(ConfigShowTaskMan, "True");
            if (string.IsNullOrWhiteSpace(_registryTaskbar.GetValue(ConfigShowSearch, "").ToString()))
                _registryTaskbar.SetValue(ConfigShowSearch, "True");
            if (string.IsNullOrWhiteSpace(_registryTaskbar.GetValue(ConfigShowWidget, "").ToString()) && WindowsSettings.IsWindows11OrGreater())
                _registryTaskbar.SetValue(ConfigShowWidget, "True");
            if (string.IsNullOrWhiteSpace(_registryTaskbar.GetValue(ConfigTaskButtonSize, "").ToString()))
                _registryTaskbar.SetValue(ConfigTaskButtonSize, "32");
            if (string.IsNullOrWhiteSpace(_registryTaskbar.GetValue(ConfigSearchWidth, "").ToString()))
                _registryTaskbar.SetValue(ConfigSearchWidth, "100");
            if (string.IsNullOrWhiteSpace(_registryTaskbar.GetValue(ConfigSpaceBetweenTaskButton, "").ToString()))
                _registryTaskbar.SetValue(ConfigSpaceBetweenTaskButton, "5");
            if (string.IsNullOrWhiteSpace(_registryTaskbar.GetValue(ConfigDesktopButtonWidth, "").ToString()))
                _registryTaskbar.SetValue(ConfigDesktopButtonWidth, "5");
            if (string.IsNullOrWhiteSpace(_registryTaskbar.GetValue(ConfigTasklistHorizontalAlignment, "").ToString()))
                _registryTaskbar.SetValue(ConfigTasklistHorizontalAlignment, HorizontalAlignment.Left.ToString("G"));
            if (string.IsNullOrWhiteSpace(_registryTaskbar.GetValue(ConfigTaskbarMinHeight, "").ToString()))
                _registryTaskbar.SetValue(ConfigTaskbarMinHeight, "52");
            if (string.IsNullOrWhiteSpace(_registryTaskbar.GetValue(ConfigTaskbarHeight, "").ToString()))
                _registryTaskbar.SetValue(ConfigTaskbarHeight, "52");
            if (string.IsNullOrWhiteSpace(_registryTaskbar.GetValue(ConfigTasklistVerticalAlignment, "").ToString()))
                _registryTaskbar.SetValue(ConfigTasklistVerticalAlignment, VerticalAlignment.Bottom.ToString("G"));
            if (string.IsNullOrWhiteSpace(_registryTaskbar.GetValue(ConfigToolbarColumn, "").ToString()))
                _registryTaskbar.SetValue(ConfigToolbarColumn, "0");
            if (string.IsNullOrWhiteSpace(_registryTaskbar.GetValue(ConfigToolbarRow, "").ToString()))
                _registryTaskbar.SetValue(ConfigToolbarRow, "0");
            if (string.IsNullOrWhiteSpace(_registryTaskbar.GetValue(ConfigTasklistColumn, "").ToString()))
                _registryTaskbar.SetValue(ConfigTasklistColumn, "0");
            if (string.IsNullOrWhiteSpace(_registryTaskbar.GetValue(ConfigTasklistRow, "").ToString()))
                _registryTaskbar.SetValue(ConfigTasklistRow, "1");
            if (string.IsNullOrWhiteSpace(_registryTaskbar.GetValue(ConfigMaxWidthTitleApplicationWindow, "").ToString()))
                _registryTaskbar.SetValue(ConfigMaxWidthTitleApplicationWindow, "100");
            if (string.IsNullOrWhiteSpace(_registryTaskbar.GetValue(ConfigFloatingButtonWidth, "").ToString()))
                _registryTaskbar.SetValue(ConfigFloatingButtonWidth, "16");
            if (string.IsNullOrWhiteSpace(_registryTaskbar.GetValue(ConfigFloatingButtonStretchMode, "").ToString()))
                _registryTaskbar.SetValue(ConfigFloatingButtonStretchMode, Stretch.None.ToString("G"));
            if (string.IsNullOrWhiteSpace(_registryTaskbar.GetValue(ConfigStartFloating, "").ToString()))
                _registryTaskbar.SetValue(ConfigStartFloating, "False");
            if (string.IsNullOrWhiteSpace(_registryTaskbar.GetValue(ConfigFloatingButtonSide, "").ToString()))
                _registryTaskbar.SetValue(ConfigFloatingButtonSide, HorizontalAlignment.Left.ToString("G"));
            if (string.IsNullOrWhiteSpace(_registryTaskbar.GetValue(ConfigPathPinnedTaskbar, "").ToString()))
                _registryTaskbar.SetValue(ConfigPathPinnedTaskbar, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Microsoft", "Internet Explorer", "Quick Launch", "User Pinned", "TaskBar"));
        }
    }

    public string GetUniqueId
    {
        get { return _registryTaskbar.GetValue("", "").ToString(); }
    }

    public string PathPinnedApp
    {
        get { return _registryTaskbar.GetValue(ConfigPathPinnedTaskbar).ToString(); }
        set
        {
            if (PathPinnedApp != value && AllowWrite)
                _registryTaskbar.SetValue(ConfigPathPinnedTaskbar, value);
        }

    }

    public HorizontalAlignment FloatingButtonSide
    {
        get { return _registryTaskbar.ReadEnum<HorizontalAlignment>(ConfigFloatingButtonSide); }
        set
        {
            if (FloatingButtonSide != value && AllowWrite)
                _registryTaskbar.SetValue(ConfigFloatingButtonSide, value.ToString("G"));
        }
    }

    public double FloatingButtonPosY
    {
        get { return _registryTaskbar.ReadDouble(ConfigFloatingButtonPosY, -1); }
        set
        {
            if (FloatingButtonPosY != value && AllowWrite)
                _registryTaskbar.SetValue(ConfigFloatingButtonPosY, value.ToString(CultureInfo.InvariantCulture));
        }
    }

    public double FloatingButtonWidth
    {
        get { return _registryTaskbar.ReadDouble(ConfigFloatingButtonWidth, 16); }
        set
        {
            if (FloatingButtonWidth != value && AllowWrite)
                _registryTaskbar.SetValue(ConfigFloatingButtonWidth, value.ToString(CultureInfo.InvariantCulture));
        }
    }

    public Stretch FloatingButtonStretchMode
    {
        get { return _registryTaskbar.ReadEnum<Stretch>(ConfigFloatingButtonStretchMode); }
        set
        {
            if (FloatingButtonStretchMode != value && AllowWrite)
                _registryTaskbar.SetValue(ConfigFloatingButtonStretchMode, value.ToString("G"));
        }
    }

    public bool StartFloating
    {
        get { return _registryTaskbar.ReadBoolean(ConfigStartFloating); }
        set
        {
            if (StartFloating != value && AllowWrite)
                _registryTaskbar.SetValue(ConfigStartFloating, value.ToString());
        }
    }

    public bool ShowClock
    {
        get { return _registryTaskbar.ReadBoolean(ConfigShowClock, true); }
        set
        {
            if (ShowClock != value && AllowWrite)
                _registryTaskbar.SetValue(ConfigShowClock, value.ToString());
        }
    }

    public bool CollapseNotifyIcons
    {
        get { return _registryTaskbar.ReadBoolean(ConfigCollapseNotifyIcons); }
        set
        {
            if (CollapseNotifyIcons != value && AllowWrite)
                _registryTaskbar.SetValue(ConfigCollapseNotifyIcons, value.ToString());
        }
    }

    public bool AllowFontSmoothing
    {
        get { return _registryTaskbar.ReadBoolean(ConfigAllowFontSmoothing); }
        set
        {
            if (AllowFontSmoothing != value && AllowWrite)
                _registryTaskbar.SetValue(ConfigAllowFontSmoothing, value.ToString());
        }
    }

    public AppBarEdge Edge
    {
        get { return _registryTaskbar.ReadEnum<AppBarEdge>("Edge"); }
        set
        {
            if (Edge != value && AllowWrite)
                _registryTaskbar.SetValue("Edge", ((int)value).ToString());
        }
    }

    public double TaskbarHeight
    {
        get { return _registryTaskbar.ReadDouble(ConfigTaskbarHeight); }
        set
        {
            if (TaskbarHeight != value && AllowWrite)
                _registryTaskbar.SetValue(ConfigTaskbarHeight, value.ToString(CultureInfo.InvariantCulture));
        }
    }

    public double TaskbarMinHeight
    {
        get { return _registryTaskbar.ReadDouble(ConfigTaskbarMinHeight); }
        set
        {
            if (TaskbarMinHeight != value && AllowWrite)
                _registryTaskbar.SetValue(ConfigTaskbarMinHeight, value.ToString(CultureInfo.InvariantCulture));
        }
    }

    public double TaskbarWidth
    {
        get { return _registryTaskbar.ReadDouble("TaskbarWidth"); }
        set
        {
            if (TaskbarWidth != value && AllowWrite)
                _registryTaskbar.SetValue("TaskbarWidth", value.ToString(CultureInfo.InvariantCulture));
        }
    }

    public SolidColorBrush TaskbarBackground
    {
        get
        {
            string bgColor = _registryTaskbar.GetValue("BackgroundColor")?.ToString();
            if (!string.IsNullOrWhiteSpace(bgColor))
            {
                return new SolidColorBrush(_registryTaskbar.ReadColor("BackgroundColor", ExploripSharedCopy.Constants.Colors.BackgroundColor));
            }
            return null;
        }
        set
        {
            if (AllowWrite)
                _registryTaskbar.SetValue("BackgroundColor", $"{value.Color.A},{value.Color.R},{value.Color.G},{value.Color.B}");
        }
    }

    public bool TaskbarAllowsTransparency
    {
        get { return _registryTaskbar.ReadBoolean("AllowsTransparency"); }
        set
        {
            if (TaskbarAllowsTransparency != value && AllowWrite)
                _registryTaskbar.SetValue("AllowsTransparency", value.ToString());
        }
    }

    public bool ShowTaskbar
    {
        get { return _registryTaskbar.ReadBoolean(ConfigShowTaskbar); }
        set
        {
            if (ShowTaskbar != value && AllowWrite)
                _registryTaskbar.SetValue(ConfigShowTaskbar, value.ToString());
        }
    }

    public bool ShowTaskManButton
    {
        get { return _registryTaskbar.ReadBoolean(ConfigShowTaskMan); }
        set
        {
            if (ShowTaskManButton != value && AllowWrite)
                _registryTaskbar.SetValue(ConfigShowTaskMan, value.ToString());
        }
    }

    public bool ShowSearchButton
    {
        get { return _registryTaskbar.ReadBoolean(ConfigShowSearch); }
        set
        {
            if (ShowSearchButton != value && AllowWrite)
                _registryTaskbar.SetValue(ConfigShowSearch, value.ToString());
        }
    }

    public bool ShowSearchZone
    {
        get { return _registryTaskbar.ReadBoolean("ShowSearchBar"); }
        set
        {
            if (ShowSearchZone != value && AllowWrite)
                _registryTaskbar.SetValue("ShowSearchBar", value.ToString());
        }
    }

    public bool ShowWidgetButton
    {
        get { return _registryTaskbar.ReadBoolean(ConfigShowWidget); }
        set
        {
            if (ShowWidgetButton != value && AllowWrite)
                _registryTaskbar.SetValue(ConfigShowWidget, value.ToString());
        }
    }

    public bool ShowCopilotButton
    {
        get { return _registryTaskbar.ReadBoolean("ShowCopilot"); }
        set
        {
            if (ShowCopilotButton != value && AllowWrite)
                _registryTaskbar.SetValue("ShowCopilot", value.ToString());
        }
    }

    public bool ShowKeyboardLayout
    {
        get { return _registryTaskbar.ReadBoolean("ShowKeyboardLayout"); }
        set
        {
            if (ShowKeyboardLayout != value && AllowWrite)
                _registryTaskbar.SetValue("ShowKeyboardLayout", value.ToString());
        }
    }

    public bool ShowTabTip
    {
        get { return _registryTaskbar.ReadBoolean("ShowTabTip"); }
        set
        {
            if (ShowTabTip != value && AllowWrite)
                _registryTaskbar.SetValue("ShowTabTip", value.ToString());
        }
    }

    public int TaskbarThumbHeight
    {
        get { return _registryTaskbar.ReadInteger(ConfigTaskbarThumbHeight); }
        set
        {
            if (TaskbarThumbHeight != value && AllowWrite)
                _registryTaskbar.SetValue(ConfigTaskbarThumbHeight, value.ToString());
        }
    }

    public int TaskbarThumbWidth
    {
        get { return _registryTaskbar.ReadInteger(ConfigTaskbarThumbWidth); }
        set
        {
            if (TaskbarThumbWidth != value && AllowWrite)
                _registryTaskbar.SetValue(ConfigTaskbarThumbWidth, value.ToString());
        }
    }

    public bool TaskbarDisableThumb
    {
        get { return _registryTaskbar.ReadBoolean(ConfigTaskbarDisableThumb); }
        set
        {
            if (TaskbarDisableThumb != value && AllowWrite)
                _registryTaskbar.SetValue(ConfigTaskbarDisableThumb, value.ToString());
        }
    }

    public double TaskButtonSize
    {
        get { return _registryTaskbar.ReadDouble(ConfigTaskButtonSize); }
        set
        {
            if (TaskButtonSize != value && AllowWrite)
                _registryTaskbar.SetValue(ConfigTaskButtonSize, value.ToString(CultureInfo.InvariantCulture));
        }
    }

    public double SearchWidth
    {
        get { return _registryTaskbar.ReadDouble(ConfigSearchWidth); }
        set
        {
            if (SearchWidth != value && AllowWrite)
                _registryTaskbar.SetValue(ConfigSearchWidth, value.ToString(CultureInfo.InvariantCulture));
        }
    }

    public double SearchHeight
    {
        get { return _registryTaskbar.ReadDouble("SearchHeight"); }
        set
        {
            if (SearchHeight != value && AllowWrite)
                _registryTaskbar.SetValue("SearchHeight", value.ToString(CultureInfo.InvariantCulture));
        }
    }

    public double SpaceBetweenTaskButton
    {
        get { return _registryTaskbar.ReadDouble(ConfigSpaceBetweenTaskButton); }
        set
        {
            if (TaskButtonSize != value && AllowWrite)
                _registryTaskbar.SetValue(ConfigSpaceBetweenTaskButton, value.ToString(CultureInfo.InvariantCulture));
        }
    }

    public bool ShowDesktopPreview
    {
        get { return _registryTaskbar.ReadBoolean("ShowDesktopPreview"); }
        set
        {
            if (ShowDesktopPreview != value && AllowWrite)
                _registryTaskbar.SetValue("ShowDesktopPreview", value.ToString());
        }
    }

    public double DesktopButtonWidth
    {
        get { return _registryTaskbar.ReadDouble(ConfigDesktopButtonWidth); }
        set
        {
            if (DesktopButtonWidth != value && AllowWrite)
                _registryTaskbar.SetValue(ConfigDesktopButtonWidth, value.ToString(CultureInfo.InvariantCulture));
        }
    }

    public HorizontalAlignment TaskListHorizontalAlignment
    {
        get { return _registryTaskbar.ReadEnum<HorizontalAlignment>(ConfigTasklistHorizontalAlignment); }
        set
        {
            if (TaskListHorizontalAlignment != value && AllowWrite)
                _registryTaskbar.SetValue(ConfigTasklistHorizontalAlignment, value.ToString("G"));
        }
    }

    public VerticalAlignment TaskListVerticalAlignment
    {
        get { return _registryTaskbar.ReadEnum<VerticalAlignment>(ConfigTasklistVerticalAlignment); }
        set
        {
            if (TaskListVerticalAlignment != value && AllowWrite)
                _registryTaskbar.SetValue(ConfigTasklistVerticalAlignment, value.ToString("G"));
        }
    }

    public int ToolbarColumn
    {
        get { return _registryTaskbar.ReadInteger(ConfigToolbarColumn); }
        set
        {
            if (ToolbarColumn != value && AllowWrite)
                _registryTaskbar.SetValue(ConfigToolbarColumn, value.ToString());
        }
    }

    public int ToolbarRow
    {
        get { return _registryTaskbar.ReadInteger(ConfigToolbarRow); }
        set
        {
            if (ToolbarRow != value && AllowWrite)
                _registryTaskbar.SetValue(ConfigToolbarRow, value.ToString());
        }
    }

    public int TasklistColumn
    {
        get { return _registryTaskbar.ReadInteger(ConfigTasklistColumn); }
        set
        {
            if (TasklistColumn != value && AllowWrite)
                _registryTaskbar.SetValue(ConfigTasklistColumn, value.ToString());
        }
    }

    public int TasklistRow
    {
        get { return _registryTaskbar.ReadInteger(ConfigTasklistRow); }
        set
        {
            if (TasklistRow != value && AllowWrite)
                _registryTaskbar.SetValue(ConfigTasklistRow, value.ToString());
        }
    }

    public bool ShowTitleApplicationWindow
    {
        get { return _registryTaskbar.ReadBoolean("ShowTitleApplicationWindow"); }
        set
        {
            if (ShowTitleApplicationWindow != value && AllowWrite)
                _registryTaskbar.SetValue("ShowTitleApplicationWindow", value.ToString());
        }
    }

    public double MaxWidthTitleApplicationWindow
    {
        get { return _registryTaskbar.ReadDouble(ConfigMaxWidthTitleApplicationWindow); }
        set
        {
            if (MaxWidthTitleApplicationWindow != value && AllowWrite)
                _registryTaskbar.SetValue(ConfigMaxWidthTitleApplicationWindow, value.ToString(CultureInfo.InvariantCulture));
        }
    }

    #region Toolbars

    public int ToolbarMaxWidth(string path)
    {
        int i = ConfigManager.ToolbarNumber(path);
        if (i >= 0)
        {
            return _registryTaskbar.ReadInteger($"{ConfigManager.ToolBarNameInRegistry}({i}).MaxWidth", 0);
        }
        return 0;
    }

    public int ToolbarZIndex(string path)
    {
        int i = ConfigManager.ToolbarNumber(path);
        if (i >= 0)
        {
            return _registryTaskbar.ReadInteger($"{ConfigManager.ToolBarNameInRegistry}({i}).ZIndex", 0);
        }
        return 0;
    }

    public void ToolbarZIndex(string path, int zindex)
    {
        int i = ConfigManager.ToolbarNumber(path);
        if (i >= 0)
        {
            _registryTaskbar.SetValue($"{ConfigManager.ToolBarNameInRegistry}({i}).ZIndex", zindex.ToString());
        }
    }

    public (int, int) ToolbarGrid(string path)
    {
        int i = ConfigManager.ToolbarNumber(path);
        if (i >= 0)
        {
            int column = _registryTaskbar.ReadInteger($"{ConfigManager.ToolBarNameInRegistry}({i}).Column", -1);
            int row = _registryTaskbar.ReadInteger($"{ConfigManager.ToolBarNameInRegistry}({i}).Row", -1);
            return (column, row);
        }
        return (-1, -1);
    }

    public void ToolbarGrid(string path, (int, int) gridPos)
    {
        int i = ConfigManager.ToolbarNumber(path);
        if (i >= 0)
        {
            _registryTaskbar.SetValue($"{ConfigManager.ToolBarNameInRegistry}({i}).Column", gridPos.Item1.ToString());
            _registryTaskbar.SetValue($"{ConfigManager.ToolBarNameInRegistry}({i}).Row", gridPos.Item2.ToString());
        }
    }

    public void ToolbarPosition(string path, Point position)
    {
        int i = ConfigManager.ToolbarNumber(path);
        if (i >= 0)
        {
            _registryTaskbar.SetValue($"{ConfigManager.ToolBarNameInRegistry}({i}).X", position.X.ToString());
            _registryTaskbar.SetValue($"{ConfigManager.ToolBarNameInRegistry}({i}).Y", position.Y.ToString());
        }
    }

    public Point ToolbarPosition(string path)
    {
        Point ret = new();
        int i = ConfigManager.ToolbarNumber(path);
        if (i >= 0)
        {
            ret.X = _registryTaskbar.ReadDouble($"{ConfigManager.ToolBarNameInRegistry}({i}).X");
            ret.Y = _registryTaskbar.ReadDouble($"{ConfigManager.ToolBarNameInRegistry}({i}).Y");
        }
        return ret;
    }

    public void ToolbarSmallSizeIcon(string path, bool smallSize)
    {
        int i = ConfigManager.ToolbarNumber(path);
        if (i >= 0)
        {
            _registryTaskbar.SetValue($"{ConfigManager.ToolBarNameInRegistry}({i}).SmallSizeIcon", smallSize.ToString());
        }
    }

    public bool ToolbarSmallSizeIcon(string path)
    {
        bool ret = true;
        int i = ConfigManager.ToolbarNumber(path);
        if (i >= 0)
        {
            ret = _registryTaskbar.ReadBoolean($"{ConfigManager.ToolBarNameInRegistry}({i}).SmallSizeIcon");
        }
        return ret;
    }

    public void ToolbarShowTitle(string path, bool showTitle)
    {
        int i = ConfigManager.ToolbarNumber(path);
        if (i >= 0)
        {
            _registryTaskbar.SetValue($"{ConfigManager.ToolBarNameInRegistry}({i}).ShowTitle", showTitle.ToString());
        }
    }

    public bool ToolbarShowTitle(string path)
    {
        bool ret = true;
        int i = ConfigManager.ToolbarNumber(path);
        if (i >= 0)
        {
            ret = _registryTaskbar.ReadBoolean($"{ConfigManager.ToolBarNameInRegistry}({i}).ShowTitle");
        }
        return ret;
    }

    public void ToolbarVisible(string path, bool visible)
    {
        int i = ConfigManager.ToolbarNumber(path);
        if (i >= 0)
        {
            _registryTaskbar.SetValue($"{ConfigManager.ToolBarNameInRegistry}({i}).Visible", visible.ToString());
        }
    }

    public bool ToolbarVisible(string path)
    {
        bool ret = true;
        int i = ConfigManager.ToolbarNumber(path);
        if (i >= 0)
        {
            ret = _registryTaskbar.ReadBoolean($"{ConfigManager.ToolBarNameInRegistry}({i}).Visible");
        }
        return ret;
    }

    #endregion
}
