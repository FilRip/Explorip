using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace ExploripCopy.Constants;

public static class Localization
{
    private const string Shell32Dll = "shell32.dll";
    private const string DskQuotaDll = "dskquota.dll";
    private const string appWizCpl = "appwiz.cpl";
    private const string ParamFilename = "%1!ls!";
    private const string ParamDouble = "%1!d!";

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
    public static string QUIT { get; private set; }
    public static string ERROR { get; private set; }
    public static string IN_PROGRESS { get; private set; }
    public static string CANCELED { get; private set; }
    public static string REMOVE { get; private set; }
    public static string PAUSE { get; private set; }
    public static string STOP { get; private set; }
    public static string OK { get; private set; }
    public static string CREATE_FOLDER { get; private set; }
    public static string CREATE_SHORTCUT { get; private set; }
    public static string CREATE_SHORTCUT_Q1 { get; private set; }
    public static string CREATE_SHORTCUT_Q2 { get; private set; }
    public static string BROWSE { get; private set; }
    public static string LOCATE { get; private set; }
    public static string SHOW_NOTIFICATION { get; private set; }
    public static string SHOW_MAIN_WINDOW { get; private set; }
    public static string NOT_ENOUGH_FREE_SPACE { get; private set; }
    public static string SIZE { get; private set; }
    public static string FROM { get; private set; }
    public static string TO { get; private set; }
    public static string START { get; private set; }
    public static string WORK_IN_PROGRESS { get; private set; }
    public static string TIME_REMAINING { get; private set; }
    public static string TIME_REMAINING_DAY { get; private set; }
    public static string TIME_REMAINING_DAYS { get; private set; }
    public static string TIME_REMAINING_HOUR { get; private set; }
    public static string TIME_REMAINING_HOURS { get; private set; }
    public static string TIME_REMAINING_MINUTES_SECONDS { get; private set; }
    public static string TIME_REMAINING_MINUTE_SECONDS { get; private set; }
    public static string TIME_REMAINING_SECONDS { get; private set; }

    internal static void LoadTranslation()
    {
        COPY_OF = Load(Shell32Dll, 4178, " - Copy");
        FILE_COLLISION_TITLE = Load(Shell32Dll, 33163, "Replace or ignore files");
        FILE_NAME_EXIST = Load(Shell32Dll, 33234, "Destination already contain a file named \" %s \"").Replace("%1!s!", "%s");
        REPLACE_FILE = Load(Shell32Dll, 33237, "Replace all files in destination");
        IGNORE_FILE = Load(Shell32Dll, 33239, "Ignore all files");
        IGNORE_FILE_SAME_DATE_SIZE = Load(Shell32Dll, 33197, "Ignore files with same date and size").Replace("%1!lu!", "");
        CANCEL = Load(Shell32Dll, 33187, "Cancel");
        CONTINUE = Load(Shell32Dll, 33188, "Continue");
        SPEED_COPY = Load(Shell32Dll, 33200, "Speed %1!s!/s").Replace("%1!s!", "%s");
        SPEED_BYTE = Load(DskQuotaDll, 14472, "bytes");
        SPEED_KILO = Load(DskQuotaDll, 14473, "kilo");
        SPEED_MEGA = Load(DskQuotaDll, 14474, "mega");
        SPEED_GIGA = Load(DskQuotaDll, 14475, "giga");
        SPEED_TERA = Load(DskQuotaDll, 14476, "tera");
        SPEED_PETA = Load(DskQuotaDll, 14477, "peta");
        SPEED_EXA = Load(DskQuotaDll, 14478, "exa");
        AVERAGE = Load("comres.dll", 2705, "average");
        MOVE_OF_FILESYSTEM = Load(Shell32Dll, 4193, "Move of '%1!ls!'").Replace(ParamFilename, "%s");
        COPY_OF_FILESYSTEM = Load(Shell32Dll, 4194, "Copy of '%1!ls!'").Replace(ParamFilename, "%s");
        DELETE_OF_FILESYSTEM = Load(Shell32Dll, 4195, "Delete of %1!ls!").Replace(ParamFilename, "%s");
        RENAME_OF_FILESYSTEM = Load(Shell32Dll, 4196, "Rename of %1!ls!").Replace(ParamFilename, "%s").Replace("%2!ls!", "%s2");
        CREATE_OF_FILESYSTEM = Load(Shell32Dll, 4199, "Create of %2!ls!").Replace("%2!ls!", "%s2");
        CALCUL = Load(Shell32Dll, 13580, "Calculate...");
        REMANING = Load(Shell32Dll, 33221, "Remaining items...");
        TOTAL = Load(Shell32Dll, 9306, "Total size") + " %s";
        FINISH = Load(Shell32Dll, 51249, "Finished");
        QUIT = Load("dinput.dll", 5268, "Exit");
        ERROR = Load(Shell32Dll, 51248, "Error");
        IN_PROGRESS = Load(Shell32Dll, 32908, "Treatment in progress");
        CANCELED = Load(Shell32Dll, 51256, "Canceled");
        REMOVE = Load(Shell32Dll, 33230, "Remove");
        PAUSE = Load("dinput.dll", 709, "Pause");
        STOP = Load("dinput.dll", 661, "Stop");
        OK = Load(Shell32Dll, 33225, "Ok");
        CREATE_FOLDER = Load(Shell32Dll, 31237, "Create a new folder").Replace(".", "");
        CREATE_SHORTCUT = Load(appWizCpl, 2200, "Create a shortcut");
        CREATE_SHORTCUT_Q1 = Load(appWizCpl, 2201, "On which element do you want to create shortcut ?");
        BROWSE = Load(Shell32Dll, 9015, "Browse");
        LOCATE = Load(appWizCpl, 12808, "Location");
        CREATE_SHORTCUT_Q2 = Load(appWizCpl, 2203, "Which name do you want to use for this shortcut ?");
        SHOW_NOTIFICATION = Load("twinui.dll", 5592, "Show notifications");
        SHOW_MAIN_WINDOW = Load("oleaccrc.dll", 146, "Show main window");
        NOT_ENOUGH_FREE_SPACE = Load(Shell32Dll, 16915, "Not enough free space in destination drive");
        SIZE = Load(Shell32Dll, 8978, "Size");
        FROM = Load(Shell32Dll, 13577, "From").Trim();
        TO = Load(Shell32Dll, 13578, "To").Trim();
        START = Load(Shell32Dll, 22073, "Start");
        WORK_IN_PROGRESS = Load("explorerframe.dll", 41480, "Work in progress...");
        TIME_REMAINING = Load(Shell32Dll, 33222, "Time remaining :");
        TIME_REMAINING_DAY = Load(Shell32Dll, 32928, "%1!d! day");
        TIME_REMAINING_DAYS = Load(Shell32Dll, 32929, "%1!d! days");
        TIME_REMAINING_HOUR = Load(Shell32Dll, 32930, "1 hour");
        TIME_REMAINING_HOURS = Load(Shell32Dll, 32931, "%1!d! hours").Replace(ParamDouble, "%h");
        TIME_REMAINING_MINUTES_SECONDS = Load(Shell32Dll, 32937, "%1!d! minutes and %2!d! seconds").Replace(ParamDouble, "%m").Replace("%2!d!", "%s");
        TIME_REMAINING_MINUTE_SECONDS = Load(Shell32Dll, 32939, "%1!d! minute and %2!d! seconds").Replace(ParamDouble, "%m").Replace("%2!d!", "%s");
        TIME_REMAINING_SECONDS = Load(Shell32Dll, 32941, "%1!d! seconds").Replace(ParamDouble, "%s");
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
            Debug.WriteLine($"Error, unable to find StringResource={Ident} in {libraryName}");
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
