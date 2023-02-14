/*
 * This code is provided under the Code Project Open Licence (CPOL)
 * See http://www.codeproject.com/info/cpol10.aspx for details
*/

using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    /// Description of NativeMethods.
    /// </summary>
    //[SecurityPermission(SecurityAction.Assert, Flags=SecurityPermissionFlag.UnmanagedCode)]
    internal sealed class NativeMethods
    {
        private NativeMethods() { }

        #region Windows Constants

        public const int WM_GETTABRECT = 0x130a;
        public const int WS_EX_TRANSPARENT = 0x20;
        public const int WM_SETFONT = 0x30;
        public const int WM_FONTCHANGE = 0x1d;
        public const int WM_HSCROLL = 0x114;
        public const int TCM_HITTEST = 0x130D;
        public const int WM_PAINT = 0xf;
        public const int WS_EX_LAYOUTRTL = 0x400000;
        public const int WS_EX_NOINHERITLAYOUT = 0x100000;


        #endregion

        #region Content Alignment

#pragma warning disable S3265 // Non-flags enums should not be used in bitwise operations
        public static readonly ContentAlignment AnyRightAlign = ContentAlignment.BottomRight | ContentAlignment.MiddleRight | ContentAlignment.TopRight;
        public static readonly ContentAlignment AnyLeftAlign = ContentAlignment.BottomLeft | ContentAlignment.MiddleLeft | ContentAlignment.TopLeft;
        public static readonly ContentAlignment AnyTopAlign = ContentAlignment.TopRight | ContentAlignment.TopCenter | ContentAlignment.TopLeft;
        public static readonly ContentAlignment AnyBottomAlign = ContentAlignment.BottomRight | ContentAlignment.BottomCenter | ContentAlignment.BottomLeft;
        public static readonly ContentAlignment AnyMiddleAlign = ContentAlignment.MiddleRight | ContentAlignment.MiddleCenter | ContentAlignment.MiddleLeft;
        public static readonly ContentAlignment AnyCenterAlign = ContentAlignment.BottomCenter | ContentAlignment.MiddleCenter | ContentAlignment.TopCenter;
#pragma warning restore S3265 // Non-flags enums should not be used in bitwise operations

        #endregion

        #region User32.dll

        //        [DllImport("user32.dll"), SecurityPermission(SecurityAction.Demand)]
        //		public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 msg, IntPtr wParam, IntPtr lParam);

        public static IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam)
        {
            //	This Method replaces the User32 method SendMessage, but will only work for sending
            //	messages to Managed controls.
            Control control = Control.FromHandle(hWnd);
            if (control == null)
            {
                return IntPtr.Zero;
            }

            Message message = new();
            message.HWnd = hWnd;
            message.LParam = lParam;
            message.WParam = wParam;
            message.Msg = msg;

#pragma warning disable S3011
            MethodInfo wproc = control.GetType().GetMethod("WndProc"
                                                           , BindingFlags.NonPublic
                                                            | BindingFlags.InvokeMethod
                                                            | BindingFlags.FlattenHierarchy
                                                            | BindingFlags.IgnoreCase
                                                            | BindingFlags.Instance);
#pragma warning restore S3011

            object[] args = new object[] { message };
            wproc.Invoke(control, args);

            return ((Message)args[0]).Result;
        }

        #endregion

        #region Misc Functions

        public static int LoWord(IntPtr dWord)
        {
            return dWord.ToInt32() & 0xffff;
        }

        public static int HiWord(IntPtr dWord)
        {
            if ((dWord.ToInt32() & 0x80000000) == 0x80000000)
                return (dWord.ToInt32() >> 16);
            else
                return (dWord.ToInt32() >> 16) & 0xffff;
        }

        public static IntPtr ToIntPtr(object structure)
        {
            IntPtr lparam;
            lparam = Marshal.AllocCoTaskMem(Marshal.SizeOf(structure));
            Marshal.StructureToPtr(structure, lparam, false);
            return lparam;
        }


        #endregion

        #region Windows Structures and Enums

        [Flags()]
        public enum TCHITTEST
        {
            TCHT_NOWHERE = 1,
            TCHT_ONITEMICON = 2,
            TCHT_ONITEMLABEL = 4,
            TCHT_ONITEM = TCHT_ONITEMICON | TCHT_ONITEMLABEL
        }



        [StructLayout(LayoutKind.Sequential)]
        public struct TchitTestInfo
        {

            public TchitTestInfo(Point location)
            {
                pt = location;
                flags = TCHITTEST.TCHT_ONITEM;
            }

            public Point pt;
            public TCHITTEST flags;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct PaintStruct
        {
            public IntPtr hdc;
            public int fErase;
            public Rect rcPaint;
            public int fRestore;
            public int fIncUpdate;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] rgbReserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;

            public Rect(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }

            public Rect(Rectangle r)
            {
                left = r.Left;
                top = r.Top;
                right = r.Right;
                bottom = r.Bottom;
            }

            public static Rect FromXYWH(int x, int y, int width, int height)
            {
                return new Rect(x, y, x + width, y + height);
            }

            public static Rect FromIntPtr(IntPtr ptr)
            {
                Rect rect = (Rect)Marshal.PtrToStructure(ptr, typeof(Rect));
                return rect;
            }

            public Size Size
            {
                get
                {
                    return new Size(right - left, bottom - top);
                }
            }
        }


        #endregion

    }

}
