using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Monitorian.Models;

public class MonitorCapability
{
    public bool IsBrightnessSupported => IsHighLevelBrightnessSupported || IsLowLevelBrightnessSupported;

    public bool IsHighLevelBrightnessSupported { get; }

    public bool IsLowLevelBrightnessSupported { get; }

    public bool IsContrastSupported { get; }

    public bool IsPrecleared { get; }

    public IReadOnlyDictionary<byte, IReadOnlyList<byte>> CapabilitiesCodes { get; }

    public string CapabilitiesString { get; }

    public string CapabilitiesReport { get; }

    public string CapabilitiesData { get; }

    public MonitorCapability(
        bool isHighLevelBrightnessSupported,
        bool isLowLevelBrightnessSupported,
        bool isContrastSupported,
        IReadOnlyDictionary<byte, byte[]> capabilitiesCodes = null,
        string capabilitiesString = null,
        string capabilitiesReport = null,
        byte[] capabilitiesData = null) : this(
            isHighLevelBrightnessSupported: isHighLevelBrightnessSupported,
            isLowLevelBrightnessSupported: isLowLevelBrightnessSupported,
            isContrastSupported: isContrastSupported,
            isPrecleared: false,
            capabilitiesCodes: capabilitiesCodes,
            capabilitiesString: capabilitiesString,
            capabilitiesReport: capabilitiesReport,
            capabilitiesData: capabilitiesData)
    { }

    private MonitorCapability(
        bool isHighLevelBrightnessSupported,
        bool isLowLevelBrightnessSupported,
        bool isContrastSupported,
        bool isPrecleared,
        IReadOnlyDictionary<byte, byte[]> capabilitiesCodes,
        string capabilitiesString,
        string capabilitiesReport,
        byte[] capabilitiesData)
    {
        this.IsHighLevelBrightnessSupported = isHighLevelBrightnessSupported;
        this.IsLowLevelBrightnessSupported = isLowLevelBrightnessSupported;
        this.IsContrastSupported = isContrastSupported;
        this.IsPrecleared = isPrecleared;
        this.CapabilitiesCodes = (capabilitiesCodes is not null) ? new ReadOnlyDictionary<byte, IReadOnlyList<byte>>(capabilitiesCodes.ToDictionary(x => x.Key, x => (x.Value is not null) ? (IReadOnlyList<byte>)Array.AsReadOnly(x.Value) : null)) : null;
        this.CapabilitiesString = capabilitiesString;
        this.CapabilitiesReport = capabilitiesReport;
        this.CapabilitiesData = (capabilitiesData is not null) ? Convert.ToBase64String(capabilitiesData) : null;
    }

    public static MonitorCapability PreclearedCapability => _preclearedCapability.Value;
    private static readonly Lazy<MonitorCapability> _preclearedCapability = new(() =>
        new(isHighLevelBrightnessSupported: false,
            isLowLevelBrightnessSupported: true,
            isContrastSupported: true,
            isPrecleared: true,
            capabilitiesCodes: null,
            capabilitiesString: null,
            capabilitiesReport: null,
            capabilitiesData: null));
}
