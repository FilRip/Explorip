using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Explorip.TaskBar.Helpers;

using ManagedShell.ShellFolders;

namespace Explorip.TaskBar.ViewModels;

public partial class PopupPreviewContentViewModel : ObservableObject
{
    [ObservableProperty()]
    private ObservableCollection<ShellFile> _listItems;
    [ObservableProperty()]
    private bool _visible;
    [ObservableProperty()]
    private ShellFile _selectedItem;

    public bool DisposeItems { get; set; }

    public PopupPreviewContentViewModel()
    {
        ListItems = [];
    }

    partial void OnVisibleChanging(bool value)
    {
        if (Visible && !value && DisposeItems)
            foreach (ShellFile sf in ListItems)
                sf.Dispose();
    }

    [RelayCommand()]
    private void Launch()
    {
        InvokeContextMenuHelper.InvokeContextMenu(SelectedItem, false);
    }

    [RelayCommand()]
    private void ContextMenu()
    {
        InvokeContextMenuHelper.InvokeContextMenu(SelectedItem, true);
    }
}
