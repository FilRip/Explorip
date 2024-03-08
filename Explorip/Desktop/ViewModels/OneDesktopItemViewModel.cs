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

using Microsoft.WindowsAPICodePack.Shell;

using Securify.ShellLink;

namespace Explorip.Desktop.ViewModels;

internal partial class OneDesktopItemViewModel : ObservableObject
{
    internal string FullPath { get; set; }
    internal bool SpecialFolder { get; set; }
    internal bool IsDirectory { get; set; }
    internal ExploripDesktopViewModel CurrentDesktop { get; set; }
    private FileSystemInfo _fileSystemInfo;
    private DateTime _lastClicked = DateTime.UtcNow.AddSeconds(-2);

    [ObservableProperty()]
    private string _name;

    [ObservableProperty()]
    private ImageSource _icon;

    [ObservableProperty(), NotifyPropertyChangedFor(nameof(BackgroundBrush))]
    private bool _isSelected;

    public SolidColorBrush BackgroundBrush
    {
        get
        {
            return (IsSelected ? Constants.Colors.SelectedBackgroundShellObject : Constants.Colors.TransparentColorBrush);
        }
    }

    private ShellObject _shellObject;
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
            Background = Constants.Colors.BackgroundColorBrush,
            Foreground = Constants.Colors.ForegroundColorBrush,
        };
        input.SetOk(Constants.Localization.CONTINUE, Constants.Icons.OkImage);
        input.SetCancel(Constants.Localization.CANCEL, Constants.Icons.CancelImage);
        if (input.ShowModal(IsDirectory ? Constants.Localization.RENAME_FOLDER : Constants.Localization.RENAME_FILE, Constants.Localization.NEW_NAME, Name) == true)
        {
            try
            {
                string newName = Path.Combine(Path.GetDirectoryName(FullPath), input.TxtUserEdit.Text);
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

            IntPtr hIcon = IconHelper.GetIconByFilename(FullPath, ManagedShell.Common.Enums.IconSize.ExtraLarge);
            if (hIcon != IntPtr.Zero)
            {
                System.Drawing.Icon icon = System.Drawing.Icon.FromHandle(hIcon);
                Icon = IconManager.Convert(icon);
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
}
