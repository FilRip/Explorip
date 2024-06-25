using System;

using Microsoft.Win32;

namespace ExploripConfig.Helpers;

public static class ExtensionsRegistry
{
    public static bool ReadBoolean(this RegistryKey registryKey, string keyName, bool defaultValue = false)
    {
        if (bool.TryParse(registryKey.GetValue(keyName, defaultValue.ToString()).ToString(), out bool result))
            return result;
        return defaultValue;
    }

    public static int ReadInteger(this RegistryKey registryKey, string keyName, int defaultValue = 0)
    {
        if (int.TryParse(registryKey.GetValue(keyName, defaultValue.ToString()).ToString(), out int result))
            return result;
        return defaultValue;
    }

    public static T ReadEnum<T>(this RegistryKey registryKey, string keyName) where T : struct, Enum
    {
        if (Enum.TryParse(registryKey.GetValue(keyName, default(T)).ToString(), out T result))
            return result;
        return default;
    }
}
