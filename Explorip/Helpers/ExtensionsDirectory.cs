using System;
using System.IO;

using static ManagedShell.Interop.NativeMethods;

namespace Explorip.Helpers;

public static class ExtensionsDirectory
{
    public static bool IsShortcut(this FileSystemInfo repertoireOuFichier)
    {
        // TODO : Recognize symbolic directory shortcut
        if (repertoireOuFichier is FileInfo)
        {
            return (Path.GetExtension(repertoireOuFichier.Name).ToLower().Trim() == ".lnk");
        }
        else
        {
            return (repertoireOuFichier.Attributes.HasFlag(FileAttributes.ReparsePoint));
        }
    }

    public static string FullPath(this Environment.SpecialFolder specialFolder)
    {
        return Environment.GetFolderPath(specialFolder);
    }

    /// <summary>
    /// Return the localized name of special folder (windows) like in C:\User<br/>
    /// For example : downloads, my documents, ...
    /// </summary>
    /// <param name="specialFolder">The special folder what you want to localize</param>
    public static string RealName(this Environment.SpecialFolder specialFolder)
    {
        string path = Environment.GetFolderPath(specialFolder);
        try
        {
            if (specialFolder == Environment.SpecialFolder.MyComputer)
            {
                IntPtr Pidl = IntPtr.Zero;
                SHGetSpecialFolderLocation(IntPtr.Zero, CSIDL.CSIDL_DRIVES, ref Pidl);
                ShFileInfo info = new();
                if (SHGetFileInfo(Pidl, EFileAttributes.NULL, ref info, (uint)System.Runtime.InteropServices.Marshal.SizeOf(info), ShGetFileInfos.TypeName | ShGetFileInfos.PIDL | ShGetFileInfos.DisplayName) != IntPtr.Zero)
                {
                    return info.szDisplayName;
                }
            }
            else
            {
                ShFileInfo info = new();
                if (SHGetFileInfo(path, EFileAttributes.NORMAL, ref info, (uint)System.Runtime.InteropServices.Marshal.SizeOf(info), ShGetFileInfos.DisplayName) != IntPtr.Zero)
                {
                    return info.szDisplayName;
                }
            }
        }
        catch (Exception) { /* Ignore errors */ }
        return Path.GetFileName(path);
    }
}
