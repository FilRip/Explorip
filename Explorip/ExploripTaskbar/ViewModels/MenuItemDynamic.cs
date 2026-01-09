using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

namespace Explorip.TaskBar.ViewModels;

public partial class MenuItemDynamic(string header) : ObservableObject
{
    [ObservableProperty()]
    private ObservableCollection<object> _children;
    [ObservableProperty()]
    private string _header = header;
    [ObservableProperty()]
    private bool _isEnabled = true;
}
