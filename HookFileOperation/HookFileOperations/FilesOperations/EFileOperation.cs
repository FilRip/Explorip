using System;

namespace Explorip.HookFileOperations.FilesOperations;

[Flags(), Serializable()]
public enum EFileOperation : uint
{
    None = 0,
    FOF_MULTIDESTFILES = 0x0001,
    FOF_CONFIRMMOUSE = 0x0002,
    /// <summary>
    /// don't create progress/report
    /// </summary>
    FOF_SILENT = 0x0004,
    FOF_RENAMEONCOLLISION = 0x0008,
    /// <summary>
    /// Don't prompt the user.
    /// </summary>
    FOF_NOCONFIRMATION = 0x0010,
    /// <summary>
    /// Fill in SHFILEOPSTRUCT.hNameMappings<br/>
    /// Must be freed using SHFreeNameMappings
    /// </summary>
    FOF_WANTMAPPINGHANDLE = 0x0020,
    FOF_ALLOWUNDO = 0x0040,
    /// <summary>
    /// on *.*, do only files
    /// </summary>
    FOF_FILESONLY = 0x0080,
    /// <summary>
    /// means don't show names of files
    /// </summary>
    FOF_SIMPLEPROGRESS = 0x0100,
    /// <summary>
    /// don't confirm making any needed dirs
    /// </summary>
    FOF_NOCONFIRMMKDIR = 0x0200,
    /// <summary>
    /// don't put up error UI
    /// </summary>
    FOF_NOERRORUI = 0x0400,
    /// <summary>
    /// dont copy NT file Security Attributes
    /// </summary>
    FOF_NOCOPYSECURITYATTRIBS = 0x0800,
    /// <summary>
    /// don't recurse into directories.
    /// </summary>
    FOF_NORECURSION = 0x1000,
    /// <summary>
    /// don't operate on connected file elements.
    /// </summary>
    FOF_NO_CONNECTED_ELEMENTS = 0x2000,
    /// <summary>
    /// during delete operation, warn if nuking instead of recycling (partially overrides FOF_NOCONFIRMATION)
    /// </summary>
    FOF_WANTNUKEWARNING = 0x4000,
    /// <summary>
    /// treat reparse points as objects, not containers
    /// </summary>
    FOF_NORECURSEREPARSE = 0x8000,

    /// <summary>
    /// Don't avoid binding to junctions (like Task folder, Recycle-Bin)
    /// </summary>
    FOFX_NOSKIPJUNCTIONS = 0x00010000,
    /// <summary>
    /// Create hard link if possible
    /// </summary>
    FOFX_PREFERHARDLINK = 0x00020000,
    /// <summary>
    /// Show elevation prompts when error UI is disabled (use with FOF_NOERRORUI)
    /// </summary>
    FOFX_SHOWELEVATIONPROMPT = 0x00040000,
    /// <summary>
    /// Fail operation as soon as a single error occurs rather than trying to process other items (applies only when using FOF_NOERRORUI)
    /// </summary>
    FOFX_EARLYFAILURE = 0x00100000,
    /// <summary>
    /// Rename collisions preserve file extns (use with FOF_RENAMEONCOLLISION)
    /// </summary>
    FOFX_PRESERVEFILEEXTENSIONS = 0x00200000,
    /// <summary>
    /// Keep newer file on naming conflicts
    /// </summary>
    FOFX_KEEPNEWERFILE = 0x00400000,
    /// <summary>
    /// Don't use copy hooks
    /// </summary>
    FOFX_NOCOPYHOOKS = 0x00800000,
    /// <summary>
    /// Don't allow minimizing the progress dialog
    /// </summary>
    FOFX_NOMINIMIZEBOX = 0x01000000,
    /// <summary>
    /// Copy security information when performing a cross-volume move operation
    /// </summary>
    FOFX_MOVEACLSACROSSVOLUMES = 0x02000000,
    /// <summary>
    /// Don't display the path of source file in progress dialog
    /// </summary>
    FOFX_DONTDISPLAYSOURCEPATH = 0x04000000,
    /// <summary>
    /// Don't display the path of destination file in progress dialog
    /// </summary>
    FOFX_DONTDISPLAYDESTPATH = 0x08000000,
    /// <summary>
    /// When Delete, move the file to the recycle bin directory
    /// </summary>
    FOFX_RECYCLEONDELETE = 0x00080000,
    FOFX_REQUIREELEVATION = 0x10000000,
    FOFX_ADDUNDORECORD = 0x20000000,
    FOFX_COPYASDOWNLOAD = 0x40000000,
    FOFX_DONTDISPLAYLOCATIONS = 0x80000000,
}
