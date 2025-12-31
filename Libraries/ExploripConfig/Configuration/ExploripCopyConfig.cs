using ExploripConfig.Helpers;

using Microsoft.Win32;

using static ExploripConfig.Helpers.ExtensionsCommandLineArguments;

namespace ExploripConfig.Configuration;

public static class ExploripCopyConfig
{
    private const string HKeyRoot = "Software\\CoolBytes\\ExploripCopy";
    private const string AutoStartOperationConfig = "AutoStartOperation";
    private const string NotificationOnEachOperationConfig = "NotificationOnEachOperation";
    private const string AutoExpandSubFolderConfig = "AutoExpandSubfolder";

    private static RegistryKey _registryRootExploripCopy;

    public static bool AllowWrite { get; set; }

    public static void Init(bool allowWrite = true)
    {
        AllowWrite = allowWrite && !ArgumentExists("disablewriteconfig");
        _registryRootExploripCopy = Registry.CurrentUser.CreateSubKey(HKeyRoot, true);
        if (_registryRootExploripCopy == null)
            AllowWrite = false;
        if (AllowWrite && _registryRootExploripCopy != null)
        {
            if (string.IsNullOrWhiteSpace(_registryRootExploripCopy.GetValue(AutoStartOperationConfig, "").ToString()))
                _registryRootExploripCopy.SetValue(AutoStartOperationConfig, "True");
            if (string.IsNullOrWhiteSpace(_registryRootExploripCopy.GetValue(NotificationOnEachOperationConfig, "").ToString()))
                _registryRootExploripCopy.SetValue(NotificationOnEachOperationConfig, "False");
            if (string.IsNullOrWhiteSpace(_registryRootExploripCopy.GetValue(AutoExpandSubFolderConfig, "").ToString()))
                _registryRootExploripCopy.SetValue(AutoExpandSubFolderConfig, "False");
        }
    }

    public static bool AutoStartOperation
    {
        get { return _registryRootExploripCopy.ReadBoolean(AutoStartOperationConfig); }
        set
        {
            if (AutoStartOperation != value)
                _registryRootExploripCopy.SetValue(AutoStartOperationConfig, value.ToString());
        }
    }

    public static bool NotificationOnEachOperation
    {
        get { return _registryRootExploripCopy.ReadBoolean(NotificationOnEachOperationConfig); }
        set
        {
            if (NotificationOnEachOperation != value)
                _registryRootExploripCopy.SetValue(NotificationOnEachOperationConfig, value.ToString());
        }
    }

    public static bool AutoExpandSubFolder
    {
        get { return _registryRootExploripCopy.ReadBoolean(AutoExpandSubFolderConfig); }
        set
        {
            if (AutoExpandSubFolder != value)
                _registryRootExploripCopy.SetValue(AutoExpandSubFolderConfig, value.ToString());
        }
    }
}
