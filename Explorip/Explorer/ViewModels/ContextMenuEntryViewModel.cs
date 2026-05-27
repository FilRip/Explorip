using System.Collections.ObjectModel;
using System.Diagnostics;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Explorip.Explorer.Controls;
using Explorip.Explorer.Helpers;

namespace Explorip.Explorer.ViewModels;

public partial class ContextMenuEntryViewModel : ObservableObject
{
    private PopUpExplorerContextMenuViewModel _parent;
    //private PopUpExplorerContextMenuSubItems _popup;

    [ObservableProperty()]
    private ShellContextMenuEntry _entry;
    [ObservableProperty(), NotifyPropertyChangedFor(nameof(HasSubItems))]
    private ObservableCollection<ContextMenuEntryViewModel> _subItems;
    [ObservableProperty()]
    private bool _isVisible;
    [ObservableProperty()]
    private bool _isEnabled;

    public ContextMenuEntryViewModel(ShellContextMenuEntry entry, PopUpExplorerContextMenuViewModel parent)
    {
        _parent = parent;
        _entry = entry;
        _isVisible = true;
        _isEnabled = true;
        _subItems = [];
        //_popup = new PopUpExplorerContextMenuSubItems();
    }

    public string Label
    {
        get { return Entry.Name.Replace("&", ""); }
    }

    public bool IconVisible
    {
        get { return Entry.Icon != null; }
    }

    public bool HasSubItems
    {
        get { return SubItems.Count > 0; }
    }

    [RelayCommand()]
    private void Execute()
    {
        if (HasSubItems)
            return;

        switch (Entry.Source)
        {
            case ETypeCommand.ShellVerb:
                string command, param;
                command = Entry.Command;
                param = _parent.ListSelected[0].ParsingName;
                int lastIndex;
                if (Entry.Command.StartsWith("\""))
                    lastIndex = command.IndexOf('\"', 1);
                else
                    lastIndex = command.IndexOf(' ');
                command = command.Substring(0, lastIndex + 1);
                if (Entry.Command.Length > lastIndex + 1)
                    param = Entry.Command.Substring(lastIndex + 1).Replace("%1", _parent.ListSelected[0].ParsingName).Trim();
                Process.Start(command, param);
                break;
            case ETypeCommand.ContextMenuHandler:
                break;
            case ETypeCommand.CommandStore:
                break;
            case ETypeCommand.SendTo:
                break;
            case ETypeCommand.CreateShortcut:
                break;
            case ETypeCommand.Rename:
                break;
            case ETypeCommand.Share:
                break;
            case ETypeCommand.New:
                break;
        }
    }

    [RelayCommand()]
    private void MouseEnter()
    {
        if (HasSubItems)
        {
            //_popup?.IsOpen = true;
        }
    }

    [RelayCommand()]
    private void MouseLeave()
    {
        if (HasSubItems)
        {
            //_popup?.IsOpen = false;
        }
    }
}
