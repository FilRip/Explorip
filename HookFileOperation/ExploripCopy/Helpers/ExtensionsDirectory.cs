using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

using Explorip.HookFileOperations.Models;

using static ManagedShell.Interop.NativeMethods;

namespace ExploripCopy.Helpers;

public static class ExtensionsDirectory
{
    public static List<OneFileOperation> ExpandDirectory(string path, string dest, EFileOperation operation)
    {
        List<OneFileOperation> list = [];

        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentNullException(nameof(path));

        SafeSearchHandle handle;
        if (!path.EndsWith(Path.DirectorySeparatorChar.ToString()))
            path += Path.DirectorySeparatorChar;

        handle = FindFirstFile(path + "*.*", out Win32FindData dataFind);
        if (!handle.IsInvalid)
        {
            List<OneFileOperation> listFiles = [];
            List<OneFileOperation> listDirectory = [];
            while (true)
            {
                if (dataFind.dwFileAttributes.HasFlag(FileAttributes.Directory))
                {
                    if (dataFind.cFileName != "." && dataFind.cFileName != "..")
                        listDirectory.Add(new OneFileOperation(operation)
                        {
                            Source = Path.Combine(path, dataFind.cFileName),
                            Destination = Path.Combine(dest, Path.GetFileName(path.TrimEnd(Path.DirectorySeparatorChar))),
                            Attributes = dataFind.dwFileAttributes,
                        });
                }
                else
                    listFiles.Add(new OneFileOperation(operation)
                    {
                        Source = Path.Combine(path, dataFind.cFileName),
                        Destination = Path.Combine(dest, Path.GetFileName(path.TrimEnd(Path.DirectorySeparatorChar)), dataFind.cFileName),
                        Attributes = dataFind.dwFileAttributes,
                        Size = dataFind.GetSize(),
                    });

                if (!FindNextFile(handle, out dataFind))
                    break;
            }

            handle.Dispose();

            if (listFiles.Count > 0)
                list.AddRange(listFiles.OrderBy(f => f.Source));
            if (listDirectory.Count > 0)
            {
                foreach (OneFileOperation op in listDirectory.OrderBy(d => d.Source))
                {
                    List<OneFileOperation> listSubDir = ExpandDirectory(op.Source, op.Destination, operation);
                    if (listSubDir.Count > 0)
                        list.AddRange(listSubDir);
                }
            }
        }
        else
            handle.Dispose();

        return list;
    }

    public static ulong DirectorySize(string path, CancellationTokenSource token = null)
    {
        ulong size = 0;
        List<string> subFolders = [];

        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentNullException(nameof(path));

        SafeSearchHandle handle;
        if (!path.EndsWith(Path.DirectorySeparatorChar.ToString()))
            path += Path.DirectorySeparatorChar;

        handle = FindFirstFile(path + "*.*", out Win32FindData dataFind);
        if (!handle.IsInvalid && (token == null || !token.IsCancellationRequested))
        {
            while (true)
            {
                if (dataFind.dwFileAttributes.HasFlag(FileAttributes.Directory))
                {
                    if (dataFind.cFileName != "." && dataFind.cFileName != "..")
                        subFolders.Add(dataFind.cFileName);
                }
                else
                {
                    size += dataFind.GetSize();
                }
                if (!FindNextFile(handle, out dataFind) || token?.IsCancellationRequested == true)
                    break;
            }
        }
        handle.Dispose();

        if (subFolders.Count > 0 && (token == null || !token.IsCancellationRequested))
            foreach (string subFolder in subFolders)
            {
                size += DirectorySize(Path.Combine(path, subFolder), token);
                if (token?.IsCancellationRequested == true)
                    return size;
            }

        return size;
    }

    internal static string SizeInText(double size, string fullText)
    {
        string word = Constants.Localization.SPEED_BYTE;
        double speed = size;

        static void ChangeDim(ref double value, string word, ref string currentWord)
        {
            if (value > 1024)
            {
                value = Math.Round(value / 1024, 2);
                currentWord = word;
            }
        }

        ChangeDim(ref speed, Constants.Localization.SPEED_KILO, ref word);
        ChangeDim(ref speed, Constants.Localization.SPEED_MEGA, ref word);
        ChangeDim(ref speed, Constants.Localization.SPEED_GIGA, ref word);
        ChangeDim(ref speed, Constants.Localization.SPEED_TERA, ref word);
        ChangeDim(ref speed, Constants.Localization.SPEED_PETA, ref word);
        ChangeDim(ref speed, Constants.Localization.SPEED_EXA, ref word);

        return fullText.Replace("%s", $"{speed} {word}");
    }
}
