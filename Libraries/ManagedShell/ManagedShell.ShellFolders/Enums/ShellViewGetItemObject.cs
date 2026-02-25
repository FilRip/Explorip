namespace ManagedShell.ShellFolders.Enums;

public enum ShellViewGetItemObject
{
    Background = 0x00000000,
    Selection = 0x00000001,
    AllView = 0x00000002,
    Checked = 0x00000003,
    TypeMask = 0x0000000F,
    ViewOrderFlag = unchecked((int)0x80000000),
}
