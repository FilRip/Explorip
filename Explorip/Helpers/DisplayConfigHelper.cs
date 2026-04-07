using System.Collections.Generic;

using static ManagedShell.Interop.NativeMethods;

namespace Explorip.Helpers;

public class MonitorInfo
{
    public string DeviceName { get; set; }
    public bool IsActive { get; set; }
    public bool IsPrimary { get; set; }
    public MonitorMode Mode { get; set; }
    public int? CloneGroupId { get; set; } // Pour identifier les groupes dupliqués

    public override string ToString()
    {
        return $"{DeviceName} - {Mode} - Active={IsActive} - Primary={IsPrimary}";
    }
}

public enum MonitorMode
{
    Disabled,
    Extended,
    Duplicated,
    Primary,
    Unknown
}

public static class DisplayConfigForDesktop
{
    public static (DisplayConfigTopologyIds topology, List<MonitorInfo> monitors) GetDesktopMonitorConfig()
    {
        GetDisplayConfigBufferSizes(QueryDisplayConfigs.DatabaseCurrent, out uint pathCount, out uint modeCount);

        DisplayConfigPathInfo[] paths = new DisplayConfigPathInfo[pathCount];
        DisplayConfigModeInfo[] modes = new DisplayConfigModeInfo[modeCount];

        QueryDisplayConfig(QueryDisplayConfigs.DatabaseCurrent,
            ref pathCount, paths,
            ref modeCount, modes,
            out DisplayConfigTopologyIds topo);

        List<MonitorInfo> monitors = BuildMonitorList(paths, topo);

        return (topo, monitors);
    }

    private static List<MonitorInfo> BuildMonitorList(DisplayConfigPathInfo[] paths, DisplayConfigTopologyIds topology)
    {
        List<MonitorInfo> list = [];
        int cloneGroup = 1;

        foreach (DisplayConfigPathInfo p in paths)
        {
            MonitorInfo info = new()
            {
                DeviceName = $"DISPLAY{p.sourceInfo.id}",
                IsActive = (p.flags & 1) != 0,
                IsPrimary = (p.sourceInfo.statusFlags & 1) != 0,
            };

            if (!info.IsActive)
                info.Mode = MonitorMode.Disabled;
            else if (topology == DisplayConfigTopologyIds.Clone)
            {
                info.Mode = MonitorMode.Duplicated;
                info.CloneGroupId = cloneGroup;
            }
            else if (topology == DisplayConfigTopologyIds.Extended)
                info.Mode = info.IsPrimary ? MonitorMode.Primary : MonitorMode.Extended;
            else
                info.Mode = MonitorMode.Unknown;

            list.Add(info);
        }

        return list;
    }
}
