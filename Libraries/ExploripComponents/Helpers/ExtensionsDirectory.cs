using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

using Explorip.Constants;

using ManagedShell.Common.Helpers;
using ManagedShell.ShellFolders.Enums;
using ManagedShell.ShellFolders.Interfaces;

using static ManagedShell.Interop.NativeMethods;

namespace ExploripComponents.Helpers;

public static class ExtensionsDirectory
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

    public static string SearchRecycledBinPath(string drive, string localizedRecycleName)
    {
        string ret = "";
        string rbRoot = drive.Trim();
        if (!rbRoot.EndsWith(Path.DirectorySeparatorChar.ToString()))
            rbRoot += Path.DirectorySeparatorChar;
        rbRoot = Path.Combine(rbRoot, "$Recycle.Bin");
        SHGetDesktopFolder(out IntPtr ptrDesktop);
        IShellFolder sfDesktop = (IShellFolder)Marshal.GetTypedObjectForIUnknown(ptrDesktop, typeof(IShellFolder));
        IShellFolder sfRecycledBin;
        uint pch = 0;
        SFGAO sfGao = SFGAO.FOLDER;
        sfDesktop.ParseDisplayName(IntPtr.Zero, IntPtr.Zero, rbRoot, ref pch, out IntPtr pidl, ref sfGao);
        Guid isf = typeof(IShellFolder).GUID;
        sfDesktop.BindToObject(pidl, IntPtr.Zero, ref isf, out IntPtr ptrRecycledBin);
        sfRecycledBin = (IShellFolder)Marshal.GetTypedObjectForIUnknown(ptrRecycledBin, typeof(IShellFolder));
        sfRecycledBin.EnumObjects(IntPtr.Zero, SHCONTF.FOLDERS | SHCONTF.INCLUDEHIDDEN, out IntPtr enumIDList);
        IEnumIDList itemsEnum = (IEnumIDList)Marshal.GetTypedObjectForIUnknown(enumIDList, typeof(IEnumIDList));
        while (itemsEnum.Next(1, out IntPtr pidlItem, out uint fetch) == 0 && fetch == 1)
        {
            ShFileInfo fi = new();
            SHGetFileInfo(pidlItem, FILE_ATTRIBUTE.DIRECTORY, ref fi, (uint)Marshal.SizeOf(fi), SHGFI.PIDL | SHGFI.DisplayName);
            if (fi.szDisplayName == localizedRecycleName)
            {
                ret = sfRecycledBin.GetDisplayNameOf(pidlItem, SHGDN.FORPARSING);
                break;
            }
        }
        Marshal.Release(pidl);
        Marshal.Release(enumIDList);
        Marshal.Release(ptrRecycledBin);
        Marshal.Release(ptrDesktop);
        Marshal.ReleaseComObject(sfDesktop);
        Marshal.ReleaseComObject(sfRecycledBin);
        Marshal.ReleaseComObject(itemsEnum);
        return ret;
    }

    /// <summary>
    /// List all available columns for a folder, with the localized name
    /// </summary>
    /// <param name="shellFolder">IShellFolder2 of folder where we want the list of columns</param>
    public static Dictionary<uint, string> ListColumns(this IShellFolder2 shellFolder)
    {
        Dictionary<uint, string> listColumns = [];
        uint i = 0;
        while (shellFolder.GetDetailsOf(IntPtr.Zero, i, out ShellDetails sdcol) == 0)
        {
            string columnName = Marshal.PtrToStringUni(sdcol.str.OleStr);
            sdcol.str.Free();
            listColumns.Add(i, columnName);
            i++;
        }
        return listColumns;
    }

    /// <summary>
    /// Return all localized values for all availables columns in folder, for one item by it's PIDL
    /// </summary>
    /// <param name="shellFolder">IShellFolder2 of folder where we want the list of columns</param>
    /// <param name="pidlItem">PIDL of item where we want the container of all availables columns</param>
    public static Dictionary<uint, string> ListValuesOfItem(this IShellFolder2 shellFolder, IntPtr pidlItem)
    {
        Dictionary<uint, string> listColumns = shellFolder.ListColumns();
        Dictionary<uint, string> result = [];
        foreach (uint numCol in listColumns.Keys)
        {
            shellFolder.GetDetailsOf(pidlItem, numCol, out ShellDetails r);
            string ret = Marshal.PtrToStringUni(r.str.OleStr);
            r.str.Free();
            result.Add(numCol, ret);
        }
        return result;
    }

    public static byte NumberOfMultiply(string currentSize)
    {
        if (currentSize.Contains(Localization.LOCALIZED_KILO))
            return 1;
        if (currentSize.Contains(Localization.LOCALIZED_MEGA))
            return 2;
        if (currentSize.Contains(Localization.LOCALIZED_GIGA))
            return 3;
        if (currentSize.Contains(Localization.LOCALIZED_TERA))
            return 4;
        if (currentSize.Contains(Localization.LOCALIZED_PETA))
            return 5;
        if (currentSize.Contains(Localization.LOCALIZED_EXA))
            return 6;
        return 0;
    }

    public static string GetDisplayNameOf(this IShellFolder sf, IntPtr pidl, SHGDN flags)
    {
        IntPtr ptrName = IntPtr.Zero;
        string ret = "";
        try
        {
            ptrName = Marshal.AllocCoTaskMem(ShellHelper.MAX_PATH * 2 + 4);
            Marshal.WriteInt32(ptrName, 0, 0);
            sf.GetDisplayNameOf(pidl, flags, ptrName);
            StringBuilder sb = new(ShellHelper.MAX_PATH);
            StrRetToBuf(ptrName, pidl, sb, ShellHelper.MAX_PATH);
            ret = sb.ToString();
        }
        catch (Exception) { /* Ignore errors */ }
        finally
        {
            if (ptrName != IntPtr.Zero)
                Marshal.FreeCoTaskMem(ptrName);
        }
        return ret;
    }
}
