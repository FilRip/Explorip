using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

using WpfScreenHelper.Enum;

namespace WpfScreenHelper;

/// <summary>
/// Represents a display device or multiple display devices on a single system.
/// </summary>
public class Screen
{
    /// <summary>
    /// Indicates if we have more than one monitor.
    /// </summary>
    private static bool MultiMonitorSupport = IsMultiMonitorSupport();
    private static IEnumerable<Screen> _listScreens;
    private static int _lastId = 0;

    // This identifier is just for us, so that we don't try to call the multimon
    // functions if we just need the primary monitor... this is safer for
    // non-multimon OSes.
    private const int PRIMARY_MONITOR = unchecked((int)0xBAADF00D);

    public static void ResetMultiMonitorSupport()
    {
        MultiMonitorSupport = IsMultiMonitorSupport();
    }

    private static bool IsMultiMonitorSupport()
    {
        return NativeMethods.GetSystemMetrics(NativeMethods.SystemMetric.SM_CMONITORS) != 0;
    }

    /// <summary>
    /// The monitor handle.
    /// </summary>
    private readonly IntPtr monitorHandle;

    /// <summary>
    /// Initializes a new instance of the <see cref="Screen"/> class.
    /// </summary>
    /// <param name="monitor">The monitor.</param>
    private Screen(IntPtr monitor)
    {
        if (NativeMethods.IsProcessDPIAware())
        {
            uint dpiX;

            try
            {
                if (monitor == (IntPtr)PRIMARY_MONITOR)
                {
                    IntPtr ptr = NativeMethods.MonitorFromPoint(new NativeMethods.PointStruct(0, 0), NativeMethods.MonitorDefault.MONITOR_DEFAULTTOPRIMARY);
                    NativeMethods.GetDpiForMonitor(ptr, NativeMethods.DpiType.EFFECTIVE, out dpiX, out _);
                }
                else
                    NativeMethods.GetDpiForMonitor(monitor, NativeMethods.DpiType.EFFECTIVE, out dpiX, out _);
            }
            catch
            {
                // Windows 7 fallback
                int hr = NativeMethods.D2D1CreateFactory(NativeMethods.D2D1FactoryType.D2D1_FACTORY_TYPE_SINGLE_THREADED, typeof(NativeMethods.ID2D1Factory).GUID, IntPtr.Zero, out NativeMethods.ID2D1Factory factory);
                if (hr < 0)
                    dpiX = 96;
                else
                {
                    factory.GetDesktopDpi(out float x, out _);
                    Marshal.ReleaseComObject(factory);
                    dpiX = (uint)x;
                }
            }

            ScaleFactor = dpiX / 96.0;
        }

        NativeMethods.DisplayDevice dd = new();
        bool deviceOk;
        if (!MultiMonitorSupport || monitor == (IntPtr)PRIMARY_MONITOR)
        {
            Size size = new(
                NativeMethods.GetSystemMetrics(NativeMethods.SystemMetric.SM_CXSCREEN),
                NativeMethods.GetSystemMetrics(NativeMethods.SystemMetric.SM_CYSCREEN));

            Bounds = new Rect(0, 0, size.Width, size.Height);
            Primary = true;
            DeviceName = "\\\\.\\DISPLAY1";
            deviceOk = NativeMethods.EnumDisplayDevices(null, 0, ref dd, NativeMethods.Edd.GET_DEVICE_INTERFACE_NAME);
        }
        else
        {
            NativeMethods.MonitorInfoEx info = new();

            NativeMethods.GetMonitorInfo(new HandleRef(null, monitor), info);

            Bounds = new Rect(
                info.rcMonitor.left,
                info.rcMonitor.top,
                info.rcMonitor.right - info.rcMonitor.left,
                info.rcMonitor.bottom - info.rcMonitor.top);
            Primary = info.dwFlags.HasFlag(NativeMethods.EMonitorInfos.Primary);
            DeviceName = info.szDevice;
            deviceOk = NativeMethods.EnumDisplayDevices(DeviceName, 0, ref dd, NativeMethods.Edd.GET_DEVICE_INTERFACE_NAME);
        }

        if (deviceOk)
        {
            Id = dd.Id;
            RegistryKeyEntry = dd.Key;
            Description = dd.String;
            InternalName = dd.Name;
            DisplayDevice = dd.StateFlags;
            DisplayNumber = GetLastId();
        }

        monitorHandle = monitor;
    }

    private static int GetLastId()
    {
        _lastId++;
        return _lastId;
    }

    private static void ResetLastId()
    {
        _lastId = 0;
    }

    /// <summary>
    /// Gets an array of all displays on the system.
    /// </summary>
    /// <returns>An enumerable of type Screen, containing all displays on the system.</returns>
    public static IEnumerable<Screen> AllScreens
    {
        get
        {
            if (_listScreens == null)
            {
                ResetLastId();
                if (MultiMonitorSupport)
                {
                    MonitorEnumCallback closure = new();
                    NativeMethods.MonitorEnumProc proc = new(closure.Callback);
                    NativeMethods.EnumDisplayMonitors(NativeMethods.NullHandleRef, null, proc, IntPtr.Zero);
                    if (closure.Screens.Count > 0)
                        _listScreens = closure.Screens.Cast<Screen>();
                }
                else
                    _listScreens = [new Screen((IntPtr)PRIMARY_MONITOR)];
            }
            return _listScreens;
        }
    }

    public static void ForceRefreshListScreens()
    {
        ResetMultiMonitorSupport();
        _listScreens = null;
        _ = AllScreens;
        ResetLastId();
    }

    /// <summary>
    /// Gets the primary display.
    /// </summary>
    /// <returns>The primary display.</returns>
    public static Screen PrimaryScreen
    {
        get
        {
            return MultiMonitorSupport ? AllScreens.FirstOrDefault(t => t.Primary) : new Screen((IntPtr)PRIMARY_MONITOR);
        }
    }

    /// <summary>
    /// Gets the bounds of the display in units.
    /// </summary>
    /// <returns>A <see cref="T:System.Windows.Rect" />, representing the bounds of the display in units.</returns>
    public Rect WpfBounds =>
        ScaleFactor.Equals(1.0)
            ? Bounds
            : new Rect(
                Bounds.X / ScaleFactor,
                Bounds.Y / ScaleFactor,
                Bounds.Width / ScaleFactor,
                Bounds.Height / ScaleFactor);

    /// <summary>
    /// Gets the device name associated with a display.
    /// </summary>
    /// <returns>The device name associated with a display.</returns>
    public string DeviceName { get; }

    /// <summary>
    /// Gets the bounds of the display in pixels.
    /// </summary>
    /// <returns>A <see cref="T:System.Windows.Rect" />, representing the bounds of the display in pixels.</returns>
    public Rect Bounds { get; }

    /// <summary>
    /// Gets a value indicating whether a particular display is the primary device.
    /// </summary>
    /// <returns>true if this display is primary; otherwise, false.</returns>
    public bool Primary { get; }

    /// <summary>
    /// Gets the scale factor of the display.
    /// </summary>
    /// <returns>The scale factor of the display.</returns>
    public double ScaleFactor { get; } = 1.0;

    /// <summary>
    /// Id of the monitor
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Entry, in the Windows registry, for this display
    /// </summary>
    public string RegistryKeyEntry { get; }

    /// <summary>
    /// Description of this monitor give us by drivers
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Internal name, for IO
    /// </summary>
    public string InternalName { get; }

    /// <summary>
    /// Some flags for this display
    /// </summary>
    public EDisplayDevice DisplayDevice { get; }

    /// <summary>
    /// Unique identification of monitor
    /// </summary>
    public int DisplayNumber { get; }

    /// <summary>
    /// Return the pointer to the monitor
    /// </summary>
    public IntPtr MonitorHandle
    {
        get { return monitorHandle; }
    }

    /// <summary>
    /// Gets the working area of the display. The working area is the desktop area of the display, excluding task bars,
    /// docked windows, and docked tool bars in pixels.
    /// </summary>
    /// <returns>A <see cref="T:System.Windows.Rect" />, representing the working area of the display in pixels.</returns>
    public Rect WorkingArea
    {
        get
        {
            Rect workingArea;

            if (!MultiMonitorSupport || monitorHandle == (IntPtr)PRIMARY_MONITOR)
            {
                NativeMethods.Rect rc = new();

                NativeMethods.SystemParametersInfo(NativeMethods.Spi.SPI_GETWORKAREA, 0, ref rc, NativeMethods.Spifs.SPIF_SENDCHANGE);

                workingArea = new Rect(rc.left, rc.top, rc.right - rc.left, rc.bottom - rc.top);
            }
            else
            {
                NativeMethods.MonitorInfoEx info = new();
                NativeMethods.GetMonitorInfo(new HandleRef(null, monitorHandle), info);

                workingArea = new Rect(info.rcWork.left, info.rcWork.top, info.rcWork.right - info.rcWork.left, info.rcWork.bottom - info.rcWork.top);
            }

            return workingArea;
        }
    }

    /// <summary>
    /// Gets the working area of the display. The working area is the desktop area of the display, excluding task bars,
    /// docked windows, and docked tool bars in units.
    /// </summary>
    /// <returns>A <see cref="T:System.Windows.Rect" />, representing the working area of the display in units.</returns>
    public Rect WpfWorkingArea =>
        ScaleFactor.Equals(1.0)
            ? WorkingArea
            : new Rect(
                WorkingArea.X / ScaleFactor,
                WorkingArea.Y / ScaleFactor,
                WorkingArea.Width / ScaleFactor,
                WorkingArea.Height / ScaleFactor);

    /// <summary>
    /// Retrieves a Screen for the display that contains the largest portion of the specified control.
    /// </summary>
    /// <param name="hwnd">The window handle for which to retrieve the Screen.</param>
    /// <returns>
    /// A Screen for the display that contains the largest region of the object. In multiple display environments
    /// where no display contains any portion of the specified window, the display closest to the object is returned.
    /// </returns>
    public static Screen FromHandle(IntPtr hwnd)
    {
        return MultiMonitorSupport
                   ? new Screen(NativeMethods.MonitorFromWindow(new HandleRef(null, hwnd), 2))
                   : new Screen((IntPtr)PRIMARY_MONITOR);
    }

    /// <summary>
    /// Retrieves a Screen for the display that contains the specified point in pixels.
    /// </summary>
    /// <param name="point">A <see cref="T:System.Windows.Point" /> that specifies the location for which to retrieve a Screen.</param>
    /// <returns>
    /// A Screen for the display that contains the point in pixels. In multiple display environments where no display contains
    /// the point, the display closest to the specified point is returned.
    /// </returns>
    public static Screen FromPoint(Point point)
    {
        if (MultiMonitorSupport)
        {
            NativeMethods.PointStruct pt = new((int)point.X, (int)point.Y);
            IntPtr screenHandle = NativeMethods.MonitorFromPoint(pt, NativeMethods.MonitorDefault.MONITOR_DEFAULTTONEAREST);
            return AllScreens.SingleOrDefault(s => s.MonitorHandle == screenHandle);
        }

        return new Screen((IntPtr)PRIMARY_MONITOR);
    }

    /// <summary>
    /// Retrieves a Screen for the display that contains the largest portion of the specified control.
    /// </summary>
    /// <param name="window">The window for which to retrieve the Screen.</param>
    /// <returns>
    /// A Screen for the display that contains the largest region of the object. In multiple display environments
    /// where no display contains any portion of the specified window, the display closest to the object is returned.
    /// </returns>
    public static Screen FromWindow(Window window)
    {
        return FromHandle(new WindowInteropHelper(window).EnsureHandle());
    }

    /// <summary>
    /// Gets or sets a value indicating whether the specified object is equal to this Screen.
    /// </summary>
    /// <param name="obj">The object to compare to this Screen.</param>
    /// <returns>true if the specified object is equal to this Screen; otherwise, false.</returns>
    public override bool Equals(object obj)
    {
        if (obj is Screen monitor && monitorHandle == monitor.monitorHandle)
            return true;

        return false;
    }

    /// <summary>
    /// Computes and retrieves a hash code for an object.
    /// </summary>
    /// <returns>A hash code for an object.</returns>
    public override int GetHashCode()
    {
        return this.monitorHandle.GetHashCode();
    }

    /// <summary>
    /// The monitor enum callback.
    /// </summary>
    private sealed class MonitorEnumCallback
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MonitorEnumCallback"/> class.
        /// </summary>
        public MonitorEnumCallback()
        {
            this.Screens = [];
        }

        /// <summary>
        /// Gets the screens.
        /// </summary>
        public ArrayList Screens { get; }

#pragma warning disable IDE0060, S1172 // Unused method parameters should be removed
        public bool Callback(IntPtr monitor, IntPtr hdc, IntPtr lprcMonitor, IntPtr lparam)
        {
            this.Screens.Add(new Screen(monitor));
            return true;
        }
#pragma warning restore IDE0060, S1172 // Unused method parameters should be removed
    }
}
