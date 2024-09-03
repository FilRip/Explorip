using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Microsoft.WindowsAPICodePack.NetworkList;

namespace Microsoft.WindowsAPICodePack.Interop.NetworkList;

[ComImport]
[TypeLibType(0x1040)]
[Guid("DCB00005-570F-4A9B-8D69-199FDBA5723B")]
internal interface INetworkConnection
{
    [return: MarshalAs(UnmanagedType.Interface)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    INetwork GetNetwork();

    bool IsConnectedToInternet
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        get;
    }

    bool IsConnected
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        get;
    }

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    ConnectivityStates GetConnectivity();

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    Guid GetConnectionId();

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    Guid GetAdapterId();

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    DomainType GetDomainType();
}