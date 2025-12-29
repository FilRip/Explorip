using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using VirtualDesktop.Interop;
using VirtualDesktop.Interop.Proxy;
using VirtualDesktop.Utils;

namespace VirtualDesktop.Models;

/// <summary>
/// Encapsulates Windows 11 (and Windows 10) virtual desktops.
/// </summary>
[DebuggerDisplay("{Name} ({Id})")]
public class VirtualDesktop
{
    #region Fields

    internal readonly IVirtualDesktop _source;
    internal string _name;
    private int _num;
    internal string _wallpaperPath;

    #endregion

    #region Constructors

    internal VirtualDesktop(IVirtualDesktop source)
    {
        _source = source;
        _name = source.GetName();
        NumDesktop = VirtualDesktopManager.GetNextNum();
        _wallpaperPath = source.GetWallpaperPath();
        Id = source.GetID();
    }

    #endregion

    #region Properties

    public int NumDesktop
    {
        get { return _num; }
        private set
        {
            _num = value;
        }
    }

    /// <summary>
    /// Gets the unique identifier for this virtual desktop.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Gets or sets the name of this virtual desktop.
    /// </summary>
    /// <remarks>
    /// This is not supported on Windows 10.
    /// </remarks>
    public string Name
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_name))
                return VirtualDesktopManager.GetCurrentConfiguration.NoNamedVirtualDesktop.Replace("%d", NumDesktop.ToString());
            return _name;
        }
        set
        {
            VirtualDesktopManager.GetProvider.VirtualDesktopManagerInternal.SetDesktopName(_source, value);
            _name = value;
        }
    }

    /// <summary>
    /// Gets or sets the path of the desktop wallpaper.
    /// </summary>
    /// <remarks>
    /// This is not supported on Windows 10.
    /// </remarks>
    public string WallpaperPath
    {
        get => _wallpaperPath;
        set
        {
            VirtualDesktopManager.GetProvider.VirtualDesktopManagerInternal.SetDesktopWallpaper(_source, value);
            _wallpaperPath = value;
        }
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Switches to this virtual desktop.
    /// </summary>
    public void Switch()
    {
        VirtualDesktopManager.GetProvider.VirtualDesktopManagerInternal.SwitchDesktop(_source);
    }

    /// <summary>
    /// Removes this virtual desktop and switches to an available one.
    /// </summary>
    /// <remarks>If this is the last virtual desktop, a new one will be created to switch to.</remarks>
    public void Remove()
        => Remove(GetRight() ?? GetLeft() ?? VirtualDesktopManager.Create());

    /// <summary>
    /// Removes this virtual desktop and switches to <paramref name="fallbackDesktop" />.
    /// </summary>
    /// <param name="fallbackDesktop">A virtual desktop to be displayed after the virtual desktop is removed.</param>
    public void Remove(VirtualDesktop fallbackDesktop)
    {
        if (fallbackDesktop == null)
            throw new ArgumentNullException(nameof(fallbackDesktop));

        VirtualDesktopManager.GetProvider.VirtualDesktopManagerInternal.RemoveDesktop(_source, fallbackDesktop._source);
    }

    /// <summary>
    /// Returns the adjacent virtual desktop on the left, or null if there isn't one.
    /// </summary>
    public VirtualDesktop? GetLeft()
    {
        return SafeInvokeHelper.SafeInvoke(
            () => VirtualDesktopManager.GetProvider.VirtualDesktopManagerInternal
                .GetAdjacentDesktop(_source, AdjacentDesktop.LeftDirection)
                .ToVirtualDesktop(),
            HResult.TYPE_E_OUTOFBOUNDS);
    }

    /// <summary>
    /// Returns the adjacent virtual desktop on the right, or null if there isn't one.
    /// </summary>
    public VirtualDesktop? GetRight()
    {
        return SafeInvokeHelper.SafeInvoke(
            () => VirtualDesktopManager.GetProvider.VirtualDesktopManagerInternal
                .GetAdjacentDesktop(_source, AdjacentDesktop.RightDirection)
                .ToVirtualDesktop(),
            HResult.TYPE_E_OUTOFBOUNDS);
    }

    public override string ToString()
        => $"VirtualDesktop {Id} '{_name}'";

    /// <summary>
    /// Move the given desktop to another posision.
    /// </summary>
    public void Move(int index)
    {
        if (index < 0)
            throw new VirtualDesktopException($"Invalid index: {index}");
        if (index >= VirtualDesktopManager.GetDesktops().Count())
            index = VirtualDesktopManager.GetDesktops().Count() - 1;
        VirtualDesktopManager.GetProvider.VirtualDesktopManagerInternal.MoveDesktop(_source, index);
    }

    #endregion
}
