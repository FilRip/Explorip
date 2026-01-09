using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using CoolBytes.JumpList.ExtensionBlocks;

using OleCf;

using Securify.ShellLink;

namespace CoolBytes.JumpList.Automatic;

public class AutomaticDestination
{
    public AutomaticDestination(string sourceFile)
    {
        SourceFile = sourceFile;

        byte[] rawBytes = File.ReadAllBytes(sourceFile);

        OleCfFile oleContainer = new(rawBytes, sourceFile);

        Directory = oleContainer.Directory;

        DirectoryEntry destList =
            oleContainer.Directory.SingleOrDefault(t => t.DirectoryName.ToLowerInvariant() == "destlist");
        if (destList != null && destList.DirectorySize > 0)
        {
            byte[] destBytes = oleContainer.GetPayloadForDirectory(destList);

            DestList = new DestList(destBytes);
        }

        DirectoryEntry destListPropertyStore =
            oleContainer.Directory.SingleOrDefault(t => t.DirectoryName == "DestListPropertyStore");
        if (destListPropertyStore != null && destListPropertyStore.DirectorySize > 0)
        {
            byte[] destListPropertyStoreBytes = oleContainer.GetPayloadForDirectory(destListPropertyStore);

            EmptyDestListPropertyStore = true;

            if (BitConverter.ToInt32(destListPropertyStoreBytes, 0) > 0)
            {
                EmptyDestListPropertyStore = false;
                DestListPropertyStore = new PropertySheet([.. destListPropertyStoreBytes.Skip(4)]);
            }
        }

        DestListEntries = [];

        if (DestList != null)
        {
            DestListCount = DestList.Header.NumberOfEntries;
            DestListVersion = DestList.Header.Version;
            LastUsedEntryNumber = DestList.Header.LastEntryNumber;

            foreach (DestListEntry entry in DestList.Entries)
            {
                DirectoryEntry dirItem =
                    oleContainer.Directory.SingleOrDefault(
                        t =>
                            string.Equals(t.DirectoryName, entry.EntryNumber.ToString("X"),
                                StringComparison.InvariantCultureIgnoreCase));

                if (dirItem != null)
                {
                    byte[] p = oleContainer.GetPayloadForDirectory(dirItem);

                    Shortcut dlnk = Shortcut.FromByteArray(p);

                    AutoDestList dle = new(entry, dlnk, entry.InteractionCount);

                    if (entry.Sps != null && !HasSps)
                    {
                        HasSps = true;
                    }

                    DestListEntries.Add(dle);
                }
                else
                {
                    AutoDestList dleNull = new(entry, null, entry.InteractionCount);

                    DestListEntries.Add(dleNull);
                }
            }
        }
    }

    public List<DirectoryEntry> Directory { get; }

    public int DestListCount { get; }

    public int PinnedDestListCount { get; }

    public int LastUsedEntryNumber { get; }

    public int DestListVersion { get; }

    public string SourceFile { get; internal set; }

    public DateTime HdLastModified { get; internal set; }

    private DestList? DestList { get; }

    public bool HasSps { get; }

    public PropertySheet? DestListPropertyStore { get; }

    public bool EmptyDestListPropertyStore { get; }

    public List<AutoDestList> DestListEntries { get; }
}
