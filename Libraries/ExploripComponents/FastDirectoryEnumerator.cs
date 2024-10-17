using System;
using System.Collections.Generic;
using System.IO;

using ManagedShell.Interop;

namespace ExploripComponents;

public static class FastDirectoryEnumerator
{
    public static void EnumerateFolderContent(string path, out List<string> subFolder, out List<string> files, string filter = "*.*")
    {
        subFolder = [];
        files = [];
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentNullException(nameof(path));
        NativeMethods.SafeSearchHandle handle;
        if (!path.EndsWith(Path.DirectorySeparatorChar.ToString()))
            path += Path.DirectorySeparatorChar;
        handle = NativeMethods.FindFirstFile(path + filter, out NativeMethods.Win32FindData dataFind);
        if (!handle.IsInvalid)
        {
            while (true)
            {
                if (dataFind.dwFileAttributes.HasFlag(FileAttributes.Directory))
                {
                    if (dataFind.cFileName != "." && dataFind.cFileName != "..")
                        subFolder.Add(dataFind.cFileName);
                }
                else
                    files.Add(dataFind.cFileName);
                if (!NativeMethods.FindNextFile(handle, out dataFind))
                    break;
            }
        }
        handle.Dispose();
    }
}
