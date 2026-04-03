using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

using ManagedShell.Interop;
using ManagedShell.ShellFolders.Enums;
using ManagedShell.ShellFolders.Interfaces;
using ManagedShell.ShellFolders.Structs;

namespace ManagedShell.ShellFolders.Models;

internal class ShellContextMenu : NativeWindow
{
    private IContextMenu _oContextMenu;
    private IContextMenu2 _oContextMenu2;
    private IContextMenu3 _oContextMenu3;
    private IShellFolder _oDesktopFolder;
    private IShellFolder _oParentFolder;
    private IShellView _backgroundShellView;
    private IntPtr[] _arrPIDLs;
    private string _strParentFolder;

    private const int MAX_PATH = 260;

    public class InvokeCommandEventArgs : EventArgs
    {
        public uint NumCommand { get; set; }

        public string TextCommand { get; set; }

        public bool Handled { get; set; }

        public string Folder { get; set; }
    }

    public event EventHandler<InvokeCommandEventArgs> InvokeCommandCallback;

    internal ShellContextMenu()
    {
        Init();
    }

    private void Init()
    {
        CreateHandle(new CreateParams());
    }

    ~ShellContextMenu()
    {
        ReleaseAll();
    }

    public bool AutoExpandSubMenu { get; set; }

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

        if (nResult == (int)NativeMethods.HResult.SUCCESS)
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
             m.Msg == (int)NativeMethods.WM.DRAWITEM)) && (_oContextMenu2.HandleMenuMsg((uint)m.Msg, m.WParam, m.LParam) == (int)NativeMethods.HResult.SUCCESS))
        {
            return;
        }

        if ((_oContextMenu3 != null &&
            m.Msg == (int)NativeMethods.WM.MENUCHAR) && (_oContextMenu3.HandleMenuMsg2((uint)m.Msg, m.WParam, m.LParam, IntPtr.Zero) == (int)NativeMethods.HResult.SUCCESS))
        {
            return;
        }

        base.WndProc(ref m);
    }

    private void InvokeCommand(IContextMenu oContextMenu, uint nCmd, string strFolder, Point pointInvoke)
    {
        if (InvokeCommandCallback != null)
        {
            byte[] name = new byte[MAX_PATH];
            oContextMenu.GetCommandString(nCmd, GetCommandStrings.VERBW, 0, name, MAX_PATH);
            InvokeCommandEventArgs args = new()
            {
                NumCommand = nCmd,
                Folder = strFolder,
                TextCommand = Encoding.UTF8.GetString(name).Split((char)0)[0],
            };
            InvokeCommandCallback.Invoke(this, args);
            if (args.Handled)
                return;
        }
        CmInvokeCommandInfoEx invoke = new()
        {
            cbSize = Marshal.SizeOf(typeof(CmInvokeCommandInfoEx)),
            lpVerb = new IntPtr(nCmd),
            lpVerbW = new IntPtr(nCmd),
            lpDirectory = strFolder,
            lpDirectoryW = strFolder,
            fMask = ContextMenuInfoCommands.UNICODE | ContextMenuInfoCommands.PTINVOKE |
            ((Control.ModifierKeys & Keys.Control) != 0 ? ContextMenuInfoCommands.CONTROL_DOWN : 0) |
            ((Control.ModifierKeys & Keys.Shift) != 0 ? ContextMenuInfoCommands.SHIFT_DOWN : 0),
            ptInvoke = new NativeMethods.Point((long)pointInvoke.X, (long)pointInvoke.Y),
            nShow = NativeMethods.WindowShowStyle.ShowNormal,
            hwnd = this.Handle,
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
        if (_backgroundShellView != null)
        {
            Marshal.ReleaseComObject(_backgroundShellView);
            _backgroundShellView = null;
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
            if (nResult != (int)NativeMethods.HResult.SUCCESS)
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
            ShellFolderGetAttributeObjects pdwAttributes = 0;
            int nResult = oDesktopFolder.ParseDisplayName(IntPtr.Zero, IntPtr.Zero, folderName, ref pchEaten, out IntPtr pPIDL, ref pdwAttributes);
            if (nResult != (int)NativeMethods.HResult.SUCCESS)
                return null;

            IntPtr pStrRet = Marshal.AllocCoTaskMem(MAX_PATH * 2 + 4);
            Marshal.WriteInt32(pStrRet, 0, 0);
            _oDesktopFolder.GetDisplayNameOf(pPIDL, SHGetDisplayNames.FORPARSING, pStrRet);
            StringBuilder strFolder = new(MAX_PATH);
            NativeMethods.StrRetToBuf(pStrRet, pPIDL, strFolder, MAX_PATH);
            Marshal.FreeCoTaskMem(pStrRet);
            _strParentFolder = strFolder.ToString();

            // Get the IShellFolder for folder
            Guid sfGuid = typeof(IShellFolder).GUID;
            nResult = oDesktopFolder.BindToObject(pPIDL, IntPtr.Zero, ref sfGuid, out IntPtr pUnknownParentFolder);
            // Free the PIDL first
            Marshal.FreeCoTaskMem(pPIDL);
            if (nResult != (int)NativeMethods.HResult.SUCCESS)
                return null;

            _oParentFolder = (IShellFolder)Marshal.GetTypedObjectForIUnknown(pUnknownParentFolder, typeof(IShellFolder));
        }

        return _oParentFolder;
    }

    private IntPtr GetPIDL(string shell)
    {
        IntPtr pidl = IntPtr.Zero;
        if (shell.Contains("B4BFCC3A-DB2C-424C-B029-7FE99A87C641", StringComparison.OrdinalIgnoreCase))
        {
            _strParentFolder = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            _oParentFolder = GetDesktopFolder();
            _oParentFolder.CreateViewObject(IntPtr.Zero, typeof(IShellView).GUID, out IntPtr opShellView);
            _backgroundShellView = (IShellView)Marshal.GetObjectForIUnknown(opShellView);
            _oContextMenu = (IContextMenu)_backgroundShellView.GetItemObject(ShellViewGetItemObject.Background, typeof(IContextMenu).GUID);
        }
        else
        {
            uint pch = 0;
            GetDesktopFolder().ParseDisplayName(IntPtr.Zero, IntPtr.Zero, shell, ref pch, out pidl, 0);
            _oParentFolder = GetDesktopFolder();
            Guid guidCM = typeof(IContextMenu).GUID;
            _oParentFolder.GetUIObjectOf(IntPtr.Zero, 1, [pidl], ref guidCM, IntPtr.Zero, out IntPtr cm);
            _oContextMenu = (IContextMenu)Marshal.GetTypedObjectForIUnknown(cm, typeof(IContextMenu));
        }
        return pidl;
    }

    private IntPtr[] GetPIDLs(FileSystemInfo[] arrFI, bool background = false)
    {
        if (null == arrFI || 0 == arrFI.Length)
            return null;

        if (background)
        {
            IntPtr pItemIDL = NativeMethods.ILCreateFromPath(arrFI[0].FullName);
            GetDesktopFolder().BindToObject(pItemIDL, IntPtr.Zero, typeof(IShellFolder).GUID, out IntPtr opsf);
            NativeMethods.ILFree(pItemIDL);
            IShellFolder psf = (IShellFolder)Marshal.GetTypedObjectForIUnknown(opsf, typeof(IShellFolder));
            psf.CreateViewObject(IntPtr.Zero, typeof(IShellView).GUID, out IntPtr opShellView);
            IShellView pShellView = (IShellView)Marshal.GetTypedObjectForIUnknown(opShellView, typeof(IShellView));
            object opContextMenu = pShellView.GetItemObject(ShellViewGetItemObject.Background, typeof(IContextMenu).GUID);
            _oContextMenu = (IContextMenu)opContextMenu;
            Marshal.ReleaseComObject(psf);
            Marshal.ReleaseComObject(pShellView);
        }

        IShellFolder oParentFolder = GetParentFolder(Directory.GetParent(arrFI[0].FullName).FullName);
        if (oParentFolder == null)
            return null;

        IntPtr[] arrPIDLs = new IntPtr[arrFI.Length];
        int n = 0;
        foreach (FileSystemInfo fi in arrFI)
        {
            // Get the file relative to folder
            uint pchEaten = 0;
            ShellFolderGetAttributeObjects pdwAttributes = 0;
            int nResult = oParentFolder.ParseDisplayName(IntPtr.Zero, IntPtr.Zero, fi.Name, ref pchEaten, out IntPtr pPIDL, ref pdwAttributes);
            if (nResult != (int)NativeMethods.HResult.SUCCESS)
            {
                FreePIDLs(arrPIDLs);
                return null;
            }
            arrPIDLs[n] = pPIDL;
            n++;
        }

        return arrPIDLs;
    }

    private static void FreePIDLs(IntPtr[] arrPIDLs)
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

    internal void ShowContextMenu(DirectoryInfo dir, Point pointScreen)
    {
        ReleaseAll();
        _arrPIDLs = GetPIDLs([dir], true);
        ShowContextMenu(pointScreen, true);
    }

    internal void ShowContextMenu(string shell, Point pointScreen, bool background = false)
    {
        ReleaseAll();
        _arrPIDLs = [GetPIDL(shell)];
        ShowContextMenu(pointScreen, background);
    }

    private void ShowContextMenu(Point pointScreen, bool background = false)
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

            if (!background && !GetContextMenuInterfaces(_oParentFolder, _arrPIDLs, out iContextMenuPtr))
            {
                ReleaseAll();
                return;
            }

            pMenu = NativeMethods.CreatePopupMenu();

            ContextMenuStates flags = ContextMenuStates.NORMAL | ContextMenuStates.EXPLORE;
            if (!background)
            {
                if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                    flags |= ContextMenuStates.EXTENDEDVERBS;
                flags |= ContextMenuStates.CANRENAME;
            }
            _oContextMenu.QueryContextMenu(
                pMenu,
                0,
                0,
                (uint)short.MaxValue,
                flags);

            if (iContextMenuPtr != IntPtr.Zero)
            {
                Guid cm2Guid = typeof(IContextMenu2).GUID;
                Guid cm3Guid = typeof(IContextMenu3).GUID;
                Marshal.QueryInterface(iContextMenuPtr, ref cm2Guid, out iContextMenuPtr2);
                Marshal.QueryInterface(iContextMenuPtr, ref cm3Guid, out iContextMenuPtr3);

                _oContextMenu2 = (IContextMenu2)Marshal.GetTypedObjectForIUnknown(iContextMenuPtr2, typeof(IContextMenu2));
                _oContextMenu3 = (IContextMenu3)Marshal.GetTypedObjectForIUnknown(iContextMenuPtr3, typeof(IContextMenu3));
            }
            else
            {
                _oContextMenu2 = (IContextMenu2)_oContextMenu;
                _oContextMenu3 = (IContextMenu3)_oContextMenu;
            }

            if (AutoExpandSubMenu)
                ExpandSubMenu(pMenu);

            uint nSelected = NativeMethods.TrackPopupMenuEx(
                pMenu,
                NativeMethods.TrackPopUpMenuActions.RETURNCMD | NativeMethods.TrackPopUpMenuActions.RIGHTBUTTON,
                (int)pointScreen.X,
                (int)pointScreen.Y,
                this.Handle,
                IntPtr.Zero);

            if (nSelected != 0)
                InvokeCommand(_oContextMenu, nSelected, _strParentFolder, pointScreen);

            NativeMethods.DestroyMenu(pMenu);
            pMenu = IntPtr.Zero;
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

    private void ExpandSubMenu(IntPtr pMenu)
    {
        if (pMenu == IntPtr.Zero || _oContextMenu2 == null)
            return;
        int nbMenu = NativeMethods.GetMenuItemCount(pMenu);
        if (nbMenu > 0)
        {
            int IdMenu;
            for (int i = 0; i < nbMenu; i++)
            {
                IdMenu = NativeMethods.GetMenuItemID(pMenu, i);
                if (IdMenu < 0)
                {
                    IntPtr IdSubMenu = NativeMethods.GetSubMenu(pMenu, i);
                    _oContextMenu2.HandleMenuMsg((int)NativeMethods.WM.INITMENUPOPUP, IdSubMenu, (IntPtr)i);
                }
            }
        }
    }
}
