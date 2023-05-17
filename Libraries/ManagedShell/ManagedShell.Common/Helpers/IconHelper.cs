using System;
using System.IO;
using System.Runtime.InteropServices;

using ManagedShell.Common.Enums;
using ManagedShell.Common.Logging;
using ManagedShell.Interop;

using static ManagedShell.Interop.NativeMethods;

namespace ManagedShell.Common.Helpers
{
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

        public static IntPtr GetIconByFilename(string fileName, IconSize size)
        {
            return GetIcon(fileName, size);
        }

        public static IntPtr GetIconByPidl(IntPtr pidl, IconSize size)
        {
            return GetIcon(pidl, size);
        }

        private static IntPtr GetIcon(string filename, IconSize size)
        {
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

                    if (!filename.StartsWith("\\") && (File.GetAttributes(filename) & FileAttributes.Directory) == FileAttributes.Directory)
                    {
                        SHGetFileInfo(filename, FILE_ATTRIBUTE_NORMAL | FILE_ATTRIBUTE_DIRECTORY, ref shinfo, (uint)Marshal.SizeOf(shinfo), (uint)(SHGFI.SysIconIndex));
                    }
                    else
                    {
                        SHGetFileInfo(filename, FILE_ATTRIBUTE_NORMAL, ref shinfo, (uint)Marshal.SizeOf(shinfo), (uint)(SHGFI.UseFileAttributes | SHGFI.SysIconIndex));
                    }

                    return GetFromImageList(shinfo.iIcon, size);
                }
                catch
                {
                    return IntPtr.Zero;
                }
            }
        }

        private static IntPtr GetIcon(IntPtr pidl, IconSize size)
        {
            lock (ComLock)
            {
                try
                {
                    ShFileInfo shinfo = new()
                    {
                        szDisplayName = string.Empty,
                        szTypeName = string.Empty
                    };

                    SHGetFileInfo(pidl, FILE_ATTRIBUTE_NORMAL, ref shinfo, (uint)Marshal.SizeOf(shinfo), (uint)(SHGFI.PIDL | SHGFI.SysIconIndex));

                    return GetFromImageList(shinfo.iIcon, size);
                }
                catch
                {
                    return IntPtr.Zero;
                }
            }
        }

        private static IntPtr GetFromImageList(int iconIndex, IconSize size)
        {
            // Initialize the IImageList object
            InitIml(size);

            IntPtr hIcon = IntPtr.Zero;
            int ILD_TRANSPARENT = 1;

            switch (size)
            {
                case IconSize.Large:
                    imlLarge.GetIcon(iconIndex, ILD_TRANSPARENT, ref hIcon);
                    break;
                case IconSize.Small:
                    imlSmall.GetIcon(iconIndex, ILD_TRANSPARENT, ref hIcon);
                    break;
                case IconSize.ExtraLarge:
                    imlExtraLarge.GetIcon(iconIndex, ILD_TRANSPARENT, ref hIcon);
                    break;
                case IconSize.Jumbo:
                    imlJumbo.GetIcon(iconIndex, ILD_TRANSPARENT, ref hIcon);
                    break;
            }

            return hIcon;
        }

        private static string TranslateIconExceptions(string filename)
        {
            if (filename.EndsWith(".settingcontent-ms"))
            {
                return "C:\\Windows\\ImmersiveControlPanel\\SystemSettings.exe";
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
}
