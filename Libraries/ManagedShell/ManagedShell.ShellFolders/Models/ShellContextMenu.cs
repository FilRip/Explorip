using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Forms;

using ManagedShell.Interop;
using ManagedShell.ShellFolders.Enums;
using ManagedShell.ShellFolders.Interfaces;

namespace ManagedShell.ShellFolders.Models
{
    internal class ShellContextMenu : NativeWindow
    {
        private IContextMenu _oContextMenu;
        private IContextMenu2 _oContextMenu2;
        private IContextMenu3 _oContextMenu3;
        private IShellFolder _oDesktopFolder;
        private IShellFolder _oParentFolder;
        private IntPtr[] _arrPIDLs;
        private string _strParentFolder;

        private const int MAX_PATH = 260;
        private const uint CMD_FIRST = 1;
        private const uint CMD_LAST = 30000;

        private const int S_OK = 0;

        internal ShellContextMenu()
        {
            CreateHandle(new CreateParams());
        }

        ~ShellContextMenu()
        {
            ReleaseAll();
        }

        private bool GetContextMenuInterfaces(IShellFolder oParentFolder, IntPtr[] arrPIDLs, out IntPtr ctxMenuPtr)
        {
            Guid cmGuid = typeof(IContextMenu).GUID;
            int nResult = oParentFolder.GetUIObjectOf(
                IntPtr.Zero,
                (uint)arrPIDLs.Length,
                arrPIDLs,
                ref cmGuid,
                IntPtr.Zero,
                out ctxMenuPtr);

            if (nResult == S_OK)
            {
                _oContextMenu = (IContextMenu)Marshal.GetTypedObjectForIUnknown(ctxMenuPtr, typeof(IContextMenu));
                return true;
            }
            else
            {
                ctxMenuPtr = IntPtr.Zero;
                _oContextMenu = null;
                return false;
            }
        }

        protected override void WndProc(ref Message m)
        {
            if ((_oContextMenu2 != null &&
                (m.Msg == (int)NativeMethods.WM.INITMENUPOPUP ||
                 m.Msg == (int)NativeMethods.WM.MEASUREITEM ||
                 m.Msg == (int)NativeMethods.WM.DRAWITEM)) && (_oContextMenu2.HandleMenuMsg((uint)m.Msg, m.WParam, m.LParam) == S_OK))
            {
                return;
            }

            if ((_oContextMenu3 != null &&
                m.Msg == (int)NativeMethods.WM.MENUCHAR) && (_oContextMenu3.HandleMenuMsg2((uint)m.Msg, m.WParam, m.LParam, IntPtr.Zero) == S_OK))
            {
                return;
            }

            base.WndProc(ref m);
        }

        private void InvokeCommand(IContextMenu oContextMenu, uint nCmd, string strFolder, Point pointInvoke)
        {
            Structs.CmInvokeCommandInfoEx invoke = new()
            {
                cbSize = Marshal.SizeOf(typeof(Structs.CmInvokeCommandInfoEx)),
                lpVerb = (IntPtr)(nCmd - CMD_FIRST),
                lpDirectory = strFolder,
                lpVerbW = (IntPtr)(nCmd - CMD_FIRST),
                lpDirectoryW = strFolder,
                fMask = CMIC.UNICODE | CMIC.PTINVOKE |
                ((Control.ModifierKeys & Keys.Control) != 0 ? CMIC.CONTROL_DOWN : 0) |
                ((Control.ModifierKeys & Keys.Shift) != 0 ? CMIC.SHIFT_DOWN : 0),
                ptInvoke = new NativeMethods.Point((long)pointInvoke.X, (long)pointInvoke.Y),
                nShow = NativeMethods.WindowShowStyle.ShowNormal,
            };

            oContextMenu.InvokeCommand(ref invoke);
        }

        private void ReleaseAll()
        {
            if (_oContextMenu != null)
            {
                Marshal.ReleaseComObject(_oContextMenu);
                _oContextMenu = null;
            }
            if (_oContextMenu2 != null)
            {
                Marshal.ReleaseComObject(_oContextMenu2);
                _oContextMenu2 = null;
            }
            if (_oContextMenu3 != null)
            {
                Marshal.ReleaseComObject(_oContextMenu3);
                _oContextMenu3 = null;
            }
            if (_oDesktopFolder != null)
            {
                Marshal.ReleaseComObject(_oDesktopFolder);
                _oDesktopFolder = null;
            }
            if (_oParentFolder != null)
            {
                Marshal.ReleaseComObject(_oParentFolder);
                _oParentFolder = null;
            }
            if (_arrPIDLs != null)
            {
                FreePIDLs(_arrPIDLs);
                _arrPIDLs = null;
            }
        }

        private IShellFolder GetDesktopFolder()
        {
            if (_oDesktopFolder == null)
            {
                // Get desktop IShellFolder
                int nResult = NativeMethods.SHGetDesktopFolder(out IntPtr pUnkownDesktopFolder);
                if (nResult != S_OK)
                    throw new Exceptions.ShellContextMenuException("Failed to get the desktop shell folder");

                _oDesktopFolder = (IShellFolder)Marshal.GetTypedObjectForIUnknown(pUnkownDesktopFolder, typeof(IShellFolder));
            }

            return _oDesktopFolder;
        }

        private IShellFolder GetParentFolder(string folderName)
        {
            if (_oParentFolder == null)
            {
                IShellFolder oDesktopFolder = GetDesktopFolder();
                if (oDesktopFolder == null)
                    return null;

                // Get the PIDL for the folder file is in
                uint pchEaten = 0;
                SFGAO pdwAttributes = 0;
                int nResult = oDesktopFolder.ParseDisplayName(IntPtr.Zero, IntPtr.Zero, folderName, ref pchEaten, out IntPtr pPIDL, ref pdwAttributes);
                if (nResult != S_OK)
                    return null;

                IntPtr pStrRet = Marshal.AllocCoTaskMem(MAX_PATH * 2 + 4);
                Marshal.WriteInt32(pStrRet, 0, 0);
                _oDesktopFolder.GetDisplayNameOf(pPIDL, SHGDN.FORPARSING, pStrRet);
                StringBuilder strFolder = new(MAX_PATH);
                NativeMethods.StrRetToBuf(pStrRet, pPIDL, strFolder, MAX_PATH);
                Marshal.FreeCoTaskMem(pStrRet);
                _strParentFolder = strFolder.ToString();

                // Get the IShellFolder for folder
                Guid sfGuid = typeof(IShellFolder).GUID;
                nResult = oDesktopFolder.BindToObject(pPIDL, IntPtr.Zero, ref sfGuid, out IntPtr pUnknownParentFolder);
                // Free the PIDL first
                Marshal.FreeCoTaskMem(pPIDL);
                if (nResult != S_OK)
                    return null;

                _oParentFolder = (IShellFolder)Marshal.GetTypedObjectForIUnknown(pUnknownParentFolder, typeof(IShellFolder));
            }

            return _oParentFolder;
        }

        private IntPtr[] GetPIDLs(FileSystemInfo[] arrFI)
        {
            if (null == arrFI || 0 == arrFI.Length)
                return null;

            IShellFolder oParentFolder = GetParentFolder(Directory.GetParent(arrFI[0].FullName).FullName);
            if (oParentFolder == null)
                return null;

            IntPtr[] arrPIDLs = new IntPtr[arrFI.Length];
            int n = 0;
            foreach (FileSystemInfo fi in arrFI)
            {
                // Get the file relative to folder
                uint pchEaten = 0;
                SFGAO pdwAttributes = 0;
                int nResult = oParentFolder.ParseDisplayName(IntPtr.Zero, IntPtr.Zero, fi.Name, ref pchEaten, out IntPtr pPIDL, ref pdwAttributes);
                if (nResult != S_OK)
                {
                    FreePIDLs(arrPIDLs);
                    return null;
                }
                arrPIDLs[n] = pPIDL;
                n++;
            }

            return arrPIDLs;
        }

        private void FreePIDLs(IntPtr[] arrPIDLs)
        {
            if (arrPIDLs != null)
                for (int n = 0; n < arrPIDLs.Length; n++)
                    if (arrPIDLs[n] != IntPtr.Zero)
                    {
                        Marshal.FreeCoTaskMem(arrPIDLs[n]);
                        arrPIDLs[n] = IntPtr.Zero;
                    }
        }

        internal void ShowContextMenu(FileSystemInfo[] filesAndDirs, Point pointScreen)
        {
            ReleaseAll();
            _arrPIDLs = GetPIDLs(filesAndDirs);
            ShowContextMenu(pointScreen);
        }

        private void ShowContextMenu(Point pointScreen)
        {
            IntPtr pMenu = IntPtr.Zero,
                iContextMenuPtr = IntPtr.Zero,
                iContextMenuPtr2 = IntPtr.Zero,
                iContextMenuPtr3 = IntPtr.Zero;

            try
            {
                if (_arrPIDLs == null)
                {
                    ReleaseAll();
                    return;
                }

                if (!GetContextMenuInterfaces(_oParentFolder, _arrPIDLs, out iContextMenuPtr))
                {
                    ReleaseAll();
                    return;
                }

                pMenu = NativeMethods.CreatePopupMenu();

                _oContextMenu.QueryContextMenu(
                    pMenu,
                    0,
                    CMD_FIRST,
                    CMD_LAST,
                    CMF.EXPLORE |
                    CMF.NORMAL |
                    ((Control.ModifierKeys & Keys.Shift) != 0 ? CMF.EXTENDEDVERBS : 0));

                Guid cm2Guid = typeof(IContextMenu2).GUID;
                Guid cm3Guid = typeof(IContextMenu3).GUID;
                Marshal.QueryInterface(iContextMenuPtr, ref cm2Guid, out iContextMenuPtr2);
                Marshal.QueryInterface(iContextMenuPtr, ref cm3Guid, out iContextMenuPtr3);

                _oContextMenu2 = (IContextMenu2)Marshal.GetTypedObjectForIUnknown(iContextMenuPtr2, typeof(IContextMenu2));
                _oContextMenu3 = (IContextMenu3)Marshal.GetTypedObjectForIUnknown(iContextMenuPtr3, typeof(IContextMenu3));

                uint nSelected = NativeMethods.TrackPopupMenuEx(
                    pMenu,
                    NativeMethods.TPM.RETURNCMD,
                    (int)pointScreen.X,
                    (int)pointScreen.Y,
                    this.Handle,
                    IntPtr.Zero);

                NativeMethods.DestroyMenu(pMenu);
                pMenu = IntPtr.Zero;

                if (nSelected != 0)
                    InvokeCommand(_oContextMenu, nSelected, _strParentFolder, pointScreen);
            }
            finally
            {
                if (pMenu != IntPtr.Zero)
                    NativeMethods.DestroyMenu(pMenu);

                if (iContextMenuPtr != IntPtr.Zero)
                    Marshal.Release(iContextMenuPtr);

                if (iContextMenuPtr2 != IntPtr.Zero)
                    Marshal.Release(iContextMenuPtr2);

                if (iContextMenuPtr3 != IntPtr.Zero)
                    Marshal.Release(iContextMenuPtr3);

                ReleaseAll();
            }
        }
    }
}
