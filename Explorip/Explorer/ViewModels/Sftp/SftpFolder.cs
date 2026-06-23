using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

namespace Explorip.Explorer.ViewModels.Sftp;

public partial class SftpFolder : SftpItem
{
    private static SftpFolder _dummyFolder = new(null);

    public SftpFolder(Renci.SshNet.Sftp.ISftpFile file) : base(file)
    {
        Icon = Constants.Icons.SmallFolder;
    }

    [ObservableProperty()]
    private ObservableCollection<SftpFolder> _children;

    [ObservableProperty()]
    private bool _isExpanded;

    protected override void ContextMenuInternal()
    {
    }

    protected override void MouseMoveInternal()
    {
    }
}
