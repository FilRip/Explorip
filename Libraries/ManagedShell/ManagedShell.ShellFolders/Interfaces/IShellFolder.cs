using System;
using System.Runtime.InteropServices;

using ManagedShell.Interop;
using ManagedShell.ShellFolders.Enums;

namespace ManagedShell.ShellFolders.Interfaces;

[ComImport()]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("000214E6-0000-0000-C000-000000000046")]
public interface IShellFolder
{
    // Translates a file object's or folder's display name into an item identifier list.
    // Return value: error code, if any
    [PreserveSig()]
    int ParseDisplayName(
        IntPtr hwnd,
        IntPtr pbc,
        [MarshalAs(UnmanagedType.LPWStr)]
        string pszDisplayName,
        ref uint pchEaten,
        out IntPtr ppidl,
        ref ShellFolderGetAttributeObjects pdwAttributes);

    // Allows a client to determine the contents of a folder by creating an item
    // identifier enumeration object and returning its IEnumIDList interface.
    // Return value: error code, if any
    [PreserveSig()]
    int EnumObjects(
        IntPtr hwnd,
        ShConstTypes grfFlags,
        out IntPtr enumIDList);

    // Retrieves an IShellFolder object for a subfolder.
    // Return value: error code, if any
    [PreserveSig()]
    int BindToObject(
        IntPtr pidl,
        IntPtr pbc,
        ref Guid riid,
        out IntPtr ppv);

    // Requests a pointer to an object's storage interface. 
    // Return value: error code, if any
    [PreserveSig()]
    int BindToStorage(
        IntPtr pidl,
        IntPtr pbc,
        ref Guid riid,
        out IntPtr ppv);

    // Determines the relative order of two file objects or folders, given their
    // item identifier lists. Return value: If this method is successful, the
    // CODE field of the int contains one of the following values (the code
    // can be retrived using the helper function GetintCode): Negative A
    // negative return value indicates that the first item should precede
    // the second (pidl1 < pidl2). 

    // Positive A positive return value indicates that the first item should
    // follow the second (pidl1 > pidl2).  Zero A return value of zero
    // indicates that the two items are the same (pidl1 = pidl2). 
    [PreserveSig()]
    int CompareIDs(
        IntPtr lParam,
        IntPtr pidl1,
        IntPtr pidl2);

    // Requests an object that can be used to obtain information from or interact
    // with a folder object.
    // Return value: error code, if any
    [PreserveSig()]
    int CreateViewObject(
        IntPtr hwndOwner,
        Guid riid,
        out IntPtr ppv);

    // Retrieves the attributes of one or more file objects or subfolders. 
    // Return value: error code, if any
    [PreserveSig()]
    int GetAttributesOf(
        uint cidl,
        [MarshalAs(UnmanagedType.LPArray)]
        IntPtr[] apidl,
        ref ShellFolderGetAttributeObjects rgfInOut);

    // Retrieves an OLE interface that can be used to carry out actions on the
    // specified file objects or folders.
    // Return value: error code, if any
    [PreserveSig()]
    int GetUIObjectOf(
        IntPtr hwndOwner,
        uint cidl,
        [MarshalAs(UnmanagedType.LPArray)]
        IntPtr[] apidl,
        ref Guid riid,
        IntPtr rgfReserved,
        out IntPtr ppv);

    // Retrieves the display name for the specified file object or subfolder. 
    // Return value: error code, if any
    [PreserveSig()]
    int GetDisplayNameOf(
        IntPtr pidl,
        SHGetDisplayNames uFlags,
        IntPtr lpName);

    // Sets the display name of a file object or subfolder, changing the item
    // identifier in the process.
    // Return value: error code, if any
    [PreserveSig()]
    int SetNameOf(
        IntPtr hwnd,
        IntPtr pidl,
        [MarshalAs(UnmanagedType.LPWStr)]
        string pszName,
        SHGetDisplayNames uFlags,
        out IntPtr ppidlOut);
}

[Flags()]
public enum ListViewColumnAlignments
{
    LEFT = 0X0000,
    RIGHT = 0X0001,
    CENTER = 0X0002,
    JUSTIFYMASK = 0X0003,
    IMAGE = 0X0800,
    BITMAP_ON_RIGHT = 0X1000,
    COL_HAS_IMAGES = 0X8000,
    FIXED_WIDTH = 0X00100,
    NO_DPI_SCALE = 0X40000,
    FIXED_RATIO = 0X80000,
    LINE_BREAK = 0X100000,
    FILL = 0X200000,
    WRAP = 0X400000,
    NO_TITLE = 0X800000,
    TILE_PLACEMENTMASK = LINE_BREAK | FILL,
    SPLITBUTTON = 0X1000000,
}

[StructLayout(LayoutKind.Sequential)]
public struct ShellDetails
{
    public ListViewColumnAlignments fmt;
    public int cxChar;
    public StrRet str;
}

public enum StrRetType : uint
{
    WSTR = 0x0000,
    OFFSET = 0x0001,
    CSTR = 0x0002,
}

[StructLayout(LayoutKind.Sequential)]
public struct StrRet
{
    private const int strlenbuf = 260;
    public StrRetType uType;
    private DummyUnionName union;

    public IntPtr OleStr => union.pOleStr; // must be freed by caller of GetDisplayNameOf
    public uint Offset => union.uOffset; // Offset into SHITEMID
    public IntPtr Str => uType == StrRetType.CSTR ? union.cStr : IntPtr.Zero;

    [StructLayout(LayoutKind.Explicit, Size = strlenbuf)]
    private struct DummyUnionName
    {
        [FieldOffset(0)]
        public IntPtr pOleStr;
        [FieldOffset(0)]
        public uint uOffset;
        [FieldOffset(0)]
        public IntPtr cStr;
    }

    public void Free()
    {
        if (uType == StrRetType.WSTR)
        {
            Marshal.FreeCoTaskMem(union.pOleStr);
            union.pOleStr = IntPtr.Zero;
        }
    }
}

[ComImport(),
Guid("93F2F68C-1D1B-11D3-A30E-00C04F79ABD1"),
InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IShellFolder2 : IShellFolder
{
    // Translates a file object's or folder's display name into an item identifier list.
    // Return value: error code, if any
    [PreserveSig()]
    new int ParseDisplayName(
        IntPtr hwnd,
        IntPtr pbc,
        [MarshalAs(UnmanagedType.LPWStr)]
        string pszDisplayName,
        ref uint pchEaten,
        out IntPtr ppidl,
        ref ShellFolderGetAttributeObjects pdwAttributes);

    // Allows a client to determine the contents of a folder by creating an item
    // identifier enumeration object and returning its IEnumIDList interface.
    // Return value: error code, if any
    [PreserveSig()]
    new int EnumObjects(
        IntPtr hwnd,
        ShConstTypes grfFlags,
        out IntPtr enumIDList);

    // Retrieves an IShellFolder object for a subfolder.
    // Return value: error code, if any
    [PreserveSig()]
    new int BindToObject(
        IntPtr pidl,
        IntPtr pbc,
        ref Guid riid,
        out IntPtr ppv);

    // Requests a pointer to an object's storage interface. 
    // Return value: error code, if any
    [PreserveSig()]
    new int BindToStorage(
        IntPtr pidl,
        IntPtr pbc,
        ref Guid riid,
        out IntPtr ppv);

    // Determines the relative order of two file objects or folders, given their
    // item identifier lists. Return value: If this method is successful, the
    // CODE field of the int contains one of the following values (the code
    // can be retrived using the helper function GetintCode): Negative A
    // negative return value indicates that the first item should precede
    // the second (pidl1 < pidl2). 

    // Positive A positive return value indicates that the first item should
    // follow the second (pidl1 > pidl2).  Zero A return value of zero
    // indicates that the two items are the same (pidl1 = pidl2). 
    [PreserveSig()]
    new int CompareIDs(
        IntPtr lParam,
        IntPtr pidl1,
        IntPtr pidl2);

    // Requests an object that can be used to obtain information from or interact
    // with a folder object.
    // Return value: error code, if any
    [PreserveSig()]
    new int CreateViewObject(
        IntPtr hwndOwner,
        Guid riid,
        out IntPtr ppv);

    // Retrieves the attributes of one or more file objects or subfolders. 
    // Return value: error code, if any
    [PreserveSig()]
    new int GetAttributesOf(
        uint cidl,
        [MarshalAs(UnmanagedType.LPArray)]
        IntPtr[] apidl,
        ref ShellFolderGetAttributeObjects rgfInOut);

    // Retrieves an OLE interface that can be used to carry out actions on the
    // specified file objects or folders.
    // Return value: error code, if any
    [PreserveSig()]
    new int GetUIObjectOf(
        IntPtr hwndOwner,
        uint cidl,
        [MarshalAs(UnmanagedType.LPArray)]
        IntPtr[] apidl,
        ref Guid riid,
        IntPtr rgfReserved,
        out IntPtr ppv);

    // Retrieves the display name for the specified file object or subfolder. 
    // Return value: error code, if any
    [PreserveSig()]
    new int GetDisplayNameOf(
        IntPtr pidl,
        SHGetDisplayNames uFlags,
        IntPtr lpName);

    // Sets the display name of a file object or subfolder, changing the item
    // identifier in the process.
    // Return value: error code, if any
    [PreserveSig()]
    new int SetNameOf(
        IntPtr hwnd,
        IntPtr pidl,
        [MarshalAs(UnmanagedType.LPWStr)]
        string pszName,
        SHGetDisplayNames uFlags,
        out IntPtr ppidlOut);

    [PreserveSig()]
    int GetDefaultSearchGUID(out Guid pguid);

    [PreserveSig()]
    int EnumSearches(out IntPtr ppenum);

    [PreserveSig()]
    int GetDefaultColumn(uint dwRes, out uint pSort, out uint pDisplay);

    [PreserveSig()]
    int GetDefaultColumnState(uint iColumn, out uint pcsFlags);

    [PreserveSig()]
    int GetDetailsEx(IntPtr pidl, ref NativeMethods.PropertyKey pscid, [MarshalAs(UnmanagedType.Struct)] out object pv);

    [PreserveSig()]
    int GetDetailsOf(IntPtr pidl, uint iColumn, out ShellDetails psd);

    [PreserveSig()]
    int MapColumnToSCID(uint iColumn, out NativeMethods.PropertyKey pscid);
}
