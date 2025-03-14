﻿using System;
using System.Runtime.InteropServices;

using WindowsDesktop.Internal;

namespace WindowsDesktop.Interop
{
    [ComInterfaceWrapper]
    internal class VirtualDesktopNotificationService(ComInterfaceAssembly assembly) : ComInterfaceWrapperBase(assembly, service: ClSid.VirtualDesktopNotificationService)
    {
        public IDisposable Register(VirtualDesktopNotification pNotification)
        {
            uint dwCookie = this.Invoke<uint>(Args(pNotification));
            return Disposable.Create(() => this.Unregister(dwCookie));
        }

        private void Unregister(uint dwCookie)
        {
            try
            {
                this.Invoke(Args(dwCookie));
            }
            catch (COMException ex) when (ex.Match(HResult.RPC_S_SERVER_UNAVAILABLE))
            {
                /* Ignore errors */
            }
        }
    }
}
