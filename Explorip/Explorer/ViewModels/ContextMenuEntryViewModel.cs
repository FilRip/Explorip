using System.Collections.ObjectModel;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Explorip.Explorer.Helpers;

namespace Explorip.Explorer.ViewModels;

public partial class ContextMenuEntryViewModel(ShellContextMenuEntry entry) : ObservableObject
{
    private readonly ShellContextMenuEntry _entry = entry;

    [ObservableProperty()]
    private ImageSource _icon;
    [ObservableProperty()]
    private string _label;
    [ObservableProperty()]
    private ObservableCollection<ContextMenuEntryViewModel> _subItems;

    [RelayCommand()]
    private void Execute()
    {

    }
}
