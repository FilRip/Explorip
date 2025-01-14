using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace ManagedShell.ShellFolders.Interfaces;

[ComImport()]
[Guid("00000121-0000-0000-C000-000000000046")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IDropSource
{
    // Determines whether a drag-and-drop operation should continue
    [PreserveSig()]
    Int32 QueryContinueDrag(
        bool fEscapePressed,
        MK grfKeyState);

    // Gives visual feedback to an end user during a drag-and-drop operation
    [PreserveSig()]
    Int32 GiveFeedback(
        DragDropEffects dwEffect);
}
