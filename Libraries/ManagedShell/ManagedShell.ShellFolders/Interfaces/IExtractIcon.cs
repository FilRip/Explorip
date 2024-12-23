using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ManagedShell.ShellFolders.Interfaces;

[Flags()]
public enum EGetIconLocation : uint
{
    GIL_NULL = 0,

    /// <summary>
    /// Set this flag to determine whether the icon should be extracted asynchronously. If the icon can be extracted rapidly, this
    /// flag is usually ignored. If extraction will take more time, GetIconLocation should return E_PENDING. See the Remarks for
    /// further discussion.
    /// </summary>
    GIL_ASYNC = 0x0020,

    /// <summary>
    /// Retrieve information about the fallback icon. Fallback icons are usually used while the desired icon is extracted and added
    /// to the cache.
    /// </summary>
    GIL_DEFAULTICON = 0x0040,

    /// <summary>The icon is displayed in a Shell folder.</summary>
    GIL_FORSHELL = 0x0002,

    /// <summary>
    /// The icon indicates a shortcut. However, the icon extractor should not apply the shortcut overlay; that will be done later.
    /// Shortcut icons are state-independent.
    /// </summary>
    GIL_FORSHORTCUT = 0x0080,

    /// <summary>
    /// The icon is in the open state if both open-state and closed-state images are available. If this flag is not specified, the
    /// icon is in the normal or closed state. This flag is typically used for folder objects.
    /// </summary>
    GIL_OPENICON = 0x0001,

    /// <summary>Explicitly return either GIL_SHIELD or GIL_FORCENOSHIELD in pwFlags. Do not block if GIL_ASYNC is set.</summary>
    GIL_CHECKSHIELD = 0x0200
}

[Flags()]
public enum EGetIconLocationResult : uint
{
    /// <summary>The calling application should create a document icon using the specified icon.</summary>
    GIL_SIMULATEDOC = 0x0001,

    /// <summary>
    /// Each object of this class has its own icon. This flag is used internally by the Shell to handle cases like Setup.exe, where
    /// objects with identical names can have different icons. Typical implementations of IExtractIcon do not require this flag.
    /// </summary>
    GIL_PERINSTANCE = 0x0002,

    /// <summary>
    /// All objects of this class have the same icon. This flag is used internally by the Shell. Typical implementations of
    /// IExtractIcon do not require this flag because the flag implies that an icon handler is not required to resolve the icon on a
    /// per-object basis. The recommended method for implementing per-class icons is to register a DefaultIcon for the class.
    /// </summary>
    GIL_PERCLASS = 0x0004,

    /// <summary>
    /// The location is not a file name/index pair. The values in pszIconFile and piIndex cannot be passed to ExtractIcon or
    /// ExtractIconEx. When this flag is omitted, the value returned in pszIconFile is a fully-qualified path name to either a .ico
    /// file or to a file that can contain icons. Also, the value returned in piIndex is an index into that file that identifies
    /// which of its icons to use. Therefore, when the GIL_NOTFILENAME flag is omitted, these values can be passed to ExtractIcon or ExtractIconEx.
    /// </summary>
    GIL_NOTFILENAME = 0x0008,

    /// <summary>The physical image bits for this icon are not cached by the calling application.</summary>
    GIL_DONTCACHE = 0x0010,

    /// <summary>Undocumented, but appears to indicate thumbnails are available.</summary>
    GIL_HASTHUMBNAIL = 0x0020,

    /// <summary>Windows Vista only. The calling application must stamp the icon with the UAC shield.</summary>
    GIL_SHIELD = 0x0200, //Windows Vista only

    /// <summary>Windows Vista only. The calling application must not stamp the icon with the UAC shield.</summary>
    GIL_FORCENOSHIELD = 0x0400, //Windows Vista only

    /// <summary>Undocumented, but appears to indicate the folder is encrypted.</summary>
    GIL_ENCRYPTED = 0x0800,

    /// <summary>Undocumented, but appears to indicate a compressed folder.</summary>
    GIL_COMPRESSED = 0x1000,
}

[ComImport(), Guid("000214eb-0000-0000-c000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IExtractIcon
{
    [PreserveSig]
    int GetIconLocation(EGetIconLocation uFlags, [MarshalAs(UnmanagedType.LPStr, SizeParamIndex = 2)] StringBuilder szIconFile, int cchMax, out int piIndex, out EGetIconLocationResult pwFlags);

    [PreserveSig]
    int Extract([MarshalAs(UnmanagedType.LPStr)] string pszFile, uint nIconIndex, IntPtr phiconLarge, IntPtr phiconSmall, uint nIconSize);
}
