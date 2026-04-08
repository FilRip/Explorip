using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Windows;

using WpfScreenHelper.Enum;

namespace WpfScreenHelper;

internal static class NativeMethods
{
    private const string User32 = "user32.dll";
    private const string Shcore = "shcore.dll";
    private const string D2D1 = "d2d1.dll";

    public delegate bool MonitorEnumProc(IntPtr monitor, IntPtr hdc, IntPtr lprcMonitor, IntPtr lParam);

    public enum DpiType
    {
        EFFECTIVE = 0,
        ANGULAR = 1,
        RAW = 2
    }

    public enum SystemMetric
    {
        SM_CXSCREEN = 0,
        SM_CYSCREEN = 1,
        SM_XVIRTUALSCREEN = 76,
        SM_YVIRTUALSCREEN = 77,
        SM_CXVIRTUALSCREEN = 78,
        SM_CYVIRTUALSCREEN = 79,
        SM_CMONITORS = 80
    }

    public enum Spi : uint
    {
        /// <summary>
        /// Retrieves the size of the work area on the primary display monitor. The work area is the portion of the screen not obscured
        /// by the system taskbar or by application desktop toolbars. The pvParam parameter must point to a RECT structure that receives
        /// the coordinates of the work area, expressed in virtual screen coordinates.
        /// To get the work area of a monitor other than the primary display monitor, call the GetMonitorInfo function.
        /// </summary>
        SPI_GETWORKAREA = 0x0030
    }

    [Flags()]
    public enum Spifs
    {
        None = 0x00,

        /// <summary>Writes the new system-wide parameter setting to the user profile.</summary>
        SPIF_UPDATEINIFILE = 0x01,

        /// <summary>Broadcasts the WM_SETTINGCHANGE message after updating the user profile.</summary>
        SPIF_SENDCHANGE = 0x02,

        /// <summary>Same as SPIF_SENDCHANGE.</summary>
        SPIF_SENDWININICHANGE = 0x02
    }

    public enum MonitorDefault
    {
        /// <summary>If the point is not contained within any display monitor, return a handle to the display monitor that is nearest to the point.</summary>
        MONITOR_DEFAULTTONEAREST = 0x00000002,

        /// <summary>If the point is not contained within any display monitor, return NULL.</summary>
        MONITOR_DEFAULTTONULL = 0x00000000,

        /// <summary>If the point is not contained within any display monitor, return a handle to the primary display monitor.</summary>
        MONITOR_DEFAULTTOPRIMARY = 0x00000001
    }

    public enum D2D1FactoryType
    {
        D2D1_FACTORY_TYPE_SINGLE_THREADED = 0,
        D2D1_FACTORY_TYPE_MULTI_THREADED = 1,
    }

    public static readonly HandleRef NullHandleRef = new(null, IntPtr.Zero);

    [DllImport(Shcore, CharSet = CharSet.Auto)]
    [ResourceExposure(ResourceScope.None)]
    public static extern IntPtr GetDpiForMonitor([In()] IntPtr hmonitor, [In()] DpiType dpiType, [Out()] out uint dpiX, [Out()] out uint dpiY);

    [DllImport(User32, CharSet = CharSet.Auto)]
    [ResourceExposure(ResourceScope.None)]
    public static extern bool GetMonitorInfo(HandleRef hmonitor, [In()][Out()] MonitorInfoEx info);

    [DllImport(User32, ExactSpelling = true)]
    [ResourceExposure(ResourceScope.None)]
    public static extern bool EnumDisplayMonitors(HandleRef hdc, ComRect rcClip, MonitorEnumProc lpfnEnum, IntPtr dwData);

    [DllImport(User32, ExactSpelling = true)]
    [ResourceExposure(ResourceScope.None)]
    public static extern IntPtr MonitorFromWindow(HandleRef handle, int flags);

    [DllImport(User32, ExactSpelling = true, CharSet = CharSet.Auto)]
    [ResourceExposure(ResourceScope.None)]
    public static extern int GetSystemMetrics(SystemMetric nIndex);

    [DllImport(User32, CharSet = CharSet.Auto)]
    [ResourceExposure(ResourceScope.None)]
    public static extern bool SystemParametersInfo(Spi nAction, int nParam, ref Rect rc, Spifs nUpdate);

    [DllImport(User32, ExactSpelling = true)]
    [ResourceExposure(ResourceScope.None)]
    public static extern IntPtr MonitorFromPoint(PointStruct pt, MonitorDefault flags);

    [DllImport(User32, ExactSpelling = true, CharSet = CharSet.Auto)]
    [ResourceExposure(ResourceScope.None)]
    public static extern bool GetCursorPos([In()][Out()] Point pt);

    [DllImport(User32, SetLastError = true)]
    public static extern bool IsProcessDPIAware();

    [DllImport(User32, SetLastError = true)]
    public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

    [DllImport(User32, SetLastError = true)]
    public static extern bool GetWindowRect(IntPtr hWnd, ref Rect rect);

    [DllImport(D2D1)]
    public static extern int D2D1CreateFactory(D2D1FactoryType factoryType, [MarshalAs(UnmanagedType.LPStruct)] Guid riid, IntPtr pFactoryOptions, out ID2D1Factory ppIFactory);

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

        public Rect(System.Windows.Rect r)
        {
            left = (int)r.Left;
            top = (int)r.Top;
            right = (int)r.Right;
            bottom = (int)r.Bottom;
        }

        public static Rect FromXYWH(int x, int y, int width, int height)
        {
            return new Rect(x, y, x + width, y + height);
        }

        public readonly Size Size => new(right - left, bottom - top);
    }

    // use this in cases where the Native API takes a POINT not a POINT*
    // classes marshal by ref.
    [StructLayout(LayoutKind.Sequential)]
    public struct PointStruct(int x, int y)
    {
        public int x = x;
        public int y = y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class Point
    {
        public int x;
        public int y;

        public Point()
        {
        }

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return "{x=" + x + ", y=" + y + "}";
        }
    }

    [Flags()]
    public enum EMonitorInfos
    {
        Primary = 0x00000001,
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
    public class MonitorInfoEx
    {
        internal int cbSize = Marshal.SizeOf(typeof(MonitorInfoEx));

        internal Rect rcMonitor = new();
        internal Rect rcWork = new();
        internal EMonitorInfos dwFlags = 0;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        internal string szDevice;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class ComRect
    {
        public int bottom;
        public int left;
        public int right;
        public int top;

        public ComRect()
        {
        }

        public ComRect(System.Windows.Rect r)
        {
            left = (int)r.X;
            top = (int)r.Y;
            right = (int)r.Right;
            bottom = (int)r.Bottom;
        }

        public ComRect(int left, int top, int right, int bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }

        public static ComRect FromXYWH(int x, int y, int width, int height)
        {
            return new ComRect(x, y, x + width, y + height);
        }

        public override string ToString()
        {
            return "Left = " + left + " Top " + top + " Right = " + right + " Bottom = " + bottom;
        }
    }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("06152247-6f50-465a-9245-118bfd3b6007")]
    internal interface ID2D1Factory
    {
        int ReloadSystemMetrics();

        [PreserveSig()]
        void GetDesktopDpi(out float dpiX, out float dpiY);

        // the rest is not implemented as we don't need it
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct DisplayDevice
    {
        public DisplayDevice()
        {
            cbSize = (uint)Marshal.SizeOf(typeof(DisplayDevice));
        }

        public uint cbSize;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string Name;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string String;
        public EDisplayDevice StateFlags;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string Id;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string Key;
    }

    public enum Edd
    {
        None = 0,
        GET_DEVICE_INTERFACE_NAME = 0x00000001,
    }

#nullable enable
    [DllImport(User32, SetLastError = false, CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool EnumDisplayDevices([Optional()] string? lpDevice, uint iDevNum, ref DisplayDevice lpDisplayDevice, Edd dwFlags);

    [Flags()]
    public enum QueryDisplayConfigs : uint
    {
        AllPaths = 0x00000001,
        OnlyActivePaths = 0x00000002,
        DatabaseCurrent = 0x00000004,
        VirtualModeAware = 0x00000010,
        IncludeHmd = 0x00000020,
        VirtualRefreshRateAware = 0x00000040,
    }

    public enum DisplayConfigModeInfoType : uint
    {
        None = 0,
        Source = 1,
        Target = 2,
        DesktopImage = 3,
    }

    [Flags()]
    public enum DisplayConfigPathSourceStatus : uint
    {
        Disabled = 0,
        InUse = 1,
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct RectInt4Bytes(int left, int top, int right, int bottom)
    {
        public int Left = left;
        public int Top = top;
        public int Right = right;
        public int Bottom = bottom;

        public readonly int Width
        {
            get { return Right - Left; }
        }

        public readonly int Height
        {
            get { return Bottom - Top; }
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Luid
    {
        public uint LowPart;
        public int HighPart;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct DisplayConfigPathSourceInfo
    {
        public Luid adapterId;
        public uint id;
        public DisplayConfigPathSourceMode modeInfoIdx;
        public DisplayConfigPathSourceStatus statusFlags;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 4)]
    public struct DisplayConfigPathSourceMode
    {
        [FieldOffset(0)]
        public uint modeInfoIdx;
        [FieldOffset(0)]
        public DisplayConfigPathSourceModeCloneInfo CloneGroupId;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct DisplayConfigPathSourceModeCloneInfo
    {
        public uint value;

        public readonly ushort CloneGroupId => (ushort)(value & 0xFFFF);

        public readonly ushort SourceModeInfoIdx => (ushort)((value >> 16) & 0xFFFF);
    }

    public enum DisplayConfigVideoOutputTechnology : uint
    {
        Other = unchecked((uint)-1),
        HD15 = 0,
        SVideo = 1,
        CompositeVideo = 2,
        ComponentVideo = 3,
        Dvi = 4,
        Hdmi = 5,
        Lvds = 6,
        D_JPN = 8,
        Sdi = 9,
        DisplayPortExternal = 10,
        DisplayPortEmbedded = 11,
        UdiExternal = 12,
        UdiEmbedded = 13,
        SdtvDongle = 14,
        Miracast = 15,
        IndirectWired = 16,
        IndirectVirtual = 17,
        DisplayPortUsbTunnel = 18,
        Internal = 0x80000000,
    }

    public enum DisplayConfigRotation : uint
    {
        Identity = 1,
        Rotate90 = 2,
        Rotate180 = 3,
        Rotate270 = 4,
    }

    public enum DisplayConfigScaling : uint
    {
        Identity = 1,
        Centered = 2,
        Stretched = 3,
        AspectRatioCenteredMax = 4,
        Custom = 5,
        Preferred = 128,
    }

    public enum ScanLineOrdering : uint
    {
        Unspecified = 0,
        Progressive = 1,
        Interlaced = 2,
        InterlacedUpperFieldFirst = 3,
        InterlaceLowerFieldFirst = 4,
    }

    [Flags()]
    public enum DisplayConfigTargetStatus : uint
    {
        InUse = 0x00000001,
        Forcible = 0x00000002,
        ForcedAvailabilityBoot = 0x00000004,
        ForcedAvailabilityPath = 0x00000008,
        ForcedAvailabilitySystem = 0x00000010,
        IsHmd = 0x00000020,
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct DisplayConfigPathTargetInfo
    {
        public Luid AdapterId;
        public uint Id;
        public DisplayConfigPathTargetMode modeInfoIdx;
        public DisplayConfigVideoOutputTechnology outputTechnology;
        public DisplayConfigRotation rotation;
        public DisplayConfigScaling scaling;
        public DisplayConfigRational refreshRate;
        public ScanLineOrdering scanLineOrdering;
        [MarshalAs(UnmanagedType.Bool)] public bool targetAvailable;
        public DisplayConfigTargetStatus statusFlags;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 4)]
    public struct DisplayConfigPathTargetMode
    {
        [FieldOffset(0)]
        public uint modeInfoIdx;
        [FieldOffset(0)]
        public DisplayConfigPathTargetModeInfo ModeInfo;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct DisplayConfigPathTargetModeInfo
    {
        public uint value;

        public readonly ushort DesktopModeInfoIdx => (ushort)(value & 0xFFFF);

        public readonly ushort TargetModeInfoIdx => (ushort)((value >> 16) & 0xFFFF);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct DisplayConfigModeInfo
    {
        public DisplayConfigModeInfoType InfoType;
        public uint Id;
        public Luid AdapterId;
        public DisplayConfigModeInfoAdditional AdditionalModeInfo;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 4)]
    public struct DisplayConfigModeInfoAdditional
    {
        [FieldOffset(0)]
        public DisplayConfigTargetMode TargetMode;
        [FieldOffset(0)]
        public DisplayConfigSourceMode SourceMode;
        [FieldOffset(0)]
        public DisplayConfigDesktopImageInfo DesktopImageInfo;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct DisplayConfigDesktopImageInfo
    {
        public PointInt4Bytes PathSourceSize;
        public RectInt4Bytes DesktopImageRegion;
        public RectInt4Bytes DesktopImageClip;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct DisplayConfigTargetMode
    {
        public DisplayConfigVideoSignalInfo targetVideoSignalInfo;
    }

    public enum D3dKmdtVideoSignalStandard : uint
    {
        Uninitialized = 0,
        VesaDmt = 1,
        VesaGtf = 2,
        VesaCvt = 3,
        Ibm = 4,
        Apple = 5,
        NtscM = 6,
        NtscJ = 7,
        Ntsc443 = 8,
        PalB = 9,
        PalB1 = 10,
        PalG = 11,
        PalH = 12,
        PalI = 13,
        PalD = 14,
        PalN = 15,
        PalNC = 16,
        Secam = 17,
        SecamB = 18,
        SecamD = 19,
        SecamG = 20,
        SecamH = 21,
        SecamK = 22,
        SecamK1 = 23,
        SecamL = 24,
        SecamL1 = 25,
        EIA861 = 26,
        EIA861A = 27,
        EIA861B = 28,
        PalK = 29,
        PalK1 = 30,
        PalL = 31,
        PalM = 32,
        Other = 255,
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct DisplayConfigVideoSignalInfo
    {
        public ulong pixelRate;
        public DisplayConfigRational hSyncFreq;
        public DisplayConfigRational vSyncFreq;
        public DisplayConfig2DRegion activeSize;
        public DisplayConfig2DRegion totalSize;
        public DisplayConfigVideoSignalAdditional AdditionalSignalInfo;
        public ScanLineOrdering scanLineOrdering;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 4)]
    public struct DisplayConfigVideoSignalAdditional
    {
        [FieldOffset(0)]
        public D3dKmdtVideoSignalStandard videoStandard;
        [FieldOffset(0)]
        public DisplayConfigVideoSignalStandardAdditional AdditionalVideoStandard;

    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct DisplayConfigVideoSignalStandardAdditional
    {
        public uint value;
        public readonly ushort VideoStandard => (ushort)(value & 0xFFFF);
        public readonly byte VSyncFreqDivider => (byte)((value >> 16) & 0x3F);
        public readonly ushort Reserved => (ushort)((value >> 22) & 0x3FF);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct DisplayConfigRational
    {
        public uint Numerator;
        public uint Denominator;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct DisplayConfig2DRegion
    {
        public uint cx;
        public uint cy;
    }

    public enum DisplayConfigPixelFormat : uint
    {
        PF8Bpp = 1,
        PF16Bpp = 2,
        PF24Bpp = 3,
        PF32Bpp = 4,
        NonGdi = 5,
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct PointInt4Bytes(int x, int y)
    {
        public int x = x;
        public int y = y;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct DisplayConfigSourceMode
    {
        public uint Width;
        public uint Height;
        public DisplayConfigPixelFormat PixelFormat;
        public PointInt4Bytes Position;
    }

    public enum Dcpi : uint
    {
        Active = 0x00000001,
        SupportVirtualMode = 0x00000008,
        BoostRefreshRate = 0x00000010,
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct DisplayConfigPathInfo
    {
        public DisplayConfigPathSourceInfo sourceInfo;
        public DisplayConfigPathTargetInfo targetInfo;
        public Dcpi flags;
    }

    [DllImport(User32)]
    internal static extern int GetDisplayConfigBufferSizes(QueryDisplayConfigs flags, out uint numPathArrayElements, out uint numModeInfoArrayElements);

    [DllImport(User32)]
    internal static extern int QueryDisplayConfig(QueryDisplayConfigs flags,
        ref uint numPathArrayElements,
        [In(), Out()] DisplayConfigPathInfo[] pathInfoArray,
        ref uint numModeInfoArrayElements,
        [In(), Out()] DisplayConfigModeInfo[] modeInfoArray,
        out SystemInformation.DisplayConfigTopologyIds currentTopologyId);

    [DllImport(User32)]
    internal static extern int QueryDisplayConfig(QueryDisplayConfigs flags,
        ref uint numPathArrayElements,
        [In(), Out()] DisplayConfigPathInfo[] pathInfoArray,
        ref uint numModeInfoArrayElements,
        [In(), Out()] DisplayConfigModeInfo[] modeInfoArray);
}
