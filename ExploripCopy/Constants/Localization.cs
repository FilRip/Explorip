using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ExploripCopy.Constants
{
    internal static class Localization
    {
        internal static string COPY_OF { get; private set; }

        internal static void LoadTranslation()
        {
            COPY_OF = Load("shell32.dll", 4178, " - Copy");
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
