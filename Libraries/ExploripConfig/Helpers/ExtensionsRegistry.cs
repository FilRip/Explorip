using System;
using System.Windows;

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

    public static Point ReadPoint(this RegistryKey registryKey, string keyName)
    {
        try
        {
            Point result = Point.Parse(registryKey.GetValue(keyName).ToString());
            return result;
        }
        catch (Exception) { /* Ignore errors */ }
        return default;
    }

    public static Rect ReadRectangle(this RegistryKey registryKey, string keyName)
    {
        try
        {
            Rect result = Rect.Parse(registryKey.GetValue(keyName).ToString());
            return result;
        }
        catch (Exception) { /* Ignore errors */ }
        return default;
    }
}
