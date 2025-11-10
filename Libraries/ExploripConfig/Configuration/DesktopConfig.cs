using System.Windows.Media;

using ExploripConfig.Helpers;

using Microsoft.Win32;

namespace ExploripConfig.Configuration;

public class DesktopConfig
{
    private RegistryKey _registryDesktop;

    public int NumScreen { get; private set; }
    public bool AllowWrite { get; private set; }

    public void Init(int numScreen, RegistryKey rootDesktop, bool allowWrite)
    {
        NumScreen = numScreen;
        AllowWrite = allowWrite;
        _registryDesktop = rootDesktop.CreateSubKey($"DISPLAY{numScreen}", true);

        if (allowWrite && _registryDesktop != null)
        {
            if (string.IsNullOrWhiteSpace(_registryDesktop.GetValue("ItemSizeX", "").ToString()))
                _registryDesktop.SetValue("ItemSizeX", "96");
            if (string.IsNullOrWhiteSpace(_registryDesktop.GetValue("ItemSizeY", "").ToString()))
                _registryDesktop.SetValue("ItemSizeY", "96");
            if (string.IsNullOrWhiteSpace(_registryDesktop.GetValue("HideBackground", "").ToString()))
                _registryDesktop.SetValue("HideBackground", "True");
            if (string.IsNullOrWhiteSpace(_registryDesktop.GetValue("BackgroundColor", "").ToString()))
                _registryDesktop.SetValue("BackgroundColor", "255,0,0,0");
        }
    }

    public int ItemSizeX
    {
        get { return _registryDesktop.ReadInteger("ItemSizeX"); }
        set
        {
            if (ItemSizeX != value && AllowWrite)
                _registryDesktop.SetValue("ItemSizeX", value.ToString());
        }
    }

    public int ItemSizeY
    {
        get { return _registryDesktop.ReadInteger("ItemSizeY"); }
        set
        {
            if (ItemSizeY != value && AllowWrite)
                _registryDesktop.SetValue("ItemSizeY", value.ToString());
        }
    }

    public bool HideBackground
    {
        get { return _registryDesktop.ReadBoolean("HideBackground"); }
        set
        {
            if (HideBackground != value && AllowWrite)
                _registryDesktop.SetValue("HideBackground", value.ToString());
        }
    }

    public SolidColorBrush DesktopBackground
    {
        get
        {
            string bgColor = _registryDesktop.GetValue("BackgroundColor")?.ToString();
            if (!string.IsNullOrWhiteSpace(bgColor))
            {
                return new SolidColorBrush(_registryDesktop.ReadColor("BackgroundColor", ExploripSharedCopy.Constants.Colors.BackgroundColor));
            }
            return null;
        }
        set
        {
            if (AllowWrite)
                _registryDesktop.SetValue("BackgroundColor", $"{value.Color.A},{value.Color.R},{value.Color.G},{value.Color.B}");
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
