using System.Runtime.InteropServices;

namespace Explorip.WinAPI.Modeles;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public struct UniversalNameInfo
{
    /// <summary>
    /// Pointer to the null-terminated UNC name string that identifies a
    /// network resource.
    /// </summary>
    [MarshalAs(UnmanagedType.LPTStr)]
    public string lpUniversalName;
}
