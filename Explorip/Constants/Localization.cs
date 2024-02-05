using System;
using System.Text;

using ManagedShell.Interop;

namespace Explorip.Constants;

public static class Localization
{
    public static string PASTE { get; private set; }
    public static string PASTE_SHORTCUT { get; private set; }
    public static string POWER_SHELL { get; private set; }
    public static string POWER_SHELL_ADMIN { get; private set; }
    public static string COMMANDLINE { get; private set; }
    public static string COMMANDLINE_ADMIN { get; private set; }
    public static string TASK_MANAGER { get; private set; }
    public static string SHOW_TITLE { get; private set; }
    public static string SMALL_ICON { get; private set; }
    public static string LARGE_ICON { get; private set; }
    public static string NEW_TOOLBAR { get; private set; }
    public static string SHOW_VISUAL_KEYBOARD { get; private set; }
    public static string CLOSE_TAB { get; private set; }
    public static string CLOSE_TAB_WITHOUT_SHORTCUT { get; private set; }
    public static string CLOSE_OTHER_TAB { get; private set; }
    public static string OPEN_NEW_TAB { get; private set; }
    public static string NO_MONITOR_CONNECTED { get; private set; }
    public static string UNLOCK_TASKBAR { get; private set; }
    public static string TOOLBAR { get; private set; }
    public static string QUIT { get; private set; }
    public static string START { get; private set; }
    public static string RESTORE { get; private set; }
    public static string MOVE { get; private set; }
    public static string MINIMIZE { get; private set; }
    public static string MAXIMIZE { get; private set; }
    public static string CLOSE { get; private set; }
    public static string ATTACH_TO_TASKBAR { get; private set; }
    public static string DETACH_FROM_TASKBAR { get; private set; }
    public static string ADJUST_DATE_TIME { get; private set; }
    public static string CANCEL { get; private set; }
    public static string OK { get; private set; }
    public static string CONTINUE { get; private set; }
    public static string CREATE_FOLDER { get; private set; }
    public static string CREATE_SHORTCUT { get; private set; }
    public static string CREATE_SHORTCUT_Q1 { get; private set; }
    public static string CREATE_SHORTCUT_Q2 { get; private set; }
    public static string BROWSE { get; private set; }
    public static string LOCATE { get; private set; }
    public static string SELECT { get; private set; }
    public static string RENAME_FOLDER { get; private set; }
    public static string RENAME_FILE { get; private set; }
    public static string NEW_NAME { get; private set; }
    public static string ERROR_DURING_RENAME { get; private set; }
    public static string ERROR { get; private set; }
    public static string SEARCH_RESULT { get; private set; }

    public static void LoadTranslation()
    {
        PASTE = Load("shell32.dll", 33562, "Paste");
        PASTE_SHORTCUT = Load("shell32.dll", 37376, "Paste shortcut");
        POWER_SHELL = Load("twinui.dll", 10928, "W_indows PowerShell");
        POWER_SHELL_ADMIN = Load("twinui.dll", 10929, "Windows PowerShell (_admin)");
        COMMANDLINE = Load("twinui.dll", 10919, "Commands prompt");
        COMMANDLINE_ADMIN = Load("twinui.dll", 10920, "Commands prompt (_admin)");
        TASK_MANAGER = Load("shell32.dll", 24743, "Tasks manager");
        SHOW_TITLE = Load("msutb.dll", 304, "Show title");
        LARGE_ICON = Load("shell32.dll", 31062, "Show large icon");
        SMALL_ICON = Load("shell32.dll", 31063, "Show small icon");
        NEW_TOOLBAR = Load("ieframe.dll", 12388, "New toolbar...");
        SHOW_VISUAL_KEYBOARD = Load("shell32.dll", 24088, "Activate or desactivate visual keyboard");
        CLOSE_TAB = Load("ieframe.dll", 18160, "Close this tab").Split((char)9)[0];
        CLOSE_TAB_WITHOUT_SHORTCUT = CLOSE_TAB.Replace("_", "");
        CLOSE_OTHER_TAB = Load("ieframe.dll", 18161, "Close all others tabs");
        OPEN_NEW_TAB = Load("ieframe.dll", 13170, "New tab");
        NO_MONITOR_CONNECTED = Load("mblctr.exe", 130, "No monitor connected");
        UNLOCK_TASKBAR = Load("shell32.dll", 24290, "Lock or unlock all taskbars");
        TOOLBAR = Load("ieframe.dll", 17986, "Toolbars");
        QUIT = Load("srh.dll", 8032, "Quit") + " Explorip";
        START = Load("shell32.dll", 22073, "Start");
        MOVE = Load("oleaccrc.dll", 120, "Move");
        MINIMIZE = Load("oleaccrc.dll", 142, "Minimize");
        MAXIMIZE = Load("oleaccrc.dll", 143, "Maximize");
        CLOSE = Load("oleaccrc.dll", 145, "Close");
        RESTORE = Load("oleaccrc.dll", 146, "Restore");
        ATTACH_TO_TASKBAR = Load("starttiledata.dll", 1009, "Attach to taskbar");
        DETACH_FROM_TASKBAR = Load("starttiledata.dll", 1010, "Detach from taskbar");
        ADJUST_DATE_TIME = Load("shell32.dll", 24135, "Modify date and hour");
        CANCEL = Load("shell32.dll", 33228, "Cancel");
        CONTINUE = Load("shell32.dll", 33229, "Continue").Replace("_", "");
        OK = Load("shell32.dll", 33225, "Ok");
        CREATE_FOLDER = Load("shell32.dll", 31237, "Create a new folder").Replace(".", "");
        CREATE_SHORTCUT = Load("appwiz.cpl", 2200, "Create a shortcut");
        CREATE_SHORTCUT_Q1 = Load("appwiz.cpl", 2201, "On which element do you want to create shortcut ?");
        BROWSE = Load("shell32.dll", 9015, "Browse");
        LOCATE = Load("appwiz.cpl", 12808, "Location");
        CREATE_SHORTCUT_Q2 = Load("appwiz.cpl", 2203, "Which name do you want to use for this shortcut ?");
        SELECT = Load("dinput.dll", 5248, "Select");
        RENAME_FOLDER = Load("shell32.dll", 16885, "Rename folder");
        RENAME_FILE = Load("shell32.dll", 16878, "Rename file");
        NEW_NAME = Load("spacecontrol.dll", 306, "New name");
        ERROR_DURING_RENAME = Load("shell32.dll", 6020, "Error during rename {0} : {1}").Replace("%2!ls!", "{0}").Replace("%1!ls!", "{1}");
        ERROR = Load("shell32.dll", 51248, "Error");
        SEARCH_RESULT = Load("shell32.dll", 34132, "Search result for %s").Replace("%s", "{0}");
    }

    internal static string Load(string libraryName, uint Ident, string DefaultText)
    {
        IntPtr libraryHandle = NativeMethods.GetModuleHandle(libraryName);
        if (libraryHandle == IntPtr.Zero)
        {
            NativeMethods.LoadLibrary(libraryName);
            libraryHandle = NativeMethods.GetModuleHandle(libraryName);
        }
        if (libraryHandle != IntPtr.Zero)
        {
            StringBuilder sb = new(1024);
            int size = NativeMethods.LoadString(libraryHandle, Ident, sb, 1024);
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
}
