using System.Diagnostics;

namespace ComputerInfo.Helpers;

public static class ComputerInfoMemory
{
    private static readonly PerformanceCounter _ramCounter = new("Memory", "Available KBytes");

    public static long TotalMemory
    {
        get
        {
            if (NativeMethods.GetPhysicallyInstalledSystemMemory(out long totalMemory))
                return totalMemory;
            return 0;
        }
    }

    public static long TotalUsed
    {
        get { return TotalMemory - TotalFree; }
    }

    public static long TotalFree
    {
        get { return (long)_ramCounter.NextValue(); }
    }
}
