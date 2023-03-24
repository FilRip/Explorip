﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

using EasyHook;

using Explorip.HookFileOperations.FilesOperations.Interfaces;

namespace Explorip.HookFileOperations
{
    public class MainHookClass : IEntryPoint
    {
        private readonly ServerInterface _server = null;
        private LocalHook copyItemHook = null, copyItemsHook = null;

#pragma warning disable IDE0060, IDE0079 // Supprimer le paramètre inutilisé
        public MainHookClass(RemoteHooking.IContext context, string channelName)
        {
            if (!string.IsNullOrWhiteSpace(channelName))
            {
                _server = RemoteHooking.IpcConnectClient<ServerInterface>(channelName);
                _server.Ping();
            }
        }
#pragma warning restore IDE0060, IDE0079 // Supprimer le paramètre inutilisé

        public void Run(RemoteHooking.IContext context, string channelName)
        {
            try
            {
                _server?.IsInstalled(RemoteHooking.GetCurrentProcessId());

                COMClassInfo copyItemsCom = new(typeof(FilesOperations.ClSidIFileOperation), typeof(IFileOperation), nameof(IFileOperation.CopyItem), nameof(IFileOperation.CopyItems)/*,
                                                                                                                     nameof(IFileOperation.MoveItem), nameof(IFileOperation.MoveItems),
                                                                                                                     nameof(IFileOperation.RenameItem), nameof(IFileOperation.RenameItems),
                                                                                                                     nameof(IFileOperation.DeleteItem), nameof(IFileOperation.DeleteItems)*/);
                copyItemsCom.Query();
                copyItemHook = LocalHook.Create(copyItemsCom.MethodPointers[0], new DelegateCopyItem(CopyItemHooked), this);
                copyItemsHook = LocalHook.Create(copyItemsCom.MethodPointers[1], new DelegateCopyItems(CopyItemsHooked), this);
                /*var ShFileOpe = LocalHook.Create(LocalHook.GetProcAddress("shell32.dll", "SHFileOperationA"), new DelegateSHFileOperationA(My_SHFileOperationA), this);
                var ShFileOpeW = LocalHook.Create(LocalHook.GetProcAddress("shell32.dll", "SHFileOperationW"), new DelegateSHFileOperationW(My_SHFileOperationW), this);*/

                copyItemHook.ThreadACL.SetExclusiveACL(new int[] { 0 });
                copyItemsHook.ThreadACL.SetExclusiveACL(new int[] { 0 });
                /*ShFileOpe.ThreadACL.SetExclusiveACL(new int[] { 0 });
                ShFileOpeW.ThreadACL.SetExclusiveACL(new int[] { 0 });*/
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            if (_server != null)
            {
                try
                {
                    while (true)
                    {
                        Thread.Sleep(100);

                        _server.Ping();
                    }
                }
                catch (Exception ex)
                {
                    _server?.ReportMessages(new string[] { $"Error : {ex.Message}", ex.StackTrace });
                }
                Uninstall();
            }
        }

        public void Uninstall()
        {
            _server?.ReportMessage($"Explorip remove hook from process {RemoteHooking.GetCurrentProcessId()}");
            copyItemHook?.Dispose();
            copyItemsHook?.Dispose();
            LocalHook.Release();
            /*ShFileOpe.Dispose();
            ShFileOpeW.Dispose();*/
        }

        #region IFileOperation

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = false)]
        private delegate void DelegateCopyItem(IFileOperation self, IShellItem punkItems, IShellItem psiDestinationFolder, [MarshalAs(UnmanagedType.LPWStr)] string pszCopyName, IFileOperationProgressSink pfopsItem);

        private void CopyItemHooked(IFileOperation self, IShellItem punkItems, IShellItem psiDestinationFolder, [MarshalAs(UnmanagedType.LPWStr)] string pszCopyName, IFileOperationProgressSink pfopsItem)
        {
            _server?.ReportMessage("Intercept CopyItem");
            _server?.CopyItem(punkItems.GetDisplayName(SIGDN.FILESYSPATH), psiDestinationFolder.GetDisplayName(SIGDN.FILESYSPATH), pszCopyName);
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = false)]
        private delegate void DelegateCopyItems(IFileOperation self, [MarshalAs(UnmanagedType.IUnknown)] object punkItems, IShellItem psiDestinationFolder);

        private void CopyItemsHooked(IFileOperation self, [MarshalAs(UnmanagedType.IUnknown)] object punkItems, IShellItem psiDestinationFolder)
        {
            List<string> listFiles = new();
            _server?.ReportMessage("Intercept CopyItems");
            /*IShellItem src;
            punkItems.GetCount(out uint nbItems);
            for (uint i = 0; i < nbItems; i++)
            {
                punkItems.GetItemAt(i, out src);
                listFiles.Add(Path.GetFileName(src.GetDisplayName(SIGDN.FILESYSPATH)));
            }
            _server?.CopyItems(listFiles.ToArray(), psiDestinationFolder.GetDisplayName(SIGDN.FILESYSPATH));*/
        }

        #endregion

        #region SHFileOperation

        /// <summary>
        /// Possible flags for the SHFileOperation method.
        /// </summary>
        [Flags()]
        public enum FileOperation : ushort
        {
            /// <summary>
            /// Do not show a dialog during the process
            /// </summary>
            FOF_SILENT = 0x0004,
            /// <summary>
            /// Do not ask the user to confirm selection
            /// </summary>
            FOF_NOCONFIRMATION = 0x0010,
            /// <summary>
            /// Delete the file to the recycle bin.  (Required flag to send a file to the bin
            /// </summary>
            FOF_ALLOWUNDO = 0x0040,
            /// <summary>
            /// Do not show the names of the files or folders that are being recycled.
            /// </summary>
            FOF_SIMPLEPROGRESS = 0x0100,
            /// <summary>
            /// Surpress errors, if any occur during the process.
            /// </summary>
            FOF_NOERRORUI = 0x0400,
            /// <summary>
            /// Warn if files are too big to fit in the recycle bin and will need
            /// to be deleted completely.
            /// </summary>
            FOF_WANTNUKEWARNING = 0x4000,
        }

        /// <summary>
        /// File Operation Function Type for SHFileOperation
        /// </summary>
        public enum FileOperationType : uint
        {
            /// <summary>
            /// Move the objects
            /// </summary>
            FO_MOVE = 0x0001,
            /// <summary>
            /// Copy the objects
            /// </summary>
            FO_COPY = 0x0002,
            /// <summary>
            /// Delete (or recycle) the objects
            /// </summary>
            FO_DELETE = 0x0003,
            /// <summary>
            /// Rename the object(s)
            /// </summary>
            FO_RENAME = 0x0004,
        }

        /// <summary>
        /// SHFILEOPSTRUCT for SHFileOperation from COM
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct ShFileOpStruct
        {

            public IntPtr hwnd;
            [MarshalAs(UnmanagedType.U4)]
            public FileOperationType wFunc;
            public string pFrom;
            public string pTo;
            public FileOperation fFlags;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fAnyOperationsAborted;
            public IntPtr hNameMappings;
            public string lpszProgressTitle;
        }

        [DllImport("shell32.dll", CharSet = CharSet.Ansi, SetLastError = true, ThrowOnUnmappableChar = true)]
        private static extern int SHFileOperationA(ref ShFileOpStruct lpFileOp);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = false)]
        private delegate int DelegateSHFileOperationA(ref ShFileOpStruct lpFileOp);

        private int My_SHFileOperationA(ref ShFileOpStruct lpFileOp)
        {
            MessageBox.Show("SHFileOperationA");
            //return SHFileOperationA(ref lpFileOp);
            return 0;
        }

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true, ThrowOnUnmappableChar = true)]
        private static extern int SHFileOperationW(ref ShFileOpStruct lpFileOp);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = false)]
        private delegate int DelegateSHFileOperationW(ref ShFileOpStruct lpFileOp);

        private int My_SHFileOperationW(ref ShFileOpStruct lpFileOp)
        {
            MessageBox.Show("SHFileOperationW");
            //return SHFileOperationW(ref lpFileOp);
            return 0;
        }

        #endregion
    }
}
