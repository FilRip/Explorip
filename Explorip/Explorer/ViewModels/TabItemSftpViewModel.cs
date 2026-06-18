using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Explorip.Explorer.ViewModels.Sftp;

using Renci.SshNet;

namespace Explorip.Explorer.ViewModels;

public partial class TabItemSftpViewModel : ObservableObject
{
    private SftpClient _sftpClient;

    [ObservableProperty()]
    private ObservableCollection<SftpFolder> _listFolders;
    [ObservableProperty()]
    private ObservableCollection<SftpFile> _listFiles;
    [ObservableProperty()]
    private string _host, _user, _password;
    [ObservableProperty()]
    private ushort _port = 22, _timeoutActivity = 5;

    public List<SftpItem> ListSelected { get; set; } = [];
    public SftpItem SelectedFolder { get; set; }

    [RelayCommand()]
    private async Task Connection()
    {
        _sftpClient = new SftpClient(Host, Port, User, Password)
        {
            OperationTimeout = TimeSpan.FromSeconds(TimeoutActivity),
            ConnectionInfo = { Timeout = TimeSpan.FromSeconds(TimeoutActivity) }
        };
        CancellationToken token = new();
        await _sftpClient.ConnectAsync(token);
        Refresh();
    }

    [RelayCommand()]
    private void Disconnection()
    {
        _sftpClient.Dispose();
    }

    private void Refresh()
    {
        ListFolders.Clear();
        ListFiles.Clear();
        foreach (Renci.SshNet.Sftp.ISftpFile file in _sftpClient.ListDirectory(SelectedFolder?.FullPath ?? "/"))
        {
            if (file.IsDirectory)
                ListFolders.Add(new SftpFolder(file));
            else
                ListFiles.Add(new SftpFile(file));
        }
    }
}
