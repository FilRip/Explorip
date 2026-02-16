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
    private const string InjectWindowsExplorerConfig = "InjectWindowsExplorer";
    private const string PriorityToLowerOperationsConfig = "PriorityToLowerOperations";
    private const string MaxBufferSizeConfig = "MaxBufferSize";

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
            if (string.IsNullOrWhiteSpace(_registryRootExploripCopy.GetValue(InjectWindowsExplorerConfig, "").ToString()))
                _registryRootExploripCopy.SetValue(InjectWindowsExplorerConfig, "False");
            if (string.IsNullOrWhiteSpace(_registryRootExploripCopy.GetValue(PriorityToLowerOperationsConfig, "").ToString()))
                _registryRootExploripCopy.SetValue(PriorityToLowerOperationsConfig, "True");
            if (string.IsNullOrWhiteSpace(_registryRootExploripCopy.GetValue(MaxBufferSizeConfig, "").ToString()))
                _registryRootExploripCopy.SetValue(MaxBufferSizeConfig, "1048576");
        }
    }

    public static int MaxBufferSize
    {
        get { return _registryRootExploripCopy.ReadInteger(MaxBufferSizeConfig); }
        set
        {
            if (MaxBufferSize != value && AllowWrite)
                _registryRootExploripCopy.SetValue(MaxBufferSizeConfig, value.ToString());
        }
    }

    public static bool AutoStartOperation
    {
        get { return _registryRootExploripCopy.ReadBoolean(AutoStartOperationConfig); }
        set
        {
            if (AutoStartOperation != value && AllowWrite)
                _registryRootExploripCopy.SetValue(AutoStartOperationConfig, value.ToString());
        }
    }

    public static bool NotificationOnEachOperation
    {
        get { return _registryRootExploripCopy.ReadBoolean(NotificationOnEachOperationConfig); }
        set
        {
            if (NotificationOnEachOperation != value && AllowWrite)
                _registryRootExploripCopy.SetValue(NotificationOnEachOperationConfig, value.ToString());
        }
    }

    public static bool AutoExpandSubFolder
    {
        get { return _registryRootExploripCopy.ReadBoolean(AutoExpandSubFolderConfig); }
        set
        {
            if (AutoExpandSubFolder != value && AllowWrite)
                _registryRootExploripCopy.SetValue(AutoExpandSubFolderConfig, value.ToString());
        }
    }

    public static bool InjectWindowsExplorer
    {
        get { return _registryRootExploripCopy.ReadBoolean(InjectWindowsExplorerConfig); }
        set
        {
            if (InjectWindowsExplorer != value && AllowWrite)
                _registryRootExploripCopy.SetValue(InjectWindowsExplorerConfig, value.ToString());
        }
    }

    public static bool PriorityToLowerOperations
    {
        get { return _registryRootExploripCopy.ReadBoolean(PriorityToLowerOperationsConfig); }
        set
        {
            if (PriorityToLowerOperations != value && AllowWrite)
                _registryRootExploripCopy.SetValue(PriorityToLowerOperationsConfig, value.ToString());
        }
    }
}
