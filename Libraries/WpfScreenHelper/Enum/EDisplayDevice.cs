using System;

namespace WpfScreenHelper.Enum;

[Flags()]
public enum EDisplayDevice
{
    Active = 0x00000001,
    Attached = 0x00000002,
    PrimaryDevice = 0x00000004,
    MirroringDriver = 0x00000008,
    VgaCompatible = 0x00000010,
    Removable = 0x00000020,
    AccDriver = 0x00000040,
    ModesPruned = 0x08000000,
    RpdUdd = 0x01000000,
    Remote = 0x04000000,
    Disconnect = 0x02000000,
    TSCompatible = 0x00200000,
    UnsafeModesOn = 0x00080000,
}
