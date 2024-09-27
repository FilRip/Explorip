using System;
using System.Diagnostics;
using System.IO;

namespace Explorip.Helpers;

internal static class HookCopyOperations
{
    internal static void InstallHook()
    {
        string path = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
        Process.Start(Path.Combine(path, "HookFileOperationsManager.exe"), Process.GetCurrentProcess().Id.ToString());
        if (ExploripConfig.Configuration.ConfigManager.UseOwnCopier && Process.GetProcessesByName("exploripcopy").Length == 0)
            Process.Start(Path.Combine(path, "ExploripCopy.exe"), Process.GetCurrentProcess().Id.ToString());
    }
}
