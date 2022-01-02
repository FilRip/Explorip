﻿using System;

namespace Explorip.WinAPI
{
    public static class Commun
    {
        public enum HRESULT : int
        {
            S_OK = 0,
            S_FALSE = 1,
            E_NOINTERFACE = unchecked((int)0x80004002),
            E_NOTIMPL = unchecked((int)0x80004001),
            E_FAIL = unchecked((int)0x80004005),
            E_UNEXPECTED = unchecked((int)0x8000FFFF),
            E_OUTOFMEMORY = unchecked((int)0x8007000E)
        }

        public const int DRAGDROP_S_DROP = 0x00040100;
        public const int DRAGDROP_S_CANCEL = 0x00040101;
        public const int DRAGDROP_S_USEDEFAULTCURSORS = 0x00040102;

        [Flags()]
        public enum WM : uint
        {
            ACTIVATE = 0x6,
            ACTIVATEAPP = 0x1C,
            AFXFIRST = 0x360,
            AFXLAST = 0x37F,
            APP = 0x8000,
            ASKCBFORMATNAME = 0x30C,
            CANCELJOURNAL = 0x4B,
            CANCELMODE = 0x1F,
            CAPTURECHANGED = 0x215,
            CHANGECBCHAIN = 0x30D,
            CHAR = 0x102,
            CHARTOITEM = 0x2F,
            CHILDACTIVATE = 0x22,
            CLEAR = 0x303,
            CLOSE = 0x10,
            COMMAND = 0x111,
            COMPACTING = 0x41,
            COMPAREITEM = 0x39,
            CONTEXTMENU = 0x7B,
            COPY = 0x301,
            COPYDATA = 0x4A,
            CREATE = 0x1,
            CTLCOLORBTN = 0x135,
            CTLCOLORDLG = 0x136,
            CTLCOLOREDIT = 0x133,
            CTLCOLORLISTBOX = 0x134,
            CTLCOLORMSGBOX = 0x132,
            CTLCOLORSCROLLBAR = 0x137,
            CTLCOLORSTATIC = 0x138,
            CUT = 0x300,
            DEADCHAR = 0x103,
            DELETEITEM = 0x2D,
            DESTROY = 0x2,
            DESTROYCLIPBOARD = 0x307,
            DEVICECHANGE = 0x219,
            DEVMODECHANGE = 0x1B,
            DISPLAYCHANGE = 0x7E,
            DRAWCLIPBOARD = 0x308,
            DRAWITEM = 0x2B,
            DROPFILES = 0x233,
            ENABLE = 0xA,
            ENDSESSION = 0x16,
            ENTERIDLE = 0x121,
            ENTERMENULOOP = 0x211,
            ENTERSIZEMOVE = 0x231,
            ERASEBKGND = 0x14,
            EXITMENULOOP = 0x212,
            EXITSIZEMOVE = 0x232,
            FONTCHANGE = 0x1D,
            GETDLGCODE = 0x87,
            GETFONT = 0x31,
            GETHOTKEY = 0x33,
            GETICON = 0x7F,
            GETMINMAXINFO = 0x24,
            GETOBJECT = 0x3D,
            GETSYSMENU = 0x313,
            GETTEXT = 0xD,
            GETTEXTLENGTH = 0xE,
            HANDHELDFIRST = 0x358,
            HANDHELDLAST = 0x35F,
            HELP = 0x53,
            HOTKEY = 0x312,
            HSCROLL = 0x114,
            HSCROLLCLIPBOARD = 0x30E,
            ICONERASEBKGND = 0x27,
            IME_CHAR = 0x286,
            IME_COMPOSITION = 0x10F,
            IME_COMPOSITIONFULL = 0x284,
            IME_CONTROL = 0x283,
            IME_ENDCOMPOSITION = 0x10E,
            IME_KEYDOWN = 0x290,
            IME_KEYLAST = 0x10F,
            IME_KEYUP = 0x291,
            IME_NOTIFY = 0x282,
            IME_REQUEST = 0x288,
            IME_SELECT = 0x285,
            IME_SETCONTEXT = 0x281,
            IME_STARTCOMPOSITION = 0x10D,
            INITDIALOG = 0x110,
            INITMENU = 0x116,
            INITMENUPOPUP = 0x117,
            INPUTLANGCHANGE = 0x51,
            INPUTLANGCHANGEREQUEST = 0x50,
            KEYDOWN = 0x100,
            KEYFIRST = 0x100,
            KEYLAST = 0x108,
            KEYUP = 0x101,
            KILLFOCUS = 0x8,
            LBUTTONDBLCLK = 0x203,
            LBUTTONDOWN = 0x201,
            LBUTTONUP = 0x202,
            LVM_GETEDITCONTROL = 0x1018,
            LVM_SETIMAGELIST = 0x1003,
            MBUTTONDBLCLK = 0x209,
            MBUTTONDOWN = 0x207,
            MBUTTONUP = 0x208,
            MDIACTIVATE = 0x222,
            MDICASCADE = 0x227,
            MDICREATE = 0x220,
            MDIDESTROY = 0x221,
            MDIGETACTIVE = 0x229,
            MDIICONARRANGE = 0x228,
            MDIMAXIMIZE = 0x225,
            MDINEXT = 0x224,
            MDIREFRESHMENU = 0x234,
            MDIRESTORE = 0x223,
            MDISETMENU = 0x230,
            MDITILE = 0x226,
            MEASUREITEM = 0x2C,
            MENUCHAR = 0x120,
            MENUCOMMAND = 0x126,
            MENUDRAG = 0x123,
            MENUGETOBJECT = 0x124,
            MENURBUTTONUP = 0x122,
            MENUSELECT = 0x11F,
            MOUSEACTIVATE = 0x21,
            MOUSEFIRST = 0x200,
            MOUSEHOVER = 0x2A1,
            MOUSELAST = 0x20A,
            MOUSELEAVE = 0x2A3,
            MOUSEMOVE = 0x200,
            MOUSEWHEEL = 0x20A,
            MOVE = 0x3,
            MOVING = 0x216,
            NCACTIVATE = 0x86,
            NCCALCSIZE = 0x83,
            NCCREATE = 0x81,
            NCDESTROY = 0x82,
            NCHITTEST = 0x84,
            NCLBUTTONDBLCLK = 0xA3,
            NCLBUTTONDOWN = 0xA1,
            NCLBUTTONUP = 0xA2,
            NCMBUTTONDBLCLK = 0xA9,
            NCMBUTTONDOWN = 0xA7,
            NCMBUTTONUP = 0xA8,
            NCMOUSEHOVER = 0x2A0,
            NCMOUSELEAVE = 0x2A2,
            NCMOUSEMOVE = 0xA0,
            NCPAINT = 0x85,
            NCRBUTTONDBLCLK = 0xA6,
            NCRBUTTONDOWN = 0xA4,
            NCRBUTTONUP = 0xA5,
            NEXTDLGCTL = 0x28,
            NEXTMENU = 0x213,
            NOTIFY = 0x4E,
            NOTIFYFORMAT = 0x55,
            NULL = 0x0,
            PAINT = 0xF,
            PAINTCLIPBOARD = 0x309,
            PAINTICON = 0x26,
            PALETTECHANGED = 0x311,
            PALETTEISCHANGING = 0x310,
            PARENTNOTIFY = 0x210,
            PASTE = 0x302,
            PENWINFIRST = 0x380,
            PENWINLAST = 0x38F,
            POWER = 0x48,
            PRINT = 0x317,
            PRINTCLIENT = 0x318,
            QUERYDRAGICON = 0x37,
            QUERYENDSESSION = 0x11,
            QUERYNEWPALETTE = 0x30F,
            QUERYOPEN = 0x13,
            QUEUESYNC = 0x23,
            QUIT = 0x12,
            RBUTTONDBLCLK = 0x206,
            RBUTTONDOWN = 0x204,
            RBUTTONUP = 0x205,
            RENDERALLFORMATS = 0x306,
            RENDERFORMAT = 0x305,
            SETCURSOR = 0x20,
            SETFOCUS = 0x7,
            SETFONT = 0x30,
            SETHOTKEY = 0x32,
            SETICON = 0x80,
            SETMARGINS = 0xD3,
            SETREDRAW = 0xB,
            SETTEXT = 0xC,
            SETTINGCHANGE = 0x1A,
            SHOWWINDOW = 0x18,
            SIZE = 0x5,
            SIZECLIPBOARD = 0x30B,
            SIZING = 0x214,
            SPOOLERSTATUS = 0x2A,
            STYLECHANGED = 0x7D,
            STYLECHANGING = 0x7C,
            SYNCPAINT = 0x88,
            SYSCHAR = 0x106,
            SYSCOLORCHANGE = 0x15,
            SYSCOMMAND = 0x112,
            SYSDEADCHAR = 0x107,
            SYSKEYDOWN = 0x104,
            SYSKEYUP = 0x105,
            TCARD = 0x52,
            TIMECHANGE = 0x1E,
            TIMER = 0x113,
            TVM_GETEDITCONTROL = 0x110F,
            TVM_SETIMAGELIST = 0x1109,
            UNDO = 0x304,
            UNINITMENUPOPUP = 0x125,
            USER = 0x400,
            USERCHANGED = 0x54,
            VKEYTOITEM = 0x2E,
            VSCROLL = 0x115,
            VSCROLLCLIPBOARD = 0x30A,
            WINDOWPOSCHANGED = 0x47,
            WINDOWPOSCHANGING = 0x46,
            WININICHANGE = 0x1A,
            SH_NOTIFY = 0x0401
        }

        public static Guid CLSID_DragDropHelper = new Guid("{4657278A-411B-11d2-839A-00C04FD918D0}");

        public static class KnownFolder
        {
            // Synchronized with Windows SDK 10.0.16299.0 KnownFolders.h
            public static Guid NetworkFolder = Guid.Parse("D20BEEC4-5CA8-4905-AE3B-BF251EA09B53");
            public static Guid ComputerFolder = Guid.Parse("0AC0837C-BBF8-452A-850D-79D08E667CA7");
            public static Guid InternetFolder = Guid.Parse("4D9F7874-4E0C-4904-967B-40B0D20C3E4B");
            public static Guid ControlPanelFolder = Guid.Parse("82A74AEB-AEB4-465C-A014-D097EE346D63");
            public static Guid PrintersFolder = Guid.Parse("76FC4E2D-D6AD-4519-A663-37BD56068185");
            public static Guid SyncManagerFolder = Guid.Parse("43668BF8-C14E-49B2-97C9-747784D784B7");
            public static Guid SyncSetupFolder = Guid.Parse("0F214138-B1D3-4A90-BBA9-27CBC0C5389A");
            public static Guid ConflictFolder = Guid.Parse("4BFEFB45-347D-4006-A5BE-AC0CB0567192");
            public static Guid SyncResultsFolder = Guid.Parse("289A9A43-BE44-4057-A41B-587A76D7E7F9");
            public static Guid RecycleBinFolder = Guid.Parse("B7534046-3ECB-4C18-BE4E-64CD4CB7D6AC");
            public static Guid ConnectionsFolder = Guid.Parse("6F0CD92B-2E97-45D1-88FF-B0D186B8DEDD");
            public static Guid Fonts = Guid.Parse("FD228CB7-AE11-4AE3-864C-16F3910AB8FE");
            public static Guid Desktop = Guid.Parse("B4BFCC3A-DB2C-424C-B029-7FE99A87C641");
            public static Guid Startup = Guid.Parse("B97D20BB-F46A-4C97-BA10-5E3608430854");
            public static Guid Programs = Guid.Parse("A77F5D77-2E2B-44C3-A6A2-ABA601054A51");
            public static Guid StartMenu = Guid.Parse("625B53C3-AB48-4EC1-BA1F-A1EF4146FC19");
            public static Guid Recent = Guid.Parse("AE50C081-EBD2-438A-8655-8A092E34987A");
            public static Guid SendTo = Guid.Parse("8983036C-27C0-404B-8F08-102D10DCFD74");
            public static Guid Documents = Guid.Parse("FDD39AD0-238F-46AF-ADB4-6C85480369C7");
            public static Guid Favorites = Guid.Parse("1777F761-68AD-4D8A-87BD-30B759FA33DD");
            public static Guid NetHood = Guid.Parse("C5ABBF53-E17F-4121-8900-86626FC2C973");
            public static Guid PrintHood = Guid.Parse("9274BD8D-CFD1-41C3-B35E-B13F55A758F4");
            public static Guid Templates = Guid.Parse("A63293E8-664E-48DB-A079-DF759E0509F7");
            public static Guid CommonStartup = Guid.Parse("82A5EA35-D9CD-47C5-9629-E15D2F714E6E");
            public static Guid CommonPrograms = Guid.Parse("0139D44E-6AFE-49F2-8690-3DAFCAE6FFB8");
            public static Guid CommonStartMenu = Guid.Parse("A4115719-D62E-491D-AA7C-E74B8BE3B067");
            public static Guid PublicDesktop = Guid.Parse("C4AA340D-F20F-4863-AFEF-F87EF2E6BA25");
            public static Guid ProgramData = Guid.Parse("62AB5D82-FDC1-4DC3-A9DD-070D1D495D97");
            public static Guid CommonTemplates = Guid.Parse("B94237E7-57AC-4347-9151-B08C6C32D1F7");
            public static Guid PublicDocuments = Guid.Parse("ED4824AF-DCE4-45A8-81E2-FC7965083634");
            public static Guid RoamingAppData = Guid.Parse("3EB685DB-65F9-4CF6-A03A-E3EF65729F3D");
            public static Guid LocalAppData = Guid.Parse("F1B32785-6FBA-4FCF-9D55-7B8E7F157091");
            public static Guid LocalAppDataLow = Guid.Parse("A520A1A4-1780-4FF6-BD18-167343C5AF16");
            public static Guid InternetCache = Guid.Parse("352481E8-33BE-4251-BA85-6007CAEDCF9D");
            public static Guid Cookies = Guid.Parse("2B0F765D-C0E9-4171-908E-08A611B84FF6");
            public static Guid History = Guid.Parse("D9DC8A3B-B784-432E-A781-5A1130A75963");
            public static Guid System = Guid.Parse("1AC14E77-02E7-4E5D-B744-2EB1AE5198B7");
            public static Guid SystemX86 = Guid.Parse("D65231B0-B2F1-4857-A4CE-A8E7C6EA7D27");
            public static Guid Windows = Guid.Parse("F38BF404-1D43-42F2-9305-67DE0B28FC23");
            public static Guid Profile = Guid.Parse("5E6C858F-0E22-4760-9AFE-EA3317B67173");
            public static Guid Pictures = Guid.Parse("33E28130-4E1E-4676-835A-98395C3BC3BB");
            public static Guid ProgramFilesX86 = Guid.Parse("7C5A40EF-A0FB-4BFC-874A-C0F2E0B9FA8E");
            public static Guid ProgramFilesCommonX86 = Guid.Parse("DE974D24-D9C6-4D3E-BF91-F4455120B917");
            public static Guid ProgramFilesX64 = Guid.Parse("6D809377-6AF0-444B-8957-A3773F02200E");
            public static Guid ProgramFilesCommonX64 = Guid.Parse("6365D5A7-0F0D-45E5-87F6-0DA56B6A4F7D");
            public static Guid ProgramFiles = Guid.Parse("905E63B6-C1BF-494E-B29C-65B732D3D21A");
            public static Guid ProgramFilesCommon = Guid.Parse("F7F1ED05-9F6D-47A2-AAAE-29D317C6F066");
            public static Guid UserProgramFiles = Guid.Parse("5CD7AEE2-2219-4A67-B85D-6C9CE15660CB");
            public static Guid UserProgramFilesCommon = Guid.Parse("BCBD3057-CA5C-4622-B42D-BC56DB0AE516");
            public static Guid AdminTools = Guid.Parse("724EF170-A42D-4FEF-9F26-B60E846FBA4F");
            public static Guid CommonAdminTools = Guid.Parse("D0384E7D-BAC3-4797-8F14-CBA229B392B5");
            public static Guid Music = Guid.Parse("4BD8D571-6D19-48D3-BE97-422220080E43");
            public static Guid Videos = Guid.Parse("18989B1D-99B5-455B-841C-AB7C74E4DDFC");
            public static Guid Ringtones = Guid.Parse("C870044B-F49E-4126-A9C3-B52A1FF411E8");
            public static Guid PublicPictures = Guid.Parse("B6EBFB86-6907-413C-9AF7-4FC2ABF07CC5");
            public static Guid PublicMusic = Guid.Parse("3214FAB5-9757-4298-BB61-92A9DEAA44FF");
            public static Guid PublicVideos = Guid.Parse("2400183A-6185-49FB-A2D8-4A392A602BA3");
            public static Guid PublicRingtones = Guid.Parse("E555AB60-153B-4D17-9F04-A5FE99FC15EC");
            public static Guid ResourceDir = Guid.Parse("8AD10C31-2ADB-4296-A8F7-E4701232C972");
            public static Guid LocalizedResourcesDir = Guid.Parse("2A00375E-224C-49DE-B8D1-440DF7EF3DDC");
            public static Guid CommonOEMLinks = Guid.Parse("C1BAE2D0-10DF-4334-BEDD-7AA20B227A9D");
            public static Guid CDBurning = Guid.Parse("9E52AB10-F80D-49DF-ACB8-4330F5687855");
            public static Guid UserProfiles = Guid.Parse("0762D272-C50A-4BB0-A382-697DCD729B80");
            public static Guid Playlists = Guid.Parse("DE92C1C7-837F-4F69-A3BB-86E631204A23");
            public static Guid SamplePlaylists = Guid.Parse("15CA69B3-30EE-49C1-ACE1-6B5EC372AFB5");
            public static Guid SampleMusic = Guid.Parse("B250C668-F57D-4EE1-A63C-290EE7D1AA1F");
            public static Guid SamplePictures = Guid.Parse("C4900540-2379-4C75-844B-64E6FAF8716B");
            public static Guid SampleVideos = Guid.Parse("859EAD94-2E85-48AD-A71A-0969CB56A6CD");
            public static Guid PhotoAlbums = Guid.Parse("69D2CF90-FC33-4FB7-9A0C-EBB0F0FCB43C");
            public static Guid Public = Guid.Parse("DFDF76A2-C82A-4D63-906A-5644AC457385");
            public static Guid ChangeRemovePrograms = Guid.Parse("DF7266AC-9274-4867-8D55-3BD661DE872D");
            public static Guid AppUpdates = Guid.Parse("A305CE99-F527-492B-8B1A-7E76FA98D6E4");
            public static Guid AddNewPrograms = Guid.Parse("DE61D971-5EBC-4F02-A3A9-6C82895E5C04");
            public static Guid Downloads = Guid.Parse("374DE290-123F-4565-9164-39C4925E467B");
            public static Guid PublicDownloads = Guid.Parse("3D644C9B-1FB8-4F30-9B45-F670235F79C0");
            public static Guid SavedSearches = Guid.Parse("7D1D3A04-DEBB-4115-95CF-2F29DA2920DA");
            public static Guid QuickLaunch = Guid.Parse("52A4F021-7B75-48A9-9F6B-4B87A210BC8F");
            public static Guid Contacts = Guid.Parse("56784854-C6CB-462B-8169-88E350ACB882");
            public static Guid SidebarParts = Guid.Parse("A75D362E-50FC-4FB7-AC2C-A8BEAA314493");
            public static Guid SidebarDefaultParts = Guid.Parse("7B396E54-9EC5-4300-BE0A-2482EBAE1A26");
            public static Guid PublicGameTasks = Guid.Parse("DEBF2536-E1A8-4C59-B6A2-414586476AEA");
            public static Guid GameTasks = Guid.Parse("054FAE61-4DD8-4787-80B6-090220C4B700");
            public static Guid SavedGames = Guid.Parse("4C5C32FF-BB9D-43B0-B5B4-2D72E54EAAA4");
            public static Guid Games = Guid.Parse("CAC52C1A-B53D-4EDC-92D7-6B2E8AC19434");
            public static Guid SEARCH_MAPI = Guid.Parse("98EC0E18-2098-4D44-8644-66979315A281");
            public static Guid SEARCH_CSC = Guid.Parse("EE32E446-31CA-4ABA-814F-A5EBD2FD6D5E");
            public static Guid Links = Guid.Parse("BFB9D5E0-C6A9-404C-B2B2-AE6DB6AF4968");
            public static Guid UsersFiles = Guid.Parse("F3CE0F7C-4901-4ACC-8648-D5D44B04EF8F");
            public static Guid UsersLibraries = Guid.Parse("A302545D-DEFF-464B-ABE8-61C8648D939B");
            public static Guid SearchHome = Guid.Parse("190337D1-B8CA-4121-A639-6D472D16972A");
            public static Guid OriginalImages = Guid.Parse("2C36C0AA-5812-4B87-BFD0-4CD0DFB19B39");
            public static Guid DocumentsLibrary = Guid.Parse("7B0DB17D-9CD2-4A93-9733-46CC89022E7C");
            public static Guid MusicLibrary = Guid.Parse("2112AB0A-C86A-4FFE-A368-0DE96E47012E");
            public static Guid PicturesLibrary = Guid.Parse("A990AE9F-A03B-4E80-94BC-9912D7504104");
            public static Guid VideosLibrary = Guid.Parse("491E922F-5643-4AF4-A7EB-4E7A138D8174");
            public static Guid RecordedTVLibrary = Guid.Parse("1A6FDBA2-F42D-4358-A798-B74D745926C5");
            public static Guid HomeGroup = Guid.Parse("52528A6B-B9E3-4ADD-B60D-588C2DBA842D");
            public static Guid HomeGroupCurrentUser = Guid.Parse("9B74B6A3-0DFD-4f11-9E78-5F7800F2E772");
            public static Guid DeviceMetadataStore = Guid.Parse("5CE4A5E9-E4EB-479D-B89F-130C02886155");
            public static Guid Libraries = Guid.Parse("1B3EA5DC-B587-4786-B4EF-BD1DC332AEAE");
            public static Guid PublicLibraries = Guid.Parse("48DAF80B-E6CF-4F4E-B800-0E69D84EE384");
            public static Guid UserPinned = Guid.Parse("9E3995AB-1F9C-4F13-B827-48B24B6C7174");
            public static Guid ImplicitAppShortcuts = Guid.Parse("BCB5256F-79F6-4CEE-B725-DC34E402FD46");
            public static Guid AccountPictures = Guid.Parse("008CA0B1-55B4-4C56-B8A8-4DE4B299D3BE");
            public static Guid PublicUserTiles = Guid.Parse("0482AF6C-08F1-4C34-8C90-E17EC98B1E17");
            public static Guid AppsFolder = Guid.Parse("1E87508D-89C2-42F0-8A7E-645A0F50CA58");
            public static Guid StartMenuAllPrograms = Guid.Parse("F26305EF-6948-40B9-B255-81453D09C785");
            public static Guid CommonStartMenuPlaces = Guid.Parse("A440879F-87A0-4F7D-B700-0207B966194A");
            public static Guid ApplicationShortcuts = Guid.Parse("A3918781-E5F2-4890-B3D9-A7E54332328C");
            public static Guid RoamingTiles = Guid.Parse("00BCFC5A-ED94-4e48-96A1-3F6217F21990");
            public static Guid RoamedTileImages = Guid.Parse("AAA8D5A5-F1D6-4259-BAA8-78E7EF60835E");
            public static Guid Screenshots = Guid.Parse("B7BEDE81-DF94-4682-A7D8-57A52620B86F");
            public static Guid CameraRoll = Guid.Parse("AB5FB87B-7CE2-4F83-915D-550846C9537B");
            public static Guid SkyDrive = Guid.Parse("A52BBA46-E9E1-435F-B3D9-28DAA648C0F6");
            public static Guid OneDrive = Guid.Parse("A52BBA46-E9E1-435F-B3D9-28DAA648C0F6");
            public static Guid SkyDriveDocuments = Guid.Parse("24D89E24-2F19-4534-9DDE-6A6671FBB8FE");
            public static Guid SkyDrivePictures = Guid.Parse("339719B5-8C47-4894-94C2-D8F77ADD44A6");
            public static Guid SkyDriveMusic = Guid.Parse("C3F2459E-80D6-45DC-BFEF-1F769F2BE730");
            public static Guid SkyDriveCameraRoll = Guid.Parse("767E6811-49CB-4273-87C2-20F355E1085B");
            public static Guid SearchHistory = Guid.Parse("0D4C3DB6-03A3-462F-A0E6-08924C41B5D4");
            public static Guid SearchTemplates = Guid.Parse("7E636BFE-DFA9-4D5E-B456-D7B39851D8A9");
            public static Guid CameraRollLibrary = Guid.Parse("2B20DF75-1EDA-4039-8097-38798227D5B7");
            public static Guid SavedPictures = Guid.Parse("3B193882-D3AD-4EAB-965A-69829D1FB59F");
            public static Guid SavedPicturesLibrary = Guid.Parse("E25B5812-BE88-4BD9-94B0-29233477B6C3");
            public static Guid RetailDemo = Guid.Parse("12D4C69E-24AD-4923-BE19-31321C43A767");
            public static Guid Device = Guid.Parse("1C2AC1DC-4358-4B6C-9733-AF21156576F0");
            public static Guid DevelopmentFiles = Guid.Parse("DBE8E08E-3053-4BBC-B183-2A7B2B191E59");
            public static Guid Objects3D = Guid.Parse("31C0DD25-9439-4F12-BF41-7FF4EDA38722");
            public static Guid AppCaptures = Guid.Parse("EDC0FE71-98D8-4F4A-B920-C8DC133CB165");
            public static Guid LocalDocuments = Guid.Parse("F42EE2D3-909F-4907-8871-4C22FC0BF756");
            public static Guid LocalPictures = Guid.Parse("0DDD015D-B06C-45D5-8C4C-F59713854639");
            public static Guid LocalVideos = Guid.Parse("35286A68-3C57-41A1-BBB1-0EAE73D76C95");
            public static Guid LocalMusic = Guid.Parse("A0C69A99-21C8-4671-8703-7934162FCF1D");
            public static Guid LocalDownloads = Guid.Parse("7D83EE9B-2244-4E70-B1F5-5393042AF1E4");
            public static Guid RecordedCalls = Guid.Parse("2F8B40C2-83ED-48EE-B383-A1F157EC6F9A");
            public static Guid AllAppMods = Guid.Parse("7AD67899-66AF-43BA-9156-6AAD42E6C596");
            public static Guid CurrentAppMods = Guid.Parse("3DB40B20-2A30-4DBE-917E-771DD21DD099");
            public static Guid AppDataDesktop = Guid.Parse("B2C5E279-7ADD-439F-B28C-C41FE1BBF672");
            public static Guid AppDataDocuments = Guid.Parse("7BE16610-1F7F-44AC-BFF0-83E15F2FFCA1");
            public static Guid AppDataFavorites = Guid.Parse("7CFBEFBC-DE1F-45AA-B843-A542AC536CC9");
            public static Guid AppDataProgramData = Guid.Parse("559D40A3-A036-40FA-AF61-84CB430A4D34");
        }
    }
}
