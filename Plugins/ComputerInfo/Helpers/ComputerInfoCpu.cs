using System.Diagnostics;

namespace ComputerInfo.Helpers;

public static class ComputerInfoCpu
{
    private static readonly PerformanceCounter _cpuCounter = new("Processor Information", "% Processor Utility", "_Total");

    public static float PercentCpuUsed
    {
        get { return _cpuCounter.NextValue(); }
    }
}
