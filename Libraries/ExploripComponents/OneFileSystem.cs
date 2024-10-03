using System.IO;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;

namespace ExploripComponents;

public abstract partial class OneFileSystem : ObservableObject
{
    [ObservableProperty()]
    private bool _isSelected;
    [ObservableProperty()]
    private SolidColorBrush _foreground;
    [ObservableProperty()]
    private SolidColorBrush _background;
    [ObservableProperty()]
    private string _fullPath;

    public string Text
    {
        get { return Path.GetFileName(FullPath); }
    }

    partial void OnIsSelectedChanged(bool value)
    {
        if (value)
            RefreshFiles();
    }

    protected virtual void RefreshFiles()
    {
    }

    public FileAttributes FileAttributes
    {
        get { return File.GetAttributes(FullPath); }
    }

    public bool Hidden
    {
        get { return FileAttributes.HasFlag(FileAttributes.Hidden); }
    }
}
