using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;

namespace Explorip.Explorer.ViewModels;

public partial class TabItemRegeditViewModel : TabItemExploripViewModel
{
    #region Fields

    #region Property fields

    [ObservableProperty(), NotifyPropertyChangedFor(nameof(ForegroundPrevious), nameof(ForegroundNext))]
    private bool _allowNavigatePrevious, _allowNavigateNext;
    [ObservableProperty()]
    private bool _modeEdit;
    [ObservableProperty()]
    private bool _modeSearch;
    [ObservableProperty()]
    private string _searchTo;
    [ObservableProperty()]
    private string _editPath;
    [ObservableProperty()]
    private string[] _comboBoxEditPath;
    [ObservableProperty()]
    private bool _showSuggestions;

    #endregion

    #endregion

    #region Properties

    public Brush ForegroundPrevious
    {
        get
        {
            if (AllowNavigatePrevious)
                return ExploripSharedCopy.Constants.Colors.AccentColorBrush;
            else
                return DisabledButtonColor;
        }
    }

    public Brush ForegroundNext
    {
        get
        {
            if (AllowNavigateNext)
                return ExploripSharedCopy.Constants.Colors.AccentColorBrush;
            else
                return DisabledButtonColor;
        }
    }

#pragma warning disable S2325 // Methods and properties that don't access instance data should be static
    public string TabTitle
    {
        get { return "Regedit"; }
    }
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static

    #endregion
}
