using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting.Messaging;

namespace ExploripCopy.Helpers
{
    internal static class CopyHelper
    {
        internal delegate void CallbackRefreshProgress(long fullSize, long remainingSize, int speed);

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

        internal static Exception CopyFile(string sourceFile, string destinationDir, int bufferSize = 10485760, int refreshFrequency = 1000, CallbackRefreshProgress CallbackRefresh = null)
        {
            try
            {
                byte[] buffer = new byte[bufferSize];
                FileInfo fi = new(sourceFile);
                long fullSize = fi.Length;
                long remaining = fullSize;
                FileStream source = new(sourceFile, FileMode.Open, FileAccess.Read);
                FileStream destination = new(destinationDir + Path.DirectorySeparatorChar + Path.GetFileName(sourceFile), FileMode.Create, FileAccess.Write);
                destination.SetLength(fullSize);
                int nbOctets = 1;
                int derniereVitesse = 0;
                Stopwatch stopwatch = new();
                while (source.CanRead && nbOctets > 0)
                {
                    if (!stopwatch.IsRunning)
                        stopwatch.Restart();
                    nbOctets = source.Read(buffer, 0, buffer.Length);
                    if (nbOctets > 0)
                    {
                        destination.Write(buffer, 0, nbOctets);
                        remaining -= nbOctets;
                        derniereVitesse += nbOctets;
                        CallbackRefresh?.BeginInvoke(fullSize, remaining, derniereVitesse, new AsyncCallback(EndReportProgress), null);
                        if (stopwatch.ElapsedMilliseconds > refreshFrequency)
                        {
                            CallbackRefresh?.BeginInvoke(fullSize, remaining, derniereVitesse, new AsyncCallback(EndReportProgress), null);
                            derniereVitesse = 0;
                            stopwatch.Stop();
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
