using System;
using System.IO;
using System.Runtime.InteropServices;

using ManagedShell.ShellFolders.Interfaces;

namespace Explorip.FilesOperations.Interfaces;

[ComImport()]
[Guid("947aab5f-0a5c-4c13-b4d6-4bf7836fc9f8")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IFileOperation
{
    public uint Advise(IFileOperationProgressSink pfops);
    public void Unadvise(uint dwCookie);

    public void SetOperationFlags(EFileOperation dwOperationFlags);
    public void SetProgressMessage([MarshalAs(UnmanagedType.LPWStr)] string pszMessage);
    public void SetProgressDialog([MarshalAs(UnmanagedType.Interface)] object popd);
    public void SetProperties([MarshalAs(UnmanagedType.Interface)] object pproparray);
    public void SetOwnerWindow(uint hwndParent);

    public void ApplyPropertiesToItem(IShellItem psiItem);
    public void ApplyPropertiesToItems([MarshalAs(UnmanagedType.Interface)] object punkItems);

    public void RenameItem(IShellItem psiItem, [MarshalAs(UnmanagedType.LPWStr)] string pszNewName,
        IFileOperationProgressSink pfopsItem);

    public void RenameItems(
        [MarshalAs(UnmanagedType.Interface)] object pUnkItems,
        [MarshalAs(UnmanagedType.LPWStr)] string pszNewName);

    public void MoveItem(
        IShellItem psiItem,
        IShellItem psiDestinationFolder,
        [MarshalAs(UnmanagedType.LPWStr)] string pszNewName,
        IFileOperationProgressSink pfopsItem);

    public void MoveItems(
        [MarshalAs(UnmanagedType.Interface)] object punkItems,
        IShellItem psiDestinationFolder);

    public void CopyItem(
        IShellItem psiItem,
        IShellItem psiDestinationFolder,
        [MarshalAs(UnmanagedType.LPWStr)] string pszCopyName,
        IFileOperationProgressSink pfopsItem);

    public void CopyItems(
        [MarshalAs(UnmanagedType.Interface)] object punkItems,
        IShellItem psiDestinationFolder);

    public void DeleteItem(
        IShellItem psiItem,
        IFileOperationProgressSink pfopsItem);

    public void DeleteItems([MarshalAs(UnmanagedType.Interface)] object punkItems);

    public uint NewItem(
        IShellItem psiDestinationFolder,
        FileAttributes dwFileAttributes,
        [MarshalAs(UnmanagedType.LPWStr)] string pszName,
        [MarshalAs(UnmanagedType.LPWStr)] string pszTemplateName,
        IFileOperationProgressSink pfopsItem);

    public void PerformOperations();

    [return: MarshalAs(UnmanagedType.Bool)]
    public bool GetAnyOperationsAborted();
}
