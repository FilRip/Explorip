﻿using System;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Windows;

using ExploripCopy.Exceptions;
using ExploripCopy.GUI;
using ExploripCopy.Models;

namespace ExploripCopy.Helpers;

internal static class CopyHelper
{
    private const uint BUFFER_SIZE = 1048576; // 1Mo by default

    public static bool Pause { get; set; }
    public static bool Stop { get; set; }
    public static EChoiceFileOperation ChoiceOnCollision { get; set; }

    internal delegate void CallbackRefreshProgress(string currentFile, long fullSize, long remainingSize, long nbBytesRead);

    internal static Exception CopyDirectory(string sourceDir, string destinationDir, long startOffset, uint bufferSize = BUFFER_SIZE, CallbackRefreshProgress CallbackRefresh = null, bool renameOnCollision = false)
    {
        Exception result;
        string destDir;
        DirectoryInfo dirInfo = new(destinationDir + Path.DirectorySeparatorChar + Path.GetFileName(sourceDir));
        if (renameOnCollision)
        {
            int nbCopy = 0;
            while (dirInfo.Exists)
            {
                dirInfo = new DirectoryInfo(destinationDir + Path.DirectorySeparatorChar + Constants.Localization.COPY_OF.Replace("%s", Path.GetFileName(sourceDir)) + (nbCopy > 0 ? $" ({nbCopy})" : ""));
                nbCopy++;
            }
        }
        if (!dirInfo.Exists)
            dirInfo.Create();
        destDir = dirInfo.FullName;
        foreach (string file in Directory.GetFiles(sourceDir))
        {
            result = CopyFile(file, destDir, startOffset, bufferSize, CallbackRefresh);
            if (result != null)
                return result;
        }
        foreach (string dir in Directory.GetDirectories(sourceDir))
        {
            result = CopyDirectory(dir, destDir, startOffset, bufferSize, CallbackRefresh);
            if (result != null)
                return result;
        }
        return null;
    }

    internal static Exception MoveDirectory(string sourceDir, string destinationDir, long startOffset, uint bufferSize = BUFFER_SIZE, CallbackRefreshProgress CallbackRefresh = null)
    {
        Exception result;
        DirectoryInfo destDir = new(destinationDir + Path.DirectorySeparatorChar + Path.GetFileName(sourceDir));
        if (!destDir.Exists)
            destDir.Create();
        foreach (string file in Directory.GetFiles(sourceDir))
        {
            result = MoveFile(file, destDir.FullName, startOffset, bufferSize, CallbackRefresh);
            if (result != null)
                return result;
        }
        foreach (string dir in Directory.GetDirectories(sourceDir))
        {
            result = MoveDirectory(dir, destDir.FullName, startOffset, bufferSize, CallbackRefresh);
            if (result != null)
                return result;
        }
        try
        {
            Directory.Delete(sourceDir);
        }
        catch (Exception ex)
        {
            return ex;
        }

        return null;
    }

    internal static Exception MoveFile(string sourceFile, string destinationDir, long startOffset, uint bufferSize = BUFFER_SIZE, CallbackRefreshProgress CallbackRefresh = null)
    {
        Exception result;
        if ((result = CopyFile(sourceFile, destinationDir, startOffset, bufferSize, CallbackRefresh)) == null)
        {
            try
            {
                File.Delete(sourceFile);
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
        else
            return result;
        return null;
    }

    internal static Exception CopyFile(string sourceFile, string destinationDir, long startOffset, uint bufferSize = BUFFER_SIZE, CallbackRefreshProgress CallbackRefresh = null, bool renameOnCollision = false)
    {
        try
        {
            byte[] buffer = new byte[bufferSize];
            FileInfo fi = new(sourceFile);
            FileInfo destFile = new(destinationDir + Path.DirectorySeparatorChar + Path.GetFileName(sourceFile));
            int nbCopy = 0;
            long fullSize = fi.Length;
            if (bufferSize > fullSize)
            {
                buffer = new byte[fullSize];
            }
            long remaining = fullSize;
            if (destFile.Exists)
            {
                if (renameOnCollision)
                {
                    while (destFile.Exists)
                    {
                        destFile = new(destinationDir + Path.DirectorySeparatorChar + Constants.Localization.COPY_OF.Replace("%s", Path.GetFileNameWithoutExtension(sourceFile)) + (nbCopy > 0 ? $" ({nbCopy})" : "") + Path.GetExtension(sourceFile));
                        nbCopy++;
                    }
                }
                else
                {
                    bool reset = false;
                    if (ChoiceOnCollision == EChoiceFileOperation.None)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            ChoiceConflictFiles window = new()
                            {
                                Owner = MainWindow.Instance,
                            };
                            window.MyDataContext.ConflictFile = Path.GetFileName(destFile.FullName);
                            window.ShowDialog();
                            ChoiceOnCollision = window.MyDataContext.Choice;
                            reset = !window.MyDataContext.DoSameForAllFiles;
                        });
                    }
                    switch (ChoiceOnCollision)
                    {
                        case EChoiceFileOperation.None:
                            return new ExploripCopyException(Constants.Localization.CANCELED);
                        case EChoiceFileOperation.KeepExisting:
                            if (reset)
                                ChoiceOnCollision = EChoiceFileOperation.None;
                            CallbackRefresh?.BeginInvoke(sourceFile, fullSize, 0, fullSize, new AsyncCallback(EndReportProgress), null);
                            return null;
                        case EChoiceFileOperation.KeepMostRecent:
                            if (fi.Length == destFile.Length)
                            {
                                if (reset)
                                    ChoiceOnCollision = EChoiceFileOperation.None;
                                CallbackRefresh?.BeginInvoke(sourceFile, fullSize, 0, fullSize, new AsyncCallback(EndReportProgress), null);
                                return null;
                            }
                            DateTime hdSrc = fi.LastWriteTimeUtc;
                            DateTime hdDest = destFile.LastWriteTimeUtc;
                            if (hdSrc.CompareTo(hdDest) > 0)
                            {
                                if (reset)
                                    ChoiceOnCollision = EChoiceFileOperation.None;
                                CallbackRefresh?.BeginInvoke(sourceFile, fullSize, 0, fullSize, new AsyncCallback(EndReportProgress), null);
                                return null;
                            }
                            break;
                    }
                    if (reset)
                        ChoiceOnCollision = EChoiceFileOperation.None;
                }
            }
            FileStream source = new(sourceFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
            {
                Position = startOffset,
            };
            if (!Directory.Exists(Path.GetFullPath(destinationDir)))
                Directory.CreateDirectory(Path.GetFullPath(destinationDir));
            DriveInfo di = new DirectoryInfo(destinationDir).GetDrive();
            if (di != null && di.AvailableFreeSpace < fullSize)
                throw new ExploripCopyException(Constants.Localization.NOT_ENOUGH_FREE_SPACE);
            FileStream destination = new(destFile.FullName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            destination.SetLength(fullSize);
            int nbBytes = 1;
            while (source.CanRead && nbBytes > 0)
            {
                if (Pause)
                    Thread.Sleep(10);
                else if (Stop)
                    throw new ExploripCopyException(Constants.Localization.STOP);
                else
                {
                    if (buffer.Length > remaining && remaining < bufferSize)
                        buffer = new byte[remaining];
                    nbBytes = source.Read(buffer, 0, buffer.Length);
                    if (nbBytes > 0)
                    {
                        destination.Write(buffer, 0, nbBytes);
                        remaining -= nbBytes;
                        CallbackRefresh?.BeginInvoke(sourceFile, fullSize, remaining, nbBytes, new AsyncCallback(EndReportProgress), null);
                    }
                }
            }
            CallbackRefresh?.Invoke(sourceFile, fullSize, 0, nbBytes);
            source.Close();
            destination.Close();
            destFile.CreationTimeUtc = fi.CreationTimeUtc;
            destFile.CreationTime = fi.CreationTime;
            destFile.LastWriteTimeUtc = fi.LastWriteTimeUtc;
            destFile.LastWriteTime = fi.LastWriteTime;
        }
        catch (Exception ex)
        {
            return ex;
        }
        return null;
    }

    public static void EndReportProgress(IAsyncResult ar)
    {
        try
        {
            AsyncResult result = (AsyncResult)ar;
            CallbackRefreshProgress caller = (CallbackRefreshProgress)result.AsyncDelegate;
            caller.EndInvoke(ar);
        }
        catch (Exception) { /* Ignore errors */ }
    }

    internal static long TotalSizeDirectory(string dir)
    {
        long result = 0;

        foreach (string file in Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories))
        {
            FileInfo fi = new(file);
            result += fi.Length;
        }

        return result;
    }

    internal static int TotalNbFiles(string dir)
    {
        return Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories).Length;
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
