using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input;

using Explorip.Helpers;

using ManagedShell.Common.Helpers;
using ManagedShell.ShellFolders.Enums;
using ManagedShell.ShellFolders.Interfaces;
using ManagedShell.ShellFolders.Structs;

using Securify.ShellLink;

using static ManagedShell.Interop.NativeMethods;

namespace ExploripComponents;

public class ShellContextMenu
{
    #region Fields

    private IntPtr _pMenu, _contextMenuPtr, _contextMenu2Ptr, _contextMenu3Ptr;
    private IContextMenu? _contextMenu;
    private IContextMenu2? _contextMenu2;
    private IContextMenu3? _contextMenu3;
    private IShellFolder? _parentFolder;
    private IntPtr[]? _listPIDL;
    private string? _strParentFolder;
    private string? _strCurrentFolder;
    private IShellView? _backgroundShellView;

    #endregion

    #region Constructor

    /// <summary>Default constructor</summary>
    public ShellContextMenu()
    {
        _pMenu = IntPtr.Zero;
        _contextMenuPtr = IntPtr.Zero;
        _contextMenu2Ptr = IntPtr.Zero;
        _contextMenu3Ptr = IntPtr.Zero;
    }

    #endregion

    #region Destructor

    /// <summary>Ensure all resources get released</summary>
    ~ShellContextMenu()
    {
        ReleaseAll();
        CleanUp();
    }

    /// <summary>
    /// Free the PIDLs
    /// </summary>
    protected void FreePIDLs()
    {
        if (_listPIDL?.Length > 0)
        {
            for (int n = 0; n < _listPIDL.Length; n++)
            {
                if (_listPIDL[n] != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(_listPIDL[n]);
                    _listPIDL[n] = IntPtr.Zero;
                }
            }
        }
    }

    private void CleanUp()
    {
        if (_pMenu != IntPtr.Zero)
        {
            DestroyMenu(_pMenu);
            _pMenu = IntPtr.Zero;
        }

        if (_contextMenuPtr != IntPtr.Zero)
        {
            Marshal.Release(_contextMenuPtr);
            _contextMenuPtr = IntPtr.Zero;
        }

        if (_contextMenu2Ptr != IntPtr.Zero)
        {
            Marshal.Release(_contextMenu2Ptr);
            _contextMenu2Ptr = IntPtr.Zero;
        }

        if (_contextMenu3Ptr != IntPtr.Zero)
        {
            Marshal.Release(_contextMenu3Ptr);
            _contextMenu3Ptr = IntPtr.Zero;
        }

        ReleaseAll();
    }

    /// <summary>
    /// Release all allocated interfaces, PIDLs 
    /// </summary>
    private void ReleaseAll()
    {
        if (_contextMenu != null)
        {
            Marshal.ReleaseComObject(_contextMenu);
            _contextMenu = null;
        }
        if (_contextMenu2 != null)
        {
            Marshal.ReleaseComObject(_contextMenu2);
            _contextMenu2 = null;
        }
        if (_contextMenu3 != null)
        {
            Marshal.ReleaseComObject(_contextMenu3);
            _contextMenu3 = null;
        }
        if (_parentFolder != null)
        {
            Marshal.ReleaseComObject(_parentFolder);
            _parentFolder = null;
        }
        if (_backgroundShellView != null)
        {
            Marshal.ReleaseComObject(_backgroundShellView);
            _backgroundShellView = null;
        }
        if (_listPIDL != null)
        {
            FreePIDLs();
            _listPIDL = null;
        }
    }

    #endregion

    #region Properties

    public bool Root { get; set; } = false;

    #endregion

    /// <summary>Gets the interfaces to the context menu</summary>
    /// <param name="arrPIDLs">PIDLs</param>
    /// <returns>true if it got the interfaces, otherwise false</returns>
    private bool GetContextMenuInterfaces(out IntPtr ctxMenuPtr)
    {
        if (_parentFolder == null || _listPIDL == null)
            throw new ExploripCommonException("Parent folder is null");
        Guid guid = typeof(IContextMenu).GUID;
        int nResult = _parentFolder.GetUIObjectOf(
            IntPtr.Zero,
            (uint)_listPIDL.Length,
            _listPIDL,
            ref guid,
            IntPtr.Zero,
            out ctxMenuPtr);

        if (nResult == (int)HResult.SUCCESS)
        {
            _contextMenu = (IContextMenu)Marshal.GetTypedObjectForIUnknown(ctxMenuPtr, typeof(IContextMenu));
            return true;
        }
        else
        {
            ctxMenuPtr = IntPtr.Zero;
            _contextMenu = null;
            return false;
        }
    }

    private void GetContextMenuInterfacesBackground(string fullPath)
    {
        Guid guidSv = typeof(IShellView).GUID;

        IntPtr pItemIDL = ILCreateFromPath(fullPath);
        GetDesktopFolder().BindToObject(pItemIDL, IntPtr.Zero, typeof(IShellFolder).GUID, out IntPtr opsf);
        ILFree(pItemIDL);
        _parentFolder = (IShellFolder)Marshal.GetObjectForIUnknown(opsf);
        _parentFolder.CreateViewObject(IntPtr.Zero, guidSv, out IntPtr opShellView);
        _backgroundShellView = (IShellView)Marshal.GetObjectForIUnknown(opShellView);
        _backgroundShellView.GetItemObject(ShellViewGetItemObject.Background, typeof(IContextMenu).GUID, out object opContextMenu);
        _contextMenu = (IContextMenu)opContextMenu;
        _contextMenu2 = (IContextMenu2)opContextMenu;
        _contextMenu3 = (IContextMenu3)opContextMenu;
    }

    private void InvokeCommand(uint nCmd, System.Windows.Point pointInvoke)
    {
        CmInvokeCommandInfoEx invoke = new()
        {
            cbSize = Marshal.SizeOf(typeof(CmInvokeCommandInfoEx)),
            lpVerbW = new IntPtr(nCmd),
            lpVerb = new IntPtr(nCmd),
            lpDirectoryW = _strParentFolder,
            lpDirectory = _strParentFolder,
            fMask = CMIC.UNICODE | CMIC.PTINVOKE |
                (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl) ? CMIC.CONTROL_DOWN : 0) |
                (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift) ? CMIC.SHIFT_DOWN : 0),
            ptInvoke = new Point((long)pointInvoke.X, (long)pointInvoke.Y),
            nShow = WindowShowStyle.ShowNormal,
            hwnd = ((MainWindow)System.Windows.Application.Current.MainWindow).MyDataContext.WindowHandle,
        };
        _contextMenu?.InvokeCommand(ref invoke);
        // TODO : New : https://superuser.com/questions/34704/how-can-i-add-an-item-to-the-new-context-menu
    }

    /// <summary>
    /// Gets the desktop folder
    /// </summary>
    /// <returns>IShellFolder for desktop folder</returns>
    public static IShellFolder GetDesktopFolder()
    {
        // Get desktop IShellFolder
        int nResult = SHGetDesktopFolder(out IntPtr pUnkownDesktopFolder);
        if (nResult != (int)HResult.SUCCESS)
            throw new ExploripCommonException("Failed to get the desktop shell folder");

        return (IShellFolder)Marshal.GetTypedObjectForIUnknown(pUnkownDesktopFolder, typeof(IShellFolder));
    }

    /// <summary>
    /// Gets the parent folder
    /// </summary>
    /// <param name="folderName">Folder path</param>
    /// <returns>IShellFolder for the folder (relative from the desktop)</returns>
    private void GetParentFolder(string? folderName, bool desktopParent)
    {
        if (!string.IsNullOrWhiteSpace(folderName) && !Root)
        {
            if (_parentFolder == null)
            {
                IShellFolder desktopFolder = GetDesktopFolder();
                if (desktopFolder == null)
                    return;

                uint pchEaten = 0;
                SFGAO pdwAttributes = 0;

                // Get the PIDL for the folder file is in
                int nResult = desktopFolder.ParseDisplayName(IntPtr.Zero, IntPtr.Zero, folderName, ref pchEaten, out IntPtr pPIDL, ref pdwAttributes);
                if (nResult != (int)HResult.SUCCESS)
                    return;

                IntPtr pStrRet = Marshal.AllocCoTaskMem(ShellHelper.MAX_PATH * 2 + 4);
                Marshal.WriteInt32(pStrRet, 0, 0);
                _ = desktopFolder.GetDisplayNameOf(pPIDL, SHGDN.FORPARSING, pStrRet);
                StringBuilder strFolder = new(ShellHelper.MAX_PATH);
                _ = StrRetToBuf(pStrRet, pPIDL, strFolder, ShellHelper.MAX_PATH);
                Marshal.FreeCoTaskMem(pStrRet);
                _strParentFolder = strFolder.ToString();

                // Get the IShellFolder for folder
                Guid guid = typeof(IShellFolder).GUID;
                nResult = desktopFolder.BindToObject(pPIDL, IntPtr.Zero, ref guid, out IntPtr pUnknownParentFolder);

                // Free the PIDL first
                Marshal.FreeCoTaskMem(pPIDL);
                if (nResult != (int)HResult.SUCCESS)
                    return;

                _parentFolder = (IShellFolder)Marshal.GetObjectForIUnknown(pUnknownParentFolder);
            }
        }
        else
        {
            if (desktopParent || Root)
                _parentFolder = GetDesktopFolder();
            else
            {
                IntPtr folderEnumPtr = IntPtr.Zero;
                IEnumIDList? folderEnum = null;
                IntPtr winHandle = IntPtr.Zero;

                IntPtr tempPidl;
                ShFileInfo info;

                info = new ShFileInfo();
                tempPidl = IntPtr.Zero;
                SHGetSpecialFolderLocation(IntPtr.Zero, CSIDL.CSIDL_DRIVES, ref tempPidl);

                SHGetFileInfo(tempPidl, 0, ref info, (uint)Marshal.SizeOf(info), SHGFI.PIDL | SHGFI.DisplayName | SHGFI.TypeName);

                string mycompName = info.szDisplayName;

                try
                {
                    _parentFolder = GetDesktopFolder();

                    if (_parentFolder.EnumObjects(winHandle, SHCONTF.FOLDERS | SHCONTF.INCLUDEHIDDEN, out folderEnumPtr) == (int)HResult.SUCCESS)
                    {
                        folderEnum = (IEnumIDList)Marshal.GetTypedObjectForIUnknown(folderEnumPtr, typeof(IEnumIDList));
                        while (folderEnum.Next(1, out IntPtr pidlSubItem, out uint celtFetched) == (int)HResult.SUCCESS && celtFetched == 1)
                        {
                            Guid guid = typeof(IShellFolder).GUID;
                            if (_parentFolder.BindToObject(
                                        pidlSubItem,
                                        IntPtr.Zero,
                                        ref guid,
                                        out IntPtr shellFolderPtr) == (int)HResult.SUCCESS)
                            {
                                IntPtr strr = Marshal.AllocCoTaskMem(ShellHelper.MAX_PATH * 2 + 4);
                                Marshal.WriteInt32(strr, 0, 0);
                                StringBuilder buf = new(ShellHelper.MAX_PATH);

                                string txt = "";
                                if (_parentFolder.GetDisplayNameOf(
                                                pidlSubItem,
                                                SHGDN.INFOLDER,
                                                strr) == (int)HResult.SUCCESS)
                                {
                                    int nResult = StrRetToBuf(strr, pidlSubItem, buf, ShellHelper.MAX_PATH);
                                    if (nResult == (int)HResult.SUCCESS)
                                        txt = buf.ToString();
                                }

                                Marshal.FreeCoTaskMem(strr);

                                if (txt == mycompName)
                                {
                                    _parentFolder = (IShellFolder)Marshal.GetObjectForIUnknown(shellFolderPtr);
                                    break;
                                }
                            }
                        }
                    }
                }
                catch (Exception) { /* Ignore errors */ }
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
    }

    #region GetPIDLs()

    /// <summary>
    /// Get the PIDLs
    /// </summary>
    /// <param name="arrFI">Array of FileInfo</param>
    /// <returns>Array of PIDLs</returns>
    protected void GetPIDLs(FileSystemInfo[] arrFI, string currentFolder)
    {
        if (arrFI == null || arrFI.Length == 0)
            return;

        GetParentFolder(currentFolder, false);
        if (_parentFolder == null)
            return;

        _listPIDL = new IntPtr[arrFI.Length];
        int n = 0;
        foreach (FileSystemInfo fi in arrFI)
        {
            // Get the file relative to folder
            uint pchEaten = 0;
            SFGAO pdwAttributes = 0;
            int nResult = _parentFolder.ParseDisplayName(IntPtr.Zero, IntPtr.Zero, fi.Name, ref pchEaten, out IntPtr pPIDL, ref pdwAttributes);
            if (nResult != (int)HResult.SUCCESS)
            {
                FreePIDLs();
                return;
            }
            _listPIDL[n] = pPIDL;
            n++;
        }
    }

    /// <summary>
    /// Get the PIDLs
    /// </summary>
    protected void GetPIDLs(DirectoryInfo dirInfo)
    {
        if (dirInfo == null)
            return;

        if (!Root && dirInfo.FullName.Length > 3)
            GetParentFolder(dirInfo.Parent!.FullName, false);
        else
            GetParentFolder(null, dirInfo.Name == Environment.SpecialFolder.MyComputer.RealName() || dirInfo.Name == Environment.SpecialFolder.Desktop.RealName());

        if (_parentFolder == null)
            throw new ExploripCommonException("Parent folder is null");

        _listPIDL = new IntPtr[1];

        // Get the file relative to folder
        uint pchEaten = 0;
        SFGAO pdwAttributes = 0;
        int nResult = _parentFolder.ParseDisplayName(IntPtr.Zero, IntPtr.Zero, dirInfo.Name, ref pchEaten, out IntPtr pPIDL, ref pdwAttributes);
        if (nResult != (int)HResult.SUCCESS && Root)
        {
            nResult = SHGetSpecialFolderLocation(IntPtr.Zero, CSIDL.CSIDL_DRIVES, ref pPIDL);
        }
        if (pPIDL != IntPtr.Zero && nResult == (int)HResult.SUCCESS)
        {
            _listPIDL[0] = pPIDL;
        }
    }

    #endregion

    #region ShowContextMenu()

    public void ShowContextMenu(FileSystemInfo[] fsi, string currentFolder, System.Windows.Point pointScreen)
    {
        ReleaseAll();
        GetPIDLs(fsi, currentFolder);
        ShowContextMenu(pointScreen);
    }

    /// <summary>
    /// Show default context menu of folder
    /// </summary>
    public void ShowContextMenu(DirectoryInfo dir, System.Windows.Point pointScreen, bool background = true)
    {
        ReleaseAll();
        if (background)
        {
            GetContextMenuInterfacesBackground(dir.FullName);
            _strCurrentFolder = dir.FullName;
        }
        else
            GetPIDLs(dir);
        ShowContextMenu(pointScreen, background);
    }

    /// <summary>
    /// Shows the context menu
    /// </summary>
    /// <param name="pointScreen">Where to show the menu</param>
    private void ShowContextMenu(System.Windows.Point pointScreen, bool background = false)
    {
        try
        {
            if ((!background) &&
                (_listPIDL == null || !GetContextMenuInterfaces(out _contextMenuPtr)))
            {
                ReleaseAll();
                return;
            }

            if (_contextMenu == null)
                throw new ExploripCommonException("ContextMenu interface not initialized");

            _pMenu = CreatePopupMenu();

            CMF flag = CMF.NORMAL;
            if (!background)
            {
                if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                    flag |= CMF.EXTENDEDVERBS;
                flag |= CMF.CANRENAME;
            }

            _contextMenu.QueryContextMenu(
                _pMenu,
                0,
                0,
                (uint)short.MaxValue,
                flag);

            if (_contextMenu2 == null)
            {
                Guid guid = typeof(IContextMenu2).GUID;
                Marshal.QueryInterface(_contextMenuPtr, ref guid, out _contextMenu2Ptr);
                _contextMenu2 = (IContextMenu2)Marshal.GetTypedObjectForIUnknown(_contextMenu2Ptr, typeof(IContextMenu2));
            }
            if (_contextMenu3 == null)
            {
                Guid guid = typeof(IContextMenu3).GUID;
                Marshal.QueryInterface(_contextMenuPtr, ref guid, out _contextMenu3Ptr);
                _contextMenu3 = (IContextMenu3)Marshal.GetTypedObjectForIUnknown(_contextMenu3Ptr, typeof(IContextMenu3));
            }

            uint cmdPaste = 0, cmdPasteShortcut = 0;
            if (background)
                EnablePaste(_pMenu, out cmdPaste, out cmdPasteShortcut);

            uint nSelected = 0;

            ExpandSubMenu(_pMenu, _contextMenu2);

            nSelected = TrackPopupMenuEx(
                _pMenu,
                TPM.RETURNCMD | TPM.RIGHTBUTTON,
                (int)pointScreen.X,
                (int)pointScreen.Y,
                ((MainWindow)System.Windows.Application.Current.Windows[0]).MyDataContext.WindowHandle,
                IntPtr.Zero);

            DestroyMenu(_pMenu);
            _pMenu = IntPtr.Zero;

            if (nSelected != 0)
            {
                if (background)
                {
                    if (nSelected == cmdPaste)
                        PasteClipboard();
                    else if (nSelected == cmdPasteShortcut)
                        PasteShortcutClipboard();
                    else
                        InvokeCommand(nSelected, pointScreen);
                }
                else
                    InvokeCommand(nSelected, pointScreen);
            }
        }
        catch (Exception)
        {
            if (System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Break();
        }
        finally
        {
            CleanUp();
        }
    }

    #endregion

    #region Manipulate submenu

    private static void ExpandSubMenu(IntPtr pMenu, IContextMenu2 cm2)
    {
        if (pMenu == IntPtr.Zero)
            return;
        int nbMenu = GetMenuItemCount(pMenu);
        if (nbMenu > 0)
        {
            int IdMenu;
            for (int i = 0; i < nbMenu; i++)
            {
                IdMenu = GetMenuItemID(pMenu, i);
                if (IdMenu < 0)
                {
                    IntPtr IdSousMenu = GetSubMenu(pMenu, i);
                    cm2.HandleMenuMsg((int)WM.INITMENUPOPUP, IdSousMenu, (IntPtr)i);
                }
            }
        }
    }

    private void PasteClipboard()
    {
        System.Windows.DataObject data = (System.Windows.DataObject)System.Windows.Clipboard.GetDataObject();
        string[] listItems = (string[])data.GetData(System.Windows.DataFormats.FileDrop);
        byte[] buffer = new byte[4];
        bool move = false;
        if (((MemoryStream)System.Windows.Clipboard.GetData("Preferred DropEffect")).Read(buffer, 0, 4) > 0)
        {
            int dropEffect = BitConverter.ToInt32(buffer, 0);
            if (dropEffect == 2)
                move = true;
        }
        Explorip.HookFileOperations.FilesOperations.FileOperation fileOp = new(GetDesktopWindow());
        foreach (string fs in listItems)
            if (move)
                fileOp.MoveItem(fs, _strCurrentFolder, Path.GetFileName(fs));
            else
                fileOp.CopyItem(fs, _strCurrentFolder, Path.GetFileName(fs));
        fileOp.ChangeOperationFlags(Explorip.HookFileOperations.FilesOperations.EFileOperation.FOF_RENAMEONCOLLISION |
            Explorip.HookFileOperations.FilesOperations.EFileOperation.FOF_NOCONFIRMMKDIR |
            Explorip.HookFileOperations.FilesOperations.EFileOperation.FOFX_ADDUNDORECORD);
        fileOp.PerformOperations();
        fileOp.Dispose();
    }

    private void PasteShortcutClipboard()
    {
        System.Windows.DataObject data = (System.Windows.DataObject)System.Windows.Clipboard.GetDataObject();
        string[] listItems = (string[])data.GetData(System.Windows.DataFormats.FileDrop);
        string shortcutLabel = Explorip.Constants.Localization.NEW_SHORTCUT_NAME.Replace(" ().lnk", "");
        Shortcut sc;
        int iteration;
        foreach (string fs in listItems)
        {
            string name = shortcutLabel.Replace("%s", Path.GetFileNameWithoutExtension(fs)) + ".lnk";
            iteration = 1;
            while (File.Exists(Path.Combine(_strCurrentFolder, name)))
            {
                name = shortcutLabel.Replace("%s", Path.GetFileNameWithoutExtension(fs)) + $" ({iteration}).lnk";
                iteration++;
            }
            sc = Shortcut.CreateShortcut(fs);
            sc.WriteToFile(Path.Combine(_strCurrentFolder, name));
        }
    }

    private void EnablePaste(IntPtr pMenu, out uint cmdPaste, out uint cmdPasteShortcut)
    {
        cmdPaste = 0;
        cmdPasteShortcut = 0;
        if (pMenu != IntPtr.Zero)
        {
            int nbMenu = GetMenuItemCount(pMenu);
            if (nbMenu > 0)
            {
                int IdMenu;
                for (int i = 0; i < nbMenu; i++)
                {
                    IdMenu = GetMenuItemID(pMenu, i);
                    if (IdMenu > 0)
                    {
                        string? libelle = GetMenuItemString(IdMenu, pMenu, false, out uint cmd);
                        if (string.IsNullOrWhiteSpace(libelle))
                        {
                            if (i == nbMenu - 1)
                                RemoveMenu(pMenu, (uint)IdMenu, false);
                            continue;
                        }
                        bool bPaste = libelle!.Trim().ToLower().Replace("&", "") == Explorip.Constants.Localization.PASTE.Trim().ToLower();
                        if (!string.IsNullOrWhiteSpace(libelle) &&
                            (bPaste ||
                             libelle.Trim().ToLower().Replace("&", "") == Explorip.Constants.Localization.PASTE_SHORTCUT.Trim().ToLower()) &&
                             PasteAvailable())
                        {
                            MenuItemInfo mi = new()
                            {
                                cbSize = (uint)Marshal.SizeOf(typeof(MenuItemInfo)),
                                fMask = MIIM.STATE,
                                fState = MFS.ENABLED,
                            };
                            if (bPaste)
                                cmdPaste = cmd;
                            else
                                cmdPasteShortcut = cmd;
                            SetMenuItemInfo(pMenu, (uint)IdMenu, false, ref mi);
                        }
                    }
                }
            }
        }
    }

    public static bool PasteAvailable()
    {
        System.Windows.IDataObject data = System.Windows.Clipboard.GetDataObject();
        return data.GetDataPresent(System.Windows.DataFormats.FileDrop);
    }

    private string? GetMenuItemString(int IdOrPositionMenu, IntPtr pointeurMenu, bool usePosition, out uint cmd)
    {
        try
        {
            MenuItemInfo sortie = new()
            {
                cbSize = (uint)Marshal.SizeOf(typeof(MenuItemInfo)),
                dwTypeData = new string('\0', 256),
                fMask = MIIM.STRING | MIIM.STATE | MIIM.ID | MIIM.BITMAP,
                fType = ManagedShell.Interop.NativeMethods.MFT.STRING | ManagedShell.Interop.NativeMethods.MFT.DISABLED | ManagedShell.Interop.NativeMethods.MFT.GRAYED
            };
            sortie.cch = sortie.dwTypeData.Length - 1;
            if (GetMenuItemInfo(pointeurMenu, (uint)IdOrPositionMenu, usePosition, ref sortie))
            {
                cmd = sortie.wID;
                return sortie.dwTypeData;
            }
        }
        catch (Exception) { /* Ignore errors */ }
        cmd = 0;
        return null;
    }

    #endregion
}
