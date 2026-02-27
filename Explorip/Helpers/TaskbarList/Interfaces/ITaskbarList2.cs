using System;
using System.Runtime.InteropServices;

namespace Explorip.Helpers.TaskbarList.Interfaces;

[ComImport()]
[Guid("602D4995-B13A-429b-A66E-1935E44F4317")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface ITaskbarList2 : ITaskbarList
{
    void MarkFullscreenWindow(IntPtr hwnd, bool fullscreen);
}
