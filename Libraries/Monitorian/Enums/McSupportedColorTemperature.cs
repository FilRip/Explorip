using System;

namespace Monitorian.Enums;

[Flags()]
internal enum McSupportedColorTemperature
{
    None = 0x00000000,
    T4000K = 0x00000001,
    T5000K = 0x00000002,
    T6500K = 0x00000004,
    T7500K = 0x00000008,
    T8200K = 0x00000010,
    T9300K = 0x00000020,
    T10000K = 0x00000040,
    T11500K = 0x00000080
}
