using System;
using System.Collections.Generic;

namespace CoolBytes.JumpList.Helpers;

internal static class Utils
{
    public static string ExtractGuidFromShellItem(byte[] rawguid)
    {
        byte[] part1 = new byte[4];
        byte[] part2 = new byte[2];
        byte[] part3 = new byte[2];
        byte[] part4 = new byte[2];
        byte[] part5 = new byte[6];

        Array.Copy(rawguid, 0, part1, 0, 4);
        Array.Copy(rawguid, 4, part2, 0, 2);
        Array.Copy(rawguid, 6, part3, 0, 2);
        Array.Copy(rawguid, 8, part4, 0, 2);
        Array.Copy(rawguid, 10, part5, 0, 6);

        Array.Reverse(part1);
        Array.Reverse(part2);
        Array.Reverse(part3);

        string p1 = BitConverter.ToString(part1).Replace("-", "");
        string p2 = BitConverter.ToString(part2).Replace("-", "");
        string p3 = BitConverter.ToString(part3).Replace("-", "");
        string p4 = BitConverter.ToString(part4).Replace("-", "");
        string p5 = BitConverter.ToString(part5).Replace("-", "");

        return $"{p1}-{p2}-{p3}-{p4}-{p5}".ToLowerInvariant();
    }

    public static string GetFolderNameFromGuid(string guid)
    {
        return GetDescriptionFromGuid(guid);
    }

    private static readonly Dictionary<string, string> Guids = [];

    public static string GetDescriptionFromGuid(string guid)
    {
        string tempValue = guid.ToLowerInvariant();

        if (tempValue.StartsWith("{"))
        {
            tempValue = tempValue.Replace("{", "").Replace("}", "");
        }

        if (Guids.ContainsKey(tempValue))
        {
            return Guids[tempValue];
        }

        return $"Unmapped GUID: {guid}";
    }

    public static string GetDescriptionFromGuidAndKey(string guid, int key)
    {
        string desc = ManagedShell.Common.Interfaces.PredefinedPropertyKey.GetPropertyKeyName(new Guid(guid), (uint)key);
        if (string.IsNullOrWhiteSpace(desc))
            desc = "(Description not available)";

        return desc;
    }
}
