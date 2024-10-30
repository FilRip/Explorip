using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

using static ManagedShell.Interop.NativeMethods;

namespace ExploripComponents;

public static class FastDirectoryEnumerator
{
    public static void EnumerateFolderContent(string path, out List<string> subFolder, out List<string> files, string filter = "*.*")
    {
        subFolder = [];
        files = [];
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentNullException(nameof(path));
        SafeSearchHandle handle;
        if (!path.EndsWith(Path.DirectorySeparatorChar.ToString()))
            path += Path.DirectorySeparatorChar;
        handle = FindFirstFile(path + filter, out Win32FindData dataFind);
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
                if (!FindNextFile(handle, out dataFind))
                    break;
            }
        }
        handle.Dispose();
    }

    public static void FolderSize(string path, ref ulong size, CancellationToken token, string filter = "*.*")
    {
        List<string> subFolders = [];
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentNullException(nameof(path));
        SafeSearchHandle handle;
        if (!path.EndsWith(Path.DirectorySeparatorChar.ToString()))
            path += Path.DirectorySeparatorChar;
        handle = FindFirstFile(path + filter, out Win32FindData dataFind);
        if (!handle.IsInvalid && !token.IsCancellationRequested)
        {
            while (true)
            {
                if (dataFind.dwFileAttributes.HasFlag(FileAttributes.Directory))
                {
                    if (dataFind.cFileName != "." && dataFind.cFileName != "..")
                        subFolders.Add(dataFind.cFileName);
                }
                else
                {
                    size += ((ulong)dataFind.nFileSizeHigh << 32) + dataFind.nFileSizeLow;
                }
                if (!FindNextFile(handle, out dataFind) || token.IsCancellationRequested)
                    break;
            }
        }
        handle.Dispose();
        if (subFolders.Count > 0)
            foreach (string subFolder in subFolders)
            {
                FolderSize(Path.Combine(path, subFolder), ref size, token, filter);
                if (token.IsCancellationRequested)
                    return;
            }
    }

    public static void EnumerateNetworkRoot(ref List<string> elements, NetResource? parentNetResource = null)
    {
        // TODO https://stackoverflow.com/questions/2557551/how-get-list-of-local-network-computers
        try
        {
            // WNetOpenEnum not working (seems require SMBv1)
            /*int result;
            IntPtr ptrHandle = IntPtr.Zero;
            parentNetResource ??= new NetResource();
            result = WNetOpenEnum(ResourceScope.GlobalNetwork, ResourceType.Any, ResourceUsage.All, parentNetResource.Value, ref ptrHandle);
            if (result != 0)
            {
                if (result == (int)WNetOpenEnumError.ERROR_EXTENDED_ERROR)
                {
                    StringBuilder sbErrorMsg = new(256);
                    StringBuilder sbErrorName = new(256);
                    WNetGetLastError(out uint errorCode, sbErrorMsg, 256, sbErrorName, 256);
                    elements.Add(sbErrorMsg.ToString());
                }
                return;
            }

            int entries;
            uint buffer = 16384;
            IntPtr ptrBuffer = Marshal.AllocHGlobal((int)buffer);
            NetResource netResource;
            while (true)
            {
                entries = -1;
                buffer = 16384;
                result = WNetEnumResource(ptrHandle, ref entries, ptrBuffer, ref buffer);
                if ((result != 0) || (entries < 1))
                    break;

                IntPtr ptr = ptrBuffer;
                for (int i = 0; i < entries; i++)
                {
                    netResource = (NetResource)Marshal.PtrToStructure(ptr, typeof(NetResource));
                    if (netResource.dwUsage.HasFlag(ResourceUsage.Container))
                    {
                        //call recursively to get all entries in a container
                        EnumerateNetworkRoot(ref elements, netResource);
                    }
                    ptr += Marshal.SizeOf(netResource);
                    if (!string.IsNullOrWhiteSpace(netResource.LocalName))
                        elements.Add(netResource.LocalName);
                }
            }
            Marshal.FreeHGlobal(ptrBuffer);
            WNetCloseEnum(ptrHandle);*/
        }
        catch (Exception) { /* Ignore errors */ }
    }
}
