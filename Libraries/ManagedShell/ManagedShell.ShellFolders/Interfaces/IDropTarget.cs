using System;
using System.Runtime.InteropServices;
using System.Windows;

using Point = ManagedShell.Interop.NativeMethods.PointInt;

namespace ManagedShell.ShellFolders.Interfaces;

[Flags()]
public enum MK
{
    LBUTTON = 0x0001,
    RBUTTON = 0x0002,
    SHIFT = 0x0004,
    CONTROL = 0x0008,
    MBUTTON = 0x0010,
    ALT = 0x0020
}

[ComImport()]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("00000122-0000-0000-C000-000000000046")]
public interface IDropTarget
{
    // Determines whether a drop can be accepted and its effect if it is accepted
    [PreserveSig()]
    int DragEnter(
        IntPtr pDataObj,
        MK grfKeyState,
        Point pt,
        ref DragDropEffects pdwEffect);

    // Provides target feedback to the user through the DoDragDrop function
    [PreserveSig()]
    int DragOver(
        MK grfKeyState,
        Point pt,
        ref DragDropEffects pdwEffect);

    // Causes the drop target to suspend its feedback actions
    [PreserveSig()]
    int DragLeave();

    // Drops the data into the target window
    [PreserveSig()]
    int DragDrop(
        IntPtr pDataObj,
        MK grfKeyState,
        Point pt,
        ref DragDropEffects pdwEffect);
}
