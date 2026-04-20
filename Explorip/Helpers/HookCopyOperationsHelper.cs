using System.Diagnostics;
using System.IO;

using ExploripConfig.Configuration;

using static ExploripConfig.Helpers.ExtensionsCommandLineArguments;

namespace Explorip.Helpers;

internal static class HookCopyOperationsHelper
{
    internal static void InstallHook()
    {
        string path = ArgumentPathExe();
        string filename = "HookFileOperationsManager.exe";
        if (!File.Exists(Path.Combine(path, filename)))
            path = Path.Combine(path, "lib");
        Process.Start(Path.Combine(path, filename), Process.GetCurrentProcess().Id.ToString());
        if ((ConfigManager.UseOwnCopier || ArgumentExists("useowncopier")) &&
            (Process.GetProcessesByName("exploripcopy").Length == 0))
        {
            Process.Start(Path.Combine(ArgumentPathExe(), "ExploripCopy.exe"), Process.GetCurrentProcess().Id.ToString());
        }
    }
}
