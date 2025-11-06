using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

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

    public static double ReadDouble(this RegistryKey registryKey, string keyName, double defaultValue = 0)
    {
        if (double.TryParse(registryKey.GetValue(keyName, defaultValue.ToString()).ToString(), NumberStyles.AllowDecimalPoint | NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out double result))
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

    public static Color ReadColor(this RegistryKey registryKey, string keyName, Color defaultColor)
    {
        try
        {
            object value = registryKey.GetValue(keyName);
            if (value is string)
            {
                string[] splitter = value.ToString().Split(',');
                if (splitter.Length == 4)
                    return Color.FromArgb(byte.Parse(splitter[0]), byte.Parse(splitter[1]), byte.Parse(splitter[2]), byte.Parse(splitter[3]));
                else if (splitter.Length == 3)
                    return Color.FromArgb(255, byte.Parse(splitter[0]), byte.Parse(splitter[1]), byte.Parse(splitter[2]));
            }
        }
        catch (Exception) { /* Ignore errors */ }
        return defaultColor;
    }
}
