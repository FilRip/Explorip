using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using VirtualDesktop.Interop;
using VirtualDesktop.Interop.Proxy;

namespace VirtualDesktop.Interop.Build10240;

internal class VirtualDesktopManagerInternal(ComInterfaceAssembly assembly, ComWrapperFactory factory) : ComWrapperBase<IVirtualDesktopManagerInternal>(assembly, Clsid.VirtualDesktopManagerInternal), IVirtualDesktopManagerInternal
{
    private readonly ComWrapperFactory _factory = factory;

    public IEnumerable<IVirtualDesktop> GetDesktops()
    {
        IObjectArray? array = this.InvokeMethod<IObjectArray>();
        if (array == null)
            yield break;

        uint count = array.GetCount();
        Type vdType = this.ComInterfaceAssembly.GetType(nameof(IVirtualDesktop));

        for (uint i = 0u; i < count; i++)
        {
            object ppvObject = array.GetAt(i, vdType.GUID);
            yield return new VirtualDesktop(this.ComInterfaceAssembly, ppvObject);
        }
    }

    public IVirtualDesktop GetCurrentDesktop()
        => this.InvokeMethodAndWrap();

    public IVirtualDesktop GetAdjacentDesktop(IVirtualDesktop pDesktopReference, AdjacentDesktop uDirection)
        => this.InvokeMethodAndWrap(Args(((VirtualDesktop)pDesktopReference).ComObject, uDirection));

    public IVirtualDesktop FindDesktop(Guid desktopId)
        => this.InvokeMethodAndWrap(Args(desktopId));

    public IVirtualDesktop CreateDesktop()
        => this.InvokeMethodAndWrap();

    public void SwitchDesktop(IVirtualDesktop desktop)
        => this.InvokeMethod(Args(((VirtualDesktop)desktop).ComObject));

    public void MoveDesktop(IVirtualDesktop pMove, int nIndex)
        => throw new NotSupportedException();

    public void RemoveDesktop(IVirtualDesktop pRemove, IVirtualDesktop pFallbackDesktop)
        => this.InvokeMethod(Args(((VirtualDesktop)pRemove).ComObject, ((VirtualDesktop)pFallbackDesktop).ComObject));

    public void MoveViewToDesktop(IntPtr hWnd, IVirtualDesktop desktop)
        => this.InvokeMethod(Args(this._factory.ApplicationViewFromHwnd(hWnd).ComObject, ((VirtualDesktop)desktop).ComObject));

    public void SetDesktopName(IVirtualDesktop desktop, string name)
        => throw new NotSupportedException();

    public void SetDesktopWallpaper(IVirtualDesktop desktop, string path)
        => throw new NotSupportedException();

    public void UpdateWallpaperPathForAllDesktops(string path)
        => throw new NotSupportedException();

    private VirtualDesktop InvokeMethodAndWrap(object?[]? parameters = null, [CallerMemberName] string methodName = "")
        => new(this.ComInterfaceAssembly, this.InvokeMethod<object>(parameters, methodName) ?? throw new VirtualDesktopException("Failed to get IVirtualDesktop instance."));
}
