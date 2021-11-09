using System;
using System.Runtime.InteropServices;

namespace Explorip.WinAPI.Modeles
{
    [Flags()]
    public enum DWM_FLAGS : int
    {
        DWM_TNP_RECTDESTINATION = 0x00000001,
        DWM_TNP_RECTSOURCE = 0x00000002,
        DWM_TNP_OPACITY = 0x00000004,
        DWM_TNP_VISIBLE= 0x00000008,
        DWM_TNP_SOURCECLIENTAREAONLY = 0x00000010,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DWM_THUMBNAIL_PROPERTIES
    {
        public DWM_FLAGS dwFlags;
        public RECT rcDestination;
        public RECT rcSource;
        public byte opacity;
        public bool fVisible;
        public bool fSourceClientAreaOnly;
    }
}
