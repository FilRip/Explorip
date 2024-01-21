using System;
using System.Diagnostics;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.WindowsAPICodePack.Shell;

namespace Explorip.Desktop.Models;

internal partial class OneDesktopShellItem : ObservableObject
{
    internal string FullPath { get; set; }
    internal Environment.SpecialFolder SpecialFolder { get; set; }

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
            if (IsSelected)
                return new SolidColorBrush(Color.FromArgb(255, System.Drawing.Color.Gray.R, System.Drawing.Color.Gray.G, System.Drawing.Color.Gray.B));
            return new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
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
    private void Execute()
    {
        Process.Start(FullPath);
    }
}
