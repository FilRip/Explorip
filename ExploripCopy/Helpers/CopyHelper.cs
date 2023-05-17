using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace ExploripCopy.Helpers
{
    internal static class CopyHelper
    {
        internal static Exception MoveFile(string sourceFile, string destinationDir, int bufferSize = 10485760, int refreshFrequency = 1000, Action<long, int> CallbackRefresh = null)
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

        internal static Exception CopyFile(string sourceFile, string destinationDir, int bufferSize = 10485760, int refreshFrequency = 1000, Action<long, int> CallbackRefresh = null)
        {
            try
            {
                byte[] buffer = new byte[bufferSize];
                FileInfo fi = new(sourceFile);
                long taille = fi.Length;
                FileStream source = new(sourceFile, FileMode.Open, FileAccess.Read);
                FileStream destination = new(destinationDir + Path.DirectorySeparatorChar + Path.GetFileName(sourceFile), FileMode.Create, FileAccess.Write);
                destination.SetLength(taille);
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
                        taille -= nbOctets;
                        derniereVitesse += nbOctets;
                        CallbackRefresh(taille, derniereVitesse);
                        if (stopwatch.ElapsedMilliseconds > refreshFrequency)
                        {
                            CallbackRefresh(taille, derniereVitesse);
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
    }
}
