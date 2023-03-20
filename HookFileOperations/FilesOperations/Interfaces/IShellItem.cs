using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Explorip.HookFileOperations.FilesOperations.Interfaces
{
    public enum SIGDN : uint
    {
        SIGDN_NORMALDISPLAY = 0x00000000,
        SIGDN_PARENTRELATIVEPARSING = 0x80018001,
        SIGDN_PARENTRELATIVEFORADDRESSBAR = 0x8001c001,
        SIGDN_DESKTOPABSOLUTEPARSING = 0x80028000,
        SIGDN_PARENTRELATIVEEDITING = 0x80031001,
        SIGDN_DESKTOPABSOLUTEEDITING = 0x8004c000,
        SIGDN_FILESYSPATH = 0x80058000,
        SIGDN_URL = 0x80068000
    }

    [ComImport()]
    [Guid("43826d1e-e718-42ee-bc55-a1e261c37bfe")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IShellItem
    {
        [return: MarshalAs(UnmanagedType.Interface)]
        object BindToHandler(IBindCtx pbc, ref Guid bhid, ref Guid riid);

        IShellItem GetParent();

        [return: MarshalAs(UnmanagedType.LPWStr)]
        string GetDisplayName(SIGDN sigdnName);

        uint GetAttributes(uint sfgaoMask);

        int Compare(IShellItem psi, uint hint);
    }
}
