﻿using System;
using System.Runtime.InteropServices;

using ManagedShell.Common.Logging;
using ManagedShell.Interop;
using ManagedShell.ShellFolders.Enums;
using ManagedShell.ShellFolders.Interfaces;

namespace ManagedShell.ShellFolders;

public class ShellNewMenuCommand : ShellMenuCommand
{
    internal IContextMenu iContextMenu;
    internal IContextMenu2 iContextMenu2;
    internal IContextMenu3 iContextMenu3;

    internal IntPtr iContextMenuPtr;
    internal IntPtr iContextMenu2Ptr;
    internal IntPtr iContextMenu3Ptr;

    internal IntPtr nativeMenuPtr;

    public void AddSubMenu(ShellFolder folder, int position, ref IntPtr parentNativeMenuPtr)
    {
        if (GetNewContextMenu(folder))
        {
            iContextMenu.QueryContextMenu(
                parentNativeMenuPtr,
                (uint)position,
                Interop.CMD_FIRST,
                Interop.CMD_LAST,
                CMF.NORMAL);

            nativeMenuPtr = Interop.GetSubMenu(parentNativeMenuPtr, position);

            Guid guid = typeof(IContextMenu2).GUID;
            if (Marshal.QueryInterface(iContextMenuPtr, ref guid,
                out iContextMenu2Ptr) == NativeMethods.S_OK && iContextMenu2Ptr != IntPtr.Zero)
            {
                try
                {
                    iContextMenu2 =
                        (IContextMenu2)Marshal.GetTypedObjectForIUnknown(iContextMenu2Ptr, typeof(IContextMenu2));
                }
                catch (Exception e)
                {
                    ShellLogger.Error($"ShellContextMenu: Error retrieving IContextMenu2 interface: {e.Message}");
                }
            }

            guid = typeof(IContextMenu3).GUID;
            if (Marshal.QueryInterface(iContextMenuPtr, ref guid,
                out iContextMenu3Ptr) == NativeMethods.S_OK && iContextMenu3Ptr != IntPtr.Zero)
            {
                try
                {
                    iContextMenu3 =
                        (IContextMenu3)Marshal.GetTypedObjectForIUnknown(iContextMenu3Ptr, typeof(IContextMenu3));
                }
                catch (Exception e)
                {
                    ShellLogger.Error($"ShellContextMenu: Error retrieving IContextMenu3 interface: {e.Message}");
                }
            }
        }
    }

    private bool GetNewContextMenu(ShellFolder folder)
    {
        Guid guid = typeof(IContextMenu).GUID;
        Guid newMenuGuid = Interop.CLSID_NewMenu;
        if (Interop.CoCreateInstance(
            ref newMenuGuid,
            IntPtr.Zero,
            CLSCTX.INPROC_SERVER,
            ref guid,
            out iContextMenuPtr) == NativeMethods.S_OK)
        {
            iContextMenu = Marshal.GetTypedObjectForIUnknown(iContextMenuPtr, typeof(IContextMenu)) as IContextMenu;

            guid = typeof(IShellExtInit).GUID;
            if (Marshal.QueryInterface(
                iContextMenuPtr,
                ref guid,
                out IntPtr iShellExtInitPtr) == NativeMethods.S_OK)
            {
                IShellExtInit iShellExtInit = Marshal.GetTypedObjectForIUnknown(
                    iShellExtInitPtr, typeof(IShellExtInit)) as IShellExtInit;

                iShellExtInit?.Initialize(folder.AbsolutePidl, IntPtr.Zero, 0);

                if (iShellExtInit != null)
                {
                    Marshal.ReleaseComObject(iShellExtInit);
                }

                if (iShellExtInitPtr != IntPtr.Zero)
                {
                    Marshal.Release(iShellExtInitPtr);
                }

                return true;
            }
        }

        return false;
    }

    internal void FreeResources()
    {
        if (iContextMenu != null)
        {
            Marshal.FinalReleaseComObject(iContextMenu);
            iContextMenu = null;
        }

        if (iContextMenu2 != null)
        {
            Marshal.FinalReleaseComObject(iContextMenu2);
            iContextMenu2 = null;
        }

        if (iContextMenu3 != null)
        {
            Marshal.FinalReleaseComObject(iContextMenu3);
            iContextMenu3 = null;
        }

        if (iContextMenuPtr != IntPtr.Zero)
        {
            Marshal.Release(iContextMenuPtr);
            iContextMenuPtr = IntPtr.Zero;
        }

        if (iContextMenu2Ptr != IntPtr.Zero)
        {
            Marshal.Release(iContextMenu2Ptr);
            iContextMenu2Ptr = IntPtr.Zero;
        }

        if (iContextMenu3Ptr != IntPtr.Zero)
        {
            Marshal.Release(iContextMenu3Ptr);
            iContextMenu3Ptr = IntPtr.Zero;
        }

        if (nativeMenuPtr != IntPtr.Zero)
        {
            Interop.DestroyMenu(nativeMenuPtr);
            nativeMenuPtr = IntPtr.Zero;
        }
    }
}
