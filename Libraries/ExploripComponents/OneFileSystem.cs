using System.IO;
using System.Threading.Tasks;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;

namespace ExploripComponents;

public abstract partial class OneFileSystem(string fullPath) : ObservableObject
{
    [ObservableProperty()]
    private bool _isSelected;
    [ObservableProperty()]
    private SolidColorBrush _foreground;
    [ObservableProperty()]
    private SolidColorBrush _background;
    [ObservableProperty()]
    private string _fullPath = fullPath;
    protected long _lastSize;

    public string Text
    {
        get { return Path.GetFileName(FullPath); }
    }

    partial void OnIsSelectedChanged(bool value)
    {
        if (value)
            Task.Run(RefreshListView);
    }

    protected virtual void RefreshListView()
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

    public virtual long Size
    {
        get
        {
            if (_lastSize == 0)
                _lastSize = new FileInfo(FullPath).Length;
            return _lastSize;
        }
    }
}
