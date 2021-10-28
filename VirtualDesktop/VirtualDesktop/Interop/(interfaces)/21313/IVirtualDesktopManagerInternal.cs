using System;
using System.Runtime.InteropServices;

namespace WindowsDesktop.Interop
{
	[ComImport]
	[Guid("00000000-0000-0000-0000-000000000000") /* replace at runtime */]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IVirtualDesktopManagerInternal
	{
		int GetCount(IntPtr hWndOrMon);

		void MoveViewToDesktop(IApplicationView pView, IVirtualDesktop pDesktop);

		[return: MarshalAs(UnmanagedType.Bool)]
		bool CanViewMoveDesktops(IApplicationView pView);

		IVirtualDesktop GetCurrentDesktop(IntPtr hWndOrMon);

		IObjectArray GetDesktops(IntPtr hWndOrMon);

		IVirtualDesktop GetAdjacentDesktop(IVirtualDesktop pDesktopReference, AdjacentDesktop uDirection);

		void SwitchDesktop(IntPtr hWndOrMon, IVirtualDesktop pDesktop);

		IVirtualDesktop CreateDesktopW(IntPtr hWndOrMon);

		IVirtualDesktop MoveDesktop(IVirtualDesktop pDesktopReference, IntPtr hWndOrMon, int nIndex);

		void RemoveDesktop(IVirtualDesktop pRemove, IVirtualDesktop pFallbackDesktop);

		IVirtualDesktop FindDesktop([In, MarshalAs(UnmanagedType.LPStruct)] Guid desktopId);

		void GetDesktopSwitchIncludeExcludeViews(IVirtualDesktop pDesktop, out IObjectArray pUnknown1, out IObjectArray pUnknown2);

		void SetDesktopName(IVirtualDesktop pDesktop, [MarshalAs(UnmanagedType.HString)] string chName);

		void SetDesktopWallpaper(IVirtualDesktop pDesktop, [MarshalAs(UnmanagedType.HString)] string chPath);

		void UpdateWallpaperPathForAllDesktops([MarshalAs(UnmanagedType.HString)] string chText);

		void CopyDesktopState(IApplicationView pUnknown0, IApplicationView pUnknown1);

		int GetDesktopIsPerMonitor();
	}
}
