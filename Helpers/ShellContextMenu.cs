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

            if (nResult == S_OK)
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
                m.Msg == (int)WM.MENUSELECT &&
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
                (m.Msg == (int)WM.INITMENUPOPUP ||
                 m.Msg == (int)WM.MEASUREITEM ||
                 m.Msg == (int)WM.DRAWITEM))
            {
                if (_oContextMenu2.HandleMenuMsg(
                    (uint)m.Msg, m.WParam, m.LParam) == S_OK)
                    return;
            }

            #endregion

            #region IContextMenu3

            if (_oContextMenu3 != null &&
                m.Msg == (int)WM.MENUCHAR)
            {
                if (_oContextMenu3.HandleMenuMsg2(
                    (uint)m.Msg, m.WParam, m.LParam, IntPtr.Zero) == S_OK)
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
            if (null != _oContextMenu)
            {
                Marshal.ReleaseComObject(_oContextMenu);
                _oContextMenu = null;
            }
            if (null != _oContextMenu2)
            {
                Marshal.ReleaseComObject(_oContextMenu2);
                _oContextMenu2 = null;
            }
            if (null != _oContextMenu3)
            {
                Marshal.ReleaseComObject(_oContextMenu3);
                _oContextMenu3 = null;
            }
            if (null != _oDesktopFolder)
            {
                Marshal.ReleaseComObject(_oDesktopFolder);
                _oDesktopFolder = null;
            }
            if (null != _oParentFolder)
            {
                Marshal.ReleaseComObject(_oParentFolder);
                _oParentFolder = null;
            }
            if (null != _arrPIDLs)
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
                if (nResult != S_OK)
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
                    if (nResult != S_OK)
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
                    if (nResult != S_OK)
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

                        if (_oParentFolder.EnumObjects(winHandle, SHCONTF.FOLDERS | SHCONTF.INCLUDEHIDDEN, out folderEnumPtr) == S_OK)
                        {
                            folderEnum = (IEnumIDList)Marshal.GetTypedObjectForIUnknown(folderEnumPtr, typeof(IEnumIDList));
                            while (folderEnum.Next(1, out IntPtr pidlSubItem, out int celtFetched) == S_OK && celtFetched == 1)
                            {
                                if (_oParentFolder.BindToObject(
                                            pidlSubItem,
                                            IntPtr.Zero,
                                            ref IID_IShellFolder,
                                            out IntPtr shellFolderPtr) == S_OK)
                                {
                                    IntPtr strr = Marshal.AllocCoTaskMem(MAX_PATH * 2 + 4);
                                    Marshal.WriteInt32(strr, 0, 0);
                                    StringBuilder buf = new StringBuilder(MAX_PATH);

                                    string txt = "";
                                    if (_oParentFolder.GetDisplayNameOf(
                                                    pidlSubItem,
                                                    SHGNO.INFOLDER,
                                                    strr) == S_OK)
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
                if (nResult != S_OK)
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

                if (cms != null)
                {
                    cms.Items.Clear();
                    CopierVersCms(cms, null, pMenu);
                }

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

        private string GetMenuItemText(uint IdMenu, IntPtr pointeurMenu)
        {
            string retour = "";
            try
            {
                MENUITEMINFO sortie = new MENUITEMINFO
                {
                    cbSize = (uint)Marshal.SizeOf(typeof(MENUITEMINFO)),
                    dwTypeData = new string('\0', 256),
                    fMask = MIIM.STRING,
                    fType = MFT.STRING | MFT.DISABLED | MFT.GRAYED
                };
                sortie.cch = sortie.dwTypeData.Length - 1;
                if (User32.GetMenuItemInfo(pointeurMenu, IdMenu, false, ref sortie))
                {
                    retour = sortie.dwTypeData;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur " + ex.Message);
            }
            return retour;
        }

        private Image GetMenuItemIcone(uint IdMenu, IntPtr pointeurMenu)
        {
            Bitmap retour = null;
            try
            {
                MENUITEMINFO sortie = new MENUITEMINFO
                {
                    cbSize = (uint)Marshal.SizeOf(typeof(MENUITEMINFO)),
                    fMask = MIIM.BITMAP,
                };
                if (User32.GetMenuItemInfo(pointeurMenu, IdMenu, false, ref sortie))
                {
                    retour = Image.FromHbitmap(sortie.hbmpItem);
                    retour.MakeTransparent();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur " + ex.Message);
            }
            return retour;
        }

        private Image GetMenuItemIcone(int position, IntPtr pointeurMenu)
        {
            Bitmap retour = null;
            try
            {
                MENUITEMINFO sortie = new MENUITEMINFO
                {
                    cbSize = (uint)Marshal.SizeOf(typeof(MENUITEMINFO)),
                    fMask = MIIM.BITMAP,
                };
                if (User32.GetMenuItemInfo(pointeurMenu, (uint)position, true, ref sortie))
                {
                    retour = Image.FromHbitmap(sortie.hbmpItem);
                    retour.MakeTransparent();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur " + ex.Message);
            }
            return retour;
        }

        private string GetMenuItemText(int position, IntPtr pointeurMenu)
        {
            string retour = "";
            try
            {
                MENUITEMINFO sortie = new MENUITEMINFO
                {
                    cbSize = (uint)Marshal.SizeOf(typeof(MENUITEMINFO)),
                    dwTypeData = new string('\0', 256),
                    fMask = MIIM.STRING,
                    fType = MFT.STRING | MFT.DISABLED | MFT.GRAYED
                };
                sortie.cch = sortie.dwTypeData.Length - 1;
                if (User32.GetMenuItemInfo(pointeurMenu, (uint)position, true, ref sortie))
                {
                    retour = sortie.dwTypeData;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur " + ex.Message);
            }
            return retour;
        }

        private uint GetMenuItemID(int position, IntPtr pointeurMenu)
        {
            uint retour = 0;
            try
            {
                MENUITEMINFO sortie = new MENUITEMINFO
                {
                    cbSize = (uint)Marshal.SizeOf(typeof(MENUITEMINFO)),
                    dwTypeData = new string('\0', 256),
                    fMask = MIIM.ID,
                    fType = MFT.STRING | MFT.DISABLED | MFT.GRAYED
                };
                sortie.cch = sortie.dwTypeData.Length - 1;
                if (User32.GetMenuItemInfo(pointeurMenu, (uint)position, true, ref sortie))
                {
                    retour = sortie.wID;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur " + ex.Message);
            }
            return retour;
        }

        private uint GetMenuItemID(uint IdMenu, IntPtr pointeurMenu)
        {
            uint retour = 0;
            try
            {
                MENUITEMINFO sortie = new MENUITEMINFO
                {
                    cbSize = (uint)Marshal.SizeOf(typeof(MENUITEMINFO)),
                    dwTypeData = new string('\0', 256),
                    fMask = MIIM.ID,
                    fType = MFT.STRING | MFT.DISABLED | MFT.GRAYED
                };
                sortie.cch = sortie.dwTypeData.Length - 1;
                if (User32.GetMenuItemInfo(pointeurMenu, IdMenu, false, ref sortie))
                {
                    retour = sortie.wID;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur " + ex.Message);
            }
            return retour;
        }

        private void CopierVersCms(ContextMenuStrip cms, ToolStripMenuItem sousMenu, IntPtr pointeurMenu)
        {
            if (pointeurMenu != IntPtr.Zero)
            {
                int nbMenu = User32.GetMenuItemCount(pointeurMenu);
                if (nbMenu > 0)
                {
                    int IdMenu;
                    string libelle;
                    ToolStripItem menuAAjouter;
                    for (int i = 0; i < nbMenu; i++)
                    {
                        IdMenu = User32.GetMenuItemID(pointeurMenu, i);
                        if (IdMenu > 0)
                        {
                            libelle = GetMenuItemText((uint)IdMenu, pointeurMenu).Trim();
                            if (string.IsNullOrWhiteSpace(libelle))
                            {
                                if (cms.Items[cms.Items.Count - 1].GetType() == typeof(ToolStripSeparator))
                                    continue;
                                menuAAjouter = new ToolStripSeparator();
                            }
                            else
                            {
                                menuAAjouter = new ToolStripMenuItem()
                                {
                                    Text = libelle,
                                    Image = GetMenuItemIcone((uint)IdMenu, pointeurMenu),
                                    Tag = GetMenuItemID((uint)IdMenu, pointeurMenu),
                                };
                                menuAAjouter.Click += MenuAAjouter_Click;
                            }
                            if (sousMenu == null)
                            {
                                cms.Items.Add(menuAAjouter);
                            }
                            else
                            {
                                if ((menuAAjouter.GetType() == typeof(ToolStripSeparator)) && (nbMenu == 1))
                                    cms.Items.Add(menuAAjouter);
                                else
                                    sousMenu.DropDownItems.Add(menuAAjouter);
                            }
                        }
                        else if (IdMenu < 0)
                        {
                            IntPtr IdSousMenu = User32.GetSubMenu(pointeurMenu, i);
                            libelle = GetMenuItemText(i, pointeurMenu);
                            if (string.IsNullOrWhiteSpace(libelle))
                            {
                                if (cms.Items[cms.Items.Count - 1].GetType() != typeof(ToolStripSeparator))
                                    cms.Items.Add(new ToolStripSeparator());
                            }
                            else
                            {
                                cms.Items.Add(new ToolStripMenuItem()
                                {
                                    Text = libelle,
                                    Image = GetMenuItemIcone(i, pointeurMenu),
                                    Tag = GetMenuItemID(i, pointeurMenu),
                                });
                                ((ToolStripMenuItem)(cms.Items[cms.Items.Count - 1])).Click += MenuAAjouter_Click;
                                CopierVersCms(cms, (ToolStripMenuItem)cms.Items[cms.Items.Count - 1], IdSousMenu);
                            }
                        }
                    }
                }
            }
        }

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

        private const int S_OK = 0;
        //private const int S_FALSE = 1;

        private static readonly int cbInvokeCommand = Marshal.SizeOf(typeof(CMINVOKECOMMANDINFOEX));

        #endregion

        #region Shell GUIDs

        private static Guid IID_IShellFolder = new Guid("{000214E6-0000-0000-C000-000000000046}");
        private static Guid IID_IContextMenu = new Guid("{000214e4-0000-0000-c000-000000000046}");
        private static Guid IID_IContextMenu2 = new Guid("{000214f4-0000-0000-c000-000000000046}");
        private static Guid IID_IContextMenu3 = new Guid("{bcfce0a0-ec17-11d0-8d10-00a0c90f2719}");

        #endregion

        #region Enums

        // Defines the values used with the IShellFolder::GetDisplayNameOf and IShellFolder::SetNameOf 
        // methods to specify the type of file or folder names used by those methods
        // The attributes that the caller is requesting, when calling IShellFolder::GetAttributesOf
        // Determines the type of items included in an enumeration. 
        // These values are used with the IShellFolder::EnumObjects method
        // Specifies how TrackPopupMenuEx positions the shortcut menu horizontally

        // Window message flags
        [Flags()]
        private enum WM : uint
        {
            ACTIVATE = 0x6,
            ACTIVATEAPP = 0x1C,
            AFXFIRST = 0x360,
            AFXLAST = 0x37F,
            APP = 0x8000,
            ASKCBFORMATNAME = 0x30C,
            CANCELJOURNAL = 0x4B,
            CANCELMODE = 0x1F,
            CAPTURECHANGED = 0x215,
            CHANGECBCHAIN = 0x30D,
            CHAR = 0x102,
            CHARTOITEM = 0x2F,
            CHILDACTIVATE = 0x22,
            CLEAR = 0x303,
            CLOSE = 0x10,
            COMMAND = 0x111,
            COMPACTING = 0x41,
            COMPAREITEM = 0x39,
            CONTEXTMENU = 0x7B,
            COPY = 0x301,
            COPYDATA = 0x4A,
            CREATE = 0x1,
            CTLCOLORBTN = 0x135,
            CTLCOLORDLG = 0x136,
            CTLCOLOREDIT = 0x133,
            CTLCOLORLISTBOX = 0x134,
            CTLCOLORMSGBOX = 0x132,
            CTLCOLORSCROLLBAR = 0x137,
            CTLCOLORSTATIC = 0x138,
            CUT = 0x300,
            DEADCHAR = 0x103,
            DELETEITEM = 0x2D,
            DESTROY = 0x2,
            DESTROYCLIPBOARD = 0x307,
            DEVICECHANGE = 0x219,
            DEVMODECHANGE = 0x1B,
            DISPLAYCHANGE = 0x7E,
            DRAWCLIPBOARD = 0x308,
            DRAWITEM = 0x2B,
            DROPFILES = 0x233,
            ENABLE = 0xA,
            ENDSESSION = 0x16,
            ENTERIDLE = 0x121,
            ENTERMENULOOP = 0x211,
            ENTERSIZEMOVE = 0x231,
            ERASEBKGND = 0x14,
            EXITMENULOOP = 0x212,
            EXITSIZEMOVE = 0x232,
            FONTCHANGE = 0x1D,
            GETDLGCODE = 0x87,
            GETFONT = 0x31,
            GETHOTKEY = 0x33,
            GETICON = 0x7F,
            GETMINMAXINFO = 0x24,
            GETOBJECT = 0x3D,
            GETSYSMENU = 0x313,
            GETTEXT = 0xD,
            GETTEXTLENGTH = 0xE,
            HANDHELDFIRST = 0x358,
            HANDHELDLAST = 0x35F,
            HELP = 0x53,
            HOTKEY = 0x312,
            HSCROLL = 0x114,
            HSCROLLCLIPBOARD = 0x30E,
            ICONERASEBKGND = 0x27,
            IME_CHAR = 0x286,
            IME_COMPOSITION = 0x10F,
            IME_COMPOSITIONFULL = 0x284,
            IME_CONTROL = 0x283,
            IME_ENDCOMPOSITION = 0x10E,
            IME_KEYDOWN = 0x290,
            IME_KEYLAST = 0x10F,
            IME_KEYUP = 0x291,
            IME_NOTIFY = 0x282,
            IME_REQUEST = 0x288,
            IME_SELECT = 0x285,
            IME_SETCONTEXT = 0x281,
            IME_STARTCOMPOSITION = 0x10D,
            INITDIALOG = 0x110,
            INITMENU = 0x116,
            INITMENUPOPUP = 0x117,
            INPUTLANGCHANGE = 0x51,
            INPUTLANGCHANGEREQUEST = 0x50,
            KEYDOWN = 0x100,
            KEYFIRST = 0x100,
            KEYLAST = 0x108,
            KEYUP = 0x101,
            KILLFOCUS = 0x8,
            LBUTTONDBLCLK = 0x203,
            LBUTTONDOWN = 0x201,
            LBUTTONUP = 0x202,
            LVM_GETEDITCONTROL = 0x1018,
            LVM_SETIMAGELIST = 0x1003,
            MBUTTONDBLCLK = 0x209,
            MBUTTONDOWN = 0x207,
            MBUTTONUP = 0x208,
            MDIACTIVATE = 0x222,
            MDICASCADE = 0x227,
            MDICREATE = 0x220,
            MDIDESTROY = 0x221,
            MDIGETACTIVE = 0x229,
            MDIICONARRANGE = 0x228,
            MDIMAXIMIZE = 0x225,
            MDINEXT = 0x224,
            MDIREFRESHMENU = 0x234,
            MDIRESTORE = 0x223,
            MDISETMENU = 0x230,
            MDITILE = 0x226,
            MEASUREITEM = 0x2C,
            MENUCHAR = 0x120,
            MENUCOMMAND = 0x126,
            MENUDRAG = 0x123,
            MENUGETOBJECT = 0x124,
            MENURBUTTONUP = 0x122,
            MENUSELECT = 0x11F,
            MOUSEACTIVATE = 0x21,
            MOUSEFIRST = 0x200,
            MOUSEHOVER = 0x2A1,
            MOUSELAST = 0x20A,
            MOUSELEAVE = 0x2A3,
            MOUSEMOVE = 0x200,
            MOUSEWHEEL = 0x20A,
            MOVE = 0x3,
            MOVING = 0x216,
            NCACTIVATE = 0x86,
            NCCALCSIZE = 0x83,
            NCCREATE = 0x81,
            NCDESTROY = 0x82,
            NCHITTEST = 0x84,
            NCLBUTTONDBLCLK = 0xA3,
            NCLBUTTONDOWN = 0xA1,
            NCLBUTTONUP = 0xA2,
            NCMBUTTONDBLCLK = 0xA9,
            NCMBUTTONDOWN = 0xA7,
            NCMBUTTONUP = 0xA8,
            NCMOUSEHOVER = 0x2A0,
            NCMOUSELEAVE = 0x2A2,
            NCMOUSEMOVE = 0xA0,
            NCPAINT = 0x85,
            NCRBUTTONDBLCLK = 0xA6,
            NCRBUTTONDOWN = 0xA4,
            NCRBUTTONUP = 0xA5,
            NEXTDLGCTL = 0x28,
            NEXTMENU = 0x213,
            NOTIFY = 0x4E,
            NOTIFYFORMAT = 0x55,
            NULL = 0x0,
            PAINT = 0xF,
            PAINTCLIPBOARD = 0x309,
            PAINTICON = 0x26,
            PALETTECHANGED = 0x311,
            PALETTEISCHANGING = 0x310,
            PARENTNOTIFY = 0x210,
            PASTE = 0x302,
            PENWINFIRST = 0x380,
            PENWINLAST = 0x38F,
            POWER = 0x48,
            PRINT = 0x317,
            PRINTCLIENT = 0x318,
            QUERYDRAGICON = 0x37,
            QUERYENDSESSION = 0x11,
            QUERYNEWPALETTE = 0x30F,
            QUERYOPEN = 0x13,
            QUEUESYNC = 0x23,
            QUIT = 0x12,
            RBUTTONDBLCLK = 0x206,
            RBUTTONDOWN = 0x204,
            RBUTTONUP = 0x205,
            RENDERALLFORMATS = 0x306,
            RENDERFORMAT = 0x305,
            SETCURSOR = 0x20,
            SETFOCUS = 0x7,
            SETFONT = 0x30,
            SETHOTKEY = 0x32,
            SETICON = 0x80,
            SETMARGINS = 0xD3,
            SETREDRAW = 0xB,
            SETTEXT = 0xC,
            SETTINGCHANGE = 0x1A,
            SHOWWINDOW = 0x18,
            SIZE = 0x5,
            SIZECLIPBOARD = 0x30B,
            SIZING = 0x214,
            SPOOLERSTATUS = 0x2A,
            STYLECHANGED = 0x7D,
            STYLECHANGING = 0x7C,
            SYNCPAINT = 0x88,
            SYSCHAR = 0x106,
            SYSCOLORCHANGE = 0x15,
            SYSCOMMAND = 0x112,
            SYSDEADCHAR = 0x107,
            SYSKEYDOWN = 0x104,
            SYSKEYUP = 0x105,
            TCARD = 0x52,
            TIMECHANGE = 0x1E,
            TIMER = 0x113,
            TVM_GETEDITCONTROL = 0x110F,
            TVM_SETIMAGELIST = 0x1109,
            UNDO = 0x304,
            UNINITMENUPOPUP = 0x125,
            USER = 0x400,
            USERCHANGED = 0x54,
            VKEYTOITEM = 0x2E,
            VSCROLL = 0x115,
            VSCROLLCLIPBOARD = 0x30A,
            WINDOWPOSCHANGED = 0x47,
            WINDOWPOSCHANGING = 0x46,
            WININICHANGE = 0x1A,
            SH_NOTIFY = 0x0401
        }

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
