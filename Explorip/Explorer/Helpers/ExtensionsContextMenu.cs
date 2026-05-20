using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;

using Explorip.Helpers;

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
    public static List<ShellContextMenuEntry> GetAllCommands(ESourceType sourceType, string fileExtension = "", bool withSendTo = true, bool addDrivesInSendTo = true)
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
            withSendTo = false;
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

        if (withSendTo)
        {
            ExpandSendTo(ref results, addDrivesInSendTo);
        }

        return results;
    }

    private static void ExpandSendTo(ref List<ShellContextMenuEntry> results, bool addDrives)
    {
        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Microsoft", "Windows", "SendTo");
        string label;
        foreach (string file in Directory.GetFiles(path))
        {
            label = Path.GetFileNameWithoutExtension(file);
            ShellContextMenuEntry info = new()
            {
                Source = ETypeCommand.SendTo,
                Name = label,
                Command = file,
                Icon = IconManager.GetIconFromFile(file, 0, false),
            };
            results.Add(info);
        }
        if (addDrives)
        {
            foreach (DriveInfo di in DriveInfo.GetDrives())
            {
                label = $"{di.VolumeLabel} ({di.Name})";
                ShellContextMenuEntry info = new()
                {
                    Source = ETypeCommand.SendTo,
                    Name = label,
                    Command = di.Name,
                    Icon = IconManager.GetIconFromFile(di.Name, 0, false),
                };
                results.Add(info);
            }
        }
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
            if (handlerKey.GetSubKeyNames().Contains("icon") && !string.IsNullOrWhiteSpace(handlerKey.GetValue("icon", "").ToString()))
            {
                iconPath = handlerKey.GetValue("icon", "").ToString();
                int index = 0;
                if (iconPath.Contains(','))
                {
                    string[] splitter = iconPath.Split(',');
                    string indexStr = splitter[splitter.Length - 1];
                    if (int.TryParse(indexStr, out index))
                        iconPath = iconPath.Substring(0, iconPath.Length - indexStr.Length);
                }
                info.Icon = IconManager.GetIconFromFile(iconPath, index, false);
            }
            results.Add(info);
        }
    }
}
