﻿using System;
using System.Collections.Generic;

using Microsoft.WindowsAPICodePack.Interop;
using Microsoft.WindowsAPICodePack.PropertySystem;
using Microsoft.WindowsAPICodePack.Shell.Interop.Common;

namespace Microsoft.WindowsAPICodePack.Shell.Common;

internal class ShellItemArray : IShellItemArray
{
    readonly List<IShellItem> shellItemsList = [];

    internal ShellItemArray(IShellItem[] shellItems)
    {
        shellItemsList.AddRange(shellItems);
    }

    #region IShellItemArray Members

    public HResult BindToHandler(IntPtr pbc, ref Guid rbhid, ref Guid riid, out IntPtr ppvOut)
    {
        throw new NotSupportedException();
    }

    public HResult GetPropertyStore(int Flags, ref Guid riid, out IntPtr ppv)
    {
        throw new NotSupportedException();
    }

    public HResult GetPropertyDescriptionList(ref PropertyKey keyType, ref Guid riid, out IntPtr ppv)
    {
        throw new NotSupportedException();
    }

    public HResult GetAttributes(ShellNativeMethods.ShellItemAttributeOptions dwAttribFlags, ShellNativeMethods.ShellFileGetAttributesOptions sfgaoMask, out ShellNativeMethods.ShellFileGetAttributesOptions psfgaoAttribs)
    {
        throw new NotSupportedException();
    }

    public HResult GetCount(out uint pdwNumItems)
    {
        pdwNumItems = (uint)shellItemsList.Count;
        return HResult.Ok;
    }

    public HResult GetItemAt(uint dwIndex, out IShellItem ppsi)
    {
        int index = (int)dwIndex;

        if (index < shellItemsList.Count)
        {
            ppsi = shellItemsList[index];
            return HResult.Ok;
        }
        else
        {
            ppsi = null;
            return HResult.Fail;
        }
    }

    public HResult EnumItems(out IntPtr ppenumShellItems)
    {
        throw new NotSupportedException();
    }

    #endregion
}
