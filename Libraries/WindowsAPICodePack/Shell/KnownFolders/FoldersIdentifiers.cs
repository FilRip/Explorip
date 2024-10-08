using System;
using System.Collections.Generic;
using System.Reflection;

using Microsoft.WindowsAPICodePack.Shell.Resources;

namespace Microsoft.WindowsAPICodePack.Shell.KnownFolders;

/// <summary>
/// Contains the GUID identifiers for well-known folders.
/// </summary>
internal static class FolderIdentifiers
{
    private static Dictionary<Guid, string> folders;

    private static void InitFolders()
    {
        folders = [];
        Type folderIDs = typeof(FolderIdentifiers);

#pragma warning disable S3011
        FieldInfo[] fields = folderIDs.GetFields(BindingFlags.NonPublic | BindingFlags.Static);
#pragma warning restore S3011
        foreach (FieldInfo f in fields)
        {
            // Ignore dictionary field.
            if (f.FieldType == typeof(Guid))
            {
                Guid id = (Guid)f.GetValue(null);
                string name = f.Name;
                folders.Add(id, name);
            }
        }
    }
    /// <summary>
    /// Returns the friendly name for a specified folder.
    /// </summary>
    /// <param name="folderId">The Guid identifier for a known folder.</param>
    /// <returns>A <see cref="T:System.String"/> value.</returns>
    internal static string NameForGuid(Guid folderId)
    {
        if (folders == null)
            InitFolders();

        if (!folders.TryGetValue(folderId, out string folder))
        {
            throw new ArgumentException(LocalizedMessages.FolderIdsUnknownGuid, "folderId");
        }
        return folder;
    }
    /// <summary>
    /// Returns a sorted list of name, guid pairs for 
    /// all known folders.
    /// </summary>
    /// <returns></returns>
    internal static SortedList<string, Guid> GetAllFolders()
    {
        if (folders == null)
            InitFolders();

        // Make a copy of the dictionary
        // because the Keys and Values collections
        // are mutable.
        ICollection<Guid> keys = folders.Keys;

        SortedList<string, Guid> slist = [];
        foreach (Guid g in keys)
        {
            slist.Add(folders[g], g);
        }

        return slist;
    }

    #region KnownFolder Guids

    /// <summary>
    /// Computer
    /// </summary>
    internal static readonly Guid Computer = new(0x0AC0837C, 0xBBF8, 0x452A, 0x85, 0x0D, 0x79, 0xD0, 0x8E, 0x66, 0x7C, 0xA7);

    /// <summary>
    /// Conflicts
    /// </summary>
    internal static readonly Guid Conflict = new(0x4bfefb45, 0x347d, 0x4006, 0xa5, 0xbe, 0xac, 0x0c, 0xb0, 0x56, 0x71, 0x92);

    /// <summary>
    /// Control Panel
    /// </summary>
    internal static readonly Guid ControlPanel = new(0x82A74AEB, 0xAEB4, 0x465C, 0xA0, 0x14, 0xD0, 0x97, 0xEE, 0x34, 0x6D, 0x63);

    /// <summary>
    /// Desktop
    /// </summary>
    internal static readonly Guid Desktop = new(0xB4BFCC3A, 0xDB2C, 0x424C, 0xB0, 0x29, 0x7F, 0xE9, 0x9A, 0x87, 0xC6, 0x41);

    /// <summary>
    /// Internet Explorer
    /// </summary>
    internal static readonly Guid Internet = new(0x4D9F7874, 0x4E0C, 0x4904, 0x96, 0x7B, 0x40, 0xB0, 0xD2, 0x0C, 0x3E, 0x4B);

    /// <summary>
    /// Network
    /// </summary>
    internal static readonly Guid Network = new(0xD20BEEC4, 0x5CA8, 0x4905, 0xAE, 0x3B, 0xBF, 0x25, 0x1E, 0xA0, 0x9B, 0x53);

    /// <summary>
    /// Printers
    /// </summary>
    internal static readonly Guid Printers = new(0x76FC4E2D, 0xD6AD, 0x4519, 0xA6, 0x63, 0x37, 0xBD, 0x56, 0x06, 0x81, 0x85);

    /// <summary>
    /// Sync Center
    /// </summary>
    internal static readonly Guid SyncManager = new(0x43668BF8, 0xC14E, 0x49B2, 0x97, 0xC9, 0x74, 0x77, 0x84, 0xD7, 0x84, 0xB7);

    /// <summary>
    /// Network Connections
    /// </summary>
    internal static readonly Guid Connections = new(0x6F0CD92B, 0x2E97, 0x45D1, 0x88, 0xFF, 0xB0, 0xD1, 0x86, 0xB8, 0xDE, 0xDD);

    /// <summary>
    /// Sync Setup
    /// </summary>
    internal static readonly Guid SyncSetup = new(0xf214138, 0xb1d3, 0x4a90, 0xbb, 0xa9, 0x27, 0xcb, 0xc0, 0xc5, 0x38, 0x9a);

    /// <summary>
    /// Sync Results
    /// </summary>
    internal static readonly Guid SyncResults = new(0x289a9a43, 0xbe44, 0x4057, 0xa4, 0x1b, 0x58, 0x7a, 0x76, 0xd7, 0xe7, 0xf9);

    /// <summary>
    /// Recycle Bin
    /// </summary>
    internal static readonly Guid RecycleBin = new(0xB7534046, 0x3ECB, 0x4C18, 0xBE, 0x4E, 0x64, 0xCD, 0x4C, 0xB7, 0xD6, 0xAC);

    /// <summary>
    /// Fonts
    /// </summary>
    internal static readonly Guid Fonts = new(0xFD228CB7, 0xAE11, 0x4AE3, 0x86, 0x4C, 0x16, 0xF3, 0x91, 0x0A, 0xB8, 0xFE);

    /// <summary>
    /// Startup
    /// </summary>
    internal static readonly Guid Startup = new(0xB97D20BB, 0xF46A, 0x4C97, 0xBA, 0x10, 0x5E, 0x36, 0x08, 0x43, 0x08, 0x54);

    /// <summary>
    /// Programs
    /// </summary>
    internal static readonly Guid Programs = new(0xA77F5D77, 0x2E2B, 0x44C3, 0xA6, 0xA2, 0xAB, 0xA6, 0x01, 0x05, 0x4A, 0x51);

    /// <summary>
    /// Start Menu
    /// </summary>
    internal static readonly Guid StartMenu = new(0x625B53C3, 0xAB48, 0x4EC1, 0xBA, 0x1F, 0xA1, 0xEF, 0x41, 0x46, 0xFC, 0x19);

    /// <summary>
    /// Recent Items
    /// </summary>
    internal static readonly Guid Recent = new(0xAE50C081, 0xEBD2, 0x438A, 0x86, 0x55, 0x8A, 0x09, 0x2E, 0x34, 0x98, 0x7A);

    /// <summary>
    /// SendTo
    /// </summary>
    internal static readonly Guid SendTo = new(0x8983036C, 0x27C0, 0x404B, 0x8F, 0x08, 0x10, 0x2D, 0x10, 0xDC, 0xFD, 0x74);

    /// <summary>
    /// Documents
    /// </summary>
    internal static readonly Guid Documents = new(0xFDD39AD0, 0x238F, 0x46AF, 0xAD, 0xB4, 0x6C, 0x85, 0x48, 0x03, 0x69, 0xC7);

    /// <summary>
    /// Favorites
    /// </summary>
    internal static readonly Guid Favorites = new(0x1777F761, 0x68AD, 0x4D8A, 0x87, 0xBD, 0x30, 0xB7, 0x59, 0xFA, 0x33, 0xDD);

    /// <summary>
    /// Network Shortcuts
    /// </summary>
    internal static readonly Guid NetHood = new(0xC5ABBF53, 0xE17F, 0x4121, 0x89, 0x00, 0x86, 0x62, 0x6F, 0xC2, 0xC9, 0x73);

    /// <summary>
    /// Printer Shortcuts
    /// </summary>
    internal static readonly Guid PrintHood = new(0x9274BD8D, 0xCFD1, 0x41C3, 0xB3, 0x5E, 0xB1, 0x3F, 0x55, 0xA7, 0x58, 0xF4);

    /// <summary>
    /// Templates
    /// </summary>
    internal static readonly Guid Templates = new(0xA63293E8, 0x664E, 0x48DB, 0xA0, 0x79, 0xDF, 0x75, 0x9E, 0x05, 0x09, 0xF7);

    /// <summary>
    /// Startup
    /// </summary>
    internal static readonly Guid CommonStartup = new(0x82A5EA35, 0xD9CD, 0x47C5, 0x96, 0x29, 0xE1, 0x5D, 0x2F, 0x71, 0x4E, 0x6E);

    /// <summary>
    /// Programs
    /// </summary>
    internal static readonly Guid CommonPrograms = new(0x0139D44E, 0x6AFE, 0x49F2, 0x86, 0x90, 0x3D, 0xAF, 0xCA, 0xE6, 0xFF, 0xB8);

    /// <summary>
    /// Start Menu
    /// </summary>
    internal static readonly Guid CommonStartMenu = new(0xA4115719, 0xD62E, 0x491D, 0xAA, 0x7C, 0xE7, 0x4B, 0x8B, 0xE3, 0xB0, 0x67);

    /// <summary>
    /// Public Desktop
    /// </summary>
    internal static readonly Guid PublicDesktop = new(0xC4AA340D, 0xF20F, 0x4863, 0xAF, 0xEF, 0xF8, 0x7E, 0xF2, 0xE6, 0xBA, 0x25);

    /// <summary>
    /// ProgramData
    /// </summary>
    internal static readonly Guid ProgramData = new(0x62AB5D82, 0xFDC1, 0x4DC3, 0xA9, 0xDD, 0x07, 0x0D, 0x1D, 0x49, 0x5D, 0x97);

    /// <summary>
    /// Templates
    /// </summary>
    internal static readonly Guid CommonTemplates = new(0xB94237E7, 0x57AC, 0x4347, 0x91, 0x51, 0xB0, 0x8C, 0x6C, 0x32, 0xD1, 0xF7);

    /// <summary>
    /// Public Documents
    /// </summary>
    internal static readonly Guid PublicDocuments = new(0xED4824AF, 0xDCE4, 0x45A8, 0x81, 0xE2, 0xFC, 0x79, 0x65, 0x08, 0x36, 0x34);

    /// <summary>
    /// Roaming
    /// </summary>
    internal static readonly Guid RoamingAppData = new(0x3EB685DB, 0x65F9, 0x4CF6, 0xA0, 0x3A, 0xE3, 0xEF, 0x65, 0x72, 0x9F, 0x3D);

    /// <summary>
    /// Local
    /// </summary>
    internal static readonly Guid LocalAppData = new(0xF1B32785, 0x6FBA, 0x4FCF, 0x9D, 0x55, 0x7B, 0x8E, 0x7F, 0x15, 0x70, 0x91);

    /// <summary>
    /// LocalLow
    /// </summary>
    internal static readonly Guid LocalAppDataLow = new(0xA520A1A4, 0x1780, 0x4FF6, 0xBD, 0x18, 0x16, 0x73, 0x43, 0xC5, 0xAF, 0x16);

    /// <summary>
    /// Temporary Internet Files
    /// </summary>
    internal static readonly Guid InternetCache = new(0x352481E8, 0x33BE, 0x4251, 0xBA, 0x85, 0x60, 0x07, 0xCA, 0xED, 0xCF, 0x9D);

    /// <summary>
    /// Cookies
    /// </summary>
    internal static readonly Guid Cookies = new(0x2B0F765D, 0xC0E9, 0x4171, 0x90, 0x8E, 0x08, 0xA6, 0x11, 0xB8, 0x4F, 0xF6);

    /// <summary>
    /// History
    /// </summary>
    internal static readonly Guid History = new(0xD9DC8A3B, 0xB784, 0x432E, 0xA7, 0x81, 0x5A, 0x11, 0x30, 0xA7, 0x59, 0x63);

    /// <summary>
    /// System32
    /// </summary>
    internal static readonly Guid System = new(0x1AC14E77, 0x02E7, 0x4E5D, 0xB7, 0x44, 0x2E, 0xB1, 0xAE, 0x51, 0x98, 0xB7);

    /// <summary>
    /// System32
    /// </summary>
    internal static readonly Guid SystemX86 = new(0xD65231B0, 0xB2F1, 0x4857, 0xA4, 0xCE, 0xA8, 0xE7, 0xC6, 0xEA, 0x7D, 0x27);

    /// <summary>
    /// Windows
    /// </summary>
    internal static readonly Guid Windows = new(0xF38BF404, 0x1D43, 0x42F2, 0x93, 0x05, 0x67, 0xDE, 0x0B, 0x28, 0xFC, 0x23);

    /// <summary>
    /// The user's username (%USERNAME%)
    /// </summary>
    internal static readonly Guid Profile = new(0x5E6C858F, 0x0E22, 0x4760, 0x9A, 0xFE, 0xEA, 0x33, 0x17, 0xB6, 0x71, 0x73);

    /// <summary>
    /// Pictures
    /// </summary>
    internal static readonly Guid Pictures = new(0x33E28130, 0x4E1E, 0x4676, 0x83, 0x5A, 0x98, 0x39, 0x5C, 0x3B, 0xC3, 0xBB);

    /// <summary>
    /// Program Files
    /// </summary>
    internal static readonly Guid ProgramFilesX86 = new(0x7C5A40EF, 0xA0FB, 0x4BFC, 0x87, 0x4A, 0xC0, 0xF2, 0xE0, 0xB9, 0xFA, 0x8E);

    /// <summary>
    /// Common Files
    /// </summary>
    internal static readonly Guid ProgramFilesCommonX86 = new(0xDE974D24, 0xD9C6, 0x4D3E, 0xBF, 0x91, 0xF4, 0x45, 0x51, 0x20, 0xB9, 0x17);

    /// <summary>
    /// Program Files
    /// </summary>
    internal static readonly Guid ProgramFilesX64 = new(0x6d809377, 0x6af0, 0x444b, 0x89, 0x57, 0xa3, 0x77, 0x3f, 0x02, 0x20, 0x0e);

    /// <summary>
    /// Common Files
    /// </summary>
    internal static readonly Guid ProgramFilesCommonX64 = new(0x6365d5a7, 0xf0d, 0x45e5, 0x87, 0xf6, 0xd, 0xa5, 0x6b, 0x6a, 0x4f, 0x7d);

    /// <summary>
    /// Program Files
    /// </summary>
    internal static readonly Guid ProgramFiles = new(0x905e63b6, 0xc1bf, 0x494e, 0xb2, 0x9c, 0x65, 0xb7, 0x32, 0xd3, 0xd2, 0x1a);

    /// <summary>
    /// Common Files
    /// </summary>
    internal static readonly Guid ProgramFilesCommon = new(0xF7F1ED05, 0x9F6D, 0x47A2, 0xAA, 0xAE, 0x29, 0xD3, 0x17, 0xC6, 0xF0, 0x66);

    /// <summary>
    /// Administrative Tools
    /// </summary>
    internal static readonly Guid AdminTools = new(0x724EF170, 0xA42D, 0x4FEF, 0x9F, 0x26, 0xB6, 0x0E, 0x84, 0x6F, 0xBA, 0x4F);

    /// <summary>
    /// Administrative Tools
    /// </summary>
    internal static readonly Guid CommonAdminTools = new(0xD0384E7D, 0xBAC3, 0x4797, 0x8F, 0x14, 0xCB, 0xA2, 0x29, 0xB3, 0x92, 0xB5);

    /// <summary>
    /// Music
    /// </summary>
    internal static readonly Guid Music = new(0x4BD8D571, 0x6D19, 0x48D3, 0xBE, 0x97, 0x42, 0x22, 0x20, 0x08, 0x0E, 0x43);

    /// <summary>
    /// Videos
    /// </summary>
    internal static readonly Guid Videos = new(0x18989B1D, 0x99B5, 0x455B, 0x84, 0x1C, 0xAB, 0x7C, 0x74, 0xE4, 0xDD, 0xFC);

    /// <summary>
    /// Public Pictures
    /// </summary>
    internal static readonly Guid PublicPictures = new(0xB6EBFB86, 0x6907, 0x413C, 0x9A, 0xF7, 0x4F, 0xC2, 0xAB, 0xF0, 0x7C, 0xC5);

    /// <summary>
    /// Public Music
    /// </summary>
    internal static readonly Guid PublicMusic = new(0x3214FAB5, 0x9757, 0x4298, 0xBB, 0x61, 0x92, 0xA9, 0xDE, 0xAA, 0x44, 0xFF);

    /// <summary>
    /// Public Videos
    /// </summary>
    internal static readonly Guid PublicVideos = new(0x2400183A, 0x6185, 0x49FB, 0xA2, 0xD8, 0x4A, 0x39, 0x2A, 0x60, 0x2B, 0xA3);

    /// <summary>
    /// Resources
    /// </summary>
    internal static readonly Guid ResourceDir = new(0x8AD10C31, 0x2ADB, 0x4296, 0xA8, 0xF7, 0xE4, 0x70, 0x12, 0x32, 0xC9, 0x72);

    /// <summary>
    /// None
    /// </summary>
    internal static readonly Guid LocalizedResourcesDir = new(0x2A00375E, 0x224C, 0x49DE, 0xB8, 0xD1, 0x44, 0x0D, 0xF7, 0xEF, 0x3D, 0xDC);

    /// <summary>
    /// OEM Links
    /// </summary>
    internal static readonly Guid CommonOEMLinks = new(0xC1BAE2D0, 0x10DF, 0x4334, 0xBE, 0xDD, 0x7A, 0xA2, 0x0B, 0x22, 0x7A, 0x9D);

    /// <summary>
    /// Temporary Burn Folder
    /// </summary>
    internal static readonly Guid CDBurning = new(0x9E52AB10, 0xF80D, 0x49DF, 0xAC, 0xB8, 0x43, 0x30, 0xF5, 0x68, 0x78, 0x55);

    /// <summary>
    /// Users
    /// </summary>
    internal static readonly Guid UserProfiles = new(0x0762D272, 0xC50A, 0x4BB0, 0xA3, 0x82, 0x69, 0x7D, 0xCD, 0x72, 0x9B, 0x80);

    /// <summary>
    /// Playlists
    /// </summary>
    internal static readonly Guid Playlists = new(0xDE92C1C7, 0x837F, 0x4F69, 0xA3, 0xBB, 0x86, 0xE6, 0x31, 0x20, 0x4A, 0x23);

    /// <summary>
    /// Sample Playlists
    /// </summary>
    internal static readonly Guid SamplePlaylists = new(0x15CA69B3, 0x30EE, 0x49C1, 0xAC, 0xE1, 0x6B, 0x5E, 0xC3, 0x72, 0xAF, 0xB5);

    /// <summary>
    /// Sample Music
    /// </summary>
    internal static readonly Guid SampleMusic = new(0xB250C668, 0xF57D, 0x4EE1, 0xA6, 0x3C, 0x29, 0x0E, 0xE7, 0xD1, 0xAA, 0x1F);

    /// <summary>
    /// Sample Pictures
    /// </summary>
    internal static readonly Guid SamplePictures = new(0xC4900540, 0x2379, 0x4C75, 0x84, 0x4B, 0x64, 0xE6, 0xFA, 0xF8, 0x71, 0x6B);

    /// <summary>
    /// Sample Videos
    /// </summary>
    internal static readonly Guid SampleVideos = new(0x859EAD94, 0x2E85, 0x48AD, 0xA7, 0x1A, 0x09, 0x69, 0xCB, 0x56, 0xA6, 0xCD);

    /// <summary>
    /// Slide Shows
    /// </summary>
    internal static readonly Guid PhotoAlbums = new(0x69D2CF90, 0xFC33, 0x4FB7, 0x9A, 0x0C, 0xEB, 0xB0, 0xF0, 0xFC, 0xB4, 0x3C);

    /// <summary>
    /// Public
    /// </summary>
    internal static readonly Guid Public = new(0xDFDF76A2, 0xC82A, 0x4D63, 0x90, 0x6A, 0x56, 0x44, 0xAC, 0x45, 0x73, 0x85);

    /// <summary>
    /// Programs and Features
    /// </summary>
    internal static readonly Guid ChangeRemovePrograms = new(0xdf7266ac, 0x9274, 0x4867, 0x8d, 0x55, 0x3b, 0xd6, 0x61, 0xde, 0x87, 0x2d);

    /// <summary>
    /// Installed Updates
    /// </summary>
    internal static readonly Guid AppUpdates = new(0xa305ce99, 0xf527, 0x492b, 0x8b, 0x1a, 0x7e, 0x76, 0xfa, 0x98, 0xd6, 0xe4);

    /// <summary>
    /// Get Programs
    /// </summary>
    internal static readonly Guid AddNewPrograms = new(0xde61d971, 0x5ebc, 0x4f02, 0xa3, 0xa9, 0x6c, 0x82, 0x89, 0x5e, 0x5c, 0x04);

    /// <summary>
    /// Downloads
    /// </summary>
    internal static readonly Guid Downloads = new(0x374de290, 0x123f, 0x4565, 0x91, 0x64, 0x39, 0xc4, 0x92, 0x5e, 0x46, 0x7b);

    /// <summary>
    /// Public Downloads
    /// </summary>
    internal static readonly Guid PublicDownloads = new(0x3d644c9b, 0x1fb8, 0x4f30, 0x9b, 0x45, 0xf6, 0x70, 0x23, 0x5f, 0x79, 0xc0);

    /// <summary>
    /// Searches
    /// </summary>
    internal static readonly Guid SavedSearches = new(0x7d1d3a04, 0xdebb, 0x4115, 0x95, 0xcf, 0x2f, 0x29, 0xda, 0x29, 0x20, 0xda);

    /// <summary>
    /// Quick Launch
    /// </summary>
    internal static readonly Guid QuickLaunch = new(0x52a4f021, 0x7b75, 0x48a9, 0x9f, 0x6b, 0x4b, 0x87, 0xa2, 0x10, 0xbc, 0x8f);

    /// <summary>
    /// Contacts
    /// </summary>
    internal static readonly Guid Contacts = new(0x56784854, 0xc6cb, 0x462b, 0x81, 0x69, 0x88, 0xe3, 0x50, 0xac, 0xb8, 0x82);

    /// <summary>
    /// Gadgets
    /// </summary>
    internal static readonly Guid SidebarParts = new(0xa75d362e, 0x50fc, 0x4fb7, 0xac, 0x2c, 0xa8, 0xbe, 0xaa, 0x31, 0x44, 0x93);

    /// <summary>
    /// Gadgets
    /// </summary>
    internal static readonly Guid SidebarDefaultParts = new(0x7b396e54, 0x9ec5, 0x4300, 0xbe, 0xa, 0x24, 0x82, 0xeb, 0xae, 0x1a, 0x26);

    /// <summary>
    /// Tree property value folder
    /// </summary>
    internal static readonly Guid TreeProperties = new(0x5b3749ad, 0xb49f, 0x49c1, 0x83, 0xeb, 0x15, 0x37, 0x0f, 0xbd, 0x48, 0x82);

    /// <summary>
    /// GameExplorer
    /// </summary>
    internal static readonly Guid PublicGameTasks = new(0xdebf2536, 0xe1a8, 0x4c59, 0xb6, 0xa2, 0x41, 0x45, 0x86, 0x47, 0x6a, 0xea);

    /// <summary>
    /// GameExplorer
    /// </summary>
    internal static readonly Guid GameTasks = new(0x54fae61, 0x4dd8, 0x4787, 0x80, 0xb6, 0x9, 0x2, 0x20, 0xc4, 0xb7, 0x0);

    /// <summary>
    /// Saved Games
    /// </summary>
    internal static readonly Guid SavedGames = new(0x4c5c32ff, 0xbb9d, 0x43b0, 0xb5, 0xb4, 0x2d, 0x72, 0xe5, 0x4e, 0xaa, 0xa4);

    /// <summary>
    /// Games
    /// </summary>
    internal static readonly Guid Games = new(0xcac52c1a, 0xb53d, 0x4edc, 0x92, 0xd7, 0x6b, 0x2e, 0x8a, 0xc1, 0x94, 0x34);

    /// <summary>
    /// Recorded TV
    /// </summary>
    internal static readonly Guid RecordedTV = new(0xbd85e001, 0x112e, 0x431e, 0x98, 0x3b, 0x7b, 0x15, 0xac, 0x09, 0xff, 0xf1);

    /// <summary>
    /// Microsoft Office Outlook
    /// </summary>
    internal static readonly Guid SearchMapi = new(0x98ec0e18, 0x2098, 0x4d44, 0x86, 0x44, 0x66, 0x97, 0x93, 0x15, 0xa2, 0x81);

    /// <summary>
    /// Offline Files
    /// </summary>
    internal static readonly Guid SearchCsc = new(0xee32e446, 0x31ca, 0x4aba, 0x81, 0x4f, 0xa5, 0xeb, 0xd2, 0xfd, 0x6d, 0x5e);

    /// <summary>
    /// Links
    /// </summary>
    internal static readonly Guid Links = new(0xbfb9d5e0, 0xc6a9, 0x404c, 0xb2, 0xb2, 0xae, 0x6d, 0xb6, 0xaf, 0x49, 0x68);

    /// <summary>
    /// The user's full name (for instance, Jean Philippe Bagel) entered when the user account was created.
    /// </summary>
    internal static readonly Guid UsersFiles = new(0xf3ce0f7c, 0x4901, 0x4acc, 0x86, 0x48, 0xd5, 0xd4, 0x4b, 0x04, 0xef, 0x8f);

    /// <summary>
    /// Search home
    /// </summary>
    internal static readonly Guid SearchHome = new(0x190337d1, 0xb8ca, 0x4121, 0xa6, 0x39, 0x6d, 0x47, 0x2d, 0x16, 0x97, 0x2a);

    /// <summary>
    /// Original Images
    /// </summary>
    internal static readonly Guid OriginalImages = new(0x2C36C0AA, 0x5812, 0x4b87, 0xbf, 0xd0, 0x4c, 0xd0, 0xdf, 0xb1, 0x9b, 0x39);

    internal static readonly Guid Objects3D = new("{31C0DD25-9439-4F12-BF41-7FF4EDA38722}");

    #endregion

    #region Win7 KnownFolders Guids

    /// <summary>
    /// UserProgramFiles
    /// </summary>
    internal static readonly Guid UserProgramFiles = new(0x5cd7aee2, 0x2219, 0x4a67, 0xb8, 0x5d, 0x6c, 0x9c, 0xe1, 0x56, 0x60, 0xcb);

    /// <summary>
    /// UserProgramFilesCommon
    /// </summary>
    internal static readonly Guid UserProgramFilesCommon = new(0xbcbd3057, 0xca5c, 0x4622, 0xb4, 0x2d, 0xbc, 0x56, 0xdb, 0x0a, 0xe5, 0x16);

    /// <summary>
    /// Ringtones
    /// </summary>
    internal static readonly Guid Ringtones = new(0xC870044B, 0xF49E, 0x4126, 0xA9, 0xC3, 0xB5, 0x2A, 0x1F, 0xF4, 0x11, 0xE8);

    /// <summary>
    /// PublicRingtones
    /// </summary>
    internal static readonly Guid PublicRingtones = new(0xE555AB60, 0x153B, 0x4D17, 0x9F, 0x04, 0xA5, 0xFE, 0x99, 0xFC, 0x15, 0xEC);

    /// <summary>
    /// UsersLibraries
    /// </summary>
    internal static readonly Guid UsersLibraries = new(0xa302545d, 0xdeff, 0x464b, 0xab, 0xe8, 0x61, 0xc8, 0x64, 0x8d, 0x93, 0x9b);

    /// <summary>
    /// DocumentsLibrary
    /// </summary>
    internal static readonly Guid DocumentsLibrary = new(0x7b0db17d, 0x9cd2, 0x4a93, 0x97, 0x33, 0x46, 0xcc, 0x89, 0x02, 0x2e, 0x7c);

    /// <summary>
    /// MusicLibrary
    /// </summary>
    internal static readonly Guid MusicLibrary = new(0x2112ab0a, 0xc86a, 0x4ffe, 0xa3, 0x68, 0xd, 0xe9, 0x6e, 0x47, 0x1, 0x2e);

    /// <summary>
    /// PicturesLibrary
    /// </summary>
    internal static readonly Guid PicturesLibrary = new(0xa990ae9f, 0xa03b, 0x4e80, 0x94, 0xbc, 0x99, 0x12, 0xd7, 0x50, 0x41, 0x4);

    /// <summary>
    /// VideosLibrary
    /// </summary>
    internal static readonly Guid VideosLibrary = new(0x491e922f, 0x5643, 0x4af4, 0xa7, 0xeb, 0x4e, 0x7a, 0x13, 0x8d, 0x81, 0x74);

    /// <summary>
    /// RecordedTVLibrary
    /// </summary>
    internal static readonly Guid RecordedTVLibrary = new(0x1a6fdba2, 0xf42d, 0x4358, 0xa7, 0x98, 0xb7, 0x4d, 0x74, 0x59, 0x26, 0xc5);

    /// <summary>
    /// OtherUsers
    /// </summary>
    internal static readonly Guid OtherUsers = new(0x52528a6b, 0xb9e3, 0x4add, 0xb6, 0xd, 0x58, 0x8c, 0x2d, 0xba, 0x84, 0x2d);

    /// <summary>
    /// DeviceMetadataStore
    /// </summary>
    internal static readonly Guid DeviceMetadataStore = new(0x5ce4a5e9, 0xe4eb, 0x479d, 0xb8, 0x9f, 0x13, 0x0c, 0x02, 0x88, 0x61, 0x55);

    /// <summary>
    /// Libraries
    /// </summary>
    internal static readonly Guid Libraries = new(0x1b3ea5dc, 0xb587, 0x4786, 0xb4, 0xef, 0xbd, 0x1d, 0xc3, 0x32, 0xae, 0xae);

    /// <summary>
    /// UserPinned
    /// </summary>
    internal static readonly Guid UserPinned = new(0x9e3995ab, 0x1f9c, 0x4f13, 0xb8, 0x27, 0x48, 0xb2, 0x4b, 0x6c, 0x71, 0x74);

    /// <summary>
    /// ImplicitAppShortcuts
    /// </summary>
    internal static readonly Guid ImplicitAppShortcuts = new(0xbcb5256f, 0x79f6, 0x4cee, 0xb7, 0x25, 0xdc, 0x34, 0xe4, 0x2, 0xfd, 0x46);

    #endregion
}
