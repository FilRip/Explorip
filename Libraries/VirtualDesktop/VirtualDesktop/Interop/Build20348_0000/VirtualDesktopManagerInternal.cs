using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using VirtualDesktop.Interop;
using VirtualDesktop.Interop.Proxy;

namespace VirtualDesktop.Interop.Build20348;

internal class VirtualDesktopManagerInternal(ComInterfaceAssembly assembly, ComWrapperFactory factory) : ComWrapperBase<IVirtualDesktopManagerInternal>(assembly, Clsid.VirtualDesktopManagerInternal), IVirtualDesktopManagerInternal
{
    private readonly ComWrapperFactory _factory = factory;

    public IEnumerable<IVirtualDesktop> GetDesktops()
    {
        IObjectArray? array = this.InvokeMethod<IObjectArray>(Args(IntPtr.Zero));
        if (array == null) yield break;

        uint count = array.GetCount();
        Type vdType = this.ComInterfaceAssembly.GetType(nameof(IVirtualDesktop));

        for (uint i = 0u; i < count; i++)
        {
            object ppvObject = array.GetAt(i, vdType.GUID);
            yield return new VirtualDesktop(this.ComInterfaceAssembly, ppvObject);
        }
    }

    public IVirtualDesktop GetCurrentDesktop()
        => this.InvokeMethodAndWrap(Args(IntPtr.Zero));

    public IVirtualDesktop GetAdjacentDesktop(IVirtualDesktop pDesktopReference, AdjacentDesktop uDirection)
        => this.InvokeMethodAndWrap(Args(((VirtualDesktop)pDesktopReference).ComObject, uDirection));

    public IVirtualDesktop FindDesktop(Guid desktopId)
        => this.InvokeMethodAndWrap(Args(desktopId));

    public IVirtualDesktop CreateDesktop()
        => this.InvokeMethodAndWrap(Args(IntPtr.Zero));

    public void SwitchDesktop(IVirtualDesktop desktop)
        => this.InvokeMethod(Args(IntPtr.Zero, ((VirtualDesktop)desktop).ComObject));


    public void MoveDesktop(IVirtualDesktop pMove, int nIndex)
        => throw new NotSupportedException();

    public void RemoveDesktop(IVirtualDesktop pRemove, IVirtualDesktop pFallbackDesktop)
        => this.InvokeMethod(Args(((VirtualDesktop)pRemove).ComObject, ((VirtualDesktop)pFallbackDesktop).ComObject));

    public void MoveViewToDesktop(IntPtr hWnd, IVirtualDesktop desktop)
        => this.InvokeMethod(Args(this._factory.ApplicationViewFromHwnd(hWnd).ComObject, ((VirtualDesktop)desktop).ComObject));

    public void SetDesktopName(IVirtualDesktop desktop, string name)
        => this.InvokeMethod(Args(((VirtualDesktop)desktop).ComObject, new HString(name)));

    public void SetDesktopWallpaper(IVirtualDesktop desktop, string path)
    {
        //not available in server 2022
    }

    public void UpdateWallpaperPathForAllDesktops(string path)
    {
        //not available in server 2022
    }

    private VirtualDesktop InvokeMethodAndWrap(object?[]? parameters = null, [CallerMemberName] string methodName = "")
        => new(this.ComInterfaceAssembly, this.InvokeMethod<object>(parameters, methodName) ?? throw new VirtualDesktopException("Failed to get IVirtualDesktop instance."));
}
