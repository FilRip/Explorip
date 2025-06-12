using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

using Explorip.HookFileOperations.Interfaces;
using Explorip.HookFileOperations.Models;

using ExploripCopy.ViewModels;

using ManagedShell.Interop;
using ManagedShell.ShellFolders.Interfaces;

namespace ExploripCopy.Models;

[Serializable()]
internal class InteractionMainProcess : IInteractionMainProcess
{
    public void StartNewFileOperation(List<OneFileOperation> listOperations)
    {
        if (listOperations?.Count > 0)
        {
            foreach (OneFileOperation operation in listOperations.Where(op => op.FileOperation == EFileOperation.Delete &&
                                                                              op.Source.Length > 3 &&
                                                                              op.Source.Substring(3).StartsWith("$Recycle.bin\\", StringComparison.OrdinalIgnoreCase)))
            {
                Guid guidShellItem = typeof(IShellItem).GUID;
                NativeMethods.SHCreateItemFromParsingName(operation.Source, IntPtr.Zero, ref guidShellItem, out IntPtr siPtr);
                if (siPtr != IntPtr.Zero)
                {
                    IShellItem si = (IShellItem)Marshal.GetTypedObjectForIUnknown(siPtr, typeof(IShellItem));
                    if (si.GetDisplayName(ManagedShell.ShellFolders.Enums.SIGDN.NORMALDISPLAY, out IntPtr ptrDisplayName) == NativeMethods.S_OK && ptrDisplayName != IntPtr.Zero)
                    {
                        string filename = Marshal.PtrToStringAuto(ptrDisplayName);
                        operation.SetDisplaySource(Path.GetFileName(filename));
                        Marshal.FreeCoTaskMem(ptrDisplayName);
                    }
                    Marshal.ReleaseComObject(si);
                }
            }
            MainViewModels.Instance.AddOperations(listOperations);
            listOperations[listOperations.Count - 1].ResetChoice = true;
            MainViewModels.Instance.ForceUpdateWaitingList();
        }
    }
}
