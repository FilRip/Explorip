using System.Runtime.InteropServices;

namespace ComputerInfo.Helpers;

internal static class NativeMethods
{
    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetPhysicallyInstalledSystemMemory(out long TotalMemoryInKilobytes);
}
