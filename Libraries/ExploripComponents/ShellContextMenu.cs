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

    private IntPtr _pMenu, _iContextMenuPtr, _iContextMenuPtr2, _iContextMenuPtr3;
    private IContextMenu? _oContextMenu;
    private IContextMenu2? _oContextMenu2;
    private IContextMenu3? _oContextMenu3;
    private IShellFolder? _oParentFolder;
    private IntPtr[]? _arrPIDLs;
    private string? _strParentFolder;
    private const uint CMD_FIRST = 0;
    private const uint CMD_LAST = (uint)short.MaxValue;
    private string? _strBackgroundFolder;
    /*private Microsoft.WindowsAPICodePack.Shell.Interop.ExplorerBrowser.ExplorerBrowserClass? _explorerBrowser;
    private System.Windows.Window? _explorerWindow;*/
    private IShellView _shellView;
    #endregion

    #region Constructor

    /// <summary>Default constructor</summary>
    public ShellContextMenu()
    {
        _pMenu = IntPtr.Zero;
        _iContextMenuPtr = IntPtr.Zero;
        _iContextMenuPtr2 = IntPtr.Zero;
        _iContextMenuPtr3 = IntPtr.Zero;
        CleanUp();
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
    /// <param name="arrPIDLs">Array of PIDLs (IntPtr)</param>
    protected static void FreePIDLs(IntPtr[] arrPIDLs)
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

    private void CleanUp()
    {
        if (_pMenu != IntPtr.Zero)
        {
            DestroyMenu(_pMenu);
            _pMenu = IntPtr.Zero;
        }

        if (_iContextMenuPtr != IntPtr.Zero)
        {
            Marshal.Release(_iContextMenuPtr);
            _iContextMenuPtr = IntPtr.Zero;
        }

        if (_iContextMenuPtr2 != IntPtr.Zero)
        {
            Marshal.Release(_iContextMenuPtr2);
            _iContextMenuPtr2 = IntPtr.Zero;
        }

        if (_iContextMenuPtr3 != IntPtr.Zero)
        {
            Marshal.Release(_iContextMenuPtr3);
            _iContextMenuPtr3 = IntPtr.Zero;
        }

        ReleaseAll();
    }

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
        /*_explorerBrowser?.Destroy();
        if (_explorerWindow != null)
        {
            _explorerWindow.Close();
            _explorerWindow = null;
        }*/
    }

    #endregion

    #region Properties

    public bool Root { get; set; } = false;

    #endregion

    /// <summary>Gets the interfaces to the context menu</summary>
    /// <param name="oParentFolder">Parent folder</param>
    /// <param name="arrPIDLs">PIDLs</param>
    /// <returns>true if it got the interfaces, otherwise false</returns>
    private bool GetContextMenuInterfaces(IntPtr[] arrPIDLs, out IntPtr ctxMenuPtr)
    {
        if (_oParentFolder == null)
            throw new ExploripCommonException("Parent folder is null");
        Guid guid = typeof(IContextMenu).GUID;
        int nResult = _oParentFolder.GetUIObjectOf(
            IntPtr.Zero,
            (uint)arrPIDLs.Length,
            arrPIDLs,
            ref guid,
            IntPtr.Zero,
            out ctxMenuPtr);

        if (nResult == (int)HResult.SUCCESS)
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

    private void GetContextMenuInterfacesBackground(string fullPath)
    {
        object opContextMenu;
        Guid guidSv = typeof(IShellView).GUID;

        IntPtr pItemIDL = ILCreateFromPath(fullPath);
        GetDesktopFolder().BindToObject(pItemIDL, IntPtr.Zero, typeof(IShellFolder).GUID, out IntPtr opsf);
        ILFree(pItemIDL);
        _oParentFolder = (IShellFolder)Marshal.GetObjectForIUnknown(opsf);
        _oParentFolder.CreateViewObject(IntPtr.Zero, guidSv, out IntPtr opShellView);
        _shellView = (IShellView)Marshal.GetObjectForIUnknown(opShellView);
        _shellView.GetItemObject(ShellViewGetItemObject.Background, typeof(IContextMenu).GUID, out opContextMenu);

        /*_explorerBrowser = new Microsoft.WindowsAPICodePack.Shell.Interop.ExplorerBrowser.ExplorerBrowserClass();
        _explorerWindow = new System.Windows.Window();
        _explorerWindow.Show();
        IntPtr handleWindow = new System.Windows.Interop.WindowInteropHelper(_explorerWindow).EnsureHandle();
        Microsoft.WindowsAPICodePack.Shell.Common.NativeRect rect = new(0, 32, (int)_explorerWindow.Width, (int)_explorerWindow.Height - 32);
        _explorerBrowser.Initialize(handleWindow, ref rect, null);
        _explorerBrowser.SetFolderSettings(new Microsoft.WindowsAPICodePack.Shell.Interop.ExplorerBrowser.FolderSettings() { Options = Microsoft.WindowsAPICodePack.Shell.Interop.ExplorerBrowser.FolderOptions.NoColumnHeaders | Microsoft.WindowsAPICodePack.Shell.Interop.ExplorerBrowser.FolderOptions.HideFilenames | Microsoft.WindowsAPICodePack.Shell.Interop.ExplorerBrowser.FolderOptions.NoSubfolders, ViewMode = Microsoft.WindowsAPICodePack.Shell.Interop.ExplorerBrowser.FolderViewMode.Auto });
        _explorerBrowser.SetOptions(Microsoft.WindowsAPICodePack.Shell.Interop.ExplorerBrowser.ExplorerBrowserOptions.NavigateOnce | Microsoft.WindowsAPICodePack.Shell.Interop.ExplorerBrowser.ExplorerBrowserOptions.NoWrapperWindow | Microsoft.WindowsAPICodePack.Shell.Interop.ExplorerBrowser.ExplorerBrowserOptions.NoBorder | Microsoft.WindowsAPICodePack.Shell.Interop.ExplorerBrowser.ExplorerBrowserOptions.NoTravelLog);
        _explorerBrowser.BrowseToObject(Microsoft.WindowsAPICodePack.Shell.Common.ShellObject.FromParsingName(fullPath).NativeShellItem, 0);
        _explorerBrowser.GetCurrentView(ref guidSv, out IntPtr newpShellView);
        IShellView newShellView = (IShellView)Marshal.GetTypedObjectForIUnknown(newpShellView, typeof(IShellView));
        newShellView.GetItemObject(ShellViewGetItemObject.Background, typeof(IContextMenu).GUID, out opContextMenu);*/

        _oContextMenu = (IContextMenu)opContextMenu;
        _oContextMenu2 = (IContextMenu2)opContextMenu;
        _oContextMenu3 = (IContextMenu3)opContextMenu;
    }

    private void InvokeCommand(uint nCmd, System.Windows.Point pointInvoke)
    {
        CmInvokeCommandInfoEx invoke = new()
        {
            cbSize = Marshal.SizeOf(typeof(CmInvokeCommandInfoEx)),
            lpVerb = (IntPtr)(nCmd - CMD_FIRST),
            lpDirectory = _strParentFolder,
            lpVerbW = (IntPtr)(nCmd - CMD_FIRST),
            lpDirectoryW = _strParentFolder,
            fMask = CMIC.UNICODE | CMIC.PTINVOKE |
            (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl) ? CMIC.CONTROL_DOWN : 0) |
            (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift) ? CMIC.SHIFT_DOWN : 0),
            ptInvoke = new Point((long)pointInvoke.X, (long)pointInvoke.Y),
            nShow = WindowShowStyle.ShowNormal,
        };
        _oContextMenu?.InvokeCommand(ref invoke);
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
            if (_oParentFolder == null)
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

                _oParentFolder = (IShellFolder)Marshal.GetObjectForIUnknown(pUnknownParentFolder);
            }
        }
        else
        {
            if (desktopParent || Root)
                _oParentFolder = GetDesktopFolder();
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
                    _oParentFolder = GetDesktopFolder();

                    if (_oParentFolder.EnumObjects(winHandle, SHCONTF.FOLDERS | SHCONTF.INCLUDEHIDDEN, out folderEnumPtr) == (int)HResult.SUCCESS)
                    {
                        folderEnum = (IEnumIDList)Marshal.GetTypedObjectForIUnknown(folderEnumPtr, typeof(IEnumIDList));
                        while (folderEnum.Next(1, out IntPtr pidlSubItem, out uint celtFetched) == (int)HResult.SUCCESS && celtFetched == 1)
                        {
                            Guid guid = typeof(IShellFolder).GUID;
                            if (_oParentFolder.BindToObject(
                                        pidlSubItem,
                                        IntPtr.Zero,
                                        ref guid,
                                        out IntPtr shellFolderPtr) == (int)HResult.SUCCESS)
                            {
                                IntPtr strr = Marshal.AllocCoTaskMem(ShellHelper.MAX_PATH * 2 + 4);
                                Marshal.WriteInt32(strr, 0, 0);
                                StringBuilder buf = new(ShellHelper.MAX_PATH);

                                string txt = "";
                                if (_oParentFolder.GetDisplayNameOf(
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
                                    _oParentFolder = (IShellFolder)Marshal.GetObjectForIUnknown(shellFolderPtr);
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
    }

    #region GetPIDLs()

    /// <summary>
    /// Get the PIDLs
    /// </summary>
    /// <param name="arrFI">Array of FileInfo</param>
    /// <returns>Array of PIDLs</returns>
    protected IntPtr[]? GetPIDLs(FileInfo[] arrFI)
    {
        if (arrFI == null || arrFI.Length == 0)
            return null;

        GetParentFolder(arrFI[0].DirectoryName!, false);
        if (_oParentFolder == null)
            return null;

        IntPtr[] arrPIDLs = new IntPtr[arrFI.Length];
        int n = 0;
        foreach (FileInfo fi in arrFI)
        {
            // Get the file relative to folder
            uint pchEaten = 0;
            SFGAO pdwAttributes = 0;
            int nResult = _oParentFolder.ParseDisplayName(IntPtr.Zero, IntPtr.Zero, fi.Name, ref pchEaten, out IntPtr pPIDL, ref pdwAttributes);
            if (nResult != (int)HResult.SUCCESS)
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
    /// <param name="arrFI">Array of FileInfo</param>
    /// <returns>Array of PIDLs</returns>
    protected IntPtr[]? GetPIDLs(FileSystemInfo[] arrFI, string currentFolder)
    {
        if (arrFI == null || arrFI.Length == 0)
            return null;

        GetParentFolder(currentFolder, false);
        if (_oParentFolder == null)
            return null;

        IntPtr[] arrPIDLs = new IntPtr[arrFI.Length];
        int n = 0;
        foreach (FileSystemInfo fi in arrFI)
        {
            // Get the file relative to folder
            uint pchEaten = 0;
            SFGAO pdwAttributes = 0;
            int nResult = _oParentFolder.ParseDisplayName(IntPtr.Zero, IntPtr.Zero, fi.Name, ref pchEaten, out IntPtr pPIDL, ref pdwAttributes);
            if (nResult != (int)HResult.SUCCESS)
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
    protected IntPtr[]? GetPIDLs(DirectoryInfo[] arrFI)
    {
        if (arrFI == null || arrFI.Length == 0)
            return null;

        if (!Root && arrFI[0].FullName.Length > 3)
            GetParentFolder(arrFI[0].Parent!.FullName, false);
        else
            GetParentFolder(null, arrFI[0].Name == Environment.SpecialFolder.MyComputer.RealName() || arrFI[0].Name == Environment.SpecialFolder.Desktop.RealName());

        if (_oParentFolder == null)
            throw new ExploripCommonException("Parent folder is null");

        IntPtr[] arrPIDLs = new IntPtr[arrFI.Length];
        int n = 0;
        foreach (DirectoryInfo fi in arrFI)
        {
            // Get the file relative to folder
            uint pchEaten = 0;
            SFGAO pdwAttributes = 0;
            int nResult = _oParentFolder.ParseDisplayName(IntPtr.Zero, IntPtr.Zero, fi.Name, ref pchEaten, out IntPtr pPIDL, ref pdwAttributes);
            if (nResult != (int)HResult.SUCCESS && Root)
            {
                nResult = SHGetSpecialFolderLocation(IntPtr.Zero, CSIDL.CSIDL_DRIVES, ref pPIDL);
            }
            if (pPIDL != IntPtr.Zero && nResult == (int)HResult.SUCCESS)
            {
                arrPIDLs[n] = pPIDL;
                n++;
            }
        }

        return arrPIDLs;
    }

    #endregion

    #region ShowContextMenu()

    public void ShowContextMenu(FileSystemInfo[] fsi, string currentFolder, System.Windows.Point pointScreen)
    {
        ReleaseAll();
        _arrPIDLs = GetPIDLs(fsi, currentFolder);
        ShowContextMenu(pointScreen);
    }

    /// <summary>
    /// Shows the context menu
    /// </summary>
    /// <param name="files">FileInfos (should all be in same directory)</param>
    /// <param name="pointScreen">Where to show the menu</param>
    public void ShowContextMenu(FileInfo[] files, System.Windows.Point pointScreen)
    {
        ReleaseAll();
        _arrPIDLs = GetPIDLs(files);
        ShowContextMenu(pointScreen);
    }

    /// <summary>
    /// Shows the context menu
    /// </summary>
    /// <param name="dirs">DirectoryInfos (should all be in same directory)</param>
    /// <param name="pointScreen">Where to show the menu</param>
    public void ShowContextMenu(DirectoryInfo[] dirs, System.Windows.Point pointScreen)
    {
        ReleaseAll();
        _arrPIDLs = GetPIDLs(dirs);
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
            _strBackgroundFolder = dir.FullName;
        }
        else
            _arrPIDLs = GetPIDLs([dir]);
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
                (_arrPIDLs == null || !GetContextMenuInterfaces(_arrPIDLs, out _iContextMenuPtr)))
            {
                ReleaseAll();
                return;
            }

            if (_oContextMenu == null)
                throw new ExploripCommonException("ContextMenu interface not initialized");

            _pMenu = CreatePopupMenu();

            CMF flag = CMF.NORMAL;
            if (!background)
                flag |= CMF.CANRENAME | CMF.EXTENDEDVERBS;

            _oContextMenu.QueryContextMenu(
                _pMenu,
                0,
                CMD_FIRST,
                CMD_LAST,
                flag);

            if (_oContextMenu2 == null)
            {
                Guid guid = typeof(IContextMenu2).GUID;
                Marshal.QueryInterface(_iContextMenuPtr, ref guid, out _iContextMenuPtr2);
                _oContextMenu2 = (IContextMenu2)Marshal.GetTypedObjectForIUnknown(_iContextMenuPtr2, typeof(IContextMenu2));
            }
            if (_oContextMenu3 == null)
            {
                Guid guid = typeof(IContextMenu3).GUID;
                Marshal.QueryInterface(_iContextMenuPtr, ref guid, out _iContextMenuPtr3);
                _oContextMenu3 = (IContextMenu3)Marshal.GetTypedObjectForIUnknown(_iContextMenuPtr3, typeof(IContextMenu3));
            }

            uint cmdPaste = 0, cmdPasteShortcut = 0;
            if (background)
                EnablePaste(_pMenu, out cmdPaste, out cmdPasteShortcut);

            uint nSelected = 0;

            ExpandSubMenu(_pMenu, _oContextMenu2);

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
                fileOp.MoveItem(fs, _strBackgroundFolder, Path.GetFileName(fs));
            else
                fileOp.CopyItem(fs, _strBackgroundFolder, Path.GetFileName(fs));
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
            while (File.Exists(Path.Combine(_strBackgroundFolder, name)))
            {
                name = shortcutLabel.Replace("%s", Path.GetFileNameWithoutExtension(fs)) + $" ({iteration}).lnk";
                iteration++;
            }
            sc = Shortcut.CreateShortcut(fs);
            sc.WriteToFile(Path.Combine(_strBackgroundFolder, name));
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
        catch (Exception ex)
        {
            Console.WriteLine("Erreur " + ex.Message);
        }
        cmd = 0;
        return null;
    }

    #endregion
}
