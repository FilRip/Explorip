using System;
using System.IO;

namespace ExploripCopy.Helpers;

public static class ExtensionsDriveInfo
{
#pragma warning disable IDE0060 // Supprimer le paramètre inutilisé
    public static DriveInfo GetDrive(this DriveInfo di, char letter)
#pragma warning restore IDE0060 // Supprimer le paramètre inutilisé
    {
        return Array.Find(DriveInfo.GetDrives(), d => d.RootDirectory.FullName.StartsWith(letter.ToString()));
    }

    public static DriveInfo GetDrive(this DirectoryInfo dir)
    {
        return Array.Find(DriveInfo.GetDrives(), d => d.RootDirectory.FullName.StartsWith(dir.FullName.Substring(0, 1)));
    }
}
