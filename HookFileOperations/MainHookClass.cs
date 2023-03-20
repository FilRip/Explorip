using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly Queue<string> _messageQueue = new();
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

                COMClassInfo copyItemsCom = new(typeof(FilesOperations.ClSidIFileOperation), typeof(IFileOperation), nameof(IFileOperation.CopyItem), nameof(IFileOperation.CopyItems));
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

                        string[] queued = null;

                        lock (_messageQueue)
                        {
                            queued = _messageQueue.ToArray();
                            _messageQueue.Clear();
                        }

                        // Send newly monitored file accesses to main explorip process
                        if (queued != null && queued.Length > 0)
                        {
                            _server.ReportMessages(queued);
                        }
                        else
                        {
                            _server.Ping();
                        }
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
        private delegate void DelegateCopyItem(IntPtr punkItems, IntPtr psiDestinationFolder, [MarshalAs(UnmanagedType.LPWStr)] string pszCopyName, IntPtr pfopsItem);

        private void CopyItemHooked(IntPtr punkItems, IntPtr psiDestinationFolder, [MarshalAs(UnmanagedType.LPWStr)] string pszCopyName, IntPtr pfopsItem)
        {
            MessageBox.Show("Copier");
            //self.CopyItem(punkItems, psiDestinationFolder);
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = false)]
        private delegate void DelegateCopyItems(IntPtr punkItems, IntPtr psiDestinationFolder);

        private void CopyItemsHooked(IntPtr punkItems, IntPtr psiDestinationFolder)
        {
            MessageBox.Show("Copiers");
            //self.CopyItem(punkItems, psiDestinationFolder);
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
            MessageBox.Show("SHFileOperation");
            //return SHFileOperationA(ref lpFileOp);
            return 0;
        }

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true, ThrowOnUnmappableChar = true)]
        private static extern int SHFileOperationW(ref ShFileOpStruct lpFileOp);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = false)]
        private delegate int DelegateSHFileOperationW(ref ShFileOpStruct lpFileOp);

        private int My_SHFileOperationW(ref ShFileOpStruct lpFileOp)
        {
            MessageBox.Show("SHFileOperation");
            //return SHFileOperationW(ref lpFileOp);
            return 0;
        }

        #endregion
    }
}
