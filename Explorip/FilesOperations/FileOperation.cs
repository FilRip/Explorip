using System;
using System.IO;
using System.Runtime.InteropServices;

using Explorip.FilesOperations.Interfaces;
using Explorip.Helpers;

using ManagedShell.ShellFolders.Interfaces;

namespace Explorip.FilesOperations;

public class FileOperation : IDisposable
{
    private static readonly Guid CLSID_FileOperation = new("3ad05575-8857-4850-9277-11b85bdb8e09");
    private static readonly Type _fileOperationType = Type.GetTypeFromCLSID(CLSID_FileOperation);
    private static Guid _shellItemGuid = typeof(IShellItem).GUID;

    private bool disposedValue;
    private readonly IFileOperation _fileOperation;
    private readonly FileOperationProgressSink _callbackSink;
    private readonly uint _sinkCookie;

    public FileOperation() : this(null) { }
    public FileOperation(FileOperationProgressSink callbackSink) : this(callbackSink, IntPtr.Zero) { }
    public FileOperation(FileOperationProgressSink callbackSink, IntPtr owner)
    {
        _callbackSink = callbackSink;
        _fileOperation = (IFileOperation)Activator.CreateInstance(_fileOperationType);

        _fileOperation.SetOperationFlags(EFileOperation.FOF_NOCONFIRMMKDIR | EFileOperation.FOFX_ADDUNDORECORD | EFileOperation.FOFX_RECYCLEONDELETE);
        if (_callbackSink != null)
            _sinkCookie = _fileOperation.Advise(_callbackSink);
        if (owner != IntPtr.Zero)
            _fileOperation.SetOwnerWindow((uint)owner);
    }
    public FileOperation(IntPtr owner) : this(null, owner) { }

    public void ChangeOperationFlags(EFileOperation flags)
    {
        _fileOperation.SetOperationFlags(flags);
    }

    public void ChangeLinkedWindow(IntPtr windowHandle)
    {
        _fileOperation.SetOwnerWindow((uint)windowHandle);
    }

    public void CopyItem(string source, string destination, string newName)
    {
        ThrowIfDisposed();
        using ComReleaser<IShellItem> sourceItem = CreateShellItem(source);
        using ComReleaser<IShellItem> destinationItem = CreateShellItem(destination);
        _fileOperation.CopyItem(sourceItem.Item, destinationItem.Item, newName, null);
    }

    public void MoveItem(string source, string destination, string newName)
    {
        ThrowIfDisposed();
        using ComReleaser<IShellItem> sourceItem = CreateShellItem(source);
        using ComReleaser<IShellItem> destinationItem = CreateShellItem(destination);
        _fileOperation.MoveItem(sourceItem.Item, destinationItem.Item, newName, null);
    }

    public void RenameItem(string source, string newName)
    {
        ThrowIfDisposed();
        using ComReleaser<IShellItem> sourceItem = CreateShellItem(source);
        _fileOperation.RenameItem(sourceItem.Item, newName, null);
    }

    public void DeleteItem(string source)
    {
        ThrowIfDisposed();
        using ComReleaser<IShellItem> sourceItem = CreateShellItem(source);
        _fileOperation.DeleteItem(sourceItem.Item, null);
    }

    public void NewItem(string folderName, string name, FileAttributes attrs)
    {
        ThrowIfDisposed();
        using ComReleaser<IShellItem> folderItem = CreateShellItem(folderName);
        _fileOperation.NewItem(folderItem.Item, attrs, name, string.Empty, _callbackSink);
    }

    public void PerformOperations()
    {
        ThrowIfDisposed();
        try
        {
            _fileOperation.PerformOperations();
        }
        catch (Exception) { /* Ignore errors */ }
    }

    private void ThrowIfDisposed()
    {
        if (disposedValue) throw new ObjectDisposedException(GetType().Name);
    }

    private static ComReleaser<IShellItem> CreateShellItem(string path)
    {
        return new ComReleaser<IShellItem>((IShellItem)ManagedShell.ShellFolders.Interop.SHCreateItemFromParsingName(path, null, ref _shellItemGuid));
    }

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
                if (_callbackSink != null) _fileOperation.Unadvise(_sinkCookie);
                Marshal.FinalReleaseComObject(_fileOperation);
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
