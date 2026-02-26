using System;
using System.IO;
using System.Windows;

using ExploripCopy.Exceptions;
using ExploripCopy.GUI;
using ExploripCopy.Models;

namespace ExploripCopy.Helpers;

internal static class MoveSameDriveHelper
{
    internal static Exception MoveDirectory(string source, string destination, CopyHelper.CallbackRefreshProgress CallbackRefresh)
    {
        if (!Directory.Exists(destination))
        {
            Directory.Move(source, destination);
            return null;
        }

        foreach (string dir in Directory.GetDirectories(source))
        {
            string path = Path.Combine(destination, Path.GetFileName(dir));
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            Exception ex = MoveDirectory(Path.Combine(source, Path.GetFileName(dir)), path, CallbackRefresh);
            if (ex != null)
                return ex;
        }
        foreach (string file in Directory.GetFiles(source))
        {
            Exception ex = MoveFile(file, Path.Combine(destination, Path.GetFileName(file)), CallbackRefresh);
            if (ex != null)
                return ex;
        }
        Directory.Delete(source, true);
        return null;
    }

    internal static Exception MoveFile(string source, string destination, CopyHelper.CallbackRefreshProgress CallbackRefresh)
    {
        FileInfo fi = new(source);
        long fileSize = fi.Length;
        if (File.Exists(destination))
        {
            bool reset = false;
            if (CopyHelper.ChoiceOnCollision == EChoiceFileOperation.None)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ChoiceConflictFiles window = new()
                    {
                        Owner = MainWindow.Instance,
                    };
                    window.DataContext.ConflictFile = Path.GetFileName(destination);
                    window.ShowDialog();
                    CopyHelper.ChoiceOnCollision = window.DataContext.Choice;
                    reset = !window.DataContext.DoSameForAllFiles;
                });
            }
            switch (CopyHelper.ChoiceOnCollision)
            {
                case EChoiceFileOperation.None:
                    return new ExploripCopyException(Constants.Localization.CANCELED);
                case EChoiceFileOperation.KeepExisting:
                    if (reset)
                        CopyHelper.ChoiceOnCollision = EChoiceFileOperation.None;
                    CallbackRefresh?.BeginInvoke(source, fileSize, 0, fileSize, new AsyncCallback(CopyHelper.EndReportProgress), null);
                    return null;
                case EChoiceFileOperation.KeepMostRecent:
                    FileInfo destFile = new(destination);
                    if (fileSize == destFile.Length)
                    {
                        if (reset)
                            CopyHelper.ChoiceOnCollision = EChoiceFileOperation.None;
                        CallbackRefresh?.BeginInvoke(source, fileSize, 0, fileSize, new AsyncCallback(CopyHelper.EndReportProgress), null);
                        return null;
                    }
                    DateTime hdSrc = fi.LastWriteTimeUtc;
                    DateTime hdDest = destFile.LastWriteTimeUtc;
                    if (hdSrc.CompareTo(hdDest) > 0)
                    {
                        if (reset)
                            CopyHelper.ChoiceOnCollision = EChoiceFileOperation.None;
                        CallbackRefresh?.BeginInvoke(source, fileSize, 0, fileSize, new AsyncCallback(CopyHelper.EndReportProgress), null);
                        return null;
                    }
                    break;
            }
            if (reset)
                CopyHelper.ChoiceOnCollision = EChoiceFileOperation.None;
            File.Delete(destination);
        }
        File.Move(source, destination);
        CallbackRefresh?.Invoke(source, fileSize, 0, fileSize);
        return null;
    }
}
