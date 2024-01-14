using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;

namespace Explorip.Explorer.ViewModels;

public partial class TabItemExplorerBrowserViewModel : TabItemExploripViewModel
{
    public TabItemExplorerBrowserViewModel() : base()
    {
        _lastFolder = "";
    }

    [ObservableProperty()]
    private string _tabTitle;

    [ObservableProperty()]
    private bool _modeEdit;
    partial void OnModeEditChanged(bool value)
    {
        _lastFolder = "";
    }

    [ObservableProperty(), NotifyPropertyChangedFor(nameof(ForegroundPrevious), nameof(ForegroundNext))]
    private bool _allowNavigatePrevious, _allowNavigateNext;

    [ObservableProperty()]
    private string _editPath;
    partial void OnEditPathChanged(string value)
    {
        SearchSubFolder();
    }

    [ObservableProperty()]
    private string[] _comboBoxEditPath;

    public Brush ForegroundPrevious
    {
        get
        {
            if (AllowNavigatePrevious)
                return Constants.Colors.AccentColorBrush;
            else
                return DisabledButtonColor;
        }
    }

    public Brush ForegroundNext
    {
        get
        {
            if (AllowNavigateNext)
                return Constants.Colors.AccentColorBrush;
            else
                return DisabledButtonColor;
        }
    }

    private bool _showSuggestions;
    public bool ShowSuggestions
    {
        get { return _showSuggestions && ModeEdit; }
        set
        {
            _showSuggestions = value;
            OnPropertyChanged();
        }
    }

    private string _lastFolder;
    private void SearchSubFolder()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(EditPath))
                return;
            string currentPath;
            if (EditPath.LastIndexOf(@"\") >= 0)
                currentPath = EditPath.Substring(0, EditPath.LastIndexOf(@"\"));
            else
                currentPath = EditPath;
            if (currentPath != _lastFolder)
            {
                _lastFolder = currentPath;
                Task.Run(() =>
                {
                    if (Directory.Exists(currentPath))
                    {
                        ComboBoxEditPath = Directory.GetDirectories(currentPath);
                        ShowSuggestions = true;
                    }
                });
            }
        }
        catch (Exception) { /* Ignoring errors */ }
    }

    [ObservableProperty()]
    private bool _modeSearch;

    [ObservableProperty()]
    private string _searchTo;
}
