﻿using System;
using System.Runtime.InteropServices;

namespace ManagedShell.Common.Helpers;

public static class VolumeHelper
{
    // From Simon Mourier https://stackoverflow.com/a/31042902

    public static float GetMasterVolume()
    {
        float volume = 0;

        // get the speakers (1st render + multimedia) device
        IMMDeviceEnumerator deviceEnumerator = (IMMDeviceEnumerator)(new MMDeviceEnumerator());

        const int eRender = 0;
        const int eMultimedia = 1;
        deviceEnumerator.GetDefaultAudioEndpoint(eRender, eMultimedia, out IMMDevice speakers);

        if (speakers != null)
        {
            speakers.Activate(typeof(IAudioEndpointVolume).GUID, 0, IntPtr.Zero, out object o);

            if (o is IAudioEndpointVolume aepv)
            {
                volume = aepv.GetMasterVolumeLevelScalar();
                Marshal.ReleaseComObject(aepv);
            }
            Marshal.ReleaseComObject(speakers);
        }
        Marshal.ReleaseComObject(deviceEnumerator);

        return volume;
    }

    public static bool IsVolumeMuted()
    {
        int isMuted = 1;

        // get the speakers (1st render + multimedia) device
        IMMDeviceEnumerator deviceEnumerator = (IMMDeviceEnumerator)(new MMDeviceEnumerator());

        const int eRender = 0;
        const int eMultimedia = 1;
        deviceEnumerator.GetDefaultAudioEndpoint(eRender, eMultimedia, out IMMDevice speakers);

        if (speakers != null)
        {
            speakers.Activate(typeof(IAudioEndpointVolume).GUID, 0, IntPtr.Zero, out object o);

            if (o is IAudioEndpointVolume aepv)
            {
                isMuted = aepv.GetMute();
                Marshal.ReleaseComObject(aepv);
            }
            Marshal.ReleaseComObject(speakers);
        }
        Marshal.ReleaseComObject(deviceEnumerator);

        if (isMuted == 1)
            return true;
        else
            return false;
    }

    [ComImport()]
    [Guid("BCDE0395-E52F-467C-8E3D-C4579291692E")]
#pragma warning disable S3260 // Non-derived "private" classes and records should be "sealed"
    private class MMDeviceEnumerator
#pragma warning restore S3260 // Non-derived "private" classes and records should be "sealed"
    {
    }

#pragma warning disable IDE1006
    [Guid("5CDF2C82-841E-4546-9722-0CF74078229A"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IAudioEndpointVolume
    {
        void _VtblGap1_6();
        float GetMasterVolumeLevelScalar();
        void _VtblGap8_5();
        int GetMute();
    }
#pragma warning restore IDE1006

#pragma warning disable IDE1006
    [Guid("A95664D2-9614-4F35-A746-DE8DB63617E6"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IMMDeviceEnumerator
    {
        void _VtblGap1_1();

        [PreserveSig()]
        int GetDefaultAudioEndpoint(int dataFlow, int role, out IMMDevice ppDevice);
    }
#pragma warning restore IDE1006

    [Guid("D666063F-1587-4E43-81F1-B948E807363F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IMMDevice
    {
        [PreserveSig()]
        int Activate([MarshalAs(UnmanagedType.LPStruct)] Guid iid, int dwClsCtx, IntPtr pActivationParams, [MarshalAs(UnmanagedType.IUnknown)] out object ppInterface);
    }
}
