using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using CoolBytes.JumpList.ExtensionBlocks;
using CoolBytes.JumpList.Helpers;

namespace CoolBytes.JumpList.Automatic;

public class DestListEntry
{
    public DestListEntry(byte[] rawBytes, int version, int mruPosition, int spsSize = 0)
    {
        MRUPosition = mruPosition;

        Checksum = BitConverter.ToInt64(rawBytes, 0);

        byte[] volDroidBytes = new byte[16];
        Buffer.BlockCopy(rawBytes, 8, volDroidBytes, 0, 16);

        VolumeDroid = new Guid(volDroidBytes);

        byte[] fileDroidBytes = new byte[16];
        Buffer.BlockCopy(rawBytes, 24, fileDroidBytes, 0, 16);

        FileDroid = new Guid(fileDroidBytes);

        byte[] volBirthDroidBytes = new byte[16];
        Buffer.BlockCopy(rawBytes, 40, volBirthDroidBytes, 0, 16);

        VolumeBirthDroid = new Guid(volBirthDroidBytes);

        byte[] fileBirthDroidBytes = new byte[16];
        Buffer.BlockCopy(rawBytes, 56, fileBirthDroidBytes, 0, 16);

        FileBirthDroid = new Guid(fileBirthDroidBytes);

        if (rawBytes[73] == 0)
        {

            Hostname = Encoding.Unicode.GetString(rawBytes, 72, 16).Split('\0')[0];
        }
        else
        {
            Hostname = Encoding.ASCII.GetString(rawBytes, 72, 16).Split('\0')[0];
        }

        EntryNumber = BitConverter.ToInt32(rawBytes, 88);
        Unknown0 = BitConverter.ToInt32(rawBytes, 92);
        AccessCount = BitConverter.ToSingle(rawBytes, 96);

        LastModified = DateTimeOffset.FromFileTime(BitConverter.ToInt64(rawBytes, 100)).ToUniversalTime();




        PinStatus = BitConverter.ToInt32(rawBytes, 108);

        if (version > 1)
        {
            InteractionCount = BitConverter.ToInt32(rawBytes, 116);
            Unknown3 = BitConverter.ToInt32(rawBytes, 120);
            Unknown4 = BitConverter.ToInt32(rawBytes, 124);

            int v3PathLen = BitConverter.ToInt16(rawBytes, 128) * 2;

            Path = Encoding.Unicode.GetString(rawBytes, 130, v3PathLen);
        }
        else
        {
            int v1PathLen = BitConverter.ToInt16(rawBytes, 112) * 2;

            Path = Encoding.Unicode.GetString(rawBytes, 114, v1PathLen);
        }

        if (Path.StartsWith("knownfolder"))
        {
            string[] splitter = Path.Split('{');
            string kfId = splitter[splitter.Length - 1];
            kfId = kfId.Substring(0, kfId.Length - 1);

            string fName = Utils.GetFolderNameFromGuid(kfId);

            Path = $"{Path} ==> {fName}";
        }

        if (Path.StartsWith("::"))
        {
            string[] pathSegs = Path.Split('\\');

            List<string> newPathSegs = [];

            foreach (string? pathSeg in pathSegs)
            {
                try
                {
                    Regex regexObj =
                        new(
                            @"\b[A-F0-9]{8}(?:-[A-F0-9]{4}){3}-[A-F0-9]{12}\b|\(\b[A-F0-9]{8}(?:-[A-F0-9]{4}){3}-[A-F0-9]{12}\b\)|\{\b[A-F0-9]{8}(?:-[A-F0-9]{4}){3}-[A-F0-9]{12}\b\}",
                            RegexOptions.IgnoreCase);
                    Match matchResults = regexObj.Match(pathSeg);


                    if (matchResults.Success)
                    {
                        string pguid = matchResults.Groups[0].Value;
                        pguid = pguid.Substring(1, pguid.Length - 2);
                        string pName = Utils.GetFolderNameFromGuid(pguid);
                        newPathSegs.Add(pName);
                    }
                    else
                    {
                        newPathSegs.Add(pathSeg);
                    }
                }
                catch (ArgumentException)
                {
                    // Syntax error in the regular expression
                }
            }

            Path = $"{Path} ==> {string.Join("\\", newPathSegs)}";
        }

        string[] split = FileDroid.ToString().Split('-');
        string tempMac = split[split.Length - 1];

        MacAddress = Regex.Replace(tempMac, ".{2}", "$0:");
        MacAddress = MacAddress.Substring(0, MacAddress.Length - 1);

        CreationTime = GetDateTimeOffsetFromGuid(FileDroid);

        if (spsSize > 0)
        {
            Sps = new PropertySheet([.. rawBytes.Skip(rawBytes.Length - spsSize)]);
        }

    }

    public long Checksum { get; }
    public int EntryNumber { get; }
    public int MRUPosition { get; }
    public Guid FileBirthDroid { get; }
    public Guid FileDroid { get; }
    public string Hostname { get; }
    public DateTimeOffset LastModified { get; }
    public string Path { get; }
    public int PinStatus { get; }
    public int Unknown0 { get; }
    public float AccessCount { get; }
    public int InteractionCount { get; }
    public int Unknown3 { get; }
    public int Unknown4 { get; }
    public Guid VolumeBirthDroid { get; }
    public Guid VolumeDroid { get; }

    public PropertySheet? Sps { get; }

    public DateTimeOffset CreationTime { get; }
    public string MacAddress { get; }

    private static DateTimeOffset GetDateTimeOffsetFromGuid(Guid guid)
    {
        // offset to move from 1/1/0001, which is 0-time for .NET, to gregorian 0-time of 10/15/1582
        DateTimeOffset gregorianCalendarStart = new(1582, 10, 15, 0, 0, 0, TimeSpan.Zero);
        const int versionByte = 7;
        const int versionByteMask = 0x0f;
        const byte timestampByte = 0;

        byte[] bytes = guid.ToByteArray();

        // reverse the version
        bytes[versionByte] &= versionByteMask;

        byte[] timestampBytes = new byte[8];
        Array.Copy(bytes, timestampByte, timestampBytes, 0, 8);

        long timestamp = BitConverter.ToInt64(timestampBytes, 0);
        long ticks = timestamp + gregorianCalendarStart.Ticks;

        return new DateTimeOffset(ticks, TimeSpan.Zero);
    }
}
