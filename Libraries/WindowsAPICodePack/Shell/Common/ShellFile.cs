﻿using System.IO;

using Microsoft.WindowsAPICodePack.Shell.Interop.Common;
using Microsoft.WindowsAPICodePack.Shell.Resources;

namespace Microsoft.WindowsAPICodePack.Shell.Common;

/// <summary>
/// A file in the Shell Namespace
/// </summary>
public class ShellFile : ShellObject
{
    #region Internal Constructor

    internal ShellFile(string path)
    {
        // Get the absolute path
        string absPath = ShellHelper.GetAbsolutePath(path);

        // Make sure this is valid
        if (!File.Exists(absPath))
        {
            throw new FileNotFoundException(
                string.Format(System.Globalization.CultureInfo.InvariantCulture,
                LocalizedMessages.FilePathNotExist, path));
        }

        ParsingName = absPath;
    }

    internal ShellFile(IShellItem2 shellItem)
    {
        nativeShellItem = shellItem;
    }

    #endregion

    #region Public Methods
    /// <summary>
    /// Constructs a new ShellFile object given a file path
    /// </summary>
    /// <param name="path">The file or folder path</param>
    /// <returns>ShellFile object created using given file path.</returns>
    static public ShellFile FromFilePath(string path)
    {
        return new ShellFile(path);
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// The path for this file
    /// </summary>
    virtual public string Path
    {
        get { return ParsingName; }
    }

    #endregion
}
