using System;
using System.Diagnostics;
using System.IO;

using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace Explorip.Updater;

internal static class ZipManager
{
    private static readonly int _bufferSize = 4096;
    private static byte[] _buffer;

    /// <summary>
    /// Extract a file storing its contents.
    /// </summary>
    /// <param name="inputStream">The input stream to source file contents from.</param>
    /// <param name="theEntry">The <see cref="ZipEntry"/> representing the stored file details </param>
    /// <param name="targetDir">The directory to store the output.</param>
    /// <returns>True if operation is successful; false otherwise.</returns>
    private static bool ExtractFile(Stream inputStream, ZipEntry theEntry, string targetDir)
    {
        if (inputStream == null)
        {
            throw new ArgumentNullException("inputStream");
        }

        if (theEntry == null)
        {
            throw new ArgumentNullException("theEntry");
        }

        if (!theEntry.IsFile)
        {
            throw new ArgumentException("Not a file", "theEntry");
        }

        if (targetDir == null)
        {
            throw new ArgumentNullException("targetDir");
        }

        // try and sort out the correct place to save this entry

        bool result = true;
        bool process = theEntry.Name.Length > 0;

        if (!process)
        {
            return false;
        }

        string entryFileName;

        if (Path.IsPathRooted(theEntry.Name))
        {
            string workName = Path.GetPathRoot(theEntry.Name);
            workName = theEntry.Name.Substring(workName.Length);
            entryFileName = Path.Combine(Path.GetDirectoryName(workName), Path.GetFileName(theEntry.Name));
        }
        else
        {
            entryFileName = theEntry.Name;
        }

        string targetName = Path.Combine(targetDir, entryFileName);
        string fullDirectoryName = Path.GetDirectoryName(Path.GetFullPath(targetName));

        // Could be an option or parameter to allow failure or try creation
        if (!Directory.Exists(fullDirectoryName))
        {
            try
            {
                Directory.CreateDirectory(fullDirectoryName);
            }
            catch (Exception)
            {
                result = false;
                process = false;
            }
        }

        if (process)
        {
            using (FileStream outputStream = File.Create(targetName))
            {
                StreamUtils.Copy(inputStream, outputStream, GetBuffer());
            }

            File.SetLastWriteTime(targetName, theEntry.DateTime);
        }
        return result;
    }

    private static byte[] GetBuffer()
    {
        _buffer ??= new byte[_bufferSize];

        return _buffer;
    }

    /// <summary>
    /// Decompress a file
    /// </summary>
    /// <param name="fileName">File to decompress</param>
    /// <param name="targetDir">Directory to create output in</param>
    /// <returns>true iff all has been done successfully</returns>
    private static bool DecompressArchive(string fileName, string targetDir)
    {
        bool result = true;

        try
        {
            using ZipFile zf = new(fileName);
            foreach (ZipEntry entry in zf)
            {
                if (entry.IsFile)
                {
                    result = ExtractFile(zf.GetInputStream(entry), entry, targetDir);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Exception decompressing - '{0}'", ex);
            result = false;
        }
        return result;
    }

    /// <summary>
    /// Extract archives based on user input
    /// Allows simple wildcards to specify multiple archives
    /// </summary>
    internal static bool Extract(string destFolder = null, params string[] fileSpecs)
    {
        if (string.IsNullOrEmpty(destFolder))
        {
            destFolder = @".\";
        }

        foreach (string spec in fileSpecs)
        {

            string[] names;
            if (spec.IndexOf('*') >= 0 || spec.IndexOf('?') >= 0)
            {
                string pathName = Path.GetDirectoryName(spec);

                if (string.IsNullOrEmpty(pathName))
                {
                    pathName = @".\";
                }
                names = Directory.GetFiles(pathName, Path.GetFileName(spec));
            }
            else
            {
                names = [spec];
            }

            foreach (string fileName in names)
            {
                if (File.Exists(fileName))
                {
                    if (!DecompressArchive(fileName, destFolder))
                        return false;
                }
                else
                {
                    return false;
                }
            }
        }
        return true;
    }
}
