﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace WindowsDesktop.Interop
{
    [ComInterfaceWrapper("IVirtualDesktopManagerInternal", 3, 10240)]
    internal sealed class VirtualDesktopManagerInternal10240(ComInterfaceAssembly assembly) : VirtualDesktopManagerInternal(assembly, latestVersion: 3)
    {
        public override void MoveViewToDesktop(ApplicationView pView, VirtualDesktop desktop)
        {
            this.Invoke(Args(pView.ComObject, desktop.ComObject));
        }

        public override VirtualDesktop GetCurrentDesktop()
        {
            return this.GetDesktop();
        }

        public override IEnumerable<VirtualDesktop> GetDesktops()
        {
            IObjectArray array = this.Invoke<IObjectArray>();
            uint count = array.GetCount();
            Type vdType = this.ComInterfaceAssembly.GetType("IVirtualDesktop");

            for (uint i = 0; i < count; i++)
            {
                array.GetAt(i, vdType.GUID, out object ppvObject);
                yield return VirtualDesktopCache.GetOrCreate(ppvObject);
            }
        }

        public override VirtualDesktop GetAdjacentDesktop(VirtualDesktop pDesktopReference, AdjacentDesktop uDirection)
        {
            return this.GetDesktop(Args(pDesktopReference.ComObject, uDirection));
        }

        public override void SwitchDesktop(VirtualDesktop desktop)
        {
            this.Invoke(Args(desktop.ComObject));
        }

        public override VirtualDesktop CreateDesktopW()
        {
            return this.GetDesktop();
        }

        public override void MoveDesktop(VirtualDesktop desktop, int index)
        {
            throw new PlatformNotSupportedException("This Windows 10 version is not supported.");
        }

        public override void RemoveDesktop(VirtualDesktop pRemove, VirtualDesktop pFallbackDesktop)
        {
            this.Invoke(Args(pRemove.ComObject, pFallbackDesktop.ComObject));
        }

        public override VirtualDesktop FindDesktop(Guid desktopId)
        {
            return this.GetDesktop(Args(desktopId));
        }

        private VirtualDesktop GetDesktop(object[] parameters = null, [CallerMemberName()] string methodName = "")
            => VirtualDesktopCache.GetOrCreate(this.Invoke<object>(parameters, methodName));

        public override void SetDesktopName(VirtualDesktop desktop, string name)
        {
            if (this.ComVersion < 2) throw new PlatformNotSupportedException("This Windows 10 version is not supported.");

            this.Invoke(Args(desktop.ComObject, name));
        }

        public override void SetDesktopWallpaper(VirtualDesktop desktop, string path)
        {
            throw new PlatformNotSupportedException("This Windows 10 version is not supported.");
        }
    }
}
