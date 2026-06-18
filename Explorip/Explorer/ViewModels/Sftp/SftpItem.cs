using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;

namespace Explorip.Explorer.ViewModels.Sftp;

public abstract partial class SftpItem(Renci.SshNet.Sftp.ISftpFile file) : ObservableObject
{
    [ObservableProperty()]
    private string _name = file.Name;

    [ObservableProperty()]
    private ImageSource _icon;

    [ObservableProperty()]
    private string _fullPath = file.FullName;
}
