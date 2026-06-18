using CommunityToolkit.Mvvm.ComponentModel;

namespace Explorip.Explorer.ViewModels.Sftp;

public partial class SftpFile(Renci.SshNet.Sftp.ISftpFile file) : SftpItem(file)
{
    [ObservableProperty()]
    private long _size = file.Length;
}
