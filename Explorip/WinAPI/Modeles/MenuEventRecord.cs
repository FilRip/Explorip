using System.Runtime.InteropServices;

namespace Explorip.WinAPI.Modeles;

[StructLayout(LayoutKind.Sequential)]
public struct MenuEventRecord
{
    public uint dwCommandId;
}
