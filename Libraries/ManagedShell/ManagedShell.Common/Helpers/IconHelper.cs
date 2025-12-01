using System;
using System.IO;
using System.Runtime.InteropServices;

using ManagedShell.Common.Enums;
using ManagedShell.Common.Logging;
using ManagedShell.Interop;

using static ManagedShell.Interop.NativeMethods;

namespace ManagedShell.Common.Helpers;

public static class IconHelper
{
    public static readonly ComTaskScheduler IconScheduler = new();
    public static readonly object ComLock = new();

    // IImageList references
    private static Guid iidImageList = new("46EB5926-582E-4017-9FDF-E8998DAA0950");
    private static IImageList imlLarge; // 32pt
    private static IImageList imlSmall; // 16pt
    private static IImageList imlExtraLarge; // 48pt
    private static IImageList imlJumbo; // 256pt

    private static void InitIml(IconSize size)
    {
        // Initialize the appropriate IImageList for the desired icon size if it hasn't been already

        if (size == IconSize.Large && imlLarge == null)
        {
            SHGetImageList((int)size, ref iidImageList, out imlLarge);
        }
        else if (size == IconSize.Small && imlSmall == null)
        {
            SHGetImageList((int)size, ref iidImageList, out imlSmall);
        }
        else if (size == IconSize.ExtraLarge && imlExtraLarge == null)
        {
            SHGetImageList((int)size, ref iidImageList, out imlExtraLarge);
        }
        else if (size == IconSize.Jumbo && imlJumbo == null)
        {
            SHGetImageList((int)size, ref iidImageList, out imlJumbo);
        }
        else if (size != IconSize.Small && size != IconSize.Large && size != IconSize.ExtraLarge && size != IconSize.Jumbo)
        {
            ShellLogger.Error($"IconHelper: Unsupported icon size {size}");
        }
    }

    public static void DisposeIml()
    {
        // Dispose any IImageList objects we instantiated.
        // Called by the main shutdown method.

        lock (ComLock)
        {
            if (imlLarge != null)
            {
                Marshal.ReleaseComObject(imlLarge);
                imlLarge = null;
            }
            if (imlSmall != null)
            {
                Marshal.ReleaseComObject(imlSmall);
                imlSmall = null;
            }
            if (imlExtraLarge != null)
            {
                Marshal.ReleaseComObject(imlExtraLarge);
                imlExtraLarge = null;
            }
            if (imlJumbo != null)
            {
                Marshal.ReleaseComObject(imlJumbo);
                imlJumbo = null;
            }
        }
    }

    public static IntPtr GetIconByFilename(string fileName, IconSize size, out IntPtr hOverlay)
    {
        return GetIcon(fileName, size, out hOverlay);
    }

    public static IntPtr GetIconByPidl(IntPtr pidl, IconSize size, out IntPtr hOverlay)
    {
        return GetIcon(pidl, size, out hOverlay);
    }

    private static IntPtr GetIcon(string filename, IconSize size, out IntPtr hOverlay)
    {
        hOverlay = IntPtr.Zero;
        lock (ComLock)
        {
            try
            {
                filename = TranslateIconExceptions(filename);

                ShFileInfo shinfo = new()
                {
                    szDisplayName = string.Empty,
                    szTypeName = string.Empty
                };

                ShGetFileInfos flags = ShGetFileInfos.Icon | ShGetFileInfos.SysIconIndex | ShGetFileInfos.OverlayIndex;
                if (!filename.StartsWith("\\") && (File.GetAttributes(filename).HasFlag(FileAttributes.Directory)))
                {
                    SHGetFileInfo(filename, EFileAttributes.NORMAL | EFileAttributes.DIRECTORY, ref shinfo, (uint)Marshal.SizeOf(shinfo), flags);
                }
                else
                {
                    SHGetFileInfo(filename, EFileAttributes.NORMAL, ref shinfo, (uint)Marshal.SizeOf(shinfo), flags);
                }

                if (shinfo.hIcon != IntPtr.Zero)
                    DestroyIcon(shinfo.hIcon);

                return GetFromImageList(shinfo.iIcon, size, out hOverlay);
            }
            catch (Exception)
            {
                return IntPtr.Zero;
            }
        }
    }

    private static IntPtr GetIcon(IntPtr pidl, IconSize size, out IntPtr hOverlay)
    {
        hOverlay = IntPtr.Zero;
        lock (ComLock)
        {
            try
            {
                ShFileInfo shinfo = new()
                {
                    szDisplayName = string.Empty,
                    szTypeName = string.Empty
                };

                SHGetFileInfo(pidl, EFileAttributes.NORMAL, ref shinfo, (uint)Marshal.SizeOf(shinfo), ShGetFileInfos.PIDL | ShGetFileInfos.SysIconIndex | ShGetFileInfos.Icon | ShGetFileInfos.OverlayIndex);

                return GetFromImageList(shinfo.iIcon, size, out hOverlay);
            }
            catch
            {
                return IntPtr.Zero;
            }
        }
    }

    private static IntPtr GetFromImageList(int iconIndex, IconSize size, out IntPtr hOverlay)
    {
        // Initialize the IImageList object
        InitIml(size);

        IntPtr hIcon = IntPtr.Zero;
        hOverlay = IntPtr.Zero;

        int overlayIndex = 0;

        switch (size)
        {
            case IconSize.Small:
                imlSmall.GetIcon(iconIndex & 0xFFFFFF, ImageListDraws.TRANSPARENT, ref hIcon);
                if (imlSmall.GetOverlayImage(iconIndex >> 24, ref overlayIndex) == 0)
                    imlSmall.GetIcon(overlayIndex, ImageListDraws.TRANSPARENT, ref hOverlay);
                break;
            case IconSize.Large:
                imlLarge.GetIcon(iconIndex & 0xFFFFFF, ImageListDraws.TRANSPARENT, ref hIcon);
                if (imlLarge.GetOverlayImage(iconIndex >> 24, ref overlayIndex) == 0)
                    imlLarge.GetIcon(overlayIndex, ImageListDraws.TRANSPARENT, ref hOverlay);
                break;
            case IconSize.ExtraLarge:
                imlExtraLarge.GetIcon(iconIndex & 0xFFFFFF, ImageListDraws.TRANSPARENT, ref hIcon);
                if (imlExtraLarge.GetOverlayImage(iconIndex >> 24, ref overlayIndex) == 0)
                    imlExtraLarge.GetIcon(overlayIndex, ImageListDraws.TRANSPARENT, ref hOverlay);
                break;
            case IconSize.Jumbo:
                imlJumbo.GetIcon(iconIndex & 0xFFFFFF, ImageListDraws.TRANSPARENT, ref hIcon);
                if (imlJumbo.GetOverlayImage(iconIndex >> 24, ref overlayIndex) == 0)
                    imlJumbo.GetIcon(overlayIndex, ImageListDraws.TRANSPARENT, ref hOverlay);
                break;
        }

        return hIcon;
    }

    private static string TranslateIconExceptions(string filename)
    {
        if (filename.EndsWith(".settingcontent-ms"))
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\ImmersiveControlPanel\SystemSettings.exe";
        }

        return filename;
    }

    public static IconSize ParseSize(int size)
    {
        if (Enum.IsDefined(typeof(IconSize), size))
        {
            return (IconSize)size;
        }

        return IconSize.Small;
    }

    public static int GetSize(int size)
    {
        return GetSize(ParseSize(size));
    }

    public static int GetSize(IconSize size)
    {
        return size switch
        {
            IconSize.Large => 32,
            IconSize.Small => 16,
            IconSize.Medium => 24,
            IconSize.ExtraLarge => 48,
            IconSize.Jumbo => 96,
            _ => 16,
        };
    }
}
