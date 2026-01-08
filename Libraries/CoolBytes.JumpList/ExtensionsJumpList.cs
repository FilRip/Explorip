using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

using CoolBytes.JumpList.Automatic;
using CoolBytes.JumpList.Custom;

using ManagedShell.Common.Helpers;

namespace CoolBytes.JumpList;

public static class ExtensionsJumpList
{
    private readonly static List<AutomaticDestination> _listAutoDest = [];
    private readonly static List<CustomDestination> _listCustomDest = [];
    private static FileSystemWatcher? _watchAuto;
    private static FileSystemWatcher? _watchCustom;
    private static Thread? _threadAuto;
    private static Thread? _threadCustom;

    public static void Init()
    {
        string pathAutomatic;
        pathAutomatic = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Microsoft", "Windows", "Recent", "AutomaticDestinations");
        _threadAuto = new(() =>
        {
            foreach (string file in Directory.GetFiles(pathAutomatic, "*.automaticDestinations-ms"))
            {
                try
                {
                    AutomaticDestination ad = new(file);
                    _listAutoDest.Add(ad);
                }
                catch (Exception) { /* Ignore errors */ }
            }
        });
        _threadAuto.Start();
        _watchAuto = new FileSystemWatcher(pathAutomatic);
        _watchAuto.Changed += WatchAuto_Changed;
        _watchAuto.Created += WatchAuto_Created;
        _watchAuto.Deleted += WatchAuto_Deleted;
        _watchAuto.Renamed += WatchAuto_Renamed;

        string pathCustom;
        pathCustom = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Microsoft", "Windows", "Recent", "CustomDestinations");
        _threadCustom = new(() =>
        {
            foreach (string file in Directory.GetFiles(pathCustom, "*.customDestinations-ms"))
            {
                try
                {
                    if (new FileInfo(file).Length <= 24)
                        continue;
                    CustomDestination cd = new(file);
                    _listCustomDest.Add(cd);
                }
                catch (Exception) { /* Ignore errors */ }
            }
        });
        _threadCustom.Start();
        _watchCustom = new FileSystemWatcher(pathCustom);
        _watchCustom.Changed += WatchCustom_Changed;
        _watchCustom.Created += WatchCustom_Created;
        _watchCustom.Deleted += WatchCustom_Deleted;
        _watchCustom.Renamed += WatchCustom_Renamed;
    }

    #region Monitoring AutomaticDestinations folder

    private static void WatchAuto_Renamed(object sender, RenamedEventArgs e)
    {
#pragma warning disable IDE0079
#pragma warning disable S1121
        _listAutoDest.SingleOrDefault(auto => auto.SourceFile == e.OldFullPath)?.SourceFile = e.FullPath;
#pragma warning restore S1121
#pragma warning restore IDE0079
    }

    private static void WatchAuto_Deleted(object sender, FileSystemEventArgs e)
    {
        _listAutoDest.RemoveAll(auto => auto.SourceFile == e.FullPath);
    }

    private static void WatchAuto_Created(object sender, FileSystemEventArgs e)
    {
        try
        {
            AutomaticDestination ad = new(e.FullPath);
            _listAutoDest.Add(ad);
        }
        catch (Exception) { /* Ignore errors */ }
    }

    private static void WatchAuto_Changed(object sender, FileSystemEventArgs e)
    {
        _listAutoDest.RemoveAll(auto => auto.SourceFile == e.FullPath);
        try
        {
            AutomaticDestination ad = new(e.FullPath);
            _listAutoDest.Add(ad);
        }
        catch (Exception) { /* Ignore errors */ }
    }

    #endregion

    #region Monitoring CustomDestinations foldzer

    private static void WatchCustom_Renamed(object sender, RenamedEventArgs e)
    {
#pragma warning disable IDE0079
#pragma warning disable S1121
        _listCustomDest.SingleOrDefault(custom => custom.SourceFile == e.OldFullPath)?.SourceFile = e.FullPath;
#pragma warning restore S1121
#pragma warning restore IDE0079
    }

    private static void WatchCustom_Deleted(object sender, FileSystemEventArgs e)
    {
        _listCustomDest.RemoveAll(custom => custom.SourceFile == e.FullPath);
    }

    private static void WatchCustom_Created(object sender, FileSystemEventArgs e)
    {
        try
        {
            CustomDestination cd = new(e.FullPath);
            _listCustomDest.Add(cd);
        }
        catch (Exception) { /* Ignore errors */ }
    }

    private static void WatchCustom_Changed(object sender, FileSystemEventArgs e)
    {
        _listCustomDest.RemoveAll(custom => custom.SourceFile == e.FullPath);
        try
        {
            CustomDestination cd = new(e.FullPath);
            _listCustomDest.Add(cd);
        }
        catch (Exception) { /* Ignore errors */ }
    }

    #endregion

    public static void Stop()
    {
        _threadAuto?.Abort();
        _threadCustom?.Abort();
        _listAutoDest?.Clear();
        _listCustomDest?.Clear();
        _watchAuto?.Dispose();
        _watchCustom?.Dispose();
    }

    public static AutomaticDestination? GetAutomaticJumpList(IntPtr hWnd)
    {
        try
        {
            string path = ShellHelper.GetPathForHandle(hWnd);
            foreach (AutomaticDestination ad in _listAutoDest)
                if (ad.DestListEntries?.Count > 0 && ad.DestListEntries[0].Lnk?.Target == path)
                    return ad;
        }
        catch (Exception) { /* Ignore errors */ }
        return null;
    }

    public static CustomDestination? GetCustomJumpList(IntPtr hWnd)
    {
        try
        {
            string path = ShellHelper.GetPathForHandle(hWnd);
            foreach (CustomDestination cd in _listCustomDest)
                if (cd.Entries?.Count > 0 && cd.Entries[0].LnkFiles?.Count > 0 && cd.Entries[0].LnkFiles[0].Target == path)
                    return cd;
        }
        catch (Exception) { /* Ignore errors */ }
        return null;
    }
}
