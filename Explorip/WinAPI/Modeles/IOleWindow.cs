using System;
using System.Runtime.InteropServices;

namespace Explorip.WinAPI.Modeles;

[ComImport()]
[Guid("00000114-0000-0000-C000-000000000046")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IOleWindow
{
    Int32 GetWindow(out IntPtr phwnd);
    Int32 ContextSensitiveHelp(bool fEnterMode);
}
