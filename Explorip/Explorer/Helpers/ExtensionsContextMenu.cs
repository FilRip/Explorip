using System.Collections.Generic;

using Microsoft.Win32;

namespace Explorip.Explorer.Helpers;

public class ShellContextMenuEntry
{
    public string Source { get; set; }
    public string KeyPath { get; set; }
    public string Name { get; set; }
    public string ExplorerCommandHandler { get; set; }
    public string Command { get; set; }
}

public static class ExtensionsContextMenu
{
    private static readonly string[] ShellPaths =
    [
        @"*\shell",
        @"AllFileSystemObjects\shell",
        @"Directory\shell",
        @"Directory\Background\shell",
        @"Drive\shell",
        @"SystemFileAssociations"
    ];

    private static readonly string[] ContextMenuPaths =
    [
        @"*\shellex\ContextMenuHandlers",
        @"AllFileSystemObjects\shellex\ContextMenuHandlers",
        @"Directory\shellex\ContextMenuHandlers",
        @"Directory\Background\shellex\ContextMenuHandlers",
        @"Drive\shellex\ContextMenuHandlers"
    ];

    public static List<ShellContextMenuEntry> GetAllCommands()
    {
        List<ShellContextMenuEntry> results = [];

        // Order classic shell command
        foreach (string path in ShellPaths)
            ScanShellContextMenu(Registry.ClassesRoot, path, results, "ShellVerb");

        // CommandStore shell
        ScanShellContextMenu(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell", results, "CommandStore");

        // ContextMenuHandlers shell
        foreach (string path in ContextMenuPaths)
            ScanShellContextMenu(Registry.ClassesRoot, path, results, "ContextMenuHandler");

        return results;
    }

    private static void ScanShellContextMenu(RegistryKey root, string subPath, List<ShellContextMenuEntry> results, string source)
    {
        using RegistryKey key = root.OpenSubKey(subPath);
        if (key == null)
            return;

        foreach (string verb in key.GetSubKeyNames())
        {
            using RegistryKey handlerKey = key.OpenSubKey(verb);
            if (handlerKey == null)
                continue;

            ShellContextMenuEntry info = new()
            {
                Source = source,
                KeyPath = $"{root.Name}\\{subPath}\\{verb}",
                Name = verb,
                ExplorerCommandHandler = handlerKey.GetValue("ExplorerCommandHandler") as string,
                Command = handlerKey.OpenSubKey("command")?.GetValue("") as string,
            };

            results.Add(info);
        }
    }
}
