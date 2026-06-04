using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using CommunityToolkit.Mvvm.ComponentModel;

using Explorip.Helpers;

using ExploripConfig.Configuration;

using ManagedShell.Common.Helpers;
using ManagedShell.Interop;
using ManagedShell.ShellFolders.Interfaces;

using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Shell.Common;

namespace Explorip.Explorer.Helpers;

public enum ESourceType
{
    File = 0,
    Folder = 1,
    MyPC = 3,
    Drive = 4,
    MultipleFiles = 5,
    MultipleFolders = 6,
    RecycleBin = 7,
    Background = 8,
}

public enum ETypeCommand
{
    ShellVerb = 0,
    ContextMenuHandler = 1,
    CommandStore = 2,
    //SendTo = 3,
    CreateShortcut = 4,
    Rename = 5,
    Share = 6,
    New = 7,
}

public partial class ShellContextMenuEntry : ObservableObject, ICloneable, IDisposable
{
    private bool disposedValue;
    private IntPtr _menuHandle;

    [ObservableProperty()]
    private ETypeCommand _source;
    [ObservableProperty()]
    private string _keyPath;
    [ObservableProperty()]
    private string _name;
    [ObservableProperty()]
    private string _explorerCommandHandler;
    [ObservableProperty()]
    private string _command;
    [ObservableProperty()]
    private ImageSource _icon;
    [ObservableProperty()]
    private string _toolTip;
    [ObservableProperty()]
    private ObservableCollection<ShellContextMenuEntry> _subitems = [];

    public IContextMenu ContextMenu { get; set; }
    public uint NumCmd { get; set; }

    public object Clone()
    {
        return new ShellContextMenuEntry()
        {
            Source = Source,
            KeyPath = KeyPath,
            Name = Name,
            ExplorerCommandHandler = ExplorerCommandHandler,
            Command = Command,
            Icon = Icon,
            ContextMenu = ContextMenu,
        };
    }

    public void ExpandIt(ref List<ShellContextMenuEntry> list, ShellFolder parentFolder, ShellObject[] selectedItems)
    {
        bool toRemove = false;
        if (Guid.TryParse(Command, out Guid guid))
        {
            IContextMenu exCommand = null;
            IntPtr hMenu = IntPtr.Zero;
            IntPtr ptrData = IntPtr.Zero;

            try
            {
                Type t = Type.GetTypeFromCLSID(guid);
                exCommand = Activator.CreateInstance(t) as IContextMenu;
                if (exCommand != null)
                {
                    ContextMenu = exCommand;
                    List<IntPtr> listPidlRelative = [];
                    foreach (ShellObject so in selectedItems)
                        listPidlRelative.Add(NativeMethods.ILFindLastID(so.PIDL));
                    Guid guidIDataObject = new("{0000010e-0000-0000-C000-000000000046}");
                    NativeMethods.SHCreateDataObject(parentFolder.PIDL, (uint)selectedItems.Length, [.. listPidlRelative], IntPtr.Zero, guidIDataObject, out ptrData);
                    if (ptrData != IntPtr.Zero)
                    {
                        ((IShellExtInit)exCommand).Initialize(IntPtr.Zero, ptrData, 0);
                        hMenu = NativeMethods.CreatePopupMenu();
                        exCommand.QueryContextMenu(hMenu, 0, 1, int.MaxValue, ManagedShell.ShellFolders.Enums.ContextMenuStates.NORMAL);
                        _menuHandle = hMenu;
                        int count = NativeMethods.GetMenuItemCount(hMenu);
                        if (count > 0)
                        {
                            int index = 0;
                            if (count > 1)
                                index = list.IndexOf(this);
                            for (uint i = 0; i < count; i++)
                            {
                                NativeMethods.MenuItemInfo menuItemInfo = new()
                                {
                                    fMask = NativeMethods.MenuItemIntegrateMembers.STATE | NativeMethods.MenuItemIntegrateMembers.STRING | NativeMethods.MenuItemIntegrateMembers.ID | NativeMethods.MenuItemIntegrateMembers.SUBMENU | NativeMethods.MenuItemIntegrateMembers.BITMAP,
                                    dwTypeData = new string((char)0, 256),
                                    cch = 255,
                                    fType = NativeMethods.MenuItemTypes.DISABLED | NativeMethods.MenuItemTypes.GRAYED | NativeMethods.MenuItemTypes.STRING,
                                };
                                if (NativeMethods.GetMenuItemInfo(hMenu, i, true, ref menuItemInfo))
                                {
                                    if (string.IsNullOrWhiteSpace(menuItemInfo.dwTypeData.Trim()))
                                    {
                                        if (i == 0)
                                            toRemove = true;
                                    }
                                    else if (i == 0)
                                    {
                                        Name = menuItemInfo.dwTypeData.Trim();
                                        NumCmd = menuItemInfo.wID;
                                        Icon = ExtensionsContextMenu.GetDefaultIcon(Command);
                                        Icon?.Freeze();
                                        if (Icon == null && menuItemInfo.hbmpItem != IntPtr.Zero)
                                            Icon = ((BitmapSource)IconImageConverter.GetImageFromHBitmap(menuItemInfo.hbmpItem, false)).MakeTransparentColor(ConfigManager.ExplorerContextMenuBackground?.Color ?? ExploripSharedCopy.Constants.Colors.BackgroundColor);
                                        if (menuItemInfo.hSubMenu != IntPtr.Zero)
                                            ExpandContextMenuSubItems(menuItemInfo.hSubMenu, this);
                                    }
                                    else
                                    {
                                        index++;
                                        ShellContextMenuEntry newEntry = (ShellContextMenuEntry)Clone();
                                        newEntry.Name = menuItemInfo.dwTypeData.Trim();
                                        newEntry.NumCmd = menuItemInfo.wID;
                                        newEntry.Icon = ExtensionsContextMenu.GetDefaultIcon(Command);
                                        newEntry.Icon?.Freeze();
                                        if (newEntry.Icon == null && menuItemInfo.hbmpItem != IntPtr.Zero)
                                            newEntry.Icon = ((BitmapSource)IconImageConverter.GetImageFromHBitmap(menuItemInfo.hbmpItem, false)).MakeTransparentColor(ConfigManager.ExplorerContextMenuBackground?.Color ?? ExploripSharedCopy.Constants.Colors.BackgroundColor);
                                        if (menuItemInfo.hSubMenu != IntPtr.Zero)
                                            ExpandContextMenuSubItems(menuItemInfo.hSubMenu, newEntry);
                                        list.Insert(index, newEntry);
                                    }
                                }
                                else
                                    toRemove = true;
                            }
                        }
                        else
                            toRemove = true;
                    }
                }
            }
            catch (Exception) { /* Errors */ }
            finally
            {
                if (toRemove)
                    list.Remove(this);
                if (ptrData != IntPtr.Zero)
                    Marshal.FreeCoTaskMem(ptrData);
            }
        }
    }

    private void ExpandContextMenuSubItems(IntPtr hMenu, ShellContextMenuEntry parent)
    {
        int count = NativeMethods.GetMenuItemCount(hMenu);
        if (count > 0)
        {
            for (uint i = 0; i < count; i++)
            {
                NativeMethods.MenuItemInfo menuItemInfo = new()
                {
                    fMask = NativeMethods.MenuItemIntegrateMembers.STATE | NativeMethods.MenuItemIntegrateMembers.STRING | NativeMethods.MenuItemIntegrateMembers.ID | NativeMethods.MenuItemIntegrateMembers.SUBMENU | NativeMethods.MenuItemIntegrateMembers.BITMAP,
                    dwTypeData = new string((char)0, 256),
                    cch = 255,
                    fType = NativeMethods.MenuItemTypes.DISABLED | NativeMethods.MenuItemTypes.GRAYED | NativeMethods.MenuItemTypes.STRING,
                };
                if (NativeMethods.GetMenuItemInfo(hMenu, i, true, ref menuItemInfo))
                {
                    if (string.IsNullOrWhiteSpace(menuItemInfo.dwTypeData.Trim()))
                        continue;
                    ShellContextMenuEntry newEntry;
                    if (i == 0)
                        newEntry = (ShellContextMenuEntry)parent?.Clone();
                    else
                        newEntry = (ShellContextMenuEntry)Clone();
                    newEntry.Name = menuItemInfo.dwTypeData.Trim();
                    newEntry.NumCmd = menuItemInfo.wID;
                    newEntry.Icon = ExtensionsContextMenu.GetDefaultIcon(Command);
                    newEntry.Icon?.Freeze();
                    if (newEntry.Icon == null && menuItemInfo.hbmpItem != IntPtr.Zero)
                        newEntry.Icon = ((BitmapSource)IconImageConverter.GetImageFromHBitmap(menuItemInfo.hbmpItem, false)).MakeTransparentColor(ConfigManager.ExplorerContextMenuBackground?.Color ?? ExploripSharedCopy.Constants.Colors.BackgroundColor);
                    parent.Subitems.Add(newEntry);
                    if (menuItemInfo.hSubMenu != IntPtr.Zero)
                        ExpandContextMenuSubItems(menuItemInfo.hSubMenu, newEntry);
                }
            }
        }
    }

    public bool IsDisposed
    {
        get { return disposedValue; }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                if (Subitems?.Count > 0)
                    for (int i = Subitems.Count - 1; i >= 0; i--)
                        Subitems[i].Dispose();
                try
                {
                    if (ContextMenu != null)
                    {
                        Marshal.ReleaseComObject(ContextMenu);
                        ContextMenu = null;
                    }
                    if (_menuHandle != IntPtr.Zero)
                    {
                        NativeMethods.DestroyMenu(_menuHandle);
                        _menuHandle = IntPtr.Zero;
                    }
                }
                catch (Exception) { /* Ignore errors, COM object certainly already released */ }
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}

public static class ExtensionsContextMenu
{
    public static List<ShellContextMenuEntry> GetAllCommands(ESourceType sourceType, string fileExtension = "")
    {
        List<ShellContextMenuEntry> results = [];

        if (sourceType == ESourceType.File)
        {
            ScanShellContextMenuCurrentUser(@"*\shell", ref results, ETypeCommand.ShellVerb);
            if (!string.IsNullOrWhiteSpace(fileExtension))
            {
                ScanShellContextMenuCurrentUser(@$"{fileExtension}\shell", ref results, ETypeCommand.ShellVerb);
                ScanShellContextMenuCurrentUser(@$"SystemFileAssociations\{fileExtension}\shell", ref results, ETypeCommand.ShellVerb);
            }
            ScanShellContextMenuCurrentUser(@"AllFileSystemObjects\shell", ref results, ETypeCommand.ShellVerb);
            ScanShellContextMenuCurrentUser(@"SystemFileAssociations\*\shell", ref results, ETypeCommand.ShellVerb);
            ScanShellContextMenuCurrentUser(@"*\shellex\ContextMenuHandlers", ref results, ETypeCommand.ContextMenuHandler);
            ScanShellContextMenuCurrentUser(@"AllFileSystemObjects\shellex\ContextMenuHandlers", ref results, ETypeCommand.ContextMenuHandler);
        }
        else if (sourceType == ESourceType.Folder)
        {
            ScanShellContextMenuCurrentUser(@"Directory\shell", ref results, ETypeCommand.ShellVerb);
            ScanShellContextMenuCurrentUser(@"Directory\Background\shell", ref results, ETypeCommand.ShellVerb);
            ScanShellContextMenuCurrentUser(@"Directory\shellex\ContextMenuHandlers", ref results, ETypeCommand.ContextMenuHandler);
            ScanShellContextMenuCurrentUser(@"Directory\Background\shellex\ContextMenuHandlers", ref results, ETypeCommand.ContextMenuHandler);
        }
        else if (sourceType == ESourceType.Drive)
        {
            ScanShellContextMenuCurrentUser(@"Drive\shell", ref results, ETypeCommand.ShellVerb);
            ScanShellContextMenuCurrentUser(@"Drive\shellex\ContextMenuHandlers", ref results, ETypeCommand.ContextMenuHandler);
        }
        else if (sourceType == ESourceType.MultipleFiles)
        {
            ScanShellContextMenuCurrentUser(@"*\shell", ref results, ETypeCommand.ShellVerb);
            ScanShellContextMenuCurrentUser(@"AllFileSystemObjects\shell", ref results, ETypeCommand.ShellVerb);
            ScanShellContextMenuCurrentUser(@"SystemFileAssociations\*\shell", ref results, ETypeCommand.ShellVerb);
            ScanShellContextMenuCurrentUser(@"*\shellex\ContextMenuHandlers", ref results, ETypeCommand.ContextMenuHandler);
            ScanShellContextMenuCurrentUser(@"AllFileSystemObjects\shellex\ContextMenuHandlers", ref results, ETypeCommand.ContextMenuHandler);
        }
        else if (sourceType == ESourceType.MultipleFolders)
        {
            ScanShellContextMenuCurrentUser(@"Directory\shell", ref results, ETypeCommand.ShellVerb);
            ScanShellContextMenuCurrentUser(@"Directory\shellex\ContextMenuHandlers", ref results, ETypeCommand.ContextMenuHandler);
        }

        // CommandStore shell
        //ScanShellContextMenu(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell", ref results, ETypeCommand.CommandStore);

        return results;
    }

    /*public static List<ShellContextMenuEntry> ExpandSendTo(bool addDrives)
    {
        List<ShellContextMenuEntry> results = [];
        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Microsoft", "Windows", "SendTo");
        string label;
        foreach (string file in Directory.GetFiles(path).Where(f => Path.GetFileName(f).ToLower() != "desktop.ini"))
        {
            label = Path.GetFileNameWithoutExtension(file);
            ShellContextMenuEntry info = new()
            {
                Source = ETypeCommand.SendTo,
                Name = label,
                Command = file,
                Icon = IconImageConverter.GetImageFromAssociatedIcon(file.Trim('\"'), ManagedShell.Common.Enums.IconSize.Small),
            };
            info.Icon?.Freeze();
            // Try to get the localized name
            NativeMethods.SHILCreateFromPath(file, out IntPtr pidl);
            if (pidl != IntPtr.Zero)
            {
                NativeMethods.SHCreateShellItem(IntPtr.Zero, IntPtr.Zero, pidl, out IntPtr shellItemPtr);
                if (shellItemPtr != IntPtr.Zero)
                {
                    IShellItem shellItem = (IShellItem)Marshal.GetTypedObjectForIUnknown(shellItemPtr, typeof(IShellItem));
                    shellItem.GetDisplayName(ManagedShell.ShellFolders.Enums.ShellItemGetDisplayName.NORMALDISPLAY, out IntPtr namePtr);
                    if (namePtr != IntPtr.Zero)
                    {
                        info.Name = Marshal.PtrToStringUni(namePtr);
                        Marshal.FreeCoTaskMem(namePtr);
                    }
                    Marshal.ReleaseComObject(shellItem);
                    Marshal.Release(shellItemPtr);
                }
                NativeMethods.ILFree(pidl);
            }
            results.Add(info);
        }
        if (addDrives)
        {
            foreach (DriveInfo di in DriveInfo.GetDrives().Where(d => d.IsReady))
            {
                if (di.DriveType == DriveType.Network)
                {
                    // Get shared name, like explorer.exe do
                    int maxPath = 256;
                    StringBuilder sb = new(maxPath);
                    if (NativeMethods.WNetGetConnection(di.Name.Trim('\\'), sb, ref maxPath) == NativeMethods.S_OK)
                        label = $"{sb.ToString().Trim((char)0).ToUpper()} ({di.Name})";
                    else
                        label = di.Name;
                }
                else
                    label = $"{di.VolumeLabel} ({di.Name})";
                ShellContextMenuEntry info = new()
                {
                    Source = ETypeCommand.SendTo,
                    Name = label,
                    Command = di.Name,
                    Icon = IconImageConverter.GetImageFromAssociatedIcon(di.Name, ManagedShell.Common.Enums.IconSize.Small),
                };
                info.Icon?.Freeze();
                results.Add(info);
            }
        }
        return results;
    }*/

    private static void ScanShellContextMenuCurrentUser(string subPath, ref List<ShellContextMenuEntry> results, ETypeCommand source)
    {
        ScanShellContextMenu(Registry.ClassesRoot, subPath, ref results, source);
        ScanShellContextMenu(Registry.CurrentUser, @$"Software\Classes\{subPath}", ref results, source);
    }

    private static void ScanShellContextMenu(RegistryKey root, string subPath, ref List<ShellContextMenuEntry> results, ETypeCommand source)
    {
        using RegistryKey key = root.OpenSubKey(subPath, false);
        if (key == null)
            return;
        string command, explorerCommand, iconPath;
        foreach (string verb in key.GetSubKeyNames())
        {
            using RegistryKey handlerKey = key.OpenSubKey(verb, false);
            if (handlerKey.GetSubKeyNames().Length == 0 && !string.IsNullOrWhiteSpace(handlerKey.GetValue("", "").ToString()))
            {
                explorerCommand = "";
                command = handlerKey.GetValue("").ToString();
                if (!Guid.TryParse(command, out _))
                    continue;
            }
            else
            {
                command = handlerKey.OpenSubKey("command", false)?.GetValue("", "").ToString();
                explorerCommand = handlerKey.GetValue("ExplorerCommandHandler", "").ToString();
                if (string.IsNullOrWhiteSpace(command) && string.IsNullOrWhiteSpace(explorerCommand))
                    continue;
            }
            ShellContextMenuEntry info = new()
            {
                Source = source,
                KeyPath = $"{root.Name}\\{subPath}\\{verb}",
                Name = verb,
                ExplorerCommandHandler = explorerCommand,
                Command = command.Replace("CLSID\\", ""),
            };
            if (!string.IsNullOrWhiteSpace(handlerKey.GetValue("", "").ToString()))
            {
                info.Name = handlerKey.GetValue("").ToString();
                if (handlerKey.GetValue("").ToString().StartsWith("@"))
                {
                    // Try get resources label
                    string lib = handlerKey.GetValue("").ToString().TrimStart('@');
                    if (lib.Contains("ms-resource://"))
                    {
                        info.Name = Constants.Localization.LoadMsResourceString(handlerKey.GetValue("").ToString(), handlerKey.GetValue("").ToString());
                    }
                    else
                    {
                        int index = 0;
                        if (lib.Contains(','))
                        {
                            string[] splitter = lib.Split(',');
                            string strIndex = splitter[splitter.Length - 1].TrimStart('-');
                            if (int.TryParse(strIndex, out index))
                                lib = lib.Substring(0, lib.Length - (strIndex.Length + 1)).TrimEnd(',');
                        }
                        info.Name = Constants.Localization.Load(lib, (uint)index, handlerKey.GetValue("").ToString());
                    }
                }
            }
            if (!string.IsNullOrWhiteSpace(handlerKey.GetValue("icon", "").ToString()))
            {
                iconPath = handlerKey.GetValue("icon", "").ToString();
                int index = 0;
                if (iconPath.Contains(','))
                {
                    string[] splitter = iconPath.Split(',');
                    string indexStr = splitter[splitter.Length - 1].TrimStart('-');
                    if (int.TryParse(indexStr, out index))
                        iconPath = iconPath.Substring(0, iconPath.Length - (indexStr.Length + 1)).TrimEnd(',');
                }
                info.Icon = IconManager.GetIconFromFile(iconPath.Trim('\"'), index, false);
                info.Icon?.Freeze();
            }
            results.RemoveAll(i => i.Source == info.Source && i.Name == info.Name && (i.Command == info.Command || i.ExplorerCommandHandler == info.ExplorerCommandHandler));
            results.Add(info);
        }
    }

    public static ImageSource GetDefaultIcon(string guid)
    {
        RegistryKey reg;
        reg = Registry.CurrentUser.OpenSubKey($"Software\\Classes\\CLSID\\{guid}");
        reg ??= Registry.ClassesRoot.OpenSubKey($"CLSID\\{guid}");

        if (reg == null)
            return null;

        string filePath;
        int index = 0;

        if (reg.GetSubKeyNames().Contains("DefaultIcon", StringComparer.OrdinalIgnoreCase))
        {
            filePath = reg.OpenSubKey("DefaultIcon").GetValue("", "").ToString();
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                if (filePath.Contains(','))
                {
                    string[] splitter = filePath.Split(',');
                    string strIndex = splitter[splitter.Length - 1].TrimStart('-');
                    if (int.TryParse(strIndex, out index))
                        filePath = filePath.Substring(0, filePath.Length - (strIndex.Length + 1)).TrimEnd(',');
                }
                return IconManager.GetIconFromFile(filePath, index, false);
            }
        }
        if (reg.GetSubKeyNames().Contains("InProcServer32", StringComparer.OrdinalIgnoreCase))
        {
            filePath = reg.OpenSubKey("InProcServer32").GetValue("", "").ToString();
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                return IconManager.GetIconFromFile(filePath, 0, false);
            }
        }

        return null;
    }
}
