using System;
using System.Text;

using ManagedShell.Common.Helpers;
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
    public static string SHOW_TASKBAR_ON_ALL_SCREENS { get; private set; }
    public static string NEW_SHORTCUT_NAME { get; private set; }
    public static string RENAME_MENUITEM { get; private set; }
    public static string SEND_TO { get; private set; }
    public static string SEND_TO_DESKTOP { get; private set; }
    public static string SHOW_SUBMENU { get; private set; }
    public static string SHOW_DETAILS_SUBMENU { get; private set; }
    public static string SHOW_JUMBO_SUBMENU { get; private set; }
    public static string SHOW_EXTRALARGE_SUBMENU { get; private set; }
    public static string SHOW_LARGE_SUBMENU { get; private set; }
    public static string SHOW_SMALL_SUBMENU { get; private set; }
    public static string GROUP_BY_SUBMENU { get; private set; }
    public static string GROUPBY_NONE_SUBMENU { get; private set; }
    public static string GROUPBY_NAME_SUBMENU { get; private set; }
    public static string GROUPBY_LASTMODIFIED_SUBMENU { get; private set; }
    public static string GROUPBY_TYPE_SUBMENU { get; private set; }
    public static string GROUPBY_SIZE_SUBMENU { get; private set; }
    public static string REFRESH { get; private set; }
    public static string ORDER_BY_SUBMENU { get; private set; }
    public static string ORDERBY_ASC_SUBMENU { get; private set; }
    public static string ORDERBY_DESC_SUBMENU { get; private set; }
    public static string REGISTRY_EDITOR { get; private set; }
    public static string LOCALIZED_BYTE { get; private set; }
    public static string LOCALIZED_KILO { get; private set; }
    public static string LOCALIZED_MEGA { get; private set; }
    public static string LOCALIZED_GIGA { get; private set; }
    public static string LOCALIZED_TERA { get; private set; }
    public static string LOCALIZED_PETA { get; private set; }
    public static string LOCALIZED_EXA { get; private set; }
    public static string NUMBER_OF_ELEMENT { get; private set; }
    public static string NUMBER_OF_SELECTED_ELEMENT { get; private set; }
    public static string CONTEXT_MENU_EXPLORER_ADDRESS_BAR_COPY { get; private set; }
    public static string CONTEXT_MENU_EXPLORER_ADDRESS_BAR_MODIFY { get; private set; }
    public static string CONTEXT_MENU_EXPLORER_ADDRESS_BAR_DELETE_HISTORY { get; private set; }
    public static string SHOW_MOSAIC_SUBMENU { get; private set; }
    public static string ERROR_PATH_NOT_FOUND { get; private set; }
    public static string COLUMN_DURATION { get; private set; }
    public static string REGEDIT_MODIFY_VALUE { get; private set; }
    public static string REGEDIT_MODIFY_VALUE_BINARY { get; private set; }
    public static string REGEDIT_DELETE_VALUE { get; private set; }
    public static string REGEDIT_RENAME_VALUE { get; private set; }
    public static string REGEDIT_CONFIRM_DELETE_VALUE { get; private set; }
    public static string REGEDIT_CONFIRM_DELETE_KEY { get; private set; }
    public static string REGEDIT_STRING_DEFAULT { get; private set; }
    public static string REGEDIT_NEW_KEY { get; private set; }
    public static string REGEDIT_NEW_STRING { get; private set; }
    public static string REGEDIT_NEW_BINARY { get; private set; }
    public static string REGEDIT_NEW_DWORD_32 { get; private set; }
    public static string REGEDIT_NEW_DWORD_64 { get; private set; }
    public static string REGEDIT_NEW_MULTIPLE_STRING { get; private set; }
    public static string REGEDIT_NEW_EXTENDED_STRING { get; private set; }
    public static string REGEDIT_NEW { get; private set; }
    public static string REGEDIT_NEW_KEY_NAME { get; private set; }
    public static string REGEDIT_NEW_VALUE_NAME { get; private set; }
    public static string REGEDIT_ERROR_DWORD32 { get; private set; }
    public static string REGEDIT_ERROR_DWORD64 { get; private set; }
    public static string REGEDIT_VALUE_NAME { get; private set; }
    public static string REGEDIT_DATA_NAME { get; private set; }
    public static string REGEDIT_TYPE_NAME { get; private set; }
    public static string TASKBAR_SHOW_TASKMGR { get; private set; }
    public static string TASKBAR_SHOW_SEARCH { get; private set; }
    public static string TASKBAR_SHOW_WIDGET { get; private set; }
    public static string SHOW_HIDDEN_ICONS { get; private set; }
    public static string HIDE { get; private set; }
    public static string SEARCH_TIP { get; private set; }
    public static string SHOW_TASKMAN_TIP { get; private set; }
    public static string SHOW_NOTIFICATION_TIP { get; private set; }
    public static string SHOW_TACTILE_KEYBOARD { get; private set; }
    public static string WIDGETS { get; private set; }
    public static string OPEN_FOLDER { get; private set; }
    public static string START_STOP { get; private set; }
    public static string SETTINGS { get; private set; }
    public static string LOCK { get; private set; }
    public static string DISCONNECT { get; private set; }
    public static string CHANGE_USER { get; private set; }
    public static string PUT_HYBERNATE { get; private set; }
    public static string SHUTDOWN { get; private set; }
    public static string RESTART { get; private set; }
    public static string DELETE { get; private set; }
    public static string PIN_TO_TASKBAR { get; private set; }
    public static string UNPIN_FROM_TASKBAR { get; private set; }
    public static string PIN_TO_STARTMENU { get; private set; }
    public static string UNPIN_FROM_STARTMENU { get; private set; }
    public static string OPEN_NEW_WINDOW { get; private set; }
    public static string CLOSE_ALL_WINDOW { get; private set; }
    public static string RENAME_MENUITEM_WPF { get; private set; }
    public static string ASK_INSTALL_NEW_VERSION { get; private set; }
    public static string ASK_DOWNLOAD_NEW_VERSION { get; private set; }
    public static string CHOICE_KEYBOARD_LAYOUT { get; private set; }
    public static string CHANGE_BACKGROUND_COLOR { get; private set; }
    public static string CHANGE_FONT_COLOR { get; private set; }
    public static string CUSTOM_COLOR { get; private set; }
    public static string SHOW_SECOND_START_MENU_PANEL { get; private set; }
    public static string SHOW_STARTMENUITEM_STARTWINDOW { get; private set; }
    public static string SMALL_ICON_TASKBAR { get; private set; }
    public static string SEARCH { get; private set; }
    public static string TASKBAR_SHOW_SEARCH_ZONE { get; private set; }

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
        SHOW_TASKBAR_ON_ALL_SCREENS = LoadMsResourceString("@{windows?ms-resource://Windows.UI.SettingsAppThreshold/SearchResources/SystemSettings_Taskbar_MultiMon/Description}", "Show taskbar on all screens");
        NEW_SHORTCUT_NAME = Load("shell32.dll", 4154, "%s - Shortcut ().lnk");
        RENAME_MENUITEM = LoadMenuItem("shell32.dll", 210, 18, "Re&name");
        SEND_TO = Load("shell32.dll", 30312, "Send to");
        SEND_TO_DESKTOP = Load("sendmail.dll", 21, "Desktop (create shortcut)");
        SHOW_SUBMENU = LoadMenuItem("shell32.dll", 215, 28674, "Show").Replace("&", "");
        SHOW_DETAILS_SUBMENU = LoadMenuItem("shell32.dll", 216, 28747, "Details", 2).Replace("&", "");
        SHOW_JUMBO_SUBMENU = LoadMenuItem("shell32.dll", 216, 28749, "Jumbo", 2).Replace("&", "");
        SHOW_EXTRALARGE_SUBMENU = LoadMenuItem("shell32.dll", 216, 28751, "Extra large", 2).Replace("&", "");
        SHOW_LARGE_SUBMENU = LoadMenuItem("shell32.dll", 216, 28750, "Large", 2).Replace("&", "");
        SHOW_SMALL_SUBMENU = LoadMenuItem("shell32.dll", 216, 28752, "Small", 2).Replace("&", "");
        GROUP_BY_SUBMENU = LoadMenuItem("shell32.dll", 215, 28676, "Group by").Replace("&", "");
        GROUPBY_NONE_SUBMENU = LoadMenuItem("shell32.dll", 216, 30210, "(none)", 2).Replace("&", "");
        GROUPBY_NAME_SUBMENU = Load("wpdshext.dll", 8976, "Name").Replace("&", "");
        GROUPBY_SIZE_SUBMENU = Load("wpdshext.dll", 8978, "Size").Replace("&", "");
        GROUPBY_LASTMODIFIED_SUBMENU = Load("wpdshext.dll", 8981, "Modified").Replace("&", "");
        GROUPBY_TYPE_SUBMENU = Load("wpdshext.dll", 8979, "Type").Replace("&", "");
        REFRESH = LoadMenuItem("shell32.dll", 215, 28931, "Refresh").Replace("&", "");
        ORDER_BY_SUBMENU = LoadMenuItem("shell32.dll", 215, 28673, "Order by").Replace("&", "");
        ORDERBY_ASC_SUBMENU = LoadMenuItem("shell32.dll", 215, 31488, "Ascending").Replace("&", "");
        ORDERBY_DESC_SUBMENU = LoadMenuItem("shell32.dll", 215, 31489, "Descending").Replace("&", "");
        REGISTRY_EDITOR = Load("regedit.exe", 16, "Registry editor");
        LOCALIZED_BYTE = Load("dskquota.dll", 14472, "bytes");
        LOCALIZED_KILO = Load("dskquota.dll", 14473, "kilo");
        LOCALIZED_MEGA = Load("dskquota.dll", 14474, "mega");
        LOCALIZED_GIGA = Load("dskquota.dll", 14475, "giga");
        LOCALIZED_TERA = Load("dskquota.dll", 14476, "tera");
        LOCALIZED_PETA = Load("dskquota.dll", 14477, "peta");
        LOCALIZED_EXA = Load("dskquota.dll", 14478, "exa");
        NUMBER_OF_ELEMENT = Load("shell32.dll", 38192, "%s element(s)");
        NUMBER_OF_SELECTED_ELEMENT = Load("shell32.dll", 38194, "%s selected elements");
        CONTEXT_MENU_EXPLORER_ADDRESS_BAR_COPY = LoadMenuItem("explorerframe.dll", 272, 1282, "Copy address as text").Replace("&", "_");
        CONTEXT_MENU_EXPLORER_ADDRESS_BAR_MODIFY = LoadMenuItem("explorerframe.dll", 272, 1280, "Modify address").Replace("&", "_");
        CONTEXT_MENU_EXPLORER_ADDRESS_BAR_DELETE_HISTORY = LoadMenuItem("explorerframe.dll", 272, 1283, "Delete history of navigation").Replace("&", "_");
        SHOW_MOSAIC_SUBMENU = LoadMenuItem("shell32.dll", 216, 28748, "Mosaic", 2).Replace("&", "");
        ERROR_PATH_NOT_FOUND = Load("shell32.dll", 12353, "Windows can't find %1. Check syntax");
        COLUMN_DURATION = Load("shell32.dll", 34828, "Duration");
        REGEDIT_MODIFY_VALUE = LoadMenuItem("regedit.exe", 105, 912, "Modify...").Replace("&", "_");
        REGEDIT_MODIFY_VALUE_BINARY = LoadMenuItem("regedit.exe", 105, 917, "Modify binary data...").Replace("&", "_");
        REGEDIT_DELETE_VALUE = LoadMenuItem("regedit.exe", 105, 913, "Delete").Replace("&", "_");
        REGEDIT_RENAME_VALUE = LoadMenuItem("regedit.exe", 105, 914, "Rename").Replace("&", "_");
        REGEDIT_CONFIRM_DELETE_VALUE = Load("regedit.exe", 50, "Delete some values of registry can do unsable system. Are you sure you want to delete this value ?");
        REGEDIT_CONFIRM_DELETE_KEY = Load("regedit.exe", 48, "Are you sure you want to delete this key (and all of his subkey) ?");
        REGEDIT_STRING_DEFAULT = Load("regedit.exe", 20, "(by default)");
        REGEDIT_NEW = LoadMenuItem("regedit.exe", 106, 516, "New").Replace("&", "_");
        REGEDIT_NEW_KEY = LoadMenuItem("regedit.exe", 106, 663, "Key", 0).Replace("&", "_");
        REGEDIT_NEW_STRING = LoadMenuItem("regedit.exe", 106, 664, "String", 0).Replace("&", "_");
        REGEDIT_NEW_BINARY = LoadMenuItem("regedit.exe", 106, 665, "Binary", 1).Replace("&", "_");
        REGEDIT_NEW_DWORD_32 = LoadMenuItem("regedit.exe", 106, 674, "Dword (32)", 1).Replace("&", "_");
        REGEDIT_NEW_DWORD_64 = LoadMenuItem("regedit.exe", 106, 688, "Dword (64)", 1).Replace("&", "_");
        REGEDIT_NEW_MULTIPLE_STRING = LoadMenuItem("regedit.exe", 106, 676, "Multiple string", 1).Replace("&", "_");
        REGEDIT_NEW_EXTENDED_STRING = LoadMenuItem("regedit.exe", 106, 677, "Extended string", 1).Replace("&", "_");
        REGEDIT_NEW_KEY_NAME = Load("regedit.exe", 23, "New key #%%u");
        REGEDIT_NEW_VALUE_NAME = Load("regedit.exe", 24, "New value #%%u");
        REGEDIT_ERROR_DWORD32 = Load("regedit.exe", 30, "Value dword 32 bits invalid");
        REGEDIT_ERROR_DWORD64 = Load("regedit.exe", 33, "Value dword 64 bits invalid");
        REGEDIT_VALUE_NAME = Load("regedit.exe", 17, "Name");
        REGEDIT_TYPE_NAME = Load("regedit.exe", 31, "Type");
        REGEDIT_DATA_NAME = Load("regedit.exe", 18, "Data");
        TASKBAR_SHOW_TASKMGR = Load("explorerframe.dll", 50259, "Show") + " " + Load("twinui.pcshell.dll", 34304, "actives applications").ToLower();
        TASKBAR_SHOW_WIDGET = Load("explorerframe.dll", 50259, "Show") + " " + Load("mshtml.dll", 53857, "widget").ToLower();
        SHOW_HIDDEN_ICONS = Load("explorer.exe", 543, "Show hidden icons");
        HIDE = Load("explorer.exe", 542, "Hide");
        SEARCH_TIP = Load("explorer.exe", 907, "Enter here to start a search");
        SHOW_TASKMAN_TIP = Load("explorer.exe", 900, "Show tasks");
        SHOW_NOTIFICATION_TIP = Load("explorer.exe", 852, "Show notifications");
        SHOW_TACTILE_KEYBOARD = Load("explorer.exe", 905, "Visual keyboard");
        WIDGETS = Load("mshtml.dll", 53857, "Widgets");
        OPEN_FOLDER = Load("shell32.dll", 32960, "Open the folder");
        START_STOP = Load("dinput.dll", 734, "Start/Shutdown");
        SETTINGS = Load("twinui.dll", 5621, "Settings");
        DISCONNECT = Load("shutdownux.dll", 3034, "Disconnecting");
        LOCK = Load("shutdownux.dll", 3042, "Lock");
        CHANGE_USER = Load("consolelogon.dll", 114, "Switch user");
        PUT_HYBERNATE = Load("shutdownux.dll", 3019, "Go to sleep mode");
        SHUTDOWN = Load("shutdownux.dll", 3013, "Shutdown");
        RESTART = Load("shutdownux.dll", 3016, "Restart");
        DELETE = Load("shell32.dll", 31252, "Delete");
        PIN_TO_TASKBAR = Load("starttiledata.dll", 1009, "Pin to taskbar");
        UNPIN_FROM_TASKBAR = Load("starttiledata.dll", 1010, "Unpin from taskbar");
        PIN_TO_STARTMENU = Load("starttiledata.dll", 1007, "Pin to Start Menu");
        UNPIN_FROM_STARTMENU = Load("starttiledata.dll", 1008, "Unpin from Start Menu");
        OPEN_NEW_WINDOW = Load("starttiledata.dll", 1001, "Open a new window");
        if (EnvironmentHelper.IsWindows11OrBetter)
        {
            CLOSE_ALL_WINDOW = LoadMenuItem("taskbar.dll", 12000, 65491, "Close all windows").Replace("&", "_");
            TASKBAR_SHOW_SEARCH_ZONE = LoadMenuItem("taskbar.dll", 205, 434, "Show search zone", 0).Replace("&", "_");
            TASKBAR_SHOW_SEARCH = LoadMenuItem("taskbar.dll", 205, 433, "Show search button", 0).Replace("&", "_");
        }
        else
        {
            CLOSE_ALL_WINDOW = LoadMenuItem("explorer.exe", 12000, 65491, "Close all windows").Replace("&", "_");
            TASKBAR_SHOW_SEARCH_ZONE = LoadMenuItem("explorer.exe", 205, 434, "Show search zone", 0).Replace("&", "_");
            TASKBAR_SHOW_SEARCH = LoadMenuItem("explorer.exe", 205, 433, "Show search button", 0).Replace("&", "_");
        }
        RENAME_MENUITEM_WPF = RENAME_MENUITEM.Replace("&", "_");
        ASK_DOWNLOAD_NEW_VERSION = Load("wscapi.dll", 6104, "Do you want to update %1");
        ASK_INSTALL_NEW_VERSION = Load("wscapi.dll", 6101, "Click to install the new version of %1");
        CHOICE_KEYBOARD_LAYOUT = Load("bootux.dll", 1634, "Choice the keyboard layout");
        CHANGE_BACKGROUND_COLOR = Load("ieframe.dll", 20051, "Background color");
        CHANGE_FONT_COLOR = Load("ieframe.dll", 20055, "Font color");
        CUSTOM_COLOR = Load("uiribbon.dll", 147, "Custom colors");
        SHOW_SECOND_START_MENU_PANEL = LoadMsResourceString("@{windows?ms-resource://Windows.UI.SettingsAppThreshold/SearchResources/SystemSettings_Start_MoreTilesEnabled/Description}", "Show more tiles in start menu");
        SHOW_STARTMENUITEM_STARTWINDOW = LoadMsResourceString("@{windows?ms-resource://Windows.UI.SettingsAppThreshold/SearchResources/SystemSettings_Start_ShowAppList/Description}", "Show applications in start menu");
        SMALL_ICON_TASKBAR = LoadMsResourceString("@{windows?ms-resource://Windows.UI.SettingsAppThreshold/SearchResources/SystemSettings_Taskbar_SmallButtons/Description}", "Use small icon for button of task bar");
        SEARCH = Load("shell32.dll", 32872, "Search");
    }

    internal static string LoadMsResourceString(string key, string defaultValue, int maxChar = 256)
    {
        StringBuilder sb = new(maxChar);
        NativeMethods.SHLoadIndirectString(key, sb, sb.Capacity, IntPtr.Zero);
        if (sb.Length == 0)
            return defaultValue;
        else
            return sb.ToString();
    }

    internal static string Load(string libraryName, uint Ident, string defaultText)
    {
        IntPtr libraryHandle = LibraryHandle(libraryName);
        if (libraryHandle != IntPtr.Zero)
        {
            StringBuilder sb = new(1024);
            int size = NativeMethods.LoadString(libraryHandle, Ident, sb, 1024);
            if (size > 0)
                return sb.ToString().Replace("&", "_");
            else
                return defaultText;
        }
        else
        {
            return defaultText;
        }
    }

    private static IntPtr LibraryHandle(string libraryName)
    {
        IntPtr libraryHandle = NativeMethods.GetModuleHandle(libraryName);
        if (libraryHandle == IntPtr.Zero)
        {
            NativeMethods.LoadLibrary(libraryName);
            libraryHandle = NativeMethods.GetModuleHandle(libraryName);
        }
        return libraryHandle;
    }

    internal static string LoadMenuItem(string libraryName, uint idMenu, uint idSubMenu, string defaultText, int numSubMenu = 0)
    {
        IntPtr libraryHandle = LibraryHandle(libraryName);
        if (libraryHandle != IntPtr.Zero)
        {
            IntPtr hMenu = NativeMethods.LoadMenu(libraryHandle, idMenu);
            if (hMenu != IntPtr.Zero)
            {
                IntPtr hSubMenu = NativeMethods.GetSubMenu(hMenu, numSubMenu);
                if (hSubMenu != IntPtr.Zero)
                {
                    NativeMethods.MenuItemInfo myMenuItemInfo = new()
                    {
                        fMask = NativeMethods.MIIM.STRING,
                        fType = NativeMethods.MFT.STRING,
                    };
                    if (NativeMethods.GetMenuItemInfo(hSubMenu, idSubMenu, false, ref myMenuItemInfo))
                    {
                        myMenuItemInfo.cch++; // one more octet for zero byte end string
                        myMenuItemInfo.dwTypeData = new string(' ', myMenuItemInfo.cch * 2); // * 2 because of possible unicode string
                        NativeMethods.GetMenuItemInfo(hSubMenu, idSubMenu, false, ref myMenuItemInfo);
                        return myMenuItemInfo.dwTypeData;
                    }
                }
            }
        }
        return defaultText;
    }
}
