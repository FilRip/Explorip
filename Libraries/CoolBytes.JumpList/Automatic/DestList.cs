using System;
using System.Collections.Generic;
using System.Text;

namespace CoolBytes.JumpList.Automatic;

public class DestList
{
    public DestList(byte[] rawBytes)
    {
        Entries = [];

        byte[] headerBytes = new byte[32];
        Buffer.BlockCopy(rawBytes, 0, headerBytes, 0, 32);

        Header = new DestListHeader(headerBytes);

        int index;
        int pathSize;
        int entrySize;

        int mruPos = 0;

        if (Header.Version == 1)
        {
            index = 32;

            while (index < rawBytes.Length && Entries.Count < Header.NumberOfEntries)
            {
                pathSize = BitConverter.ToInt16(rawBytes, index + 112);
                //now that we know pathSize we can determine how big each record is
                entrySize = 114 + pathSize * 2;

                byte[] entryBytes1 = new byte[entrySize];
                Buffer.BlockCopy(rawBytes, index, entryBytes1, 0, entrySize);

                DestListEntry entry1 = new(entryBytes1, Header.Version, mruPos);

                Entries.Add(entry1);

                index += entrySize;

                mruPos += 1;
            }
        }
    
        else
        {
            //windows 10 has version 3              

            index = 32;

            while (index < rawBytes.Length)
            {
                pathSize = BitConverter.ToInt16(rawBytes, index + 128);
                //now that we know pathSize we can determine how big each record is
                entrySize = 128 + 2 + pathSize * 2;
                //128 is offset to the string, 2 for the size itself, double path for unicode

                //after entry size, if the 4 bytes are 0, theres no extra data, but if not, its an SPS
                int spsSize = BitConverter.ToInt32(rawBytes,index + entrySize);



                byte[] entryBytes2 = new byte[entrySize+spsSize+4];
                Buffer.BlockCopy(rawBytes, index, entryBytes2, 0, entrySize+spsSize);

                DestListEntry entry2 = new(entryBytes2, Header.Version, mruPos,spsSize);

                Entries.Add(entry2);

                index += entrySize + spsSize + 4; //4 is the size of the sps

                mruPos += 1;
            }
        }
    }

    public DestListHeader Header { get; }

    public List<DestListEntry> Entries { get; }

    public override string ToString()
    {
        StringBuilder sb = new();

        sb.AppendLine($"Header: {Header}");

        sb.AppendLine("");
        sb.AppendLine($"DestList Entries Expected: {Header.NumberOfEntries:N0}, Found: {Entries.Count:N0}");
        foreach (DestListEntry destListEntry in Entries)
        {
            sb.AppendLine(destListEntry.ToString());
        }

        return sb.ToString();
    }
}
