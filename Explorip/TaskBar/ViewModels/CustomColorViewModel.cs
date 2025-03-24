using System.Linq;
using System.Windows;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Explorip.TaskBar.Controls;

using ExploripConfig.Configuration;

namespace Explorip.TaskBar.ViewModels;

public partial class CustomColorViewModel : ObservableObject
{
    private Taskbar _parentTaskbar;

    [ObservableProperty()]
    private SolidColorBrush _currentBackground;
    [ObservableProperty()]
    private SolidColorBrush _currentTextBackground;
    [ObservableProperty()]
    private SolidColorBrush _currentTextForeground;
    [ObservableProperty()]
    private Color _currentColor;
    [ObservableProperty()]
    private bool _OpenColorPicker;

    [RelayCommand()]
    private void Save()
    {
        SolidColorBrush brush = new(CurrentColor);
        ConfigManager.GetTaskbarConfig(ParentTaskbar.ScreenName).TaskbarBackground = brush;
        ParentTaskbar.Background = brush;
    }

    public Taskbar ParentTaskbar
    {
        get { return _parentTaskbar; }
        set
        {
            _parentTaskbar = value;
            Init();
        }
    }

    [RelayCommand()]
    private void Close()
    {
        CustomColor win = Application.Current.Windows.OfType<CustomColor>().FirstOrDefault();
        win?.Close();
    }

    public void Init()
    {
        SolidColorBrush brush = ConfigManager.GetTaskbarConfig(ParentTaskbar.ScreenName).TaskbarBackground;
        if (brush != null)
        {
            CurrentBackground = brush;
            CurrentColor = brush.Color;
        }
        CurrentTextBackground = ExploripSharedCopy.Constants.Colors.BackgroundColorBrush;
        CurrentTextForeground = ExploripSharedCopy.Constants.Colors.ForegroundColorBrush;
    }
}
