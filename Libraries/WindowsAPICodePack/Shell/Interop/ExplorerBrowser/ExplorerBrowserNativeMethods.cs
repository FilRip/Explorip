using System;
using System.Runtime.InteropServices;
using System.Security;

using Microsoft.WindowsAPICodePack.Interop;

namespace Microsoft.WindowsAPICodePack.Shell.Interop.ExplorerBrowser;

/// <summary>
/// Internal class that contains interop declarations for 
/// functions that are not benign and are performance critical. 
/// </summary>
[SuppressUnmanagedCodeSecurity]
internal static class ExplorerBrowserNativeMethods
{
    [DllImport("SHLWAPI.DLL", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern HResult IUnknown_SetSite(
        [In(), MarshalAs(UnmanagedType.IUnknown)] object punk,
        [In(), MarshalAs(UnmanagedType.IUnknown)] object punkSite);


    [DllImport("SHLWAPI.DLL", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern HResult ConnectToConnectionPoint(
        [In(), MarshalAs(UnmanagedType.IUnknown)] object punk,
        ref Guid riidEvent,
        [MarshalAs(UnmanagedType.Bool)] bool fConnect,
        [In(), MarshalAs(UnmanagedType.IUnknown)] object punkTarget,
        ref uint pdwCookie,
        ref IntPtr ppcpOut);

}
