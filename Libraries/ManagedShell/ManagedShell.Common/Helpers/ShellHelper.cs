using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using ManagedShell.Common.Logging;

using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;

using static ManagedShell.Interop.NativeMethods;

namespace ManagedShell.Common.Helpers;

public static class ShellHelper
{
    public const int MAX_PATH = 260;
    internal const string ShellViewName = "SHELLDLL_DefView";

    public static string GetDisplayName(string filename)
    {
        ShFileInfo shinfo = new()
        {
            szDisplayName = string.Empty,
            szTypeName = string.Empty
        };
        SHGetFileInfo(filename, EFileAttributes.NORMAL, ref shinfo, (uint)Marshal.SizeOf(shinfo), ShGetFileInfos.DisplayName);

        return shinfo.szDisplayName;
    }

    public static string UsersStartMenuPath => GetSpecialFolderPath((int)ConstSpecialItemIDList.CSIDL_STARTMENU);

    public static string AllUsersStartMenuPath => GetSpecialFolderPath((int)ConstSpecialItemIDList.CSIDL_COMMON_STARTMENU);

    public static string GetSpecialFolderPath(int FOLDER)
    {
        StringBuilder sbPath = new(MAX_PATH);
        SHGetFolderPath(IntPtr.Zero, FOLDER, IntPtr.Zero, 0, sbPath);
        return sbPath.ToString();
    }

    public static bool ExecuteProcess(string filename)
    {
        Process proc = new();
        proc.StartInfo.UseShellExecute = true;
        proc.StartInfo.FileName = filename;

        try
        {
            return proc.Start();
        }
        catch
        {
            // No 'Open' command associated with this filetype in the registry
            ShowOpenWithDialog(proc.StartInfo.FileName);
            return false;
        }
    }

    public static bool StartProcess(string filename, string args = null, bool hidden = false, bool useShellExecute = false, string verb = null, bool loadUserProfile = false, string workingDir = "")
    {
        try
        {
            ProcessStartInfo psi = new()
            {
                UseShellExecute = useShellExecute,
                CreateNoWindow = hidden,
                WindowStyle = hidden ? ProcessWindowStyle.Hidden : ProcessWindowStyle.Normal,
                Verb = verb,
                LoadUserProfile = loadUserProfile,
                WorkingDirectory = (string.IsNullOrWhiteSpace(workingDir) ? Path.GetDirectoryName(filename) : workingDir),
            };

            if (filename.StartsWith("appx:"))
            {
                psi.FileName = "LaunchWinApp.exe";
                psi.Arguments = "shell:appsFolder\\" + filename.Substring(5);
            }
            else if (filename.Contains("://"))
            {
                psi.FileName = "explorer.exe";
                psi.Arguments = filename;
            }
            else
            {
                if (EnvironmentHelper.IsAppRunningAsShell && filename.ToLower().EndsWith("explorer.exe"))
                {
                    // if we are shell and launching explorer, give it a parameter so that it doesn't do shell things.
                    // this opens My Computer
                    psi.FileName = filename;
                    psi.Arguments = "::{20D04FE0-3AEA-1069-A2D8-08002B30309D}";
                }
                else
                {
                    psi.FileName = filename;
                    psi.Arguments = args;
                }
            }

            Process.Start(psi);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool StartProcess(string filename, string args, bool useShellExecute, string workingDir)
    {
        try
        {
            ProcessStartInfo psi = new()
            {
                UseShellExecute = useShellExecute,
                FileName = filename,
                Arguments = args,
                WorkingDirectory = (string.IsNullOrWhiteSpace(workingDir) ? Path.GetDirectoryName(filename) : workingDir),
            };

            Process.Start(psi);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool StartProcess(string filename, string args, string verb, bool useShellExecute)
    {
        try
        {
            Process proc = new();
            proc.StartInfo.UseShellExecute = useShellExecute;
            proc.StartInfo.FileName = filename;
            proc.StartInfo.Arguments = args;
            proc.StartInfo.Verb = verb;
            try
            {
                proc.Start();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Error running the {0} verb on {1}. ({2})", verb, filename, ex.Message));
                return false;
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool LaunchDetachedProcess(string filename, string args, bool createNewConsole = false)
    {
        StartupInfo si = new();
        si.cb = (uint)Marshal.SizeOf(si);

        string commandLine = $"\"{filename}\" {args}";

        ECreateProcess flag = ECreateProcess.DETACHED_PROCESS;
        if (createNewConsole)
            flag |= ECreateProcess.CREATE_NEW_CONSOLE;

        bool success = CreateProcess(
            null,
            commandLine,
            IntPtr.Zero,
            IntPtr.Zero,
            false,
            flag,
            IntPtr.Zero,
            null,
            ref si,
            out ProcessInformation pi);

        if (!success)
        {
            int errorCode = Marshal.GetLastWin32Error();
            ShellLogger.Error($"CreateProcess failed with error code {errorCode}");
            return false;
        }

        CloseHandle(pi.hProcess);
        CloseHandle(pi.hThread);
        return true;
    }

    public static bool Exists(string filename)
    {
        if (Array.Exists(Path.GetInvalidPathChars(), invalid => filename.Contains(invalid)))
            return false;

        return !filename.StartsWith("\\\\") && (File.Exists(filename) || Directory.Exists(filename));
    }

    public static int GetMenuDropAlignment()
    {
        int menuDropAlignment = 0;

        try
        {
            RegistryKey windowsKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Windows", false);

            if (windowsKey != null)
            {
                object menuDropAlignmentValue = windowsKey.GetValue("MenuDropAlignment");

                if (menuDropAlignmentValue != null)
                {
                    menuDropAlignment = Convert.ToInt32(menuDropAlignmentValue);
                }
            }
        }
        catch (Exception) { /* Ignore errors */ }

        return menuDropAlignment;
    }

    public static bool GetFileExtensionsVisible()
    {
        RegistryKey hideFileExt =
            Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced",
                false);
        object hideFileExtValue = hideFileExt?.GetValue("HideFileExt");

        return hideFileExtValue != null && hideFileExtValue.ToString() == "0";
    }

    public static bool IsFileVisible(string fileName)
    {
        if (Exists(fileName))
        {
            try
            {
                FileAttributes attributes = File.GetAttributes(fileName);
                return (((attributes & FileAttributes.Hidden) != FileAttributes.Hidden) && ((attributes & FileAttributes.System) != FileAttributes.System) && ((attributes & FileAttributes.Temporary) != FileAttributes.Temporary));
            }
            catch
            {
                return false;
            }
        }

        return false;
    }

    public static bool ShowFileProperties(string Filename)
    {
        ShellExecuteInfo info = new();
        info.cbSize = Marshal.SizeOf(info);
        info.lpVerb = "properties";
        info.lpFile = Filename;
        info.nShow = (int)WindowShowStyle.Show;
        info.fMask = SEE_MASK_INVOKEIDLIST;
        return ShellExecuteEx(ref info);
    }

    /// <summary>
    /// Calls the Windows OpenWith dialog (shell32.dll) to open the file specified.
    /// </summary>
    /// <param name="fileName">Path to the file to open</param>
    public static void ShowOpenWithDialog(string fileName)
    {
        Process owProc = new();
        owProc.StartInfo.UseShellExecute = true;
        owProc.StartInfo.FileName = Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\system32\rundll32.exe";
        owProc.StartInfo.Arguments = Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\system32\shell32.dll,OpenAs_RunDLL " + fileName;
        owProc.Start();
    }

    public static void StartTaskManager()
    {
        StartProcess("cmd.exe", "/K set __compat_layer=RunAsInvoker & start taskmgr.exe & exit", useShellExecute: true, hidden: true);
    }

    public static void ShowRunDialog(string title, string info)
    {
        SHRunFileDialog(IntPtr.Zero, IntPtr.Zero, null, title, info, RunFileDialogs.None);
    }

    public static void ShowWindowSwitcher()
    {
        ShellKeyCombo(VK.LWIN, VK.TAB);
    }

    public static void ShowActionCenter()
    {
        ShellKeyCombo(VK.LWIN, VK.KEY_A);
    }

    public static void ShowNotificationCenter()
    {
        if (EnvironmentHelper.IsWindows11OrBetter)
            ShellKeyCombo(VK.LWIN, VK.KEY_N);
        else
            ShellKeyCombo(VK.LWIN, VK.KEY_A);
    }

    public static void ShowStartMenu()
    {
        ShellKeyCombo(VK.LWIN, VK.LWIN);
    }

    public static void ShowStartContextMenu()
    {
        ShellKeyCombo(VK.LWIN, VK.KEY_X);
    }

    public static void ShowConfigPanel()
    {
        ShellKeyCombo(VK.LWIN, VK.KEY_I);
    }

    public static void ShowCopilot()
    {
        ShellKeyCombo(VK.LWIN, VK.KEY_C);
    }

    public enum MsSettings
    {
        None = 0,
        Display = 1,
        NightLight = 2,
        AdvancedScaling = 3,
        ConnectWirelessDisplay = 4,
        Graphics = 5,
        DisplayOrientation = 6,
        Sound = 7,
        SoundDeviceManager = 8,
        AppVolume = 9,
        Notifications = 10,
        FocusAssist = 11,
        QuietMomentScheduler = 12,
        QuietMomentPresentation = 13,
        QuietMomentGame = 14,
        PowerAndSleep = 15,
        Battery = 16,
        BatteryUsage = 17,
        BatterySaver = 18,
        Storage = 19,
        StoragePolicies = 20,
        SaveLocation = 21,
        TabletMode = 22,
        Multitasking = 23,
        Projecting = 24,
        SharedExperience = 25,
        Clipboard = 26,
        RemoteDesktop = 27,
        DeviceEncryption = 28,
        About = 29,
        Bluetooth = 30,
        Printers = 31,
        Mouse = 32,
        Touchpad = 33,
        Typing = 34,
        TextSuggestions = 35,
        Wheel = 36,
        Pan = 37,
        AutoPlay = 38,
        USB = 39,
        Phone = 40,
        AddPhone = 41,
        YourPhone = 42,
        Network = 43,
        NetworkStatus = 44,
        AvailableNetwork = 45,
        Cellular = 46,
        WiFi = 47,
        WiFiSettings = 48,
        WiFiCalling = 49,
        Ethernet = 50,
        DialUp = 51,
        DirectAccess = 52,
        VPN = 53,
        AirplaneMode = 54,
        MobileHotspot = 55,
        NFC = 56,
        DataUsage = 57,
        Proxy = 58,
        Personalization = 59,
        Background = 60,
        Colors = 61,
        LockScreen = 62,
        Themes = 63,
        Fonts = 64,
        PersonalizeStart = 65,
        PersonalizeStartPlaces = 66,
        Taskbar = 67,
        Apps = 68,
        AppsOptionalFeatures = 69,
        DefaultApps = 70,
        OfflineMaps = 71,
        DownloadMaps = 72,
        AppsWebSites = 73,
        VideoPlayback = 74,
        Startup = 75,
        YourAccount = 76,
        EmailAccounts = 77,
        SignIn = 78,
        WindowsHelloFace = 79,
        WindowsHelloFingerprint = 80,
        LogInSecurityKey = 81,
        DynamicLock = 82,
        AccessWorkAndSchool = 83,
        Family = 84,
        SetupKiok = 85,
        SyncSettings = 86,
        DateTime = 87,
        Region = 88,
        JapanIME = 89,
        PinyinIME = 90,
        WubiIME = 91,
        KoreaIME = 92,
        Language = 93,
        DisplayLanguage = 94,
        AddDisplayLanguage = 95,
        Speech = 96,
        Gamebar = 97,
        Captures = 98,
        Broadcasting = 99,
        GameMode = 100,
        Xbox = 101,
        AppsExtras = 102,
        EaseAccessDisplay = 103,
        EaseAccessMouseCursor = 104,
        EaseAccessTextCursor = 105,
        EaseAccessMagnifier = 106,
        EaseAccessColors = 107,
        EaseAccessAdaptiveColors = 108,
        EaseAccessNightLight = 109,
        EaseAccessHighContrast = 110,
        EaseAccessNarrator = 111,
        EaseAccessStartNarrator = 112,
        EaseAccessAudio = 113,
        EaseAccessClosedCaptions = 114,
        EaseAccessSpeech = 115,
        EaseAccessKeyboard = 116,
        EaseAccessMouse = 117,
        EaseAccessEyeControl = 118,
        SearchPermissions = 119,
        SearchingWindows = 120,
        SearchMoreDetails = 121,
        Privacy = 122,
        PrivacySpeech = 123,
        PrivacyInkType = 124,
        PrivacyDiagnostics = 125,
        PrivacyDiagnosticsData = 126,
        PrivacyActivity = 127,
        PrivacyLocation = 128,
        PrivacyWebcam = 129,
        PrivacyMicrophone = 130,
        PrivacyVoiceActivation = 131,
        PrivacyNotifications = 132,
        PrivacyAccount = 133,
        PrivacyContacts = 134,
        PrivacyCalendar = 135,
        PrivacyCallHistory = 136,
        PrivacyEmail = 137,
        PrivacyEyeTracker = 138,
        PrivacyTasks = 139,
        PrivacyMessaging = 140,
        PrivacyRadios = 141,
        PrivacyOtherDevices = 142,
        PrivacyBackgroundApps = 143,
        PrivacyAppDiagnostics = 144,
        PrivacyFileDownloads = 145,
        PrivacyDocuments = 146,
        PrivacyPictures = 147,
        PrivacyVideos = 148,
        PrivacyFileSystem = 149,
        WindowsUpdate = 150,
        WindowsUpdateCheckForUpdates = 151,
        WindowsUpdateHistory = 152,
        WindowsUpdateRestartOptions = 153,
        WindowsUpdateAvancedOptions = 154,
        WindowsUpdateChangeActiveHours = 155,
        WindowsUpdateOptionalUpdates = 156,
        DeliveryOptimization = 157,
        WindowsSecurity = 158,
        WindowsSecurityOpen = 159,
        Backup = 160,
        Troubleshoot = 161,
        Recovery = 162,
        Activation = 163,
        FindMyDevice = 164,
        Developers = 165,
        Insiders = 166,
        MixedReality = 167,
        MixedRealityAudioAndSpeech = 168,
        MixedRealityEnvironment = 169,
        MixedRealityHeadset = 170,
        MixedRealityUninstall = 171,
        SurfaceHubAccounts = 172,
        SurfaceHubTeamConferencing = 173,
        SurfaceHubTeamDeviceManager = 174,
        SurfaceHubSessionCleanUp = 175,
        SurfaceHubWelcomeScreen = 176,
    }

    public static void ShowSettings(MsSettings settings)
    {
        string command = "ms-settings:";
        switch (settings)
        {
            case MsSettings.None:
                break;
            case MsSettings.Display:
                command += "display";
                break;
            case MsSettings.NightLight:
                command += "nightlight";
                break;
            case MsSettings.AdvancedScaling:
                command += "display-advanced";
                break;
            case MsSettings.ConnectWirelessDisplay:
                command = "ms-settings-connectabledevices:devicediscovery";
                break;
            case MsSettings.Graphics:
                command += "display-advancedgraphics";
                break;
            case MsSettings.DisplayOrientation:
                command += "screenrotation";
                break;
            case MsSettings.Sound:
                command += "sound";
                break;
            case MsSettings.SoundDeviceManager:
                command += "sound-devices";
                break;
            case MsSettings.AppVolume:
                command += "apps-volume";
                break;
            case MsSettings.Notifications:
                command += "notifications";
                break;
            case MsSettings.FocusAssist:
                command += "quietmomentshome";
                break;
            case MsSettings.QuietMomentScheduler:
                command += "quietmomentsscheduled";
                break;
            case MsSettings.QuietMomentPresentation:
                command += "quietmomentspresentation";
                break;
            case MsSettings.QuietMomentGame:
                command += "quietmomentsgame";
                break;
            case MsSettings.PowerAndSleep:
                command += "powersleep";
                break;
            case MsSettings.Battery:
                command += "batterysaver";
                break;
            case MsSettings.BatteryUsage:
                command += "batterysaver-usagedetails";
                break;
            case MsSettings.BatterySaver:
                command += "batterysaver-settings";
                break;
            case MsSettings.Storage:
                command += "storagesense";
                break;
            case MsSettings.StoragePolicies:
                command += "storagepolicies";
                break;
            case MsSettings.SaveLocation:
                command += "savelocations";
                break;
            case MsSettings.TabletMode:
                command += "tabletmode";
                break;
            case MsSettings.Multitasking:
                command += "multitasking";
                break;
            case MsSettings.Projecting:
                command += "project";
                break;
            case MsSettings.SharedExperience:
                command += "crossdevice";
                break;
            case MsSettings.Clipboard:
                command += "clipboard";
                break;
            case MsSettings.RemoteDesktop:
                command += "remotedesktop";
                break;
            case MsSettings.DeviceEncryption:
                command += "deviceencryption";
                break;
            case MsSettings.About:
                command += "about";
                break;
            case MsSettings.Bluetooth:
                command += "bluetooth";
                break;
            case MsSettings.Printers:
                command += "printers";
                break;
            case MsSettings.Mouse:
                command += "mousetouchpad";
                break;
            case MsSettings.Touchpad:
                command += "devices-touchpad";
                break;
            case MsSettings.Typing:
                command += "typing";
                break;
            case MsSettings.TextSuggestions:
                command += "devicestyping-hwkbtextsuggestions";
                break;
            case MsSettings.Wheel:
                command += "wheel";
                break;
            case MsSettings.Pan:
                command += "pen";
                break;
            case MsSettings.AutoPlay:
                command += "autoplay";
                break;
            case MsSettings.USB:
                command += "usb";
                break;
            case MsSettings.Phone:
                command += "mobile-devices";
                break;
            case MsSettings.AddPhone:
                command += "mobile-devices-addphone";
                break;
            case MsSettings.YourPhone:
                command += "mobile-devices-addphone-direct";
                break;
            case MsSettings.Network:
                command += "network";
                break;
            case MsSettings.NetworkStatus:
                command += "network-status";
                break;
            case MsSettings.AvailableNetwork:
                command = "ms-availablenetworks:";
                break;
            case MsSettings.Cellular:
                command += "network-cellular";
                break;
            case MsSettings.WiFi:
                command += "network-wifi";
                break;
            case MsSettings.WiFiSettings:
                command += "network-wifisettings";
                break;
            case MsSettings.WiFiCalling:
                command += "network-wificalling";
                break;
            case MsSettings.Ethernet:
                command += "network-ethernet";
                break;
            case MsSettings.DialUp:
                command += "network-dialup";
                break;
            case MsSettings.DirectAccess:
                command += "network-directaccess";
                break;
            case MsSettings.VPN:
                command += "network-vpn";
                break;
            case MsSettings.AirplaneMode:
                command += "network-airplanemode";
                break;
            case MsSettings.MobileHotspot:
                command += "network-mobilehotspot";
                break;
            case MsSettings.NFC:
                command += "nfctransactions";
                break;
            case MsSettings.DataUsage:
                command += "datausage";
                break;
            case MsSettings.Proxy:
                command += "network-proxy";
                break;
            case MsSettings.Personalization:
                command += "personalization";
                break;
            case MsSettings.Background:
                command += "personalization-background";
                break;
            case MsSettings.Colors:
                command += "personalization-colors";
                break;
            case MsSettings.LockScreen:
                command += "lockscreen";
                break;
            case MsSettings.Themes:
                command += "themes";
                break;
            case MsSettings.Fonts:
                command += "fonts";
                break;
            case MsSettings.PersonalizeStart:
                command += "personalization-start";
                break;
            case MsSettings.PersonalizeStartPlaces:
                command += "personalization-start-places";
                break;
            case MsSettings.Taskbar:
                command += "taskbar";
                break;
            case MsSettings.Apps:
                command += "appsfeatures";
                break;
            case MsSettings.AppsOptionalFeatures:
                command += "optionalfeatures";
                break;
            case MsSettings.DefaultApps:
                command += "defaultapps";
                break;
            case MsSettings.OfflineMaps:
                command += "maps";
                break;
            case MsSettings.DownloadMaps:
                command += "maps-downloadmaps";
                break;
            case MsSettings.AppsWebSites:
                command += "appsforwebsites";
                break;
            case MsSettings.VideoPlayback:
                command += "videoplayback";
                break;
            case MsSettings.Startup:
                command += "startupapps";
                break;
            case MsSettings.YourAccount:
                command += "yourinfo";
                break;
            case MsSettings.EmailAccounts:
                command += "emailandaccounts";
                break;
            case MsSettings.SignIn:
                command += "signinoptions";
                break;
            case MsSettings.WindowsHelloFace:
                command += "signinoptions-launchfaceenrollment";
                break;
            case MsSettings.WindowsHelloFingerprint:
                command += "signinoptions-launchfingerprintenrollment";
                break;
            case MsSettings.LogInSecurityKey:
                command += "signinoptions-launchsecuritykeyenrollment";
                break;
            case MsSettings.DynamicLock:
                command += "signinoptions-dynamiclock";
                break;
            case MsSettings.AccessWorkAndSchool:
                command += "workplace";
                break;
            case MsSettings.Family:
                command += "otherusers";
                break;
            case MsSettings.SetupKiok:
                command += "assignedaccess";
                break;
            case MsSettings.SyncSettings:
                command += "sync";
                break;
            case MsSettings.DateTime:
                command += "dateandtime";
                break;
            case MsSettings.Region:
                command += "regionformatting";
                break;
            case MsSettings.JapanIME:
                command += "regionlanguage-jpnime";
                break;
            case MsSettings.PinyinIME:
                command += "regionlanguage-chsime-pinyin";
                break;
            case MsSettings.WubiIME:
                command += "regionlanguage-chsime-wubi";
                break;
            case MsSettings.KoreaIME:
                command += "regionlanguage-korime";
                break;
            case MsSettings.Language:
                command += "regionlanguage";
                break;
            case MsSettings.DisplayLanguage:
                command += "regionlanguage-setdisplaylanguage";
                break;
            case MsSettings.AddDisplayLanguage:
                command += "regionlanguage-adddisplaylanguage";
                break;
            case MsSettings.Speech:
                command += "speech";
                break;
            case MsSettings.Gamebar:
                command += "gaming-gamebar";
                break;
            case MsSettings.Captures:
                command += "gaming-gamedvr";
                break;
            case MsSettings.Broadcasting:
                command += "gaming-broadcasting";
                break;
            case MsSettings.GameMode:
                command += "gaming-gamemode";
                break;
            case MsSettings.Xbox:
                command += "gaming-xboxnetworking";
                break;
            case MsSettings.AppsExtras:
                command += "extras";
                break;
            case MsSettings.EaseAccessDisplay:
                command += "easeofaccess-display";
                break;
            case MsSettings.EaseAccessMouseCursor:
                command += "easeofaccess-cursorandpointersize";
                break;
            case MsSettings.EaseAccessTextCursor:
                command += "easeofaccess-cursor";
                break;
            case MsSettings.EaseAccessMagnifier:
                command += "easeofaccess-magnifier";
                break;
            case MsSettings.EaseAccessColors:
                command += "easeofaccess-colorfilter";
                break;
            case MsSettings.EaseAccessAdaptiveColors:
                command += "easeofaccess-colorfilter-adaptivecolorlink";
                break;
            case MsSettings.EaseAccessNightLight:
                command += "easeofaccess-colorfilter-bluelightlink";
                break;
            case MsSettings.EaseAccessHighContrast:
                command += "easeofaccess-highcontrast";
                break;
            case MsSettings.EaseAccessNarrator:
                command += "easeofaccess-narrator";
                break;
            case MsSettings.EaseAccessStartNarrator:
                command += "easeofaccess-narrator-isautostartenabled";
                break;
            case MsSettings.EaseAccessAudio:
                command += "easeofaccess-audio";
                break;
            case MsSettings.EaseAccessClosedCaptions:
                command += "easeofaccess-closedcaptioning";
                break;
            case MsSettings.EaseAccessSpeech:
                command += "easeofaccess-speechrecognition";
                break;
            case MsSettings.EaseAccessKeyboard:
                command += "easeofaccess-keyboard";
                break;
            case MsSettings.EaseAccessMouse:
                command += "easeofaccess-mouse";
                break;
            case MsSettings.EaseAccessEyeControl:
                command += "easeofaccess-eyecontrol";
                break;
            case MsSettings.SearchPermissions:
                command += "search-permissions";
                break;
            case MsSettings.SearchingWindows:
                command += "cortana-windowssearch";
                break;
            case MsSettings.SearchMoreDetails:
                command += "search-moredetails";
                break;
            case MsSettings.Privacy:
                command += "privacy";
                break;
            case MsSettings.PrivacySpeech:
                command += "privacy-speech";
                break;
            case MsSettings.PrivacyInkType:
                command += "privacy-speechtyping";
                break;
            case MsSettings.PrivacyDiagnostics:
                command += "privacy-feedback";
                break;
            case MsSettings.PrivacyDiagnosticsData:
                command += "privacy-feedback-telemetryviewergroup";
                break;
            case MsSettings.PrivacyActivity:
                command += "privacy-activityhistory";
                break;
            case MsSettings.PrivacyLocation:
                command += "privacy-location";
                break;
            case MsSettings.PrivacyWebcam:
                command += "privacy-webcam";
                break;
            case MsSettings.PrivacyMicrophone:
                command += "privacy-microphone";
                break;
            case MsSettings.PrivacyVoiceActivation:
                command += "privacy-voiceactivation";
                break;
            case MsSettings.PrivacyNotifications:
                command += "privacy-notifications";
                break;
            case MsSettings.PrivacyAccount:
                command += "privacy-accountinfo";
                break;
            case MsSettings.PrivacyContacts:
                command += "privacy-contacts";
                break;
            case MsSettings.PrivacyCalendar:
                command += "privacy-calendar";
                break;
            case MsSettings.PrivacyCallHistory:
                command += "privacy-callhistory";
                break;
            case MsSettings.PrivacyEmail:
                command += "privacy-email";
                break;
            case MsSettings.PrivacyEyeTracker:
                command += "privacy-eyetracker";
                break;
            case MsSettings.PrivacyTasks:
                command += "privacy-tasks";
                break;
            case MsSettings.PrivacyMessaging:
                command += "privacy-messaging";
                break;
            case MsSettings.PrivacyRadios:
                command += "privacy-radios";
                break;
            case MsSettings.PrivacyOtherDevices:
                command += "privacy-customdevices";
                break;
            case MsSettings.PrivacyBackgroundApps:
                command += "privacy-backgroundapps";
                break;
            case MsSettings.PrivacyAppDiagnostics:
                command += "privacy-appdiagnostics";
                break;
            case MsSettings.PrivacyFileDownloads:
                command += "privacy-automaticfiledownloads";
                break;
            case MsSettings.PrivacyDocuments:
                command += "privacy-documents";
                break;
            case MsSettings.PrivacyPictures:
                command += "privacy-pictures";
                break;
            case MsSettings.PrivacyVideos:
                command += "privacy-documents";
                break;
            case MsSettings.PrivacyFileSystem:
                command += "privacy-broadfilesystemaccess";
                break;
            case MsSettings.WindowsUpdate:
                command += "windowsupdate";
                break;
            case MsSettings.WindowsUpdateCheckForUpdates:
                command += "windowsupdate-action";
                break;
            case MsSettings.WindowsUpdateHistory:
                command += "windowsupdate-history";
                break;
            case MsSettings.WindowsUpdateRestartOptions:
                command += "windowsupdate-restartoptions";
                break;
            case MsSettings.WindowsUpdateAvancedOptions:
                command += "windowsupdate-options";
                break;
            case MsSettings.WindowsUpdateChangeActiveHours:
                command += "windowsupdate-activehours";
                break;
            case MsSettings.WindowsUpdateOptionalUpdates:
                command += "windowsupdate-optionalupdates";
                break;
            case MsSettings.DeliveryOptimization:
                command += "delivery-optimization";
                break;
            case MsSettings.WindowsSecurity:
                command += "windowsdefender";
                break;
            case MsSettings.WindowsSecurityOpen:
                command = "windowsdefender:";
                break;
            case MsSettings.Backup:
                command += "backup";
                break;
            case MsSettings.Troubleshoot:
                command += "troubleshoot";
                break;
            case MsSettings.Recovery:
                command += "recovery";
                break;
            case MsSettings.Activation:
                command += "activation";
                break;
            case MsSettings.FindMyDevice:
                command += "findmydevice";
                break;
            case MsSettings.Developers:
                command += "developers";
                break;
            case MsSettings.Insiders:
                command += "windowsinsider";
                break;
            case MsSettings.MixedReality:
                command += "holographic";
                break;
            case MsSettings.MixedRealityAudioAndSpeech:
                command += "holographic-audio";
                break;
            case MsSettings.MixedRealityEnvironment:
                command += "privacy-holographic-environment";
                break;
            case MsSettings.MixedRealityHeadset:
                command += "holographic-headset";
                break;
            case MsSettings.MixedRealityUninstall:
                command += "holographic-management";
                break;
            case MsSettings.SurfaceHubAccounts:
                command += "surfacehub-accounts";
                break;
            case MsSettings.SurfaceHubTeamConferencing:
                command += "surfacehub-calling";
                break;
            case MsSettings.SurfaceHubTeamDeviceManager:
                command += "surfacehub-devicemanagenent";
                break;
            case MsSettings.SurfaceHubSessionCleanUp:
                command += "surfacehub-sessioncleanup";
                break;
            case MsSettings.SurfaceHubWelcomeScreen:
                command += "surfacehub-welcome";
                break;
            default:
                command = null;
                break;
        }
        if (!string.IsNullOrWhiteSpace(command))
        {
            ExecuteProcess(command);
        }
    }

    /// <summary>
    /// Send file to recycle bin
    /// </summary>
    /// <param name="path">Location of directory or file to recycle</param>
    /// <param name="flags">FileOperationFlags to add in addition to FOF_ALLOWUNDO</param>
    public static bool SendToRecycleBin(string path, FileOperations flags)
    {
        try
        {
            ShFileOpStruct fs = new()
            {
                wFunc = FileOperationType.FO_DELETE,
                pFrom = path + '\0' + '\0',
                fFlags = FileOperations.FOF_ALLOWUNDO | flags,
            };
            SHFileOperation(ref fs);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Send file to recycle bin.  Display dialog, display warning if files are too big to fit (FOF_WANTNUKEWARNING)
    /// </summary>
    /// <param name="path">Location of directory or file to recycle</param>
    public static bool SendToRecycleBin(string path)
    {
        return SendToRecycleBin(path, FileOperations.FOF_NOCONFIRMATION | FileOperations.FOF_WANTNUKEWARNING);
    }

    /// <summary>
    /// Send file silently to recycle bin.  Surpress dialog, surpress errors, delete if too large.
    /// </summary>
    /// <param name="path">Location of directory or file to recycle</param>
    public static bool MoveToRecycleBin(string path)
    {
        return SendToRecycleBin(path, FileOperations.FOF_NOCONFIRMATION | FileOperations.FOF_NOERRORUI | FileOperations.FOF_SILENT);

    }

    public static void ToggleDesktopIcons(bool enable)
    {
        if (!EnvironmentHelper.IsAppRunningAsShell)
        {
            IntPtr toggleDesktopCommand = new(0x7402);
            IntPtr hWnd = FindWindowEx(FindWindow("Progman", "Program Manager"), IntPtr.Zero, ShellViewName, "");

            if (hWnd == IntPtr.Zero)
            {
                EnumWindows((hwnd, lParam) =>
                {
                    StringBuilder cName = new(256);
                    GetClassName(hwnd, cName, cName.Capacity);
                    if (cName.ToString() == "WorkerW")
                    {
                        IntPtr child = FindWindowEx(hwnd, IntPtr.Zero, ShellViewName, null);
                        if (child != IntPtr.Zero)
                        {
                            hWnd = child;
                            return true;
                        }
                    }

                    return true;
                }, 0);
            }

            if (IsDesktopVisible() != enable)
            {
                SendMessageTimeout(hWnd, (uint)WM.COMMAND, toggleDesktopCommand, IntPtr.Zero, 2, 200, ref hWnd);
            }
        }
    }

    public static string GetPathForHandle(IntPtr hWnd)
    {
        StringBuilder outFileName = new(1024);

        // get process id
        uint procId = GetProcIdForHandle(hWnd);

        if (procId != 0)
        {
            // open process
            // QueryLimitedInformation flag allows us to access elevated applications as well
            IntPtr hProc = OpenProcess(ProcessAccess.QueryLimitedInformation, false, (int)procId);

            // get filename
            int len = outFileName.Capacity;
            QueryFullProcessImageName(hProc, 0, outFileName, ref len);
            CloseHandle(hProc);
            outFileName.Replace("Excluded,", "");
            outFileName.Replace(",SFC protected", "");
        }

        return outFileName.ToString();
    }

    public static uint GetProcIdForHandle(IntPtr hWnd)
    {
        GetWindowThreadProcessId(hWnd, out uint procId);
        return procId;
    }

    public static string GetAppUserModelIdPropertyForHandle(IntPtr hWnd, uint pid = 5)
    {
        string aumid = string.Empty;

        Guid g = new("886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99");
        SHGetPropertyStoreForWindow(hWnd, ref g, out IPropertyStore propStore);

        PropertyKey PKEY_AppUserModel_ID = new()
        {
            fmtid = new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3"),
            pid = pid,
        };

        if (propStore != null)
        {
            propStore.GetValue(PKEY_AppUserModel_ID, out PropVariant prop);

            try
            {
                aumid = prop.Value.ToString();
            }
            catch (Exception)
            { /* Ignore errors */ }

            prop.Clear();
        }

        return aumid;
    }

    public static PropVariant GetWindowPropertyKeyStore(IntPtr hwnd, PropertyKey key)
    {
        PropVariant propVariant = default;
        Guid guid = typeof(IPropertyStore).GUID;
        SHGetPropertyStoreForWindow(hwnd, ref guid, out IPropertyStore propStore);
        propStore?.GetValue(ref key, out propVariant);
        return propVariant;
    }

    public static PropVariant GetWindowPropertyKey(IPropertyStore propertyStore, PropertyKey key)
    {
        PropVariant result = default;
        propertyStore?.GetValue(ref key, out result);
        return result;
    }

    public static string GetAppUserModelIdForHandle(IntPtr hWnd)
    {
        if (!EnvironmentHelper.IsWindows8OrBetter)
        {
            return GetAppUserModelIdPropertyForHandle(hWnd);
        }

        GetWindowThreadProcessId(hWnd, out uint procId);

        if (procId > 0 && procId < int.MaxValue)
        {
            IntPtr hProcess = OpenProcess(ProcessAccess.QueryLimitedInformation, false, (int)procId);

            uint len = 130;
            StringBuilder outAumid = new((int)len);

            GetApplicationUserModelId(hProcess, ref len, outAumid);
            CloseHandle(hProcess);
            if (outAumid.Length > 0)
            {
                return outAumid.ToString();
            }
        }

        return GetAppUserModelIdPropertyForHandle(hWnd);
    }

    /// <summary>
    /// Calls the LockWorkStation method on the User32 API.
    /// </summary>
    public static void Lock()
    {
        LockWorkStation();
    }

    /// <summary>
    /// Calls the logoff method on the Win32 API.
    /// </summary>
    public static void Logoff()
    {
        ExitWindowsEx((uint)ExitWindows.Logoff, 0x0);
    }

    private static bool IsDesktopVisible()
    {
        IntPtr hWnd = GetWindow(FindWindowEx(FindWindow("Progman", "Program Manager"), IntPtr.Zero, ShellViewName, ""), GetWindowCmd.GW_CHILD);


        if (hWnd == IntPtr.Zero)
        {
            EnumWindows((hwnd, lParam) =>
            {
                StringBuilder cName = new(256);
                GetClassName(hwnd, cName, cName.Capacity);
                if (cName.ToString() == "WorkerW")
                {
                    IntPtr child = FindWindowEx(hwnd, IntPtr.Zero, ShellViewName, null);
                    if (child != IntPtr.Zero)
                    {
                        hWnd = FindWindowEx(child, IntPtr.Zero, "SysListView32", "FolderView");
                        return false;
                    }
                }
                return true;
            }, 0);
        }

        WindowInfo info = new();
        info.cbSize = (uint)Marshal.SizeOf(info);
        GetWindowInfo(hWnd, ref info);
        return (info.dwStyle & 0x10000000) == 0x10000000;
    }

    public static void ShellKeyCombo(VK wVk_1, VK wVk_2, VK wVk_3 = VK.NONE)
    {
        Input[] inputs = new Input[(wVk_3 == VK.NONE ? 4 : 6)];

        inputs[0].type = TypeInput.Keyboard;
        inputs[0].mkhi.ki.time = 0;
        inputs[0].mkhi.ki.wScan = 0;
        inputs[0].mkhi.ki.dwExtraInfo = GetMessageExtraInfo();
        inputs[0].mkhi.ki.wVk = (ushort)wVk_1;
        inputs[0].mkhi.ki.dwFlags = 0;

        inputs[1].type = TypeInput.Keyboard;
        inputs[1].mkhi.ki.wScan = 0;
        inputs[1].mkhi.ki.dwExtraInfo = GetMessageExtraInfo();
        inputs[1].mkhi.ki.wVk = (ushort)wVk_2;
        inputs[1].mkhi.ki.dwFlags = 0;

        int position = 2;
        if (wVk_3 != VK.NONE)
        {
            inputs[position].type = TypeInput.Keyboard;
            inputs[position].mkhi.ki.wScan = 0;
            inputs[position].mkhi.ki.dwExtraInfo = GetMessageExtraInfo();
            inputs[position].mkhi.ki.wVk = (ushort)wVk_3;
            inputs[position].mkhi.ki.dwFlags = 0;
            position++;

            inputs[position].type = TypeInput.Keyboard;
            inputs[position].mkhi.ki.wScan = 0;
            inputs[position].mkhi.ki.dwExtraInfo = GetMessageExtraInfo();
            inputs[position].mkhi.ki.wVk = (ushort)wVk_3;
            inputs[position].mkhi.ki.dwFlags = KeyEventScans.KeyUp;
            position++;
        }

        inputs[position].type = TypeInput.Keyboard;
        inputs[position].mkhi.ki.wScan = 0;
        inputs[position].mkhi.ki.dwExtraInfo = GetMessageExtraInfo();
        inputs[position].mkhi.ki.wVk = (ushort)wVk_2;
        inputs[position].mkhi.ki.dwFlags = KeyEventScans.KeyUp;
        position++;

        inputs[position].type = TypeInput.Keyboard;
        inputs[position].mkhi.ki.wScan = 0;
        inputs[position].mkhi.ki.dwExtraInfo = GetMessageExtraInfo();
        inputs[position].mkhi.ki.wVk = (ushort)wVk_1;
        inputs[position].mkhi.ki.dwFlags = KeyEventScans.KeyUp;

        SendInput((uint)(wVk_3 == VK.NONE ? 4 : 6), inputs, Marshal.SizeOf(typeof(Input)));
    }

    public static void SetShellReadyEvent()
    {
        int hShellReadyEvent = OpenEvent(EVENT_MODIFY_STATE, true, @"ShellDesktopSwitchEvent");

        if (hShellReadyEvent != 0)
        {
            SetEvent(hShellReadyEvent);
            CloseHandle(hShellReadyEvent);
        }
    }

    public static bool IsSameFile(string path1, string path2)
    {
        if (path1 == path2) return true;

        using SafeFileHandle sfh1 = CreateFile(path1, FileAccess.Read, FileShare.ReadWrite,
            IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);
        if (sfh1.IsInvalid)
            ShellLogger.Error($"Win32 error occured when trying to open file {path1}", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));

        using SafeFileHandle sfh2 = CreateFile(path2, FileAccess.Read, FileShare.ReadWrite,
            IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);
        if (sfh2.IsInvalid)
            ShellLogger.Error($"Win32 error occured when trying to open file {path2}", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));

        bool result1 = GetFileInformationByHandle(sfh1, out ByHandleFileInformation fileInfo1);
        if (!result1)
            ShellLogger.Error($"GetFileInformationByHandle has failed on {path1}");

        bool result2 = GetFileInformationByHandle(sfh2, out ByHandleFileInformation fileInfo2);
        if (!result2)
            ShellLogger.Error($"GetFileInformationByHandle has failed on {path2}");

        return fileInfo1.VolumeSerialNumber == fileInfo2.VolumeSerialNumber
               && fileInfo1.FileIndexHigh == fileInfo2.FileIndexHigh
               && fileInfo1.FileIndexLow == fileInfo2.FileIndexLow;
    }
}
