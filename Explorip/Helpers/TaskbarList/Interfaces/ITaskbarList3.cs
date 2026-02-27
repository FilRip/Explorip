using System;
using System.Runtime.InteropServices;

using ManagedShell.Interop;

namespace Explorip.Helpers.TaskbarList.Interfaces;

[ComImport()]
[Guid("EA1AFB91-9E28-4B86-90E9-9E9F8A5EEA84")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface ITaskbarList3 : ITaskbarList2
{
    void SetProgressValue(IntPtr hwnd, ulong completed, ulong total);
    void SetProgressState(IntPtr hwnd, int state);
    void RegisterTab(IntPtr hwndTab, IntPtr hwndMDI);
    void UnregisterTab(IntPtr hwndTab);
    void SetTabOrder(IntPtr hwndTab, IntPtr hwndInsertBefore);
    void SetTabActive(IntPtr hwndTab, IntPtr hwndMDI, int flags);
    void ThumbBarAddButtons(IntPtr hwnd, uint count, IntPtr buttons);
    void ThumbBarUpdateButtons(IntPtr hwnd, uint count, IntPtr buttons);
    void ThumbBarSetImageList(IntPtr hwnd, IntPtr himl);
    void SetOverlayIcon(IntPtr hwnd, IntPtr hIcon, string description);
    void SetThumbnailTooltip(IntPtr hwnd, string tooltip);
    void SetThumbnailClip(IntPtr hwnd, ref NativeMethods.Rect clip);
}
