namespace ExploripComponents;

public static class Constants
{
    public const int DelayIgnoreDrag = 200;
    public const int DelayIgnoreRename = 500;
    public const int DelayBeforeForceRefreshItems = 200;
    public static readonly int DoubleClickDelay = ManagedShell.Interop.NativeMethods.GetDoubleClickTime();
}

public enum GroupBy
{
    NONE = 0,
    NAME = 1,
    SIZE = 2,
    TYPE = 3,
    LAST_MODIFIED = 4,
}

public enum OrderBy
{
    NONE = 0,
    NAME = 1,
    SIZE = 2,
    TYPE = 3,
    LAST_MODIFIED = 4,
}

public enum OrderDirection
{
    ASC = 0,
    DESC = 1,
}
