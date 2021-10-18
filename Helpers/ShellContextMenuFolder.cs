using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Explorip.Helpers
{
    public class ShellContextMenuFolder : NativeWindow
    {
        private IContextMenu2 pContextMenu2 = null;
        private IContextMenu pContextMenu = null;
        private IShellView pShellView;
        private IShellFolder psf;
        private IntPtr pItemIDL;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == (int)WinAPI.Commun.WM.INITMENUPOPUP ||
                m.Msg == (int)WinAPI.Commun.WM.DRAWITEM ||
                m.Msg == (int)WinAPI.Commun.WM.MEASUREITEM)
            {
                if (pContextMenu2 != null)
                {
                    Console.WriteLine($"WndProc Msg={m.Msg}, WParam={m.WParam}, LParam={m.LParam}");
                    pContextMenu2.HandleMenuMsg((uint)m.Msg, (int)m.WParam, m.LParam);
                }

                return;
            }
            else
                base.WndProc(ref m);
        }

        public ShellContextMenuFolder()
        {
            CleanUp();
            CreateHandle(new CreateParams());
        }

        public void ShowContextMenu(string sPath, Point pointScreeen, ContextMenuStrip cms)
        {
            try
            {
                CleanUp();
                pItemIDL = WinAPI.Shell32.ILCreateFromPath(sPath);
                SHGetDesktopFolder(out IShellFolder psfDesktop);
                WinAPI.Commun.HRESULT hr = psfDesktop.BindToObject(pItemIDL, IntPtr.Zero, typeof(IShellFolder).GUID, out object opsf);
                if (hr == WinAPI.Commun.HRESULT.S_OK)
                {
                    psf = (IShellFolder)opsf;
                    hr = psf.CreateViewObject(Handle, typeof(IShellView).GUID, out object opShellView);
                    if (hr == WinAPI.Commun.HRESULT.S_OK)
                    {
                        // Get the background context menu
                        pShellView = (IShellView)opShellView;
                        hr = pShellView.GetItemObject((uint)SVGIO.SVGIO_BACKGROUND, typeof(IContextMenu).GUID, out object opContextMenu);
                        if (hr == WinAPI.Commun.HRESULT.S_OK)
                        {
                            pContextMenu = (IContextMenu)opContextMenu;
                            IntPtr hMenu = WinAPI.User32.CreatePopupMenu();
                            hr = pContextMenu.QueryContextMenu(hMenu, 0, 1, 0x7fff, CMF_EXTENDEDVERBS);
                            if (hr == WinAPI.Commun.HRESULT.S_OK)
                            {
                                pContextMenu2 = (IContextMenu2)pContextMenu;
                                if (cms == null)
                                {
                                    // Execute the chosen menu item
                                    uint nCmd = TrackPopupMenu(hMenu, TPM_LEFTALIGN | TPM_LEFTBUTTON | TPM_RIGHTBUTTON | TPM_RETURNCMD, pointScreeen.X, pointScreeen.Y, 0, Handle, IntPtr.Zero);
                                    if (nCmd != 0)
                                    {
                                        CMINVOKECOMMANDINFO cmi = new CMINVOKECOMMANDINFO
                                        {
                                            cbSize = Marshal.SizeOf(typeof(CMINVOKECOMMANDINFO)),
                                            fMask = 0,
                                            hwnd = Handle,
                                            lpVerb = (IntPtr)(nCmd - 1),
                                            lpParameters = IntPtr.Zero,
                                            lpDirectory = IntPtr.Zero,
                                            nShow = SW_SHOWNORMAL,
                                            dwHotKey = 0,
                                            hIcon = IntPtr.Zero
                                        };
                                        pContextMenu.InvokeCommand(ref cmi);
                                    }
                                    WinAPI.User32.DestroyMenu(hMenu);
                                }
                                else
                                {
                                    cms.Items.Clear();
                                    PopUpMenuExtensions.EtendreSousMenuPopUpFolder(hMenu, pContextMenu2);
                                    PopUpMenuExtensions.CopierVersCms(cms, null, hMenu, menuAAjouter_Click);
                                    cms.Show(pointScreeen);
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void menuAAjouter_Click(object sender, EventArgs e)
        {
            try
            {
                uint nCmd = uint.Parse(((ToolStripMenuItem)sender).Tag.ToString());
                CMINVOKECOMMANDINFO cmi = new CMINVOKECOMMANDINFO
                {
                    cbSize = Marshal.SizeOf(typeof(CMINVOKECOMMANDINFO)),
                    fMask = 0,
                    hwnd = Handle,
                    lpVerb = (IntPtr)(nCmd - 1),
                    lpParameters = IntPtr.Zero,
                    lpDirectory = IntPtr.Zero,
                    nShow = SW_SHOWNORMAL,
                    dwHotKey = 0,
                    hIcon = IntPtr.Zero
                };
                pContextMenu.InvokeCommand(ref cmi);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erreur");
            }
        }

        private void CleanUp()
        {
            if (pContextMenu2 != null)
                Marshal.ReleaseComObject(pContextMenu);

            if (pShellView != null)
                Marshal.ReleaseComObject(pShellView);

            if (psf != null)
                Marshal.ReleaseComObject(psf);

            if (pItemIDL != IntPtr.Zero)
                WinAPI.Shell32.ILFree(pItemIDL);
        }

        [DllImport("Shell32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int SHGetDesktopFolder(out IShellFolder ppshf);

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr CreatePopupMenu();

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern uint TrackPopupMenu(IntPtr hMenu, uint uFlags, int x, int y, int nReserved, IntPtr hWnd, IntPtr prcRect);

        public const int TPM_LEFTBUTTON = 0x0000;
        public const int TPM_RIGHTBUTTON = 0x0002;
        public const int TPM_LEFTALIGN = 0x0000;
        public const int TPM_CENTERALIGN = 0x0004;
        public const int TPM_RIGHTALIGN = 0x0008;
        public const int TPM_TOPALIGN = 0x0000;
        public const int TPM_VCENTERALIGN = 0x0010;
        public const int TPM_BOTTOMALIGN = 0x0020;
        public const int TPM_HORIZONTAL = 0x0000;     /* Horz alignment matters more */
        public const int TPM_VERTICAL = 0x0040;     /* Vert alignment matters more */
        public const int TPM_NONOTIFY = 0x0080;     /* Don't send any notification msgs */
        public const int TPM_RETURNCMD = 0x0100;
        public const int TPM_RECURSE = 0x0001;
        public const int TPM_HORPOSANIMATION = 0x0400;
        public const int TPM_HORNEGANIMATION = 0x0800;
        public const int TPM_VERPOSANIMATION = 0x1000;
        public const int TPM_VERNEGANIMATION = 0x2000;
        public const int TPM_NOANIMATION = 0x4000;
        public const int TPM_LAYOUTRTL = 0x8000;
        public const int TPM_WORKAREA = 0x10000;

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool GetMenuItemInfo(IntPtr hMenu, int uItem, bool fByPosition, [In, Out] ref MENUITEMINFO lpmii);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct MENUITEMINFO
        {
            public uint cbSize;
            public uint fMask;
            public uint fType;         // used if MIIM_TYPE (4.0) or MIIM_FTYPE (>4.0)
            public uint fState;        // used if MIIM_STATE
            public uint wID;           // used if MIIM_ID
            public IntPtr hSubMenu;      // used if MIIM_SUBMENU
            public IntPtr hbmpChecked;   // used if MIIM_CHECKMARKS
            public IntPtr hbmpUnchecked; // used if MIIM_CHECKMARKS
            public IntPtr dwItemData;   // used if MIIM_DATA
#pragma warning disable IDE0044
            [MarshalAs(UnmanagedType.LPWStr)]
            string dwTypeData;    // used if MIIM_TYPE (4.0) or MIIM_STRING (>4.0)
#pragma warning restore IDE0044
            public uint cch;           // used if MIIM_TYPE (4.0) or MIIM_STRING (>4.0)
            public IntPtr hbmpItem;      // used if MIIM_BITMAP
        }

        public const int MIIM_STATE = 0x00000001;
        public const int MIIM_ID = 0x00000002;
        public const int MIIM_SUBMENU = 0x00000004;
        public const int MIIM_CHECKMARKS = 0x00000008;
        public const int MIIM_TYPE = 0x00000010;
        public const int MIIM_DATA = 0x00000020;

        public const int MIIM_STRING = 0x00000040;
        public const int MIIM_BITMAP = 0x00000080;
        public const int MIIM_FTYPE = 0x00000100;

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int GetMenuString(IntPtr hMenu, uint uIDItem, StringBuilder lpString, int cchMax, uint flags);

        public const int MF_BYCOMMAND = 0x00000000;
        public const int MF_BYPOSITION = 0x00000400;

        [DllImport("User32.dll", SetLastError = true)]
        public static extern IntPtr GetSubMenu(IntPtr hMenu, int nPos);

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int GetMenuItemCount(IntPtr hMenu);

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool DestroyMenu(IntPtr hMenu);

        [ComImport]
        [Guid("000214E6-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IShellFolder
        {
            WinAPI.Commun.HRESULT ParseDisplayName(IntPtr hwnd,
                // IBindCtx pbc,
                IntPtr pbc,
                [MarshalAs(UnmanagedType.LPWStr)] string pszDisplayName, [In, Out] ref uint pchEaten, out IntPtr ppidl, [In, Out] ref SFGAO pdwAttributes);
            WinAPI.Commun.HRESULT EnumObjects(IntPtr hwnd, SHCONTF grfFlags, out IEnumIDList ppenumIDList);
            [PreserveSig()]
            WinAPI.Commun.HRESULT BindToObject(IntPtr pidl,
                //IBindCtx pbc,
                IntPtr pbc,
                [In] ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out object ppv);
            WinAPI.Commun.HRESULT BindToStorage(IntPtr pidl, IntPtr pbc, [In] ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out object ppv);
            WinAPI.Commun.HRESULT CompareIDs(IntPtr lParam, IntPtr pidl1, IntPtr pidl2);
            //WinAPI.Commun.HRESULT CreateViewObject(IntPtr hwndOwner, [In] ref Guid riid, out IntPtr ppv);
            WinAPI.Commun.HRESULT CreateViewObject(IntPtr hwndOwner, [In] ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out object ppv);
            WinAPI.Commun.HRESULT GetAttributesOf(uint cidl, IntPtr apidl, [In, Out] ref SFGAO rgfInOut);
            //WinAPI.Commun.HRESULT GetUIObjectOf(IntPtr hwndOwner, uint cidl, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.SysInt, SizeParamIndex = 1)] IntPtr apidl,
            //    [In] ref Guid riid, [In, Out] ref uint rgfReserved, [MarshalAs(UnmanagedType.Interface)] out object ppv);
            WinAPI.Commun.HRESULT GetUIObjectOf(IntPtr hwndOwner, uint cidl, ref IntPtr apidl, [In] ref Guid riid, [In, Out] ref uint rgfReserved, out IntPtr ppv);
            WinAPI.Commun.HRESULT GetDisplayNameOf(IntPtr pidl, SHGDNF uFlags, out STRRET pName);
            WinAPI.Commun.HRESULT SetNameOf(IntPtr hwnd, IntPtr pidl, [MarshalAs(UnmanagedType.LPWStr)] string pszName, SHGDNF uFlags, out IntPtr ppidlOut);
        }

        [ComImport]
        [Guid("000214F2-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IEnumIDList
        {
            [PreserveSig()]
            WinAPI.Commun.HRESULT Next(uint celt, out IntPtr rgelt, out int pceltFetched);
            [PreserveSig()]
            WinAPI.Commun.HRESULT Skip(uint celt);
            void Reset();
            [return: MarshalAs(UnmanagedType.Interface)]
            IEnumIDList Clone();
        }

        [Flags]
        public enum SHCONTF : ushort
        {
            SHCONTF_CHECKING_FOR_CHILDREN = 0x0010,
            SHCONTF_FOLDERS = 0x0020,
            SHCONTF_NONFOLDERS = 0x0040,
            SHCONTF_INCLUDEHIDDEN = 0x0080,
            SHCONTF_INIT_ON_FIRST_NEXT = 0x0100,
            SHCONTF_NETPRINTERSRCH = 0x0200,
            SHCONTF_SHAREABLE = 0x0400,
            SHCONTF_STORAGE = 0x0800,
            SHCONTF_NAVIGATION_ENUM = 0x1000,
            SHCONTF_FASTITEMS = 0x2000,
            SHCONTF_FLATLIST = 0x4000,
            SHCONTF_ENABLE_ASYNC = 0x8000
        }

        [Flags]
        public enum SFGAO : uint
        {
            CANCOPY = 0x00000001,
            CANMOVE = 0x00000002,
            CANLINK = 0x00000004,
            STORAGE = 0x00000008,
            CANRENAME = 0x00000010,
            CANDELETE = 0x00000020,
            HASPROPSHEET = 0x00000040,
            DROPTARGET = 0x00000100,
            CAPABILITYMASK = 0x00000177,
            ENCRYPTED = 0x00002000,
            ISSLOW = 0x00004000,
            GHOSTED = 0x00008000,
            LINK = 0x00010000,
            SHARE = 0x00020000,
            READONLY = 0x00040000,
            HIDDEN = 0x00080000,
            DISPLAYATTRMASK = 0x000FC000,
            STREAM = 0x00400000,
            STORAGEANCESTOR = 0x00800000,
            VALIDATE = 0x01000000,
            REMOVABLE = 0x02000000,
            COMPRESSED = 0x04000000,
            BROWSABLE = 0x08000000,
            FILESYSANCESTOR = 0x10000000,
            FOLDER = 0x20000000,
            FILESYSTEM = 0x40000000,
            HASSUBFOLDER = 0x80000000,
            CONTENTSMASK = 0x80000000,
            STORAGECAPMASK = 0x70C50008,
            PKEYSFGAOMASK = 0x81044000
        }

        public enum SHGDNF
        {
            SHGDN_NORMAL = 0,
            SHGDN_INFOLDER = 0x1,
            SHGDN_FOREDITING = 0x1000,
            SHGDN_FORADDRESSBAR = 0x4000,
            SHGDN_FORPARSING = 0x8000
        }

        [StructLayout(LayoutKind.Explicit, Size = 264)]
        public struct STRRET
        {
            [FieldOffset(0)]
            public uint uType;
            [FieldOffset(4)]
            public IntPtr pOleStr;
            [FieldOffset(4)]
            public uint uOffset;
            [FieldOffset(4)]
            public IntPtr cStr;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
            public RECT(int Left, int Top, int Right, int Bottom)
            {
                left = Left;
                top = Top;
                right = Right;
                bottom = Bottom;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MSG
        {
            public IntPtr hwnd;
            public uint message;
            public int wParam;
            public IntPtr lParam;
            public int time;
            public Point pt;
        }

        [ComImport]
        [Guid("000214E3-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IShellView : IOleWindow
        {
            #region <IOleWindow>
            new WinAPI.Commun.HRESULT GetWindow(out IntPtr phwnd);
            new WinAPI.Commun.HRESULT ContextSensitiveHelp(bool fEnterMode);
            #endregion

            WinAPI.Commun.HRESULT TranslateAccelerator(MSG pmsg);
            WinAPI.Commun.HRESULT EnableModeless(bool fEnable);
            WinAPI.Commun.HRESULT UIActivate(uint uState);
            WinAPI.Commun.HRESULT Refresh();
            WinAPI.Commun.HRESULT CreateViewWindow(IShellView psvPrevious, FOLDERSETTINGS pfs, IShellBrowser psb, RECT prcView, out IntPtr pIntPtr);
            WinAPI.Commun.HRESULT DestroyViewWindow();
            WinAPI.Commun.HRESULT GetCurrentInfo(out FOLDERSETTINGS pfs);
            //WinAPI.Commun.HRESULT AddPropertySheetPages(int dwReserved, LPFNSVADDPROPSHEETPAGE pfn, IntPtr lparam);
            WinAPI.Commun.HRESULT AddPropertySheetPages(int dwReserved, IntPtr pfn, IntPtr lparam);
            WinAPI.Commun.HRESULT SaveViewState();
            WinAPI.Commun.HRESULT SelectItem(IntPtr pidlItem, SVSIF uFlags);
            WinAPI.Commun.HRESULT GetItemObject(uint uItem, ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out object ppv);
        };

        public enum SVGIO
        {
            SVGIO_BACKGROUND = 0,
            SVGIO_SELECTION = 0x1,
            SVGIO_ALLVIEW = 0x2,
            SVGIO_CHECKED = 0x3,
            SVGIO_TYPE_MASK = 0xf,
            SVGIO_FLAG_VIEWORDER = unchecked((int)0x80000000)
        }

        [ComImport]
        [Guid("00000114-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IOleWindow
        {
            WinAPI.Commun.HRESULT GetWindow(out IntPtr phwnd);
            WinAPI.Commun.HRESULT ContextSensitiveHelp(bool fEnterMode);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FOLDERSETTINGS
        {
            public uint ViewMode;
            public uint fFlags;
        }

        public enum SVSIF
        {
            SVSI_DESELECT = 0,
            SVSI_SELECT = 0x1,
            SVSI_EDIT = 0x3,
            SVSI_DESELECTOTHERS = 0x4,
            SVSI_ENSUREVISIBLE = 0x8,
            SVSI_FOCUSED = 0x10,
            SVSI_TRANSLATEPT = 0x20,
            SVSI_SELECTIONMARK = 0x40,
            SVSI_POSITIONITEM = 0x80,
            SVSI_CHECK = 0x100,
            SVSI_CHECK2 = 0x200,
            SVSI_KEYBOARDSELECT = 0x401,
            SVSI_NOTAKEFOCUS = 0x40000000
        }

        [ComImport]
        [Guid("000214E2-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IShellBrowser : IOleWindow
        {
            #region <IOleWindow>
            new WinAPI.Commun.HRESULT GetWindow(out IntPtr phwnd);
            new WinAPI.Commun.HRESULT ContextSensitiveHelp(bool fEnterMode);
            #endregion
            WinAPI.Commun.HRESULT InsertMenusSB(IntPtr IntPtrShared, ref OLEMENUGROUPWIDTHS lpMenuWidths);
            WinAPI.Commun.HRESULT SetMenuSB(IntPtr IntPtrShared, IntPtr holemenuRes, IntPtr IntPtrActiveObject);
            WinAPI.Commun.HRESULT RemoveMenusSB(IntPtr IntPtrShared);
            WinAPI.Commun.HRESULT SetStatusTextSB(string pszStatusText);
            WinAPI.Commun.HRESULT EnableModelessSB(bool fEnable);
            WinAPI.Commun.HRESULT TranslateAcceleratorSB(MSG pmsg, UInt16 wID);
            WinAPI.Commun.HRESULT BrowseObject(IntPtr pidl, uint wFlags);
            WinAPI.Commun.HRESULT GetViewStateStream(uint grfMode, out System.Runtime.InteropServices.ComTypes.IStream ppStrm);
            WinAPI.Commun.HRESULT GetControlWindow(uint id, out IntPtr pIntPtr);
            WinAPI.Commun.HRESULT SendControlMsg(uint id, uint uMsg, int wParam, IntPtr lParam, out IntPtr pret);
            //WinAPI.Commun.HRESULT QueryActiveShellView([Out, MarshalAs(UnmanagedType.Interface)] IShellView ppshv);
            WinAPI.Commun.HRESULT QueryActiveShellView(out IShellView ppshv);
            WinAPI.Commun.HRESULT OnViewWindowActive(IShellView pshv);
            WinAPI.Commun.HRESULT SetToolbarItems(TBBUTTON lpButtons, uint nButtons, uint uFlags);
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct OLEMENUGROUPWIDTHS
        {
            [MarshalAs(UnmanagedType.U2, SizeConst = 6)]
            public int[] width;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct TBBUTTON
        {
            public int iBitmap;
            public int idCommand;
            public byte fsState;
            public byte fsStyle;
            public byte bReserved0;
            public byte bReserved1;
            public int dwData;
            public IntPtr iString;
        }

        [ComImport]
        [Guid("000214e4-0000-0000-c000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IContextMenu
        {
            WinAPI.Commun.HRESULT QueryContextMenu(IntPtr hmenu, uint indexMenu, uint idCmdFirst, uint idCmdLast, uint uFlags);
            [PreserveSig()]
            WinAPI.Commun.HRESULT InvokeCommand(ref CMINVOKECOMMANDINFO pici);

            [PreserveSig()]
            WinAPI.Commun.HRESULT GetCommandString(uint idCmd, uint uType, IntPtr pReserved, StringBuilder pszName, uint cchMax);
        }

        [ComImport]
        [Guid("000214f4-0000-0000-c000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IContextMenu2 : IContextMenu
        {
            new WinAPI.Commun.HRESULT QueryContextMenu(IntPtr hmenu, uint indexMenu, uint idCmdFirst, uint idCmdLast, uint uFlags);
            [PreserveSig()]
            new WinAPI.Commun.HRESULT InvokeCommand(ref CMINVOKECOMMANDINFO pici);

            [PreserveSig()]
            new WinAPI.Commun.HRESULT GetCommandString(uint idCmd, uint uType, IntPtr pReserved, StringBuilder pszName, uint cchMax);

            [PreserveSig()]
            WinAPI.Commun.HRESULT HandleMenuMsg(uint uMsg, int wParam, IntPtr lParam);
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct CMINVOKECOMMANDINFO
        {
            public int cbSize;
            public int fMask;
            public IntPtr hwnd;
            public IntPtr lpVerb;
            public IntPtr lpParameters;
            public IntPtr lpDirectory;
            public int nShow;
            public int dwHotKey;
            public IntPtr hIcon;
        }

        public const int CMF_NORMAL = 0x00000000;
        public const int CMF_DEFAULTONLY = 0x00000001;
        public const int CMF_VERBSONLY = 0x00000002;
        public const int CMF_EXPLORE = 0x00000004;
        public const int CMF_NOVERBS = 0x00000008;
        public const int CMF_CANRENAME = 0x00000010;
        public const int CMF_NODEFAULT = 0x00000020;
        public const int CMF_INCLUDESTATIC = 0x00000040;
        public const int CMF_ITEMMENU = 0x00000080;
        public const int CMF_EXTENDEDVERBS = 0x00000100;
        public const int CMF_DISABLEDVERBS = 0x00000200;
        public const int CMF_ASYNCVERBSTATE = 0x00000400;
        public const int CMF_OPTIMIZEFORINVOKE = 0x00000800;
        public const int CMF_SYNCCASCADEMENU = 0x00001000;
        public const int CMF_DONOTPICKDEFAULT = 0x00002000;
        public const int CMF_RESERVED = unchecked((int)0xffff0000);

        public const int SW_SHOWNORMAL = 1;

        public const int GCS_VERBA = 0x00000000;     // canonical verb
        public const int GCS_HELPTEXTA = 0x00000001;     // help text (for status bar)
        public const int GCS_VALIDATEA = 0x00000002;     // validate command exists
        public const int GCS_VERBW = 0x00000004;     // canonical verb (unicode)
        public const int GCS_HELPTEXTW = 0x00000005;     // help text (unicode version)
        public const int GCS_VALIDATEW = 0x00000006;     // validate command exists (unicode)
        public const int GCS_VERBICONW = 0x00000014;     // icon string (unicode)
        public const int GCS_UNICODE = 0x00000004;     // for bit testing - Unicode string
    }
}
