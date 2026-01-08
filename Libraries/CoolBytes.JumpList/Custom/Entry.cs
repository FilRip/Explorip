using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Securify.ShellLink;

namespace CoolBytes.JumpList.Custom;

public class Entry
{
    private readonly byte[] footerBytes = [0xAB, 0xFB, 0xBF, 0xBA];

    private readonly byte[] lnkHeaderBytes =
    [
        0x4C, 0x00, 0x00, 0x00, 0x01, 0x14, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00,
        0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46
    ];

    private readonly Dictionary<string, byte[]> lnkBytes;

    public Entry(byte[] rawBytes, int entryOffset)
    {
        LnkFiles = [];
        lnkBytes = [];

        Unknown0 = BitConverter.ToInt32(rawBytes, 0);
        Rank = BitConverter.ToSingle(rawBytes, 4);
        Unknown2 = BitConverter.ToInt32(rawBytes, 8);
        HeaderType = BitConverter.ToInt32(rawBytes, 12);

        Name = string.Empty;
        if (HeaderType == 0)
        {
            short nameLen = BitConverter.ToInt16(rawBytes, 16);
            Name = Encoding.Unicode.GetString(rawBytes, 18, nameLen * 2).Split('\0')[0];
        }

        List<int> lnkOffsets = [];
        int index = 0;

        int footerPos = CustomDestination.ByteSearch(rawBytes, footerBytes, index);

        while (index < rawBytes.Length)
        {
            int lo = CustomDestination.ByteSearch(rawBytes, lnkHeaderBytes, index);

            if (lo == -1)
            {
                break;
            }

            lnkOffsets.Add(lo);

            index = lo + 1; //add length so we do not hit on it again
        }

        //  Debug.WriteLine($"Link offsets contains {lnkOffsets.Count} offsets: {string.Join(", ", lnkOffsets)}");

        //  Debug.WriteLine($"Footer pos: {footerPos}");

        int counter = 0;
        int max = lnkOffsets.Count - 1;
        foreach (int lnkOffset in lnkOffsets)
        {
            int start = 0;
            int end = 0;
            if (counter == max)
            {
                //last one, so we need to use footerpos
                start = lnkOffset;
                end = footerPos;
            }
            else
            {
                start = lnkOffset;
                end = lnkOffsets[counter + 1];
            }

            byte[] bytes = new byte[end - start];

            Buffer.BlockCopy(rawBytes, start, bytes, 0, bytes.Length);

            string name = $"Offset_0x{entryOffset + lnkOffset:X}.lnk";

            lnkBytes.Add(name, bytes);

            Shortcut l = Shortcut.FromByteArray(bytes);

            LnkFiles.Add(l);

            counter += 1;
        }
    }

    public string Name { get; }
    public int Unknown0 { get; }
    public float Rank { get; }
    public int Unknown2 { get; }
    public int HeaderType { get; }

    public List<Shortcut> LnkFiles { get; }
}
