using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Explorip.Explorer.Controls;
using Explorip.Explorer.Helpers;

namespace Explorip.Explorer.ViewModels;

public partial class ContextMenuEntryViewModel(ShellContextMenuEntry entry, PopUpExplorerContextMenuViewModel parent) : ObservableObject
{
    private readonly PopUpExplorerContextMenuViewModel _parent = parent;
    private PopUpExplorerContextMenuSubItems _popup;

    [ObservableProperty()]
    private ShellContextMenuEntry _entry = entry;
    [ObservableProperty(), NotifyPropertyChangedFor(nameof(HasSubItems))]
    private ObservableCollection<ContextMenuEntryViewModel> _subItems = [];
    [ObservableProperty()]
    private bool _isVisible = true;
    [ObservableProperty()]
    private bool _isEnabled = true;
    [ObservableProperty()]
    private bool _isOpen;

    public SolidColorBrush Background
    {
        get { return _parent.Background; }
    }

    public SolidColorBrush Foreground
    {
        get { return _parent.Foreground; }
    }

    public double Dpi
    {
        get { return _parent.Dpi; }
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

    public bool IsMouseOver { get; set; }

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
        Debug.WriteLine($"MouseEnter ContextMenu {Label}");
        IsMouseOver = true;
        if (HasSubItems && _popup == null)
        {
            Debug.WriteLine($"MouseEnter ContextMenu ShowSubItems {Label}");
            _popup = new PopUpExplorerContextMenuSubItems()
            {
                DataContext = this,
                IsOpen = true,
            };
            SubItems[0].IsMouseOver = true;
        }
    }

    [RelayCommand()]
    private void MouseLeave()
    {
        Debug.WriteLine($"MouseLeave ContextMenu {Label}");
        if (HasSubItems && _popup != null && !SubItems.Any(p => p.IsMouseOver))
        {
            Debug.WriteLine($"MouseLeave CloseSubItems {Label}");
            _popup.IsOpen = false;
            _popup = null;
        }
        IsMouseOver = false;
    }
}
