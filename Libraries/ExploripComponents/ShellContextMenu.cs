using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input;

using ManagedShell.Common.Helpers;
using ManagedShell.ShellFolders.Enums;
using ManagedShell.ShellFolders.Interfaces;
using ManagedShell.ShellFolders.Structs;

using Microsoft.WindowsAPICodePack.Shell.KnownFolders;

using Securify.ShellLink;

using static ManagedShell.Interop.NativeMethods;

namespace ExploripComponents;

public class ShellContextMenu
{
    #region Champs

    private IntPtr pMenu, iContextMenuPtr, iContextMenuPtr2, iContextMenuPtr3;
    private IContextMenu? _oContextMenu;
    private IContextMenu2? _oContextMenu2;
    private IContextMenu3? _oContextMenu3;
    private static IShellFolder? _oDesktopFolder;
    private IShellFolder? _oParentFolder;
    private nint[]? _arrPIDLs;
    private string? _strParentFolder;
    private const uint CMD_FIRST = 0;
    private const uint CMD_LAST = (uint)short.MaxValue;
    private string? _strBackgroundFolder;
    private static readonly int cbInvokeCommand = Marshal.SizeOf(typeof(CmInvokeCommandInfoEx));

    #endregion

    #region Constructor

    /// <summary>Default constructor</summary>
    public ShellContextMenu()
    {
        pMenu = IntPtr.Zero;
        iContextMenuPtr = IntPtr.Zero;
        iContextMenuPtr2 = IntPtr.Zero;
        iContextMenuPtr3 = IntPtr.Zero;
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

    #endregion

    #region GetContextMenuInterfaces()

    /// <summary>Gets the interfaces to the context menu</summary>
    /// <param name="oParentFolder">Parent folder</param>
    /// <param name="arrPIDLs">PIDLs</param>
    /// <returns>true if it got the interfaces, otherwise false</returns>
    private bool GetContextMenuInterfaces(nint[] arrPIDLs, out nint ctxMenuPtr)
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

    #endregion

    #region InvokeCommand

    private void InvokeCommand(IContextMenu oContextMenu, uint nCmd, System.Windows.Point pointInvoke)
    {
        CmInvokeCommandInfoEx invoke = new()
        {
            cbSize = cbInvokeCommand,
            lpVerb = (nint)(nCmd - CMD_FIRST),
            lpDirectory = _strParentFolder,
            lpVerbW = (nint)(nCmd - CMD_FIRST),
            lpDirectoryW = _strParentFolder,
            fMask = CMIC.UNICODE | CMIC.PTINVOKE |
            (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl) ? CMIC.CONTROL_DOWN : 0) |
            (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift) ? CMIC.SHIFT_DOWN : 0),
            ptInvoke = new Point((long)pointInvoke.X, (long)pointInvoke.Y),
            nShow = WindowShowStyle.ShowNormal,
        };
        oContextMenu.InvokeCommand(ref invoke);
    }

    #endregion

    #region ReleaseAll()

    private void CleanUp()
    {
        if (pMenu != IntPtr.Zero)
        {
            DestroyMenu(pMenu);
            pMenu = IntPtr.Zero;
        }

        if (iContextMenuPtr != IntPtr.Zero)
        {
            Marshal.Release(iContextMenuPtr);
            iContextMenuPtr = IntPtr.Zero;
        }

        if (iContextMenuPtr2 != IntPtr.Zero)
        {
            Marshal.Release(iContextMenuPtr2);
            iContextMenuPtr2 = IntPtr.Zero;
        }

        if (iContextMenuPtr3 != IntPtr.Zero)
        {
            Marshal.Release(iContextMenuPtr3);
            iContextMenuPtr3 = IntPtr.Zero;
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
    }

    #endregion

    #region GetDesktopFolder()

    /// <summary>
    /// Gets the desktop folder
    /// </summary>
    /// <returns>IShellFolder for desktop folder</returns>
    public static IShellFolder GetDesktopFolder()
    {
        if (_oDesktopFolder == null)
        {
            // Get desktop IShellFolder
            int nResult = SHGetDesktopFolder(out nint pUnkownDesktopFolder);
            if (nResult != (int)HResult.SUCCESS)
                throw new ExploripCommonException("Failed to get the desktop shell folder");

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
    private void GetParentFolder(string? folderName, bool desktopParent)
    {
        if (!string.IsNullOrWhiteSpace(folderName) && !Root)
        {
            if (_oParentFolder == null)
            {
                GetDesktopFolder();
                if (_oDesktopFolder == null)
                    return;

                uint pchEaten = 0;
                SFGAO pdwAttributes = 0;

                // Get the PIDL for the folder file is in
                int nResult = _oDesktopFolder.ParseDisplayName(IntPtr.Zero, IntPtr.Zero, folderName, ref pchEaten, out nint pPIDL, ref pdwAttributes);
                if (nResult != (int)HResult.SUCCESS)
                    return;

                nint pStrRet = Marshal.AllocCoTaskMem(ShellHelper.MAX_PATH * 2 + 4);
                Marshal.WriteInt32(pStrRet, 0, 0);
                _ = _oDesktopFolder.GetDisplayNameOf(pPIDL, SHGDN.FORPARSING, pStrRet);
                StringBuilder strFolder = new(ShellHelper.MAX_PATH);
                _ = StrRetToBuf(pStrRet, pPIDL, strFolder, ShellHelper.MAX_PATH);
                Marshal.FreeCoTaskMem(pStrRet);
                _strParentFolder = strFolder.ToString();

                // Get the IShellFolder for folder
                Guid guid = typeof(IShellFolder).GUID;
                nResult = _oDesktopFolder.BindToObject(pPIDL, IntPtr.Zero, ref guid, out nint pUnknownParentFolder);

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
                nint folderEnumPtr = IntPtr.Zero;
                IEnumIDList? folderEnum = null;
                nint winHandle = IntPtr.Zero;

                nint tempPidl;
                ShFileInfo info;

                info = new ShFileInfo();
                tempPidl = IntPtr.Zero;
                _ = SHGetSpecialFolderLocation(IntPtr.Zero, CSIDL.CSIDL_DRIVES, ref tempPidl);

                SHGetFileInfo(tempPidl, 0, ref info, (uint)Marshal.SizeOf(info), SHGFI.PIDL | SHGFI.DisplayName | SHGFI.TypeName);

                string mycompName = info.szDisplayName;

                try
                {
                    _oParentFolder = GetDesktopFolder();

                    if (_oParentFolder.EnumObjects(winHandle, SHCONTF.FOLDERS | SHCONTF.INCLUDEHIDDEN, out folderEnumPtr) == (int)HResult.SUCCESS)
                    {
                        folderEnum = (IEnumIDList)Marshal.GetTypedObjectForIUnknown(folderEnumPtr, typeof(IEnumIDList));
                        while (folderEnum.Next(1, out nint pidlSubItem, out uint celtFetched) == (int)HResult.SUCCESS && celtFetched == 1)
                        {
                            Guid guid = typeof(IShellFolder).GUID;
                            if (_oParentFolder.BindToObject(
                                        pidlSubItem,
                                        IntPtr.Zero,
                                        ref guid,
                                        out nint shellFolderPtr) == (int)HResult.SUCCESS)
                            {
                                nint strr = Marshal.AllocCoTaskMem(ShellHelper.MAX_PATH * 2 + 4);
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

    #endregion

    #region GetPIDLs()

    /// <summary>
    /// Get the PIDLs
    /// </summary>
    /// <param name="arrFI">Array of FileInfo</param>
    /// <returns>Array of PIDLs</returns>
    protected nint[]? GetPIDLs(FileInfo[] arrFI)
    {
        if (arrFI == null || arrFI.Length == 0)
            return null;

        GetParentFolder(arrFI[0].DirectoryName!, false);
        if (_oParentFolder == null)
            return null;

        nint[] arrPIDLs = new nint[arrFI.Length];
        int n = 0;
        foreach (FileInfo fi in arrFI)
        {
            // Get the file relative to folder
            uint pchEaten = 0;
            SFGAO pdwAttributes = 0;
            int nResult = _oParentFolder.ParseDisplayName(IntPtr.Zero, IntPtr.Zero, fi.Name, ref pchEaten, out nint pPIDL, ref pdwAttributes);
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
    protected nint[]? GetPIDLs(FileSystemInfo[] arrFI, string currentFolder)
    {
        if (arrFI == null || arrFI.Length == 0)
            return null;

        GetParentFolder(currentFolder, false);
        if (_oParentFolder == null)
            return null;

        nint[] arrPIDLs = new nint[arrFI.Length];
        int n = 0;
        foreach (FileSystemInfo fi in arrFI)
        {
            // Get the file relative to folder
            uint pchEaten = 0;
            SFGAO pdwAttributes = 0;
            int nResult = _oParentFolder.ParseDisplayName(IntPtr.Zero, IntPtr.Zero, fi.Name, ref pchEaten, out nint pPIDL, ref pdwAttributes);
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
    protected nint[]? GetPIDLs(DirectoryInfo[] arrFI, bool background = false)
    {
        if (arrFI == null || arrFI.Length == 0)
            return null;

        if (background && !Root)
        {
            nint pItemIDL = ILCreateFromPath(arrFI[0].FullName);
            GetDesktopFolder().BindToObject(pItemIDL, IntPtr.Zero, typeof(IShellFolder).GUID, out nint opsf);
            IShellFolder psf = (IShellFolder)Marshal.GetObjectForIUnknown(opsf);
            psf.CreateViewObject(IntPtr.Zero, typeof(IShellView).GUID, out nint opShellView);
            IShellView pShellView = (IShellView)Marshal.GetObjectForIUnknown(opShellView);
            pShellView.GetItemObject((uint)ShellViewGetItemObject.Background, typeof(IContextMenu).GUID, out object opContextMenu);
            _oContextMenu = (IContextMenu)opContextMenu;
        }

        if (arrFI[0].FullName.Length > 3 && !Root)
            GetParentFolder(arrFI[0].Parent!.FullName, false);
        else
            GetParentFolder(null, arrFI[0].Name == KnownFolders.Computer.CanonicalName || arrFI[0].Name == "Desktop");

        if (_oParentFolder == null)
            throw new ExploripCommonException("Parent folder is null");

        nint[] arrPIDLs = new nint[arrFI.Length];
        int n = 0;
        foreach (DirectoryInfo fi in arrFI)
        {
            // Get the file relative to folder
            uint pchEaten = 0;
            SFGAO pdwAttributes = 0;
            int nResult = _oParentFolder.ParseDisplayName(IntPtr.Zero, IntPtr.Zero, fi.Name, ref pchEaten, out nint pPIDL, ref pdwAttributes);
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

    #region FreePIDLs()

    /// <summary>
    /// Free the PIDLs
    /// </summary>
    /// <param name="arrPIDLs">Array of PIDLs (IntPtr)</param>
    protected static void FreePIDLs(nint[] arrPIDLs)
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
        _arrPIDLs = GetPIDLs([dir], background);
        if (background)
            _strBackgroundFolder = dir.FullName;
        ShowContextMenu(pointScreen, background);
    }

    /// <summary>
    /// Shows the context menu
    /// </summary>
    /// <param name="arrFI">FileInfos (should all be in same directory)</param>
    /// <param name="pointScreen">Where to show the menu</param>
    private void ShowContextMenu(System.Windows.Point pointScreen, bool background = false)
    {
        try
        {
            if (_arrPIDLs == null)
            {
                ReleaseAll();
                return;
            }

            if (!background && !GetContextMenuInterfaces(_arrPIDLs, out iContextMenuPtr))
            {
                ReleaseAll();
                return;
            }

            pMenu = CreatePopupMenu();

            if (_oContextMenu == null)
                throw new ExploripCommonException("ContextMenu interface not initialized");

            CMF flag = CMF.NORMAL;
            if (!background)
                flag |= CMF.CANRENAME | CMF.EXTENDEDVERBS;

            _oContextMenu.QueryContextMenu(
                pMenu,
                0,
                CMD_FIRST,
                CMD_LAST,
                flag);

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

            uint cmdPaste = 0, cmdPasteShortcut = 0;
            if (background)
                EnablePaste(pMenu, out cmdPaste, out cmdPasteShortcut);

            uint nSelected = 0;

            nSelected = TrackPopupMenuEx(
                pMenu,
                TPM.RETURNCMD | TPM.RIGHTBUTTON,
                (int)pointScreen.X,
                (int)pointScreen.Y,
                ((MainWindow)System.Windows.Application.Current.Windows[0]).MyDataContext.WindowHandle,
                IntPtr.Zero);

            DestroyMenu(pMenu);
            pMenu = IntPtr.Zero;

            if (nSelected != 0)
            {
                if (background)
                {
                    if (nSelected == cmdPaste)
                        PasteClipboard();
                    else if (nSelected == cmdPasteShortcut)
                        PasteShortcutClipboard();
                    else
                        InvokeCommand(_oContextMenu, nSelected, pointScreen);
                }
                else
                    InvokeCommand(_oContextMenu, nSelected, pointScreen);
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
        string shortcutLabel = Load("shell32.dll", 4154, "%s - Shortcut ().lnk").Replace(" ().lnk", "");
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

    private void EnablePaste(nint pMenu, out uint cmdPaste, out uint cmdPasteShortcut)
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
                        bool bPaste = libelle!.Trim().ToLower().Replace("&", "") == Load("shell32.dll", 33562, "Paste").Trim().ToLower();
                        if (!string.IsNullOrWhiteSpace(libelle) &&
                            (bPaste ||
                             libelle.Trim().ToLower().Replace("&", "") == Load("shell32.dll", 37376, "Paste shortcut").Trim().ToLower()) &&
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

    public static string Load(string libraryName, uint Ident, string DefaultText)
    {
        IntPtr libraryHandle = GetModuleHandle(libraryName);
        if (libraryHandle != IntPtr.Zero)
        {
            StringBuilder sb = new(1024);
            int size = LoadString(libraryHandle, Ident, sb, 1024);
            if (size > 0)
                return sb.ToString();
            else
                return DefaultText;
        }
        else
            return DefaultText;
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

    public bool Root { get; set; } = false;
}
