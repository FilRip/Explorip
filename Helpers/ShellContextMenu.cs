using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

using Explorip.WinAPI;
using Explorip.WinAPI.Modeles;
using Explorip.Exceptions;

namespace Explorip.Helpers
{
    // TODO : Ouvrir avec : https://social.msdn.microsoft.com/Forums/en-US/d513e241-8f7e-419c-b87f-0b653abb3d83/translate?forum=Offtopic

    public class ShellContextMenu : NativeWindow
    {
        #region Constructor
        /// <summary>Default constructor</summary>
        public ShellContextMenu()
        {
            CleanUp();
            this.CreateHandle(new CreateParams());
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
        private bool GetContextMenuInterfaces(IShellFolder oParentFolder, IntPtr[] arrPIDLs, out IntPtr ctxMenuPtr)
        {
            int nResult = oParentFolder.GetUIObjectOf(
                IntPtr.Zero,
                (uint)arrPIDLs.Length,
                arrPIDLs,
                ref IID_IContextMenu,
                IntPtr.Zero,
                out ctxMenuPtr);

            if (nResult == (int)Commun.HRESULT.S_OK)
            {
                _oContextMenu = (IContextMenu)Marshal.GetTypedObjectForIUnknown(ctxMenuPtr, typeof(IContextMenu));

                /*IntPtr pUnknownContextMenu2 = IntPtr.Zero;
                if (S_OK == Marshal.QueryInterface(pUnknownContextMenu, ref IID_IContextMenu2, out pUnknownContextMenu2))
                {
                    _oContextMenu2 = (IContextMenu2)Marshal.GetTypedObjectForIUnknown(pUnknownContextMenu2, typeof(IContextMenu2));
                }
                IntPtr pUnknownContextMenu3 = IntPtr.Zero;
                if (S_OK == Marshal.QueryInterface(pUnknownContextMenu, ref IID_IContextMenu3, out pUnknownContextMenu3))
                {
                    _oContextMenu3 = (IContextMenu3)Marshal.GetTypedObjectForIUnknown(pUnknownContextMenu3, typeof(IContextMenu3));
                }*/

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

        /// <summary>
        /// This method receives WindowMessages. It will make the "Open With" and "Send To" work 
        /// by calling HandleMenuMsg and HandleMenuMsg2. It will also call the OnContextMenuMouseHover 
        /// method of Browser when hovering over a ContextMenu item.
        /// </summary>
        /// <param name="m">the Message of the Browser's WndProc</param>
        /// <returns>true if the message has been handled, false otherwise</returns>
        protected override void WndProc(ref Message m)
        {
            #region IContextMenu

            if (_oContextMenu != null &&
                m.Msg == (int)Commun.WM.MENUSELECT &&
                ((int)ShellHelper.HiWord(m.WParam) & (int)MFT.SEPARATOR) == 0 &&
                ((int)ShellHelper.HiWord(m.WParam) & (int)MFT.POPUP) == 0)
            {
                //string info = string.Empty;

                /*if (ShellHelper.LoWord(m.WParam) == (int)CMD_CUSTOM.ExpandCollapse)
                    info = "Expands or collapses the current selected item";
                else
                {
                    info = "";/* ContextMenuHelper.GetCommandString(
                         _oContextMenu,
                         ShellHelper.LoWord(m.WParam) - CMD_FIRST,
                         false);*/
                //}

                //br.OnContextMenuMouseHover(new ContextMenuMouseHoverEventArgs(info.ToString()));
            }

            #endregion

            #region IContextMenu2

            if (_oContextMenu2 != null &&
                (m.Msg == (int)Commun.WM.INITMENUPOPUP ||
                 m.Msg == (int)Commun.WM.MEASUREITEM ||
                 m.Msg == (int)Commun.WM.DRAWITEM))
            {
                if (_oContextMenu2.HandleMenuMsg(
                    (uint)m.Msg, m.WParam, m.LParam) == (int)Commun.HRESULT.S_OK)
                    return;
            }

            #endregion

            #region IContextMenu3

            if (_oContextMenu3 != null &&
                m.Msg == (int)Commun.WM.MENUCHAR)
            {
                if (_oContextMenu3.HandleMenuMsg2(
                    (uint)m.Msg, m.WParam, m.LParam, IntPtr.Zero) == (int)Commun.HRESULT.S_OK)
                    return;
            }

            #endregion

            base.WndProc(ref m);
        }

        #endregion

        #region InvokeCommand
        private void InvokeCommand(IContextMenu oContextMenu, uint nCmd, string strFolder, Point pointInvoke)
        {
            CMINVOKECOMMANDINFOEX invoke = new CMINVOKECOMMANDINFOEX()
            {
                cbSize = cbInvokeCommand,
                lpVerb = (IntPtr)(nCmd - CMD_FIRST),
                lpDirectory = strFolder,
                lpVerbW = (IntPtr)(nCmd - CMD_FIRST),
                lpDirectoryW = strFolder,
                fMask = CMIC.UNICODE | CMIC.PTINVOKE |
                ((Control.ModifierKeys & Keys.Control) != 0 ? CMIC.CONTROL_DOWN : 0) |
                ((Control.ModifierKeys & Keys.Shift) != 0 ? CMIC.SHIFT_DOWN : 0),
                ptInvoke = new POINT(pointInvoke.X, pointInvoke.Y),
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
            if (null == _oDesktopFolder)
            {
                // Get desktop IShellFolder
                int nResult = Shell32.SHGetDesktopFolder(out IntPtr pUnkownDesktopFolder);
                if (nResult != (int)Commun.HRESULT.S_OK)
                {
                    throw new ShellContextMenuException("Failed to get the desktop shell folder");
                }
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
                if (null == _oParentFolder)
                {
                    IShellFolder oDesktopFolder = GetDesktopFolder();
                    if (null == oDesktopFolder)
                    {
                        return null;
                    }

                    uint pchEaten = 0;
                    SFGAO pdwAttributes = 0;

                    // Get the PIDL for the folder file is in
                    int nResult = oDesktopFolder.ParseDisplayName(IntPtr.Zero, IntPtr.Zero, folderName, ref pchEaten, out IntPtr pPIDL, ref pdwAttributes);
                    if (nResult != (int)Commun.HRESULT.S_OK)
                    {
                        return null;
                    }

                    IntPtr pStrRet = Marshal.AllocCoTaskMem(MAX_PATH * 2 + 4);
                    Marshal.WriteInt32(pStrRet, 0, 0);
                    _ = _oDesktopFolder.GetDisplayNameOf(pPIDL, SHGNO.FORPARSING, pStrRet);
                    StringBuilder strFolder = new StringBuilder(MAX_PATH);
                    Shlwapi.StrRetToBuf(pStrRet, pPIDL, strFolder, MAX_PATH);
                    Marshal.FreeCoTaskMem(pStrRet);
                    _strParentFolder = strFolder.ToString();

                    // Get the IShellFolder for folder
                    nResult = oDesktopFolder.BindToObject(pPIDL, IntPtr.Zero, ref IID_IShellFolder, out IntPtr pUnknownParentFolder);
                    // Free the PIDL first
                    Marshal.FreeCoTaskMem(pPIDL);
                    if (nResult != (int)Commun.HRESULT.S_OK)
                    {
                        return null;
                    }
                    _oParentFolder = (IShellFolder)Marshal.GetTypedObjectForIUnknown(pUnknownParentFolder, typeof(IShellFolder));
                }
            }
            else
            {
                if (desktopParent)
                {
                    _oParentFolder = GetDesktopFolder();
                }
                else
                {
                    IntPtr folderEnumPtr = IntPtr.Zero;
                    IEnumIDList folderEnum = null;
                    IntPtr winHandle = IntPtr.Zero;

                    IntPtr tempPidl;
                    SHFILEINFO info;

                    info = new SHFILEINFO();
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
                                if (_oParentFolder.BindToObject(
                                            pidlSubItem,
                                            IntPtr.Zero,
                                            ref IID_IShellFolder,
                                            out IntPtr shellFolderPtr) == (int)Commun.HRESULT.S_OK)
                                {
                                    IntPtr strr = Marshal.AllocCoTaskMem(MAX_PATH * 2 + 4);
                                    Marshal.WriteInt32(strr, 0, 0);
                                    StringBuilder buf = new StringBuilder(MAX_PATH);

                                    string txt = "";
                                    if (_oParentFolder.GetDisplayNameOf(
                                                    pidlSubItem,
                                                    SHGNO.INFOLDER,
                                                    strr) == (int)Commun.HRESULT.S_OK)
                                    {
                                        Shlwapi.StrRetToBuf(strr, pidlSubItem, buf, MAX_PATH);
                                        txt = buf.ToString();
                                    }

                                    Marshal.FreeCoTaskMem(strr);

                                    if (txt == mycompName)
                                    {
                                        _oParentFolder = (IShellFolder)Marshal.GetTypedObjectForIUnknown(shellFolderPtr, typeof(IShellFolder));
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
            if (null == arrFI || 0 == arrFI.Length)
            {
                return null;
            }

            IShellFolder oParentFolder = GetParentFolder(arrFI[0].DirectoryName, false);
            if (null == oParentFolder)
            {
                return null;
            }

            IntPtr[] arrPIDLs = new IntPtr[arrFI.Length];
            int n = 0;
            foreach (FileInfo fi in arrFI)
            {
                // Get the file relative to folder
                uint pchEaten = 0;
                SFGAO pdwAttributes = 0;
                IntPtr pPIDL = IntPtr.Zero;
                int nResult = oParentFolder.ParseDisplayName(IntPtr.Zero, IntPtr.Zero, fi.Name, ref pchEaten, out pPIDL, ref pdwAttributes);
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
        protected IntPtr[] GetPIDLs(DirectoryInfo[] arrFI)
        {
            if (null == arrFI || 0 == arrFI.Length)
            {
                return null;
            }

            IShellFolder oParentFolder;
            if ((arrFI[0].FullName.Length > 3) && (arrFI[0].Name != "My Computer"))
            {
                oParentFolder = GetParentFolder(arrFI[0].Parent.FullName, false);
                if (null == oParentFolder)
                {
                    return null;
                }
            }
            else
            {
                oParentFolder = GetParentFolder(null, (arrFI[0].Name == "My Computer"));
            }

            IntPtr[] arrPIDLs = new IntPtr[arrFI.Length];
            int n = 0;
            foreach (DirectoryInfo fi in arrFI)
            {
                // Get the file relative to folder
                uint pchEaten = 0;
                SFGAO pdwAttributes = 0;
                IntPtr pPIDL = IntPtr.Zero;
                int nResult = oParentFolder.ParseDisplayName(IntPtr.Zero, IntPtr.Zero, fi.Name, ref pchEaten, out pPIDL, ref pdwAttributes);
                if (nResult != (int)Commun.HRESULT.S_OK)
                {
                    Shell32.SHGetSpecialFolderLocation(IntPtr.Zero, Shell32.CSIDL.DRIVES, ref pPIDL);
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
            this.ShowContextMenu(pointScreen, cms);
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
            this.ShowContextMenu(pointScreen, cms);
        }

        private IntPtr pMenu = IntPtr.Zero,
            iContextMenuPtr = IntPtr.Zero,
            iContextMenuPtr2 = IntPtr.Zero,
            iContextMenuPtr3 = IntPtr.Zero;

        private Point _positionSouris;

        /// <summary>
        /// Shows the context menu
        /// </summary>
        /// <param name="arrFI">FileInfos (should all be in same directory)</param>
        /// <param name="pointScreen">Where to show the menu</param>
        public void ShowContextMenu(Point pointScreen, ContextMenuStrip cms)
        {
            try
            {
                _positionSouris = pointScreen;

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

                pMenu = User32.CreatePopupMenu();

                int nResult = _oContextMenu.QueryContextMenu(
                    pMenu,
                    0,
                    CMD_FIRST,
                    CMD_LAST,
                    CMF.EXPLORE |
                    CMF.NORMAL | CMF.CANRENAME | 
                    ((Control.ModifierKeys & Keys.Shift) != 0 ? CMF.EXTENDEDVERBS : 0));

                Marshal.QueryInterface(iContextMenuPtr, ref IID_IContextMenu2, out iContextMenuPtr2);
                Marshal.QueryInterface(iContextMenuPtr, ref IID_IContextMenu3, out iContextMenuPtr3);

                _oContextMenu2 = (IContextMenu2)Marshal.GetTypedObjectForIUnknown(iContextMenuPtr2, typeof(IContextMenu2));
                _oContextMenu3 = (IContextMenu3)Marshal.GetTypedObjectForIUnknown(iContextMenuPtr3, typeof(IContextMenu3));

                if (cms == null)
                {
                    uint nSelected = 0;

                    nSelected = User32.TrackPopupMenuEx(
                        pMenu,
                        User32.TPM.RETURNCMD,
                        pointScreen.X,
                        pointScreen.Y,
                        this.Handle,
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
                    PopUpMenuExtensions.CopierVersCms(cms, null, pMenu, MenuAAjouter_Click);
                    cms.Show(pointScreen);
                    return;
                }
            }
            catch
            {
                throw;
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

        #region Local variabled
        private IContextMenu _oContextMenu;
        private IContextMenu2 _oContextMenu2;
        private IContextMenu3 _oContextMenu3;
        private IShellFolder _oDesktopFolder;
        private IShellFolder _oParentFolder;
        private IntPtr[] _arrPIDLs;
        private string _strParentFolder;
        #endregion

        #region Variables and Constants

        private const int MAX_PATH = 260;
        private const uint CMD_FIRST = 1;
        private const uint CMD_LAST = 30000;

        private static readonly int cbInvokeCommand = Marshal.SizeOf(typeof(CMINVOKECOMMANDINFOEX));

        #endregion

        #region Shell GUIDs

        private static Guid IID_IShellFolder = new Guid("{000214E6-0000-0000-C000-000000000046}");
        private static Guid IID_IContextMenu = new Guid("{000214e4-0000-0000-c000-000000000046}");
        private static Guid IID_IContextMenu2 = new Guid("{000214f4-0000-0000-c000-000000000046}");
        private static Guid IID_IContextMenu3 = new Guid("{bcfce0a0-ec17-11d0-8d10-00a0c90f2719}");

        #endregion

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
