using System;
using System.Runtime.InteropServices;

namespace ManagedShell.Interop;

/// <summary>
/// Container class for Win32 Native methods used within the desktop application (e.g. shutdown, sleep, et al).
/// </summary>
public partial class NativeMethods
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Rect(int left, int top, int right, int bottom)
    {
        public int Left = left;
        public int Top = top;
        public int Right = right;
        public int Bottom = bottom;

#pragma warning disable IDE0251 // Définir comme membre 'readonly'
        public int Width
        {
            get { return Right - Left; }
        }

        public int Height
        {
            get { return Bottom - Top; }
        }
#pragma warning restore IDE0251 // Définir comme membre 'readonly'
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Point(long x, long y)
    {
        public long x = x;
        public long y = y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PointInt(int x, int y)
    {
        public int x = x;
        public int y = y;
    }

    // lo = x; hi = y
    public static IntPtr MakeParam(int loWord, int hiWord)
    {
        int i = ((short)hiWord << 16) | ((short)loWord & 0xffff);
        return new IntPtr(i);
    }

    /// <summary>
    /// Retrieves the High Word of a WParam of a WindowMessage
    /// </summary>
    /// <param name="ptr">The pointer to the WParam</param>
    /// <returns>The unsigned integer for the High Word</returns>
    public static ulong HiWord(IntPtr ptr)
    {
        if (((ulong)ptr & 0x80000000) == 0x80000000)
            return ((ulong)ptr >> 16);
        else
            return ((ulong)ptr >> 16) & 0xffff;
    }

    /// <summary>
    /// Retrieves the Low Word of a WParam of a WindowMessage
    /// </summary>
    /// <param name="ptr">The pointer to the WParam</param>
    /// <returns>The unsigned integer for the Low Word</returns>
    public static ulong LoWord(IntPtr ptr)
    {
        return (ulong)ptr & 0xffff;
    }
}
