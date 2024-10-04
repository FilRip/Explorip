using System;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;

using ManagedShell.Common.Helpers;

namespace ExploripComponents;

public partial class OneFile(string fullPath) : OneFileSystem(fullPath)
{
    private ImageSource _icon;
    [ObservableProperty()]
    private ImageSource _iconOverlay;

    public ImageSource Icon
    {
        get
        {
            if (_icon == null && !string.IsNullOrWhiteSpace(FullPath))
            {
                IntPtr hIcon = IconHelper.GetIconByFilename(FullPath, ManagedShell.Common.Enums.IconSize.Small, out nint hOverlay);
                _icon = IconImageConverter.GetImageFromHIcon(hIcon);
                if (hOverlay != IntPtr.Zero)
                    IconOverlay = IconImageConverter.GetImageFromHIcon(hOverlay);
            }
            return _icon;
        }
    }
}
