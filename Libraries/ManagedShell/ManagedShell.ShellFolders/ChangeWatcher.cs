﻿using System;
using System.Collections.Generic;
using System.IO;

using ManagedShell.Common.Logging;

namespace ManagedShell.ShellFolders;

class ChangeWatcher : IDisposable
{
    private readonly FileSystemEventHandler _changedEventHandler;
    private readonly FileSystemEventHandler _createdEventHandler;
    private readonly FileSystemEventHandler _deletedEventHandler;
    private readonly RenamedEventHandler _renamedEventHandler;
    private List<FileSystemWatcher> _watchers = [];

    public ChangeWatcher(List<string> pathList, FileSystemEventHandler changedEventHandler, FileSystemEventHandler createdEventHandler, FileSystemEventHandler deletedEventHandler, RenamedEventHandler renamedEventHandler)
    {
        if (pathList == null || pathList.Count < 1)
        {
            return;
        }

        _changedEventHandler = changedEventHandler;
        _createdEventHandler = createdEventHandler;
        _deletedEventHandler = deletedEventHandler;
        _renamedEventHandler = renamedEventHandler;

        try
        {
            foreach (string path in pathList)
            {
                FileSystemWatcher watcher = new(path)
                {
                    NotifyFilter = NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size
                };

                watcher.Changed += _changedEventHandler;
                watcher.Created += _createdEventHandler;
                watcher.Deleted += _deletedEventHandler;
                watcher.Renamed += _renamedEventHandler;

                _watchers.Add(watcher);
            }

        }
        catch (Exception e)
        {
            ShellLogger.Error($"ChangeWatcher: Unable to instantiate watcher: {e.Message}");
        }
    }

    public void StartWatching()
    {
        foreach (FileSystemWatcher watcher in _watchers)
        {
            if (watcher == null)
            {
                ShellLogger.Error("ChangeWatcher: Unable to start watching directory because the watcher is null.");
                return;
            }

            try
            {
                watcher.EnableRaisingEvents = true;
            }
            catch (Exception e)
            {
                ShellLogger.Error($"ChangeWatcher: Unable to start watching directory: {e.Message}");
            }
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public bool IsDisposed
    {
        get { return _disposed; }
    }

    private bool _disposed;
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                if (_watchers == null)
                {
                    return;
                }

                foreach (FileSystemWatcher watcher in _watchers)
                {
                    if (watcher == null)
                    {
                        return;
                    }

                    watcher.Changed -= _changedEventHandler;
                    watcher.Created -= _createdEventHandler;
                    watcher.Deleted -= _deletedEventHandler;
                    watcher.Renamed -= _renamedEventHandler;

                    watcher.Dispose();
                }

                _watchers = null;
            }
            _disposed = true;
        }
    }
}
