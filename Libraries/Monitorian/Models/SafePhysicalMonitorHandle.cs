using System;
using System.Runtime.InteropServices;

namespace Monitorian.Models;

public class SafePhysicalMonitorHandle : SafeHandle
{
    public SafePhysicalMonitorHandle(IntPtr handle) : base(IntPtr.Zero, true)
    {
        this.handle = handle; // IntPtr.Zero may be a valid handle.
    }

    public override bool IsInvalid => false; // The validity cannot be checked by the handle.

    protected override bool ReleaseHandle()
    {
        return NativeMethods.DestroyPhysicalMonitor(handle);
    }
}
