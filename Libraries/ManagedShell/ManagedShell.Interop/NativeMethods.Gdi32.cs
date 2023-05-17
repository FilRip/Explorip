using System;
using System.Runtime.InteropServices;

namespace ManagedShell.Interop
{
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
    }
}
