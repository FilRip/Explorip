using System;
using System.Runtime.InteropServices;

namespace ManagedShell.Common.Interfaces;

[ComImport(), Guid("37c994e7-432b-4834-a2f7-dce1f13b834b")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface ITipInvocation
{
    void Toggle(IntPtr hwnd);
}
