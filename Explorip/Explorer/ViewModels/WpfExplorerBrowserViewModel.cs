using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;

using ExploripConfig.Configuration;

namespace Explorip.Explorer.ViewModels;

public partial class WpfExplorerBrowserViewModel : ObservableObject
{
    public WpfExplorerBrowserViewModel() : base()
    {
        _middleFileOperationFontSize = ConfigManager.ExplorerMiddleFileOperationFontSize;
        ContextMenuPopUp = new Popup()
        {
            AllowsTransparency = true,
            StaysOpen = false,
            Child = new Border()
            {
                CornerRadius = ConfigManager.ExplorerContextMenuCornerRadius,
                BorderThickness = new Thickness(0),
                Background = ConfigManager.ExplorerContextMenuBackground,
            }
        };
        ((Border)ContextMenuPopUp.Child).Child = new ItemsControl()
        {
            Margin = new Thickness(ConfigManager.ExplorerContextMenuCornerRadius.TopLeft),
            Background = Brushes.Transparent,
            Foreground = ExploripSharedCopy.Constants.Colors.ForegroundColorBrush,
        };
    }

    [ObservableProperty()]
    private bool _windowMaximized;

    [ObservableProperty()]
    private bool _selectionLeft;

    [ObservableProperty()]
    private bool _selectionRight;

    [ObservableProperty()]
    private double _middleFileOperationFontSize;

    public Popup ContextMenuPopUp { get; private set; }
}
