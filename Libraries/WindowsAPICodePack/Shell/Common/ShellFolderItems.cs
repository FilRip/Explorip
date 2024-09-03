using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Microsoft.WindowsAPICodePack.Interop;
using Microsoft.WindowsAPICodePack.Shell.Interop.Common;

namespace Microsoft.WindowsAPICodePack.Shell.Common;

class ShellFolderItems : IEnumerator<ShellObject>
{
    #region Private Fields

    private IEnumIDList nativeEnumIdList;
    private ShellObject currentItem;
    private bool disposedValue;
    readonly ShellContainer nativeShellFolder;

    #endregion

    #region Internal Constructor

    internal ShellFolderItems(ShellContainer nativeShellFolder)
    {
        this.nativeShellFolder = nativeShellFolder;

        HResult hr = nativeShellFolder.NativeShellFolder.EnumObjects(
            IntPtr.Zero,
            ShellNativeMethods.ShellFolderEnumerationOptions.Folders | ShellNativeMethods.ShellFolderEnumerationOptions.NonFolders,
            out nativeEnumIdList);


        if (!CoreErrorHelper.Succeeded(hr))
        {
            if (hr == HResult.Canceled)
            {
                throw new System.IO.FileNotFoundException();
            }
            else
            {
                throw new ShellException(hr);
            }
        }


    }

    #endregion

    #region IEnumerator<ShellObject> Members

    public ShellObject Current
    {
        get
        {
            return currentItem;
        }
    }

    #endregion

    #region IEnumerator Members

    object IEnumerator.Current
    {
        get { return currentItem; }

    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool MoveNext()
    {
        if (nativeEnumIdList == null) { return false; }

        uint itemsRequested = 1;
        HResult hr = nativeEnumIdList.Next(itemsRequested, out IntPtr item, out uint numItemsReturned);

        if (numItemsReturned < itemsRequested || hr != HResult.Ok) { return false; }

        currentItem = ShellObjectFactory.Create(item, nativeShellFolder);

        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    public void Reset()
    {
        nativeEnumIdList?.Reset();
    }

    #endregion

    #region Dispose
    public bool IsDisposed
    {
        get { return disposedValue; }
    }
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing && nativeEnumIdList != null)
            {
                Marshal.ReleaseComObject(nativeEnumIdList);
                nativeEnumIdList = null;
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    #endregion
}
