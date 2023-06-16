using Microsoft.Win32;

namespace Explorip.Helpers
{
    internal static class ShellContextMenuHelper
    {
        internal static void AddSelectAll()
        {
            RegistryKey key;
            key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Classes\*\shell\Select", true);
            WriteValues(key);
            key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Classes\Folder\shell\Select", true);
            WriteValues(key);
            key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Classes\Directory\Background\shell\Select", true);
            WriteValues(key, true);
            key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Classes\LibraryFolder\Background\shell\Select", true);
            WriteValues(key);

            static void WriteValues(RegistryKey currentKey, bool onlySelectAll = false)
            {
                currentKey.SetValue("MUIVerb", "Select");
                currentKey.SetValue("icon", "imageres.dll,-5308");
                currentKey.SetValue("SubCommands", "Windows.selectall" + (onlySelectAll ? "" : ";Windows.selectnone;Windows.invertselection"));
            }
        }

        internal static void RemoveSelectAll()
        {
            Registry.CurrentUser.DeleteSubKeyTree(@"SOFTWARE\Classes\*\shell\Select");
            Registry.CurrentUser.DeleteSubKeyTree(@"SOFTWARE\Classes\Folder\shell\Select");
            Registry.CurrentUser.DeleteSubKeyTree(@"SOFTWARE\Classes\Directory\Background\shell\Select");
            Registry.CurrentUser.DeleteSubKeyTree(@"SOFTWARE\Classes\LibraryFolder\Background\shell\Select");
        }
    }
}
