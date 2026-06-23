using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Explorip.Explorer.ViewModels.Sftp;

public abstract partial class SftpItem(Renci.SshNet.Sftp.ISftpFile file) : ObservableObject
{
    [ObservableProperty()]
    private string _name = file?.Name;

    [ObservableProperty()]
    private ImageSource _icon;

    [ObservableProperty()]
    private string _fullPath = file?.FullName;

    [ObservableProperty()]
    private bool _isSelected = false;

    [ObservableProperty()]
    private string _newName;

    [ObservableProperty()]
    private bool _renameMode = false;

    [ObservableProperty()]
    private bool _readOnlyBox = true;

    protected abstract void ContextMenuInternal();

    [RelayCommand()]
    public void ContextMenu()
    {
        ContextMenuInternal();
    }

    protected abstract void MouseMoveInternal();

    [RelayCommand()]
    public void MouseMove()
    {
        MouseMoveInternal();
    }

    [ObservableProperty()]
    private double _opacity = 1;
}
