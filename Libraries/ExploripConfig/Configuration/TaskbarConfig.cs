using System;
using System.Windows;
using System.Windows.Media;

using ExploripConfig.Helpers;

using ExploripSharedCopy.Helpers;

using ManagedShell.AppBar;

using Microsoft.Win32;

namespace ExploripConfig.Configuration;

public class TaskbarConfig
{
    private RegistryKey _registryTaskbar;

    public int NumScreen { get; private set; }
    public bool AllowWrite { get; private set; }

    public void Init(int numScreen, RegistryKey rootRK, bool allowWrite)
    {
        NumScreen = numScreen;
        AllowWrite = allowWrite;

        _registryTaskbar = rootRK.CreateSubKey($"DISPLAY{numScreen}", true);

        if (allowWrite && _registryTaskbar != null)
        {
            if (string.IsNullOrWhiteSpace(_registryTaskbar.GetValue("ShowClock", "").ToString()))
                _registryTaskbar.SetValue("ShowClock", "True");
            if (string.IsNullOrWhiteSpace(_registryTaskbar.GetValue("CollapseNotifyIcons", "").ToString()))
                _registryTaskbar.SetValue("CollapseNotifyIcons", "True");
            if (string.IsNullOrWhiteSpace(_registryTaskbar.GetValue("AllowFontSmoothing", "").ToString()))
                _registryTaskbar.SetValue("AllowFontSmoothing", "True");
            string edge = _registryTaskbar.GetValue("Edge", "").ToString();
            if (string.IsNullOrWhiteSpace(edge) || !Enum.TryParse<AppBarEdge>(edge, out _))
                Edge = AppBarEdge.Bottom;
            if (string.IsNullOrWhiteSpace(_registryTaskbar.GetValue("TaskbarThumbHeight", "").ToString()))
                _registryTaskbar.SetValue("TaskbarThumbHeight", "150");
            if (string.IsNullOrWhiteSpace(_registryTaskbar.GetValue("TaskbarThumbWidth", "").ToString()))
                _registryTaskbar.SetValue("TaskbarThumbWidth", "250");
            if (string.IsNullOrWhiteSpace(_registryTaskbar.GetValue("TaskbarDisableThumb", "").ToString()))
                _registryTaskbar.SetValue("TaskbarDisableThumb", "False");
            if (string.IsNullOrWhiteSpace(_registryTaskbar.GetValue("ShowTaskbar", "").ToString()))
                _registryTaskbar.SetValue("ShowTaskbar", "True");
            if (string.IsNullOrWhiteSpace(_registryTaskbar.GetValue("ShowTaskMan", "").ToString()))
                _registryTaskbar.SetValue("ShowTaskMan", "True");
            if (string.IsNullOrWhiteSpace(_registryTaskbar.GetValue("ShowSearch", "").ToString()))
                _registryTaskbar.SetValue("ShowSearch", "True");
            if (string.IsNullOrWhiteSpace(_registryTaskbar.GetValue("ShowWidget", "").ToString()) && WindowsSettings.IsWindows11OrGreater())
                _registryTaskbar.SetValue("ShowWidget", "True");
            if (string.IsNullOrWhiteSpace(_registryTaskbar.GetValue("TaskButtonSize", "").ToString()))
                _registryTaskbar.SetValue("TaskButtonSize", "32");
            if (string.IsNullOrWhiteSpace(_registryTaskbar.GetValue("SearchWidth", "").ToString()))
                _registryTaskbar.SetValue("SearchWidth", "100");
            if (string.IsNullOrWhiteSpace(_registryTaskbar.GetValue("SpaceBetweenTaskButton", "").ToString()))
                _registryTaskbar.SetValue("SpaceBetweenTaskButton", "5");
            if (string.IsNullOrWhiteSpace(_registryTaskbar.GetValue("DesktopButtonWidth", "").ToString()))
                _registryTaskbar.SetValue("DesktopButtonWidth", "5");
            if (string.IsNullOrWhiteSpace(_registryTaskbar.GetValue("TasklistHorizontalAlignment", "").ToString()))
                _registryTaskbar.SetValue("TasklistHorizontalAlignment", HorizontalAlignment.Left.ToString("G"));
            if (string.IsNullOrWhiteSpace(_registryTaskbar.GetValue("TaskbarMinHeight", "").ToString()))
                _registryTaskbar.SetValue("TaskbarMinHeight", "52");
            if (string.IsNullOrWhiteSpace(_registryTaskbar.GetValue("TasklistVerticalAlignment", "").ToString()))
                _registryTaskbar.SetValue("TasklistVerticalAlignment", VerticalAlignment.Bottom.ToString("G"));
            if (string.IsNullOrWhiteSpace(_registryTaskbar.GetValue("ToolbarColumn", "").ToString()))
                _registryTaskbar.SetValue("ToolbarColumn", "0");
            if (string.IsNullOrWhiteSpace(_registryTaskbar.GetValue("ToolbarRow", "").ToString()))
                _registryTaskbar.SetValue("ToolbarRow", "0");
            if (string.IsNullOrWhiteSpace(_registryTaskbar.GetValue("TasklistColumn", "").ToString()))
                _registryTaskbar.SetValue("TasklistColumn", "0");
            if (string.IsNullOrWhiteSpace(_registryTaskbar.GetValue("TasklistRow", "").ToString()))
                _registryTaskbar.SetValue("TasklistRow", "1");
        }
    }

    public bool ShowClock
    {
        get { return _registryTaskbar.ReadBoolean("ShowClock", true); }
        set
        {
            if (ShowClock != value && AllowWrite)
                _registryTaskbar.SetValue("ShowClock", value.ToString());
        }
    }

    public bool CollapseNotifyIcons
    {
        get { return _registryTaskbar.ReadBoolean("CollapseNotifyIcons"); }
        set
        {
            if (CollapseNotifyIcons != value && AllowWrite)
                _registryTaskbar.SetValue("CollapseNotifyIcons", value.ToString());
        }
    }

    public bool AllowFontSmoothing
    {
        get { return _registryTaskbar.ReadBoolean("AllowFontSmoothing"); }
        set
        {
            if (AllowFontSmoothing != value && AllowWrite)
                _registryTaskbar.SetValue("AllowFontSmoothing", value.ToString());
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
        get { return _registryTaskbar.ReadDouble("TaskbarHeight"); }
        set
        {
            if (TaskbarHeight != value && AllowWrite)
                _registryTaskbar.SetValue("TaskbarHeight", value.ToString());
        }
    }

    public double TaskbarMinHeight
    {
        get { return _registryTaskbar.ReadDouble("TaskbarMinHeight"); }
        set
        {
            if (TaskbarMinHeight != value && AllowWrite)
                _registryTaskbar.SetValue("TaskbarMinHeight", value.ToString());
        }
    }

    public double TaskbarWidth
    {
        get { return _registryTaskbar.ReadDouble("TaskbarWidth"); }
        set
        {
            if (TaskbarWidth != value && AllowWrite)
                _registryTaskbar.SetValue("TaskbarWidth", value.ToString());
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
        get { return _registryTaskbar.ReadBoolean("ShowTaskbar"); }
        set
        {
            if (ShowTaskbar != value && AllowWrite)
                _registryTaskbar.SetValue("ShowTaskbar", value.ToString());
        }
    }

    public bool ShowTaskManButton
    {
        get { return _registryTaskbar.ReadBoolean("ShowTaskMan"); }
        set
        {
            if (ShowTaskManButton != value && AllowWrite)
                _registryTaskbar.SetValue("ShowTaskMan", value.ToString());
        }
    }

    public bool ShowSearchButton
    {
        get { return _registryTaskbar.ReadBoolean("ShowSearch"); }
        set
        {
            if (ShowSearchButton != value && AllowWrite)
                _registryTaskbar.SetValue("ShowSearch", value.ToString());
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
        get { return _registryTaskbar.ReadBoolean("ShowWidget"); }
        set
        {
            if (ShowWidgetButton != value && AllowWrite)
                _registryTaskbar.SetValue("ShowWidget", value.ToString());
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
        get { return _registryTaskbar.ReadInteger("TaskbarThumbHeight"); }
        set
        {
            if (TaskbarThumbHeight != value && AllowWrite)
                _registryTaskbar.SetValue("TaskbarThumbHeight", value.ToString());
        }
    }

    public int TaskbarThumbWidth
    {
        get { return _registryTaskbar.ReadInteger("TaskbarThumbWidth"); }
        set
        {
            if (TaskbarThumbWidth != value && AllowWrite)
                _registryTaskbar.SetValue("TaskbarThumbWidth", value.ToString());
        }
    }

    public bool TaskbarDisableThumb
    {
        get { return _registryTaskbar.ReadBoolean("DisableThumb"); }
        set
        {
            if (TaskbarDisableThumb != value && AllowWrite)
                _registryTaskbar.SetValue("DisableThumb", value.ToString());
        }
    }

    public double TaskButtonSize
    {
        get { return _registryTaskbar.ReadDouble("TaskButtonSize"); }
        set
        {
            if (TaskButtonSize != value && AllowWrite)
                _registryTaskbar.SetValue("TaskButtonSize", value.ToString());
        }
    }

    public double SearchWidth
    {
        get { return _registryTaskbar.ReadDouble("SearchWidth"); }
        set
        {
            if (SearchWidth != value && AllowWrite)
                _registryTaskbar.SetValue("SearchWidth", value.ToString());
        }
    }

    public double SearchHeight
    {
        get { return _registryTaskbar.ReadDouble("SearchHeight"); }
        set
        {
            if (SearchHeight != value && AllowWrite)
                _registryTaskbar.SetValue("SearchHeight", value.ToString());
        }
    }

    public double SpaceBetweenTaskButton
    {
        get { return _registryTaskbar.ReadDouble("SpaceBetweenTaskButton"); }
        set
        {
            if (TaskButtonSize != value && AllowWrite)
                _registryTaskbar.SetValue("SpaceBetweenTaskButton", value.ToString());
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
        get { return _registryTaskbar.ReadDouble("DesktopButtonWidth"); }
        set
        {
            if (DesktopButtonWidth != value && AllowWrite)
                _registryTaskbar.SetValue("DesktopButtonWidth", value.ToString());
        }
    }

    public HorizontalAlignment TaskListHorizontalAlignment
    {
        get { return _registryTaskbar.ReadEnum<HorizontalAlignment>("TasklistHorizontalAlignment"); }
        set
        {
            if (TaskListHorizontalAlignment != value && AllowWrite)
                _registryTaskbar.SetValue("TasklistHorizontalAlignment", value.ToString("G"));
        }
    }

    public VerticalAlignment TaskListVerticalAlignment
    {
        get { return _registryTaskbar.ReadEnum<VerticalAlignment>("TasklistVerticalAlignment"); }
        set
        {
            if (TaskListVerticalAlignment != value && AllowWrite)
                _registryTaskbar.SetValue("TasklistVerticalAlignment", value.ToString("G"));
        }
    }

    public int ToolbarColumn
    {
        get { return _registryTaskbar.ReadInteger("ToolbarColumn"); }
        set
        {
            if (ToolbarColumn != value && AllowWrite)
                _registryTaskbar.SetValue("ToolbarColumn", value.ToString());
        }
    }

    public int ToolbarRow
    {
        get { return _registryTaskbar.ReadInteger("ToolbarRow"); }
        set
        {
            if (ToolbarRow != value && AllowWrite)
                _registryTaskbar.SetValue("ToolbarRow", value.ToString());
        }
    }

    public int TasklistColumn
    {
        get { return _registryTaskbar.ReadInteger("TasklistColumn"); }
        set
        {
            if (TasklistColumn != value && AllowWrite)
                _registryTaskbar.SetValue("TasklistColumn", value.ToString());
        }
    }

    public int TasklistRow
    {
        get { return _registryTaskbar.ReadInteger("TasklistRow"); }
        set
        {
            if (TasklistRow != value && AllowWrite)
                _registryTaskbar.SetValue("TasklistRow", value.ToString());
        }
    }

    #region Toolbars

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
