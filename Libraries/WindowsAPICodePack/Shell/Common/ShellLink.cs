﻿using System;

using Microsoft.WindowsAPICodePack.Shell.Interop.Common;

namespace Microsoft.WindowsAPICodePack.Shell.Common;

/// <summary>
/// Represents a link to existing FileSystem or Virtual item.
/// </summary>
public class ShellLink : ShellObject
{
    /// <summary>
    /// Path for this file e.g. c:\Windows\file.txt,
    /// </summary>
    private string _internalPath;

    #region Internal Constructors

    internal ShellLink(IShellItem2 shellItem)
    {
        nativeShellItem = shellItem;
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// The path for this link
    /// </summary>
    public virtual string Path
    {
        get
        {
            if (_internalPath == null && NativeShellItem != null)
            {
                _internalPath = base.ParsingName;
            }
            return _internalPath;
        }
        protected set
        {
            _internalPath = value;
        }
    }

    private string internalTargetLocation;
    /// <summary>
    /// Gets the location to which this link points to.
    /// </summary>
    public string TargetLocation
    {
        get
        {
            if (string.IsNullOrEmpty(internalTargetLocation) && NativeShellItem2 != null)
            {
                internalTargetLocation = Properties.System.Link.TargetParsingPath.Value;
            }
            return internalTargetLocation;
        }
        set
        {
            if (value == null) { return; }

            internalTargetLocation = value;

            if (NativeShellItem2 != null)
            {
                Properties.System.Link.TargetParsingPath.Value = internalTargetLocation;
            }
        }
    }

    /// <summary>
    /// Gets the ShellObject to which this link points to.
    /// </summary>
    public ShellObject TargetShellObject
    {
        get { return ShellObjectFactory.Create(TargetLocation); }
    }

    /// <summary>
    /// Gets or sets the link's title
    /// </summary>
    public string Title
    {
        get
        {
            if (NativeShellItem2 != null) { return Properties.System.Title.Value; }
            return null;
        }
        set
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (NativeShellItem2 != null)
            {
                Properties.System.Title.Value = value;
            }
        }
    }

    private string internalArguments;
    /// <summary>
    /// Gets the arguments associated with this link.
    /// </summary>
    public string Arguments
    {
        get
        {
            if (string.IsNullOrEmpty(internalArguments) && NativeShellItem2 != null)
            {
                internalArguments = Properties.System.Link.Arguments.Value;
            }

            return internalArguments;
        }
    }

    private string internalComments;
    /// <summary>
    /// Gets the comments associated with this link.
    /// </summary>
    public string Comments
    {
        get
        {
            if (string.IsNullOrEmpty(internalComments) && NativeShellItem2 != null)
            {
                internalComments = Properties.System.Comment.Value;
            }

            return internalComments;
        }
    }

    #endregion
}
