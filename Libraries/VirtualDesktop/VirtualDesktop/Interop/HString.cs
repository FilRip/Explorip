using System;
using System.Runtime.InteropServices;

using VirtualDesktop.Utils;

namespace VirtualDesktop.Interop;

// ## Note
// .NET 5 has removed WinRT support, so HString cannot marshal to System.String.
// Since marshalling with UnmanagedType.HString fails, use IntPtr to get the string via C#/WinRT MarshalString.
// 
// see also: https://github.com/microsoft/CsWinRT/blob/master/docs/interop.md

[StructLayout(LayoutKind.Sequential)]
public readonly struct HString
{
    private readonly IntPtr _abi;

    internal HString(string str)
    {
        NativeMethods.WindowsCreateString(str, str.Length, out IntPtr result);
        this._abi = result;
    }

    public static implicit operator string(HString hStr)
    {
        IntPtr ptrBuffer = NativeMethods.WindowsGetStringRawBuffer(hStr._abi, out uint length);
        string result = Marshal.PtrToStringUni(ptrBuffer, (int)length)!;
        NativeMethods.WindowsDeleteString(hStr._abi);
        return result;
    }
}
