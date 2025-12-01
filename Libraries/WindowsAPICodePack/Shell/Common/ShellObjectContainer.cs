using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Microsoft.WindowsAPICodePack.Interop;
using Microsoft.WindowsAPICodePack.Shell.Interop.Common;

namespace Microsoft.WindowsAPICodePack.Shell.Common;

/// <summary>
/// Represents the base class for all types of Shell "containers". Any class deriving from this class
/// can contain other ShellObjects (e.g. ShellFolder, FileSystemKnownFolder, ShellLibrary, etc)
/// </summary>
public abstract class ShellContainer : ShellObject, IEnumerable<ShellObject>
{
    #region Private Fields

    private IShellFolder desktopFolderEnumeration;
    private IShellFolder nativeShellFolder;

    #endregion

    #region Internal Properties

    internal IShellFolder NativeShellFolder
    {
        get
        {
            if (nativeShellFolder == null)
            {
                Guid guid = new(ShellIidGuid.IShellFolder);
                Guid handler = new(ShellBhidGuid.ShellFolderObject);

                HResult hr = NativeShellItem.BindToHandler(
                    IntPtr.Zero, ref handler, ref guid, out nativeShellFolder);

                if (CoreErrorHelper.Failed(hr))
                {
                    string str = ShellHelper.GetParsingName(NativeShellItem);
                    if (str != null && str != Environment.GetFolderPath(Environment.SpecialFolder.Desktop))
                    {
#pragma warning disable IDE0079
#pragma warning disable S2372 // Exceptions should not be thrown from property getters
                        throw new ShellException(hr);
#pragma warning restore S2372 // Exceptions should not be thrown from property getters
#pragma warning restore IDE0079
                    }
                }
            }

            return nativeShellFolder;
        }
    }

    #endregion

    #region Internal Constructor

    protected ShellContainer() { }

#pragma warning disable IDE0079
#pragma warning disable S3442 // "abstract" classes should not have "public" constructors
    internal ShellContainer(IShellItem2 shellItem) : base(shellItem) { }
#pragma warning disable IDE0079
#pragma warning disable S3442 // "abstract" classes should not have "public" constructors

    #endregion

    #region Disposable Pattern

    /// <summary>
    /// Release resources
    /// </summary>
    /// <param name="disposing"><B>True</B> indicates that this is being called from Dispose(), rather than the finalizer.</param>
    protected override void Dispose(bool disposing)
    {
        if (nativeShellFolder != null)
        {
            Marshal.ReleaseComObject(nativeShellFolder);
            nativeShellFolder = null;
        }

        if (desktopFolderEnumeration != null)
        {
            Marshal.ReleaseComObject(desktopFolderEnumeration);
            desktopFolderEnumeration = null;
        }

        base.Dispose(disposing);
    }

    #endregion

    #region IEnumerable<ShellObject> Members

    /// <summary>
    /// Enumerates through contents of the ShellObjectContainer
    /// </summary>
    /// <returns>Enumerated contents</returns>
    public IEnumerator<ShellObject> GetEnumerator()
    {
        if (NativeShellFolder == null)
        {
            if (desktopFolderEnumeration == null)
            {
                ShellNativeMethods.SHGetDesktopFolder(out desktopFolderEnumeration);
            }

            nativeShellFolder = desktopFolderEnumeration;
        }

        return new ShellFolderItems(this);
    }

    #endregion

    #region IEnumerable Members

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return new ShellFolderItems(this);
    }

    #endregion
}
