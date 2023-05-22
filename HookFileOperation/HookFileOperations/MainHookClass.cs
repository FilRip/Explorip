using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;

using EasyHook;

using Explorip.HookFileOperations.FilesOperations.Interfaces;
using Explorip.HookFileOperations.Ipc;

namespace Explorip.HookFileOperations
{
    public class MainHookClass : IEntryPoint
    {
        private readonly ServerInterface _server = null;
        private LocalHook _copyItemHook = null, _copyItemsHook = null;
        private LocalHook _moveItemHook = null, _moveItemsHook = null;
        private LocalHook _renameItemHook = null, _renameItemsHook = null;
        private LocalHook _deleteItemHook = null, _deleteItemsHook = null;
        private LocalHook _performOperationsHook = null, _newItemHook = null;
        //private LocalHook _setOperationFlagsHook = null;

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

                COMClassInfo copyItemsCom = new(typeof(FilesOperations.ClSidIFileOperation), typeof(IFileOperation), nameof(IFileOperation.CopyItem), nameof(IFileOperation.CopyItems),
                                                                                                                     nameof(IFileOperation.MoveItem), nameof(IFileOperation.MoveItems),
                                                                                                                     nameof(IFileOperation.RenameItem), nameof(IFileOperation.RenameItems),
                                                                                                                     nameof(IFileOperation.DeleteItem), nameof(IFileOperation.DeleteItems),
                                                                                                                     nameof(IFileOperation.PerformOperations), nameof(IFileOperation.NewItem)/*,
                                                                                                                     nameof(IFileOperation.SetOperationFlags)*/);
                copyItemsCom.Query();

                _copyItemHook = LocalHook.Create(copyItemsCom.MethodPointers[0], new DelegateCopyItem(CopyItemHooked), this);
                _copyItemsHook = LocalHook.Create(copyItemsCom.MethodPointers[1], new DelegateCopyItems(CopyItemsHooked), this);
                _moveItemHook = LocalHook.Create(copyItemsCom.MethodPointers[2], new DelegateMoveItem(MoveItemHooked), this);
                _moveItemsHook = LocalHook.Create(copyItemsCom.MethodPointers[3], new DelegateMoveItems(MoveItemsHooked), this);
                _renameItemHook = LocalHook.Create(copyItemsCom.MethodPointers[4], new DelegateRenameItem(RenameItemHooked), this);
                _renameItemsHook = LocalHook.Create(copyItemsCom.MethodPointers[5], new DelegateRenameItems(RenameItemsHooked), this);
                _deleteItemHook = LocalHook.Create(copyItemsCom.MethodPointers[6], new DelegateDeleteItem(DeleteItemHooked), this);
                _deleteItemsHook = LocalHook.Create(copyItemsCom.MethodPointers[7], new DelegateDeleteItems(DeleteItemsHooked), this);
                _performOperationsHook = LocalHook.Create(copyItemsCom.MethodPointers[8], new DelegatePerformOperations(PerformOperationsHooked), this);
                _newItemHook = LocalHook.Create(copyItemsCom.MethodPointers[9], new DelegateNewItem(NewItemHooked), this);
                //_setOperationFlagsHook = LocalHook.Create(copyItemsCom.MethodPointers[10], new DelegateSetOperationFlags(NewSetOperationFlags), this);

                _copyItemHook.ThreadACL.SetExclusiveACL(new int[] { 0 });
                _copyItemsHook.ThreadACL.SetExclusiveACL(new int[] { 0 });
                _moveItemHook.ThreadACL.SetExclusiveACL(new int[] { 0 });
                _moveItemsHook.ThreadACL.SetExclusiveACL(new int[] { 0 });
                _deleteItemHook.ThreadACL.SetExclusiveACL(new int[] { 0 });
                _deleteItemsHook.ThreadACL.SetExclusiveACL(new int[] { 0 });
                _renameItemHook.ThreadACL.SetExclusiveACL(new int[] { 0 });
                _renameItemsHook.ThreadACL.SetExclusiveACL(new int[] { 0 });
                _performOperationsHook.ThreadACL.SetExclusiveACL(new int[] { 0 });
                _newItemHook.ThreadACL.SetExclusiveACL(new int[] { 0 });
                //_setOperationFlagsHook.ThreadACL.SetExclusiveACL(new int[] { 0 });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
                catch (Exception)
                {
                    /* Ignore errors */
                }
                Uninstall();
            }
        }

        public void Uninstall()
        {
            _copyItemHook?.Dispose();
            _copyItemsHook?.Dispose();
            _moveItemHook?.Dispose();
            _moveItemsHook?.Dispose();
            _deleteItemHook?.Dispose();
            _deleteItemsHook?.Dispose();
            _renameItemHook?.Dispose();
            _renameItemsHook?.Dispose();
            _performOperationsHook?.Dispose();
            _newItemHook?.Dispose();
            //_setOperationFlagsHook?.Dispose();

            LocalHook.Release();
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
            _server?.ReportMessage("Intercept CopyItems");
            Guid guidIShellItem = typeof(IShellItem).GUID;
            if (Marshal.QueryInterface(Marshal.GetIUnknownForObject(punkItems), ref guidIShellItem, out IntPtr ptrShellItem) == 0)
            {
                IShellItem si = (IShellItem)Marshal.GetObjectForIUnknown(ptrShellItem);
                string src = si.GetDisplayName(SIGDN.FILESYSPATH);
                _server?.CopyItem(src, psiDestinationFolder.GetDisplayName(SIGDN.FILESYSPATH), Path.GetFileName(src));
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = false)]
        private delegate void DelegateMoveItem(IFileOperation self, IShellItem punkItems, IShellItem psiDestinationFolder, [MarshalAs(UnmanagedType.LPWStr)] string pszCopyName, IFileOperationProgressSink pfopsItem);
        private void MoveItemHooked(IFileOperation self, IShellItem punkItems, IShellItem psiDestinationFolder, [MarshalAs(UnmanagedType.LPWStr)] string pszCopyName, IFileOperationProgressSink pfopsItem)
        {
            _server?.ReportMessage("Intercept MoveItem");
            _server?.MoveItem(punkItems.GetDisplayName(SIGDN.FILESYSPATH), psiDestinationFolder.GetDisplayName(SIGDN.FILESYSPATH), pszCopyName);
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = false)]
        private delegate void DelegateMoveItems(IFileOperation self, [MarshalAs(UnmanagedType.IUnknown)] object punkItems, IShellItem psiDestinationFolder);
        private void MoveItemsHooked(IFileOperation self, [MarshalAs(UnmanagedType.IUnknown)] object punkItems, IShellItem psiDestinationFolder)
        {
            _server?.ReportMessage("Intercept MoveItems");
            Guid guidIShellItem = typeof(IShellItem).GUID;
            if (Marshal.QueryInterface(Marshal.GetIUnknownForObject(punkItems), ref guidIShellItem, out IntPtr ptrShellItem) == 0)
            {
                IShellItem si = (IShellItem)Marshal.GetObjectForIUnknown(ptrShellItem);
                string src = si.GetDisplayName(SIGDN.FILESYSPATH);
                _server?.MoveItem(src, psiDestinationFolder.GetDisplayName(SIGDN.FILESYSPATH), Path.GetFileName(src));
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = false)]
        private delegate void DelegateDeleteItem(IFileOperation self, IShellItem punkItems, IFileOperationProgressSink pfopsItem);
        private void DeleteItemHooked(IFileOperation self, IShellItem punkItems, IFileOperationProgressSink pfopsItem)
        {
            _server?.ReportMessage("Intercept DeleteItem");
            _server?.DeleteItem(punkItems.GetDisplayName(SIGDN.FILESYSPATH));
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = false)]
        private delegate void DelegateDeleteItems(IFileOperation self, [MarshalAs(UnmanagedType.IUnknown)] object punkItems);
        private void DeleteItemsHooked(IFileOperation self, [MarshalAs(UnmanagedType.IUnknown)] object punkItems)
        {
            _server?.ReportMessage("Intercept DeleteItems");
            Guid guidIShellItem = typeof(IShellItem).GUID;
            Guid guidIDataObject = typeof(IDataObject).GUID;
            if (Marshal.QueryInterface(Marshal.GetIUnknownForObject(punkItems), ref guidIShellItem, out IntPtr ptrShellItem) == 0)
            {
                IShellItem si = (IShellItem)Marshal.GetObjectForIUnknown(ptrShellItem);
                string src = si.GetDisplayName(SIGDN.FILESYSPATH);
                _server?.DeleteItem(src);
            }
            else if (Marshal.QueryInterface(Marshal.GetIUnknownForObject(punkItems), ref guidIDataObject, out IntPtr ptrDataObject) == 0)
            {
                IDataObject dataObject = (IDataObject)Marshal.GetObjectForIUnknown(ptrDataObject);
                FORMATETC format = new()
                {
                    cfFormat = (short)UFormat.CF_HDROP,
                    ptd = IntPtr.Zero,
                    dwAspect = DVASPECT.DVASPECT_CONTENT,
                    lindex = -1,
                    tymed = TYMED.TYMED_HGLOBAL,
                };
                dataObject.GetData(format, out STGMEDIUM medium);
                int offset = (int)Marshal.ReadIntPtr(medium.unionmember);
                IntPtr currentPos = IntPtr.Add(medium.unionmember, offset);
                string src = Marshal.PtrToStringUni(currentPos);
                while (!string.IsNullOrWhiteSpace(src))
                {
                    _server?.DeleteItem(src);
                    int nextFile = System.Text.Encoding.Unicode.GetBytes(src).Length + 2;
                    currentPos = IntPtr.Add(currentPos, nextFile);
                    src = Marshal.PtrToStringUni(currentPos);
                }
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = false)]
        private delegate void DelegateRenameItem(IFileOperation self, IShellItem punkItems, [MarshalAs(UnmanagedType.LPWStr)] string pszNewName, IFileOperationProgressSink pfopsItem);
        private void RenameItemHooked(IFileOperation self, IShellItem punkItems, [MarshalAs(UnmanagedType.LPWStr)] string pszNewName, IFileOperationProgressSink pfopsItem)
        {
            _server?.ReportMessage("Intercept RenameItem");
            _server?.RenameItem(punkItems.GetDisplayName(SIGDN.FILESYSPATH), pszNewName);
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = false)]
        private delegate void DelegateRenameItems(IFileOperation self, [MarshalAs(UnmanagedType.IUnknown)] object punkItems, [MarshalAs(UnmanagedType.LPWStr)] string pszNewName);
        private void RenameItemsHooked(IFileOperation self, [MarshalAs(UnmanagedType.IUnknown)] object punkItems, [MarshalAs(UnmanagedType.LPWStr)] string pszNewName)
        {
            _server?.ReportMessage("Intercept RenameItems");
            Guid guidIShellItem = typeof(IShellItem).GUID;
            if (Marshal.QueryInterface(Marshal.GetIUnknownForObject(punkItems), ref guidIShellItem, out IntPtr ptrShellItem) == 0)
            {
                IShellItem si = (IShellItem)Marshal.GetObjectForIUnknown(ptrShellItem);
                string src = si.GetDisplayName(SIGDN.FILESYSPATH);
                _server?.RenameItem(src, pszNewName);
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = false)]
        private delegate void DelegatePerformOperations(IFileOperation self);
        private void PerformOperationsHooked(IFileOperation self)
        {
            _server.PerformOperations();
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = false)]
        private delegate uint DelegateNewItem(IFileOperation self, IShellItem psiDestinationFolder, FileAttributes dwFileAttributes, [MarshalAs(UnmanagedType.LPWStr)] string pszName, [MarshalAs(UnmanagedType.LPWStr)] string pszTemplateName, IFileOperationProgressSink pfopsItem);
        private uint NewItemHooked(IFileOperation self, IShellItem psiDestinationFolder, FileAttributes dwFileAttributes, [MarshalAs(UnmanagedType.LPWStr)] string pszName, [MarshalAs(UnmanagedType.LPWStr)] string pszTemplateName, IFileOperationProgressSink pfopsItem)
        {
            _server?.ReportMessage("Intercept NewItem");
            _server?.NewItem(psiDestinationFolder.GetDisplayName(SIGDN.FILESYSPATH), dwFileAttributes, pszName);
            return 0;
        }

        /*[UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = false)]
        private delegate void DelegateSetOperationFlags(IFileOperation self, EFileOperation flags);
        private void NewSetOperationFlags(IFileOperation self, EFileOperation flags)
        {
            _server?.ReportMessage("Intercept change flags");
            _server.SetOperationFlags(flags);
        }*/
        #endregion
    }
}
