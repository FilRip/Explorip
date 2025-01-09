using System;
using System.IO;

using Microsoft.Win32;

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
                if (SHGetFileInfo(Pidl, FILE_ATTRIBUTE.NULL, ref info, (uint)System.Runtime.InteropServices.Marshal.SizeOf(info), SHGFI.TypeName | SHGFI.PIDL | SHGFI.DisplayName) != IntPtr.Zero)
                {
                    return info.szDisplayName;
                }
            }
            else
            {
                ShFileInfo info = new();
                if (SHGetFileInfo(path, FILE_ATTRIBUTE.NORMAL, ref info, (uint)System.Runtime.InteropServices.Marshal.SizeOf(info), SHGFI.DisplayName) != IntPtr.Zero)
                {
                    return info.szDisplayName;
                }
            }
        }
        catch (Exception) { /* Ignore errors */ }
        return Path.GetFileName(path);
    }

    public static string RealName(this RegistryHive registry)
    {
        return registry switch
        {
            RegistryHive.ClassesRoot => "HKEY_CLASSES_ROOT",
            RegistryHive.CurrentUser => "HKEY_CURRENT_USER",
            RegistryHive.LocalMachine => "HKEY_LOCAL_MACHINE",
            RegistryHive.Users => "HKEY_USERS",
            RegistryHive.PerformanceData => "HKEY_PERFORMANCE_DATA",
            RegistryHive.CurrentConfig => "HKEY_CURRENT_CONFIG",
            RegistryHive.DynData => "HKEY_DYN_DATA",
            _ => "ERROR?",
        };
    }
}
