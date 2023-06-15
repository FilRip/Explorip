using System;
using System.Diagnostics;
using System.IO;

namespace Explorip.Helpers
{
    internal class HookCopyOperations
    {
        private static HookCopyOperations _instance;

        public static HookCopyOperations GetInstance()
        {
            _instance ??= new HookCopyOperations();
            return _instance;
        }

        internal void InstallHook()
        {
            string path = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
            Process.Start(Path.Combine(path, "HookFileOperationsManager.exe"), Process.GetCurrentProcess().Id.ToString());
            if (ExtensionsCommandLineArguments.ArgumentPresent("useowncopier") && Process.GetProcessesByName("exploripcopy").Length == 0)
                Process.Start(Path.Combine(path, "ExploripCopy.exe"), Process.GetCurrentProcess().Id.ToString());
        }
    }
}
