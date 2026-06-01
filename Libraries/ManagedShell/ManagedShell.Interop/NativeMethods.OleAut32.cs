using System;
using System.Runtime.InteropServices;

namespace ManagedShell.Interop;

public partial class NativeMethods
{
    private const string OleAut32_DllName = "oleaut32.dll";

    [DllImport(OleAut32_DllName)]
    internal static extern void GetActiveObject(ref Guid guid, IntPtr reserved, [MarshalAs(UnmanagedType.IUnknown)] out object ppObject);
}
