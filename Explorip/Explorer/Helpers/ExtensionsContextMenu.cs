using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Media;

using CoolBytes.Helpers;

using Explorip.Explorer.Helpers.ContextMenu;
using Explorip.Helpers;

using ManagedShell.Common.Helpers;
using ManagedShell.Interop;
using ManagedShell.ShellFolders.Interfaces;

using Microsoft.Win32;

namespace Explorip.Explorer.Helpers;

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

    public static List<ShellContextMenuEntry> ExpandOpenWith(string ext, RegistryKey root = null)
    {
        List<ShellContextMenuEntry> result = [];
        if (string.IsNullOrWhiteSpace(ext) || ext == ".")
            ext = "*";
        if (root == null)
            result.AddRange(ExpandOpenWith(ext, Registry.ClassesRoot));
        root ??= Registry.CurrentUser.OpenSubKey("Software\\Classes");
        RegistryKey reg = root.OpenSubKey(ext);
        if (reg != null && reg.GetSubKeyNames().Contains("OpenWithProgids"))
        {
            reg = reg.OpenSubKey("OpenWithProgids");
            foreach (string app in reg.GetValueNames())
            {
                try
                {
                    RegistryKey appKey = root.OpenSubKey(app) ?? Registry.ClassesRoot.OpenSubKey(app);
                    if (appKey != null)
                    {
                        RegistryKey appDescriptionKey = appKey.OpenSubKey("Application");
                        RegistryKey appDefaultIconKey = appKey.OpenSubKey("DefaultIcon");
                        RegistryKey shellCommand = appKey.OpenSubKey("Shell\\open\\command");
                        if (appDescriptionKey != null)
                        {
                            ShellContextMenuEntry entry = new()
                            {
                                Command = $"shell:AppsFolder\\{appDescriptionKey.GetValue("AppUserModelID", "")}",
                                Source = ETypeCommand.OpenWith,
                                KeyPath = $"{appKey.Name}\\{ext}",
                                Name = Constants.Localization.LoadMsResourceString(appDescriptionKey.GetValue("ApplicationName", "").ToString(), app),
                            };
                            if (shellCommand != null && !string.IsNullOrWhiteSpace(shellCommand.GetValue("", "").ToString()))
                                entry.Command = shellCommand.GetValue("", "").ToString();
                            string uriIcon = null;
                            if (appDescriptionKey.GetValueNames().Contains("ApplicationIcon"))
                                uriIcon = appDescriptionKey.GetValue("ApplicationIcon", "").ToString();
                            else if (appDefaultIconKey != null && !string.IsNullOrWhiteSpace(appDefaultIconKey.GetValue("", "").ToString()))
                                uriIcon = appDefaultIconKey.GetValue("", "").ToString();
                            if (!string.IsNullOrWhiteSpace(uriIcon))
                                entry.Icon = IconManager.GetImageSource(uriIcon);
                            result.Add(entry);
                        }
                    }
                }
                catch (Exception) { /* TODO : Ignore errors ? */ }
            }
        }
        if (root.OpenSubKey(ext)?.GetSubKeyNames()?.Contains("OpenWithList") == true)
        {
            reg = root.OpenSubKey(ext).OpenSubKey("OpenWithList");
            foreach (string key in reg.GetSubKeyNames())
            {
                StringBuilder sb = new(256);
                if (NativeMethods.SearchPath(null, key, null, 256, sb, out _) > 0)
                {
                    FileInfo fi = new(sb.ToString());
                    if (fi.Length == 0)
                        continue;
                    ShellContextMenuEntry entry = new()
                    {
                        Command = key,
                        Source = ETypeCommand.OpenWith,
                        KeyPath = $"{root.Name}\\{reg.Name}\\{key}",
                        Name = Path.GetFileNameWithoutExtension(key),
                        Icon = IconManager.GetIconFromFile(sb.ToString(), 0, false),
                    };
                    entry.Icon?.Freeze();
                    result.Add(entry);
                }
            }
        }
        if (ext != "*")
            result.AddRange(ExpandOpenWith("*", root));
        result = result.Distinct(r => r.Name);
        return result;
    }
}
