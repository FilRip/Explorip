using System;
using System.Runtime.InteropServices;
using System.Windows;

using Point = ManagedShell.Interop.NativeMethods.PointInt;

namespace ManagedShell.ShellFolders.Interfaces;

[ComImport()]
[Guid("4657278B-411B-11d2-839A-00C04FD918D0")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IDropTargetHelper
{
    // Notifies the drag-image manager that the drop target's IDropTarget::DragEnter method has been called
    [PreserveSig]
    int DragEnter(
        IntPtr hwndTarget,
        IntPtr pDataObject,
        ref Point ppt,
        DragDropEffects dwEffect);

    // Notifies the drag-image manager that the drop target's IDropTarget::DragLeave method has been called
    [PreserveSig]
    int DragLeave();

    // Notifies the drag-image manager that the drop target's IDropTarget::DragOver method has been called
    [PreserveSig]
    int DragOver(
        ref Point ppt,
        DragDropEffects dwEffect);

    // Notifies the drag-image manager that the drop target's IDropTarget::Drop method has been called
    [PreserveSig]
    int Drop(
        IntPtr pDataObject,
        ref Point ppt,
        DragDropEffects dwEffect);

    // Notifies the drag-image manager to show or hide the drag image
    [PreserveSig]
    int Show(
        bool fShow);
}
