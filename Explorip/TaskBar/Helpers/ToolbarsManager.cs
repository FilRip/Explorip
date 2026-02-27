using System;
using System.Collections.Generic;

using ManagedShell.ShellFolders;

namespace Explorip.TaskBar.Helpers;

internal static class ToolbarsManager
{
    private static readonly Dictionary<string, ShellFolder> _listFolders = [];
    private static readonly object _lock = new();

    public static ShellFolder GetToolbar(string path, bool loadAsync = true)
    {
        string expandedPath = Environment.ExpandEnvironmentVariables(path);
        lock (_lock)
        {
            if (_listFolders.TryGetValue(expandedPath, out ShellFolder result))
                return result;
            ShellFolder newToolbar = new(expandedPath, IntPtr.Zero, loadAsync);
            _ = newToolbar.Files;
            _listFolders.Add(expandedPath, newToolbar);
            return newToolbar;
        }
    }

    public static void DeleteToolbar(string path)
    {
        string expandedPath = Environment.ExpandEnvironmentVariables(path);
        lock (_lock)
        {
            if (_listFolders.TryGetValue(expandedPath, out ShellFolder toDelete))
            {
                if (toDelete.Files?.Count > 0)
                    foreach (ShellFile sf in toDelete.Files)
                        sf.Dispose();
                toDelete.Dispose();
                _listFolders.Remove(expandedPath);
            }
        }
    }
}
