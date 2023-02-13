using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Explorip.WinAPI.Modeles
{
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
        Int32 DragEnter(
            IntPtr pDataObj,
            MK grfKeyState,
            ManagedPoint pt,
            ref DragDropEffects pdwEffect);

        // Provides target feedback to the user through the DoDragDrop function
        [PreserveSig()]
        Int32 DragOver(
            MK grfKeyState,
            ManagedPoint pt,
            ref DragDropEffects pdwEffect);

        // Causes the drop target to suspend its feedback actions
        [PreserveSig()]
        Int32 DragLeave();

        // Drops the data into the target window
        [PreserveSig()]
        Int32 DragDrop(
            IntPtr pDataObj,
            MK grfKeyState,
            ManagedPoint pt,
            ref DragDropEffects pdwEffect);
    }
}
