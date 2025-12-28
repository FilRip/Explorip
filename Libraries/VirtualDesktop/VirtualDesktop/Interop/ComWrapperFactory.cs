using System;

using VirtualDesktop.Interop.Proxy;

namespace VirtualDesktop.Interop;

internal class ComWrapperFactory(
    Func<object, ComWrapperBase<IApplicationView>> applicationView,
    Func<IntPtr, ComWrapperBase<IApplicationView>> applicationViewFromHwnd,
    Func<object, ComWrapperBase<IVirtualDesktop>> virtualDesktop)
{
    public Func<object, ComWrapperBase<IApplicationView>> ApplicationView { get; } = applicationView;

    public Func<IntPtr, ComWrapperBase<IApplicationView>> ApplicationViewFromHwnd { get; } = applicationViewFromHwnd;

    public Func<object, ComWrapperBase<IVirtualDesktop>> VirtualDesktop { get; } = virtualDesktop;
}
