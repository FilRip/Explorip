using System;
using System.Runtime.InteropServices;

using Microsoft.WindowsAPICodePack.Interop;
using Microsoft.WindowsAPICodePack.Interop.PowerManagement;
using Microsoft.WindowsAPICodePack.Resources;

namespace Microsoft.WindowsAPICodePack.PowerManagement;

internal static class Power
{
    internal static PowerManagementNativeMethods.SystemPowerCapabilities
        GetSystemPowerCapabilities()
    {

        uint retval = PowerManagementNativeMethods.CallNtPowerInformation(
          PowerManagementNativeMethods.PowerInformationLevel.SystemPowerCapabilities,
          IntPtr.Zero, 0, out PowerManagementNativeMethods.SystemPowerCapabilities powerCap,
          (uint)Marshal.SizeOf(typeof(PowerManagementNativeMethods.SystemPowerCapabilities))
          );

        if (retval == CoreNativeMethods.StatusAccessDenied)
        {
            throw new UnauthorizedAccessException(LocalizedMessages.PowerInsufficientAccessCapabilities);
        }

        return powerCap;
    }

    internal static PowerManagementNativeMethods.SystemBatteryState GetSystemBatteryState()
    {

        uint retval = PowerManagementNativeMethods.CallNtPowerInformation(
          PowerManagementNativeMethods.PowerInformationLevel.SystemBatteryState,
          IntPtr.Zero, 0, out PowerManagementNativeMethods.SystemBatteryState batteryState,
          (uint)Marshal.SizeOf(typeof(PowerManagementNativeMethods.SystemBatteryState))
          );

        if (retval == CoreNativeMethods.StatusAccessDenied)
        {
            throw new UnauthorizedAccessException(LocalizedMessages.PowerInsufficientAccessBatteryState);
        }

        return batteryState;
    }

    /// <summary>
    /// Registers the application to receive power setting notifications 
    /// for the specific power setting event.
    /// </summary>
    /// <param name="handle">Handle indicating where the power setting 
    /// notifications are to be sent.</param>
    /// <param name="powerSetting">The GUID of the power setting for 
    /// which notifications are to be sent.</param>
    /// <returns>Returns a notification handle for unregistering 
    /// power notifications.</returns>
    internal static int RegisterPowerSettingNotification(
        IntPtr handle, Guid powerSetting)
    {
        int outHandle = PowerManagementNativeMethods.RegisterPowerSettingNotification(
            handle,
            ref powerSetting,
            0);

        return outHandle;
    }
}