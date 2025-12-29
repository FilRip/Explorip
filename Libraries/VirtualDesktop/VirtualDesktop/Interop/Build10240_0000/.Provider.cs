using System;

using VirtualDesktop.Interop.Proxy;

namespace VirtualDesktop.Interop.Build10240;

internal class VirtualDesktopProvider10240 : VirtualDesktopProvider
{
    private IVirtualDesktopManager? _virtualDesktopManager;
    private ApplicationViewCollection? _applicationViewCollection;
    private VirtualDesktopManagerInternal? _virtualDesktopManagerInternal;
    private VirtualDesktopPinnedApps? _virtualDesktopPinnedApps;
    private VirtualDesktopNotificationService? _virtualDesktopNotificationService;

    public override IApplicationViewCollection ApplicationViewCollection
        => this._applicationViewCollection ?? throw InitializationIsRequired;

    public override IVirtualDesktopManager VirtualDesktopManager
        => this._virtualDesktopManager ?? throw InitializationIsRequired;

    public override IVirtualDesktopManagerInternal VirtualDesktopManagerInternal
        => this._virtualDesktopManagerInternal ?? throw InitializationIsRequired;

    public override IVirtualDesktopPinnedApps VirtualDesktopPinnedApps
        => this._virtualDesktopPinnedApps ?? throw InitializationIsRequired;

    public override IVirtualDesktopNotificationService VirtualDesktopNotificationService
        => this._virtualDesktopNotificationService ?? throw InitializationIsRequired;

    private protected override void InitializeCore(ComInterfaceAssembly assembly)
    {
        Type type = Type.GetTypeFromCLSID(Clsid.VirtualDesktopManager)
            ?? throw new VirtualDesktopException($"No type found for CLSID '{Clsid.VirtualDesktopManager}'.");
        this._virtualDesktopManager = Activator.CreateInstance(type) is IVirtualDesktopManager manager
            ? manager
            : throw new VirtualDesktopException($"Failed to create instance of Type '{typeof(IVirtualDesktopManager)}'.");

        this._applicationViewCollection = new ApplicationViewCollection(assembly);
        ComWrapperFactory factory = new(
            x => new ApplicationView(assembly, x),
            x => this._applicationViewCollection.GetViewForHwnd(x),
            x => new VirtualDesktop(assembly, x));
        this._virtualDesktopManagerInternal = new VirtualDesktopManagerInternal(assembly, factory);
        this._virtualDesktopPinnedApps = new VirtualDesktopPinnedApps(assembly, factory);
        this._virtualDesktopNotificationService = new VirtualDesktopNotificationService(assembly, factory);
    }
}
