using System;

using Microsoft.Win32;

namespace Explorip.Constants;

internal static class RegistrySettings
{
    private static bool? _showVersion;
    private static string _currentVersion;

    public static bool ShowVersion()
    {
        if (!_showVersion.HasValue)
        {
            _showVersion = false;
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Control Panel/Desktop", false);
            if (key != null)
            {
                object value = key.GetValue("PaintDesktopVersion");
                if (value != null)
                    _showVersion = (value.ToString() == "1");
            }
        }
        return _showVersion.Value;
    }

    public static string CurrentVersion()
    {
        if (string.IsNullOrWhiteSpace(_currentVersion))
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software/Microsoft/Windows NT/CurrentVersion");
            if (key != null)
            {
                object name = key.GetValue("ProductName");
                object build = key.GetValue("BuildLab");
                _currentVersion = name + Environment.NewLine + build;
            }
        }
        return _currentVersion;
    }
}
