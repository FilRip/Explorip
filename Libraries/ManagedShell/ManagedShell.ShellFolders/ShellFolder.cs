﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

using ManagedShell.Common.Common;
using ManagedShell.Common.Logging;
using ManagedShell.Interop;
using ManagedShell.ShellFolders.Enums;
using ManagedShell.ShellFolders.Interfaces;

namespace ManagedShell.ShellFolders;

public class ShellFolder : ShellItem
{
    private static string _userDesktopPath;

    private readonly IntPtr _hwndInput;
    private readonly bool _loadAsync;
    private readonly ChangeWatcher _changeWatcher;

    private IntPtr _shellFolderPtr;

    public bool IsDesktop { get; private set; }

    private IShellFolder _shellFolder;

    public IShellFolder ShellFolderInterface
    {
        get
        {
            if (_shellFolder == null && AbsolutePidl != IntPtr.Zero)
            {
                _shellFolder = GetShellFolder(AbsolutePidl);
            }

            return _shellFolder;
        }
    }

    private ThreadSafeObservableCollection<ShellFile> _files;

    public ThreadSafeObservableCollection<ShellFile> Files
    {
        get
        {
            if (_files != null)
            {
                return _files;
            }

            _files = [];
            Initialize();

            return _files;
        }
    }

    public ShellFolder(string parsingName, IntPtr hwndInput, bool loadAsync = false) : this(parsingName, hwndInput, loadAsync, true)
    { }

    public ShellFolder(string parsingName, IntPtr hwndInput, bool loadAsync, bool watchChanges) : base(parsingName)
    {
        _hwndInput = hwndInput;
        _loadAsync = loadAsync;

        HandleDesktopFolder(parsingName);

        if (_shellItem == null && parsingName.StartsWith("{"))
        {
            parsingName = "::" + parsingName;
            _shellItem = GetShellItem(parsingName);
        }

        if (_shellItem == null && !parsingName.ToLower().StartsWith("shell:"))
        {
            parsingName = "shell:" + parsingName;
            _shellItem = GetShellItem(parsingName);
        }

        if (_shellItem != null && IsFileSystem && IsFolder)
        {
            List<string> watchList =
            [
                Path
            ];

            if (IsDesktop)
            {
                // The Desktop combines user and common desktop directories, so we need to watch both for changes.

                string publicDesktopPath = Environment.GetFolderPath(
                    Environment.SpecialFolder.CommonDesktopDirectory, Environment.SpecialFolderOption.DoNotVerify);

                if (!string.IsNullOrEmpty(publicDesktopPath))
                {
                    watchList.Add(publicDesktopPath);
                }
            }

            if (watchChanges)
            {
                _changeWatcher = new ChangeWatcher(watchList, ChangedEventHandler, CreatedEventHandler, DeletedEventHandler, RenamedEventHandler);
            }
        }
    }

    private void HandleDesktopFolder(string parsingName)
    {
        // The Desktop can only be acquired via SHGetDesktopFolder*. If our parsing name matches the desktop directory, use this logic.
        // *This isn't true on Windows 10, but we should use this logic anyway to provide consistency with SHCreateDesktop behavior.

        if (_userDesktopPath == null)
        {
            SetUserDesktopPath();
        }

        if (!string.IsNullOrEmpty(_userDesktopPath) && parsingName.ToLower() == _userDesktopPath)
        {
            IsDesktop = true;

            // Dispose of things that may have been created as part of the ShellItem constructor
            if (_shellItem != null)
            {
                Marshal.FinalReleaseComObject(_shellItem);
                _shellItem = null;
            }

            // Set properties based on SHGetDesktopFolder
            _shellFolder = GetShellFolder();
            Interop.SHGetIDListFromObject(_shellFolder, out _absolutePidl);
            Interop.SHCreateItemFromIDList(_absolutePidl, typeof(IShellItem).GUID, out _shellItem);
        }
    }

    private static void SetUserDesktopPath(string newPath)
    {
        _userDesktopPath = newPath;
    }
    private static void SetUserDesktopPath()
    {
        SetUserDesktopPath(Environment.GetFolderPath(Environment.SpecialFolder.Desktop, Environment.SpecialFolderOption.DoNotVerify).ToLower());
    }

    private void Initialize()
    {
        // If this method is called outside of the main thread, deadlocks may occur

        // Even if this ShellFolder was instantiated within an async folder enumeration, set IShellFolder here instead of the new thread.
        // The IShellFolder is used later (such as in context menus) where we need to be running on the UI thread, and the
        // IShellFolder from the background thread cannot be used.
        if (ShellFolderInterface == null)
        {
            return;
        }

        _changeWatcher?.StartWatching();

        if (_loadAsync)
        {
            // Enumerate the directory on a new thread so that we don't block the UI during a potentially long operation
            // Because files is an ObservableCollection, we don't need to do anything special for the UI to update
            Task.Factory.StartNew(Enumerate, CancellationToken.None, TaskCreationOptions.None, Interop.ShellItemScheduler);
        }
        else
        {
            Enumerate();
        }
    }

    private void Enumerate()
    {
        IntPtr hEnum = IntPtr.Zero;

        Files.Clear();

        try
        {
            if (ShellFolderInterface?.EnumObjects(_hwndInput, SHCONTF.FOLDERS | SHCONTF.NONFOLDERS,
                out hEnum) == NativeMethods.S_OK)
            {
                try
                {
                    IEnumIDList enumIdList =
                        (IEnumIDList)Marshal.GetTypedObjectForIUnknown(hEnum, typeof(IEnumIDList));

                    while (enumIdList.Next(1, out IntPtr pidlChild, out uint numFetched) == NativeMethods.S_OK && numFetched == 1)
                    {
                        if (_isDisposed)
                        {
                            break;
                        }

                        AddFile(pidlChild);
                    }

                    Marshal.FinalReleaseComObject(enumIdList);
                }
                catch (Exception e)
                {
                    ShellLogger.Error($"ShellFolder: Exception while enumerating IShellFolder: {e.Message}");
                }
                finally
                {
                    Marshal.Release(hEnum);
                }
            }
            else
            {
                ShellLogger.Error($"ShellFolder: Unable to enumerate IShellFolder of " + Path);
            }
        }
        catch (Exception e)
        {
            ShellLogger.Error($"ShellFolder: Unable to enumerate IShellFolder: {e.Message}");
        }
    }

    private void ChangedEventHandler(object sender, FileSystemEventArgs e)
    {
        Task.Factory.StartNew(() =>
        {
            ShellLogger.Info($"ShellFolder: Item {e.ChangeType}: {e.Name} ({e.FullPath})");

            bool exists = false;

            foreach (ShellFile file in Files)
            {
                if (_isDisposed)
                {
                    break;
                }

                if (file.Path == e.FullPath)
                {
                    exists = true;
                    file.Refresh();

                    break;
                }
            }

            if (!exists)
            {
                AddFile(e.FullPath);
            }
        }, CancellationToken.None, TaskCreationOptions.None, Interop.ShellItemScheduler);
    }

    private void CreatedEventHandler(object sender, FileSystemEventArgs e)
    {
        Task.Factory.StartNew(() =>
        {
            ShellLogger.Info($"ShellFolder: Item {e.ChangeType}: {e.Name} ({e.FullPath})");

            if (!FileExists(e.FullPath))
            {
                AddFile(e.FullPath);
            }
        }, CancellationToken.None, TaskCreationOptions.None, Interop.ShellItemScheduler);
    }

    private void DeletedEventHandler(object sender, FileSystemEventArgs e)
    {
        Task.Factory.StartNew(() =>
        {
            ShellLogger.Info($"ShellFolder: Item {e.ChangeType}: {e.Name} ({e.FullPath})");

            RemoveFile(e.FullPath);
        }, CancellationToken.None, TaskCreationOptions.None, Interop.ShellItemScheduler);
    }

    private void RenamedEventHandler(object sender, RenamedEventArgs e)
    {
        Task.Factory.StartNew(() =>
        {
            ShellLogger.Info($"ShellFolder: Item {e.ChangeType}: From {e.OldName} ({e.OldFullPath}) to {e.Name} ({e.FullPath})");

            int existing = RemoveFile(e.OldFullPath);

            if (!FileExists(e.FullPath))
            {
                AddFile(e.FullPath, existing);
            }
        }, CancellationToken.None, TaskCreationOptions.None, Interop.ShellItemScheduler);
    }

    private static IShellFolder GetShellFolder()
    {
        Interop.SHGetDesktopFolder(out IntPtr desktopFolderPtr);
        return (IShellFolder)Marshal.GetTypedObjectForIUnknown(desktopFolderPtr, typeof(IShellFolder));
    }

    private IShellFolder GetShellFolder(IntPtr folderPidl)
    {
        IShellFolder desktop = GetShellFolder();
        Guid guid = typeof(IShellFolder).GUID;

        if (desktop.BindToObject(folderPidl, IntPtr.Zero, ref guid, out _shellFolderPtr) == NativeMethods.S_OK)
        {
            Marshal.ReleaseComObject(desktop);
            return (IShellFolder)Marshal.GetTypedObjectForIUnknown(_shellFolderPtr, typeof(IShellFolder));
        }

        ShellLogger.Error($"ShellFolder: Unable to bind IShellFolder for {folderPidl}");
        return null;
    }

    #region Helpers

#pragma warning disable S3241
    private bool AddFile(string parsingName, int position = -1)
    {
        ShellFile file = new(this, parsingName);

        return AddFile(file, position);
    }

    private bool AddFile(IntPtr relPidl, int position = -1)
    {
        ShellFile file = new(this, ShellFolderInterface, relPidl, _loadAsync);

        return AddFile(file, position);
    }
#pragma warning restore S3241

    private bool AddFile(ShellFile file, int position = -1)
    {
        if (file.Loaded)
        {
            if (position >= 0)
            {
                Files.Insert(position, file);
            }
            else
            {
                Files.Add(file);
            }

            return true;
        }

        return false;
    }

    private int RemoveFile(string parsingName)
    {
        for (int i = 0; i < Files.Count; i++)
        {
            if (Files[i].Path == parsingName)
            {
                ShellFile file = Files[i];
                Files.RemoveAt(i);
                file.Dispose();

                return i;
            }

            if (_isDisposed)
            {
                break;
            }
        }

        return -1;
    }

    private bool FileExists(string parsingName)
    {
        bool exists = false;

        foreach (ShellFile file in Files)
        {
            if (file.Path == parsingName)
            {
                exists = true;
                break;
            }

            if (_isDisposed)
            {
                break;
            }
        }

        return exists;
    }
    #endregion

    protected override void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                _changeWatcher?.Dispose();

                try
                {
                    if (_files != null)
                    {
                        foreach (ShellFile file in Files)
                        {
                            file.Dispose();
                        }

                        Files.Clear();
                    }
                }
                catch (Exception e)
                {
                    ShellLogger.Warning($"ShellFolder: Unable to dispose files: {e.Message}");
                }

                if (_shellFolder != null)
                {
                    Marshal.ReleaseComObject(_shellFolder);
                    _shellFolder = null;
                }

                if (_shellFolderPtr != IntPtr.Zero)
                {
                    Marshal.Release(_shellFolderPtr);
                    _shellFolderPtr = IntPtr.Zero;
                }
            }
            base.Dispose(disposing);
        }
    }
}
