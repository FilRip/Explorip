using System;
using System.Runtime.InteropServices;

using ManagedShell.Interop;

namespace Explorip.Helpers.TaskbarList.Interfaces;

[ComImport()]
[Guid("C43DC798-95D1-4BEA-9030-BB99E2983A1A")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface ITaskbarList4 : ITaskbarList3
{
    void SetTabProperties(IntPtr hwndTab, NativeMethods.SourceThumbnailProperties flags);
    void SetTabThumbnailClip(IntPtr hwndTab, ref NativeMethods.Rect clip);
}
