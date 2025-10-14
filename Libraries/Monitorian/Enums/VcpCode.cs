namespace Monitorian.Enums;

internal enum VcpCode : byte
{
    None = 0x0,
    Luminance = 0x10,
    Contrast = 0x12,
    Temperature = 0x14, // Select Color Preset
    InputSource = 0x60, // Input Select
    SpeakerVolume = 0x62,
    PowerMode = 0xD6,
}
