using System;

namespace ManagedShell.ShellFolders.Enums;

[Flags()]
public enum ShellItemImageGetBitmaps
{
    RESIZETOFIT = 0x00,
    BIGGERSIZEOK = 0x01,
    MEMORYONLY = 0x02,
    ICONONLY = 0x04,
    THUMBNAILONLY = 0x08,
    INCACHEONLY = 0x10,
    CROPTOSQUARE = 0x00000020,
    WIDETHUMBNAILS = 0x00000040,
    ICONBACKGROUND = 0x00000080,
    SCALEUP = 0x00000100,
}
