using System;
using System.Collections.Generic;
using System.IO;

namespace CoolBytes.JumpList.Custom;

public class CustomDestination
{
    private readonly byte[] footerBytes = [0xAB, 0xFB, 0xBF, 0xBA];

    public CustomDestination(string sourceFile)
    {
        SourceFile = sourceFile;

        byte[] rawBytes = File.ReadAllBytes(sourceFile);
        if (rawBytes.Length <= 24)
        {
            throw new ExtensionsJumpListException("Empty custom destinations jump list");
        }

        int footerSig = BitConverter.ToInt32(footerBytes, 0);
        int fileSig = BitConverter.ToInt32(rawBytes, rawBytes.Length - 4);

        if (footerSig != fileSig)
        {
            throw new ExtensionsJumpListException("Invalid signature (footer missing)");
        }

        Entries = [];

        int index = 0;

        //first, check for footer offsets. some files have more than one

        List<int> footerOffsets = [];

        while (index < rawBytes.Length)
        {
            int lo = ByteSearch(rawBytes, footerBytes, index);

            if (lo == -1)
            {
                break;
            }

            footerOffsets.Add(lo);

            index = lo + footerBytes.Length; //add length so we do not hit on it again
        }

        List<byte[]> byteChunks = [];

        List<int> absOffsets = [];

        int chunkStart = 0;
        foreach (int footerOffset in footerOffsets)
        {
            int chunkSize = footerOffset - chunkStart + 4;
            byte[] bytes = new byte[chunkSize];

            Buffer.BlockCopy(rawBytes, chunkStart, bytes, 0, bytes.Length);

            absOffsets.Add(chunkStart);

            byteChunks.Add(bytes);

            chunkStart += chunkSize;
        }

        int counter = 0;
        foreach (byte[] byteChunk in byteChunks)
        {
            if (byteChunk.Length > 30)
            {
                Entry e = new(byteChunk, absOffsets[counter]);

                Entries.Add(e);
                counter += 1;
            }
        }
    }

    public string SourceFile { get; internal set; }

    public DateTime HdLastModified { get; internal set; }

    public List<Entry> Entries { get; }

    public static int ByteSearch(byte[] searchIn, byte[] searchBytes, int start = 0)
    {
        int found = -1;
        bool matched;
        //only look at this if we have a populated search array and search bytes with a sensible start
        if (searchIn.Length > 0 && searchBytes.Length > 0 && start <= searchIn.Length - searchBytes.Length &&
            searchIn.Length >= searchBytes.Length)
        {
            //iterate through the array to be searched
            for (int i = start; i <= searchIn.Length - searchBytes.Length; i++)
            {
                //if the start bytes match we will start comparing all other bytes
                if (searchIn[i] == searchBytes[0])
                {
                    if (searchIn.Length > 1)
                    {
                        //multiple bytes to be searched we have to compare byte by byte
                        matched = true;
                        for (int y = 1; y <= searchBytes.Length - 1; y++)
                        {
                            if (searchIn[i + y] != searchBytes[y])
                            {
                                matched = false;
                                break;
                            }
                        }
                        //everything matched up
                        if (matched)
                        {
                            found = i;
                            break;
                        }
                    }
                    else
                    {
                        //search byte is only one bit nothing else to do
                        found = i;
                        break; //stop the loop
                    }
                }
            }
        }
        return found;
    }
}