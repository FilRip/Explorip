using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ManagedShell.ShellFolders.Interfaces;

internal enum ShellViewGetItemObject
{
    Background = 0x00000000,
    Selection = 0x00000001,
    AllView = 0x00000002,
    Checked = 0x00000003,
    TypeMask = 0x0000000F,
    ViewOrderFlag = unchecked((int)0x80000000)
}

public enum FolderViewMode : uint
{
    //FVM_AUTO = -1,
    FVM_FIRST = 1,
    FVM_ICON = 1,
    FVM_SMALLICON = 2,
    FVM_LIST = 3,
    FVM_DETAILS = 4,
    FVM_THUMBNAIL = 5,
    FVM_TILE = 6,
    FVM_THUMBSTRIP = 7,
    FVM_CONTENT = 8,
    FVM_LAST = 8
}

[Flags()]
#pragma warning disable S2344 // Enumeration type names should not have "Flags" or "Enum" suffixes
public enum FolderFlags : uint
#pragma warning restore S2344 // Enumeration type names should not have "Flags" or "Enum" suffixes
{
    FWF_NONE = 0,
    FWF_AUTOARRANGE = 0x1,
    FWF_ABBREVIATEDNAMES = 0x2,
    FWF_SNAPTOGRID = 0x4,
    FWF_OWNERDATA = 0x8,
    FWF_BESTFITWINDOW = 0x10,
    FWF_DESKTOP = 0x20,
    FWF_SINGLESEL = 0x40,
    FWF_NOSUBFOLDERS = 0x80,
    FWF_TRANSPARENT = 0x100,
    FWF_NOCLIENTEDGE = 0x200,
    FWF_NOSCROLL = 0x400,
    FWF_ALIGNLEFT = 0x800,
    FWF_NOICONS = 0x1000,
    FWF_SHOWSELALWAYS = 0x2000,
    FWF_NOVISIBLE = 0x4000,
    FWF_SINGLECLICKACTIVATE = 0x8000,
    FWF_NOWEBVIEW = 0x10000,
    FWF_HIDEFILENAMES = 0x20000,
    FWF_CHECKSELECT = 0x40000,
    FWF_NOENUMREFRESH = 0x80000,
    FWF_NOGROUPING = 0x100000,
    FWF_FULLROWSELECT = 0x200000,
    FWF_NOFILTERS = 0x400000,
    FWF_NOCOLUMNHEADER = 0x800000,
    FWF_NOHEADERINALLVIEWS = 0x1000000,
    FWF_EXTENDEDTILES = 0x2000000,
    FWF_TRICHECKSELECT = 0x4000000,
    FWF_AUTOCHECKSELECT = 0x8000000,
    FWF_NOBROWSERVIEWSTATE = 0x10000000,
    FWF_SUBSETGROUPS = 0x20000000,
    FWF_USESEARCHFOLDER = 0x40000000,
    FWF_ALLOWRTLREADING = 0x80000000
}

[StructLayout(LayoutKind.Sequential)]
public struct FolderSettings
{
    public FolderViewMode ViewMode;
    public FolderFlags Flags;
}

[ComImport(),
 Guid("000214E3-0000-0000-C000-000000000046"),
 InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IShellView
{
    // IOleWindow
    [PreserveSig()]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int GetWindow(
        out IntPtr phwnd);

    [PreserveSig()]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int ContextSensitiveHelp(
        bool fEnterMode);

    // IShellView
    [PreserveSig()]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int TranslateAccelerator(
        IntPtr pmsg);

    [PreserveSig()]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int EnableModeless(
        bool fEnable);

    [PreserveSig()]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int UIActivate(
        uint uState);

    [PreserveSig()]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int Refresh();

    [PreserveSig()]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int CreateViewWindow(
        [MarshalAs(UnmanagedType.IUnknown)] object psvPrevious,
        IntPtr pfs,
        [MarshalAs(UnmanagedType.IUnknown)] object psb,
        IntPtr prcView,
        out IntPtr phWnd);

    [PreserveSig()]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int DestroyViewWindow();

    [PreserveSig()]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int GetCurrentInfo(
        out IntPtr pfs);

    [PreserveSig()]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int AddPropertySheetPages(
        uint dwReserved,
        IntPtr pfn,
        uint lparam);

    [PreserveSig()]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int SaveViewState();

    [PreserveSig()]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int SelectItem(
        IntPtr pidlItem,
        uint uFlags);

    [PreserveSig()]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int GetItemObject(
        ShellViewGetItemObject uItem,
        ref Guid riid,
        [MarshalAs(UnmanagedType.IUnknown)] out object ppv);
}
