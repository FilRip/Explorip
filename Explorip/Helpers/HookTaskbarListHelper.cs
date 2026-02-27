using System;
using System.Runtime.InteropServices;

using ManagedShell.Interop;

using Microsoft.Win32;

namespace Explorip.Helpers;

internal static class HookTaskbarListHelper
{
    private static uint regPtr = 0;
    private static TaskbarList.TaskbarList _instance;

    public static void InstallHook()
    {
        if (!Program.ModeShell)
            return;
        RegistryKey original = Registry.ClassesRoot.OpenSubKey("CLSID\\{" + typeof(TaskbarList.TaskbarList).GUID.ToString() + "}", false);
        RegistryKey userKey = Registry.CurrentUser.CreateSubKey("Software\\Classes\\CLSID\\{" + typeof(TaskbarList.TaskbarList).GUID.ToString() + "}", true);
        userKey.SetValue("", original.GetValue(""));
        original.Close();
        userKey.CreateSubKey("LocalServer32").SetValue("", Environment.GetCommandLineArgs()[0]);
        userKey.Close();

        Guid regGuid = typeof(TaskbarList.TaskbarList).GUID;
        _instance = new TaskbarList.TaskbarList();
        NativeMethods.CoRegisterClassObject(ref regGuid, _instance, NativeMethods.ClassesContexts.LOCAL_SERVER, NativeMethods.RegistryClasses.REGCLS_MULTIPLEUSE, out regPtr);
    }

    public static void UninstallHook()
    {
        if (regPtr > 0)
        {
            NativeMethods.CoRevokeClassObject(regPtr);
            Registry.CurrentUser.DeleteSubKey("Software\\Classes\\CLSID\\{" + typeof(TaskbarList.TaskbarList).GUID.ToString() + "}", false);
        }
    }

    internal static TaskbarList.TaskbarList TaskbarListImplementation
    {
        get { return _instance; }
    }

    internal static void RegisterAsShellForCurrentUser()
    {
        Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon", true).SetValue("Shell", Environment.GetCommandLineArgs()[0]);
    }

    internal static void UnregisterAsShellForCurrentUser()
    {
        Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon", true).DeleteValue("Shell");
    }
}
