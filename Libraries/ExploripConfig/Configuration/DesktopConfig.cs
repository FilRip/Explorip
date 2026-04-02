using System.Diagnostics;
using System.Globalization;
using System.Windows.Media;

using ExploripConfig.Helpers;

using Microsoft.Win32;

namespace ExploripConfig.Configuration;

public class DesktopConfig
{
    #region Constants
    private const string ConfigItemSizeX = "ItemSizeX";
    private const string ConfigItemSizeY = "ItemSizeY";
    private const string ConfigHideBackground = "HideBackground";
    private const string ConfigBackgroundColor = "BackgroundColor";
    private const string ConfigPath = "Path";
    private const string ConfigShowCommonDesktop = "ShowCommonDesktop";
    private const string ConfigShowCommonIcon = "ShowCommonIcons";
    private const string ConfigShowAllIcons = "ShowAllIcons";
    private const string ConfigBorderRadius = "BorderCornerRadius";
    private const string ConfigBorderColor = "BorderColor";
    private const string ConfigBorderSize = "BorderSize";
    private const string ConfigSelectedItemBackgroundColor = "SelectedItemBackgroundColor";
    private const string ConfigMouseOverBackgroundColor = "MouseOverItemBackgroundColor";
    #endregion

    private RegistryKey _registryDesktop;

    public int NumScreen { get; private set; }
    public bool AllowWrite { get; private set; }
    public string UniqueId { get; private set; }

    public void Init(int numScreen, RegistryKey rootDesktop, bool allowWrite, string uniqueId)
    {
        NumScreen = numScreen;
        AllowWrite = allowWrite;
        UniqueId = uniqueId;

        _registryDesktop = rootDesktop.CreateSubKey($"DISPLAY{numScreen}", true);
        if (allowWrite && _registryDesktop != null)
        {
            _registryDesktop.CreateSubKey("IconsPosition");
            if (string.IsNullOrWhiteSpace(_registryDesktop.GetValue("", "").ToString()))
                _registryDesktop.SetValue("", uniqueId);
            if (string.IsNullOrWhiteSpace(_registryDesktop.GetValue(ConfigItemSizeX, "").ToString()))
                _registryDesktop.SetValue(ConfigItemSizeX, "96");
            if (string.IsNullOrWhiteSpace(_registryDesktop.GetValue(ConfigItemSizeY, "").ToString()))
                _registryDesktop.SetValue(ConfigItemSizeY, "96");
            if (string.IsNullOrWhiteSpace(_registryDesktop.GetValue(ConfigHideBackground, "").ToString()))
                _registryDesktop.SetValue(ConfigHideBackground, "True");
            if (string.IsNullOrWhiteSpace(_registryDesktop.GetValue(ConfigBackgroundColor, "").ToString()))
                _registryDesktop.SetValue(ConfigBackgroundColor, "255,0,0,0");
            if (string.IsNullOrWhiteSpace(_registryDesktop.GetValue(ConfigMouseOverBackgroundColor, "").ToString()))
                _registryDesktop.SetValue(ConfigMouseOverBackgroundColor, "128,63,63,63");
            if (string.IsNullOrWhiteSpace(_registryDesktop.GetValue(ConfigSelectedItemBackgroundColor, "").ToString()))
                _registryDesktop.SetValue(ConfigSelectedItemBackgroundColor, "255,63,63,63");
        }
    }

    public string GetUniqueId
    {
        get { return _registryDesktop.GetValue("", "").ToString(); }
    }

    public int ItemSizeX
    {
        get { return _registryDesktop.ReadInteger(ConfigItemSizeX); }
        set
        {
            if (ItemSizeX != value && AllowWrite)
                _registryDesktop.SetValue(ConfigItemSizeX, value.ToString());
        }
    }

    public int ItemSizeY
    {
        get { return _registryDesktop.ReadInteger(ConfigItemSizeY); }
        set
        {
            if (ItemSizeY != value && AllowWrite)
                _registryDesktop.SetValue(ConfigItemSizeY, value.ToString());
        }
    }

    public bool HideBackground
    {
        get { return _registryDesktop.ReadBoolean(ConfigHideBackground); }
        set
        {
            if (HideBackground != value && AllowWrite)
                _registryDesktop.SetValue(ConfigHideBackground, value.ToString());
        }
    }

    public SolidColorBrush DesktopBackground
    {
        get
        {
            string bgColor = _registryDesktop.GetValue(ConfigBackgroundColor)?.ToString();
            if (!string.IsNullOrWhiteSpace(bgColor))
            {
                return new SolidColorBrush(_registryDesktop.ReadColor(ConfigBackgroundColor, ExploripSharedCopy.Constants.Colors.BackgroundColor));
            }
            return null;
        }
        set
        {
            if (AllowWrite)
                _registryDesktop.SetValue(ConfigBackgroundColor, $"{value.Color.A},{value.Color.R},{value.Color.G},{value.Color.B}");
        }
    }

    public (int, int) GetItemPosition(string itemName)
    {
        if (!string.IsNullOrWhiteSpace(itemName))
        {
            string pos = _registryDesktop.OpenSubKey("IconsPosition", true).GetValue(itemName, "")?.ToString();
            if (!string.IsNullOrWhiteSpace(pos))
            {
                if (pos.IndexOf(',') < 0)
                    Debugger.Break();
                string[] splitter = pos.Split(',');
                return (int.Parse(splitter[0]), int.Parse(splitter[1]));
            }
        }
        return (-1, -1);
    }

    public void SetItemPosition(string itemName, (int, int) position)
    {
        _registryDesktop.OpenSubKey("IconsPosition", true).SetValue(itemName, $"{position.Item1},{position.Item2}");
    }

    public string Path
    {
        get { return _registryDesktop.GetValue(ConfigPath, "").ToString(); }
        set
        {
            if (Path != value && AllowWrite)
                _registryDesktop.SetValue(ConfigPath, value);
        }
    }

    public bool ShowCommonDesktop
    {
        get { return _registryDesktop.ReadBoolean(ConfigShowCommonDesktop, true); }
        set
        {
            if (ShowCommonDesktop != value && AllowWrite)
                _registryDesktop.SetValue(ConfigShowCommonDesktop, value.ToString());
        }
    }

    public bool ShowCommonIcons
    {
        get { return _registryDesktop.ReadBoolean(ConfigShowCommonIcon, true); }
        set
        {
            if (ShowCommonIcons != value && AllowWrite)
                _registryDesktop.SetValue(ConfigShowCommonIcon, value.ToString());
        }
    }

    public bool ShowAllIcons
    {
        get { return _registryDesktop.ReadBoolean(ConfigShowAllIcons, false); }
        set
        {
            if (ShowAllIcons != value && AllowWrite)
                _registryDesktop.SetValue(ConfigShowAllIcons, value.ToString());
        }
    }

    internal bool ShowAllIconsPresent
    {
        get { return !string.IsNullOrWhiteSpace(_registryDesktop.GetValue(ConfigShowAllIcons, "")?.ToString()); }
    }

    public double BorderRadius
    {
        get { return _registryDesktop.ReadDouble(ConfigBorderRadius); }
        set
        {
            if (BorderRadius != value && AllowWrite)
                _registryDesktop.SetValue(ConfigBorderRadius, value.ToString(CultureInfo.InvariantCulture));
        }
    }

    public double BorderSize
    {
        get { return _registryDesktop.ReadDouble(ConfigBorderSize); }
        set
        {
            if (BorderRadius != value && AllowWrite)
                _registryDesktop.SetValue(ConfigBorderRadius, value.ToString(CultureInfo.InvariantCulture));
        }
    }

    public Color BorderColor
    {
        get { return _registryDesktop.ReadColor(ConfigBorderColor, Color.FromArgb(0, 0, 0, 0)); }
        set
        {
            if (AllowWrite)
                _registryDesktop.SetValue(ConfigBorderColor, $"{value.A},{value.R},{value.G},{value.B}");
        }
    }

    public Color SelectedItemBackgroundColor
    {
        get { return _registryDesktop.ReadColor(ConfigSelectedItemBackgroundColor, Color.FromArgb(0, 0, 0, 0)); }
        set
        {
            if (AllowWrite)
                _registryDesktop.SetValue(ConfigSelectedItemBackgroundColor, $"{value.A},{value.R},{value.G},{value.B}");
        }
    }

    public Color MouseOverBackgroundColor
    {
        get { return _registryDesktop.ReadColor(ConfigMouseOverBackgroundColor, Color.FromArgb(0, 0, 0, 0)); }
        set
        {
            if (AllowWrite)
                _registryDesktop.SetValue(ConfigMouseOverBackgroundColor, $"{value.A},{value.R},{value.G},{value.B}");
        }
    }
}
