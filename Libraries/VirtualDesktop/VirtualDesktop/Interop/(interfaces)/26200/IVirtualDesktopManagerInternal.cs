using System;
using System.Runtime.InteropServices;

namespace WindowsDesktop.Interop
{
    [ComImport()]
    [Guid("00000000-0000-0000-0000-000000000000") /* replace at runtime */]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IVirtualDesktopManagerInternal
    {
        int GetCount();

        void MoveViewToDesktop(IApplicationView pView, IVirtualDesktop pDesktop);

        [return: MarshalAs(UnmanagedType.Bool)]
        bool CanViewMoveDesktops(IApplicationView pView);

        IVirtualDesktop GetCurrentDesktop();

        void GetDesktops(out IObjectArray hWndOrMon);

        [PreserveSig()]
        int GetAdjacentDesktop(IVirtualDesktop pDesktopReference, int uDirection, out IVirtualDesktop pAdjacentDesktop);

        void SwitchDesktop(IVirtualDesktop pDesktop);

        IVirtualDesktop CreateDesktop();

        void MoveDesktop(IVirtualDesktop pDesktopReference, int nIndex);

        void RemoveDesktop(IVirtualDesktop pRemove, IVirtualDesktop pFallbackDesktop);

        IVirtualDesktop FindDesktop(ref Guid desktopId);

        void GetDesktopSwitchIncludeExcludeViews(IVirtualDesktop desktop, out IObjectArray unknown1, out IObjectArray unknown2);//15

        void SetDesktopName(IVirtualDesktop pDesktop, [MarshalAs(UnmanagedType.HString)] string chName);

        void SetDesktopWallpaper(IVirtualDesktop pDesktop, [MarshalAs(UnmanagedType.HString)] string chPath);

        void UpdateWallpaperPathForAllDesktops([MarshalAs(UnmanagedType.HString)] string wallpaper);//18

        void CopyDesktopState(IApplicationView pView0, IApplicationView pView1);//19

        void CreateRemoteDesktop([MarshalAs(UnmanagedType.HString)] string path, out IVirtualDesktop desktop);

        void SwitchRemoteDesktop(IVirtualDesktop desktop);

        void SwitchDesktopWithAnimation(IVirtualDesktop desktop);

        void GetLastActiveDesktop(out IVirtualDesktop desktop);

        void WaitForAnimationToComplete();
    }
}
