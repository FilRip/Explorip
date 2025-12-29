using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;

using Microsoft.Win32;

using VirtualDesktop.Properties;
using VirtualDesktop.Utils;

namespace VirtualDesktop.Interop;

internal class OsBuildSettings
{
    internal Version? OSBuild { get; set; }
    internal SettingsProperty? Properties { get; set; }
}

internal static class Iid
{
    private static readonly Regex _osBuildRegex = new(@"v_(?<build>\d+_\d+)");

    // ReSharper disable once InconsistentNaming
    public static Dictionary<string, Guid> GetIIDs(string[] interfaceNames)
    {
        Dictionary<string, Guid> result = [];

        // Order configuration props by build version
        OsBuildSettings?[] orderedProps = [.. Settings.Default.Properties.OfType<SettingsProperty>()
            .Select(prop =>
            {
                if (Version.TryParse(OS.VersionPrefix + _osBuildRegex.Match(prop.Name).Groups["build"].ToString().Replace('_', '.'), out Version? build))
                {
                    return new OsBuildSettings()
                    {
                        OSBuild = build,
                        Properties = prop,
                    };
                }

                return null;
            })
            .Where(s => s != null)
            .OrderByDescending(s => s!.OSBuild)];

        // TODO: Select per major version first?
        // Find first prop with build version <= current OS version
        OsBuildSettings? selectedSettings = orderedProps.FirstOrDefault(p => p != null && p.OSBuild <= OS.Build);

        if (selectedSettings == null)
        {
            Version?[] supportedBuilds = [.. orderedProps.Select(v => v!.OSBuild)];
            throw new VirtualDesktopException(
                "Invalid application configuration. Unable to determine interop interfaces for " +
                $"current OS Build: {OS.Build}. All configured OS Builds " +
                $"have build version greater than current OS: {supportedBuilds}");
        }

        foreach (string? str in (StringCollection)Settings.Default[selectedSettings.Properties!.Name])
        {
            if (str == null)
                continue;

            string[] pair = str.Split(',');
            if (pair.Length != 2)
                continue;
            if (!interfaceNames.Contains(pair[0]) || result.ContainsKey(pair[0]))
                continue;
            if (!Guid.TryParse(pair[1], out Guid guid))
                continue;

            result.Add(pair[0], guid);
        }

        string[] except = [.. interfaceNames.Except(result.Keys)];
        if (except.Length > 0)
        {
            foreach (KeyValuePair<string, Guid> kvp in GetIIDsFromRegistry(except))
                result.Add(kvp.Key, kvp.Value);
        }

        return result;
    }

    // ReSharper disable once InconsistentNaming
    private static Dictionary<string, Guid> GetIIDsFromRegistry(string[] targets)
    {
        using RegistryKey interfaceKey = Registry.ClassesRoot.OpenSubKey("Interface")
            ?? throw new VirtualDesktopException(@"Registry key '\HKEY_CLASSES_ROOT\Interface' is missing.");

        Dictionary<string, Guid> result = [];

        foreach (string? name in interfaceKey.GetSubKeyNames())
        {
            using RegistryKey key = interfaceKey.OpenSubKey(name);

            if (key?.GetValue("") is string value)
            {
                string match = targets.FirstOrDefault(x => x == value);
                string[] splitter = key.Name.Split('\\');
                if (match != null && Guid.TryParse(splitter[splitter.Length - 1], out Guid guid))
                    result[match] = guid;
            }
        }

        return result;
    }
}
