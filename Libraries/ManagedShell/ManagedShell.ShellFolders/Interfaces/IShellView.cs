using System;
using System.Runtime.InteropServices;

using ManagedShell.ShellFolders.Enums;

using static ManagedShell.Interop.NativeMethods;

namespace ManagedShell.ShellFolders.Interfaces;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct FolderSettings
{
    public FolderViewMode ViewMode;
    public EFolder Flags;
}

[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode)]
[return: MarshalAs(UnmanagedType.Bool)]
public delegate bool AddPropSheetPageProc(IntPtr hpage, IntPtr lParam);

[ComImport(),
 Guid("000214E3-0000-0000-C000-000000000046"),
 InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IShellView
{
    // IOleWindow
    [PreserveSig()]
    int GetWindow(
        out IntPtr phwnd);

    [PreserveSig()]
    int ContextSensitiveHelp(
        [MarshalAs(UnmanagedType.Bool)] bool fEnterMode);

    void TranslateAccelerator(
        IntPtr pmsg);

    void EnableModeless(
        [MarshalAs(UnmanagedType.Bool)] bool fEnable);

    void UIActivate(
        uint uState);

    void Refresh();

    IntPtr CreateViewWindow(
        IShellView psvPrevious,
        FolderSettings pfs,
        IShellBrowser psb,
        Rect prcView);

    void DestroyViewWindow();

    FolderSettings GetCurrentInfo();

    void AddPropertySheetPages(
        uint dwReserved,
        AddPropSheetPageProc pfn,
        IntPtr lparam);

    void SaveViewState();

    void SelectItem(
        IntPtr pidlItem,
        SVSIF uFlags);

    [return: MarshalAs(UnmanagedType.Interface, IidParameterIndex = 1)]
    object GetItemObject(
        ShellViewGetItemObject uItem,
        ref Guid riid);
}
