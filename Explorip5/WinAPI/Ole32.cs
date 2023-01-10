using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Explorip.WinAPI.Modeles;

using ManagedShell.ShellFolders.Enums;

namespace Explorip.WinAPI
{
    public static class Ole32
    {
        [DllImport("ole32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int DoDragDrop(
            IntPtr pDataObject,
            [MarshalAs(UnmanagedType.Interface)]
            IDropSource pDropSource,
            DragDropEffects dwOKEffect,
            out DragDropEffects pdwEffect);

        [DllImport("ole32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int RegisterDragDrop(
            IntPtr hWnd,
            Modeles.IDropTarget IdropTgt);

        // Retrieves a drag/drop helper interface for drawing the drag/drop images
        [DllImport("ole32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int CoCreateInstance(
            ref Guid rclsid,
            IntPtr pUnkOuter,
            CLSCTX dwClsContext,
            ref Guid riid,
            out IntPtr ppv);

        // Revokes the registration of the specified application window as a potential target for 
        // OLE drag-and-drop operations
        [DllImport("ole32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int RevokeDragDrop(IntPtr hWnd);
    }
}
