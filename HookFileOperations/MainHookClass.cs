using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

using EasyHook;

using Explorip.HookFileOperations.FileOperations;
using Explorip.HookFileOperations.FilesOperations.Interfaces;

using static ManagedShell.Interop.NativeMethods;

namespace Explorip.HookFileOperations
{
    public class MainHookClass : IEntryPoint
    {
        private readonly ServerInterface _server = null;
        private readonly Queue<string> _messageQueue = new();
        private LocalHook copyItemHook = null, copyItemsHook = null;

#pragma warning disable IDE0060 // Supprimer le paramètre inutilisé
        public MainHookClass(RemoteHooking.IContext context, string channelName)
        {
            if (!string.IsNullOrWhiteSpace(channelName))
            {
                _server = RemoteHooking.IpcConnectClient<ServerInterface>(channelName);
                _server.Ping();
            }
        }
#pragma warning restore IDE0060 // Supprimer le paramètre inutilisé

        public void Run(RemoteHooking.IContext context, string channelName)
        {
            try
            {
                _server?.IsInstalled(RemoteHooking.GetCurrentProcessId());

                COMClassInfo copyItemsCom = new(typeof(ClSidIFileOperation), typeof(IFileOperation), nameof(IFileOperation.CopyItem), nameof(IFileOperation.CopyItems));
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
                catch (Exception) { /* Ignore errors */ }

                Uninstall();
            }
        }

        public void Uninstall()
        {
            copyItemHook?.Dispose();
            copyItemsHook?.Dispose();
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
