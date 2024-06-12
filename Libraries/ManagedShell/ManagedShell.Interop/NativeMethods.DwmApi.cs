using System;
using System.Runtime.InteropServices;

namespace ManagedShell.Interop;

public partial class NativeMethods
{
    public const int DWM_TNP_VISIBLE = 0x8,
        DWM_TNP_OPACITY = 0x4,
        DWM_TNP_RECTDESTINATION = 0x1;

    const string DwmApi_DllName = "dwmapi";

    [DllImport(DwmApi_DllName)]
    internal static extern int DwmRegisterThumbnail(IntPtr dest, IntPtr src, out IntPtr thumb);

    [DllImport(DwmApi_DllName)]
    internal static extern int DwmUnregisterThumbnail(IntPtr thumb);

    [DllImport(DwmApi_DllName)]
    internal static extern int DwmQueryThumbnailSourceSize(IntPtr thumb, out Psize size);

    [DllImport(DwmApi_DllName)]
    internal static extern int DwmUpdateThumbnailProperties(IntPtr hThumb, ref DwmThumbnailProperties props);

    [DllImport(DwmApi_DllName, EntryPoint = "#113", SetLastError = true)]
    internal static extern uint DwmActivateLivePreview(uint enable, IntPtr targetHwnd, IntPtr callingHwnd, AeroPeekType type);

    // Windows 8.1+ version has an extra unknown parameter
    [DllImport(DwmApi_DllName, EntryPoint = "#113", SetLastError = true)]
    internal static extern uint DwmActivateLivePreview(uint enable, IntPtr targetHwnd, IntPtr callingHwnd, AeroPeekType type, IntPtr unknown);

    [DllImport(DwmApi_DllName)]
    internal static extern int DwmGetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE dwAttribute, out uint pvAttribute, int cbAttribute);

    [DllImport(DwmApi_DllName)]
    internal static extern int DwmSetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE dwAttribute, ref int pvAttribute, int cbAttribute);

    [DllImport(DwmApi_DllName, PreserveSig = false)]
    internal static extern bool DwmIsCompositionEnabled();

    [DllImport(DwmApi_DllName)]
    internal static extern int DwmExtendFrameIntoClientArea(IntPtr hdc, ref Margins marInset);

    [DllImport(DwmApi_DllName)]
    internal static extern int DwmDefWindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, out IntPtr result);

    [StructLayout(LayoutKind.Sequential)]
    public struct DwmColorizationParams
    {
        public uint clrColor;
        public uint clrAfterGlow;
        public uint nIntensity;
        public uint clrAfterGlowBalance;
        public uint clrBlurBalance;
        public uint clrGlassReflectionIntensity;
        public bool fOpaque;
    }

    [DllImport(DwmApi_DllName, EntryPoint = "#127")]
    internal static extern void DwmGetColorizationParameters(ref DwmColorizationParams colors);

    [StructLayout(LayoutKind.Sequential)]
    internal struct PaintStruct
    {
        internal IntPtr hdc;
        internal int fErase;
        internal Rect rcPaint;
        internal int fRestore;
        internal int fIncUpdate;
        internal int Reserved1;
        internal int Reserved2;
        internal int Reserved3;
        internal int Reserved4;
        internal int Reserved5;
        internal int Reserved6;
        internal int Reserved7;
        internal int Reserved8;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Margins(int Left, int Right, int Top, int Bottom)
    {
        public int cxLeftWidth = Left;
        public int cxRightWidth = Right;
        public int cyTopHeight = Top;
        public int cyBottomHeight = Bottom;
    }

    [Flags()]
    public enum DWM_TNP
    {
        RECTDESTINATION = 0x00000001,
        RECTSOURCE = 0x00000002,
        OPACITY = 0x00000004,
        VISIBLE = 0x00000008,
        SOURCECLIENTAREAONLY = 0x00000010,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DwmThumbnailProperties
    {
        public DWM_TNP dwFlags;
        public Rect rcDestination;
        public Rect rcSource;
        public byte opacity;
        public bool fVisible;
        public bool fSourceClientAreaOnly;
    }

    public enum DWMWINDOWATTRIBUTE
    {
        DWMWA_NCRENDERING_ENABLED = 1,
        DWMWA_NCRENDERING_POLICY,
        DWMWA_TRANSITIONS_FORCEDISABLED,
        DWMWA_ALLOW_NCPAINT,
        DWMWA_CAPTION_BUTTON_BOUNDS,
        DWMWA_NONCLIENT_RTL_LAYOUT,
        DWMWA_FORCE_ICONIC_REPRESENTATION,
        DWMWA_FLIP3D_POLICY,
        DWMWA_EXTENDED_FRAME_BOUNDS,
        DWMWA_HAS_ICONIC_BITMAP,
        DWMWA_DISALLOW_PEEK,
        DWMWA_EXCLUDED_FROM_PEEK,
        DWMWA_CLOAK,
        DWMWA_CLOAKED,
        DWMWA_FREEZE_REPRESENTATION,
        DWMWA_LAST
    }

    public enum DWMNCRENDERINGPOLICY
    {
        DWMNCRP_USEWINDOWSTYLE,
        DWMNCRP_DISABLED,
        DWMNCRP_ENABLED,
        DWMNCRP_LAST
    }

    public enum AeroPeekType : uint
    {
        Default = 0,
        Desktop = 1,
        Window = 3
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Psize
    {
        public int x;
        public int y;
    }
}
