using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Media;

using Explorip.Helpers;

using ManagedShell.Interop;
using ManagedShell.ShellFolders.Interfaces;

using Microsoft.Win32;

namespace Explorip.Explorer.Helpers;

public enum ESourceType
{
    File = 0,
    Folder = 1,
    MyPC = 3,
    Drive = 4,
    MultipleFiles = 5,
    MultipleFolders = 6,
}

public enum ETypeCommand
{
    ShellVerb = 0,
    ContextMenuHandler = 1,
    CommandStore = 2,
    SendTo = 3,
    CreateShortcut = 4,
    Rename = 5,
    Share = 6,
    New = 7,
}

public class ShellContextMenuEntry
{
    public ETypeCommand Source { get; set; }
    public string KeyPath { get; set; }
    public string Name { get; set; }
    public string ExplorerCommandHandler { get; set; }
    public string Command { get; set; }
    public ImageSource Icon { get; set; }
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

    public static List<ShellContextMenuEntry> ExpandSendTo(bool addDrives)
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
                Icon = IconManager.GetIconFromFile(file, 0, false),
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
                    Icon = IconManager.GetIconFromFile(di.Name, 0, false),
                };
                info.Icon?.Freeze();
                results.Add(info);
            }
        }
        return results;
    }

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
                Command = command,
            };
            if (!string.IsNullOrWhiteSpace(handlerKey.GetValue("", "").ToString()))
                info.Name = handlerKey.GetValue("").ToString();
            if (!string.IsNullOrWhiteSpace(handlerKey.GetValue("icon", "").ToString()))
            {
                iconPath = handlerKey.GetValue("icon", "").ToString();
                int index = 0;
                if (iconPath.Contains(','))
                {
                    string[] splitter = iconPath.Split(',');
                    string indexStr = splitter[splitter.Length - 1];
                    if (int.TryParse(indexStr, out index))
                        iconPath = iconPath.Substring(0, iconPath.Length - (indexStr.Length + 1));
                }
                info.Icon = IconManager.GetIconFromFile(iconPath, index, false);
                info.Icon?.Freeze();
            }
            results.RemoveAll(i => i.Source == info.Source && i.Name == info.Name && (i.Command == info.Command || i.ExplorerCommandHandler == info.ExplorerCommandHandler));
            results.Add(info);
        }
    }
}
