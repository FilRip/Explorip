using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

using Securify.ShellLink.Internal;

namespace Securify.ShellLink.Const
{
    /// <summary>
    /// The KNOWNFOLDERID constants represent GUIDs that identify standard folders registered with the system as Known Folders. 
    /// These folders are installed with Windows Vista and later operating systems, and a computer will have only folders 
    /// appropriate to it installed
    /// </summary>
    public static class KnownFolderId
    {
        /// <summary>
        /// Account Pictures
        /// </summary>
        public static readonly Guid FOLDERID_AccountPictures = new("{008ca0b1-55b4-4c56-b8a8-4de4b299d3be}");
        /// <summary>
        /// Get Programs
        /// </summary>
        public static readonly Guid FOLDERID_AddNewPrograms = new("{de61d971-5ebc-4f02-a3a9-6c82895e5c04}");
        /// <summary>
        /// Administrative Tools
        /// </summary>
        public static readonly Guid FOLDERID_AdminTools = new("{724EF170-A42D-4FEF-9F26-B60E846FBA4F}");
        /// <summary>
        /// Application Shortcuts
        /// </summary>
        public static readonly Guid FOLDERID_ApplicationShortcuts = new("{A3918781-E5F2-4890-B3D9-A7E54332328C}");
        /// <summary>
        /// Applications
        /// </summary>
        public static readonly Guid FOLDERID_AppsFolder = new("{1e87508d-89c2-42f0-8a7e-645a0f50ca58}");
        /// <summary>
        /// Installed Updates
        /// </summary>
        public static readonly Guid FOLDERID_AppUpdates = new("{a305ce99-f527-492b-8b1a-7e76fa98d6e4}");
        /// <summary>
        /// Camera Roll
        /// </summary>
        public static readonly Guid FOLDERID_CameraRoll = new("{AB5FB87B-7CE2-4F83-915D-550846C9537B}");
        /// <summary>
        /// Temporary Burn Folder
        /// </summary>
        public static readonly Guid FOLDERID_CDBurning = new("{9E52AB10-F80D-49DF-ACB8-4330F5687855}");
        /// <summary>
        /// Programs and Features
        /// </summary>
        public static readonly Guid FOLDERID_ChangeRemovePrograms = new("{df7266ac-9274-4867-8d55-3bd661de872d}");
        /// <summary>
        /// Administrative Tools
        /// </summary>
        public static readonly Guid FOLDERID_CommonAdminTools = new("{D0384E7D-BAC3-4797-8F14-CBA229B392B5}");
        /// <summary>
        /// OEM Links
        /// </summary>
        public static readonly Guid FOLDERID_CommonOEMLinks = new("{C1BAE2D0-10DF-4334-BEDD-7AA20B227A9D}");
        /// <summary>
        /// Programs
        /// </summary>
        public static readonly Guid FOLDERID_CommonPrograms = new("{0139D44E-6AFE-49F2-8690-3DAFCAE6FFB8}");
        /// <summary>
        /// Start Menu
        /// </summary>
        public static readonly Guid FOLDERID_CommonStartMenu = new("{A4115719-D62E-491D-AA7C-E74B8BE3B067}");
        /// <summary>
        /// Startup
        /// </summary>
        public static readonly Guid FOLDERID_CommonStartup = new("{82A5EA35-D9CD-47C5-9629-E15D2F714E6E}");
        /// <summary>
        /// Templates
        /// </summary>
        public static readonly Guid FOLDERID_CommonTemplates = new("{B94237E7-57AC-4347-9151-B08C6C32D1F7}");
        /// <summary>
        /// Computer
        /// </summary>
        public static readonly Guid FOLDERID_ComputerFolder = new("{0AC0837C-BBF8-452A-850D-79D08E667CA7}");
        /// <summary>
        /// Conflicts
        /// </summary>
        public static readonly Guid FOLDERID_ConflictFolder = new("{4bfefb45-347d-4006-a5be-ac0cb0567192}");
        /// <summary>
        /// Network Connections
        /// </summary>
        public static readonly Guid FOLDERID_ConnectionsFolder = new("{6F0CD92B-2E97-45D1-88FF-B0D186B8DEDD}");
        /// <summary>
        /// Contacts
        /// </summary>
        public static readonly Guid FOLDERID_Contacts = new("{56784854-C6CB-462b-8169-88E350ACB882}");
        /// <summary>
        /// Control Panel
        /// </summary>
        public static readonly Guid FOLDERID_ControlPanelFolder = new("{82A74AEB-AEB4-465C-A014-D097EE346D63}");
        /// <summary>
        /// Cookies
        /// </summary>
        public static readonly Guid FOLDERID_Cookies = new("{2B0F765D-C0E9-4171-908E-08A611B84FF6}");
        /// <summary>
        /// Desktop
        /// </summary>
        public static readonly Guid FOLDERID_Desktop = new("{B4BFCC3A-DB2C-424C-B029-7FE99A87C641}");
        /// <summary>
        /// DeviceMetadataStore
        /// </summary>
        public static readonly Guid FOLDERID_DeviceMetadataStore = new("{5CE4A5E9-E4EB-479D-B89F-130C02886155}");
        /// <summary>
        /// Documents
        /// </summary>
        public static readonly Guid FOLDERID_Documents = new("{FDD39AD0-238F-46AF-ADB4-6C85480369C7}");
        /// <summary>
        /// Documents
        /// </summary>
        public static readonly Guid FOLDERID_DocumentsLibrary = new("{7B0DB17D-9CD2-4A93-9733-46CC89022E7C}");
        /// <summary>
        /// Downloads
        /// </summary>
        public static readonly Guid FOLDERID_Downloads = new("{374DE290-123F-4565-9164-39C4925E467B}");
        /// <summary>
        /// Favorites
        /// </summary>
        public static readonly Guid FOLDERID_Favorites = new("{1777F761-68AD-4D8A-87BD-30B759FA33DD}");
        /// <summary>
        /// Fonts
        /// </summary>
        public static readonly Guid FOLDERID_Fonts = new("{FD228CB7-AE11-4AE3-864C-16F3910AB8FE}");
        /// <summary>
        /// Games
        /// </summary>
        public static readonly Guid FOLDERID_Games = new("{CAC52C1A-B53D-4edc-92D7-6B2E8AC19434}");
        /// <summary>
        /// GameExplorer
        /// </summary>
        public static readonly Guid FOLDERID_GameTasks = new("{054FAE61-4DD8-4787-80B6-090220C4B700}");
        /// <summary>
        /// History
        /// </summary>
        public static readonly Guid FOLDERID_History = new("{D9DC8A3B-B784-432E-A781-5A1130A75963}");
        /// <summary>
        /// Homegroup
        /// </summary>
        public static readonly Guid FOLDERID_HomeGroup = new("{52528A6B-B9E3-4ADD-B60D-588C2DBA842D}");
        /// <summary>
        /// The user's username (%USERNAME%)
        /// </summary>
        public static readonly Guid FOLDERID_HomeGroupCurrentUser = new("{9B74B6A3-0DFD-4f11-9E78-5F7800F2E772}");
        /// <summary>
        /// ImplicitAppShortcuts
        /// </summary>
        public static readonly Guid FOLDERID_ImplicitAppShortcuts = new("{BCB5256F-79F6-4CEE-B725-DC34E402FD46}");
        /// <summary>
        /// Temporary Internet Files
        /// </summary>
        public static readonly Guid FOLDERID_InternetCache = new("{352481E8-33BE-4251-BA85-6007CAEDCF9D}");
        /// <summary>
        /// The Internet
        /// </summary>
        public static readonly Guid FOLDERID_InternetFolder = new("{4D9F7874-4E0C-4904-967B-40B0D20C3E4B}");
        /// <summary>
        /// Libraries
        /// </summary>
        public static readonly Guid FOLDERID_Libraries = new("{1B3EA5DC-B587-4786-B4EF-BD1DC332AEAE}");
        /// <summary>
        /// Links
        /// </summary>
        public static readonly Guid FOLDERID_Links = new("{bfb9d5e0-c6a9-404c-b2b2-ae6db6af4968}");
        /// <summary>
        /// Local
        /// </summary>
        public static readonly Guid FOLDERID_LocalAppData = new("{F1B32785-6FBA-4FCF-9D55-7B8E7F157091}");
        /// <summary>
        /// LocalLow
        /// </summary>
        public static readonly Guid FOLDERID_LocalAppDataLow = new("{A520A1A4-1780-4FF6-BD18-167343C5AF16}");
        /// <summary>
        /// Localized Resources
        /// </summary>
        public static readonly Guid FOLDERID_LocalizedResourcesDir = new("{2A00375E-224C-49DE-B8D1-440DF7EF3DDC}");
        /// <summary>
        /// Music
        /// </summary>
        public static readonly Guid FOLDERID_Music = new("{4BD8D571-6D19-48D3-BE97-422220080E43}");
        /// <summary>
        /// Music
        /// </summary>
        public static readonly Guid FOLDERID_MusicLibrary = new("{2112AB0A-C86A-4FFE-A368-0DE96E47012E}");
        /// <summary>
        /// Network Shortcuts
        /// </summary>
        public static readonly Guid FOLDERID_NetHood = new("{C5ABBF53-E17F-4121-8900-86626FC2C973}");
        /// <summary>
        /// Network
        /// </summary>
        public static readonly Guid FOLDERID_NetworkFolder = new("{D20BEEC4-5CA8-4905-AE3B-BF251EA09B53}");
        /// <summary>
        /// Original Images
        /// </summary>
        public static readonly Guid FOLDERID_OriginalImages = new("{2C36C0AA-5812-4b87-BFD0-4CD0DFB19B39}");
        /// <summary>
        /// Slide Shows
        /// </summary>
        public static readonly Guid FOLDERID_PhotoAlbums = new("{69D2CF90-FC33-4FB7-9A0C-EBB0F0FCB43C}");
        /// <summary>
        /// Pictures
        /// </summary>
        public static readonly Guid FOLDERID_PicturesLibrary = new("{A990AE9F-A03B-4E80-94BC-9912D7504104}");
        /// <summary>
        /// Pictures
        /// </summary>
        public static readonly Guid FOLDERID_Pictures = new("{33E28130-4E1E-4676-835A-98395C3BC3BB}");
        /// <summary>
        /// Playlists
        /// </summary>
        public static readonly Guid FOLDERID_Playlists = new("{DE92C1C7-837F-4F69-A3BB-86E631204A23}");
        /// <summary>
        /// Printers
        /// </summary>
        public static readonly Guid FOLDERID_PrintersFolder = new("{76FC4E2D-D6AD-4519-A663-37BD56068185}");
        /// <summary>
        /// Printer Shortcuts
        /// </summary>
        public static readonly Guid FOLDERID_PrintHood = new("{9274BD8D-CFD1-41C3-B35E-B13F55A758F4}");
        /// <summary>
        /// The user's username (%USERNAME%)
        /// </summary>
        public static readonly Guid FOLDERID_Profile = new("{5E6C858F-0E22-4760-9AFE-EA3317B67173}");
        /// <summary>
        /// ProgramData
        /// </summary>
        public static readonly Guid FOLDERID_ProgramData = new("{62AB5D82-FDC1-4DC3-A9DD-070D1D495D97}");
        /// <summary>
        /// Program Files
        /// </summary>
        public static readonly Guid FOLDERID_ProgramFiles = new("{905e63b6-c1bf-494e-b29c-65b732d3d21a}");
        /// <summary>
        /// Program Files
        /// </summary>
        public static readonly Guid FOLDERID_ProgramFilesX64 = new("{6D809377-6AF0-444b-8957-A3773F02200E}");
        /// <summary>
        /// Program Files
        /// </summary>
        public static readonly Guid FOLDERID_ProgramFilesX86 = new("{7C5A40EF-A0FB-4BFC-874A-C0F2E0B9FA8E}");
        /// <summary>
        /// Common Files
        /// </summary>
        public static readonly Guid FOLDERID_ProgramFilesCommon = new("{F7F1ED05-9F6D-47A2-AAAE-29D317C6F066}");
        /// <summary>
        /// Common Files
        /// </summary>
        public static readonly Guid FOLDERID_ProgramFilesCommonX64 = new("{6365D5A7-0F0D-45E5-87F6-0DA56B6A4F7D}");
        /// <summary>
        /// Common Files
        /// </summary>
        public static readonly Guid FOLDERID_ProgramFilesCommonX86 = new("{DE974D24-D9C6-4D3E-BF91-F4455120B917}");
        /// <summary>
        /// Programs
        /// </summary>
        public static readonly Guid FOLDERID_Programs = new("{A77F5D77-2E2B-44C3-A6A2-ABA601054A51}");
        /// <summary>
        /// Public
        /// </summary>
        public static readonly Guid FOLDERID_Public = new("{DFDF76A2-C82A-4D63-906A-5644AC457385}");
        /// <summary>
        /// Public Desktop
        /// </summary>
        public static readonly Guid FOLDERID_PublicDesktop = new("{C4AA340D-F20F-4863-AFEF-F87EF2E6BA25}");
        /// <summary>
        /// Public Documents
        /// </summary>
        public static readonly Guid FOLDERID_PublicDocuments = new("{ED4824AF-DCE4-45A8-81E2-FC7965083634}");
        /// <summary>
        /// Public Downloads
        /// </summary>
        public static readonly Guid FOLDERID_PublicDownloads = new("{3D644C9B-1FB8-4f30-9B45-F670235F79C0}");
        /// <summary>
        /// GameExplorer
        /// </summary>
        public static readonly Guid FOLDERID_PublicGameTasks = new("{DEBF2536-E1A8-4c59-B6A2-414586476AEA}");
        /// <summary>
        /// Libraries
        /// </summary>
        public static readonly Guid FOLDERID_PublicLibraries = new("{48DAF80B-E6CF-4F4E-B800-0E69D84EE384}");
        /// <summary>
        /// Public Music
        /// </summary>
        public static readonly Guid FOLDERID_PublicMusic = new("{3214FAB5-9757-4298-BB61-92A9DEAA44FF}");
        /// <summary>
        /// Public Pictures
        /// </summary>
        public static readonly Guid FOLDERID_PublicPictures = new("{B6EBFB86-6907-413C-9AF7-4FC2ABF07CC5}");
        /// <summary>
        /// Ringtones
        /// </summary>
        public static readonly Guid FOLDERID_PublicRingtones = new("{E555AB60-153B-4D17-9F04-A5FE99FC15EC}");
        /// <summary>
        /// Public Account Pictures
        /// </summary>
        public static readonly Guid FOLDERID_PublicUserTiles = new("{0482af6c-08f1-4c34-8c90-e17ec98b1e17}");
        /// <summary>
        /// Public Videos
        /// </summary>
        public static readonly Guid FOLDERID_PublicVideos = new("{2400183A-6185-49FB-A2D8-4A392A602BA3}");
        /// <summary>
        /// Quick Launch
        /// </summary>
        public static readonly Guid FOLDERID_QuickLaunch = new("{52a4f021-7b75-48a9-9f6b-4b87a210bc8f}");
        /// <summary>
        /// Recent Items
        /// </summary>
        public static readonly Guid FOLDERID_Recent = new("{AE50C081-EBD2-438A-8655-8A092E34987A}");
        /// <summary>
        /// Not used. This value is undefined as of Windows 7.
        /// </summary>
        public static readonly Guid FOLDERID_RecordedTV = new("{bd85e001-112e-431e-983b-7b15ac09fff1}");
        /// <summary>
        /// Recorded TV
        /// </summary>
        public static readonly Guid FOLDERID_RecordedTVLibrary = new("{1A6FDBA2-F42D-4358-A798-B74D745926C5}");
        /// <summary>
        /// Recycle Bin
        /// </summary>
        public static readonly Guid FOLDERID_RecycleBinFolder = new("{B7534046-3ECB-4C18-BE4E-64CD4CB7D6AC}");
        /// <summary>
        /// Resources
        /// </summary>
        public static readonly Guid FOLDERID_ResourceDir = new("{8AD10C31-2ADB-4296-A8F7-E4701232C972}");
        /// <summary>
        /// Ringtones
        /// </summary>
        public static readonly Guid FOLDERID_Ringtones = new("{C870044B-F49E-4126-A9C3-B52A1FF411E8}");
        /// <summary>
        /// Roaming
        /// </summary>
        public static readonly Guid FOLDERID_RoamingAppData = new("{3EB685DB-65F9-4CF6-A03A-E3EF65729F3D}");
        /// <summary>
        /// RoamedTileImages
        /// </summary>
        public static readonly Guid FOLDERID_RoamedTileImages = new("{AAA8D5A5-F1D6-4259-BAA8-78E7EF60835E}");
        /// <summary>
        /// RoamingTiles
        /// </summary>
        public static readonly Guid FOLDERID_RoamingTiles = new("{00BCFC5A-ED94-4e48-96A1-3F6217F21990}");
        /// <summary>
        /// Sample Music
        /// </summary>
        public static readonly Guid FOLDERID_SampleMusic = new("{B250C668-F57D-4EE1-A63C-290EE7D1AA1F}");
        /// <summary>
        /// Sample Pictures
        /// </summary>
        public static readonly Guid FOLDERID_SamplePictures = new("{C4900540-2379-4C75-844B-64E6FAF8716B}");
        /// <summary>
        /// Sample Playlists
        /// </summary>
        public static readonly Guid FOLDERID_SamplePlaylists = new("{15CA69B3-30EE-49C1-ACE1-6B5EC372AFB5}");
        /// <summary>
        /// Sample Videos
        /// </summary>
        public static readonly Guid FOLDERID_SampleVideos = new("{859EAD94-2E85-48AD-A71A-0969CB56A6CD}");
        /// <summary>
        /// Saved Games
        /// </summary>
        public static readonly Guid FOLDERID_SavedGames = new("{4C5C32FF-BB9D-43b0-B5B4-2D72E54EAAA4}");
        /// <summary>
        /// Saved Pictures
        /// </summary>
        public static readonly Guid FOLDERID_SavedPictures = new("{3B193882-D3AD-4eab-965A-69829D1FB59F}");
        /// <summary>
        /// Saved Pictures Library
        /// </summary>
        public static readonly Guid FOLDERID_SavedPicturesLibrary = new("{E25B5812-BE88-4bd9-94B0-29233477B6C3}");
        /// <summary>
        /// Searches
        /// </summary>
        public static readonly Guid FOLDERID_SavedSearches = new("{7d1d3a04-debb-4115-95cf-2f29da2920da}");
        /// <summary>
        /// Screenshots
        /// </summary>
        public static readonly Guid FOLDERID_Screenshots = new("{b7bede81-df94-4682-a7d8-57a52620b86f}");
        /// <summary>
        /// Offline Files
        /// </summary>
        public static readonly Guid FOLDERID_SEARCH_CSC = new("{ee32e446-31ca-4aba-814f-a5ebd2fd6d5e}");
        /// <summary>
        /// History
        /// </summary>
        public static readonly Guid FOLDERID_SearchHistory = new("{0D4C3DB6-03A3-462F-A0E6-08924C41B5D4}");
        /// <summary>
        /// Search Results
        /// </summary>
        public static readonly Guid FOLDERID_SearchHome = new("{190337d1-b8ca-4121-a639-6d472d16972a}");
        /// <summary>
        /// Microsoft Office Outlook
        /// </summary>
        public static readonly Guid FOLDERID_SEARCH_MAPI = new("{98ec0e18-2098-4d44-8644-66979315a281}");
        /// <summary>
        /// Templates
        /// </summary>
        public static readonly Guid FOLDERID_SearchTemplates = new("{7E636BFE-DFA9-4D5E-B456-D7B39851D8A9}");
        /// <summary>
        /// SendTo
        /// </summary>
        public static readonly Guid FOLDERID_SendTo = new("{8983036C-27C0-404B-8F08-102D10DCFD74}");
        /// <summary>
        /// Gadgets
        /// </summary>
        public static readonly Guid FOLDERID_SidebarDefaultParts = new("{7B396E54-9EC5-4300-BE0A-2482EBAE1A26}");
        /// <summary>
        /// Gadgets
        /// </summary>
        public static readonly Guid FOLDERID_SidebarParts = new("{A75D362E-50FC-4fb7-AC2C-A8BEAA314493}");
        /// <summary>
        /// OneDrive
        /// </summary>
        public static readonly Guid FOLDERID_SkyDrive = new("{A52BBA46-E9E1-435f-B3D9-28DAA648C0F6}");
        /// <summary>
        /// Camera Roll
        /// </summary>
        public static readonly Guid FOLDERID_SkyDriveCameraRoll = new("{767E6811-49CB-4273-87C2-20F355E1085B}");
        /// <summary>
        /// Documents
        /// </summary>
        public static readonly Guid FOLDERID_SkyDriveDocuments = new("{24D89E24-2F19-4534-9DDE-6A6671FBB8FE}");
        /// <summary>
        /// Pictures
        /// </summary>
        public static readonly Guid FOLDERID_SkyDrivePictures = new("{339719B5-8C47-4894-94C2-D8F77ADD44A6}");
        /// <summary>
        /// Start Menu
        /// </summary>
        public static readonly Guid FOLDERID_StartMenu = new("{625B53C3-AB48-4EC1-BA1F-A1EF4146FC19}");
        /// <summary>
        /// Startup
        /// </summary>
        public static readonly Guid FOLDERID_Startup = new("{B97D20BB-F46A-4C97-BA10-5E3608430854}");
        /// <summary>
        /// Sync Center
        /// </summary>
        public static readonly Guid FOLDERID_SyncManagerFolder = new("{43668BF8-C14E-49B2-97C9-747784D784B7}");
        /// <summary>
        /// Sync Results
        /// </summary>
        public static readonly Guid FOLDERID_SyncResultsFolder = new("{289a9a43-be44-4057-a41b-587a76d7e7f9}");
        /// <summary>
        /// Sync Setup
        /// </summary>
        public static readonly Guid FOLDERID_SyncSetupFolder = new("{0F214138-B1D3-4a90-BBA9-27CBC0C5389A}");
        /// <summary>
        /// System32
        /// </summary>
        public static readonly Guid FOLDERID_System = new("{1AC14E77-02E7-4E5D-B744-2EB1AE5198B7}");
        /// <summary>
        /// System32
        /// </summary>
        public static readonly Guid FOLDERID_SystemX86 = new("{D65231B0-B2F1-4857-A4CE-A8E7C6EA7D27}");
        /// <summary>
        /// Templates
        /// </summary>
        public static readonly Guid FOLDERID_Templates = new("{A63293E8-664E-48DB-A079-DF759E0509F7}");
        /// <summary>
        /// Not used in Windows Vista. Unsupported as of Windows 7.
        /// </summary>
        public static readonly Guid FOLDERID_TreeProperties = new("{5b3749ad-b49f-49c1-83eb-15370fbd4882}");
        /// <summary>
        /// User Pinned
        /// </summary>
        public static readonly Guid FOLDERID_UserPinned = new("{9E3995AB-1F9C-4F13-B827-48B24B6C7174}");
        /// <summary>
        /// Users
        /// </summary>
        public static readonly Guid FOLDERID_UserProfiles = new("{0762D272-C50A-4BB0-A382-697DCD729B80}");
        /// <summary>
        /// Programs
        /// </summary>
        public static readonly Guid FOLDERID_UserProgramFiles = new("{5CD7AEE2-2219-4A67-B85D-6C9CE15660CB}");
        /// <summary>
        /// Programs
        /// </summary>
        public static readonly Guid FOLDERID_UserProgramFilesCommon = new("{BCBD3057-CA5C-4622-B42D-BC56DB0AE516}");
        /// <summary>
        /// The user's full name (for instance, Jean Philippe Bagel) entered when the user account was created.
        /// </summary>
        public static readonly Guid FOLDERID_UsersFiles = new("{f3ce0f7c-4901-4acc-8648-d5d44b04ef8f}");
        /// <summary>
        /// Libraries
        /// </summary>
        public static readonly Guid FOLDERID_UsersLibraries = new("{A302545D-DEFF-464b-ABE8-61C8648D939B}");
        /// <summary>
        /// Videos
        /// </summary>
        public static readonly Guid FOLDERID_Videos = new("{18989B1D-99B5-455B-841C-AB7C74E4DDFC}");
        /// <summary>
        /// Videos
        /// </summary>
        public static readonly Guid FOLDERID_VideosLibrary = new("{491E922F-5643-4AF4-A7EB-4E7A138D8174}");
        /// <summary>
        /// Windows
        /// </summary>
        public static readonly Guid FOLDERID_Windows = new("{F38BF404-1D43-42F2-9305-67DE0B28FC23}");

        #region All
        /// <summary>
        /// Returns all KNOWNFOLDERID GUIDs
        /// </summary>
        public static Dictionary<string, Guid> All
        {
            get
            {
                return typeof(KnownFolderId)
                    .GetFields(BindingFlags.Public | BindingFlags.Static)
                    .Where(f => f.FieldType == typeof(Guid))
                    .ToDictionary(f => f.Name,
                        f => (Guid)f.GetValue(null));
            }
        }
        #endregion // All

        #region GetDisplayName
        /// <summary>
        /// Retrieves the display name of a FOLDERID.
        /// </summary>
        public static string GetDisplayName(Guid FOLDERID)
        {
            if (Win32.SHGetKnownFolderIDList(FOLDERID, 0, IntPtr.Zero, out IntPtr pidl) == 0 && pidl != IntPtr.Zero)
            {
                if (Win32.SHGetNameFromIDList(pidl, SIGDN.SIGDN_NORMALDISPLAY, out IntPtr pszName) == 0)
                {
                    try
                    {
                        return Marshal.PtrToStringAuto(pszName);
                    }
                    catch (Exception)
                    {
                        return "";
                    }
                    finally
                    {
                        Win32.ILFree(pidl);
                        Win32.CoTaskMemFree(pszName);
                    }
                }
                Win32.ILFree(pidl);
            }

            return "";
        }
        #endregion // GetDisplayName
    }
}
