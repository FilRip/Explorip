using System.Runtime.InteropServices;

namespace Explorip.WinAPI.Modeles
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DwmColorizationParams
    {
        public uint clrColor;
        public uint clrAfterGlow;
        public uint nIntensity;
        public uint clrAfterGlowBalance;
        public uint clrBlurBalance;
        public uint clrGlassReflectionIntensity;
        public bool fOpaque;
    }
}
