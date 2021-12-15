using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Explorip.WinAPI.Modeles;

namespace Explorip.WinAPI
{
    public static class Ole32
    {
        [DllImport("ole32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int DoDragDrop(
            IntPtr pDataObject,
            [MarshalAs(UnmanagedType.Interface)]
            IDropSource pDropSource,
            DragDropEffects dwOKEffect,
            out DragDropEffects pdwEffect);
    }
}
