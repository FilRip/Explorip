using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

using Microsoft.WindowsAPICodePack.Interop;
using Microsoft.WindowsAPICodePack.PropertySystem;
using Microsoft.WindowsAPICodePack.Shell.Common;
using Microsoft.WindowsAPICodePack.Shell.Interop.PropertySystem;
using Microsoft.WindowsAPICodePack.Shell.Interop.Taskbar;

namespace Microsoft.WindowsAPICodePack.Shell.Interop.Common;

internal enum SICHINTF
{
    SICHINT_DISPLAY = 0x00000000,
    SICHINT_CANONICAL = 0x10000000,
    SICHINT_TEST_FILESYSPATH_IF_NOT_EQUAL = 0x20000000,
    SICHINT_ALLFIELDS = unchecked((int)0x80000000)
}

// Disable warning if a method declaration hides another inherited from a parent COM interface
// To successfully import a COM interface, all inherited methods need to be declared again with 
// the exception of those already declared in "IUnknown"

#region COM Interfaces

[ComImport(),
Guid(ShellIidGuid.IModalWindow),
InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IModalWindow
{
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime),
    PreserveSig]
    int Show([In()] IntPtr parent);
}

[ComImport(),
Guid(ShellIidGuid.IShellItem),
InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IShellItem
{
    // Not supported: IBindCtx.
    [PreserveSig()]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    HResult BindToHandler(
        [In()] IntPtr pbc,
        [In()] ref Guid bhid,
        [In()] ref Guid riid,
        [Out(), MarshalAs(UnmanagedType.Interface)] out IShellFolder ppv);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void GetParent([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

    [PreserveSig()]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    HResult GetDisplayName(
        [In()] ShellNativeMethods.ShellItemDesignNameOptions sigdnName,
        out IntPtr ppszName);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void GetAttributes([In()] ShellNativeMethods.ShellFileGetAttributesOptions sfgaoMask, out ShellNativeMethods.ShellFileGetAttributesOptions psfgaoAttribs);

    [PreserveSig()]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    HResult Compare(
        [In(), MarshalAs(UnmanagedType.Interface)] IShellItem psi,
        [In()] SICHINTF hint,
        out int piOrder);
}

[ComImport(),
Guid(ShellIidGuid.IShellItem2),
InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IShellItem2 : IShellItem
{
    // Not supported: IBindCtx.
    [PreserveSig()]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new HResult BindToHandler(
        [In()] IntPtr pbc,
        [In()] ref Guid bhid,
        [In()] ref Guid riid,
        [Out(), MarshalAs(UnmanagedType.Interface)] out IShellFolder ppv);

    [PreserveSig()]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new HResult GetParent([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

    [PreserveSig()]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    HResult GetDisplayName(
        [In()] ShellNativeMethods.ShellItemDesignNameOptions sigdnName,
        [MarshalAs(UnmanagedType.LPWStr)] out string ppszName);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new void GetAttributes([In()] ShellNativeMethods.ShellFileGetAttributesOptions sfgaoMask, out ShellNativeMethods.ShellFileGetAttributesOptions psfgaoAttribs);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void Compare(
        [In(), MarshalAs(UnmanagedType.Interface)] IShellItem psi,
        [In()] uint hint,
        out int piOrder);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), PreserveSig]
    int GetPropertyStore(
        [In()] ShellNativeMethods.GetPropertyStoreOptions Flags,
        [In()] ref Guid riid,
        [Out(), MarshalAs(UnmanagedType.Interface)] out IPropertyStore ppv);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void GetPropertyStoreWithCreateObject([In()] ShellNativeMethods.GetPropertyStoreOptions Flags, [In(), MarshalAs(UnmanagedType.IUnknown)] object punkCreateObject, [In()] ref Guid riid, out IntPtr ppv);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void GetPropertyStoreForKeys([In()] ref PropertyKey rgKeys, [In()] uint cKeys, [In()] ShellNativeMethods.GetPropertyStoreOptions Flags, [In()] ref Guid riid, [Out(), MarshalAs(UnmanagedType.IUnknown)] out IPropertyStore ppv);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void GetPropertyDescriptionList([In()] ref PropertyKey keyType, [In()] ref Guid riid, out IntPtr ppv);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    HResult Update([In(), MarshalAs(UnmanagedType.Interface)] IBindCtx pbc);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void GetProperty([In()] ref PropertyKey key, [Out()] PropVariant ppropvar);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void GetCLSID([In()] ref PropertyKey key, out Guid pclsid);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void GetFileTime([In()] ref PropertyKey key, out System.Runtime.InteropServices.ComTypes.FILETIME pft);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void GetInt32([In()] ref PropertyKey key, out int pi);

    [PreserveSig()]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    HResult GetString([In()] ref PropertyKey key, [MarshalAs(UnmanagedType.LPWStr)] out string ppsz);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void GetUInt32([In()] ref PropertyKey key, out uint pui);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void GetUInt64([In()] ref PropertyKey key, out ulong pull);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void GetBool([In()] ref PropertyKey key, out int pf);
}

[ComImport(),
Guid(ShellIidGuid.IShellItemArray),
InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IShellItemArray
{
    // Not supported: IBindCtx.
    [PreserveSig()]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    HResult BindToHandler(
        [In(), MarshalAs(UnmanagedType.Interface)] IntPtr pbc,
        [In()] ref Guid rbhid,
        [In()] ref Guid riid,
        out IntPtr ppvOut);

    [PreserveSig()]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    HResult GetPropertyStore(
        [In()] int Flags,
        [In()] ref Guid riid,
        out IntPtr ppv);

    [PreserveSig()]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    HResult GetPropertyDescriptionList(
        [In()] ref PropertyKey keyType,
        [In()] ref Guid riid,
        out IntPtr ppv);

    [PreserveSig()]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    HResult GetAttributes(
        [In()] ShellNativeMethods.ShellItemAttributeOptions dwAttribFlags,
        [In()] ShellNativeMethods.ShellFileGetAttributesOptions sfgaoMask,
        out ShellNativeMethods.ShellFileGetAttributesOptions psfgaoAttribs);

    [PreserveSig()]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    HResult GetCount(out uint pdwNumItems);

    [PreserveSig()]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    HResult GetItemAt(
        [In()] uint dwIndex,
        [MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

    // Not supported: IEnumShellItems (will use GetCount and GetItemAt instead).
    [PreserveSig()]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    HResult EnumItems([MarshalAs(UnmanagedType.Interface)] out IntPtr ppenumShellItems);
}

[ComImport(),
Guid(ShellIidGuid.IShellLibrary),
InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IShellLibrary
{
    [PreserveSig()]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    HResult LoadLibraryFromItem(
        [In(), MarshalAs(UnmanagedType.Interface)] IShellItem library,
        [In()] AccessModes grfMode);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void LoadLibraryFromKnownFolder(
        [In()] ref Guid knownfidLibrary,
        [In()] AccessModes grfMode);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void AddFolder([In(), MarshalAs(UnmanagedType.Interface)] IShellItem location);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void RemoveFolder([In(), MarshalAs(UnmanagedType.Interface)] IShellItem location);

    [PreserveSig()]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    HResult GetFolders(
        [In()] ShellNativeMethods.LibraryFolderFilter lff,
        [In()] ref Guid riid,
        [MarshalAs(UnmanagedType.Interface)] out IShellItemArray ppv);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void ResolveFolder(
        [In(), MarshalAs(UnmanagedType.Interface)] IShellItem folderToResolve,
        [In()] uint timeout,
        [In()] ref Guid riid,
        [MarshalAs(UnmanagedType.Interface)] out IShellItem ppv);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void GetDefaultSaveFolder(
        [In()] ShellNativeMethods.DefaultSaveFolderType dsft,
        [In()] ref Guid riid,
        [MarshalAs(UnmanagedType.Interface)] out IShellItem ppv);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void SetDefaultSaveFolder(
        [In()] ShellNativeMethods.DefaultSaveFolderType dsft,
        [In(), MarshalAs(UnmanagedType.Interface)] IShellItem si);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void GetOptions(
        out ShellNativeMethods.LibraryOptions lofOptions);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void SetOptions(
        [In()] ShellNativeMethods.LibraryOptions lofMask,
        [In()] ShellNativeMethods.LibraryOptions lofOptions);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void GetFolderType(out Guid ftid);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void SetFolderType([In()] ref Guid ftid);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void GetIcon([MarshalAs(UnmanagedType.LPWStr)] out string icon);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void SetIcon([In(), MarshalAs(UnmanagedType.LPWStr)] string icon);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void Commit();

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void Save(
        [In(), MarshalAs(UnmanagedType.Interface)] IShellItem folderToSaveIn,
        [In(), MarshalAs(UnmanagedType.LPWStr)] string libraryName,
        [In()] ShellNativeMethods.LibrarySaveOptions lsf,
        [MarshalAs(UnmanagedType.Interface)] out IShellItem2 savedTo);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void SaveInKnownFolder(
        [In()] ref Guid kfidToSaveIn,
        [In(), MarshalAs(UnmanagedType.LPWStr)] string libraryName,
        [In()] ShellNativeMethods.LibrarySaveOptions lsf,
        [MarshalAs(UnmanagedType.Interface)] out IShellItem2 savedTo);
};

[ComImport()]
[Guid("bcc18b79-ba16-442f-80c4-8a59c30c463b")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
interface IShellItemImageFactory
{
    [PreserveSig()]
    HResult GetImage(
    [In(), MarshalAs(UnmanagedType.Struct)] CoreNativeMethods.Size size,
    [In()] ShellNativeMethods.SIIGBF flags,
    [Out()] out IntPtr phbm);
}

[ComImport(),
Guid(ShellIidGuid.IThumbnailCache),
InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
interface IThumbnailCache
{
    void GetThumbnail([In()] IShellItem pShellItem,
    [In()] uint cxyRequestedThumbSize,
    [In()] ShellNativeMethods.ThumbnailOptions flags,
    [Out()] out ISharedBitmap ppvThumb,
    [Out()] out ShellNativeMethods.ThumbnailCacheOptions pOutFlags,
    [Out()] ShellNativeMethods.ThumbnailId pThumbnailID);

    void GetThumbnailByID([In()] ShellNativeMethods.ThumbnailId thumbnailID,
    [In()] uint cxyRequestedThumbSize,
    [Out()] out ISharedBitmap ppvThumb,
    [Out()] out ShellNativeMethods.ThumbnailCacheOptions pOutFlags);
}

[ComImport(),
Guid(ShellIidGuid.ISharedBitmap),
InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
interface ISharedBitmap
{
    void GetSharedBitmap([Out()] out IntPtr phbm);
    void GetSize([Out()] out CoreNativeMethods.Size pSize);
    void GetFormat([Out()] out ThumbnailAlphaType pat);
    void InitializeBitmap([In()] IntPtr hbm, [In()] ThumbnailAlphaType wtsAT);
    void Detach([Out()] out IntPtr phbm);
}
[ComImport(),
Guid(ShellIidGuid.IShellFolder),
InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
ComConversionLoss]
internal interface IShellFolder
{
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void ParseDisplayName(IntPtr hwnd, [In(), MarshalAs(UnmanagedType.Interface)] IBindCtx pbc, [In(), MarshalAs(UnmanagedType.LPWStr)] string pszDisplayName, [In(), Out] ref uint pchEaten, [Out()] IntPtr ppidl, [In(), Out] ref uint pdwAttributes);
    [PreserveSig()]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    HResult EnumObjects([In()] IntPtr hwnd, [In()] ShellNativeMethods.ShellFolderEnumerationOptions grfFlags, [MarshalAs(UnmanagedType.Interface)] out IEnumIDList ppenumIDList);

    [PreserveSig()]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    HResult BindToObject([In()] IntPtr pidl, /*[In(), MarshalAs(UnmanagedType.Interface)] IBindCtx*/ IntPtr pbc, [In()] ref Guid riid, [Out(), MarshalAs(UnmanagedType.Interface)] out IShellFolder ppv);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void BindToStorage([In()] ref IntPtr pidl, [In(), MarshalAs(UnmanagedType.Interface)] IBindCtx pbc, [In()] ref Guid riid, out IntPtr ppv);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void CompareIDs([In()] IntPtr lParam, [In()] ref IntPtr pidl1, [In()] ref IntPtr pidl2);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void CreateViewObject([In()] IntPtr hwndOwner, [In()] ref Guid riid, out IntPtr ppv);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void GetAttributesOf([In()] uint cidl, [In()] IntPtr apidl, [In(), Out] ref uint rgfInOut);


    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void GetUIObjectOf([In()] IntPtr hwndOwner, [In()] uint cidl, [In()] IntPtr apidl, [In()] ref Guid riid, [In(), Out] ref uint rgfReserved, out IntPtr ppv);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void GetDisplayNameOf([In()] ref IntPtr pidl, [In()] uint uFlags, out IntPtr pName);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void SetNameOf([In()] IntPtr hwnd, [In()] ref IntPtr pidl, [In(), MarshalAs(UnmanagedType.LPWStr)] string pszName, [In()] uint uFlags, [Out()] IntPtr ppidlOut);
}

[ComImport(),
Guid(ShellIidGuid.IShellFolder2),
InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
ComConversionLoss]
internal interface IShellFolder2 : IShellFolder
{
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new void ParseDisplayName([In()] IntPtr hwnd, [In(), MarshalAs(UnmanagedType.Interface)] IBindCtx pbc, [In(), MarshalAs(UnmanagedType.LPWStr)] string pszDisplayName, [In(), Out] ref uint pchEaten, [Out()] IntPtr ppidl, [In(), Out] ref uint pdwAttributes);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new void EnumObjects([In()] IntPtr hwnd, [In()] ShellNativeMethods.ShellFolderEnumerationOptions grfFlags, [MarshalAs(UnmanagedType.Interface)] out IEnumIDList ppenumIDList);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new void BindToObject([In()] IntPtr pidl, /*[In(), MarshalAs(UnmanagedType.Interface)] IBindCtx*/ IntPtr pbc, [In()] ref Guid riid, [Out(), MarshalAs(UnmanagedType.Interface)] out IShellFolder ppv);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new void BindToStorage([In()] ref IntPtr pidl, [In(), MarshalAs(UnmanagedType.Interface)] IBindCtx pbc, [In()] ref Guid riid, out IntPtr ppv);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new void CompareIDs([In()] IntPtr lParam, [In()] ref IntPtr pidl1, [In()] ref IntPtr pidl2);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new void CreateViewObject([In()] IntPtr hwndOwner, [In()] ref Guid riid, out IntPtr ppv);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new void GetAttributesOf([In()] uint cidl, [In()] IntPtr apidl, [In(), Out] ref uint rgfInOut);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new void GetUIObjectOf([In()] IntPtr hwndOwner, [In()] uint cidl, [In()] IntPtr apidl, [In()] ref Guid riid, [In(), Out] ref uint rgfReserved, out IntPtr ppv);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new void GetDisplayNameOf([In()] ref IntPtr pidl, [In()] uint uFlags, out IntPtr pName);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new void SetNameOf([In()] IntPtr hwnd, [In()] ref IntPtr pidl, [In(), MarshalAs(UnmanagedType.LPWStr)] string pszName, [In()] uint uFlags, [Out()] IntPtr ppidlOut);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void GetDefaultSearchGUID(out Guid pguid);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void EnumSearches([Out()] out IntPtr ppenum);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void GetDefaultColumn([In()] uint dwRes, out uint pSort, out uint pDisplay);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void GetDefaultColumnState([In()] uint iColumn, out uint pcsFlags);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void GetDetailsEx([In()] ref IntPtr pidl, [In()] ref PropertyKey pscid, [MarshalAs(UnmanagedType.Struct)] out object pv);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void GetDetailsOf([In()] ref IntPtr pidl, [In()] uint iColumn, out IntPtr psd);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void MapColumnToSCID([In()] uint iColumn, out PropertyKey pscid);
}

[ComImport(),
Guid(ShellIidGuid.IEnumIDList),
InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IEnumIDList
{
    [PreserveSig()]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    HResult Next(uint celt, out IntPtr rgelt, out uint pceltFetched);

    [PreserveSig()]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    HResult Skip([In()] uint celt);

    [PreserveSig()]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    HResult Reset();

    [PreserveSig()]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    HResult Clone([MarshalAs(UnmanagedType.Interface)] out IEnumIDList ppenum);
}

[ComImport(),
Guid(ShellIidGuid.IShellLinkW),
InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IShellLinkW
{
    void GetPath(
        [Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile,
        int cchMaxPath,
        //ref _WIN32_FIND_DATAW pfd,
        IntPtr pfd,
        uint fFlags);
    void GetIDList(out IntPtr ppidl);
    void SetIDList(IntPtr pidl);
    void GetDescription(
        [Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile,
        int cchMaxName);
    void SetDescription(
        [MarshalAs(UnmanagedType.LPWStr)] string pszName);
    void GetWorkingDirectory(
        [Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir,
        int cchMaxPath
        );
    void SetWorkingDirectory(
        [MarshalAs(UnmanagedType.LPWStr)] string pszDir);
    void GetArguments(
        [Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs,
        int cchMaxPath);
    void SetArguments(
        [MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
    void GetHotKey(out short wHotKey);
    void SetHotKey(short wHotKey);
    void GetShowCmd(out uint iShowCmd);
    void SetShowCmd(uint iShowCmd);
    void GetIconLocation(
        [Out(), MarshalAs(UnmanagedType.LPWStr)] out StringBuilder pszIconPath,
        int cchIconPath,
        out int iIcon);
    void SetIconLocation(
        [MarshalAs(UnmanagedType.LPWStr)] string pszIconPath,
        int iIcon);
    void SetRelativePath(
        [MarshalAs(UnmanagedType.LPWStr)] string pszPathRel,
        uint dwReserved);
    void Resolve(IntPtr hwnd, uint fFlags);
    void SetPath(
        [MarshalAs(UnmanagedType.LPWStr)] string pszFile);
}

[ComImport(),
Guid(ShellIidGuid.CShellLink),
ClassInterface(ClassInterfaceType.None)]
internal class CShellLink { }

// Summary:
//     Provides the managed definition of the IPersistStream interface, with functionality
//     from IPersist.
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("00000109-0000-0000-C000-000000000046")]
internal interface IPersistStream
{
    // Summary:
    //     Retrieves the class identifier (CLSID) of an object.
    //
    // Parameters:
    //   pClassID:
    //     When this method returns, contains a reference to the CLSID. This parameter
    //     is passed uninitialized.
    [PreserveSig()]
    void GetClassID(out Guid pClassID);
    //
    // Summary:
    //     Checks an object for changes since it was last saved to its current file.
    //
    // Returns:
    //     S_OK if the file has changed since it was last saved; S_FALSE if the file
    //     has not changed since it was last saved.
    [PreserveSig()]
    HResult IsDirty();

    [PreserveSig()]
    HResult Load([In(), MarshalAs(UnmanagedType.Interface)] IStream stm);

    [PreserveSig()]
    HResult Save([In(), MarshalAs(UnmanagedType.Interface)] IStream stm, bool fRemember);

    [PreserveSig()]
    HResult GetSizeMax(out ulong cbSize);
}

[ComImport(),
Guid(ShellIidGuid.ICondition),
InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface ICondition : IPersistStream
{
    // Summary:
    //     Retrieves the class identifier (CLSID) of an object.
    //
    // Parameters:
    //   pClassID:
    //     When this method returns, contains a reference to the CLSID. This parameter
    //     is passed uninitialized.
    [PreserveSig()]
    new void GetClassID(out Guid pClassID);
    //
    // Summary:
    //     Checks an object for changes since it was last saved to its current file.
    //
    // Returns:
    //     S_OK if the file has changed since it was last saved; S_FALSE if the file
    //     has not changed since it was last saved.
    [PreserveSig()]
    new HResult IsDirty();

    [PreserveSig()]
    new HResult Load([In(), MarshalAs(UnmanagedType.Interface)] IStream stm);

    [PreserveSig()]
    new HResult Save([In(), MarshalAs(UnmanagedType.Interface)] IStream stm, bool fRemember);

    [PreserveSig()]
    new HResult GetSizeMax(out ulong cbSize);

    // For any node, return what kind of node it is.
    [PreserveSig()]
    HResult GetConditionType([Out()] out SearchConditionType pNodeType);

    // riid must be IID_IEnumUnknown, IID_IEnumVARIANT or IID_IObjectArray, or in the case of a negation node IID_ICondition.
    // If this is a leaf node, E_FAIL will be returned.
    // If this is a negation node, then if riid is IID_ICondition, *ppv will be set to a single ICondition, otherwise an enumeration of one.
    // If this is a conjunction or a disjunction, *ppv will be set to an enumeration of the subconditions.
    [PreserveSig()]
    HResult GetSubConditions([In()] ref Guid riid, [Out(), MarshalAs(UnmanagedType.Interface)] out object ppv);

    // If this is not a leaf node, E_FAIL will be returned.
    // Retrieve the property name, operation and value from the leaf node.
    // Any one of ppszPropertyName, pcop and ppropvar may be NULL.
    [PreserveSig()]
    HResult GetComparisonInfo(
        [Out(), MarshalAs(UnmanagedType.LPWStr)] out string ppszPropertyName,
        [Out()] out SearchConditionOperation pcop,
        [Out()] PropVariant ppropvar);

    // If this is not a leaf node, E_FAIL will be returned.
    // *ppszValueTypeName will be set to the semantic type of the value, or to NULL if this is not meaningful.
    [PreserveSig()]
    HResult GetValueType([Out(), MarshalAs(UnmanagedType.LPWStr)] out string ppszValueTypeName);

    // If this is not a leaf node, E_FAIL will be returned.
    // If the value of the leaf node is VT_EMPTY, *ppszNormalization will be set to an empty string.
    // If the value is a string (VT_LPWSTR, VT_BSTR or VT_LPSTR), then *ppszNormalization will be set to a
    // character-normalized form of the value.
    // Otherwise, *ppszNormalization will be set to some (character-normalized) string representation of the value.
    [PreserveSig()]
    HResult GetValueNormalization([Out(), MarshalAs(UnmanagedType.LPWStr)] out string ppszNormalization);

    // Return information about what parts of the input produced the property, the operation and the value.
    // Any one of ppPropertyTerm, ppOperationTerm and ppValueTerm may be NULL.
    // For a leaf node returned by the parser, the position information of each IRichChunk identifies the tokens that
    // contributed the property/operation/value, the string value is the corresponding part of the input string, and
    // the PROPVARIANT is VT_EMPTY.
    [PreserveSig()]
    HResult GetInputTerms([Out()] out IRichChunk ppPropertyTerm, [Out()] out IRichChunk ppOperationTerm, [Out()] out IRichChunk ppValueTerm);

    // Make a deep copy of this ICondition.
    [PreserveSig()]
    HResult Clone([Out()] out ICondition ppc);
};

[ComImport(),
Guid(ShellIidGuid.IRichChunk),
InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IRichChunk
{
    // The position *pFirstPos is zero-based.
    // Any one of pFirstPos, pLength, ppsz and pValue may be NULL.
    [PreserveSig()]
    HResult GetData(/*[Out(), annotation("__out_opt")] ULONG* pFirstPos, [Out(), annotation("__out_opt")] ULONG* pLength, [Out(), annotation("__deref_opt_out_opt")] LPWSTR* ppsz, [Out(), annotation("__out_opt")] PROPVARIANT* pValue*/);
}

[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid(ShellIidGuid.IEnumUnknown)]
internal interface IEnumUnknown
{
    [PreserveSig()]
    HResult Next(uint requestedNumber, ref IntPtr buffer, ref uint fetchedNumber);
    [PreserveSig()]
    HResult Skip(uint number);
    [PreserveSig()]
    HResult Reset();
    [PreserveSig()]
    HResult Clone(out IEnumUnknown result);
}


[ComImport(),
Guid(ShellIidGuid.IConditionFactory),
InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IConditionFactory
{
    [PreserveSig()]
    HResult MakeNot([In()] ICondition pcSub, [In()] bool fSimplify, [Out()] out ICondition ppcResult);

    [PreserveSig()]
    HResult MakeAndOr([In()] SearchConditionType ct, [In()] IEnumUnknown peuSubs, [In()] bool fSimplify, [Out()] out ICondition ppcResult);

    [PreserveSig()]
    HResult MakeLeaf(
        [In(), MarshalAs(UnmanagedType.LPWStr)] string pszPropertyName,
        [In()] SearchConditionOperation cop,
        [In(), MarshalAs(UnmanagedType.LPWStr)] string pszValueType,
        [In()] PropVariant ppropvar,
        IRichChunk richChunk1,
        IRichChunk richChunk2,
        IRichChunk richChunk3,
        [In()] bool fExpand,
        [Out()] out ICondition ppcResult);

    [PreserveSig()]
    HResult Resolve(/*[In()] ICondition pc, [In()] STRUCTURED_QUERY_RESOLVE_OPTION sqro, [In()] ref SYSTEMTIME pstReferenceTime, [Out()] out ICondition ppcResolved*/);

};

[ComImport(),
Guid(ShellIidGuid.IConditionFactory),
CoClass(typeof(ConditionFactoryCoClass))]
internal interface INativeConditionFactory : IConditionFactory
{
}

[ComImport(),
ClassInterface(ClassInterfaceType.None),
TypeLibType(TypeLibTypeFlags.FCanCreate),
Guid(ShellClSidGuid.ConditionFactory)]
internal class ConditionFactoryCoClass
{
}



[ComImport(),
Guid(ShellIidGuid.ISearchFolderItemFactory),
InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface ISearchFolderItemFactory
{
    [PreserveSig()]
    HResult SetDisplayName([In(), MarshalAs(UnmanagedType.LPWStr)] string pszDisplayName);

    [PreserveSig()]
    HResult SetFolderTypeID([In()] Guid ftid);

    [PreserveSig()]
    HResult SetFolderLogicalViewMode([In()] FolderLogicalViewMode flvm);

    [PreserveSig()]
    HResult SetIconSize([In()] int iIconSize);

    [PreserveSig()]
    HResult SetVisibleColumns([In()] uint cVisibleColumns, [In(), MarshalAs(UnmanagedType.LPArray)] PropertyKey[] rgKey);

    [PreserveSig()]
    HResult SetSortColumns([In()] uint cSortColumns, [In(), MarshalAs(UnmanagedType.LPArray)] SortColumn[] rgSortColumns);

    [PreserveSig()]
    HResult SetGroupColumn([In()] ref PropertyKey keyGroup);

    [PreserveSig()]
    HResult SetStacks([In()] uint cStackKeys, [In(), MarshalAs(UnmanagedType.LPArray)] PropertyKey[] rgStackKeys);

    [PreserveSig()]
    HResult SetScope([In(), MarshalAs(UnmanagedType.Interface)] IShellItemArray ppv);

    [PreserveSig()]
    HResult SetCondition([In()] ICondition pCondition);

    [PreserveSig()]
    int GetShellItem(ref Guid riid, [Out(), MarshalAs(UnmanagedType.Interface)] out IShellItem ppv);

    [PreserveSig()]
    HResult GetIDList([Out()] IntPtr ppidl);
};

[ComImport(),
Guid(ShellIidGuid.ISearchFolderItemFactory),
CoClass(typeof(SearchFolderItemFactoryCoClass))]
internal interface INativeSearchFolderItemFactory : ISearchFolderItemFactory
{
}

[ComImport(),
ClassInterface(ClassInterfaceType.None),
TypeLibType(TypeLibTypeFlags.FCanCreate),
Guid(ShellClSidGuid.SearchFolderItemFactory)]
internal class SearchFolderItemFactoryCoClass
{
}

[ComImport(),
Guid(ShellIidGuid.IQuerySolution),
InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
interface IQuerySolution : IConditionFactory
{
    [PreserveSig()]
    new HResult MakeNot([In()] ICondition pcSub, [In()] bool fSimplify, [Out()] out ICondition ppcResult);

    [PreserveSig()]
    new HResult MakeAndOr([In()] SearchConditionType ct, [In()] IEnumUnknown peuSubs, [In()] bool fSimplify, [Out()] out ICondition ppcResult);

    [PreserveSig()]
    new HResult MakeLeaf(
        [In(), MarshalAs(UnmanagedType.LPWStr)] string pszPropertyName,
        [In()] SearchConditionOperation cop,
        [In(), MarshalAs(UnmanagedType.LPWStr)] string pszValueType,
        [In()] PropVariant ppropvar,
        IRichChunk richChunk1,
        IRichChunk richChunk2,
        IRichChunk richChunk3,
        [In()] bool fExpand,
        [Out()] out ICondition ppcResult);

    [PreserveSig()]
    new HResult Resolve(/*[In()] ICondition pc, [In()] int sqro, [In()] ref SYSTEMTIME pstReferenceTime, [Out()] out ICondition ppcResolved*/);

    // Retrieve the condition tree and the "main type" of the solution.
    // ppQueryNode and ppMainType may be NULL.
    [PreserveSig()]
    HResult GetQuery([Out(), MarshalAs(UnmanagedType.Interface)] out ICondition ppQueryNode, [Out(), MarshalAs(UnmanagedType.Interface)] out IEntity ppMainType);

    // Identify parts of the input string not accounted for.
    // Each parse error is represented by an IRichChunk where the position information
    // reflect token counts, the string is NULL and the value is a VT_I4
    // where lVal is from the ParseErrorType enumeration. The valid
    // values for riid are IID_IEnumUnknown and IID_IEnumVARIANT.
    [PreserveSig()]
    HResult GetErrors([In()] ref Guid riid, [Out()] out /* void** */ IntPtr ppParseErrors);

    // Report the query string, how it was tokenized and what LCID and word breaker were used (for recognizing keywords).
    // ppszInputString, ppTokens, pLocale and ppWordBreaker may be NULL.
    [PreserveSig()]
    HResult GetLexicalData([MarshalAs(UnmanagedType.LPWStr)] out string ppszInputString, [Out()] /* ITokenCollection** */ out IntPtr ppTokens, [Out()] out uint plcid, [Out()] /* IUnknown** */ out IntPtr ppWordBreaker);
}

[ComImport(),
Guid(ShellIidGuid.IQueryParser),
InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IQueryParser
{
    // Parse parses an input string, producing a query solution.
    // pCustomProperties should be an enumeration of IRichChunk objects, one for each custom property
    // the application has recognized. pCustomProperties may be NULL, equivalent to an empty enumeration.
    // For each IRichChunk, the position information identifies the character span of the custom property,
    // the string value should be the name of an actual property, and the PROPVARIANT is completely ignored.
    [PreserveSig()]
    HResult Parse([In(), MarshalAs(UnmanagedType.LPWStr)] string pszInputString, [In()] IEnumUnknown pCustomProperties, [Out()] out IQuerySolution ppSolution);

    // Set a single option. See STRUCTURED_QUERY_SINGLE_OPTION above.
    [PreserveSig()]
    HResult SetOption([In()] StructuredQuerySingleOption option, [In()] PropVariant pOptionValue);

    [PreserveSig()]
    HResult GetOption([In()] StructuredQuerySingleOption option, [Out()] PropVariant pOptionValue);

    // Set a multi option. See STRUCTURED_QUERY_MULTIOPTION above.
    [PreserveSig()]
    HResult SetMultiOption([In()] StructuredQueryMultipleOption option, [In(), MarshalAs(UnmanagedType.LPWStr)] string pszOptionKey, [In()] PropVariant pOptionValue);

    // Get a schema provider for browsing the currently loaded schema.
    [PreserveSig()]
    HResult GetSchemaProvider([Out()] out /*ISchemaProvider*/ IntPtr ppSchemaProvider);

    // Restate a condition as a query string according to the currently selected syntax.
    // The parameter fUseEnglish is reserved for future use; must be FALSE.
    [PreserveSig()]
    HResult RestateToString([In()] ICondition pCondition, [In()] bool fUseEnglish, [Out(), MarshalAs(UnmanagedType.LPWStr)] out string ppszQueryString);

    // Parse a condition for a given property. It can be anything that would go after 'PROPERTY:' in an AQS expession.
    [PreserveSig()]
    HResult ParsePropertyValue([In(), MarshalAs(UnmanagedType.LPWStr)] string pszPropertyName, [In(), MarshalAs(UnmanagedType.LPWStr)] string pszInputString, [Out()] out IQuerySolution ppSolution);

    // Restate a condition for a given property. If the condition contains a leaf with any other property name, or no property name at all,
    // E_INVALIDARG will be returned.
    [PreserveSig()]
    HResult RestatePropertyValueToString([In()] ICondition pCondition, [In()] bool fUseEnglish, [Out(), MarshalAs(UnmanagedType.LPWStr)] out string ppszPropertyName, [Out(), MarshalAs(UnmanagedType.LPWStr)] out string ppszQueryString);
}

[ComImport(),
Guid(ShellIidGuid.IQueryParserManager),
InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IQueryParserManager
{
    // Create a query parser loaded with the schema for a certain catalog localize to a certain language, and initialized with
    // standard defaults. One valid value for riid is IID_IQueryParser.
    [PreserveSig()]
    HResult CreateLoadedParser([In(), MarshalAs(UnmanagedType.LPWStr)] string pszCatalog, [In()] ushort langidForKeywords, [In()] ref Guid riid, [Out()] out IQueryParser ppQueryParser);

    // In addition to setting AQS/NQS and automatic wildcard for the given query parser, this sets up standard named entity handlers and
    // sets the keyboard locale as locale for word breaking.
    [PreserveSig()]
    HResult InitializeOptions([In()] bool fUnderstandNQS, [In()] bool fAutoWildCard, [In()] IQueryParser pQueryParser);

    // Change one of the settings for the query parser manager, such as the name of the schema binary, or the location of the localized and unlocalized
    // schema binaries. By default, the settings point to the schema binaries used by Windows Shell.
    [PreserveSig()]
    HResult SetOption([In()] QueryParserManagerOption option, [In()] PropVariant pOptionValue);

};

[ComImport(),
Guid(ShellIidGuid.IQueryParserManager),
CoClass(typeof(QueryParserManagerCoClass))]
internal interface INativeQueryParserManager : IQueryParserManager
{
}

[ComImport(),
ClassInterface(ClassInterfaceType.None),
TypeLibType(TypeLibTypeFlags.FCanCreate),
Guid(ShellClSidGuid.QueryParserManager)]
internal class QueryParserManagerCoClass
{
}

[ComImport(),
Guid("24264891-E80B-4fd3-B7CE-4FF2FAE8931F"),
InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IEntity
{
    [return: MarshalAs(UnmanagedType.LPWStr)]
    string Name();

    int Base(out IEntity pBaseEntity);

    [return: MarshalAs(UnmanagedType.IUnknown)]
    object Relationships(in Guid riid);

    IRelationship GetRelationship([In, MarshalAs(UnmanagedType.LPWStr)] string pszRelationName);

    [return: MarshalAs(UnmanagedType.IUnknown)]
    object MetaData(in Guid riid);

    [return: MarshalAs(UnmanagedType.IUnknown)]
    object NamedEntities(in Guid riid);

    INamedEntity GetNamedEntity([In, MarshalAs(UnmanagedType.LPWStr)] string pszValue);

    [return: MarshalAs(UnmanagedType.LPWStr)]
    string DefaultPhrase();
}

[ComImport(), Guid("B72F8FD8-0FAB-4dd9-BDBF-245A6CE1485B"), ClassInterface(ClassInterfaceType.None)]
public class QueryParser { }

[ComImport(), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("ABDBD0B1-7D54-49fb-AB5C-BFF4130004CD"), CoClass(typeof(QueryParser))]
public interface INamedEntity
{
    [return: MarshalAs(UnmanagedType.LPWStr)]
    string GetValue();

    [return: MarshalAs(UnmanagedType.LPWStr)]
    string DefaultPhrase();
}

[ComImport(), Guid("2769280B-5108-498c-9C7F-A51239B63147"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IRelationship
{
    [return: MarshalAs(UnmanagedType.LPWStr)]
    string Name();

    [return: MarshalAs(UnmanagedType.Bool)]
    bool IsReal();

    IEntity Destination();

    [return: MarshalAs(UnmanagedType.IUnknown)]
    object MetaData(in Guid riid);

    [return: MarshalAs(UnmanagedType.LPWStr)]
    string DefaultPhrase();
}

#endregion
