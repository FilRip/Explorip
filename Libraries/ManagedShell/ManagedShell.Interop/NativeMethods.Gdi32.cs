using System;
using System.Runtime.InteropServices;

namespace ManagedShell.Interop;

public partial class NativeMethods
{
    const string Gdi32_DllName = "gdi32.dll";

    [DllImport(Gdi32_DllName)]
    internal static extern bool DeleteObject(IntPtr hObject);

    [DllImport(Gdi32_DllName)]
    internal static extern IntPtr GetStockObject(int fnObject);

    [DllImport(Gdi32_DllName)]
    internal static extern IntPtr CreateSolidBrush(int crColor);

    internal enum CombineRgnStyles
    {
        RGN_AND = 1,
        RGN_OR = 2,
        RGN_XOR = 3,
        RGN_DIFF = 4,
        RGN_COPY = 5,
        RGN_MIN = RGN_AND,
        RGN_MAX = RGN_COPY
    }

    [DllImport(Gdi32_DllName)]
    internal static extern int CombineRgn(IntPtr hrgnDest, IntPtr hrgnSrc1, IntPtr hrgnSrc2, CombineRgnStyles fnCombineMode);

    [DllImport(Gdi32_DllName)]
    internal static extern int SelectClipRgn(IntPtr hdc, IntPtr hrgn);

    [DllImport(Gdi32_DllName)]
    internal static extern IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

    [DllImport(Gdi32_DllName)]
    internal static extern IntPtr CreateRoundRectRgn(int x1, int y1, int x2, int y2, int cx, int cy);

    public enum Rop
    {
        SRCCOPY = 0x00CC0020,
        SRCPAINT = 0x00EE0086,
        SRCAND = 0x008800C6,
        SRCINVERT = 0x00660046,
        SRCERASE = 0x00440328,
        NOTSRCCOPY = 0x00330008,
        NOTSRCERASE = 0x001100A6,
        MERGECOPY = 0x00C000CA,
        MERGEPAINT = 0x00BB0226,
        PATCOPY = 0x00F00021,
        PATPAINT = 0x00FB0A09,
        PATINVERT = 0x005A0049,
        DSTINVERT = 0x00550009,
        BLACKNESS = 0x00000042,
        WHITENESS = 0x00FF0062,
        NOMIRRORBITMAP = -2147483648,
        CAPTUREBLT = 0x40000000
    }

    [DllImport(Gdi32_DllName)]
    internal static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hObjectSource, int nXSrc, int nYSrc, Rop dwRop);

    [DllImport(Gdi32_DllName)]
    internal static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight);

    [DllImport(Gdi32_DllName)]
    internal static extern IntPtr CreateCompatibleDC(IntPtr hDC);

    [DllImport(Gdi32_DllName)]
    internal static extern bool DeleteDC(IntPtr hDC);

    [DllImport(Gdi32_DllName)]
    internal static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
}
