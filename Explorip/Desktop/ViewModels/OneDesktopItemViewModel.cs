using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Explorip.Helpers;

using ExploripSharedCopy.Controls;

using ManagedShell.Common.Helpers;

using Microsoft.WindowsAPICodePack.Shell.Common;

namespace Explorip.Desktop.ViewModels;

internal partial class OneDesktopItemViewModel : ObservableObject, IDisposable
{
    internal string FullPath { get; set; }
    internal bool SpecialFolder { get; set; }
    internal bool IsDirectory { get; set; }
    internal ExploripDesktopViewModel CurrentDesktop { get; set; }
    private FileSystemInfo _fileSystemInfo;
    private DateTime _lastClicked = DateTime.UtcNow.AddSeconds(-2);

    [ObservableProperty()]
    private string _name;

    [ObservableProperty(), NotifyPropertyChangedFor(nameof(IconSize))]
    private ImageSource _icon;
    [ObservableProperty()]
    private ImageSource _overlayIcon;

    [ObservableProperty(), NotifyPropertyChangedFor(nameof(BackgroundBrush))]
    private bool _isSelected;

    public SolidColorBrush BackgroundBrush
    {
        get
        {
            return (IsSelected ? ExploripSharedCopy.Constants.Colors.SelectedBackgroundShellObject : ExploripSharedCopy.Constants.Colors.TransparentColorBrush);
        }
    }

    private ShellObject _shellObject;
    private bool disposedValue;

    internal ShellObject CurrentShellObject
    {
        get
        {
            _shellObject ??= ShellObject.FromParsingName(FullPath);
            return _shellObject;
        }
        set
        {
            _shellObject = value;
        }
    }

    public double IconSize
    {
        get
        {
            return 48;
            //return Icon?.Height ?? 0;
        }
    }

    [RelayCommand()]
    private void Execute(object args)
    {
        if (args is not string arg || string.IsNullOrWhiteSpace(arg))
            Process.Start(FullPath);
        else
            Process.Start(FullPath, arg);
    }

    [RelayCommand()]
    private void SelectIt()
    {
        bool previousSelected = IsSelected;
        if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
        {
            CurrentDesktop.UnSelectAll();
            IsSelected = true;
        }
        else
        {
            IsSelected = !previousSelected;
            previousSelected = false;
        }
        if (previousSelected && DateTime.UtcNow.Subtract(_lastClicked).TotalMilliseconds > 1000 && CurrentDesktop.ListSelectedItem().Length == 1)
        {
            Rename();
        }
        _lastClicked = DateTime.UtcNow;
    }

    [RelayCommand()]
    private void Rename()
    {
        InputBoxWindow input = new()
        {
            Icon = Icon,
            Background = ExploripSharedCopy.Constants.Colors.BackgroundColorBrush,
            Foreground = ExploripSharedCopy.Constants.Colors.ForegroundColorBrush,
        };
        input.SetOk(Constants.Localization.CONTINUE, Constants.Icons.OkImage);
        input.SetCancel(Constants.Localization.CANCEL, Constants.Icons.CancelImage);
        if (input.ShowModal(IsDirectory ? Constants.Localization.RENAME_FOLDER : Constants.Localization.RENAME_FILE, Constants.Localization.NEW_NAME, Name) == true)
        {
            try
            {
                string newName = Path.Combine(Path.GetDirectoryName(FullPath), input.UserEdit);
                if (IsDirectory)
                    Directory.Move(FullPath, newName);
                else
                    File.Move(FullPath, newName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(Constants.Localization.ERROR, string.Format(Constants.Localization.ERROR_DURING_RENAME, Name, ex.Message), MessageBoxButton.OK);
            }
        }
    }

    internal FileSystemInfo FileSystemIO
    {
        get
        {
            if (!SpecialFolder)
            {
                _fileSystemInfo ??= (IsDirectory ? new DirectoryInfo(FullPath) : new FileInfo(FullPath));
                return _fileSystemInfo;
            }
            return null;
        }
    }

    internal void GetIcon()
    {
        try
        {
            FileInfo fi = new(FullPath);
            if (fi.Attributes.HasFlag(FileAttributes.Directory))
                IsDirectory = true;

            IntPtr hIcon = IconHelper.GetIconByFilename(FullPath, ManagedShell.Common.Enums.IconSize.ExtraLarge, out IntPtr hOverlay);
            if (hIcon != IntPtr.Zero)
            {
                System.Drawing.Icon icon = System.Drawing.Icon.FromHandle(hIcon);
                Icon = IconManager.Convert(icon);
                if (hOverlay != IntPtr.Zero)
                    OverlayIcon = IconManager.Convert(System.Drawing.Icon.FromHandle(hOverlay));
            }
            if (Icon == null)
            {
                System.Drawing.Icon icon = System.Drawing.Icon.ExtractAssociatedIcon(FullPath);
                if (icon != null)
                    Icon = IconManager.Convert(icon);
            }
        }
        catch (Exception) { /* Ignore errors, can't get icon */ }
    }

    public override string ToString()
    {
        return FullPath;
    }

    #region IDisposable

    internal bool IsDisposed
    {
        get { return disposedValue; }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _shellObject?.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion
}
