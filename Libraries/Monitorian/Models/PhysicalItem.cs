namespace Monitorian.Models;

public class PhysicalItem(
    string description,
    int monitorIndex,
    SafePhysicalMonitorHandle handle,
    MonitorCapability capability)
{
    public string Description { get; } = description;

    public int MonitorIndex { get; } = monitorIndex;

    public SafePhysicalMonitorHandle Handle { get; } = handle;

    public MonitorCapability Capability { get; } = capability;
}
