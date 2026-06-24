using System;
using System.Net.Sockets;
using System.Text;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace Explorip.Explorer.ViewModels.Sftp;

public abstract partial class SftpItem(ISftpFile file, SftpClient sftpClient) : ObservableObject
{
    protected SftpClient _sftpClient = sftpClient;

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

    [ObservableProperty()]
    private DateTime? _lastModified = file?.LastWriteTime;

    [ObservableProperty()]
    private SolidColorBrush _backgroundColor = new(Colors.Transparent);

    [ObservableProperty()]
    private string _permissions = MakePermissions(file);

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
    private double _opacity = (file?.Name?.StartsWith(".") == true ? 0.7 : 1);

    public static string MakePermissions(ISftpFile file)
    {
        if (file == null)
            return "";

        StringBuilder sb = new();

#pragma warning disable S3358
        sb.Append(file.IsRegularFile ? '-' : (file.IsDirectory ? 'd' : (file.IsSymbolicLink ? 'l' : (file.IsNamedPipe ? 'p' : (file.IsSocket ? 's' : (file.IsCharacterDevice ? 'c' : (file.IsBlockDevice ? 'b' : '-')))))));
#pragma warning restore S3358
        sb.Append(file.OwnerCanRead ? 'r' : '-');
        sb.Append(file.OwnerCanWrite ? 'w' : '-');
        if (file.OwnerCanExecute)
            sb.Append(file.Attributes.IsUIDBitSet ? 's' : 'x');
        else
            sb.Append(file.Attributes.IsUIDBitSet ? 'S' : '-');

        sb.Append(file.GroupCanRead ? 'r' : '-');
        sb.Append(file.GroupCanWrite ? 'w' : '-');
        if (file.GroupCanExecute)
            sb.Append(file.Attributes.IsGroupIDBitSet ? 's' : 'x');
        else
            sb.Append(file.Attributes.IsGroupIDBitSet ? 'S' : '-');

        sb.Append(file.OthersCanRead ? 'r' : '-');
        sb.Append(file.OthersCanWrite ? 'w' : '-');
        if (file.OthersCanExecute)
            sb.Append(file.Attributes.IsStickyBitSet ? 't' : 'x');
        else
            sb.Append(file.Attributes.IsStickyBitSet ? 'T' : '-');

        return sb.ToString();
    }
}
