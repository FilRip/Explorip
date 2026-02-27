using System;
using System.Runtime.InteropServices;

using Explorip.Helpers.TaskbarList.Interfaces;

using ManagedShell.Interop;

namespace Explorip.Helpers.TaskbarList;

[ComVisible(true)]
[Guid("56FDF344-FD6D-11d0-958A-006097C9A090")]
[ClassInterface(ClassInterfaceType.None)]
public class TaskbarList : ITaskbarList4
{
    public void HrInit()
    {
    }

    public void AddTab(IntPtr hwnd)
    {
    }

    public void DeleteTab(IntPtr hwnd)
    {
    }

    public void ActivateTab(IntPtr hwnd)
    {
    }

    public void SetActiveAlt(IntPtr hwnd)
    {
    }

    public void MarkFullscreenWindow(IntPtr hwnd, bool fullscreen)
    {
    }

    public void SetProgressValue(IntPtr hwnd, ulong completed, ulong total)
    {
    }

    public void SetProgressState(IntPtr hwnd, int state)
    {
    }

    public void RegisterTab(IntPtr hwndTab, IntPtr hwndMDI)
    {
    }

    public void UnregisterTab(IntPtr hwndTab)
    {
    }

    public void SetTabOrder(IntPtr hwndTab, IntPtr hwndInsertBefore)
    {
    }

    public void SetTabActive(IntPtr hwndTab, IntPtr hwndMDI, int flags)
    {
    }

    public void ThumbBarAddButtons(IntPtr hwnd, uint count, IntPtr buttons)
    {
    }

    public void ThumbBarUpdateButtons(IntPtr hwnd, uint count, IntPtr buttons)
    {
    }

    public void ThumbBarSetImageList(IntPtr hwnd, IntPtr himl)
    {
    }

    public void SetOverlayIcon(IntPtr hwnd, IntPtr hIcon, string description)
    {
    }

    public void SetThumbnailTooltip(IntPtr hwnd, string tooltip)
    {
    }

    public void SetThumbnailClip(IntPtr hwnd, ref NativeMethods.Rect clip)
    {
    }

    public void SetTabProperties(IntPtr hwndTab, NativeMethods.SourceThumbnailProperties flags)
    {
    }

    public void SetTabThumbnailClip(IntPtr hwndTab, ref NativeMethods.Rect clip)
    {
    }
}
