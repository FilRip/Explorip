using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Input;

using Explorip.Helpers;
using Explorip.HookFileOperations.FilesOperations;

using ManagedShell.Common.Helpers;
using ManagedShell.ShellFolders.Enums;
using ManagedShell.ShellFolders.Interfaces;
using ManagedShell.ShellFolders.Structs;

using Securify.ShellLink;

using Windows.UI.WebUI;

using static ManagedShell.Interop.NativeMethods;

using Localization = Explorip.Constants.Localization;
using MFT = ManagedShell.Interop.NativeMethods.MFT;
using Point = ManagedShell.Interop.NativeMethods.Point;

namespace ExploripComponents;

/// <summary>Default constructor</summary>
public class ShellContextMenu(WpfExplorerViewModel viewModel)
{
    #region Fields

    private IntPtr _pMenu = IntPtr.Zero, _contextMenuPtr = IntPtr.Zero, _contextMenu2Ptr = IntPtr.Zero, _contextMenu3Ptr = IntPtr.Zero;
    private IContextMenu? _contextMenu;
    private IContextMenu2? _contextMenu2;
    private IContextMenu3? _contextMenu3;
    private IShellFolder? _parentFolder;
    private IntPtr[]? _listPIDL;
    private string? _strParentFolder;
    private string? _strCurrentFolder;
    private IShellView? _backgroundShellView;
    private readonly WpfExplorerViewModel _viewModel = viewModel;
    private uint _cmdPaste, _cmdPasteShortcut, _cmdRename;
    private uint _cmdDetails, _cmdSmall, _cmdLarge, _cmdExtraLarge, _cmdJumbo;
    private uint _cmdGbName, _cmdGbType, _cmdGbSize, _cmdGbLastModified, _cmdGbNone;

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
    /// <returns>true if it got the interfaces, otherwise false</returns>
    private bool GetContextMenuInterfaces()
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
            out _contextMenuPtr);

        if (nResult == (int)HResult.SUCCESS)
        {
            _contextMenu = (IContextMenu)Marshal.GetTypedObjectForIUnknown(_contextMenuPtr, typeof(IContextMenu));
            return true;
        }
        else
        {
            _contextMenuPtr = IntPtr.Zero;
            _contextMenu = null;
            return false;
        }
    }

    private void GetContextMenuInterfacesBackground(string fullPath)
    {
        Guid guidSv = typeof(IShellView).GUID;

        if (fullPath == "::{B4BFCC3A-DB2C-424C-B029-7FE99A87C641}")
            _parentFolder = GetDesktopFolder();
        else
        {
            IntPtr pItemIDL = ILCreateFromPath(fullPath);
            GetDesktopFolder().BindToObject(pItemIDL, IntPtr.Zero, typeof(IShellFolder).GUID, out IntPtr opsf);
            ILFree(pItemIDL);
            _parentFolder = (IShellFolder)Marshal.GetObjectForIUnknown(opsf);
        }
        _parentFolder.CreateViewObject(IntPtr.Zero, guidSv, out IntPtr opShellView);
        _backgroundShellView = (IShellView)Marshal.GetObjectForIUnknown(opShellView);
        object opContextMenu = _backgroundShellView.GetItemObject(ShellViewGetItemObject.Background, typeof(IContextMenu).GUID);
        object opContextMenu2 = _backgroundShellView.GetItemObject(ShellViewGetItemObject.Background, typeof(IContextMenu2).GUID);
        object opContextMenu3 = _backgroundShellView.GetItemObject(ShellViewGetItemObject.Background, typeof(IContextMenu3).GUID);

        if (opContextMenu != null)
            _contextMenu = (IContextMenu)opContextMenu;
        if (opContextMenu2 != null)
            _contextMenu2 = (IContextMenu2)opContextMenu2;
        if (opContextMenu3 != null)
            _contextMenu3 = (IContextMenu3)opContextMenu3;
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
            hwnd = _viewModel.WindowHandle,
        };
        _contextMenu?.InvokeCommand(ref invoke);
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
    public void ShowContextMenu(DirectoryInfo dir, System.Windows.Point pointScreen)
    {
        ReleaseAll();
        GetPIDLs(dir);
        ShowContextMenu(pointScreen);
    }

    public void ShowContextMenu(string dir, System.Windows.Point pointScreen)
    {
        ReleaseAll();

        GetContextMenuInterfacesBackground(dir);
        _strCurrentFolder = dir;

        ShowContextMenu(pointScreen, true);
    }

    #endregion

    /// <summary>
    /// Shows the context menu
    /// </summary>
    /// <param name="pointScreen">Where to show the menu</param>
    private void ShowContextMenu(System.Windows.Point pointScreen, bool background = false)
    {
        try
        {
            if ((!background) &&
                (_listPIDL == null || !GetContextMenuInterfaces()))
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

            if (_contextMenu2 == null && _contextMenuPtr != IntPtr.Zero)
            {
                Guid guid = typeof(IContextMenu2).GUID;
                Marshal.QueryInterface(_contextMenuPtr, ref guid, out _contextMenu2Ptr);
                _contextMenu2 = (IContextMenu2)Marshal.GetTypedObjectForIUnknown(_contextMenu2Ptr, typeof(IContextMenu2));
            }
            if (_contextMenu3 == null && _contextMenuPtr != IntPtr.Zero)
            {
                Guid guid = typeof(IContextMenu3).GUID;
                Marshal.QueryInterface(_contextMenuPtr, ref guid, out _contextMenu3Ptr);
                _contextMenu3 = (IContextMenu3)Marshal.GetTypedObjectForIUnknown(_contextMenu3Ptr, typeof(IContextMenu3));
            }

            _contextMenu.QueryContextMenu(
                _pMenu,
                0,
                0,
                (uint)short.MaxValue,
                flag);

            RemoveLastEmptyMenuItem(_pMenu);

            ExpandSubMenu(_pMenu);

            GetSpecialCmd(_pMenu, background);

            uint nSelected = TrackPopupMenuEx(
                _pMenu,
                TPM.RETURNCMD | TPM.RIGHTBUTTON,
                (int)pointScreen.X,
                (int)pointScreen.Y,
                _viewModel.WindowHandle,
                IntPtr.Zero);

            if (nSelected != 0)
            {
                if (background && _backgroundShellView != null)
                {
                    if (nSelected == _cmdPaste)
                        PasteClipboard();
                    else if (nSelected == _cmdPasteShortcut)
                        PasteShortcutClipboard();
                    else if (nSelected == _cmdDetails)
                        _viewModel.ChangeIconSize(true, ManagedShell.Common.Enums.IconSize.Small);
                    else if (nSelected == _cmdSmall)
                        _viewModel.ChangeIconSize(false, ManagedShell.Common.Enums.IconSize.Small);
                    else if (nSelected == _cmdLarge)
                        _viewModel.ChangeIconSize(false, ManagedShell.Common.Enums.IconSize.Large);
                    else if (nSelected == _cmdExtraLarge)
                        _viewModel.ChangeIconSize(false, ManagedShell.Common.Enums.IconSize.ExtraLarge);
                    else if (nSelected == _cmdJumbo)
                        _viewModel.ChangeIconSize(false, ManagedShell.Common.Enums.IconSize.Jumbo);
                    else if (nSelected == _cmdGbName)
                        _viewModel.ChangeGroupBy(GroupBy.NAME);
                    else if (nSelected == _cmdGbLastModified)
                        _viewModel.ChangeGroupBy(GroupBy.LAST_MODIFIED);
                    else if (nSelected == _cmdGbSize)
                        _viewModel.ChangeGroupBy(GroupBy.SIZE);
                    else if (nSelected == _cmdGbType)
                        _viewModel.ChangeGroupBy(GroupBy.TYPE);
                    else if (nSelected == _cmdGbNone)
                        _viewModel.ChangeGroupBy(GroupBy.NONE);
                    else
                        InvokeCommand(nSelected, pointScreen);
                }
                else
                {
                    if (nSelected == _cmdRename)
                        _viewModel.RenameMode();
                    else
                        InvokeCommand(nSelected, pointScreen);
                }
            }

            DestroyMenu(_pMenu);
            _pMenu = IntPtr.Zero;
        }
        catch (Exception)
        {
            if (Debugger.IsAttached)
                Debugger.Break();
        }
        finally
        {
            CleanUp();
        }
    }

    #region Manipulate submenu

    private void ExpandSubMenu(IntPtr pMenu)
    {
        if (pMenu == IntPtr.Zero || _contextMenu2 == null)
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
                    IntPtr IdSubMenu = GetSubMenu(pMenu, i);
                    _contextMenu2.HandleMenuMsg((int)WM.INITMENUPOPUP, IdSubMenu, (IntPtr)i);
                }
            }
        }
    }

    private void PasteClipboard()
    {
        DataObject data = (DataObject)Clipboard.GetDataObject();
        string[] listItems = (string[])data.GetData(DataFormats.FileDrop);
        byte[] buffer = new byte[4];
        bool move = false;
        if (((MemoryStream)Clipboard.GetData("Preferred DropEffect")).Read(buffer, 0, 4) > 0)
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
        fileOp.ChangeOperationFlags(EFileOperation.FOF_RENAMEONCOLLISION |
            EFileOperation.FOF_NOCONFIRMMKDIR |
            EFileOperation.FOFX_ADDUNDORECORD);
        fileOp.PerformOperations();
        fileOp.Dispose();
    }

    private void PasteShortcutClipboard()
    {
        DataObject data = (DataObject)Clipboard.GetDataObject();
        string[] listItems = (string[])data.GetData(DataFormats.FileDrop);
        string shortcutLabel = Localization.NEW_SHORTCUT_NAME.Replace(" ().lnk", "");
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

    private void RemoveLastEmptyMenuItem(IntPtr pMenu)
    {
        int nbMenu = GetMenuItemCount(pMenu);
        string? libelle = GetMenuItemString(nbMenu - 1, pMenu, true, out _);
        if (string.IsNullOrWhiteSpace(libelle))
            RemoveMenu(pMenu, (uint)nbMenu - 1, MF.BYPOSITION);
    }

    private void GetSpecialCmd(IntPtr pMenu, bool background)
    {
        _cmdPaste = 0;
        _cmdPasteShortcut = 0;
        _cmdRename = 0;

        if (pMenu != IntPtr.Zero)
        {
            int nbMenu = GetMenuItemCount(pMenu);
            if (nbMenu > 0)
            {
                for (uint i = 0; i < nbMenu; i++)
                {
                    string? label = GetMenuItemStringWithSubmenu((int)i, pMenu, true, out uint cmd, out IntPtr hMenu);
                    if (string.IsNullOrWhiteSpace(label))
                        continue;
                    if (background)
                    {
                        bool bPaste = label!.Trim().ToLower().Replace("&", "") == Localization.PASTE.Trim().ToLower();
                        if (!string.IsNullOrWhiteSpace(label) &&
                            (bPaste ||
                                label.Trim().ToLower().Replace("&", "") == Localization.PASTE_SHORTCUT.Trim().ToLower()) &&
                                PasteAvailable())
                        {
                            MenuItemInfo mi = new()
                            {
                                cbSize = (uint)Marshal.SizeOf(typeof(MenuItemInfo)),
                                fMask = MIIM.STATE,
                                fState = MFS.ENABLED,
                            };
                            if (bPaste)
                                _cmdPaste = cmd;
                            else
                                _cmdPasteShortcut = cmd;
                            SetMenuItemInfo(pMenu, i, true, ref mi);
                            continue;
                        }
                        if (hMenu != IntPtr.Zero)
                        {
                            if (label.Replace("&", "").Trim().ToLower() == Localization.SHOW_SUBMENU.Trim().ToLower())
                            {
                                int nbSubMenu = GetMenuItemCount(hMenu);
                                for (int j = 0; j < nbSubMenu; j++)
                                {
                                    string? subLabel = GetMenuItemString(j, hMenu, true, out uint subCmd);
                                    if (string.IsNullOrWhiteSpace(subLabel))
                                        continue;
                                    if (subLabel!.Trim().ToLower().Replace("&", "") == Localization.SHOW_DETAILS_SUBMENU.Trim().ToLower())
                                    {
                                        _cmdDetails = subCmd;
                                        CheckMenuItem(hMenu, (uint)j, true, _viewModel.ViewDetails);
                                    }
                                    else if (subLabel!.Trim().ToLower().Replace("&", "") == Localization.SHOW_SMALL_SUBMENU.Trim().ToLower())
                                    {
                                        _cmdSmall = subCmd;
                                        CheckMenuItem(hMenu, (uint)j, true, !_viewModel.ViewDetails && _viewModel.CurrentIconSize == ManagedShell.Common.Enums.IconSize.Small);
                                    }
                                    else if (subLabel!.Trim().ToLower().Replace("&", "") == Localization.SHOW_LARGE_SUBMENU.Trim().ToLower())
                                    {
                                        _cmdLarge = subCmd;
                                        CheckMenuItem(hMenu, (uint)j, true, !_viewModel.ViewDetails && _viewModel.CurrentIconSize == ManagedShell.Common.Enums.IconSize.Large);
                                    }
                                    else if (subLabel!.Trim().ToLower().Replace("&", "") == Localization.SHOW_EXTRALARGE_SUBMENU.Trim().ToLower())
                                    {
                                        _cmdExtraLarge = subCmd;
                                        CheckMenuItem(hMenu, (uint)j, true, !_viewModel.ViewDetails && _viewModel.CurrentIconSize == ManagedShell.Common.Enums.IconSize.ExtraLarge);
                                    }
                                    else if (subLabel!.Trim().ToLower().Replace("&", "") == Localization.SHOW_JUMBO_SUBMENU.Trim().ToLower())
                                    {
                                        _cmdJumbo = subCmd;
                                        CheckMenuItem(hMenu, (uint)j, true, !_viewModel.ViewDetails && _viewModel.CurrentIconSize == ManagedShell.Common.Enums.IconSize.Jumbo);
                                    }
                                }
                            }
                            else if (label.Replace("&", "").Trim().ToLower() == Localization.GROUP_BY_SUBMENU.Trim().ToLower())
                            {
                                int nbSubMenu = GetMenuItemCount(hMenu);
                                for (int j = 0; j < nbSubMenu; j++)
                                {
                                    string? subLabel = GetMenuItemString(j, hMenu, true, out uint subCmd);
                                    if (string.IsNullOrWhiteSpace(subLabel))
                                        continue;
                                    if (subLabel!.Trim().ToLower().Replace("&", "") == Localization.GROUPBY_NAME_SUBMENU.Trim().ToLower())
                                    {
                                        _cmdGbName = subCmd;
                                        CheckMenuItem(hMenu, (uint)j, true, _viewModel.CurrentGroup == GroupBy.NAME);
                                    }
                                    else if (subLabel!.Trim().ToLower().Replace("&", "") == Localization.GROUPBY_LASTMODIFIED_SUBMENU.Trim().ToLower())
                                    {
                                        _cmdGbLastModified = subCmd;
                                        CheckMenuItem(hMenu, (uint)j, true, _viewModel.CurrentGroup == GroupBy.LAST_MODIFIED);
                                    }
                                    else if (subLabel!.Trim().ToLower().Replace("&", "") == Localization.GROUPBY_TYPE_SUBMENU.Trim().ToLower())
                                    {
                                        _cmdGbType = subCmd;
                                        CheckMenuItem(hMenu, (uint)j, true, _viewModel.CurrentGroup == GroupBy.TYPE);
                                    }
                                    else if (subLabel!.Trim().ToLower().Replace("&", "") == Localization.GROUPBY_SIZE_SUBMENU.Trim().ToLower())
                                    {
                                        _cmdGbSize = subCmd;
                                        CheckMenuItem(hMenu, (uint)j, true, _viewModel.CurrentGroup == GroupBy.SIZE);
                                    }
                                    else if (subLabel!.Trim().ToLower().Replace("&", "") == Localization.GROUPBY_NONE_SUBMENU.Trim().ToLower())
                                    {
                                        _cmdGbNone = subCmd;
                                    }
                                }
                            }
                        }
                        continue;
                    }
                    else if (label!.Trim().ToLower() == Localization.RENAME_MENUITEM.Trim().ToLower())
                    {
                        _cmdRename = cmd;
                        continue;
                    }
                }
            }
        }
    }

    private void CheckMenuItem(IntPtr pMenu, uint numMenu, bool usePosition, bool isChecked)
    {
        MenuItemInfo mi = new()
        {
            cbSize = (uint)Marshal.SizeOf(typeof(MenuItemInfo)),
            fMask = MIIM.STATE,
            fState = (isChecked ? MFS.CHECKED : MFS.UNCHECKED),
        };
        SetMenuItemInfo(pMenu, numMenu, usePosition, ref mi);
    }

    public static bool PasteAvailable()
    {
        IDataObject data = Clipboard.GetDataObject();
        return data.GetDataPresent(DataFormats.FileDrop);
    }

    private string? GetMenuItemString(int IdOrPositionMenu, IntPtr pMenu, bool usePosition, out uint cmd)
    {
        try
        {
            MenuItemInfo result = new()
            {
                cbSize = (uint)Marshal.SizeOf(typeof(MenuItemInfo)),
                dwTypeData = new string('\0', 256),
                fMask = MIIM.STRING | MIIM.STATE | MIIM.ID,
                fType = MFT.STRING | MFT.DISABLED | MFT.GRAYED,
            };
            result.cch = result.dwTypeData.Length - 1;
            if (GetMenuItemInfo(pMenu, (uint)IdOrPositionMenu, usePosition, ref result))
            {
                cmd = result.wID;
                return result.dwTypeData;
            }
        }
        catch (Exception) { /* Ignore errors */ }
        cmd = 0;

        return null;
    }

    private string? GetMenuItemStringWithSubmenu(int IdOrPositionMenu, IntPtr pMenu, bool usePosition, out uint cmd, out IntPtr hMenu)
    {
        try
        {
            MenuItemInfo result = new()
            {
                cbSize = (uint)Marshal.SizeOf(typeof(MenuItemInfo)),
                dwTypeData = new string('\0', 256),
                fMask = MIIM.STRING | MIIM.STATE | MIIM.ID | MIIM.SUBMENU,
                fType = MFT.STRING | MFT.DISABLED | MFT.GRAYED,
            };
            result.cch = result.dwTypeData.Length - 1;
            if (GetMenuItemInfo(pMenu, (uint)IdOrPositionMenu, usePosition, ref result))
            {
                cmd = result.wID;
                hMenu = result.hSubMenu;
                return result.dwTypeData;
            }
        }
        catch (Exception) { /* Ignore errors */ }
        cmd = 0;
        hMenu = IntPtr.Zero;

        return null;
    }

    #endregion
}
