﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Explorip.FilesOperations.Interfaces;

using ManagedShell.Interop;
using ManagedShell.ShellFolders.Enums;
using ManagedShell.ShellFolders.Interfaces;

namespace Explorip.FilesOperations
{
    public class FileOperationProgressSink : IFileOperationProgressSink
    {
        public virtual void StartOperations()
        {
            TraceAction("StartOperations", "", 0);
        }

        public virtual void FinishOperations(uint hrResult)
        {
            TraceAction("FinishOperations", "", hrResult);
        }

        public virtual void PreRenameItem(uint dwFlags,
            IShellItem psiItem, string pszNewName)
        {
            TraceAction("PreRenameItem", psiItem, 0);
        }

        public virtual void PostRenameItem(uint dwFlags,
            IShellItem psiItem, string pszNewName,
            uint hrRename, IShellItem psiNewlyCreated)
        {
            TraceAction("PostRenameItem", psiNewlyCreated, hrRename);
        }

        public virtual void PreMoveItem(
            uint dwFlags, IShellItem psiItem,
            IShellItem psiDestinationFolder, string pszNewName)
        {
            TraceAction("PreMoveItem", psiItem, 0);
        }

        public virtual void PostMoveItem(
            uint dwFlags, IShellItem psiItem,
            IShellItem psiDestinationFolder,
            string pszNewName, uint hrMove,
            IShellItem psiNewlyCreated)
        {
            TraceAction("PostMoveItem", psiNewlyCreated, hrMove);
        }

        public virtual void PreCopyItem(
            uint dwFlags, IShellItem psiItem,
            IShellItem psiDestinationFolder, string pszNewName)
        {
            TraceAction("PreCopyItem", psiItem, 0);
        }

        public virtual void PostCopyItem(
            uint dwFlags, IShellItem psiItem,
            IShellItem psiDestinationFolder, string pszNewName,
            uint hrCopy, IShellItem psiNewlyCreated)
        {
            TraceAction("PostCopyItem", psiNewlyCreated, hrCopy);
        }

        public virtual void PreDeleteItem(
            uint dwFlags, IShellItem psiItem)
        {
            TraceAction("PreDeleteItem", psiItem, 0);
        }

        public virtual void PostDeleteItem(
            uint dwFlags, IShellItem psiItem,
            uint hrDelete, IShellItem psiNewlyCreated)
        {
            TraceAction("PostDeleteItem", psiItem, hrDelete);
        }

        public virtual void PreNewItem(uint dwFlags,
            IShellItem psiDestinationFolder, string pszNewName)
        {
            TraceAction("PreNewItem", pszNewName, 0);
        }

        public virtual void PostNewItem(uint dwFlags,
            IShellItem psiDestinationFolder, string pszNewName,
            string pszTemplateName, uint dwFileAttributes,
            uint hrNew, IShellItem psiNewItem)
        {
            TraceAction("PostNewItem", psiNewItem, hrNew);
        }

        public virtual void UpdateProgress(
            uint iWorkTotal, uint iWorkSoFar)
        {
            Debug.WriteLine("UpdateProgress: " + iWorkSoFar + "/" + iWorkTotal);
        }

        public void ResetTimer() { /* Empty method ?! */ }
        public void PauseTimer() { /* Empty method ?! */ }
        public void ResumeTimer() { /* Empty method ?! */ }

        [Conditional("DEBUG")]
        private static void TraceAction(
            string action, string item, uint hresult)
        {
            string message = string.Format(
                "{0} ({1})", action, (CopyEngineResult)hresult);
            if (!string.IsNullOrEmpty(item)) message += " : " + item;
            Debug.WriteLine(message);
        }

        [Conditional("DEBUG")]
        private static void TraceAction(
            string action, IShellItem item, uint hresult)
        {
            string displayName = "";
            if (item?.GetDisplayName(SIGDN.NORMALDISPLAY, out IntPtr ptrDisplayName) == NativeMethods.S_OK && ptrDisplayName != IntPtr.Zero)
            {
                displayName = Marshal.PtrToStringAuto(ptrDisplayName);
                Marshal.FreeCoTaskMem(ptrDisplayName);
            }    
            TraceAction(action,
                displayName,
                hresult);
        }
    }
}
