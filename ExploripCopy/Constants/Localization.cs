﻿using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ExploripCopy.Constants
{
    public static class Localization
    {
        public static string COPY_OF { get; private set; }
        public static string FILE_COLLISION_TITLE { get; private set; }
        public static string FILE_NAME_EXIST { get; private set; }
        public static string REPLACE_FILE { get; private set; }
        public static string IGNORE_FILE { get; private set; }
        public static string IGNORE_FILE_SAME_DATE_SIZE { get; private set; }
        public static string CANCEL { get; private set; }
        public static string CONTINUE { get; private set; }
        public static string SPEED_COPY { get; private set; }
        public static string SPEED_BYTE { get; private set; }
        public static string SPEED_KILO { get; private set; }
        public static string SPEED_MEGA { get; private set; }
        public static string SPEED_GIGA { get; private set; }
        public static string SPEED_TERA { get; private set; }
        public static string SPEED_PETA { get; private set; }
        public static string SPEED_EXA { get; private set; }
        public static string AVERAGE { get; private set; }
        public static string MOVE_OF_FILESYSTEM { get; private set; }
        public static string COPY_OF_FILESYSTEM { get; private set; }
        public static string DELETE_OF_FILESYSTEM { get; private set; }
        public static string RENAME_OF_FILESYSTEM { get; private set; }
        public static string CREATE_OF_FILESYSTEM { get; private set; }
        public static string CALCUL { get; private set; }
        public static string REMANING { get; private set; }
        public static string TOTAL { get; private set; }
        public static string FINISH { get; private set; }

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
            SPEED_COPY = Load("shell32.dll", 33200, "Speed %1!s!/s").Replace("%1!s!", "%s");
            SPEED_BYTE = Load("dskquota.dll", 14472, "bytes");
            SPEED_KILO = Load("dskquota.dll", 14473, "kilo");
            SPEED_MEGA = Load("dskquota.dll", 14474, "mega");
            SPEED_GIGA = Load("dskquota.dll", 14475, "giga");
            SPEED_TERA = Load("dskquota.dll", 14476, "tera");
            SPEED_PETA = Load("dskquota.dll", 14477, "peta");
            SPEED_EXA = Load("dskquota.dll", 14478, "exa");
            AVERAGE = Load("comres.dll", 2705, "average");
            MOVE_OF_FILESYSTEM = Load("shell32.dll", 4193, "Move of '%1!ls!'").Replace("%1!ls!", "%s");
            COPY_OF_FILESYSTEM = Load("shell32.dll", 4194, "Copy of '%1!ls!'").Replace("%1!ls!", "%s");
            DELETE_OF_FILESYSTEM = Load("shell32.dll", 4195, "Delete of %1!ls!").Replace("%1!ls!", "%s");
            RENAME_OF_FILESYSTEM = Load("shell32.dll", 4196, "Rename of %1!ls!").Replace("%1!ls!", "%s");
            CREATE_OF_FILESYSTEM = Load("shell32.dll", 4199, "Create of %1!ls!").Replace("%1!ls!", "%s");
            CALCUL = Load("shell32.dll", 13580, "Calculate...");
            REMANING = Load("shell32.dll", 33221, "Remaining items...");
            TOTAL = Load("shell32.dll", 9306, "Total size") + " %s";
            FINISH = Load("shell32.dll", 51249, "Finished");
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
