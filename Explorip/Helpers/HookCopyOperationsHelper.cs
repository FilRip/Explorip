using System;
using System.Diagnostics;
using System.IO;

using ExploripConfig.Configuration;

namespace Explorip.Helpers;

internal static class HookCopyOperationsHelper
{
    internal static void InstallHook()
    {
        string path = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
        Process.Start(Path.Combine(path, "HookFileOperationsManager.exe"), Process.GetCurrentProcess().Id.ToString());
        if (ConfigManager.UseOwnCopier)
        {
            if (Process.GetProcessesByName("exploripcopy").Length == 0)
                Process.Start(Path.Combine(path, "ExploripCopy.exe"), Process.GetCurrentProcess().Id.ToString());
            if (ExploripCopyConfig.InjectWindowsExplorer && Process.GetProcessesByName("explorer").Length > 0)
                Process.Start(Path.Combine(path, "HookFileOperationsManager.exe"), Process.GetProcessesByName("explorer")[0].Id.ToString());
        }
    }
}
