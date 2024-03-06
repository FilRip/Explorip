using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ManagedShell.ShellFolders.Interfaces
{
    internal enum ShellViewGetItemObject
    {
        Background = 0x00000000,
        Selection = 0x00000001,
        AllView = 0x00000002,
        Checked = 0x00000003,
        TypeMask = 0x0000000F,
        ViewOrderFlag = unchecked((int)0x80000000)
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
}
