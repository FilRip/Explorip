using CommunityToolkit.Mvvm.ComponentModel;

namespace Explorip.Explorer.ViewModels;

public partial class WpfExplorerBrowserViewModel : ObservableObject
{
    public WpfExplorerBrowserViewModel() : base()
    {
    }

    [ObservableProperty()]
    private bool _windowMaximized;

    [ObservableProperty()]
    private bool _selectionLeft;

    [ObservableProperty()]
    private bool _selectionRight;
}
