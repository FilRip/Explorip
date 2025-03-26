using System.Linq;
using System.Windows;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Explorip.StartMenu.Controls;
using Explorip.StartMenu.Window;

using ExploripConfig.Configuration;

namespace Explorip.StartMenu.ViewModels;

public partial class CustomColorViewModel : ObservableObject
{
    [ObservableProperty()]
    private SolidColorBrush _currentBackground;
    [ObservableProperty()]
    private Color _currentColor;
    [ObservableProperty()]
    private bool _OpenColorPicker;

    [RelayCommand()]
    private void Save()
    {
        SolidColorBrush brush = new(CurrentColor);
        ConfigManager.StartMenuBackground = brush;
        StartMenuWindow.MyStartMenu.Background = brush;
    }

    [RelayCommand()]
    private void Close()
    {
        CustomColor win = Application.Current.Windows.OfType<CustomColor>().FirstOrDefault();
        win?.Close();
    }

    public void Init()
    {
        SolidColorBrush brush = ConfigManager.StartMenuBackground;
        if (brush != null)
        {
            CurrentBackground = brush;
            CurrentColor = brush.Color;
        }
    }
}
