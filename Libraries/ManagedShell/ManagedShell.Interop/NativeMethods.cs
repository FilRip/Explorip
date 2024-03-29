﻿using System;
using System.Runtime.InteropServices;

namespace ManagedShell.Interop;

/// <summary>
/// Container class for Win32 Native methods used within the desktop application (e.g. shutdown, sleep, et al).
/// </summary>
public partial class NativeMethods
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Rect
    {
        public Rect(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

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
    public struct Point
    {
        public Point(long x, long y)
        {
            this.x = x;
            this.y = y;
        }

        public long x;
        public long y;
    }

    // lo = x; hi = y
    public static IntPtr MakeLParam(int loWord, int hiWord)
    {
        int i = ((short)hiWord << 16) | ((short)loWord & 0xffff);
        return new IntPtr(i);
    }
}