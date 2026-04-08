using CommunityToolkit.Mvvm.ComponentModel;

using ExploripConfig.Configuration;

namespace Explorip.Explorer.ViewModels;

public partial class WpfExplorerBrowserViewModel : ObservableObject
{
    public WpfExplorerBrowserViewModel() : base()
    {
        _middleFileOperationFontSize = ConfigManager.ExplorerMiddleFileOperationFontSize;
    }

    [ObservableProperty()]
    private bool _windowMaximized;

    [ObservableProperty()]
    private bool _selectionLeft;

    [ObservableProperty()]
    private bool _selectionRight;

    [ObservableProperty()]
    private double _middleFileOperationFontSize;
}
