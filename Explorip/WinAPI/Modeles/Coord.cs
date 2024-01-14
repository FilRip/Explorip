﻿using System.Runtime.InteropServices;

namespace Explorip.WinAPI.Modeles;

[StructLayout(LayoutKind.Sequential)]
public struct Coord
{
    public short X;
    public short Y;

    public Coord(short X, short Y)
    {
        this.X = X;
        this.Y = Y;
    }
}
