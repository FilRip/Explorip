using System;
using System.Runtime.InteropServices;

namespace Explorip.WinAPI.Modeles
{
    [Flags()]
    public enum DWM_TNP
    {
        RECTDESTINATION = 0x00000001,
        RECTSOURCE = 0x00000002,
        OPACITY = 0x00000004,
        VISIBLE = 0x00000008,
        SOURCECLIENTAREAONLY = 0x00000010,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DWM_THUMBNAIL_PROPERTIES
    {
        public DWM_TNP dwFlags;
        public RECT rcDestination;
        public RECT rcSource;
        public byte opacity;
        public bool fVisible;
        public bool fSourceClientAreaOnly;
    }
}
