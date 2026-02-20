using System.Globalization;

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
    private const string ConfigDragGhostOpacity = "DragGhostOpacity";
    private const string WaitBeforeStartScrollConfig = "WaitBeforeStartDragScroll";
    private const string WaitBetweenTwoScrollingConfig = "WaitBetweenTwoDragScrolling";
    private const string SpeedForDragScrollingConfig = "SpeedForDragScrolling";
    private const string MaxItemsInAdornerConfig = "MaxItemsInAdorner";
    private const string OpacityDecreaseConfig = "OpacityDecreaseInAdorner";
    private const string PosXConfig = "PosX";
    private const string PosYConfig = "PosY";
    private const string SizeXConfig = "SizeX";
    private const string SizeYConfig = "SizeY";
    private const string PlaySoundWhenFinishConfig = "PlaySoundWhenFinish";
    private const string SoundToPlayWhenFinishConfig = "PathSoundFinish";
    private const string PlaySoundWhenErrorConfig = "PlaySoundWhenError";
    private const string SoundToPlayWhenErrorConfig = "PathSoundError";

    private const string DefaultFalse = "False";

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
                _registryRootExploripCopy.SetValue(NotificationOnEachOperationConfig, DefaultFalse);
            if (string.IsNullOrWhiteSpace(_registryRootExploripCopy.GetValue(AutoExpandSubFolderConfig, "").ToString()))
                _registryRootExploripCopy.SetValue(AutoExpandSubFolderConfig, DefaultFalse);
            if (string.IsNullOrWhiteSpace(_registryRootExploripCopy.GetValue(InjectWindowsExplorerConfig, "").ToString()))
                _registryRootExploripCopy.SetValue(InjectWindowsExplorerConfig, DefaultFalse);
            if (string.IsNullOrWhiteSpace(_registryRootExploripCopy.GetValue(PriorityToLowerOperationsConfig, "").ToString()))
                _registryRootExploripCopy.SetValue(PriorityToLowerOperationsConfig, "True");
            if (string.IsNullOrWhiteSpace(_registryRootExploripCopy.GetValue(MaxBufferSizeConfig, "").ToString()))
                _registryRootExploripCopy.SetValue(MaxBufferSizeConfig, "1048576");
            if (string.IsNullOrWhiteSpace(_registryRootExploripCopy.GetValue(ConfigDragGhostOpacity, "").ToString()))
                _registryRootExploripCopy.SetValue(ConfigDragGhostOpacity, "0.7");
            if (string.IsNullOrWhiteSpace(_registryRootExploripCopy.GetValue(WaitBeforeStartScrollConfig, "").ToString()))
                _registryRootExploripCopy.SetValue(WaitBeforeStartScrollConfig, "2000");
            if (string.IsNullOrWhiteSpace(_registryRootExploripCopy.GetValue(WaitBetweenTwoScrollingConfig, "").ToString()))
                _registryRootExploripCopy.SetValue(WaitBetweenTwoScrollingConfig, "500");
            if (string.IsNullOrWhiteSpace(_registryRootExploripCopy.GetValue(SpeedForDragScrollingConfig, "").ToString()))
                _registryRootExploripCopy.SetValue(SpeedForDragScrollingConfig, "50");
            if (string.IsNullOrWhiteSpace(_registryRootExploripCopy.GetValue(MaxItemsInAdornerConfig, "").ToString()))
                _registryRootExploripCopy.SetValue(MaxItemsInAdornerConfig, "3");
            if (string.IsNullOrWhiteSpace(_registryRootExploripCopy.GetValue(OpacityDecreaseConfig, "").ToString()))
                _registryRootExploripCopy.SetValue(OpacityDecreaseConfig, "0.2");
            if (string.IsNullOrWhiteSpace(_registryRootExploripCopy.GetValue(PlaySoundWhenFinishConfig, "").ToString()))
                _registryRootExploripCopy.SetValue(PlaySoundWhenFinishConfig, DefaultFalse);
            if (string.IsNullOrWhiteSpace(_registryRootExploripCopy.GetValue(PlaySoundWhenErrorConfig, "").ToString()))
                _registryRootExploripCopy.SetValue(PlaySoundWhenErrorConfig, DefaultFalse);
            if (string.IsNullOrWhiteSpace(_registryRootExploripCopy.GetValue(SoundToPlayWhenFinishConfig, "").ToString()))
                _registryRootExploripCopy.SetValue(SoundToPlayWhenFinishConfig, "tada.wav");
            if (string.IsNullOrWhiteSpace(_registryRootExploripCopy.GetValue(SoundToPlayWhenErrorConfig, "").ToString()))
                _registryRootExploripCopy.SetValue(SoundToPlayWhenErrorConfig, "Windows Error.wav");
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

    public static double DragGhostOpacity
    {
        get { return _registryRootExploripCopy.ReadDouble(ConfigDragGhostOpacity); }
        set
        {
            if (DragGhostOpacity != value && AllowWrite)
                _registryRootExploripCopy.SetValue(ConfigDragGhostOpacity, value.ToString(CultureInfo.InvariantCulture));
        }
    }

    public static double OpacityDecrease
    {
        get { return _registryRootExploripCopy.ReadDouble(OpacityDecreaseConfig); }
        set
        {
            if (OpacityDecrease != value && AllowWrite)
                _registryRootExploripCopy.SetValue(OpacityDecreaseConfig, value.ToString(CultureInfo.InvariantCulture));
        }
    }

    public static int MaxItemsInAdorner
    {
        get { return _registryRootExploripCopy.ReadInteger(MaxItemsInAdornerConfig); }
        set
        {
            if (MaxItemsInAdorner != value && AllowWrite)
                _registryRootExploripCopy.SetValue(MaxItemsInAdornerConfig, value.ToString());
        }
    }

    public static int WaitBeforeStartDragScrolling
    {
        get { return _registryRootExploripCopy.ReadInteger(WaitBeforeStartScrollConfig); }
        set
        {
            if (WaitBeforeStartDragScrolling != value && AllowWrite)
                _registryRootExploripCopy.SetValue(WaitBeforeStartScrollConfig, value.ToString());
        }
    }

    public static int WaitBetweenTwoDragScrolling
    {
        get { return _registryRootExploripCopy.ReadInteger(WaitBetweenTwoScrollingConfig); }
        set
        {
            if (WaitBetweenTwoDragScrolling != value && AllowWrite)
                _registryRootExploripCopy.SetValue(WaitBetweenTwoScrollingConfig, value.ToString());
        }
    }

    public static int SpeedForDragScrolling
    {
        get { return _registryRootExploripCopy.ReadInteger(SpeedForDragScrollingConfig); }
        set
        {
            if (SpeedForDragScrolling != value && AllowWrite)
                _registryRootExploripCopy.SetValue(SpeedForDragScrollingConfig, value.ToString());
        }
    }

    public static int PosX
    {
        get { return _registryRootExploripCopy.ReadInteger(PosXConfig); }
        set
        {
            if (PosX != value && AllowWrite)
                _registryRootExploripCopy.SetValue(PosXConfig, value.ToString());
        }
    }

    public static int PosY
    {
        get { return _registryRootExploripCopy.ReadInteger(PosYConfig); }
        set
        {
            if (PosY != value && AllowWrite)
                _registryRootExploripCopy.SetValue(PosYConfig, value.ToString());
        }
    }

    public static int SizeX
    {
        get { return _registryRootExploripCopy.ReadInteger(SizeXConfig); }
        set
        {
            if (SizeX != value && AllowWrite)
                _registryRootExploripCopy.SetValue(SizeXConfig, value.ToString());
        }
    }

    public static int SizeY
    {
        get { return _registryRootExploripCopy.ReadInteger(SizeYConfig); }
        set
        {
            if (SizeY != value && AllowWrite)
                _registryRootExploripCopy.SetValue(SizeYConfig, value.ToString());
        }
    }

    public static bool PlaySoundWhenFinish
    {
        get { return _registryRootExploripCopy.ReadBoolean(PlaySoundWhenFinishConfig); }
        set
        {
            if (PlaySoundWhenFinish != value && AllowWrite)
                _registryRootExploripCopy.SetValue(PlaySoundWhenFinishConfig, value.ToString());
        }
    }

    public static bool PlaySoundWhenError
    {
        get { return _registryRootExploripCopy.ReadBoolean(PlaySoundWhenErrorConfig); }
        set
        {
            if (PlaySoundWhenError != value && AllowWrite)
                _registryRootExploripCopy.SetValue(PlaySoundWhenErrorConfig, value.ToString());
        }
    }

    public static string SoundToPlayWhenFinish
    {
        get { return _registryRootExploripCopy.GetValue(SoundToPlayWhenFinishConfig, "")?.ToString(); }
        set
        {
            if (SoundToPlayWhenFinish != value && AllowWrite)
                _registryRootExploripCopy.SetValue(SoundToPlayWhenFinishConfig, value);
        }
    }

    public static string SoundToPlayWhenError
    {
        get { return _registryRootExploripCopy.GetValue(SoundToPlayWhenErrorConfig, "")?.ToString(); }
        set
        {
            if (SoundToPlayWhenError != value && AllowWrite)
                _registryRootExploripCopy.SetValue(SoundToPlayWhenErrorConfig, value);
        }
    }
}
