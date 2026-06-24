using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Explorip.Explorer.ViewModels.Sftp;

using Renci.SshNet;
using Renci.SshNet.Sftp;

using SftpFile = Explorip.Explorer.ViewModels.Sftp.SftpFile;

namespace Explorip.Explorer.ViewModels;

public partial class TabItemSftpViewModel : ObservableObject, IDisposable
{
    internal static readonly SftpFolder DummyFolder = new(null, null);
    private static readonly object _lockSftpClient = new();

    private SftpClient _sftpClient;
    private bool disposedValue;

    [ObservableProperty()]
    private ObservableCollection<SftpFolder> _listFolders = [];
    [ObservableProperty()]
    private ObservableCollection<SftpFile> _listFiles = [];
    [ObservableProperty()]
    private string _host, _user, _password;
    [ObservableProperty()]
    private int _port = 22, _timeoutActivity = 5;
    [ObservableProperty()]
    private SftpFolder _selectedFolder;
    [ObservableProperty()]
    private bool _showError;
    [ObservableProperty()]
    private string _errorMessage;

    public List<SftpItem> ListSelected { get; set; } = [];

    [RelayCommand()]
    private async Task Connection()
    {
        _sftpClient?.Dispose();
        _sftpClient = new SftpClient(Host, Port, User, Password)
        {
            OperationTimeout = TimeSpan.FromSeconds(TimeoutActivity),
            ConnectionInfo = { Timeout = TimeSpan.FromSeconds(TimeoutActivity) }
        };
        CancellationToken token = new();
        ListFolders.Clear();
        ShowError = false;
        try
        {
            await _sftpClient.ConnectAsync(token);
            Refresh();
        }
        catch (Exception ex)
        {
            ShowError = true;
            ErrorMessage = ex.Message;
        }
    }

    [RelayCommand()]
    private void Disconnection()
    {
        _sftpClient.Dispose();
    }

    partial void OnSelectedFolderChanged(SftpFolder value)
    {
        Refresh();
    }

    private void Refresh(SftpFolder folder = null)
    {
        lock (_lockSftpClient)
        {
            ShowError = false;
            ListFiles.Clear();
            folder ??= SelectedFolder;
            folder?.Children?.Clear();
            try
            {
                foreach (ISftpFile file in _sftpClient.ListDirectory(SelectedFolder?.FullPath ?? "/"))
                {
                    if (file.IsDirectory)
                    {
                        if (SelectedFolder == null)
                            ListFolders.Add(new SftpFolder(file, _sftpClient));
                        else
                            folder.Children.Add(new SftpFolder(file, _sftpClient));
                        bool hasSubDir = _sftpClient.ListDirectory(file.FullName)?.OfType<ISftpFile>()?.Any(f => f.IsDirectory) ?? false;
                        if (hasSubDir)
                            if (SelectedFolder == null)
                                ListFolders[ListFolders.Count - 1].Children.Add(DummyFolder);
                            else
                                folder.Children[folder.Children.Count - 1].Children.Add(DummyFolder);
                    }
                    else
                        ListFiles.Add(new SftpFile(file, _sftpClient));
                }
            }
            catch (Exception ex)
            {
                ShowError = true;
                ErrorMessage = ex.Message;
            }
        }
    }

    #region IDisposable

    public bool IsDisposed
    {
        get { return disposedValue; }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                ListFolders?.Clear();
                ListFiles?.Clear();
                _sftpClient?.Dispose();
            }
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion
}
