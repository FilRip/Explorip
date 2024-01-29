using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.WindowsAPICodePack.Shell;

namespace Explorip.Desktop.ViewModels;

internal partial class OneDesktopItemViewModel : ObservableObject
{
    internal string FullPath { get; set; }
    internal Environment.SpecialFolder SpecialFolder { get; set; }
    internal bool IsDirectory { get; set; }
    internal ExploripDesktopViewModel CurrentDesktop { get; set; }
    internal FileSystemInfo _fileSystemInfo;

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
        Process.Start(FullPath, args.ToString());
    }

    [RelayCommand()]
    private void SelectIt()
    {
        if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
            CurrentDesktop.UnselectAll();
        IsSelected = !IsSelected;
    }

    internal FileSystemInfo FileSystemIO
    {
        get
        {
            _fileSystemInfo ??= (IsDirectory ? new DirectoryInfo(FullPath) : new FileInfo(FullPath));
            return _fileSystemInfo;
        }
    }
}
