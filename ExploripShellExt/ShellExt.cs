using System;
using System.Runtime.InteropServices;

using Microsoft.Win32;

namespace ExploripShellExt
{
    #region Shell Interfaces

    [ComImport(), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("000214FC-0000-0000-C000-000000000046")]
    internal interface ICopyHookW
    {
        /// <summary>
        /// Determines whether the Shell will be allowed to move, copy, delete, or 
        /// rename a folder or printer object.
        /// </summary>
        /// <param name="hwnd">
        /// A window handle to the dialog box to display information about the status 
        /// of the file operation.
        /// </param>
        /// <param name="fileOperation">
        /// A value that indicates which operation to perform. 
        /// </param>
        /// <param name="flags">
        /// The flags that control the operation. 
        /// * For folder copy hooks, this parameter can be one ore more of the values 
        ///   in FILEOP_FLAGS.
        /// * For printer copy hooks, this value is one of the values defined in 
        ///   PRINTEROP.
        /// </param>
        /// <param name="srcFile">
        /// A string that contains the name of the source folder.
        /// </param>
        /// <param name="srcAttribs">
        /// The attributes of the source folder. 
        /// </param>
        /// <param name="destFile">
        /// A string that contains the name of the destination folder.
        /// </param>
        /// <param name="destAttribs">
        /// The attributes of the destination folder.
        /// </param>
        /// <returns>
        /// Returns an integer value that indicates whether the Shell should perform 
        /// the operation. One of the following: 
        /// * DialogResult.Yes - Allows the operation.
        /// * DialogResult.No - Prevents the operation on this folder but continues 
        ///   with any other operations that have been approved (for example, a batch 
        ///   copy operation).
        /// * DialogResult.Cancel - Prevents the current operation and cancels any 
        ///   pending operations.
        /// </returns>
        [PreserveSig()]
        uint CopyCallback(
            IntPtr hwnd,
            FILEOP fileOperation,
            uint flags,
            [MarshalAs(UnmanagedType.LPWStr)]
            string srcFile,
            uint srcAttribs,
            [MarshalAs(UnmanagedType.LPWStr)]
            string destFile,
            uint destAttribs);
    }

    #endregion


    #region Shell Registration

    internal static class ShellExtReg
    {
        /// <summary>
        /// Register the folder copy hook handler.
        /// </summary>
        /// <param name="name">The name of the copy hook handler.</param>
        /// <param name="friendlyName">The CLSID of the component.</param>
        public static void RegisterShellExtCopyHookHandler(string name,
            Guid clsid)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("name must not be empty");
            }
            if (clsid == Guid.Empty)
            {
                throw new ArgumentException("clsid must not be empty");
            }

            // Create the key HKCR\Directory\shellex\CopyHookHandlers\<Name>.
            string keyName = string.Format(@"Software\Classes\Directory\ShellEx\CopyHookHandlers\{0}", name);
            using RegistryKey key = Registry.CurrentUser.CreateSubKey(keyName);
            // Set the default value of the key.
            key.SetValue(null, clsid.ToString("B"));
        }

        /// <summary>
        /// Unregister the folder copy hook handler.
        /// </summary>
        /// <param name="name">The name of the copy hook handler.</param>
        public static void UnregisterShellExtCopyHookHandler(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("name must not be null");
            }

            // Remove the key HKCR\Directory\shellex\CopyHookHandlers\<Name>.
            string keyName = string.Format(@"Software\Classes\Directory\ShellEx\CopyHookHandlers\{0}", name);
            Registry.CurrentUser.DeleteSubKeyTree(keyName, false);
        }
    }

    #endregion


    #region Enums & Structs

    public enum FILEOP
    {
        Move = 1,
        Copy = 2,
        Delete = 3,
        Rename = 4
    }

    /// <summary>
    /// Flags that control the file operation. 
    /// </summary>
    [Flags()]
    internal enum FFILEOP
    {
        FOF_MULTIDESTFILES = 0x0001,
        FOF_CONFIRMMOUSE = 0x0002,
        /// <summary>
        /// Don't display progress UI (confirm prompts may be displayed still).
        /// </summary>
        FOF_SILENT = 0x0004,
        /// <summary>
        /// Automatically rename the source files to avoid the collisions.
        /// </summary>
        FOF_RENAMEONCOLLISION = 0x0008,
        /// <summary>
        /// Don't display confirmation UI, assume "yes" for cases that can be 
        /// bypassed, "no" for those that can not.
        /// </summary>
        FOF_NOCONFIRMATION = 0x0010,
        /// <summary>
        /// Fill in SHFILEOPSTRUCT.hNameMappings.
        /// Must be freed using SHFreeNameMappings.
        /// </summary>
        FOF_WANTMAPPINGHANDLE = 0x0020,
        /// <summary>
        /// Enable undo including Recycle behavior for IFileOperation::Delete().
        /// </summary>
        FOF_ALLOWUNDO = 0x0040,
        /// <summary>
        /// Only operate on the files (non folders), both files and folders are 
        /// assumed without this.
        /// </summary>
        FOF_FILESONLY = 0x0080,
        /// <summary>
        /// Means don't show names of files.
        /// </summary>
        FOF_SIMPLEPROGRESS = 0x0100,
        /// <summary>
        /// Don't dispplay confirmatino UI before making any needed directories, 
        /// assume "Yes" in these cases.
        /// </summary>
        FOF_NOCONFIRMMKDIR = 0x0200,
        /// <summary>
        /// Don't put up error UI, other UI may be displayed, progress, confirmations.
        /// </summary>
        FOF_NOERRORUI = 0x0400,
        /// <summary>
        /// Don't copy file security attributes (ACLs).
        /// </summary>
        FOF_NOCOPYSECURITYATTRIBS = 0x0800,
        /// <summary>
        /// Don't recurse into directories for operations that would recurse.
        /// </summary>
        FOF_NORECURSION = 0x1000,
        /// <summary>
        /// Don't operate on connected elements ("xxx_files" folders that go with 
        /// .htm files).
        /// </summary>
        FOF_NO_CONNECTED_ELEMENTS = 0x2000,
        /// <summary>
        /// During delete operation, warn if nuking instead of recycling (partially 
        /// overrides FOF_NOCONFIRMATION).
        /// </summary>
        FOF_WANTNUKEWARNING = 0x4000,
        /// <summary>
        /// Deprecated; the operations engine always does the right thing on 
        /// FolderLink objects (symlinks, reparse points, folder shortcuts)
        /// </summary>
        FOF_NORECURSEREPARSE = 0x8000,
        /// <summary>
        /// Don't display any UI at all.
        /// </summary>
        FOF_NO_UI = FOF_SILENT | FOF_NOCONFIRMATION | FOF_NOERRORUI | FOF_NOCONFIRMMKDIR
    }

    #endregion
}
