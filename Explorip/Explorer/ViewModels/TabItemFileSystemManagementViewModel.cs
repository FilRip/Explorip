using System;
using System.IO;

using CommunityToolkit.Mvvm.ComponentModel;

using DiscUtils;
using DiscUtils.Complete;

namespace Explorip.Explorer.ViewModels;

public partial class TabItemFileSystemManagementViewModel : ObservableObject, IDisposable
{
    [ObservableProperty()]
    private string _filePath;
    private readonly FileStream _fs;
    private bool disposedValue;
    [ObservableProperty(), NotifyPropertyChangedFor(nameof(ShowError))]
    private string _lastError;

    public TabItemFileSystemManagementViewModel(string path)
    {
        _filePath = path;
        try
        {
            _fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
            SetupHelper.SetupComplete();
            DiscUtils.FileSystemInfo[] detect = FileSystemManager.DetectFileSystems(_fs);
        }
        catch (Exception ex)
        {
            _lastError = ex.Message;
        }
    }

    public bool ShowError
    {
        get { return !string.IsNullOrWhiteSpace(LastError); }
    }

    #region IDisposable

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _fs.Close();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public bool IsDisposed
    {
        get { return disposedValue; }
    }

    #endregion
}
