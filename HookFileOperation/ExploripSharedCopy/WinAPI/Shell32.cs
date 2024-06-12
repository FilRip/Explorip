using System.Runtime.InteropServices;

using ExploripSharedCopy.WinAPI.Modeles;

namespace ExploripSharedCopy.WinAPI;

internal static class Shell32
{
    [DllImport("shell32.dll")]
    internal static extern uint SHQueryRecycleBin(string drive, ref ShQueryRbInfo result);

    internal static bool RecycledEnabledOnDrive(string drive)
    {
        if (drive.StartsWith("\\"))
            return false;
        ShQueryRbInfo data = new()
        {
            cbSize = Marshal.SizeOf(typeof(ShQueryRbInfo)),
        };
        uint result = SHQueryRecycleBin(drive, ref data);
        return result == 0;
    }
}
