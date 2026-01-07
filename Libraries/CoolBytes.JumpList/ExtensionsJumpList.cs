using System;
using System.IO;

using CoolBytes.JumpList.Automatic;
using CoolBytes.JumpList.Custom;

namespace CoolBytes.JumpList;

public static class ExtensionsJumpList
{
    public static AutomaticDestination? GetAutomaticJumpList(string fullPath)
    {
        string pathAutomatic;
        pathAutomatic = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Microsoft", "Windows", "Recent", "AutomaticDestinations");
        foreach (string file in Directory.GetFiles(pathAutomatic, "*.automaticDestinations-ms"))
        {
            try
            {
                AutomaticDestination ad = new(file);
                if (ad?.DestListEntries?.Count > 0 && ad.DestListEntries[0].Lnk != null &&
                    (string.IsNullOrWhiteSpace(ad.DestListEntries[0].Lnk!.Target) ? Environment.ExpandEnvironmentVariables(@"%windir%\explorer.exe") : ad.DestListEntries[0].Lnk!.Target).ToLower() == fullPath.ToLower())
                    return ad;
            }
            catch (Exception) { /* Ignore errors */ }
        }
        return null;
    }

    public static CustomDestination? GetCustomJumpList(string fullPath)
    {
        string pathCustom;
        pathCustom = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Microsoft", "Windows", "Recent", "CustomDestinations");
        foreach (string file in Directory.GetFiles(pathCustom, "*.customDestinations-ms"))
        {
            try
            {
                CustomDestination cd = new(file);
                if (cd?.Entries?.Count > 0 && cd.Entries[0].LnkFiles?.Count > 0 &&
                    (string.IsNullOrWhiteSpace(cd.Entries[0].LnkFiles[0].Target) ? Environment.ExpandEnvironmentVariables(@"%windir%\explorer.exe") : cd.Entries[0].LnkFiles[0].Target).ToLower() == fullPath.ToLower())
                    return cd;
            }
            catch (Exception) { /* Ignore errors */ }
        }
        return null;
    }
}
