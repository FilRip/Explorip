using System.Collections.ObjectModel;
using System.Linq;

using CommunityToolkit.Mvvm.ComponentModel;

using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace Explorip.Explorer.ViewModels.Sftp;

public partial class SftpFolder : SftpItem
{
    public SftpFolder(ISftpFile file, SftpClient sftpClient) : base(file, sftpClient)
    {
        Icon = Constants.Icons.SmallFolder;
    }

    [ObservableProperty()]
    private ObservableCollection<SftpFolder> _children = [];

    [ObservableProperty()]
    private bool _isExpanded;

    protected override void ContextMenuInternal()
    {
    }

    protected override void MouseMoveInternal()
    {
    }

    partial void OnIsExpandedChanged(bool value)
    {
        if (IsExpanded)
        {
            Children.Clear();
            foreach (ISftpFile file in _sftpClient.ListDirectory(FullPath ?? "/").Where(f => f.IsDirectory))
            {
                Children.Add(new SftpFolder(file, _sftpClient));
                bool hasSubDir = _sftpClient.ListDirectory(file.FullName)?.OfType<ISftpFile>()?.Any(f => f.IsDirectory) ?? false;
                if (hasSubDir)
                    Children[Children.Count - 1].Children.Add(TabItemSftpViewModel.DummyFolder);
            }
        }
    }
}
