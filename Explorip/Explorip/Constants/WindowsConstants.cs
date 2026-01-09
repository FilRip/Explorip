namespace Explorip.Constants;

public static class WindowsConstants
{
    public const int DelayIgnoreDrag = 200;
    public static readonly int DoubleClickDelay = ManagedShell.Interop.NativeMethods.GetDoubleClickTime();

    public const string StyleMenuItemWithSubMenu = "MenuItemWithSubMenuStyle";
}
