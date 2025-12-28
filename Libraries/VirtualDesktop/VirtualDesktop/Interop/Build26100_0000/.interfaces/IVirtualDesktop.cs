using System;
using System.Runtime.InteropServices;
using VirtualDesktop.Interop.Build10240;

namespace VirtualDesktop.Interop.Build26100;

[ComImport()]
[Guid("00000000-0000-0000-0000-000000000000") /* replace at runtime */]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IVirtualDesktop
{
    bool IsViewVisible(IApplicationView view);

    Guid GetID();

    HString GetName();

    HString GetWallpaperPath();
    
    bool IsRemote();      
}
