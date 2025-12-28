using System;
using System.Text;

using CoolBytes.Helpers;

using ManagedShell.Common.Helpers;
using ManagedShell.Interop;

namespace Explorip.Constants;

public static class Localization
{
    private const string Shell32Dll = "shell32.dll";
    private const string TwinUIDll = "twinui.dll";
    private const string IEFrameDll = "ieframe.dll";
    private const string OleAccRcDll = "oleaccrc.dll";
    private const string StartTileDataDll = "starttiledata.dll";
    private const string AppWizCpl = "appwiz.cpl";
    private const string RegEditExe = "regedit.exe";
    private const string DiskQuotaDll = "dskquota.dll";
    private const string WpdShExtDll = "wpdshext.dll";
    private const string ExplorerFrameDll = "explorerframe.dll";
    private const string ShutdownUXDll = "shutdownux.dll";
    private const string ExplorerExe = "explorer.exe";
    private const string DeskMonDll = "deskmon.dll";
    private const string ComDlg32 = "comdlg32.dll";
    private const string TwinUiPcShellDll = "twinui.pcshell.dll";

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
    public static string UPDATE_AND_SHUTDOWN { get; private set; }
    public static string UPDATE_AND_RESTART { get; private set; }
    public static string TASKBAR { get; private set; }
    public static string TASKBAR_SHOW_DESKTOP { get; private set; }
    public static string TASKBAR_GROUP_APPLICATION_WINDOW { get; private set; }
    public static string VISIBLE { get; private set; }
    public static string NOTIFICATION_CENTER { get; private set; }
    public static string ALIGN_TO_LEFT { get; private set; }
    public static string ALIGN_TO_RIGHT { get; private set; }
    public static string ALIGN_CENTER { get; private set; }
    public static string STYLE_LIST_OF_TASKS { get; private set; }
    public static string UP { get; private set; }
    public static string DOWN { get; private set; }
    public static string PLUGINS { get; private set; }
    public static string RELOAD_PLUGINS { get; private set; }
    public static string NO_PLUGINS { get; private set; }
    public static string SHOW_TOOLBARS_POSITION { get; private set; }
    public static string COLLAPSE { get; private set; }
    public static string EXPAND { get; private set; }
    public static string FLOATING { get; private set; }
    public static string SCREEN { get; private set; }
    public static string CONFIRM_LEAVE { get; private set; }
    public static string DESKTOP { get; private set; }
    public static string WARNING { get; private set; }
    public static string NONAME_DESKTOP { get; private set; }

    public static void LoadTranslation()
    {
        PASTE = Load(Shell32Dll, 33562, "Paste");
        PASTE_SHORTCUT = Load(Shell32Dll, 37376, "Paste shortcut");
        POWER_SHELL = Load(TwinUIDll, 10928, "W_indows PowerShell");
        POWER_SHELL_ADMIN = Load(TwinUIDll, 10929, "Windows PowerShell (_admin)");
        COMMANDLINE = Load(TwinUIDll, 10919, "Commands prompt");
        COMMANDLINE_ADMIN = Load(TwinUIDll, 10920, "Commands prompt (_admin)");
        TASK_MANAGER = Load(Shell32Dll, 24743, "Tasks manager");
        SHOW_TITLE = Load("msutb.dll", 304, "Show title");
        LARGE_ICON = Load(Shell32Dll, 31062, "Show large icon");
        SMALL_ICON = Load(Shell32Dll, 31063, "Show small icon");
        NEW_TOOLBAR = Load(IEFrameDll, 12388, "New toolbar...");
        SHOW_VISUAL_KEYBOARD = Load(Shell32Dll, 24088, "Activate or desactivate visual keyboard");
        CLOSE_TAB = Load(IEFrameDll, 18160, "Close this tab").Split((char)9)[0];
        CLOSE_TAB_WITHOUT_SHORTCUT = CLOSE_TAB.Replace("_", "");
        CLOSE_OTHER_TAB = Load(IEFrameDll, 18161, "Close all others tabs");
        OPEN_NEW_TAB = Load(IEFrameDll, 13170, "New tab");
        NO_MONITOR_CONNECTED = Load("mblctr.exe", 130, "No monitor connected");
        UNLOCK_TASKBAR = Load(Shell32Dll, 24290, "Lock or unlock all taskbars");
        TOOLBAR = Load(IEFrameDll, 17986, "Toolbars");
        QUIT = Load("srh.dll", 8032, "Quit") + " Explorip";
        START = Load(Shell32Dll, 22073, "Start");
        MOVE = Load(Shell32Dll, 30382, "Move");
        MINIMIZE = Load(OleAccRcDll, 142, "Minimize");
        MAXIMIZE = Load(OleAccRcDll, 143, "Maximize");
        CLOSE = Load(OleAccRcDll, 145, "Close");
        RESTORE = Load(OleAccRcDll, 146, "Restore");
        ATTACH_TO_TASKBAR = Load(StartTileDataDll, 1009, "Attach to taskbar");
        DETACH_FROM_TASKBAR = Load(StartTileDataDll, 1010, "Detach from taskbar");
        ADJUST_DATE_TIME = Load(Shell32Dll, 24135, "Modify date and hour");
        CANCEL = Load(Shell32Dll, 33228, "Cancel");
        CONTINUE = Load(Shell32Dll, 33229, "Continue").Replace("_", "");
        OK = Load(Shell32Dll, 33225, "Ok");
        CREATE_FOLDER = Load(Shell32Dll, 31237, "Create a new folder").Replace(".", "");
        CREATE_SHORTCUT = Load(AppWizCpl, 2200, "Create a shortcut");
        CREATE_SHORTCUT_Q1 = Load(AppWizCpl, 2201, "On which element do you want to create shortcut ?");
        BROWSE = Load(Shell32Dll, 9015, "Browse");
        LOCATE = Load(AppWizCpl, 12808, "Location");
        CREATE_SHORTCUT_Q2 = Load(AppWizCpl, 2203, "Which name do you want to use for this shortcut ?");
        SELECT = Load("dinput.dll", 5248, "Select");
        RENAME_FOLDER = Load(Shell32Dll, 16885, "Rename folder");
        RENAME_FILE = Load(Shell32Dll, 16878, "Rename file");
        NEW_NAME = Load("spacecontrol.dll", 306, "New name");
        ERROR_DURING_RENAME = Load(Shell32Dll, 6020, "Error during rename {0} : {1}").Replace("%2!ls!", "{0}").Replace("%1!ls!", "{1}");
        ERROR = Load(Shell32Dll, 51248, "Error");
        SEARCH_RESULT = Load(Shell32Dll, 34132, "Search result for %s").Replace("%s", "{0}");
        SHOW_TASKBAR_ON_ALL_SCREENS = LoadMsResourceString("@{windows?ms-resource://Windows.UI.SettingsAppThreshold/SearchResources/SystemSettings_Taskbar_MultiMon/Description}", "Show taskbar on all screens");
        NEW_SHORTCUT_NAME = Load(Shell32Dll, 4154, "%s - Shortcut ().lnk");
        RENAME_MENUITEM = LoadMenuItem(Shell32Dll, 210, 18, "Re&name");
        SEND_TO = Load(Shell32Dll, 30312, "Send to");
        SEND_TO_DESKTOP = Load("sendmail.dll", 21, "Desktop (create shortcut)");
        SHOW_SUBMENU = LoadMenuItem(Shell32Dll, 215, 28674, "Show").Replace("&", "");
        SHOW_DETAILS_SUBMENU = LoadMenuItem(Shell32Dll, 216, 28747, "Details", 2).Replace("&", "");
        SHOW_JUMBO_SUBMENU = LoadMenuItem(Shell32Dll, 216, 28749, "Jumbo", 2).Replace("&", "");
        SHOW_EXTRALARGE_SUBMENU = LoadMenuItem(Shell32Dll, 216, 28751, "Extra large", 2).Replace("&", "");
        SHOW_LARGE_SUBMENU = LoadMenuItem(Shell32Dll, 216, 28750, "Large", 2).Replace("&", "");
        SHOW_SMALL_SUBMENU = LoadMenuItem(Shell32Dll, 216, 28752, "Small", 2).Replace("&", "");
        GROUP_BY_SUBMENU = LoadMenuItem(Shell32Dll, 215, 28676, "Group by").Replace("&", "");
        GROUPBY_NONE_SUBMENU = LoadMenuItem(Shell32Dll, 216, 30210, "(none)", 2).Replace("&", "");
        GROUPBY_NAME_SUBMENU = Load(WpdShExtDll, 8976, "Name").Replace("&", "");
        GROUPBY_SIZE_SUBMENU = Load(WpdShExtDll, 8978, "Size").Replace("&", "");
        GROUPBY_LASTMODIFIED_SUBMENU = Load(WpdShExtDll, 8981, "Modified").Replace("&", "");
        GROUPBY_TYPE_SUBMENU = Load(WpdShExtDll, 8979, "Type").Replace("&", "");
        REFRESH = LoadMenuItem(Shell32Dll, 215, 28931, "Refresh").Replace("&", "");
        ORDER_BY_SUBMENU = LoadMenuItem(Shell32Dll, 215, 28673, "Order by").Replace("&", "");
        ORDERBY_ASC_SUBMENU = LoadMenuItem(Shell32Dll, 215, 31488, "Ascending").Replace("&", "");
        ORDERBY_DESC_SUBMENU = LoadMenuItem(Shell32Dll, 215, 31489, "Descending").Replace("&", "");
        REGISTRY_EDITOR = Load(RegEditExe, 16, "Registry editor");
        LOCALIZED_BYTE = Load(DiskQuotaDll, 14472, "bytes");
        LOCALIZED_KILO = Load(DiskQuotaDll, 14473, "kilo");
        LOCALIZED_MEGA = Load(DiskQuotaDll, 14474, "mega");
        LOCALIZED_GIGA = Load(DiskQuotaDll, 14475, "giga");
        LOCALIZED_TERA = Load(DiskQuotaDll, 14476, "tera");
        LOCALIZED_PETA = Load(DiskQuotaDll, 14477, "peta");
        LOCALIZED_EXA = Load(DiskQuotaDll, 14478, "exa");
        NUMBER_OF_ELEMENT = Load(Shell32Dll, 38192, "%s element(s)");
        NUMBER_OF_SELECTED_ELEMENT = Load(Shell32Dll, 38194, "%s selected elements");
        CONTEXT_MENU_EXPLORER_ADDRESS_BAR_COPY = LoadMenuItem(ExplorerFrameDll, 272, 1282, "Copy address as text").Replace("&", "_");
        CONTEXT_MENU_EXPLORER_ADDRESS_BAR_MODIFY = LoadMenuItem(ExplorerFrameDll, 272, 1280, "Modify address").Replace("&", "_");
        CONTEXT_MENU_EXPLORER_ADDRESS_BAR_DELETE_HISTORY = LoadMenuItem(ExplorerFrameDll, 272, 1283, "Delete history of navigation").Replace("&", "_");
        SHOW_MOSAIC_SUBMENU = LoadMenuItem(Shell32Dll, 216, 28748, "Mosaic", 2).Replace("&", "");
        ERROR_PATH_NOT_FOUND = Load(Shell32Dll, 12353, "Windows can't find %1. Check syntax");
        COLUMN_DURATION = Load(Shell32Dll, 34828, "Duration");
        REGEDIT_MODIFY_VALUE = LoadMenuItem(RegEditExe, 105, 912, "Modify...").Replace("&", "_");
        REGEDIT_MODIFY_VALUE_BINARY = LoadMenuItem(RegEditExe, 105, 917, "Modify binary data...").Replace("&", "_");
        REGEDIT_DELETE_VALUE = LoadMenuItem(RegEditExe, 105, 913, "Delete").Replace("&", "_");
        REGEDIT_RENAME_VALUE = LoadMenuItem(RegEditExe, 105, 914, "Rename").Replace("&", "_");
        REGEDIT_CONFIRM_DELETE_VALUE = Load(RegEditExe, 50, "Delete some values of registry can do unsable system. Are you sure you want to delete this value ?");
        REGEDIT_CONFIRM_DELETE_KEY = Load(RegEditExe, 48, "Are you sure you want to delete this key (and all of his subkey) ?");
        REGEDIT_STRING_DEFAULT = Load(RegEditExe, 20, "(by default)");
        REGEDIT_NEW = LoadMenuItem(RegEditExe, 106, 516, "New").Replace("&", "_");
        REGEDIT_NEW_KEY = LoadMenuItem(RegEditExe, 106, 663, "Key", 0).Replace("&", "_");
        REGEDIT_NEW_STRING = LoadMenuItem(RegEditExe, 106, 664, "String", 0).Replace("&", "_");
        REGEDIT_NEW_BINARY = LoadMenuItem(RegEditExe, 106, 665, "Binary", 1).Replace("&", "_");
        REGEDIT_NEW_DWORD_32 = LoadMenuItem(RegEditExe, 106, 674, "Dword (32)", 1).Replace("&", "_");
        REGEDIT_NEW_DWORD_64 = LoadMenuItem(RegEditExe, 106, 688, "Dword (64)", 1).Replace("&", "_");
        REGEDIT_NEW_MULTIPLE_STRING = LoadMenuItem(RegEditExe, 106, 676, "Multiple string", 1).Replace("&", "_");
        REGEDIT_NEW_EXTENDED_STRING = LoadMenuItem(RegEditExe, 106, 677, "Extended string", 1).Replace("&", "_");
        REGEDIT_NEW_KEY_NAME = Load(RegEditExe, 23, "New key #%%u");
        REGEDIT_NEW_VALUE_NAME = Load(RegEditExe, 24, "New value #%%u");
        REGEDIT_ERROR_DWORD32 = Load(RegEditExe, 30, "Value dword 32 bits invalid");
        REGEDIT_ERROR_DWORD64 = Load(RegEditExe, 33, "Value dword 64 bits invalid");
        REGEDIT_VALUE_NAME = Load(RegEditExe, 17, "Name");
        REGEDIT_TYPE_NAME = Load(RegEditExe, 31, "Type");
        REGEDIT_DATA_NAME = Load(RegEditExe, 18, "Data");
        TASKBAR_SHOW_TASKMGR = Load(ExplorerFrameDll, 50259, "Show") + " " + Load("twinui.pcshell.dll", 34304, "actives applications").ToLower();
        TASKBAR_SHOW_WIDGET = Load(ExplorerFrameDll, 50259, "Show") + " " + Load("mshtml.dll", 53857, "widget").ToLower();
        SHOW_HIDDEN_ICONS = Load(ExplorerExe, 543, "Show hidden icons");
        HIDE = Load(ExplorerExe, 542, "Hide");
        SEARCH_TIP = Load(ExplorerExe, 907, "Enter here to start a search");
        SHOW_TASKMAN_TIP = Load(ExplorerExe, 900, "Show tasks");
        SHOW_NOTIFICATION_TIP = Load(ExplorerExe, 852, "Show notifications");
        SHOW_TACTILE_KEYBOARD = Load(ExplorerExe, 905, "Visual keyboard");
        WIDGETS = Load("mshtml.dll", 53857, "Widgets");
        OPEN_FOLDER = Load(Shell32Dll, 32960, "Open the folder");
        START_STOP = Load("dinput.dll", 734, "Start/Shutdown");
        SETTINGS = Load(TwinUIDll, 5621, "Settings");
        DISCONNECT = Load(ShutdownUXDll, 3034, "Disconnecting");
        LOCK = Load(ShutdownUXDll, 3042, "Lock");
        CHANGE_USER = Load("consolelogon.dll", 114, "Switch user");
        PUT_HYBERNATE = Load(ShutdownUXDll, 3019, "Go to sleep mode");
        SHUTDOWN = Load(ShutdownUXDll, 3013, "Shutdown");
        RESTART = Load(ShutdownUXDll, 3016, "Restart");
        DELETE = Load(Shell32Dll, 31252, "Delete");
        PIN_TO_TASKBAR = Load(StartTileDataDll, 1009, "Pin to taskbar");
        UNPIN_FROM_TASKBAR = Load(StartTileDataDll, 1010, "Unpin from taskbar");
        PIN_TO_STARTMENU = Load(StartTileDataDll, 1007, "Pin to Start Menu");
        UNPIN_FROM_STARTMENU = Load(StartTileDataDll, 1008, "Unpin from Start Menu");
        OPEN_NEW_WINDOW = Load(StartTileDataDll, 1001, "Open a new window");
        if (EnvironmentHelper.IsWindows11OrBetter)
        {
            CLOSE_ALL_WINDOW = LoadMenuItem("taskbar.dll", 12000, 65491, "Close all windows").Replace("&", "_");
            TASKBAR_SHOW_SEARCH_ZONE = LoadMsResourceString("@{windows?ms-resource://Windows.UI.SettingsAppThreshold/SystemSettings/Resources/SystemSettings_DesktopTaskbar_Search_Box}", "Show search zone");
            TASKBAR_SHOW_SEARCH = LoadMsResourceString("@{windows?ms-resource://Windows.UI.SettingsAppThreshold/SystemSettings/Resources/SystemSettings_DesktopTaskbar_Search_Icon}", "Show search button");
            SMALL_ICON_TASKBAR = Load(Shell32Dll, 31063, "Small icons");
            TASKBAR_SHOW_DESKTOP = LoadMsResourceString("@{windows?ms-resource://Windows.UI.SettingsAppThreshold/SystemSettings/Resources/SystemSettings_DesktopTaskbar_Sd}", "Show desktop preview button");
            SHOW_SECOND_START_MENU_PANEL = LoadMsResourceString("@{windows?ms-resource://Windows.UI.SettingsAppThreshold/SystemSettings/Resources/SystemSettings_Start_MorePinnedLayout/Content}", "Show more tiles in start menu");
            SHOW_STARTMENUITEM_STARTWINDOW = Load("propsys.dll", 38974, "Group of Start Menu");
        }
        else
        {
            CLOSE_ALL_WINDOW = LoadMenuItem(ExplorerExe, 12000, 65491, "Close all windows").Replace("&", "_");
            TASKBAR_SHOW_SEARCH_ZONE = LoadMenuItem(ExplorerExe, 205, 434, "Show search zone", 0).Replace("&", "_");
            TASKBAR_SHOW_SEARCH = LoadMenuItem(ExplorerExe, 205, 433, "Show search button", 0).Replace("&", "_");
            SMALL_ICON_TASKBAR = LoadMsResourceString("@{windows?ms-resource://Windows.UI.SettingsAppThreshold/SearchResources/SystemSettings_Taskbar_SmallButtons/Description}", "Use small icon for button of task bar");
            TASKBAR_SHOW_DESKTOP = Load("uiautomationcore.dll", 1, "Button") + " " + Load(ExplorerExe, 22000, "Desktop").ToLower();
            SHOW_SECOND_START_MENU_PANEL = LoadMsResourceString("@{windows?ms-resource://Windows.UI.SettingsAppThreshold/SearchResources/SystemSettings_Start_MoreTilesEnabled/Description}", "Show more tiles in start menu");
            SHOW_STARTMENUITEM_STARTWINDOW = LoadMsResourceString("@{windows?ms-resource://Windows.UI.SettingsAppThreshold/SearchResources/SystemSettings_Start_ShowAppList/Description}", "Show applications in start menu");
        }
        RENAME_MENUITEM_WPF = RENAME_MENUITEM.Replace("&", "_");
        ASK_DOWNLOAD_NEW_VERSION = Load("wscapi.dll", 6104, "Do you want to update %1");
        ASK_INSTALL_NEW_VERSION = Load("wscapi.dll", 6101, "Click to install the new version of %1");
        CHOICE_KEYBOARD_LAYOUT = Load("bootux.dll", 1634, "Choice the keyboard layout");
        CHANGE_BACKGROUND_COLOR = Load(IEFrameDll, 20051, "Background color");
        CHANGE_FONT_COLOR = Load(IEFrameDll, 20055, "Font color");
        CUSTOM_COLOR = Load("uiribbon.dll", 147, "Custom colors");
        SEARCH = Load(ExplorerFrameDll, 12897, "Search");
        UPDATE_AND_SHUTDOWN = Load(ShutdownUXDll, 3026, "Update and shutdown");
        UPDATE_AND_RESTART = Load(ShutdownUXDll, 3030, "Update and restart");
        TASKBAR = Load(ExplorerExe, 518, "Taskbar");
        TASKBAR_GROUP_APPLICATION_WINDOW = Load(Shell32Dll, 24289, "Regroup same application windows in taskbar");
        VISIBLE = Load("propsys.dll", 42120, "Visible");
        NOTIFICATION_CENTER = Load(ExplorerExe, 852, "Notifications center");
        ALIGN_TO_LEFT = Load("inetres.dll", 5232, "Align to left");
        ALIGN_TO_RIGHT = Load("inetres.dll", 5234, "Aligne to right");
        ALIGN_CENTER = Load("inetres.dll", 5233, "Center");
        STYLE_LIST_OF_TASKS = Load("mmcbase.dll", 14138, "Style of list of tasks");
        DOWN = Load("windows.ui.xaml.dll", 5377, "Down");
        UP = Load("windows.ui.xaml.dll", 5513, "Up");
        PLUGINS = "Plugins";
        RELOAD_PLUGINS = "Reload plugins";
        NO_PLUGINS = "No plugins loaded";
        SHOW_TOOLBARS_POSITION = Load(IEFrameDll, 10324, "Show the toolbars...");
        EXPAND = Load("uiautomationcore.dll", 204, "Expand");
        COLLAPSE = Load("uiautomationcore.dll", 205, "Collapse");
        FLOATING = Load(OleAccRcDll, 1013, "Floating").FirstCharToUpperCase();
        SCREEN = Load(DeskMonDll, 1, "Screen");
        CONFIRM_LEAVE = Load(ComDlg32, 259, "Are you sure you want to leave ?");
        DESKTOP = Load(Shell32Dll, 34625, "Desktop");
        WARNING = Load(Shell32Dll, 9021, "Warning");
        NONAME_DESKTOP = Load(TwinUiPcShellDll, 13011, "Desktop %d");
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
                        fMask = NativeMethods.MenuItemIntegrateMembers.STRING,
                        fType = NativeMethods.MenuItemTypes.STRING,
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
