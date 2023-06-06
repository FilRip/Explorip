using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Windows;

using ExploripCopy.Exceptions;
using ExploripCopy.GUI;
using ExploripCopy.Models;

namespace ExploripCopy.Helpers
{
    internal static class CopyHelper
    {
        public static bool Pause { get; set; }
        public static EChoiceFileOperation ChoiceOnCollision { get; set; }

        internal delegate void CallbackRefreshProgress(string currentFile, long fullSize, long remainingSize, int speed);

        internal static Exception CopyDirectory(string sourceDir, string destinationDir, int bufferSize = 10485760, int refreshFrequency = 1000, CallbackRefreshProgress CallbackRefresh = null, bool renameOnCollision = false)
        {
            Exception result;
            string destDir = destinationDir;
            if (renameOnCollision)
            {
                DirectoryInfo dirInfo = new(destinationDir + Path.DirectorySeparatorChar + Path.GetFileName(sourceDir));
                int nbCopy = 0;
                while (dirInfo.Exists)
                {
                    dirInfo = new DirectoryInfo(destinationDir + Path.DirectorySeparatorChar + Constants.Localization.COPY_OF.Replace("%s", Path.GetFileName(sourceDir)) + (nbCopy > 0 ? $" ({nbCopy})" : ""));
                    nbCopy++;
                }
                destDir = dirInfo.FullName;
            }
            foreach (string file in Directory.GetFiles(sourceDir))
            {
                result = CopyFile(file, destDir, bufferSize, refreshFrequency, CallbackRefresh);
                if (result != null)
                    return result;
            }
            foreach (string dir in Directory.GetDirectories(sourceDir))
            {
                result = CopyDirectory(dir, destDir + Path.DirectorySeparatorChar + Path.GetFileName(dir), bufferSize, refreshFrequency, CallbackRefresh);
                if (result != null)
                    return result;
            }
            return null;
        }

        internal static Exception MoveDirectory(string sourceDir, string destinationDir, int bufferSize = 10485760, int refreshFrequency = 1000, CallbackRefreshProgress CallbackRefresh = null)
        {
            Exception result;
            DirectoryInfo destDir = new(destinationDir + Path.DirectorySeparatorChar + Path.GetFileName(sourceDir));
            if (!destDir.Exists)
                destDir.Create();
            foreach (string file in Directory.GetFiles(sourceDir))
            {
                result = MoveFile(file, destDir.FullName, bufferSize, refreshFrequency, CallbackRefresh);
                if (result != null)
                    return result;
            }
            foreach (string dir in Directory.GetDirectories(sourceDir))
            {
                result = MoveDirectory(dir, destDir.FullName, bufferSize, refreshFrequency, CallbackRefresh);
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

        internal static Exception MoveFile(string sourceFile, string destinationDir, int bufferSize = 10485760, int refreshFrequency = 1000, CallbackRefreshProgress CallbackRefresh = null)
        {
            Exception result;
            if ((result = CopyFile(sourceFile, destinationDir, bufferSize, refreshFrequency, CallbackRefresh)) == null)
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

        internal static Exception CopyFile(string sourceFile, string destinationDir, int bufferSize = 10485760, int refreshFrequency = 1000, CallbackRefreshProgress CallbackRefresh = null, bool renameOnCollision = false)
        {
            try
            {
                byte[] buffer = new byte[bufferSize];
                FileInfo fi = new(sourceFile);
                FileInfo destFile = new(destinationDir + Path.DirectorySeparatorChar + Path.GetFileName(sourceFile));
                int nbCopy = 0;
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
                                reset = window.MyDataContext.DoSameForAllFiles;
                            });
                        }
                        switch (ChoiceOnCollision)
                        {
                            case EChoiceFileOperation.None:
                                return new ExploripCopyException("Canceled by user");
                            case EChoiceFileOperation.KeepExisting:
                                if (reset)
                                    ChoiceOnCollision = EChoiceFileOperation.None;
                                return null;
                            case EChoiceFileOperation.KeepMostRecent:
                                if (fi.Length > destFile.Length)
                                {
                                    if (reset)
                                        ChoiceOnCollision = EChoiceFileOperation.None;
                                    return null;
                                }
                                DateTime hdSrc = fi.LastWriteTimeUtc;
                                DateTime hdDest = destFile.LastWriteTimeUtc;
                                if (hdSrc.CompareTo(hdDest) > 0)
                                {
                                    if (reset)
                                        ChoiceOnCollision = EChoiceFileOperation.None;
                                    return null;
                                }
                                break;
                        }
                        if (reset)
                            ChoiceOnCollision = EChoiceFileOperation.None;
                    }
                }
                long fullSize = fi.Length;
                long remaining = fullSize;
                FileStream source = new(sourceFile, FileMode.Open, FileAccess.Read);
                if (!Directory.Exists(Path.GetFullPath(destinationDir)))
                    Directory.CreateDirectory(Path.GetFullPath(destinationDir));
                FileStream destination = new(destFile.FullName, FileMode.Create, FileAccess.Write);
                destination.SetLength(fullSize);
                int nbOctets = 1;
                int derniereVitesse = 0;
                Stopwatch stopwatch = new();
                while (source.CanRead && nbOctets > 0)
                {
                    if (Pause)
                        Thread.Sleep(100);
                    else
                    {
                        if (!stopwatch.IsRunning)
                            stopwatch.Restart();
                        nbOctets = source.Read(buffer, 0, buffer.Length);
                        if (nbOctets > 0)
                        {
                            destination.Write(buffer, 0, nbOctets);
                            remaining -= nbOctets;
                            derniereVitesse += nbOctets;
                            CallbackRefresh?.BeginInvoke(sourceFile, fullSize, remaining, derniereVitesse, new AsyncCallback(EndReportProgress), null);
                            if (stopwatch.ElapsedMilliseconds > refreshFrequency)
                            {
                                CallbackRefresh?.BeginInvoke(sourceFile, fullSize, remaining, derniereVitesse, new AsyncCallback(EndReportProgress), null);
                                derniereVitesse = 0;
                                stopwatch.Stop();
                            }
                        }
                    }
                }
                source.Close();
                destination.Close();
            }
            catch (Exception ex)
            {
                return ex;
            }
            return null;
        }

        private static void EndReportProgress(IAsyncResult ar)
        {
            try
            {
                AsyncResult result = (AsyncResult)ar;
                CallbackRefreshProgress caller = (CallbackRefreshProgress)result.AsyncDelegate;
                caller.EndInvoke(ar);
            }
            catch (Exception) { /* Ignore errors */ }
        }
    }
}
