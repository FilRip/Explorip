using System;
using System.Runtime.InteropServices;

namespace Microsoft.WindowsAPICodePack.Shell.Interop.Common;

internal static class IntPtrExtensions
{
    public static T MarshalAs<T>(this IntPtr ptr)
    {
        return (T)Marshal.PtrToStructure(ptr, typeof(T));
    }
}
