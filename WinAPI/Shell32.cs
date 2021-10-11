using System;
using System.Runtime.InteropServices;

namespace Filexplorip.WinAPI
{
    public static class Shell32
    {
        public const int MAX_PATH = 256;
        [StructLayout(LayoutKind.Sequential)]
        public struct SHITEMID
        {
            public ushort cb;
            [MarshalAs(UnmanagedType.LPArray)]
            public byte[] abID;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ITEMIDLIST
        {
            public SHITEMID mkid;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BROWSEINFO
        {
            public IntPtr hwndOwner;
            public IntPtr pidlRoot;
            public IntPtr pszDisplayName;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpszTitle;
            public uint ulFlags;
            public IntPtr lpfn;
            public int lParam;
            public IntPtr iImage;
        }

        [Flags()]
        public enum BIF : uint
        {
            RETURNONLYFSDIRS = 0x0001,
            DONTGOBELOWDOMAIN = 0x0002,
            STATUSTEXT = 0x0004,
            RETURNFSANCESTORS = 0x0008,
            EDITBOX = 0x0010,
            VALIDATE = 0x0020,
            NEWDIALOGSTYLE = 0x0040,
            USENEWUI = (NEWDIALOGSTYLE | EDITBOX),
            BROWSEINCLUDEURLS = 0x0080,
            BROWSEFORCOMPUTER = 0x1000,
            BROWSEFORPRINTER = 0x2000,
            BROWSEINCLUDEFILES = 0x4000,
            SHAREABLE = 0x8000,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SHFILEINFO
        {
            public const int NAMESIZE = 80;
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = NAMESIZE)]
            public string szTypeName;
        };

        public const uint FILE_ATTRIBUTE_DIRECTORY = 0x00000010;
        public const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;

        [DllImport("Shell32.dll")]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, SHGFI uFlags);

        [DllImport("Shell32.dll")]
        public static extern IntPtr SHGetFileInfo(IntPtr pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, SHGFI uFlags);

        [DllImport("shell32.dll")]
        public static extern int SHGetDesktopFolder(out IntPtr ppshf);

        public enum CSIDL
        {
            DESKTOP = 0x0000,    // <desktop>
            INTERNET = 0x0001,    // Internet Explorer (icon on desktop)
            PROGRAMS = 0x0002,    // Start Menu\Programs
            CONTROLS = 0x0003,    // My Computer\Control Panel
            PRINTERS = 0x0004,    // My Computer\Printers
            PERSONAL = 0x0005,    // My Documents
            FAVORITES = 0x0006,    // <user name>\Favorites
            STARTUP = 0x0007,    // Start Menu\Programs\Startup
            RECENT = 0x0008,    // <user name>\Recent
            SENDTO = 0x0009,    // <user name>\SendTo
            BITBUCKET = 0x000a,    // <desktop>\Recycle Bin
            STARTMENU = 0x000b,    // <user name>\Start Menu
            MYDOCUMENTS = 0x000c,    // logical "My Documents" desktop icon
            MYMUSIC = 0x000d,    // "My Music" folder
            MYVIDEO = 0x000e,    // "My Videos" folder
            DESKTOPDIRECTORY = 0x0010,    // <user name>\Desktop
            DRIVES = 0x0011,    // My Computer
            NETWORK = 0x0012,    // Network Neighborhood (My Network Places)
            NETHOOD = 0x0013,    // <user name>\nethood
            FONTS = 0x0014,    // windows\fonts
            TEMPLATES = 0x0015,
            COMMON_STARTMENU = 0x0016,    // All Users\Start Menu
            COMMON_PROGRAMS = 0X0017,    // All Users\Start Menu\Programs
            COMMON_STARTUP = 0x0018,    // All Users\Startup
            COMMON_DESKTOPDIRECTORY = 0x0019,    // All Users\Desktop
            APPDATA = 0x001a,    // <user name>\Application Data
            PRINTHOOD = 0x001b,    // <user name>\PrintHood
            LOCAL_APPDATA = 0x001c,    // <user name>\Local Settings\Applicaiton Data (non roaming)
            ALTSTARTUP = 0x001d,    // non localized startup
            COMMON_ALTSTARTUP = 0x001e,    // non localized common startup
            COMMON_FAVORITES = 0x001f,
            INTERNET_CACHE = 0x0020,
            COOKIES = 0x0021,
            HISTORY = 0x0022,
            COMMON_APPDATA = 0x0023,    // All Users\Application Data
            WINDOWS = 0x0024,    // GetWindowsDirectory()
            SYSTEM = 0x0025,    // GetSystemDirectory()
            PROGRAM_FILES = 0x0026,    // C:\Program Files
            MYPICTURES = 0x0027,    // C:\Program Files\My Pictures
            PROFILE = 0x0028,    // USERPROFILE
            SYSTEMX86 = 0x0029,    // x86 system directory on RISC
            PROGRAM_FILESX86 = 0x002a,    // x86 C:\Program Files on RISC
            PROGRAM_FILES_COMMON = 0x002b,    // C:\Program Files\Common
            PROGRAM_FILES_COMMONX86 = 0x002c,    // x86 Program Files\Common on RISC
            COMMON_TEMPLATES = 0x002d,    // All Users\Templates
            COMMON_DOCUMENTS = 0x002e,    // All Users\Documents
            COMMON_ADMINTOOLS = 0x002f,    // All Users\Start Menu\Programs\Administrative Tools
            ADMINTOOLS = 0x0030,    // <user name>\Start Menu\Programs\Administrative Tools
            CONNECTIONS = 0x0031,    // Network and Dial-up Connections
            COMMON_MUSIC = 0x0035,    // All Users\My Music
            COMMON_PICTURES = 0x0036,    // All Users\My Pictures
            COMMON_VIDEO = 0x0037,    // All Users\My Video
            CDBURN_AREA = 0x003b    // USERPROFILE\Local Settings\Application Data\Microsoft\CD Burning
        }

        [Flags()]
        public enum SHGFI : uint
        {
            ADDOVERLAYS = 0x000000020,
            ATTR_SPECIFIED = 0x000020000,
            ATTRIBUTES = 0x000000800,
            DISPLAYNAME = 0x000000200,
            EXETYPE = 0x000002000,
            ICON = 0x000000100,
            ICONLOCATION = 0x000001000,
            LARGEICON = 0x000000000,
            LINKOVERLAY = 0x000008000,
            OPENICON = 0x000000002,
            OVERLAYINDEX = 0x000000040,
            PIDL = 0x000000008,
            SELECTED = 0x000010000,
            SHELLICONSIZE = 0x000000004,
            SMALLICON = 0x000000001,
            SYSICONINDEX = 0x000004000,
            TYPENAME = 0x000000400,
            USEFILEATTRIBUTES = 0x000000010,
        }

        [DllImport("shell32.dll", SetLastError = true)]
        public static extern int SHGetSpecialFolderLocation(IntPtr hwndOwner, CSIDL nFolder, ref IntPtr ppidl);
    }
}
