using System;
using System.Collections.Generic;
using System.IO;

using CoolBytes.JumpList.Models;

using OpenMcdf;

using Securify.ShellLink;

namespace CoolBytes.JumpList;

public static class ExtensionsJumpList
{
    public static Models.JumpList? GetAutomaticJumpList(string fullPath)
    {
        string pathAutomatic;
        pathAutomatic = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Microsoft", "Windows", "Recent", "AutomaticDestinations");
        foreach (string file in Directory.GetFiles(pathAutomatic, "*.automaticDestinations-ms"))
        {
            Models.JumpList? result = null;
            CompoundFile cf = new(file);
            if (cf.RootStorage.TryGetStream("DestList", out CFStream cfStorage))
            {
                byte[] destListData = cfStorage.GetData();
                if (destListData.Length > 32)
                {
                    int offset = 0;
                    result = new Models.JumpList();
                    result.Header.Version = BitConverter.ToInt32(destListData, offset);
                    result.Header.CountEntries = BitConverter.ToInt32(destListData, offset + 4);
                    result.Header.CountPinnedEntries = BitConverter.ToInt32(destListData, offset + 8);
                    result.Header.LastId = BitConverter.ToInt32(destListData, offset + 16);
                    result.Header.LastRevision = BitConverter.ToInt32(destListData, offset + 24);
                    offset = 32;
                    for (int count = 0; count < result.Header.CountEntries; count++)
                    {
                        JumpListItem item = new()
                        {
                            //Id = reader.ReadUInt64(),
                        };
                        /*ulong timestamp = reader.ReadUInt64();
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
                        item.StreamNumber = reader.ReadUInt64();*/
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
                    }
                }
            }
            cf.Close();
            cf.Dispose();
            if (result?.Items?.Count > 0 && result.Items[0].FullPath().ToLower() == fullPath.ToLower())
                return result;
        }
        return null;
    }

    public static List<Shortcut> GetCustomJumpList(string fullPath)
    {
        string pathCustom;
        pathCustom = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Microsoft", "Windows", "Recent", "CustomDestinations");

        List<Shortcut> result = [];

        if (File.Exists(pathCustom))
        {
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
        }
        return result;
    }
}
