using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ExploripCopy.Constants
{
    public static class Localization
    {
        public static string COPY_OF { get; private set; }
        public static string FILE_COLLISION_TITLE { get; private set; }
        public static string FILE_NAME_EXIST { get; private set; }
        public static string REPLACE_FILE { get;private set; }
        public static string IGNORE_FILE { get;private set; }
        public static string IGNORE_FILE_SAME_DATE_SIZE { get; private set; }
        public static string CANCEL { get;private set; }
        public static string CONTINUE { get;private set; }

        internal static void LoadTranslation()
        {
            COPY_OF = Load("shell32.dll", 4178, " - Copy");
            FILE_COLLISION_TITLE = Load("shell32.dll", 33163, "Replace or ignore files");
            FILE_NAME_EXIST = Load("shell32.dll", 33234, "Destination already contain a file named \" %s \"").Replace("%1!s!", "%s");
            REPLACE_FILE = Load("shell32.dll", 33237, "Replace all files in destination");
            IGNORE_FILE = Load("shell32.dll", 33239, "Ignore all files");
            IGNORE_FILE_SAME_DATE_SIZE = Load("shell32.dll", 33197, "Ignore files with same date and size").Replace("%1!lu!", "");
            CANCEL = Load("shell32.dll", 33187, "Cancel");
            CONTINUE = Load("shell32.dll", 33188, "Continue");
        }

        private static string Load(string libraryName, uint Ident, string DefaultText)
        {
            IntPtr libraryHandle = GetModuleHandle(libraryName);
            if (libraryHandle == IntPtr.Zero)
            {
                LoadLibrary(libraryName);
                libraryHandle = GetModuleHandle(libraryName);
            }
            if (libraryHandle != IntPtr.Zero)
            {
                StringBuilder sb = new(1024);
                int size = LoadString(libraryHandle, Ident, sb, 1024);
                if (size > 0)
                    return sb.ToString().Replace("&", "_");
                else
                    return DefaultText;
            }
            else
            {
                return DefaultText;
            }
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPTStr)] string lpFileName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int LoadString(IntPtr hInstance, uint uID, StringBuilder lpBuffer, int nBufferMax);
    }
}
