using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Explorip.FilesOperations.Interfaces;
using Explorip.Helpers;
using Explorip.WinAPI;
using Explorip.WinAPI.Modeles;

namespace Explorip.FilesOperations
{
    public class FileOperation : IDisposable
    {
        private static readonly Guid CLSID_FileOperation = new Guid("3ad05575-8857-4850-9277-11b85bdb8e09");
        private static readonly Type _fileOperationType = Type.GetTypeFromCLSID(CLSID_FileOperation);
        private static Guid _shellItemGuid = typeof(IShellItem).GUID;

        private bool _disposed;
        private readonly IFileOperation _fileOperation;
        private readonly FileOperationProgressSink _callbackSink;
        private readonly uint _sinkCookie;

        public FileOperation() : this(null) { }
        public FileOperation(FileOperationProgressSink callbackSink) : this(callbackSink, null) { }
        public FileOperation(FileOperationProgressSink callbackSink, IWin32Window owner)
        {
            _callbackSink = callbackSink;
            _fileOperation = (IFileOperation)Activator.CreateInstance(_fileOperationType);

            _fileOperation.SetOperationFlags(FileOperationFlags.FOF_NOCONFIRMMKDIR | FileOperationFlags.FOFX_ADDUNDORECORD);
            if (_callbackSink != null) _sinkCookie = _fileOperation.Advise(_callbackSink);
            if (owner != null) _fileOperation.SetOwnerWindow((uint)owner.Handle);
        }

        public void CopyItem(string source, string destination, string newName)
        {
            ThrowIfDisposed();
            using (ComReleaser<IShellItem> sourceItem = CreateShellItem(source))
            using (ComReleaser<IShellItem> destinationItem = CreateShellItem(destination))
            {
                _fileOperation.SetOperationFlags(FileOperationFlags.FOF_RENAMEONCOLLISION);
                _fileOperation.CopyItem(sourceItem.Item, destinationItem.Item, newName, null);
            }
        }

        public void MoveItem(string source, string destination, string newName)
        {
            ThrowIfDisposed();
            using (ComReleaser<IShellItem> sourceItem = CreateShellItem(source))
            using (ComReleaser<IShellItem> destinationItem = CreateShellItem(destination))
            {
                _fileOperation.MoveItem(sourceItem.Item, destinationItem.Item, newName, null);
            }
        }

        public void RenameItem(string source, string newName)
        {
            ThrowIfDisposed();
            using (ComReleaser<IShellItem> sourceItem = CreateShellItem(source))
            {
                _fileOperation.RenameItem(sourceItem.Item, newName, null);
            }
        }

        public void DeleteItem(string source)
        {
            ThrowIfDisposed();
            using (ComReleaser<IShellItem> sourceItem = CreateShellItem(source))
            {
                _fileOperation.DeleteItem(sourceItem.Item, null);
            }
        }

        public void NewItem(string folderName, string name, FileAttributes attrs)
        {
            ThrowIfDisposed();
            using (ComReleaser<IShellItem> folderItem = CreateShellItem(folderName))
            {
                _fileOperation.NewItem(folderItem.Item, attrs, name, string.Empty, _callbackSink);
            }
        }

        public void PerformOperations()
        {
            ThrowIfDisposed();
            try
            {
                _fileOperation.PerformOperations();
            }
            catch (Exception) { }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(GetType().Name);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                if (_callbackSink != null) _fileOperation.Unadvise(_sinkCookie);
                Marshal.FinalReleaseComObject(_fileOperation);
            }
        }

        private static ComReleaser<IShellItem> CreateShellItem(string path)
        {
            return new ComReleaser<IShellItem>((IShellItem)Shell32.SHCreateItemFromParsingName(path, null, ref _shellItemGuid));
        }
    }
}
