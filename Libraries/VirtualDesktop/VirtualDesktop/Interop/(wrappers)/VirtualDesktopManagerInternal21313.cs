﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace WindowsDesktop.Interop
{
    [ComInterfaceWrapper("IVirtualDesktopManagerInternal", 21313)]
    internal class VirtualDesktopManagerInternal21313(ComInterfaceAssembly assembly) : VirtualDesktopManagerInternal(assembly)
    {
        public override void MoveViewToDesktop(ApplicationView pView, VirtualDesktop desktop)
        {
            this.Invoke(Args(pView.ComObject, desktop.ComObject));
        }

        public override VirtualDesktop GetCurrentDesktop()
        {
            return this.GetDesktop(Args(IntPtr.Zero));
        }

        public override IEnumerable<VirtualDesktop> GetDesktops()
        {
            IObjectArray array = this.Invoke<IObjectArray>(Args(IntPtr.Zero));
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
            this.Invoke(Args(IntPtr.Zero, desktop.ComObject));
        }

        public override VirtualDesktop CreateDesktopW()
        {
            return this.GetDesktop(Args(IntPtr.Zero));
        }

        public override void MoveDesktop(VirtualDesktop desktop, int index)
        {
            this.Invoke(Args(desktop.ComObject, IntPtr.Zero, index));
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
            this.Invoke(Args(desktop.ComObject, name));
        }

        public override void SetDesktopWallpaper(VirtualDesktop desktop, string path)
        {
            this.Invoke(Args(desktop.ComObject, path));
        }
    }
}
