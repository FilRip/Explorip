using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using CoolBytes.JumpList.Models;

using OpenMcdf;

using Securify.ShellLink;

namespace CoolBytes.JumpList;

public static class ExtensionsJumpList
{
    public static uint ComputeWindowsCrc32(string appId)
    {
        byte[] bytes = Encoding.Unicode.GetBytes(appId);

        uint crc = 0xFFFFFFFF;

        foreach (byte b in bytes)
        {
            crc ^= b;
            for (int i = 0; i < 8; i++)
            {
                if ((crc & 1) != 0)
                    crc = (crc >> 1) ^ 0xEDB88320;
                else
                    crc >>= 1;
            }
        }

        return ~crc;
    }

    public static Models.JumpList? GetAutomaticJumpList(string appModelUserId)
    {
        uint crc32 = ComputeWindowsCrc32(appModelUserId);
        string filename = crc32.ToString("X");
        string pathAutomatic;
        pathAutomatic = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Microsoft", "Windows", "Recent", "AutomaticDestinations") + filename + ".automaticDestinations-ms";
        Models.JumpList? result = null;
        if (File.Exists(pathAutomatic))
        {
            CompoundFile cf = new(pathAutomatic);
            if (cf.RootStorage.TryGetStream("DestList", out CFStream cfStorage))
            {
                byte[] destListData = cfStorage.GetData();
                if (destListData.Length > 32)
                {
                    using MemoryStream streamDestListData = new(destListData);
                    BinaryReader reader = new(streamDestListData);
                    result = new Models.JumpList();
                    result.Header.Version = reader.ReadUInt64();
                    result.Header.CountEntries = reader.ReadUInt64();
                    result.Header.LastId = reader.ReadUInt64();
                    result.Header.Reserved = reader.ReadUInt64();
                    for (ulong count = 0; count < result.Header.CountEntries; count++)
                    {
                        JumpListItem item = new()
                        {
                            Id = reader.ReadUInt64(),
                        };
                        ulong timestamp = reader.ReadUInt64();
                        item.SetTimestamp(timestamp);
                        item.AccessCount = reader.ReadUInt32();
                        item.Pinned = (reader.ReadUInt32() == 1);
                        reader.ReadUInt64();
                        reader.ReadUInt64();
                        reader.ReadUInt64();
                        reader.ReadUInt64();
                        reader.ReadUInt64();
                        reader.ReadUInt64();
                        reader.ReadUInt64();
                        reader.ReadUInt64();
                        reader.ReadUInt64();
                        reader.ReadUInt64();
                        reader.ReadUInt64();
                        reader.ReadUInt64();
                        item.StreamNumber = reader.ReadUInt64();
                        if (cf.RootStorage.TryGetStream(item.StreamNumber.ToString(), out CFStream cfStream))
                        {
                            byte[] lnkData = cfStream.GetData();
                            try
                            {
                                item.AddShortcut(lnkData);
                            }
                            catch (Exception) { /* Ignore errors */ }
                        }
                        result.Items.Add(item);
                        reader.Close();
                        reader.Dispose();
                    }
                }
            }
        }
        return result;
    }

    public static List<Shortcut> GetCustomJumpList(string appModelUserId)
    {
        uint crc32 = ComputeWindowsCrc32(appModelUserId);
        string filename = crc32.ToString("X");
        string pathCustom;
        pathCustom = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Microsoft", "Windows", "Recent", "CustomDestinations") + filename + ".customDestinations-ms";

        List<Shortcut> result = [];

        using (FileStream fs = new(pathCustom, FileMode.Open, FileAccess.Read))
        {
            using BinaryReader br = new(fs);
            br.ReadUInt32(); // Version
            uint count = br.ReadUInt32();
            for (int i = 0; i < count; i++)
            {
                try
                {
                    uint size = br.ReadUInt32();
                    if (size == 0 || size > fs.Length)
                        break;
                    byte[] lnkBytes = br.ReadBytes((int)size);
                    result.Add(Shortcut.FromByteArray(lnkBytes));
                }
                catch (Exception)
                {
                    // Ignore errors
                }
            }
        }
        return result;
    }
}
