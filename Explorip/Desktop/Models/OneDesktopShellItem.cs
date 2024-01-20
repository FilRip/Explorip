using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;

using Microsoft.WindowsAPICodePack.Shell;

namespace Explorip.Desktop.Models;

internal partial class OneDesktopShellItem : ObservableObject
{
    internal string FullPath { get; set; }

    [ObservableProperty()]
    private string _name;

    [ObservableProperty()]
    private ImageSource _icon;

    [ObservableProperty()]
    private bool _isSelected;

    private ShellObject _shellObject;
    internal ShellObject CurrentShellObject
    {
        get
        {
            _shellObject ??= ShellObject.FromParsingName(FullPath);
            return _shellObject;
        }
        set
        {
            _shellObject = value;
        }
    }
}
