using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using Explorip.Exceptions;
using Explorip.WinAPI;
using Explorip.WinAPI.Modeles;

using IContextMenu = Explorip.WinAPI.Modeles.IContextMenu;
using IContextMenu2 = Explorip.WinAPI.Modeles.IContextMenu2;
using IContextMenu3 = Explorip.WinAPI.Modeles.IContextMenu3;
using IEnumIDList = Explorip.WinAPI.Modeles.IEnumIDList;
using IShellFolder = Explorip.WinAPI.Modeles.IShellFolder;

namespace Explorip.Helpers
{
    // TODO : Ouvrir avec : https://social.msdn.microsoft.com/Forums/en-US/d513e241-8f7e-419c-b87f-0b653abb3d83/translate?forum=Offtopic

    public class ShellContextMenu : NativeWindow
    {
        #region Champs

        private IntPtr pMenu = IntPtr.Zero,
            iContextMenuPtr = IntPtr.Zero,
            iContextMenuPtr2 = IntPtr.Zero,
            iContextMenuPtr3 = IntPtr.Zero;
        private IContextMenu _oContextMenu;
        private IContextMenu2 _oContextMenu2;
        private IContextMenu3 _oContextMenu3;
        private IShellFolder _oDesktopFolder;
        private IShellFolder _oParentFolder;
        private IntPtr[] _arrPIDLs;
        private string _strParentFolder;
        private const uint CMD_FIRST = 0;
        private const uint CMD_LAST = (uint)short.MaxValue;

        private static readonly int cbInvokeCommand = Marshal.SizeOf(typeof(CmInvokeCommandInfoEx));

        private Point _positionSouris;

        #endregion

        #region Constructor
        /// <summary>Default constructor</summary>
        public ShellContextMenu()
        {
            CleanUp();
            CreateHandle(new CreateParams());
        }
        #endregion

        #region Destructor
        /// <summary>Ensure all resources get released</summary>
        ~ShellContextMenu()
        {
            ReleaseAll();
            CleanUp();
        }
        #endregion

        #region GetContextMenuInterfaces()
        /// <summary>Gets the interfaces to the context menu</summary>
        /// <param name="oParentFolder">Parent folder</param>
        /// <param name="arrPIDLs">PIDLs</param>
        /// <returns>true if it got the interfaces, otherwise false</returns>
        private bool GetContextMenuInterfaces(IntPtr[] arrPIDLs, out IntPtr ctxMenuPtr)
        {
            Guid guid = typeof(IContextMenu).GUID;
            int nResult = _oParentFolder.GetUIObjectOf(
                IntPtr.Zero,
                (uint)arrPIDLs.Length,
                arrPIDLs,
                ref guid,
                IntPtr.Zero,
                out ctxMenuPtr);

            if (nResult == (int)Commun.HRESULT.S_OK)
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
        #endregion

        #region Override

        protected override void WndProc(ref Message m)
        {
            if (_oContextMenu2 != null &&
                (m.Msg == (int)Commun.WM.INITMENUPOPUP ||
                 m.Msg == (int)Commun.WM.MEASUREITEM ||
                 m.Msg == (int)Commun.WM.DRAWITEM) &&
                 _oContextMenu2.HandleMenuMsg((uint)m.Msg, m.WParam, m.LParam) == (int)Commun.HRESULT.S_OK)
            {
                return;
            }

            if (_oContextMenu3 != null &&
                m.Msg == (int)Commun.WM.MENUCHAR &&
                _oContextMenu3.HandleMenuMsg2((uint)m.Msg, m.WParam, m.LParam, IntPtr.Zero) == (int)Commun.HRESULT.S_OK)
            {
                return;
            }

            base.WndProc(ref m);
        }

        #endregion

        #region InvokeCommand
        private void InvokeCommand(IContextMenu oContextMenu, uint nCmd, string strFolder, Point pointInvoke)
        {
            CmInvokeCommandInfoEx invoke = new()
            {
                cbSize = cbInvokeCommand,
                lpVerb = (IntPtr)(nCmd - CMD_FIRST),
                lpDirectory = strFolder,
                lpVerbW = (IntPtr)(nCmd - CMD_FIRST),
                lpDirectoryW = strFolder,
                fMask = CMIC.UNICODE | CMIC.PTINVOKE |
                ((Control.ModifierKeys & Keys.Control) != 0 ? CMIC.CONTROL_DOWN : 0) |
                ((Control.ModifierKeys & Keys.Shift) != 0 ? CMIC.SHIFT_DOWN : 0),
                ptInvoke = new ManagedPoint(pointInvoke.X, pointInvoke.Y),
                nShow = SW.SHOWNORMAL
            };
            oContextMenu.InvokeCommand(ref invoke);
        }
        #endregion

        #region ReleaseAll()
        /// <summary>
        /// Release all allocated interfaces, PIDLs 
        /// </summary>
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
        #endregion

        #region GetDesktopFolder()
        /// <summary>
        /// Gets the desktop folder
        /// </summary>
        /// <returns>IShellFolder for desktop folder</returns>
        private IShellFolder GetDesktopFolder()
        {
            if (_oDesktopFolder == null)
            {
                // Get desktop IShellFolder
                int nResult = Shell32.SHGetDesktopFolder(out IntPtr pUnkownDesktopFolder);
                if (nResult != (int)Commun.HRESULT.S_OK)
                    throw new ShellContextMenuException("Failed to get the desktop shell folder");

                _oDesktopFolder = (IShellFolder)Marshal.GetTypedObjectForIUnknown(pUnkownDesktopFolder, typeof(IShellFolder));
            }

            return _oDesktopFolder;
        }
        #endregion

        #region GetParentFolder()
        /// <summary>
        /// Gets the parent folder
        /// </summary>
        /// <param name="folderName">Folder path</param>
        /// <returns>IShellFolder for the folder (relative from the desktop)</returns>
        private IShellFolder GetParentFolder(string folderName, bool desktopParent)
        {
            if (!string.IsNullOrWhiteSpace(folderName))
            {
                if (_oParentFolder == null)
                {
                    IShellFolder oDesktopFolder = GetDesktopFolder();
                    if (oDesktopFolder == null)
                        return null;

                    uint pchEaten = 0;
                    SFGAO pdwAttributes = 0;

                    // Get the PIDL for the folder file is in
                    int nResult = oDesktopFolder.ParseDisplayName(IntPtr.Zero, IntPtr.Zero, folderName, ref pchEaten, out IntPtr pPIDL, ref pdwAttributes);
                    if (nResult != (int)Commun.HRESULT.S_OK)
                        return null;

                    IntPtr pStrRet = Marshal.AllocCoTaskMem(ManagedShell.Common.Helpers.ShellHelper.MAX_PATH * 2 + 4);
                    Marshal.WriteInt32(pStrRet, 0, 0);
                    _ = _oDesktopFolder.GetDisplayNameOf(pPIDL, SHGNO.FORPARSING, pStrRet);
                    StringBuilder strFolder = new(ManagedShell.Common.Helpers.ShellHelper.MAX_PATH);
                    Shlwapi.StrRetToBuf(pStrRet, pPIDL, strFolder, ManagedShell.Common.Helpers.ShellHelper.MAX_PATH);
                    Marshal.FreeCoTaskMem(pStrRet);
                    _strParentFolder = strFolder.ToString();

                    // Get the IShellFolder for folder
                    Guid guid = typeof(IShellFolder).GUID;
                    nResult = oDesktopFolder.BindToObject(pPIDL, IntPtr.Zero, ref guid, out object pUnknownParentFolder);
                    // Free the PIDL first
                    Marshal.FreeCoTaskMem(pPIDL);
                    if (nResult != (int)Commun.HRESULT.S_OK)
                        return null;

                    _oParentFolder = (IShellFolder)pUnknownParentFolder;
                }
            }
            else
            {
                if (desktopParent)
                    _oParentFolder = GetDesktopFolder();
                else
                {
                    IntPtr folderEnumPtr = IntPtr.Zero;
                    IEnumIDList folderEnum = null;
                    IntPtr winHandle = IntPtr.Zero;

                    IntPtr tempPidl;
                    ShFileInfo info;

                    info = new ShFileInfo();
                    tempPidl = IntPtr.Zero;
                    Shell32.SHGetSpecialFolderLocation(IntPtr.Zero, Shell32.CSIDL.DRIVES, ref tempPidl);

                    Shell32.SHGetFileInfo(tempPidl, 0, ref info, (uint)Marshal.SizeOf(info), Shell32.SHGFI.PIDL | Shell32.SHGFI.DISPLAYNAME | Shell32.SHGFI.TYPENAME);

                    string mycompName = info.szDisplayName;

                    try
                    {
                        _oParentFolder = GetDesktopFolder();

                        if (_oParentFolder.EnumObjects(winHandle, SHCONTF.FOLDERS | SHCONTF.INCLUDEHIDDEN, out folderEnumPtr) == (int)Commun.HRESULT.S_OK)
                        {
                            folderEnum = (IEnumIDList)Marshal.GetTypedObjectForIUnknown(folderEnumPtr, typeof(IEnumIDList));
                            while (folderEnum.Next(1, out IntPtr pidlSubItem, out int celtFetched) == (int)Commun.HRESULT.S_OK && celtFetched == 1)
                            {
                                Guid guid = typeof(IShellFolder).GUID;
                                if (_oParentFolder.BindToObject(
                                            pidlSubItem,
                                            IntPtr.Zero,
                                            ref guid,
                                            out object shellFolderPtr) == (int)Commun.HRESULT.S_OK)
                                {
                                    IntPtr strr = Marshal.AllocCoTaskMem(ManagedShell.Common.Helpers.ShellHelper.MAX_PATH * 2 + 4);
                                    Marshal.WriteInt32(strr, 0, 0);
                                    StringBuilder buf = new(ManagedShell.Common.Helpers.ShellHelper.MAX_PATH);

                                    string txt = "";
                                    if (_oParentFolder.GetDisplayNameOf(
                                                    pidlSubItem,
                                                    SHGNO.INFOLDER,
                                                    strr) == (int)Commun.HRESULT.S_OK)
                                    {
                                        Shlwapi.StrRetToBuf(strr, pidlSubItem, buf, ManagedShell.Common.Helpers.ShellHelper.MAX_PATH);
                                        txt = buf.ToString();
                                    }

                                    Marshal.FreeCoTaskMem(strr);

                                    if (txt == mycompName)
                                    {
                                        _oParentFolder = (IShellFolder)shellFolderPtr;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                    finally
                    {
                        if (folderEnum != null)
                        {
                            Marshal.ReleaseComObject(folderEnum);
                            Marshal.Release(folderEnumPtr);
                        }
                    }
                }
            }
            return _oParentFolder;
        }
        #endregion

        #region GetPIDLs()
        /// <summary>
        /// Get the PIDLs
        /// </summary>
        /// <param name="arrFI">Array of FileInfo</param>
        /// <returns>Array of PIDLs</returns>
        protected IntPtr[] GetPIDLs(FileInfo[] arrFI)
        {
            if (arrFI == null || arrFI.Length == 0)
                return null;

            IShellFolder oParentFolder = GetParentFolder(arrFI[0].DirectoryName, false);
            if (oParentFolder == null)
                return null;

            IntPtr[] arrPIDLs = new IntPtr[arrFI.Length];
            int n = 0;
            foreach (FileInfo fi in arrFI)
            {
                // Get the file relative to folder
                uint pchEaten = 0;
                SFGAO pdwAttributes = 0;
                int nResult = oParentFolder.ParseDisplayName(IntPtr.Zero, IntPtr.Zero, fi.Name, ref pchEaten, out IntPtr pPIDL, ref pdwAttributes);
                if (nResult != (int)Commun.HRESULT.S_OK)
                {
                    FreePIDLs(arrPIDLs);
                    return null;
                }
                arrPIDLs[n] = pPIDL;
                n++;
            }

            return arrPIDLs;
        }

        /// <summary>
        /// Get the PIDLs
        /// </summary>
        /// <param name="arrFI">Array of DirectoryInfo</param>
        /// <returns>Array of PIDLs</returns>
        protected IntPtr[] GetPIDLs(DirectoryInfo[] arrFI, bool background = false)
        {
            if (arrFI == null || arrFI.Length == 0)
                return null;

            if (background)
            {
                IntPtr pItemIDL = Shell32.ILCreateFromPath(arrFI[0].FullName);
                GetDesktopFolder().BindToObject(pItemIDL, IntPtr.Zero, typeof(IShellFolder).GUID, out object opsf);
                IShellFolder psf = (IShellFolder)opsf;
                psf.CreateViewObject(IntPtr.Zero, typeof(IShellView).GUID, out object opShellView);
                IShellView pShellView = (IShellView)opShellView;
                pShellView.GetItemObject((uint)SVGIO.SVGIO_BACKGROUND, typeof(IContextMenu).GUID, out object opContextMenu);
                _oContextMenu = (IContextMenu)opContextMenu;
            }

            IShellFolder oParentFolder;
            if ((arrFI[0].FullName.Length > 3) && (arrFI[0].Name != "My Computer") && (arrFI[0].Name != "Desktop") && (arrFI[0].Exists))
            {
                oParentFolder = GetParentFolder(arrFI[0].Parent.FullName, false);
                if (oParentFolder == null)
                    return null;
            }
            else
                oParentFolder = GetParentFolder(null, (arrFI[0].Name == "My Computer" || arrFI[0].Name == "Desktop"));

            IntPtr[] arrPIDLs = new IntPtr[arrFI.Length];
            int n = 0;
            foreach (DirectoryInfo fi in arrFI)
            {
                // Get the file relative to folder
                uint pchEaten = 0;
                SFGAO pdwAttributes = 0;
                int nResult = oParentFolder.ParseDisplayName(IntPtr.Zero, IntPtr.Zero, fi.Name, ref pchEaten, out IntPtr pPIDL, ref pdwAttributes);
                if (nResult != (int)Commun.HRESULT.S_OK)
                {
                    if ((fi.Name == "Desktop") && (!fi.Exists))
                    {
                        Shell32.SHGetSpecialFolderLocation(IntPtr.Zero, Shell32.CSIDL.DESKTOP, ref pPIDL);
                    }
                    else if ((fi.Name == "My Computer") && (!fi.Exists))
                    {
                        Shell32.SHGetSpecialFolderLocation(IntPtr.Zero, Shell32.CSIDL.DRIVES, ref pPIDL);
                    }
                }
                if (pPIDL != IntPtr.Zero)
                {
                    arrPIDLs[n] = pPIDL;
                    n++;
                }
            }

            return arrPIDLs;
        }
        #endregion

        #region FreePIDLs()
        /// <summary>
        /// Free the PIDLs
        /// </summary>
        /// <param name="arrPIDLs">Array of PIDLs (IntPtr)</param>
        protected void FreePIDLs(IntPtr[] arrPIDLs)
        {
            if (null != arrPIDLs)
            {
                for (int n = 0; n < arrPIDLs.Length; n++)
                {
                    if (arrPIDLs[n] != IntPtr.Zero)
                    {
                        Marshal.FreeCoTaskMem(arrPIDLs[n]);
                        arrPIDLs[n] = IntPtr.Zero;
                    }
                }
            }
        }
        #endregion

        #region ShowContextMenu()

        /// <summary>
        /// Shows the context menu
        /// </summary>
        /// <param name="files">FileInfos (should all be in same directory)</param>
        /// <param name="pointScreen">Where to show the menu</param>
        public void ShowContextMenu(FileInfo[] files, Point pointScreen, ContextMenuStrip cms)
        {
            // Release all resources first.
            ReleaseAll();
            _arrPIDLs = GetPIDLs(files);
            ShowContextMenu(pointScreen, cms);
        }

        /// <summary>
        /// Shows the context menu
        /// </summary>
        /// <param name="dirs">DirectoryInfos (should all be in same directory)</param>
        /// <param name="pointScreen">Where to show the menu</param>
        public void ShowContextMenu(DirectoryInfo[] dirs, Point pointScreen, ContextMenuStrip cms)
        {
            // Release all resources first.
            ReleaseAll();
            _arrPIDLs = GetPIDLs(dirs);
            ShowContextMenu(pointScreen, cms);
        }

        public void ShowContextMenu(DirectoryInfo dir, Point pointScreen, ContextMenuStrip cms)
        {
            ReleaseAll();
            _arrPIDLs = GetPIDLs(new DirectoryInfo[] { dir }, true);
            ShowContextMenu(pointScreen, cms, true);
        }

        /// <summary>
        /// Shows the context menu
        /// </summary>
        /// <param name="arrFI">FileInfos (should all be in same directory)</param>
        /// <param name="pointScreen">Where to show the menu</param>
        public void ShowContextMenu(Point pointScreen, ContextMenuStrip cms, bool background = false)
        {
            try
            {
                _positionSouris = pointScreen;

                if (_arrPIDLs == null)
                {
                    ReleaseAll();
                    return;
                }

                if (!background &&
                    !GetContextMenuInterfaces(_arrPIDLs, out iContextMenuPtr))
                {
                    ReleaseAll();
                    return;
                }

                pMenu = User32.CreatePopupMenu();

                _oContextMenu.QueryContextMenu(
                    pMenu,
                    0,
                    CMD_FIRST,
                    CMD_LAST,
                    (CMF.NORMAL | CMF.CANRENAME | CMF.EXTENDEDVERBS));

                if (iContextMenuPtr != IntPtr.Zero)
                {
                    Guid guid = typeof(IContextMenu2).GUID;
                    Marshal.QueryInterface(iContextMenuPtr, ref guid, out iContextMenuPtr2);
                    guid = typeof(IContextMenu3).GUID;
                    Marshal.QueryInterface(iContextMenuPtr, ref guid, out iContextMenuPtr3);
                    _oContextMenu2 = (IContextMenu2)Marshal.GetTypedObjectForIUnknown(iContextMenuPtr2, typeof(IContextMenu2));
                    _oContextMenu3 = (IContextMenu3)Marshal.GetTypedObjectForIUnknown(iContextMenuPtr3, typeof(IContextMenu3));
                }
                else
                {
                    _oContextMenu2 = (IContextMenu2)_oContextMenu;
                    _oContextMenu3 = (IContextMenu3)_oContextMenu;
                }

                if (cms == null)
                {
                    uint nSelected = 0;

                    nSelected = User32.TrackPopupMenuEx(
                        pMenu,
                        User32.TPM.RETURNCMD,
                        pointScreen.X,
                        pointScreen.Y,
                        Handle,
                        IntPtr.Zero);

                    User32.DestroyMenu(pMenu);
                    pMenu = IntPtr.Zero;

                    if (nSelected != 0)
                    {
                        InvokeCommand(_oContextMenu, nSelected, _strParentFolder, pointScreen);
                    }
                }
                else
                {
                    cms.Items.Clear();
                    PopUpMenuExtensions.EtendreSousMenuPopUp(pMenu, _oContextMenu2);
                    PopUpMenuExtensions.CopierVersCms(cms, null, pMenu, MenuAAjouter_Click, background);
                    cms.Show(pointScreen);
                    return;
                }
            }
            catch (Exception)
            {
                if (System.Diagnostics.Debugger.IsAttached)
                    System.Diagnostics.Debugger.Break();
            }
            finally
            {
                if (cms == null)
                    CleanUp();
            }
        }

        private void CleanUp()
        {
            if (pMenu != IntPtr.Zero)
            {
                User32.DestroyMenu(pMenu);
            }

            if (iContextMenuPtr != IntPtr.Zero)
                Marshal.Release(iContextMenuPtr);

            if (iContextMenuPtr2 != IntPtr.Zero)
                Marshal.Release(iContextMenuPtr2);

            if (iContextMenuPtr3 != IntPtr.Zero)
                Marshal.Release(iContextMenuPtr3);

            ReleaseAll();
        }

        #endregion

        private void MenuAAjouter_Click(object sender, EventArgs e)
        {
            uint ID = uint.Parse(((ToolStripMenuItem)sender).Tag.ToString());
            InvokeCommand(_oContextMenu, ID, _strParentFolder, _positionSouris);
        }
    }

    #region ShellHelper

    internal static class ShellHelper
    {
        #region Low/High Word

        /// <summary>
        /// Retrieves the High Word of a WParam of a WindowMessage
        /// </summary>
        /// <param name="ptr">The pointer to the WParam</param>
        /// <returns>The unsigned integer for the High Word</returns>
        public static long HiWord(IntPtr ptr)
        {
            if (((long)ptr & 0x80000000) == 0x80000000)
                return ((long)ptr >> 16);
            else
                return ((long)ptr >> 16) & 0xffff;
        }

        /// <summary>
        /// Retrieves the Low Word of a WParam of a WindowMessage
        /// </summary>
        /// <param name="ptr">The pointer to the WParam</param>
        /// <returns>The unsigned integer for the Low Word</returns>
        public static long LoWord(IntPtr ptr)
        {
            return (long)ptr & 0xffff;
        }

        #endregion
    }

    #endregion
}
