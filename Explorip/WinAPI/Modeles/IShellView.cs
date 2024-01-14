using System;
using System.Runtime.InteropServices;

namespace Explorip.WinAPI.Modeles;

public enum SVGIO : uint
{
    SVGIO_BACKGROUND = 0,
    SVGIO_SELECTION = 0x1,
    SVGIO_ALLVIEW = 0x2,
    SVGIO_CHECKED = 0x3,
    SVGIO_TYPE_MASK = 0xf,
    SVGIO_FLAG_VIEWORDER = unchecked(0x80000000)
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

[ComImport()]
[Guid("000214E3-0000-0000-C000-000000000046")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IShellView : IOleWindow
{
    #region <IOleWindow>
    new Int32 GetWindow(out IntPtr phwnd);
    new Int32 ContextSensitiveHelp(bool fEnterMode);
    #endregion

    Int32 TranslateAccelerator(object pmsg);
    Int32 EnableModeless(bool fEnable);
    Int32 UIActivate(uint uState);
    Int32 Refresh();
    Int32 CreateViewWindow(IShellView psvPrevious, object pfs, object psb, object prcView, out IntPtr pIntPtr);
    Int32 DestroyViewWindow();
    Int32 GetCurrentInfo(out object pfs);
    Int32 AddPropertySheetPages(int dwReserved, IntPtr pfn, IntPtr lparam);
    Int32 SaveViewState();
    Int32 SelectItem(IntPtr pidlItem, SVSIF uFlags);
    Int32 GetItemObject(SVGIO uItem, ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out object ppv);
};
