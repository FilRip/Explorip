﻿using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace ManagedShell.Interop;

public partial class NativeMethods
{
    const string Shell32_DllName = "shell32.dll";
    public const int S_OK = 0;
    public static readonly Guid CLSID_DragDropHelper = new("{4657278A-411B-11d2-839A-00C04FD918D0}");

    [StructLayout(LayoutKind.Sequential)]
    public struct AppBarData
    {
        public int cbSize;
        public IntPtr hWnd;
        public int uCallbackMessage;
        public int uEdge;
        public Rect rc;
        public IntPtr lParam;
    }

    public enum ABMsg
    {
        ABM_NEW = 0,
        ABM_REMOVE,
        ABM_QUERYPOS,
        ABM_SETPOS,
        ABM_GETSTATE,
        ABM_GETTASKBARPOS,
        ABM_ACTIVATE,
        ABM_GETAUTOHIDEBAR,
        ABM_SETAUTOHIDEBAR,
        ABM_WINDOWPOSCHANGED,
        ABM_SETSTATE
    }
    public enum ABEdge
    {
        ABE_LEFT = 0,
        ABE_TOP,
        ABE_RIGHT,
        ABE_BOTTOM
    }

    public enum AppBarNotifications
    {
        // Notifies an appbar that the taskbar's autohide or 
        // always-on-top state has changed—that is, the user has selected 
        // or cleared the "Always on top" or "Auto hide" check box on the
        // taskbar's property sheet. 
        StateChange = 0x00000000,
        // Notifies an appbar when an event has occurred that may affect 
        // the appbar's size and position. Events include changes in the
        // taskbar's size, position, and visibility state, as well as the
        // addition, removal, or resizing of another appbar on the same 
        // side of the screen.
        PosChanged = 0x00000001,
        // Notifies an appbar when a full-screen application is opening or
        // closing. This notification is sent in the form of an 
        // application-defined message that is set by the ABM_NEW message. 
        FullScreenApp = 0x00000002,
        // Notifies an appbar that the user has selected the Cascade, 
        // Tile Horizontally, or Tile Vertically command from the 
        // taskbar's shortcut menu.
        WindowArrange = 0x00000003
    }

    [DllImport(Shell32_DllName, CallingConvention = CallingConvention.StdCall)]
    internal static extern uint SHAppBarMessage(int dwMessage, ref AppBarData pData);

    public const uint FILE_ATTRIBUTE_NORMAL = 0x80;
    public const uint FILE_ATTRIBUTE_DIRECTORY = 0x10;


    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct ShFileInfo
    {
        public IntPtr hIcon;
        public int iIcon;
        public uint dwAttributes;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szDisplayName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string szTypeName;
    }

    [Flags()]
    public enum SHGFI
    {
        /// <summary>get icon</summary>
        Icon = 0x000000100,
        /// <summary>get display name</summary>
        DisplayName = 0x000000200,
        /// <summary>get type name</summary>
        TypeName = 0x000000400,
        /// <summary>get attributes</summary>
        Attributes = 0x000000800,
        /// <summary>get icon location</summary>
        IconLocation = 0x000001000,
        /// <summary>return exe type</summary>
        ExeType = 0x000002000,
        /// <summary>get system icon index</summary>
        SysIconIndex = 0x000004000,
        /// <summary>put a link overlay on icon</summary>
        LinkOverlay = 0x000008000,
        /// <summary>show icon in selected state</summary>
        Selected = 0x000010000,
        /// <summary>get only specified attributes</summary>
        Attr_Specified = 0x000020000,
        /// <summary>get large icon</summary>
        LargeIcon = 0x000000000,
        /// <summary>get small icon</summary>
        SmallIcon = 0x000000001,
        /// <summary>get open icon</summary>
        OpenIcon = 0x000000002,
        /// <summary>get shell size icon</summary>
        ShellIconSize = 0x000000004,
        /// <summary>pszPath is a pidl</summary>
        PIDL = 0x000000008,
        /// <summary>use passed dwFileAttribute</summary>
        UseFileAttributes = 0x000000010,
        /// <summary>apply the appropriate overlays</summary>
        AddOverlays = 0x000000020,
        /// <summary>Get the index of the overlay in the upper 8 bits of the iIcon</summary>
        OverlayIndex = 0x000000040,
    }

    [DllImport(Shell32_DllName, CharSet = CharSet.Auto)]
    internal static extern bool ShellExecuteEx(ref ShellExecuteInfo lpExecInfo);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct ShellExecuteInfo
    {
        public int cbSize;
        public uint fMask;
        public IntPtr hwnd;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string lpVerb;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string lpFile;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string lpParameters;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string lpDirectory;
        public int nShow;
        public IntPtr hInstApp;
        public IntPtr lpIDList;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string lpClass;
        public IntPtr hkeyClass;
        public uint dwHotKey;
        public IntPtr hIcon;
        public IntPtr hProcess;
    }

    public const uint SEE_MASK_INVOKEIDLIST = 12;

    /// <summary>
    /// Possible flags for the SHFileOperation method.
    /// </summary>
    [Flags()]
    public enum FileOperation : ushort
    {
        /// <summary>
        /// Do not show a dialog during the process
        /// </summary>
        FOF_SILENT = 0x0004,
        /// <summary>
        /// Do not ask the user to confirm selection
        /// </summary>
        FOF_NOCONFIRMATION = 0x0010,
        /// <summary>
        /// Delete the file to the recycle bin.  (Required flag to send a file to the bin
        /// </summary>
        FOF_ALLOWUNDO = 0x0040,
        /// <summary>
        /// Do not show the names of the files or folders that are being recycled.
        /// </summary>
        FOF_SIMPLEPROGRESS = 0x0100,
        /// <summary>
        /// Surpress errors, if any occur during the process.
        /// </summary>
        FOF_NOERRORUI = 0x0400,
        /// <summary>
        /// Warn if files are too big to fit in the recycle bin and will need
        /// to be deleted completely.
        /// </summary>
        FOF_WANTNUKEWARNING = 0x4000,
    }

    /// <summary>
    /// File Operation Function Type for SHFileOperation
    /// </summary>
    public enum FileOperationType : uint
    {
        /// <summary>
        /// Move the objects
        /// </summary>
        FO_MOVE = 0x0001,
        /// <summary>
        /// Copy the objects
        /// </summary>
        FO_COPY = 0x0002,
        /// <summary>
        /// Delete (or recycle) the objects
        /// </summary>
        FO_DELETE = 0x0003,
        /// <summary>
        /// Rename the object(s)
        /// </summary>
        FO_RENAME = 0x0004,
    }

    /// <summary>
    /// SHFILEOPSTRUCT for SHFileOperation from COM
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct ShFileOpStruct
    {

        public IntPtr hwnd;
        [MarshalAs(UnmanagedType.U4)]
        public FileOperationType wFunc;
        public string pFrom;
        public string pTo;
        public FileOperation fFlags;
        [MarshalAs(UnmanagedType.Bool)]
        public bool fAnyOperationsAborted;
        public IntPtr hNameMappings;
        public string lpszProgressTitle;
    }

    [DllImport(Shell32_DllName, CharSet = CharSet.Auto)]
    internal static extern int SHFileOperation(ref ShFileOpStruct FileOp);

    public enum NIN : uint
    {
        SELECT = 0x400,
        BALLOONSHOW = 0x402,
        BALLOONHIDE = 0x403,
        BALLOONTIMEOUT = 0x404,
        BALLOONUSERCLICK = 0x405,
        POPUPOPEN = 0x406,
        POPUPCLOSE = 0x407,
    }

    /// <summary>
    /// Numerical values of the NIM_* messages represented as an enumeration.
    /// </summary>
    public enum NIM : uint
    {
        /// <summary>
        /// Add a new icon.
        /// </summary>
        NIM_ADD = 0,

        /// <summary>
        /// Modify an existing icon.
        /// </summary>
        NIM_MODIFY = 1,

        /// <summary>
        /// Delete an icon.
        /// </summary>
        NIM_DELETE = 2,

        /// <summary>
        /// Shell v5 and above - Return focus to the notification area.
        /// </summary>
        NIM_SETFOCUS = 3,

        /// <summary>
        /// Shell v4 and above - Instructs the taskbar to behave accordingly based on the version (uVersion) set in the notifiyicondata struct.
        /// </summary>
        NIM_SETVERSION = 4
    }

    /// <summary>
    /// Shell_NotifyIcon flags.  NIF_*
    /// </summary>
    [Flags()]
    public enum NIF : uint
    {
        MESSAGE = 0x0001,
        ICON = 0x0002,
        TIP = 0x0004,
        STATE = 0x0008,
        INFO = 0x0010,
        GUID = 0x0020,

        /// <summary>
        /// Vista only.
        /// </summary>
        REALTIME = 0x0040,
        /// <summary>
        /// Vista only.
        /// </summary>
        SHOWTIP = 0x0080,

        XP_MASK = STATE | INFO | GUID,
        VISTA_MASK = REALTIME | SHOWTIP,
    }

    /// <summary>
    /// Notify icon info balloon flags
    /// </summary>
    [Flags()]
#pragma warning disable S4070 // Non-flags enums should not be marked with "FlagsAttribute"
    public enum NIIF : uint
#pragma warning restore S4070 // Non-flags enums should not be marked with "FlagsAttribute"
    {
        NONE = 0x00000000,
        INFO = 0x00000001,
        WARNING = 0x00000002,
        ERROR = INFO | WARNING,
        /// <summary>Use app-provided icon in the balloon. XP SP2 and later.</summary>
        USER = 0x00000004,
        /// <summary>XP and later.</summary>
        NOSOUND = 0x00000010,
        /// <summary>Vista and later.</summary>
        LARGE_ICON = 0x00000020,
        /// <summary>Windows 7 and later</summary>
        NIIF_RESPECT_QUIET_TIME = 0x00000080,
        /// <summary>Reserved. XP and later.</summary>
        ICON_MASK = 0x0000000F,
    }

    [Flags()]
    public enum NIS
    {
        /// <summary>The icon is hidden.</summary>
        NIS_HIDDEN = 0x00000001,
        /// <summary>The icon resource is shared between multiple icons.</summary>
        NIS_SHAREDICON = 0x00000002
    }

    /// <summary>
    /// Notify icon data structure type
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct NotifyIconData
    {
        public uint cbSize;
        public uint hWnd;
        public uint uID;
        public NIF uFlags;
        public uint uCallbackMessage;
        public uint hIcon;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string szTip;
        public NIS dwState;
        public NIS dwStateMask;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string szInfo;
        public uint uTimeoutOrVersion;  // used with NIM_SETVERSION, values 0, 3 and 4
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string szInfoTitle;
        public NIIF dwInfoFlags;
        public Guid guidItem;
        public uint hBalloonIcon;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ShellTrayData
    {
        public int dwUnknown;
        public uint dwMessage;
        public NotifyIconData nid;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WinNotifyIconIdentifier
    {
        public int dwMagic;
        public int dwMessage;
        public int cbSize;
        public int dwPadding;
        public uint hWnd;
        public uint uID;
        public Guid guidItem;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AppBarDataV2
    {
        public int cbSize;
        public uint hWnd;
        public uint uCallbackMessage;
        public uint uEdge;
        public Rect rc;
        public int lParam;
        public int dw64BitAlign;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AppBarMsgDataV3
    {
        public AppBarDataV2 abd;
        public int dwMessage;
        public int dwPadding1;
        public uint hSharedMemory;
        public int dwPadding2;
        public int dwSourceProcessId;
        public int dwPadding3;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct PropertyKey
    {
        public Guid fmtid;
        public uint pid;
    }

    [ComImport(), Guid("886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPropertyStore
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetCount([Out()] out uint cProps);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetAt([In()] uint iProp, out PropertyKey pkey);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetValue([In()] ref PropertyKey key, out PropVariant pv);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetValue([In()] ref PropertyKey key, [In()] ref PropVariant pv);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Commit();
    }

    [DllImport(Shell32_DllName, SetLastError = true)]
    internal static extern int SHGetPropertyStoreForWindow(IntPtr handle, ref Guid riid, out IPropertyStore propertyStore);

    [Flags()]
    public enum RunFileDialog : uint
    {

        /// <summary>
        /// Don't use any of the flags (only works alone)
        /// </summary>
        None = 0x0000,

        /// <summary>
        /// Removes the browse button
        /// </summary>
        NoBrowse = 0x0001,

        /// <summary>
        /// No default item selected
        /// </summary>
        NoDefault = 0x0002,

        /// <summary>
        /// Calculates the working directory from the file name
        /// </summary>
        CalcDirectory = 0x0004,

        /// <summary>
        /// Removes the edit box label
        /// </summary>
        NoLabel = 0x0008,

        /// <summary>
        /// Removes the separate memory space checkbox (Windows NT only)
        /// </summary>
        NoSeperateMemory = 0x0020
    }

    [DllImport(Shell32_DllName, CharSet = CharSet.Auto, EntryPoint = "#61", SetLastError = true)]
    internal static extern bool SHRunFileDialog(IntPtr hwndOwner,
        IntPtr hIcon,
        string lpszPath,
        string lpszDialogTitle,
        string lpszDialogTextBody,
        RunFileDialog uflags);

#pragma warning disable S1104 // Fields should not have public accessibility
    public struct ImageListDrawParams
    {
        public int cbSize;
        public IntPtr himl;
        public int i;
        public IntPtr hdcDst;
        public int x;
        public int y;
        public int cx;
        public int cy;
        public int xBitmap;        // x offest from the upperleft of bitmap
        public int yBitmap;        // y offset from the upperleft of bitmap
        public int rgbBk;
        public int rgbFg;
        public int fStyle;
        public int dwRop;
        public int fState;
        public int Frame;
        public int crEffect;
    }
#pragma warning restore S1104 // Fields should not have public accessibility

    [StructLayout(LayoutKind.Sequential)]
    public struct ImageInfo
    {
        public IntPtr hbmImage;
        public IntPtr hbmMask;
        public int Unused1;
        public int Unused2;
        public Rect rcImage;
    }

    [Flags()]
#pragma warning disable S4070 // Non-flags enums should not be marked with "FlagsAttribute"
    public enum ILD
    {
        NORMAL = 0x00000000,
        TRANSPARENT = 0x00000001,
        MASK = 0x00000010,
        IMAGE = 0x00000020,
        ROP = 0x00000040,
        BLEND25 = 0x00000002,
        BLEND50 = 0x00000004,
        OVERLAYMASK = 0x00000F00,
        PRESERVEALPHA = 0x00001000,
        SCALE = 0x00002000,
        DPISCALE = 0x00004000,
        ASYNC = 0x00008000,
        SELECTED = BLEND50,
        FOCUS = BLEND25,
        BLEND = BLEND50,
    }
#pragma warning restore S4070 // Non-flags enums should not be marked with "FlagsAttribute"

    [ComImport()]
    [Guid("46EB5926-582E-4017-9FDF-E8998DAA0950")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IImageList
    {
        [PreserveSig()]
        int Add(
            IntPtr hbmImage,
            IntPtr hbmMask,
            ref int pi);

        [PreserveSig()]
        int ReplaceIcon(
            int i,
            IntPtr hicon,
            ref int pi);

        [PreserveSig()]
        int SetOverlayImage(
            int iImage,
            int iOverlay);

        [PreserveSig()]
        int Replace(
            int i,
            IntPtr hbmImage,
            IntPtr hbmMask);

        [PreserveSig()]
        int AddMasked(
            IntPtr hbmImage,
            int crMask,
            ref int pi);

        [PreserveSig()]
        int Draw(
            ref ImageListDrawParams pimldp);

        [PreserveSig()]
        int Remove(
        int i);

        [PreserveSig()]
        int GetIcon(
            int i,
            ILD flags,
            ref IntPtr picon);

        [PreserveSig()]
        int GetImageInfo(
            int i,
            ref ImageInfo pImageInfo);

        [PreserveSig()]
        int Copy(
            int iDst,
            IImageList punkSrc,
            int iSrc,
            int uFlags);

        [PreserveSig()]
        int Merge(
            int i1,
            IImageList punk2,
            int i2,
            int dx,
            int dy,
            ref Guid riid,
            ref IntPtr ppv);

        [PreserveSig()]
        int Clone(
            ref Guid riid,
            ref IntPtr ppv);

        [PreserveSig()]
        int GetImageRect(
            int i,
            ref Rect prc);

        [PreserveSig()]
        int GetIconSize(
            ref int cx,
            ref int cy);

        [PreserveSig()]
        int SetIconSize(
            int cx,
            int cy);

        [PreserveSig()]
        int GetImageCount(
        ref int pi);

        [PreserveSig()]
        int SetImageCount(
            int uNewCount);

        [PreserveSig()]
        int SetBkColor(
            int clrBk,
            ref int pclr);

        [PreserveSig()]
        int GetBkColor(
            ref int pclr);

        [PreserveSig()]
        int BeginDrag(
            int iTrack,
            int dxHotspot,
            int dyHotspot);

        [PreserveSig()]
        int EndDrag();

        [PreserveSig()]
        int DragEnter(
            IntPtr hwndLock,
            int x,
            int y);

        [PreserveSig()]
        int DragLeave(
            IntPtr hwndLock);

        [PreserveSig()]
        int DragMove(
            int x,
            int y);

        [PreserveSig()]
        int SetDragCursorImage(
            ref IImageList punk,
            int iDrag,
            int dxHotspot,
            int dyHotspot);

        [PreserveSig()]
        int DragShowNolock(
            int fShow);

        [PreserveSig()]
        int GetDragImage(
            ref Point ppt,
            ref Point pptHotspot,
            ref Guid riid,
            ref IntPtr ppv);

        [PreserveSig()]
        int GetItemFlags(
            int i,
            ref int dwFlags);

        [PreserveSig()]
        int GetOverlayImage(
            int iOverlay,
            ref int piIndex);
    };

    [DllImport(Shell32_DllName, EntryPoint = "#727")]
    internal static extern int SHGetImageList(
        int iImageList,
        ref Guid riid,
        out IImageList ppv
        );

    [DllImport(Shell32_DllName)]
    internal static extern int SHGetKnownFolderPath(
        [MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags, IntPtr hToken,
        out IntPtr ppszPath);

    [DllImport(Shell32_DllName, CharSet = CharSet.Unicode)]
    internal extern static int SHGetKnownFolderPath(ref Guid folderId, KnownFolder flags, IntPtr token, [MarshalAs(UnmanagedType.LPWStr)] out string pszPath);

    [Flags()]
    public enum KnownFolder : uint
    {
        None = 0,
        SimpleIDList = 0x00000100,
        NotParentRelative = 0x00000200,
        DefaultPath = 0x00000400,
        Init = 0x00000800,
        NoAlias = 0x00001000,
        DontUnexpand = 0x00002000,
        DontVerify = 0x00004000,
        Create = 0x00008000,
        NoAppcontainerRedirection = 0x00010000,
        AliasOnly = 0x80000000
    }

    [DllImport(Shell32_DllName, EntryPoint = "ExtractIconExW", CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
    internal static extern int ExtractIconEx(string sFile, int iIndex, out IntPtr piLargeVersion, out IntPtr piSmallVersion, int amountIcons);

    [DllImport(Shell32_DllName)]
    internal static extern int SHGetDesktopFolder(out IntPtr ppshf);

    [DllImport(Shell32_DllName)]
    internal static extern int SHGetFolderLocation(IntPtr hwndOwner, CSIDL nFolder, IntPtr hToken, uint dwReserved, out IntPtr ppidl);

    [DllImport(Shell32_DllName, SetLastError = true)]
    internal static extern void ILFree(IntPtr pidl);

    [DllImport(Shell32_DllName, SetLastError = true, CharSet = CharSet.Unicode, PreserveSig = false)]
    [return: MarshalAs(UnmanagedType.Interface)]
    internal static extern object SHCreateItemFromParsingName([MarshalAs(UnmanagedType.LPWStr)] string pszPath, IBindCtx pbc, ref Guid riid);

    [DllImport(Shell32_DllName, SetLastError = true, CharSet = CharSet.Unicode, PreserveSig = false)]
    [return: MarshalAs(UnmanagedType.Interface)]
    internal static extern object SHCreateItemFromParsingName([MarshalAs(UnmanagedType.LPWStr)] string pszPath, IntPtr pbc, ref Guid riid, out IntPtr shellItem);

    [DllImport(Shell32_DllName, ExactSpelling = true, PreserveSig = false)]
    internal static extern void SHBindToParent(
        IntPtr pidl,
        ref Guid riid,
        out IntPtr ppv,
        IntPtr ppidlLast);

    [DllImport(Shell32_DllName, SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern IntPtr ILCreateFromPath([MarshalAs(UnmanagedType.LPWStr)] string pszPath);

    [DllImport(Shell32_DllName, SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern int SHGetSpecialFolderLocation(IntPtr hwndOwner, CSIDL nFolder, ref IntPtr ppidl);

    [Flags()]
    public enum FILE_ATTRIBUTE : uint
    {
        NULL = 0x0000000,
        NORMAL = 0x00000080,
        DIRECTORY = 0x00000010,
    }

    [DllImport(Shell32_DllName, CharSet = CharSet.Unicode)]
    internal static extern IntPtr SHGetFileInfo(string pszPath, FILE_ATTRIBUTE dwFileAttributes, ref ShFileInfo psfi, uint cbFileInfo, SHGFI uFlags);

    [DllImport(Shell32_DllName, CharSet = CharSet.Unicode)]
    internal static extern IntPtr SHGetFileInfo(IntPtr pszPath, FILE_ATTRIBUTE dwFileAttributes, ref ShFileInfo psfi, uint cbFileInfo, SHGFI uFlags);

    [DllImport(Shell32_DllName, CharSet = CharSet.Unicode)]
    internal static extern int SHCreateShellItem(IntPtr parentPidl, IntPtr parentShellFolder, IntPtr pidl, out IntPtr shellItemPtr);

    [DllImport(Shell32_DllName, SetLastError = false)]
    internal static extern int SHCreateDefaultContextMenu(ref object defContextMenu, in Guid riid, out IntPtr ppv);

    [DllImport(Shell32_DllName, CharSet = CharSet.Auto)]
    internal static extern int SHParseDisplayName(string pszName, IntPtr pbc, out IntPtr ppidl, uint sfgaoIn, out uint sfgaoOut);

    [DllImport(Shell32_DllName, ExactSpelling = true, SetLastError = false)]
    internal static extern IntPtr ILCombine(IntPtr pidl1, IntPtr pidl2);

    [DllImport(Shell32_DllName, CharSet = CharSet.Unicode)]
    internal static extern bool Shell_NotifyIcon(NIM dwMessage, ref NotifyIconData lpData);
}
