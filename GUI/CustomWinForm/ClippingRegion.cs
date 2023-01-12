using System;
using System.Drawing;

using ManagedShell.Interop;

namespace CustomWinForm
{
    internal class ClippingRegion : IDisposable
    {
        private IntPtr _hClipRegion;
        private IntPtr _hDc;

        public ClippingRegion(IntPtr hdc, Rectangle cliprect, Rectangle canvasrect)
        {
            CreateRectangleClip(hdc, cliprect, canvasrect);
        }

        public ClippingRegion(IntPtr hdc, NativeMethods.Rect cliprect, NativeMethods.Rect canvasrect)
        {
            CreateRectangleClip(hdc, cliprect, canvasrect);
        }

        public ClippingRegion(IntPtr hdc, Rectangle cliprect, Rectangle canvasrect, uint radius)
        {
            CreateRoundedRectangleClip(hdc, cliprect, canvasrect, radius);
        }

        public ClippingRegion(IntPtr hdc, NativeMethods.Rect cliprect, NativeMethods.Rect canvasrect, uint radius)
        {
            CreateRoundedRectangleClip(hdc, cliprect, canvasrect, radius);
        }

        public void CreateRectangleClip(IntPtr hdc, Rectangle cliprect, Rectangle canvasrect)
        {
            _hDc = hdc;
            IntPtr clip = NativeMethods.CreateRectRgn(cliprect.Left, cliprect.Top, cliprect.Right, cliprect.Bottom);
            IntPtr canvas = NativeMethods.CreateRectRgn(canvasrect.Left, canvasrect.Top, canvasrect.Right, canvasrect.Bottom);
            _hClipRegion = NativeMethods.CreateRectRgn(canvasrect.Left, canvasrect.Top, canvasrect.Right, canvasrect.Bottom);
            NativeMethods.CombineRgn(_hClipRegion, canvas, clip, NativeMethods.CombineRgnStyles.RGN_DIFF);
            NativeMethods.SelectClipRgn(_hDc, _hClipRegion);
            NativeMethods.DeleteObject(clip);
            NativeMethods.DeleteObject(canvas);
        }

        public void CreateRectangleClip(IntPtr hdc, NativeMethods.Rect cliprect, NativeMethods.Rect canvasrect)
        {
            _hDc = hdc;
            IntPtr clip = NativeMethods.CreateRectRgn(cliprect.Left, cliprect.Top, cliprect.Right, cliprect.Bottom);
            IntPtr canvas = NativeMethods.CreateRectRgn(canvasrect.Left, canvasrect.Top, canvasrect.Right, canvasrect.Bottom);
            _hClipRegion = NativeMethods.CreateRectRgn(canvasrect.Left, canvasrect.Top, canvasrect.Right, canvasrect.Bottom);
            NativeMethods.CombineRgn(_hClipRegion, canvas, clip, NativeMethods.CombineRgnStyles.RGN_DIFF);
            NativeMethods.SelectClipRgn(_hDc, _hClipRegion);
            NativeMethods.DeleteObject(clip);
            NativeMethods.DeleteObject(canvas);
        }

        public void CreateRoundedRectangleClip(IntPtr hdc, Rectangle cliprect, Rectangle canvasrect, uint radius)
        {
            int r = (int)radius;
            _hDc = hdc;
            // create rounded regions
            IntPtr clip = NativeMethods.CreateRoundRectRgn(cliprect.Left, cliprect.Top, cliprect.Right, cliprect.Bottom, r, r);
            IntPtr canvas = NativeMethods.CreateRectRgn(canvasrect.Left, canvasrect.Top, canvasrect.Right, canvasrect.Bottom);
            _hClipRegion = NativeMethods.CreateRoundRectRgn(canvasrect.Left, canvasrect.Top, canvasrect.Right, canvasrect.Bottom, r, r);
            NativeMethods.CombineRgn(_hClipRegion, canvas, clip, NativeMethods.CombineRgnStyles.RGN_DIFF);
            // add it in
            NativeMethods.SelectClipRgn(_hDc, _hClipRegion);
            NativeMethods.DeleteObject(clip);
            NativeMethods.DeleteObject(canvas);
        }

        public void CreateRoundedRectangleClip(IntPtr hdc, NativeMethods.Rect cliprect, NativeMethods.Rect canvasrect, uint radius)
        {
            int r = (int)radius;
            _hDc = hdc;
            // create rounded regions
            IntPtr clip = NativeMethods.CreateRoundRectRgn(cliprect.Left, cliprect.Top, cliprect.Right, cliprect.Bottom, r, r);
            IntPtr canvas = NativeMethods.CreateRectRgn(canvasrect.Left, canvasrect.Top, canvasrect.Right, canvasrect.Bottom);
            _hClipRegion = NativeMethods.CreateRoundRectRgn(canvasrect.Left, canvasrect.Top, canvasrect.Right, canvasrect.Bottom, r, r);
            NativeMethods.CombineRgn(_hClipRegion, canvas, clip, NativeMethods.CombineRgnStyles.RGN_DIFF);
            // add it in
            NativeMethods.SelectClipRgn(_hDc, _hClipRegion);
            NativeMethods.DeleteObject(clip);
            NativeMethods.DeleteObject(canvas);
        }

        public void Release()
        {
            if (_hClipRegion != IntPtr.Zero)
            {
                // remove region
                NativeMethods.SelectClipRgn(_hDc, IntPtr.Zero);
                // delete region
                NativeMethods.DeleteObject(_hClipRegion);
            }
        }

        public void Dispose()
        {
            Release();
        }
    }
}
