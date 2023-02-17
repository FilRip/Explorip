using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

using CustomWinForm;

using ManagedShell.Interop;

namespace System.Windows.Forms
{
    public class CustomForm : Form
    {
        // Doc : https://www.codeproject.com/Articles/55180/Extending-the-Non-Client-Area-in-Aero
        //       https://www.codeproject.com/Articles/32623/Vista-Aero-ToolStrip-on-Non-Client-Area?fid=1533828&df=90&mpp=25&sort=Position&spc=Relaxed&prof=True&view=Normal&fr=1#xx0xx
        private bool _aeroEnabled;
        private bool _extendNoClientArea;
        private bool _removeNoClientArea;
        private bool _margingSet;
        //private bool _isPainting;
        private NativeMethods.Margins _dwmMargins;
        private int _iStoreHeight;

        public CustomForm() : base()
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
            CheckAeroEnabled();
            DoubleBuffered = true;
        }

        private void CheckAeroEnabled()
        {
            if (Environment.OSVersion.Version.Major >= 10)
            {
                _aeroEnabled = NativeMethods.DwmIsCompositionEnabled();
            }
        }

        public bool ExtendNoClientArea
        {
            get { return _extendNoClientArea; }
            set
            {
                _extendNoClientArea = value;
                _removeNoClientArea = false;
            }
        }

        public bool RemoveNoClientArea
        {
            get { return _removeNoClientArea; }
            set
            {
                _removeNoClientArea = value;
                _extendNoClientArea = false;
            }
        }

        public bool ShowTitle { get; set; }

        public Rectangle NoClientArea
        {
            get { return new Rectangle(_dwmMargins.cxLeftWidth, _dwmMargins.cyTopHeight, _dwmMargins.cxRightWidth, _dwmMargins.cyBottomHeight); }
            set
            {
                if (!_aeroEnabled)
                    throw new CustomWinFormException("Aero is not enabled");
                if (_removeNoClientArea)
                    throw new CustomWinFormException("Remove NoClientArea already enabled for this form. Disable it to enable ExtendNoClientArea");
                _dwmMargins.cxLeftWidth = value.X;
                _dwmMargins.cyTopHeight = value.Y;
                _dwmMargins.cxRightWidth = value.Width;
                _dwmMargins.cyBottomHeight = value.Height;
                _extendNoClientArea = true;
            }
        }

        //public HorizontalAlignment TitleAlignment { get; set; }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (_aeroEnabled)
            {
                if (_removeNoClientArea)
                {
                    e.Graphics.Clear(Color.Transparent);
                    e.Graphics.FillRectangle(SystemBrushes.ButtonFace,
                            Rectangle.FromLTRB(
                                _dwmMargins.cxLeftWidth - 0,
                                _dwmMargins.cyTopHeight - 0,
                                Width - _dwmMargins.cxRightWidth - 0,
                                Height - _dwmMargins.cyBottomHeight - 0));
                }
            }
            else
                e.Graphics.Clear(BackColor);
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            if (_aeroEnabled && _removeNoClientArea)
                NativeMethods.DwmExtendFrameIntoClientArea(this.Handle, ref _dwmMargins);
        }

        private static int LoWord(int dwValue)
        {
            return dwValue & 0xFFFF;
        }

        private static int HiWord(int dwValue)
        {
            return (dwValue >> 16) & 0xFFFF;
        }

        protected override void WndProc(ref Message m)
        {
            if (_aeroEnabled)
            {
                if (_removeNoClientArea)
                {
                    int dwmHandled = NativeMethods.DwmDefWindowProc(m.HWnd, m.Msg, m.WParam, m.LParam, out IntPtr result);

                    if (dwmHandled == 1)
                    {
                        m.Result = result;
                        return;
                    }

                    if (m.Msg == (int)NativeMethods.WM.NCCALCSIZE && (int)m.WParam == 1)
                    {
                        NativeMethods.NcCalcSizeParams nccsp = (NativeMethods.NcCalcSizeParams)Marshal.PtrToStructure(m.LParam, typeof(NativeMethods.NcCalcSizeParams));

                        // Adjust (shrink) the client rectangle to accommodate the border:
                        nccsp.rect0.Top += 0;
                        nccsp.rect0.Bottom += 0;
                        nccsp.rect0.Left += 0;
                        nccsp.rect0.Right += 0;

                        if (!_margingSet)
                        {
                            //Set what client area would be for passing to DwmExtendIntoClientArea
                            _dwmMargins.cyTopHeight = nccsp.rect2.Top - nccsp.rect1.Top;
                            _dwmMargins.cxLeftWidth = nccsp.rect2.Left - nccsp.rect1.Left;
                            _dwmMargins.cyBottomHeight = nccsp.rect1.Bottom - nccsp.rect2.Bottom;
                            _dwmMargins.cxRightWidth = nccsp.rect1.Right - nccsp.rect2.Right;
                            _margingSet = true;
                        }

                        Marshal.StructureToPtr(nccsp, m.LParam, false);

                        m.Result = IntPtr.Zero;
                    }
                    else if (m.Msg == (int)NativeMethods.WM.NCHITTEST && (int)m.Result == 0)
                    {
                        m.Result = HitTestNCA(m.LParam);
                    }
                    else if (m.Msg == (int)NativeMethods.WM.SYSCOMMAND)
                    {
                        UInt32 param;
                        if (IntPtr.Size == 4)
                            param = (UInt32)(m.WParam.ToInt32());
                        else
                            param = (UInt32)(m.WParam.ToInt64());
                        if ((param & 0xFFF0) == NativeMethods.SC_RESTORE)
                        {
                            this.Height = _iStoreHeight;
                        }
                        else if (this.WindowState == FormWindowState.Normal)
                        {
                            _iStoreHeight = this.Height;
                        }
                        base.WndProc(ref m);
                    }
                    else
                    {
                        base.WndProc(ref m);
                    }
                }
                /*else if (_extendNoClientArea)
                {
                    CustomProc(ref m);
                }*/
                else
                    base.WndProc(ref m);
            }
            else
                base.WndProc(ref m);
        }

        private IntPtr HitTestNCA(IntPtr lparam)
        {
            int HTCLIENT = 1;
            int HTCAPTION = 2;
            int HTLEFT = 10;
            int HTRIGHT = 11;
            int HTTOP = 12;
            int HTTOPLEFT = 13;
            int HTTOPRIGHT = 14;
            int HTBOTTOM = 15;
            int HTBOTTOMLEFT = 16;
            int HTBOTTOMRIGHT = 17;

            Point p = new((Int16)LoWord((int)lparam), (Int16)HiWord((int)lparam));

            Rectangle topleft = RectangleToScreen(new Rectangle(0, 0, _dwmMargins.cxLeftWidth, _dwmMargins.cxLeftWidth));

            if (topleft.Contains(p))
                return new IntPtr(HTTOPLEFT);

            Rectangle topright = RectangleToScreen(new Rectangle(Width - _dwmMargins.cxRightWidth, 0, _dwmMargins.cxRightWidth, _dwmMargins.cxRightWidth));

            if (topright.Contains(p))
                return new IntPtr(HTTOPRIGHT);

            Rectangle botleft = RectangleToScreen(new Rectangle(0, Height - _dwmMargins.cyBottomHeight, _dwmMargins.cxLeftWidth, _dwmMargins.cyBottomHeight));

            if (botleft.Contains(p))
                return new IntPtr(HTBOTTOMLEFT);

            Rectangle botright = RectangleToScreen(new Rectangle(Width - _dwmMargins.cxRightWidth, Height - _dwmMargins.cyBottomHeight, _dwmMargins.cxRightWidth, _dwmMargins.cyBottomHeight));

            if (botright.Contains(p))
                return new IntPtr(HTBOTTOMRIGHT);

            Rectangle top = RectangleToScreen(new Rectangle(0, 0, Width, _dwmMargins.cxLeftWidth));

            if (top.Contains(p))
                return new IntPtr(HTTOP);

            Rectangle cap = RectangleToScreen(new Rectangle(0, _dwmMargins.cxLeftWidth, Width, _dwmMargins.cyTopHeight - _dwmMargins.cxLeftWidth));

            if (cap.Contains(p))
                return new IntPtr(HTCAPTION);

            Rectangle left = RectangleToScreen(new Rectangle(0, 0, _dwmMargins.cxLeftWidth, Height));

            if (left.Contains(p))
                return new IntPtr(HTLEFT);

            Rectangle right = RectangleToScreen(new Rectangle(Width - _dwmMargins.cxRightWidth, 0, _dwmMargins.cxRightWidth, Height));

            if (right.Contains(p))
                return new IntPtr(HTRIGHT);

            Rectangle bottom = RectangleToScreen(new Rectangle(0, Height - _dwmMargins.cyBottomHeight, Width, _dwmMargins.cyBottomHeight));

            if (bottom.Contains(p))
                return new IntPtr(HTBOTTOM);

            return new IntPtr(HTCLIENT);
        }

        /*private void GetFrameSize()
        {
            if (this.MinimizeBox)
                _iFrameOffset = 100;
            else
                _iFrameOffset = 40;
            switch (this.FormBorderStyle)
            {
                case FormBorderStyle.Sizable:
                    _iCaptionHeight = CAPTION_HEIGHT;
                    _iFrameHeight = FRAME_WIDTH;
                    _iFrameWidth = FRAME_WIDTH;
                    break;
                case FormBorderStyle.Fixed3D:
                    _iCaptionHeight = 27;
                    _iFrameHeight = 4;
                    _iFrameWidth = 4;
                    break;
                case FormBorderStyle.FixedDialog:
                    _iCaptionHeight = 25;
                    _iFrameHeight = 2;
                    _iFrameWidth = 2;
                    break;
                case FormBorderStyle.FixedSingle:
                    _iCaptionHeight = 25;
                    _iFrameHeight = 2;
                    _iFrameWidth = 2;
                    break;
                case FormBorderStyle.FixedToolWindow:
                    _iFrameOffset = 20;
                    _iCaptionHeight = 21;
                    _iFrameHeight = 2;
                    _iFrameWidth = 2;
                    break;
                case FormBorderStyle.SizableToolWindow:
                    _iFrameOffset = 20;
                    _iCaptionHeight = 26;
                    _iFrameHeight = 4;
                    _iFrameWidth = 4;
                    break;
                default:
                    _iCaptionHeight = CAPTION_HEIGHT;
                    _iFrameHeight = FRAME_WIDTH;
                    _iFrameWidth = FRAME_WIDTH;
                    break;
            }
        }

        private static IntPtr MSG_HANDLED = new IntPtr(0);
        private void CustomProc(ref Message m)
        {
            switch (m.Msg)
            {
                case (int)NativeMethods.WM.PAINT:
                    NativeMethods.PAINTSTRUCT ps = new();
                    if (!_isPainting)
                    {
                        _isPainting = true;
                        NativeMethods.BeginPaint(m.HWnd, ref ps);
                        PaintThis(ps.hdc, ps.rcPaint);
                        NativeMethods.EndPaint(m.HWnd, ref ps);
                        _isPainting = false;
                        base.WndProc(ref m);
                    }
                    else
                    {
                        base.WndProc(ref m);
                    }
                    break;
                case (int)NativeMethods.WM.CREATE:
                    GetFrameSize();
                    FrameChanged();
                    m.Result = MSG_HANDLED;
                    base.WndProc(ref m);
                    break;
                case (int)NativeMethods.WM.NCCALCSIZE:
                    if (m.WParam != IntPtr.Zero && m.Result == IntPtr.Zero)
                    {
                        if (_bExtendIntoFrame)
                        {
                            NativeMethods.NCCALCSIZE_PARAMS nc = (NativeMethods.NCCALCSIZE_PARAMS)Marshal.PtrToStructure(m.LParam, typeof(NativeMethods.NCCALCSIZE_PARAMS));
                            nc.rect0.Right -= 6;
                            nc.rect1 = nc.rect0;
                            Marshal.StructureToPtr(nc, m.LParam, false);
                            m.Result = (IntPtr)WVR_VALIDRECTS;
                        }
                    }
                    else
                        base.WndProc(ref m);
                    break;
                case (int)NativeMethods.WM.SYSCOMMAND:
                    UInt32 param;
                    if (IntPtr.Size == 4)
                        param = (UInt32)(m.WParam.ToInt32());
                    else
                        param = (UInt32)(m.WParam.ToInt64());
                    if ((param & 0xFFF0) == NativeMethods.SC_RESTORE)
                    {
                        this.Height = _iStoreHeight;
                    }
                    else if (this.WindowState == FormWindowState.Normal)
                    {
                        _iStoreHeight = this.Height;
                    }
                    base.WndProc(ref m);
                    break;
                case (int)NativeMethods.WM.NCHITTEST:
                    if (m.Result == (IntPtr)HIT_CONSTANTS.HTNOWHERE)
                    {
                        if (NativeMethods.DwmDefWindowProc(m.HWnd, m.Msg, m.WParam, m.LParam, out IntPtr res) != 0)
                            m.Result = res;
                        else
                            m.Result = (IntPtr)HitTest();
                    }
                    else
                        base.WndProc(ref m);
                    break;
                case (int)NativeMethods.WM.DWMCOMPOSITIONCHANGED:
                case (int)NativeMethods.WM.ACTIVATE:
                    NativeMethods.DwmExtendFrameIntoClientArea(this.Handle, ref _dwmMargins);
                    m.Result = MSG_HANDLED;
                    base.WndProc(ref m);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        private enum HIT_CONSTANTS
        {
            HTERROR = -2,
            HTTRANSPARENT = -1,
            HTNOWHERE = 0,
            HTCLIENT = 1,
            HTCAPTION = 2,
            HTSYSMENU = 3,
            HTGROWBOX = 4,
            HTMENU = 5,
            HTHSCROLL = 6,
            HTVSCROLL = 7,
            HTMINBUTTON = 8,
            HTMAXBUTTON = 9,
            HTLEFT = 10,
            HTRIGHT = 11,
            HTTOP = 12,
            HTTOPLEFT = 13,
            HTTOPRIGHT = 14,
            HTBOTTOM = 15,
            HTBOTTOMLEFT = 16,
            HTBOTTOMRIGHT = 17,
            HTBORDER = 18,
            HTOBJECT = 19,
            HTCLOSE = 20,
            HTHELP = 21
        }

        private const int CAPTION_HEIGHT = 30;
        private const int FRAME_WIDTH = 8;
        private int _iCaptionHeight = CAPTION_HEIGHT;
        private int _iFrameHeight = FRAME_WIDTH;
        private int _iFrameWidth = FRAME_WIDTH;
        private int _iFrameOffset = 100;
        private const int WVR_VALIDRECTS = 0x400;
        private bool _bExtendIntoFrame = false;
        private NativeMethods.Rect _tClientRect = new();
        private const int BLACK_BRUSH = 4;

        private int CaptionHeight
        {
            get { return _iCaptionHeight; }
        }

        private int FrameWidth
        {
            get { return _iFrameWidth; }
        }

        private int FrameHeight
        {
            get { return _iFrameHeight; }
        }

        private const int SWP_FRAMECHANGED = 0x0020;
        private void FrameChanged()
        {
            NativeMethods.Rect rcClient;
            NativeMethods.GetWindowRect(this.Handle, out rcClient);
            // force a calc size message
            NativeMethods.SetWindowPos(this.Handle,
                         IntPtr.Zero,
                         rcClient.Left, rcClient.Top,
                         rcClient.Right - rcClient.Left, rcClient.Bottom - rcClient.Top,
                         SWP_FRAMECHANGED);
        }

        private HIT_CONSTANTS HitTest()
        {
            NativeMethods.Rect windowRect;
            Point cursorPoint = new();
            NativeMethods.Rect posRect;
            NativeMethods.GetCursorPos(ref cursorPoint);
            NativeMethods.GetWindowRect(this.Handle, out windowRect);
            cursorPoint.X -= windowRect.Left;
            cursorPoint.Y -= windowRect.Top;
            int width = windowRect.Right - windowRect.Left;
            int height = windowRect.Bottom - windowRect.Top;

            posRect = new NativeMethods.Rect(0, 0, FrameWidth, FrameHeight);
            if (NativeMethods.PtInRect(ref posRect, cursorPoint))
                return HIT_CONSTANTS.HTTOPLEFT;

            posRect = new NativeMethods.Rect(width - FrameWidth, 0, width, FrameHeight);
            if (NativeMethods.PtInRect(ref posRect, cursorPoint))
                return HIT_CONSTANTS.HTTOPRIGHT;

            posRect = new NativeMethods.Rect(FrameWidth, 0, width - (FrameWidth * 2) - _iFrameOffset, FrameHeight);
            if (NativeMethods.PtInRect(ref posRect, cursorPoint))
                return HIT_CONSTANTS.HTTOP;

            posRect = new NativeMethods.Rect(FrameWidth, FrameHeight, width - ((FrameWidth * 2) + _iFrameOffset), _dwmMargins.cyTopHeight);
            if (NativeMethods.PtInRect(ref posRect, cursorPoint))
                return HIT_CONSTANTS.HTCAPTION;

            posRect = new NativeMethods.Rect(0, FrameHeight, FrameWidth, height - FrameHeight);
            if (NativeMethods.PtInRect(ref posRect, cursorPoint))
                return HIT_CONSTANTS.HTLEFT;

            posRect = new NativeMethods.Rect(0, height - FrameHeight, FrameWidth, height);
            if (NativeMethods.PtInRect(ref posRect, cursorPoint))
                return HIT_CONSTANTS.HTBOTTOMLEFT;

            posRect = new NativeMethods.Rect(FrameWidth, height - FrameHeight, width - FrameWidth, height);
            if (NativeMethods.PtInRect(ref posRect, cursorPoint))
                return HIT_CONSTANTS.HTBOTTOM;

            posRect = new NativeMethods.Rect(width - FrameWidth, height - FrameHeight, width, height);
            if (NativeMethods.PtInRect(ref posRect, cursorPoint))
                return HIT_CONSTANTS.HTBOTTOMRIGHT;

            posRect = new NativeMethods.Rect(width - FrameWidth, FrameHeight, width, height - FrameHeight);
            if (NativeMethods.PtInRect(ref posRect, cursorPoint))
                return HIT_CONSTANTS.HTRIGHT;

            return HIT_CONSTANTS.HTCLIENT;
        }

        private void PaintThis(IntPtr hdc, NativeMethods.Rect rc)
        {
            NativeMethods.GetClientRect(this.Handle, out NativeMethods.Rect clientRect);
            if (_bExtendIntoFrame)
            {
                clientRect.Left = _tClientRect.Left - _dwmMargins.cxLeftWidth;
                clientRect.Top = _dwmMargins.cyTopHeight;
                clientRect.Right -= _dwmMargins.cxRightWidth;
                clientRect.Bottom -= _dwmMargins.cyBottomHeight;
            }
            else if (!_isPainting)
            {
                clientRect.Left = _dwmMargins.cxLeftWidth;
                clientRect.Top = _dwmMargins.cyTopHeight;
                clientRect.Right -= _dwmMargins.cxRightWidth;
                clientRect.Bottom -= _dwmMargins.cyBottomHeight;
            }
            if (!_isPainting)
            {
                int clr;
                IntPtr hb;
                using (ClippingRegion cp = new ClippingRegion(hdc, clientRect, rc))
                {
                    if (_aeroEnabled)
                    {
                        NativeMethods.FillRect(hdc, ref rc, NativeMethods.GetStockObject(BLACK_BRUSH));
                    }
                    else
                    {
                        clr = ColorTranslator.ToWin32(Color.FromArgb(0xC2, 0xD9, 0xF7));
                        hb = NativeMethods.CreateSolidBrush(clr);
                        NativeMethods.FillRect(hdc, ref clientRect, hb);
                        NativeMethods.DeleteObject(hb);
                    }
                }
                clr = ColorTranslator.ToWin32(this.BackColor);
                hb = NativeMethods.CreateSolidBrush(clr);
                NativeMethods.FillRect(hdc, ref clientRect, hb);
                NativeMethods.DeleteObject(hb);
            }
            else
            {
                NativeMethods.FillRect(hdc, ref rc, NativeMethods.GetStockObject(BLACK_BRUSH));
            }
            if (_bExtendIntoFrame && ShowTitle)
            {
                Rectangle captionBounds = new Rectangle(4, 4, rc.Right, CaptionHeight);
                using (Graphics g = Graphics.FromHdc(hdc))
                {
                    using (Font fc = new Font("Segoe UI", 12, FontStyle.Regular))
                    {
                        SizeF sz = g.MeasureString(this.Text, fc);
                        int offset = (rc.Right - (int)sz.Width) / 2;
                        if (offset < 2 * FrameWidth)
                            offset = 2 * FrameWidth;
                        captionBounds.X = offset;
                        captionBounds.Y = 4;
                        using (StringFormat sf = new StringFormat())
                        {
                            sf.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.None;
                            sf.FormatFlags = StringFormatFlags.NoWrap;
                            sf.Alignment = StringAlignment.Near;
                            sf.LineAlignment = StringAlignment.Near;
                            using (GraphicsPath path = new GraphicsPath())
                            {
                                g.SmoothingMode = SmoothingMode.HighQuality;
                                path.AddString(this.Text, fc.FontFamily, (int)fc.Style, fc.Size, captionBounds, sf);
                                g.FillPath(Brushes.Black, path);
                            }
                        }
                    }
                }
            }
        }*/
    }
}
