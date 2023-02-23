﻿using System.Runtime.InteropServices;

namespace Explorip.WinAPI.Modeles
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct NativePoint
    {
        public NativePoint(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int x;
        public int y;
    }
}