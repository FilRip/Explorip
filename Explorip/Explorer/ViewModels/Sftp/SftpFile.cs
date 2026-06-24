using System.IO;

using CommunityToolkit.Mvvm.ComponentModel;

using Explorip.Helpers;

using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace Explorip.Explorer.ViewModels.Sftp;

public partial class SftpFile : SftpItem
{
    [ObservableProperty()]
    private long _size;

    public SftpFile(ISftpFile file, SftpClient sftpClient) : base(file, sftpClient)
    {
        _size = file.Length;
        Icon = IconManager.GetIconFromExtension(Path.GetExtension(file.Name));
    }

    protected override void ContextMenuInternal()
    {
    }

    protected override void MouseMoveInternal()
    {
    }
}
