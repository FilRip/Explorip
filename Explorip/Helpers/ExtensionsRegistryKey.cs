using Microsoft.Win32;

namespace Explorip.Helpers;

public static class ExtensionsRegistryKey
{
    public static string RealName(this RegistryHive registry)
    {
        return registry switch
        {
            RegistryHive.ClassesRoot => "HKEY_CLASSES_ROOT",
            RegistryHive.CurrentUser => "HKEY_CURRENT_USER",
            RegistryHive.LocalMachine => "HKEY_LOCAL_MACHINE",
            RegistryHive.Users => "HKEY_USERS",
            RegistryHive.PerformanceData => "HKEY_PERFORMANCE_DATA",
            RegistryHive.CurrentConfig => "HKEY_CURRENT_CONFIG",
            RegistryHive.DynData => "HKEY_DYN_DATA",
            _ => "ERROR?",
        };
    }

    public static void CopySubKey(this RegistryKey parentKey, string keyToCopy, string keyNameDestination)
    {
        RegistryKey destinationKey = parentKey.CreateSubKey(keyNameDestination);
        RegistryKey sourceKey = parentKey.OpenSubKey(keyToCopy);
        RecurseCopyKey(sourceKey, destinationKey);
    }

    public static void RenameSubKey(this RegistryKey parentKey, string keyToRename, string newKeyName)
    {
        CopySubKey(parentKey, keyToRename, newKeyName);
        parentKey.DeleteSubKeyTree(keyToRename);
    }

    public static void RecurseCopyKey(RegistryKey sourceKey, RegistryKey destinationKey)
    {
        foreach (string valueName in sourceKey.GetValueNames())
        {
            object objValue = sourceKey.GetValue(valueName);
            RegistryValueKind valKind = sourceKey.GetValueKind(valueName);
            destinationKey.SetValue(valueName, objValue, valKind);
        }
        foreach (string sourceSubKeyName in sourceKey.GetSubKeyNames())
        {
            RegistryKey sourceSubKey = sourceKey.OpenSubKey(sourceSubKeyName);
            RegistryKey destSubKey = destinationKey.CreateSubKey(sourceSubKeyName);
            RecurseCopyKey(sourceSubKey, destSubKey);
        }
    }
}
