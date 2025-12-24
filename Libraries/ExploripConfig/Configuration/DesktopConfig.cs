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
        string pos = _registryDesktop.GetValue(itemName, "")?.ToString();
        if (!string.IsNullOrWhiteSpace(pos))
        {
            string[] splitter = pos.Split(',');
            return (int.Parse(splitter[0]), int.Parse(splitter[1]));
        }
        return (-1, -1);
    }

    public void SetItemPosition(string itemName, (int, int) position)
    {
        _registryDesktop.SetValue(itemName, $"{position.Item1},{position.Item2}");
    }
}
