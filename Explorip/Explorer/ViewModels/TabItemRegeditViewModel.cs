using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Explorip.Helpers;

namespace Explorip.Explorer.ViewModels;

#nullable enable

public partial class TabItemRegeditViewModel : TabItemExploripViewModel, IDisposable
{
    #region Fields

    private readonly List<OneRegistryKey> _historic;
    private int _currentNavigation;
    private bool disposedValue;

    #endregion

    #region Property fields

    [ObservableProperty()]
    private bool _modeEdit;
    [ObservableProperty()]
    private bool _modeSearch;
    [ObservableProperty()]
    private string? _searchTo;
    [ObservableProperty()]
    private string? _editPath;
    [ObservableProperty()]
    private string[]? _comboBoxEditPath;
    [ObservableProperty()]
    private bool _showSuggestions;
    [ObservableProperty()]
    private string? _errorMessage;
    [ObservableProperty()]
    private bool _errorVisible;
    [ObservableProperty()]
    private OneRegistryKey? _currentSelectedKey;
    [ObservableProperty()]
    private List<OneRegistryValue> _listViewItems;
    [ObservableProperty()]
    private List<OneRegistryKey> _regKeyItems;
    [ObservableProperty()]
    private bool _showModifyValue = false;
    [ObservableProperty()]
    private string _newValue = "";

    #endregion

    #region Constructors

    public TabItemRegeditViewModel() : base()
    {
        _historic = [];
        _currentNavigation = 0;
        _listViewItems = [];
        _regKeyItems = [];
        CreateBaseLocalRegistry();
    }

    #endregion

    #region Properties

    public bool AllowNavigatePrevious
    {
        get { return _currentNavigation > 0; }
    }

    public bool AllowNavigateNext
    {
        get { return _currentNavigation < _historic.Count - 1; }
    }

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
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static
    {
        get { return Constants.Localization.REGISTRY_EDITOR; }
    }

    #endregion

    #region RelayCommand

    #region Navigation

    [RelayCommand()]
    public void GotoPrevious()
    {
        if (AllowNavigatePrevious)
        {
            _currentNavigation--;
            BrowseTo(_historic[_currentNavigation], false);
            RefreshNavigation();
        }
    }

    [RelayCommand()]
    public void GotoNext()
    {
        if (AllowNavigateNext)
        {
            _currentNavigation++;
            BrowseTo(_historic[_currentNavigation], false);
            RefreshNavigation();
        }
    }

    [RelayCommand()]
    public void CleanHistoric()
    {
        _currentNavigation = 0;
        _historic.Clear();
        _historic.Add(CurrentSelectedKey!);
        RefreshNavigation();
    }

    #endregion

    #region Edit path manually

    [RelayCommand()]
    public void EditPath_KeyDown(KeyEventArgs e)
    {
        // TODO
    }

    [RelayCommand()]
    public void EditPath_PreviewMouseDown(KeyEventArgs e)
    {
        // TODO
    }

    [RelayCommand()]
    public void EditPath_LostFocus()
    {
        // TODO
    }

    #endregion

    #region Search in registry

    [RelayCommand()]
    public void SearchText_KeyDown(KeyEventArgs e)
    {
        // TODO
    }

    [RelayCommand()]
    public void SearchButton_Click()
    {
        // TODO
    }

    #endregion

    #endregion

    private void BrowseTo(OneRegistryKey registryKey, bool addToHistoric = true)
    {
        // TODO
        if (addToHistoric)
            _historic.Add(registryKey);
        _currentNavigation++;
        RefreshNavigation();
    }

    public void RefreshNavigation()
    {
        OnPropertyChanged(nameof(AllowNavigateNext));
        OnPropertyChanged(nameof(AllowNavigatePrevious));
    }

    private void CreateBaseLocalRegistry()
    {
        RegKeyItems.Add(new OneRegistryKey(null, null, null, displayText: Environment.SpecialFolder.MyComputer.RealName()) { MainViewModel = this, IsExpanded = true, IsSelected = true });
        CurrentSelectedKey = RegKeyItems[0];
    }

    #region Destructor

    public bool IsDisposed
    {
        get { return disposedValue; }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: supprimer l'état managé (objets managés)
            }
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion
}
