using System;
using System.Runtime.InteropServices;

namespace Explorip.WinAPI.Modeles
{
    [Flags()]
    public enum SHGNO
    {
        NORMAL = 0x0000,
        INFOLDER = 0x0001,
        FOREDITING = 0x1000,
        FORADDRESSBAR = 0x4000,
        FORPARSING = 0x8000
    }

    [Flags()]
    public enum SFGAO : uint
    {
        BROWSABLE = 0x8000000,
        CANCOPY = 1,
        CANDELETE = 0x20,
        CANLINK = 4,
        CANMONIKER = 0x400000,
        CANMOVE = 2,
        CANRENAME = 0x10,
        CAPABILITYMASK = 0x177,
        COMPRESSED = 0x4000000,
        CONTENTSMASK = 0x80000000,
        DISPLAYATTRMASK = 0xfc000,
        DROPTARGET = 0x100,
        ENCRYPTED = 0x2000,
        FILESYSANCESTOR = 0x10000000,
        FILESYSTEM = 0x40000000,
        FOLDER = 0x20000000,
        GHOSTED = 0x8000,
        HASPROPSHEET = 0x40,
        HASSTORAGE = 0x400000,
        HASSUBFOLDER = 0x80000000,
        HIDDEN = 0x80000,
        ISSLOW = 0x4000,
        LINK = 0x10000,
        NEWCONTENT = 0x200000,
        NONENUMERATED = 0x100000,
        READONLY = 0x40000,
        REMOVABLE = 0x2000000,
        SHARE = 0x20000,
        STORAGE = 8,
        STORAGEANCESTOR = 0x800000,
        STORAGECAPMASK = 0x70c50008,
        STREAM = 0x400000,
        VALIDATE = 0x1000000
    }

    [Flags()]
    public enum SHCONTF
    {
        FOLDERS = 0x0020,
        NONFOLDERS = 0x0040,
        INCLUDEHIDDEN = 0x0080,
        INIT_ON_FIRST_NEXT = 0x0100,
        NETPRINTERSRCH = 0x0200,
        SHAREABLE = 0x0400,
        STORAGE = 0x0800,
    }

    [ComImport()]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("000214E6-0000-0000-C000-000000000046")]
    public interface IShellFolder
    {
        // Translates a file object's or folder's display name into an item identifier list.
        // Return value: error code, if any
        [PreserveSig()]
        Int32 ParseDisplayName(
            IntPtr hwnd,
            IntPtr pbc,
            [MarshalAs(UnmanagedType.LPWStr)]
            string pszDisplayName,
            ref uint pchEaten,
            out IntPtr ppidl,
            ref SFGAO pdwAttributes);

        // Allows a client to determine the contents of a folder by creating an item
        // identifier enumeration object and returning its IEnumIDList interface.
        // Return value: error code, if any
        [PreserveSig()]
        Int32 EnumObjects(
            IntPtr hwnd,
            SHCONTF grfFlags,
            out IntPtr enumIDList);

        // Retrieves an IShellFolder object for a subfolder.
        // Return value: error code, if any
        [PreserveSig()]
        Int32 BindToObject(
            IntPtr pidl,
            IntPtr pbc,
            [In] ref Guid riid,
            [MarshalAs(UnmanagedType.Interface)] out object ppv);

        // Requests a pointer to an object's storage interface. 
        // Return value: error code, if any
        [PreserveSig()]
        Int32 BindToStorage(
            IntPtr pidl,
            IntPtr pbc,
            ref Guid riid,
            out IntPtr ppv);

        // Determines the relative order of two file objects or folders, given their
        // item identifier lists. Return value: If this method is successful, the
        // CODE field of the HRESULT contains one of the following values (the code
        // can be retrived using the helper function GetHResultCode): Negative A
        // negative return value indicates that the first item should precede
        // the second (pidl1 < pidl2). 

        // Positive A positive return value indicates that the first item should
        // follow the second (pidl1 > pidl2).  Zero A return value of zero
        // indicates that the two items are the same (pidl1 = pidl2). 
        [PreserveSig()]
        Int32 CompareIDs(
            IntPtr lParam,
            IntPtr pidl1,
            IntPtr pidl2);

        // Requests an object that can be used to obtain information from or interact
        // with a folder object.
        // Return value: error code, if any
        [PreserveSig()]
        Int32 CreateViewObject(
            IntPtr hwndOwner,
            Guid riid,
            [MarshalAs(UnmanagedType.Interface)] out object ppv);

        // Retrieves the attributes of one or more file objects or subfolders. 
        // Return value: error code, if any
        [PreserveSig()]
        Int32 GetAttributesOf(
            uint cidl,
            [MarshalAs(UnmanagedType.LPArray)]
            IntPtr[] apidl,
            ref SFGAO rgfInOut);

        // Retrieves an OLE interface that can be used to carry out actions on the
        // specified file objects or folders.
        // Return value: error code, if any
        [PreserveSig()]
        Int32 GetUIObjectOf(
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
        Int32 GetDisplayNameOf(
            IntPtr pidl,
            SHGNO uFlags,
            IntPtr lpName);

        // Sets the display name of a file object or subfolder, changing the item
        // identifier in the process.
        // Return value: error code, if any
        [PreserveSig()]
        Int32 SetNameOf(
            IntPtr hwnd,
            IntPtr pidl,
            [MarshalAs(UnmanagedType.LPWStr)]
            string pszName,
            SHGNO uFlags,
            out IntPtr ppidlOut);
    }
}
